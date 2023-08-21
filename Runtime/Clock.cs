#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityClock
{
    public static class Clock
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            _time = default;
            _daySpan = default;

            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }
            playableGraph = CreatePlayableGraph();
            CancelSafe();
        }

        private static TimeOnly _time;
        private static TimeSpan _daySpan;

        public const float dayMultiplier = 1f / TimeSpan.TicksPerDay;
        private const float middayMultiplier = dayMultiplier * 2f;
        private static readonly int _UnityClock_Time = Shader.PropertyToID(nameof(_UnityClock_Time));
        private static readonly int _UnityClock_Day = Shader.PropertyToID(nameof(_UnityClock_Day));
        private static readonly int _UnityClock_Midday = Shader.PropertyToID(nameof(_UnityClock_Midday));

        private static CancellationTokenSource? updateCancellationTokenSource;
        
        /// <summary>
        /// The current time on the clock.
        /// </summary>
        public static TimeOnly time
        {
            get => _time;
            set
            {
                var ticks = value.Ticks;
                var day = Day_Internal(ticks);
                var deltaTime = (float)(value - _time).TotalDays; // Always returns positive.

                playableGraph.Evaluate(deltaTime); 

                {
                    Shader.SetGlobalVector(_UnityClock_Time, new Vector4(value.Hour, value.Minute, value.Second, value.Millisecond));
                    Shader.SetGlobalFloat(_UnityClock_Day, day);
                    Shader.SetGlobalFloat(_UnityClock_Midday, Midday_Internal(ticks));
                }

                SetTimeWithoutNotify(value);
            }
        }

        public static TimeSpan daySpan
        {
            get => _daySpan;
            set
            {
                _daySpan = value;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return;
                }
#endif

                if (_daySpan == TimeSpan.Zero)
                {
                    CancelSafe();
                }
                else if (updateCancellationTokenSource == null)
                {
                    UpdateTimeAsync((updateCancellationTokenSource = new()).Token);
                }

                static async void UpdateTimeAsync(CancellationToken cancellationToken = default)
                {
                    try
                    {
                        await Awaitable.NextFrameAsync(cancellationToken);
                        var elapsedTime = TimeSpan.FromSeconds(Time.deltaTime / daySpan.TotalDays);
                        time = time.Add(elapsedTime);
                        UpdateTimeAsync(cancellationToken);
                    }
                    catch (OperationCanceledException) { }
                }
            }
        }

        public static PlayableGraph playableGraph { get; private set; } = CreatePlayableGraph();

        private static PlayableGraph CreatePlayableGraph()
        {
            var playableGraph = PlayableGraph.Create($"{nameof(Clock)}Graph");
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            ScriptPlayableOutput cycleDurationOutput = ScriptPlayableOutput.Create(playableGraph, nameof(cycleDurationOutput));
            var cycleDurationPlayable = ScriptPlayable<CycleDurationBehaviour>.Create(playableGraph);
            cycleDurationOutput.SetSourcePlayable(cycleDurationPlayable);

            return playableGraph;
        }

        private static void CancelSafe()
        {
            updateCancellationTokenSource?.Cancel();
            updateCancellationTokenSource?.Dispose();
            updateCancellationTokenSource = null;
        }

        public static void SetTimeWithoutNotify(TimeOnly time) => _time = time;

        public static TimeOnly Lerp(TimeOnly start, TimeOnly end, float t) => LerpUnclamped(start, end, Mathf.Clamp01(t));
        public static TimeOnly LerpUnclamped(TimeOnly start, TimeOnly end, float t) => start.Add((end - start) * t);

        public static TimeSpan Lerp(TimeSpan start, TimeSpan end, float t) => LerpUnclamped(start, end, Mathf.Clamp01(t));
        public static TimeSpan LerpUnclamped(TimeSpan start, TimeSpan end, float t) => start + (end - start) * t;

        /// <summary>
        /// Determines where time falls between start and end.
        /// </summary>
        /// <returns>A value between 0 and 1, representing where time falls between start and end.</returns>
        public static float InverseLerp(TimeOnly start, TimeOnly end) => InverseLerp(start, end, time);

        /// <summary>
        /// Determines where time falls between start and end.
        /// </summary>
        /// <returns>A value between 0 and 1, representing where time falls between start and end.</returns>
        public static float InverseLerp(TimeOnly start, TimeOnly end, TimeOnly time)
        {
            var startTicks = start.Ticks;
            var endTicks = end.Ticks;
            if (startTicks == endTicks)
            {
                return 0f;
            }

            var timeTicks = time.Ticks;
            if (startTicks > endTicks)
            {
                endTicks += TimeSpan.TicksPerDay;
            }
            if (startTicks > timeTicks)
            {
                timeTicks += TimeSpan.TicksPerDay;
            }

            return Mathf.InverseLerp(startTicks, endTicks, timeTicks);
        }

        public static float InverseLerp(TimeSpan start, TimeSpan end, TimeSpan span) => Mathf.InverseLerp(start.Ticks, end.Ticks, span.Ticks);

        /// <summary>
        /// Returns a value between 1 and 0 (exclusive) where 1 is 24:00:00 (never reached) and 0 is 0:00:00.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Day(TimeOnly time) => Day_Internal(time.Ticks);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] internal static float Day_Internal(long ticks) => ticks * dayMultiplier;

        /// <summary>
        /// Returns a value between 1 and 0 where 1 is midday and 0 is midnight.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Midday(TimeOnly time) => Midday_Internal(time.Ticks);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] internal static float Midday_Internal(long ticks) => 1f - Mathf.Abs(ticks * middayMultiplier - 1f);
    }
}