#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    public class TimeSpanField : TextTimeSpanField
    {
        public new class UxmlFactory : UxmlFactory<TimeSpanField, UxmlTraits> { }

        public new static readonly string ussClassName = "unity-clock-time-only";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        protected TimeSpan rawTime
        {
            get => new(base.rawValue);
            set => base.rawValue = value.Ticks;
        }

        public virtual TimeSpan time
        {
            get => new(base.value);
            set => base.value = value.Ticks;
        }

        public TimeSpanField() : this(null) { }
        public TimeSpanField(string? label) : base(label)
        {
            AddToClassList(ussClassName);
            labelElement.AddToClassList(labelUssClassName);
            AddLabelDragger<long>();
        }

        public virtual void SetValueWithoutNotify(TimeSpan newValue) => base.SetValueWithoutNotify(newValue.Ticks);
        public virtual void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeSpan startValue) => ApplyInputDeviceDelta(delta, speed, startValue.Ticks);
    }
}
