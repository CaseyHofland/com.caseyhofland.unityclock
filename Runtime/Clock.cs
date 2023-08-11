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
        private const float middayMultiplier = dayMultiplier * 2f;
        private static readonly int _UnityClock_Time = Shader.PropertyToID(nameof(_UnityClock_Time));
        private static readonly int _UnityClock_Day = Shader.PropertyToID(nameof(_UnityClock_Day));
        private static readonly int _UnityClock_Midday = Shader.PropertyToID(nameof(_UnityClock_Midday));

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
                _time = value;
                var ticks = value.Ticks;
                {
                    Shader.SetGlobalVector(_UnityClock_Time, new Vector4(value.Hour, value.Minute, value.Second, value.Millisecond));
                    Shader.SetGlobalFloat(_UnityClock_Day, day);
                    Shader.SetGlobalFloat(_UnityClock_Midday, Midday_Internal(ticks));
                }
                timeChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Invoked whenever the time changes.
        /// </summary>
        [Obsolete] public static event Action<TimeOnly>? timeChanged;

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