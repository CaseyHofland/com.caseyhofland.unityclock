#nullable enable
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    public abstract class TextTimeSpanField : TextValueField<long>
    {
        public new class UxmlTraits : BaseField<long>.UxmlTraits
        {
            private UxmlStringAttributeDescription m_Time = new()
            {
                name = "time",
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var textTimeSpanField = (TextTimeSpanField)ve;
                textTimeSpanField.value = textTimeSpanField.StringToValue(m_Time.GetValueFromBag(bag, cc));
                textTimeSpanField.isDelayed = true;
            }
        }

        protected class TimeSpanInput : TextValueInput
        {
            private TextTimeSpanField parentTimeOnlyField => (TextTimeSpanField)base.parent;

            protected override string allowedCharacters => "0123456789:.";

            internal TimeSpanInput()
            {
                base.formatString = "g";
            }

            //private static readonly MethodInfo calculateIntDragSensitivity = Type.GetType()

            public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue)
            {
                //double num = NumericFieldDraggerUtility.CalculateIntDragSensitivity(startValue);
                //float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
                //long num2 = StringToValue(base.text);
                //num2 += (long)Math.Round((double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num);

                long num2 = StringToValue(base.text);
                if (parentTimeOnlyField.isDelayed)
                {
                    base.text = ValueToString(num2);
                }
                else
                {
                    parentTimeOnlyField.value = num2;
                }
            }

            protected override string ValueToString(long value)
            {
                return new TimeSpan(value).ToString(base.formatString);
            }
            protected override long StringToValue(string str) => TimeSpan.Parse(!str.Contains(':') ? str + ":0" : str).Ticks;
        }

        protected TimeSpanInput timeSpanInput => (TimeSpanInput)base.textInputBase;

        public TextTimeSpanField(string? label) : base(label, -1, new TimeSpanInput()) { }

        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue) => timeSpanInput.ApplyInputDeviceDelta(delta, speed, startValue);
        protected override string ValueToString(long value) => new TimeSpan(value).ToString(formatString, CultureInfo.InvariantCulture.DateTimeFormat);
        protected override long StringToValue(string str) => TimeSpan.TryParse(!str.Contains(':') ? str + ":0" : str, out TimeSpan result) ? result.Ticks : base.rawValue;
    }
}
