#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace UnityClock.Editor
{
    [CustomPropertyDrawer(typeof(TimeOnlyAttribute), true)]
    public class TimeOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.numericType != SerializedPropertyNumericType.Int64)
            {
                base.OnGUI(position, property, label);
            }

            using var _ = new EditorGUI.PropertyScope(position, label, property);
            var timeOnly = new TimeOnly(property.longValue);
            var timeOnlyAttribute = (TimeOnlyAttribute)attribute;

            position.height = EditorGUIUtility.singleLineHeight;
            // Draw foldout and text field.
            {
                var foldoutLabel = new GUIContent(label);
                var textFieldPosition = new Rect(position);
                textFieldPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
                textFieldPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f;
                EditorGUI.BeginChangeCheck();
                var timeString = EditorGUI.DelayedTextField(textFieldPosition, timeOnly.ToString(timeOnlyAttribute.timeFormat));
                if (!timeString.Contains(":"))
                {
                    timeString += ":0";
                }
                if (EditorGUI.EndChangeCheck() && TimeSpan.TryParse(timeString, out var result))
                {
                    timeOnly = TimeOnly.MinValue.Add(result);
                }

                if (timeOnlyAttribute.showHour || timeOnlyAttribute.showMinute || timeOnlyAttribute.showSecond || timeOnlyAttribute.showMillisecond || timeOnlyAttribute.showInterpolant)
                {
                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, foldoutLabel, true);
                }
            }

            // Draw sliders.
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Draw hour, minute, second and millisecond.
                {
                    EditorGUI.BeginChangeCheck();

                    timeOnly.Deconstruct(out int hour, out int minute, out int second, out int millisecond);
                    if (timeOnlyAttribute.showHour)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        hour = EditorGUI.IntSlider(position, nameof(timeOnly.Hour), hour, 0, 23);
                    }
                    if (timeOnlyAttribute.showMinute)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        minute = EditorGUI.IntSlider(position, nameof(timeOnly.Minute), minute, 0, 59);
                    }
                    if (timeOnlyAttribute.showSecond)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        second = EditorGUI.IntSlider(position, nameof(timeOnly.Second), second, 0, 59);
                    }
                    if (timeOnlyAttribute.showMillisecond)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        millisecond = EditorGUI.IntSlider(position, nameof(timeOnly.Millisecond), millisecond, 0, 999);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        timeOnly = new TimeOnly(hour, minute, second, millisecond);
                    }
                }

                // Draw interpolant
                if (timeOnlyAttribute.showInterpolant) 
                {
                    EditorGUI.BeginChangeCheck();
                    position.y += EditorGUIUtility.singleLineHeight + 2f;
                    var interpolant = Clock.InverseLerp(TimeOnly.MinValue, TimeOnly.MaxValue, timeOnly);
                    interpolant = EditorGUI.Slider(position, ObjectNames.NicifyVariableName(nameof(interpolant)), interpolant, 0f, 1f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        timeOnly = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(TimeOnly.MaxValue.Ticks) * interpolant);
                    }
                }

                EditorGUI.indentLevel--;
            }

            property.longValue = timeOnly.Ticks;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var timeOnlyAttribute = (TimeOnlyAttribute)attribute;

            var height = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.isExpanded)
            {
                if (timeOnlyAttribute.showHour)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeOnlyAttribute.showMinute)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeOnlyAttribute.showSecond)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeOnlyAttribute.showMillisecond)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeOnlyAttribute.showInterpolant)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
            }

            return height;
        }
    }
}
