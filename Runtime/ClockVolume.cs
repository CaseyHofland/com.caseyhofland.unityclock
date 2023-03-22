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

            var time = (TimeOnly)state.parameters[0].GetValue<SerializableTimeOnly>();
            var daySpan = state.parameters[1].GetValue<SerializableTimeSpan>(); ;

            Clock.time = time.Add(Clock.ElapsedTime(daySpan));
        }

        /// <summary>
        /// A <see cref="VolumeParameter"/> that holds a <see cref="SerializableTimeOnly"/> value.
        /// </summary>
        [Serializable]
        public sealed class TimeOnlyParameter : VolumeParameter<SerializableTimeOnly>
        {
            /// <summary>
            /// Creates a new <see cref="TimeOnlyParameter"/> instance.
            /// </summary>
            /// <param name="value">The initial value to store in the parameter.</param>
            /// <param name="overrideState">The initial override state for the parameter.</param>
            public TimeOnlyParameter(TimeOnly value, bool overrideState = false) : base(value, overrideState) { }

            public override void Interp(SerializableTimeOnly from, SerializableTimeOnly to, float t) => m_Value = ((TimeOnly)from).Add(((TimeOnly)to - from) * t);
        }

        /// <summary>
        /// A <see cref="VolumeParameter"/> that holds a <see cref="TimeSpan"/> value.
        /// </summary>
        [Serializable]
        public sealed class TimeSpanParameter : VolumeParameter<SerializableTimeSpan>
        {
            /// <summary>
            /// Creates a new <see cref="TimeSpanParameter"/> instance.
            /// </summary>
            /// <param name="value">The initial value to store in the parameter.</param>
            /// <param name="overrideState">The initial override state for the parameter.</param>
            public TimeSpanParameter(TimeSpan value, bool overrideState = false) : base(value, overrideState) { }

            public override void Interp(SerializableTimeSpan from, SerializableTimeSpan to, float t) => m_Value = from + ((TimeSpan)to - from) * t;
        }
    }
}