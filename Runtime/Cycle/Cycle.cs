using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;

namespace UnityClock
{
    public interface ICycle
    {
        int loops { get; }
        TimeOnly start { get; }
        TimeOnly duration { get; }
    }

    public interface ICycleSource
    {
        void Create(PlayableGraph graph);
        void Destroy();
        void OnPlay();
        void OnStop();
    }

    public static class CyclePlayableExtensions
    {
        public static void SetCycle<U>(this U playable, ICycle cycle, float length) where U : struct, IPlayable
        {
            var speed = length * cycle.loops;

            var duration = Clock.Day(cycle.duration);
            if (duration > 0)
            {
                playable.SetDuration(duration);
                speed /= duration;
            }
            playable.SetSpeed(speed);

            var start = Clock.Day(cycle.start);
            playable.SetLeadTime(start * speed);
            if (duration > 0)
            {
                playable.GetGraph().GetOrAddCycleDurationBehaviour().AddCycle(playable);
                playable.Pause();
            }
        }

        public static void DestroyCycle<U>(this U playable) where U : struct, IPlayable
        {
            playable.GetGraph().GetOrAddCycleDurationBehaviour().RemoveCycle(playable);
            playable.Destroy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SyncCycle<U>(this U playable) where U : struct, IPlayable
            => playable.SetTime(Clock.Day(Clock.time) * playable.GetSpeed() - playable.GetLeadTime());
    }
}
