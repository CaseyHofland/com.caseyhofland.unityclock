#nullable enable
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    public class TimeOnlyField : TextValueField<long>
    {
        public new class UxmlFactory : UxmlFactory<TimeOnlyField, UxmlTraits> { }
        public new class UxmlTraits : BaseField<long>.UxmlTraits
        {
            private UxmlStringAttributeDescription m_Time = new()
            {
                name = nameof(time),
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var timeOnlyField = (TimeOnlyField)ve;
                timeOnlyField.value = timeOnlyField.StringToValue(m_Time.GetValueFromBag(bag, cc));
                timeOnlyField.isDelayed = true;
            }

            ////private UxmlIntAttributeDescription m_Hour = new()
            ////{
            ////    name = "hour",
            ////    defaultValue = 12
            ////};

            ////private UxmlIntAttributeDescription m_Minute = new()
            ////{
            ////    name = "minute",
            ////};

            ////private UxmlIntAttributeDescription m_Second = new()
            ////{
            ////    name = "second",
            ////};

            ////private UxmlIntAttributeDescription m_Millisecond = new()
            ////{
            ////    name = "millisecond",
            ////};

            //private UxmlBoolAttributeDescription m_ShowHour = new()
            //{
            //    name = "show-hour",
            //    defaultValue = true
            //};

            //private UxmlBoolAttributeDescription m_ShowMinute = new()
            //{
            //    name = "show-minute",
            //    defaultValue = true
            //};

            //private UxmlBoolAttributeDescription m_ShowSecond = new()
            //{
            //    name = "show-second",
            //    defaultValue = false
            //};

            //private UxmlBoolAttributeDescription m_ShowMillisecond = new()
            //{
            //    name = "show-millisecond",
            //    defaultValue = false
            //};

            //private UxmlBoolAttributeDescription m_ShowInterpolant = new()
            //{
            //    name = "show-interpolant",
            //    defaultValue = true
            //};

            //private UxmlBoolAttributeDescription m_ShowInputFields = new()
            //{
            //    name = "show-input-fields",
            //    defaultValue = true
            //};

            //public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            //{
            //    base.Init(ve, bag, cc);

            //    var timeOnlyField = (TimeOnlyField)ve;
            //    timeOnlyField.showHour = m_ShowHour.GetValueFromBag(bag, cc);
            //    timeOnlyField.showMinute = m_ShowMinute.GetValueFromBag(bag, cc);
            //    timeOnlyField.showSecond = m_ShowSecond.GetValueFromBag(bag, cc);
            //    timeOnlyField.showMillisecond = m_ShowMillisecond.GetValueFromBag(bag, cc);
            //    timeOnlyField.showInterpolant = m_ShowInterpolant.GetValueFromBag(bag, cc);
            //    timeOnlyField.showInputFields = m_ShowInputFields.GetValueFromBag(bag, cc);
            //}
        }

        private class TimeSpanInput : TextValueInput
        {
            private LongField parentLongField => (LongField)base.parent;

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
                if (parentLongField.isDelayed)
                {
                    base.text = ValueToString(num2);
                }
                else
                {
                    parentLongField.value = num2;
                }
            }

            protected override string ValueToString(long value)
            {
                UnityEngine.Debug.Log(value);
                return new TimeSpan(value).ToString(base.formatString);
            }
            protected override long StringToValue(string str) => TimeSpan.Parse(!str.Contains(':') ? str + ":0" : str).Ticks;
        }

        public new static readonly string ussClassName = "unity-clock-time-only";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        //private Label _input;
        //private SliderInt hour = new(nameof(TimeOnly.Hour), 0, 23);
        //private SliderInt minute = new(nameof(TimeOnly.Minute), 0, 59);
        //private SliderInt second = new(nameof(TimeOnly.Second), 0, 59);
        //private SliderInt millisecond = new(nameof(TimeOnly.Millisecond), 0, 999);
        //private Slider interpolant = new("Interpolant", 0f, 1f);

        //private bool _showHour;
        //public bool showHour
        //{
        //    get => _showHour;
        //    set
        //    {
        //        if (_showHour != value)
        //        {
        //            _showHour = value;
        //            // UpdateHourVisibility();
        //        }
        //    }
        //}

        //private bool _showMinute;
        //public bool showMinute
        //{
        //    get => _showMinute;
        //    set
        //    {
        //        if (_showMinute != value)
        //        {
        //            _showMinute = value;
        //            // UpdateMinuteVisibility();
        //        }
        //    }
        //}

        //private bool _showSecond;
        //public bool showSecond
        //{
        //    get => _showSecond;
        //    set
        //    {
        //        if (_showSecond != value)
        //        {
        //            _showSecond = value;
        //            // UpdateSecondVisibility();
        //        }
        //    }
        //}

        //private bool _showMillisecond;
        //public bool showMillisecond
        //{
        //    get => _showMillisecond;
        //    set
        //    {
        //        if (_showMillisecond != value)
        //        {
        //            _showMillisecond = value;
        //            // UpdateMillisecondVisibility();
        //        }
        //    }
        //}

        //private bool _showInterpolant;
        //public bool showInterpolant
        //{
        //    get => _showInterpolant;
        //    set
        //    {
        //        if (_showInterpolant != value)
        //        {
        //            _showInterpolant = value;
        //            // UpdateInterpolantVisibility();
        //        }
        //    }
        //}

        //private bool _showInputFields;
        //public virtual bool showInputFields
        //{
        //    get => _showInputFields;
        //    set
        //    {
        //        if (_showInputFields != value)
        //        {
        //            _showInputFields = value;
        //            // UpdateTextFieldVisibility();
        //        }
        //    }
        //}

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

        private TimeSpanInput timeSpanInput => (TimeSpanInput)base.textInputBase;

        public TimeOnlyField() : this(null) { }
        public TimeOnlyField(string? label) : base(label, -1, new TimeSpanInput())
        {
            AddToClassList(ussClassName);
            labelElement.AddToClassList(labelUssClassName);
            AddLabelDragger<long>();
        }

        public virtual void SetValueWithoutNotify(TimeOnly newValue)
        {
            base.SetValueWithoutNotify(newValue.Ticks);
        }

        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, long startValue) => timeSpanInput.ApplyInputDeviceDelta(delta, speed, startValue);
        protected override string ValueToString(long value) => new TimeSpan(value).ToString(formatString, CultureInfo.InvariantCulture.DateTimeFormat);
        protected override long StringToValue(string str) => TimeSpan.TryParse(!str.Contains(':') ? str + ":0" : str, out TimeSpan result) ? result.Ticks : base.rawValue;

        public virtual void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeOnly startValue) => ApplyInputDeviceDelta(delta, speed, startValue.Ticks);
    }
}
