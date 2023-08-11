using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityClock
{
    /// <summary>
    /// A volume component that holds settings for the Clock.
    /// </summary>
    [Serializable, VolumeComponentMenu(nameof(Clock) + " (Experimental!)")]
    public sealed class ClockVolume : VolumeComponent
    {
        public TimeSpanParameter timePassed = new(new TimeSpan(12, 0, 0), true);
        public TimeSpanParameter daySpan = new(default);

        private double lastTimeAsDouble;

        protected override void OnEnable()
        {
            base.OnEnable();

            displayName = nameof(Clock);
        }

        public override void Override(VolumeComponent state, float interpFactor)
        {
            if (Application.IsPlaying(this))
            {
                var deltaTime = Time.timeAsDouble - lastTimeAsDouble;
                if (daySpan.overrideState && daySpan.value != TimeSpan.Zero)
                {
                    var elapsedTime = TimeSpan.FromSeconds(deltaTime / daySpan.value.TotalDays);
                    timePassed.value += elapsedTime;
                }
                lastTimeAsDouble = Time.timeAsDouble;
            }

            base.Override(state, interpFactor);
            var ticks = state.parameters[0].GetValue<TimeSpan>().Ticks % TimeSpan.TicksPerDay;  // Loop value when ticks is positive.
            ticks = (TimeSpan.TicksPerDay + ticks) % TimeSpan.TicksPerDay;                      // Loop value when ticks is negative.
            Clock.time = new TimeOnly(ticks);
        }
    }
}