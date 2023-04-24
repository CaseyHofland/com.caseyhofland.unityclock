#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    public class TimeOnlyField : TextTimeSpanField
    {
        public new class UxmlFactory : UxmlFactory<TimeOnlyField, UxmlTraits> { }

        public new static readonly string ussClassName = "unity-clock-time-only";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        protected TimeOnly rawTime
        {
            get => new(base.rawValue);
            set => base.rawValue = value.Ticks;
        }

        public virtual TimeOnly time
        {
            get => new(base.value);
            set => base.value = value.Ticks;
        }

        public TimeOnlyField() : this(null) { }
        public TimeOnlyField(string? label) : base(label)
        {
            AddToClassList(ussClassName);
            labelElement.AddToClassList(labelUssClassName);
            AddLabelDragger<long>();
        }

        public virtual void SetValueWithoutNotify(TimeOnly newValue) => base.SetValueWithoutNotify(newValue.Ticks);
        public virtual void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeOnly startValue) => ApplyInputDeviceDelta(delta, speed, startValue.Ticks);
    }
}
