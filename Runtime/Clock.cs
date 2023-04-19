#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityClock
{
    public class Clock
    {
        private static TimeOnly _time;

        public const float dayMultiplier = 1f / TimeSpan.TicksPerDay;
        internal const float pingPongMultiplier = dayMultiplier * 2f;
        private static readonly int _UnityClock_Clock_Time = Shader.PropertyToID(nameof(_UnityClock_Clock_Time));
        private static readonly int _UnityClock_Clock_Interpolant = Shader.PropertyToID(nameof(_UnityClock_Clock_Interpolant));
        private static readonly int _UnityClock_Clock_PingPong = Shader.PropertyToID(nameof(_UnityClock_Clock_PingPong));

        /// <summary>
        /// The elapsed time for a day span since the start of the game.
        /// </summary>
        /// <param name="daySpan">The time it takes for the clock to progress 24 hours.</param>
        public static TimeSpan ElapsedTime(TimeSpan daySpan) => !Application.isPlaying || daySpan == TimeSpan.Zero
            ? TimeSpan.Zero
            : TimeSpan.FromSeconds(Time.timeAsDouble / daySpan.TotalDays);
        
        /// <summary>
        /// The current time on the clock.
        /// </summary>
        public static TimeOnly time
        {
            get => _time;
            set
            {
                if (value == _time)
                {
                    return;
                }

                _time = value;
                var ticks = value.Ticks;
                Shader.SetGlobalVector(_UnityClock_Clock_Time, new Vector4(value.Hour, value.Minute, value.Second, value.Millisecond));
                Shader.SetGlobalFloat(_UnityClock_Clock_Interpolant, ticks * dayMultiplier);
                Shader.SetGlobalFloat(_UnityClock_Clock_PingPong, PingPong(ticks));
                timeChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Invoked whenever the time changes.
        /// </summary>
        public static event Action<TimeOnly>? timeChanged;

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
        /// Returns a value between 1 and 0 where 1 is midday and 0 is midnight.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float PingPong(TimeOnly time) => PingPong(time.Ticks);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] internal static float PingPong(long ticks) => 1f - Mathf.Abs(ticks * pingPongMultiplier - 1f);
    }
}