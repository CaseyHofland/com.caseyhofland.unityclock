#nullable enable
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityClock
{
    /// <summary>
    /// A <see cref="VolumeParameter"/> that holds a <see cref="SerializableTimeOnly"/> value.
    /// </summary>
    [Serializable]
    public sealed class TimeOnlyParameter : VolumeParameter<TimeOnly>
    {
        [SerializeField, TimeOnly(true, true, true, true, true)] private new long m_Value;

        public override TimeOnly value 
        { 
            get => new(m_Value);
            set => m_Value = value.Ticks;
        }

        /// <summary>
        /// Creates a new <see cref="TimeOnlyParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public TimeOnlyParameter(TimeOnly value, bool overrideState = false) : base(value, overrideState) { }

        public override void Interp(TimeOnly from, TimeOnly to, float t) => m_Value = from.Add((to - from) * t).Ticks;
    }

    /// <summary>
    /// A <see cref="VolumeParameter"/> that holds a <see cref="TimeSpan"/> value.
    /// </summary>
    [Serializable]
    public sealed class TimeSpanParameter : VolumeParameter<TimeSpan>
    {
        [SerializeField, TimeSpan(true, true, true, true, true)] private new long m_Value;

        public override TimeSpan value 
        { 
            get => new(m_Value); 
            set => m_Value = value.Ticks;
        }

        /// <summary>
        /// Creates a new <see cref="TimeSpanParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public TimeSpanParameter(TimeSpan value, bool overrideState = false) : base(value, overrideState) { }

        public override void Interp(TimeSpan from, TimeSpan to, float t) => m_Value = (from + (to - from) * t).Ticks;
    }
}
