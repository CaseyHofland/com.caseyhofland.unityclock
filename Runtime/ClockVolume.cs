using System;
using UnityEngine.Rendering;

namespace UnityClock
{
    /// <summary>
    /// A volume component that holds settings for the Clock.
    /// </summary>
    [Serializable, VolumeComponentMenu(nameof(Clock))]
    public sealed class ClockVolume : VolumeComponent
    {
        public TimeOnlyParameter time = new(new TimeOnly(12, 0), true);
        public TimeSpanParameter daySpan = new(default);

        protected override void OnEnable()
        {
            base.OnEnable();

            displayName = nameof(Clock);
        }

        public override void Override(VolumeComponent state, float interpFactor)
        {
            base.Override(state, interpFactor);

            var time = state.parameters[0].GetValue<TimeOnly>();
            var daySpan = state.parameters[1].GetValue<TimeSpan>();

            Clock.time = time.Add(Clock.ElapsedTime(daySpan));
        }
    }
}