#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.UIElements
{
    public class TimeOnlyFoldout : Foldout
    {
        public new class UxmlFactory : UxmlFactory<TimeOnlyFoldout, UxmlTraits> { }

        public TimeOnlyFoldout() : this(null) { }
        public TimeOnlyFoldout(string? label) : base()
        {
            var timeOnlyField = new TimeOnlyField(label);
            timeOnlyField.style.position = Position.Absolute;
            timeOnlyField.style.width = new Length(100f, LengthUnit.Percent);
            timeOnlyField.style.paddingRight = 18f;
            timeOnlyField.style.left = 18f;

            hierarchy.parent.Add(timeOnlyField);

            var hourSlider = new SliderInt(nameof(TimeOnly.Hour), 0, 23);
            Add(hourSlider);

            var minuteSlider = new SliderInt(nameof(TimeOnly.Minute), 0, 59);
            Add(minuteSlider);

            var secondSlider = new SliderInt(nameof(TimeOnly.Second), 0, 59);
            Add(secondSlider);

            var millisecondSlider = new SliderInt(nameof(TimeOnly.Millisecond), 0, 999);
            Add(millisecondSlider);
        }
    }
}
