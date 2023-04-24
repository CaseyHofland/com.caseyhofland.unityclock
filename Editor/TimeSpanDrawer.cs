#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace UnityClock.Editor
{
    [CustomPropertyDrawer(typeof(TimeSpanAttribute), true)]
    public class TimeSpanDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.numericType != SerializedPropertyNumericType.Int64)
            {
                base.OnGUI(position, property, label);
            }

            using var _ = new EditorGUI.PropertyScope(position, label, property);
            var timeSpan = new TimeSpan(property.longValue);
            var timeSpanAttribute = (TimeSpanAttribute)attribute;

            position.height = EditorGUIUtility.singleLineHeight;
            // Draw foldout and text field.
            {
                var foldoutLabel = new GUIContent(label);
                var textFieldPosition = new Rect(position);
                textFieldPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
                textFieldPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f;
                EditorGUI.BeginChangeCheck();
                var timeString = EditorGUI.DelayedTextField(textFieldPosition, timeSpan.ToString(timeSpanAttribute.timeFormat));
                if (!timeString.Contains(":"))
                {
                    timeString += ":0";
                }
                if (EditorGUI.EndChangeCheck() && TimeSpan.TryParse(timeString, out var result))
                {
                    timeSpan = result;
                }

                if (timeSpanAttribute.showDays || timeSpanAttribute.showHours || timeSpanAttribute.showMinutes || timeSpanAttribute.showSeconds || timeSpanAttribute.showMilliseconds)
                {
                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, foldoutLabel, true);
                }
            }

            // Draw sliders.
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Draw days, hours, minutes, seconds and milliseconds.
                {
                    EditorGUI.BeginChangeCheck();

                    bool minus = timeSpan.Ticks < 0;
                    int days = Math.Abs(timeSpan.Days), hours = Math.Abs(timeSpan.Hours), minutes = Math.Abs(timeSpan.Minutes), seconds = Math.Abs(timeSpan.Seconds), milliseconds = Math.Abs(timeSpan.Milliseconds);
                    if (timeSpanAttribute.showMinus)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        minus = EditorGUI.Toggle(position, ObjectNames.NicifyVariableName("minusThing"), minus);
                    }
                    if (timeSpanAttribute.showDays)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        days = EditorGUI.IntField(position, nameof(TimeSpan.Days), days);
                    }
                    if (timeSpanAttribute.showHours)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        hours = EditorGUI.IntSlider(position, nameof(timeSpan.Hours), hours, 0, 23);
                    }
                    if (timeSpanAttribute.showMinutes)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        minutes = EditorGUI.IntSlider(position, nameof(timeSpan.Minutes), minutes, 0, 59);
                    }
                    if (timeSpanAttribute.showSeconds)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        seconds = EditorGUI.IntSlider(position, nameof(timeSpan.Seconds), seconds, 0, 59);
                    }
                    if (timeSpanAttribute.showMilliseconds)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        milliseconds = EditorGUI.IntSlider(position, nameof(timeSpan.Milliseconds), milliseconds, 0, 999);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        timeSpan = new TimeSpan(days, hours, minutes, seconds, milliseconds);
                        if (minus)
                        {
                            timeSpan = -timeSpan;
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }

            property.longValue = timeSpan.Ticks;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var timeSpanAttribute = (TimeSpanAttribute)attribute;

            var height = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.isExpanded)
            {
                if (timeSpanAttribute.showMinus)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeSpanAttribute.showDays)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeSpanAttribute.showHours)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeSpanAttribute.showMinutes)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeSpanAttribute.showSeconds)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
                if (timeSpanAttribute.showMilliseconds)
                {
                    height += EditorGUIUtility.singleLineHeight + 2f;
                }
            }

            return height;
        }
    }
}
