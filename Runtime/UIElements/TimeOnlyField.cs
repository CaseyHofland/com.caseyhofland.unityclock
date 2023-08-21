using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    //
    // Summary:
    //     Makes a text field for entering TimeOnlys.
    public class TimeOnlyField : TextValueField<TimeOnly>
    {
        private class TimeOnlyInput : TextValueInput
        {
            private TimeOnlyField parentTimeOnlyField => (TimeOnlyField)base.parent;

            protected override string allowedCharacters => UITimeFieldsUtils.k_AllowedCharactersForTime;

            internal TimeOnlyInput()
            {
                base.formatString = UITimeFieldsUtils.k_TimeFieldFormatString;
            }

            public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeOnly startValue)
            {
                var ticksPerSpeed = speed switch
                {
                    DeltaSpeed.Fast => TimeSpan.TicksPerMinute,
                    DeltaSpeed.Slow => TimeSpan.TicksPerMillisecond,
                    _ => TimeSpan.TicksPerSecond
                };

                double num = InternalEngineBridge.CalculateIntDragSensitivity(startValue.Ticks / TimeSpan.TicksPerSecond);
                double niceDelta = (double)InternalEngineBridge.NiceDelta(delta, 1f);   // Acceleration = 1. Normally `speed` would define `acceleration`, but for TimeOnly we use `speed` to define `ticksPerSpeed` instead.
                long roundedDelta = (long)Math.Round(niceDelta * num) * ticksPerSpeed;
                
                long value = LoopMinMaxTimeOnlyValue(roundedDelta, StringToValue(base.text).Ticks);
                if (parentTimeOnlyField.isDelayed)
                {
                    base.text = ValueToString(new TimeOnly(value));
                }
                else
                {
                    parentTimeOnlyField.value = new TimeOnly(value);
                }
            }

            private long LoopMinMaxTimeOnlyValue(long niceDelta, long value)
            {
                value = (value + niceDelta) % TimeSpan.TicksPerDay;             // Loop value when niceDelta is positive.
                value = (TimeSpan.TicksPerDay + value) % TimeSpan.TicksPerDay;  // Loop value when niceDelta is negative.
                return value;
            }

            protected override string ValueToString(TimeOnly v)
            {
                return v.ToString(base.formatString);
            }

            protected override TimeOnly StringToValue(string str)
            {
                UITimeFieldsUtils.TryConvertStringToTimeOnly(str, this.GetOriginalText(), out var value);
                return value;
            }
        }

        //
        // Summary:
        //     USS class name of elements of this type.
        public new static readonly string ussClassName = "unity-timeonly-field";

        //
        // Summary:
        //     USS class name of labels in elements of this type.
        public new static readonly string labelUssClassName = ussClassName + "__label";

        //
        // Summary:
        //     USS class name of input elements in elements of this type.
        public new static readonly string inputUssClassName = ussClassName + "__input";

        private TimeOnlyInput timeOnlyInput => (TimeOnlyInput)base.textInputBase;

        //
        // Summary:
        //     Converts the given TimeOnly to a string.
        //
        // Parameters:
        //   v:
        //     The TimeOnly to be converted to string.
        //
        // Returns:
        //     The TimeOnly as string.
        protected override string ValueToString(TimeOnly v)
        {
            return v.ToString(base.formatString);
        }

        //
        // Summary:
        //     Converts a string to a TimeSpan.
        //
        // Parameters:
        //   str:
        //     The string to convert.
        //
        // Returns:
        //     The TimeSpan parsed from the string.
        protected override TimeOnly StringToValue(string str)
        {
            return UITimeFieldsUtils.TryConvertStringToTimeOnly(str, base.textInputBase.GetOriginalText(), out var value) ? value : base.rawValue;
        }

        //
        // Summary:
        //     Constructor.
        public TimeOnlyField() : this(null) { }

        //
        // Summary:
        //     Constructor.
        //
        // Parameters:
        //   label:
        public TimeOnlyField(string label) : base(label, -1, (TextValueInput)new TimeOnlyInput())
        {
            AddToClassList(ussClassName);
            base.labelElement.AddToClassList(labelUssClassName);
            InternalEngineBridge.GetVisualInput(this).AddToClassList(inputUssClassName);
            AddLabelDragger<TimeOnly>();
        }

        //
        // Summary:
        //     Applies the values of a 3D delta and a speed from an input device.
        //
        // Parameters:
        //   delta:
        //     A vector used to compute the value change.
        //
        //   speed:
        //     A multiplier for the value change.
        //
        //   startValue:
        //     The start value.
        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeOnly startValue)
        {
            timeOnlyInput.ApplyInputDeviceDelta(delta, speed, startValue);
        }
    }
}
