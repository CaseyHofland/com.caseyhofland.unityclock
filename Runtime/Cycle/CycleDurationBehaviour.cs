using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityClock
{
    internal class CycleDurationBehaviour : PlayableBehaviour
    {
        private record PlayInfo
        {
            public readonly Playable playable;
            public readonly bool play;

            public PlayInfo(Playable playable, bool play)
            {
                this.playable = playable;
                this.play = play;
            }

            public void Trigger(double time)
            {
                if (play)
                {
                    playable.SetTime(time * playable.GetSpeed() - playable.GetLeadTime());
                    playable.Play();
                }
                else
                {
                    playable.SetTime(0d);
                    playable.Pause();
                }
            }
        }

        private record TimedPlayInfo : IComparable<TimedPlayInfo>
        {
            public readonly double time;
            public readonly PlayInfo playInfo;

            public TimedPlayInfo(double time, PlayInfo playInfo)
            {
                this.time = time;
                this.playInfo = playInfo;
            }

            public int CompareTo(TimedPlayInfo other)
            {
                int result;
                return (result = time.CompareTo(other.time)) == 0
                    ? playInfo != other.playInfo
                        ? 1
                        : 0
                    : result;
            }
        }

        private readonly SortedSet<TimedPlayInfo> _timedPlayInfos = new();

        private void GetTimedPlayInfo<U>(in U playable, out TimedPlayInfo startInfo, out TimedPlayInfo endInfo) where U : struct, IPlayable
        {
            if (playable is not Playable castPlayable)
                throw new InvalidCastException("playable is not of type Playable");

            var speed = playable.GetSpeed();
            if (speed < 0)
                throw new ArgumentException("Speed of the Playable must not be 0.", nameof(playable));

            var start = playable.GetLeadTime() / speed;
            var end = start + playable.GetDuration() / speed;

            startInfo = new(start, new(castPlayable, true));
            endInfo = new(end, new(castPlayable, false));
        }

        public void AddCycle<U>(U playable) where U : struct, IPlayable
        {
            GetTimedPlayInfo(playable, out var startInfo, out var endInfo);

            _timedPlayInfos.Add(startInfo);
            _timedPlayInfos.Add(endInfo);
        }

        public void RemoveCycle<U>(U playable) where U : struct, IPlayable
        {
            GetTimedPlayInfo(playable, out var startInfo, out var endInfo);

            _timedPlayInfos.Remove(startInfo);
            _timedPlayInfos.Remove(endInfo);
        }

        /// <summary>
        /// This method is called during the PrepareFrame phase of the PlayableGraph.
        /// </summary>
        /// <remarks>
        /// Called once before processing starts.
        /// </remarks>
        /// <param name="playable">The reference to the playable associated with this PlayableBehaviour.</param>
        /// <param name="info">Playable context information such as weight, evaluationType, and so on.</param>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            var time = playable.GetTime() % 1d;
            var previousTime = playable.GetPreviousTime() % 1d;

            // Fire notifications from previousTime till time.
            if (previousTime > time)
            {
                TriggerCyclesInRange(previousTime, 1d);
                TriggerCyclesInRange(0d, time);
            }
            else
            {
                TriggerCyclesInRange(previousTime, time);
            }
        }
        
        private void TriggerCyclesInRange(double previousTime, double time)
        {
            // TODO: Improve searching using BinarySearch, taking advantage of the fact that the set is sorted.
            foreach (var timedPlayInfo in _timedPlayInfos)
            {
                if (timedPlayInfo.time >= previousTime && timedPlayInfo.time < time)
                {
                    timedPlayInfo.playInfo.Trigger(time);
                }
            }
        }

        // Code taken from https://stackoverflow.com/a/594528/8966605
        private static int BinarySearch<T>(IList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            var comp = Comparer<T>.Default;
            int lo = 0, hi = list.Count - 1;
            while (lo < hi)
            {
                int m = (hi + lo) / 2;  // this might overflow; be careful.
                if (comp.Compare(list[m], value) < 0) lo = m + 1;
                else hi = m - 1;
            }
            if (comp.Compare(list[lo], value) < 0) lo++;
            return lo;
        }
    }

    internal static class CycleDurationBehaviourExtensions
    {
        internal static bool TryGetCycleDurationBehaviour(this PlayableGraph graph, out CycleDurationBehaviour behaviour)
        {
            for (int i = 0; i < graph.GetRootPlayableCount(); i++)
            {
                var playable = graph.GetRootPlayable(i);
                if ((IPlayable)playable is ScriptPlayable<CycleDurationBehaviour> cycleDurationPlayable)
                {
                    behaviour = cycleDurationPlayable.GetBehaviour();
                    return true;
                }
            }

            behaviour = null;
            return false;
        }

        internal static CycleDurationBehaviour GetOrAddCycleDurationBehaviour(this PlayableGraph graph)
        {
            if (graph.TryGetCycleDurationBehaviour(out var behaviour))
            {
                return behaviour;
            }

            ScriptPlayableOutput cycleDurationOutput = ScriptPlayableOutput.Create(graph, nameof(cycleDurationOutput));
            var cycleDurationPlayable = ScriptPlayable<CycleDurationBehaviour>.Create(graph);
            cycleDurationOutput.SetSourcePlayable(cycleDurationPlayable);
            return cycleDurationPlayable.GetBehaviour();
        }
    }
}
