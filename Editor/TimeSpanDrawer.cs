#nullable enable
using System;
using System.Reflection;
using UnityClock.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.Editor
{
    [CustomPropertyDrawer(typeof(TimeSpanAttribute), true)]
    public class TimeSpanDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Check if property is of type long.
            if (property.numericType != SerializedPropertyNumericType.Int64)
            {
                return base.CreatePropertyGUI(property);
            }

            // Create the TimeSpanField.
            var timeSpanField = new TimeSpanField(preferredLabel)
            {
                isDelayed = fieldInfo.GetCustomAttribute<DelayedAttribute>(true) != null,
            };

            // Bind the property.
            timeSpanField.RegisterCallback<BlurEvent>(OnBlur);
            timeSpanField.TrackPropertyValue(property, PropertyChanged);
            PropertyChanged(property);

            // Return the TimeSpanField.
            timeSpanField.AddToClassList(TimeSpanField.alignedFieldUssClassName);
            return timeSpanField;

            // Bind property methods.
            void OnBlur(BlurEvent blurEvent)
            {
                property.longValue = timeSpanField.value.Ticks;
                property.serializedObject.ApplyModifiedProperties();
            }

            void PropertyChanged(SerializedProperty property)
            {
                timeSpanField.SetValueWithoutNotify(new TimeSpan(property.longValue));
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if property is of type long.
            if (property.numericType != SerializedPropertyNumericType.Int64)
            {
                base.OnGUI(position, property, label);
                return;
            }

            using var _ = new EditorGUI.PropertyScope(position, label, property);
            var timeSpan = new TimeSpan(property.longValue);
            position.height = EditorGUIUtility.singleLineHeight;

            // Draw foldout and text field.
            {
                var foldoutLabel = new GUIContent(label);
                var textFieldPosition = new Rect(position);
                textFieldPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
                textFieldPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f;

                EditorGUI.BeginChangeCheck();
                var timeString = (fieldInfo.GetCustomAttribute<DelayedAttribute>(true) != null)
                    ? EditorGUI.DelayedTextField(position, label, timeSpan.ToString("g"))
                    : EditorGUI.TextField(position, label, timeSpan.ToString("g"));
                if (EditorGUI.EndChangeCheck() && TimeSpan.TryParse(timeString, out var result))
                {
                    timeSpan = result;
                }

                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, foldoutLabel, true);
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
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        minus = EditorGUI.Toggle(position, ObjectNames.NicifyVariableName(nameof(minus)), minus);
                    }
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        days = EditorGUI.IntField(position, nameof(TimeSpan.Days), days);
                    }
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        hours = EditorGUI.IntSlider(position, nameof(timeSpan.Hours), hours, 0, 23);
                    }
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        minutes = EditorGUI.IntSlider(position, nameof(timeSpan.Minutes), minutes, 0, 59);
                    }
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        seconds = EditorGUI.IntSlider(position, nameof(timeSpan.Seconds), seconds, 0, 59);
                    }
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
            var height = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.isExpanded)
            {
                height += (EditorGUIUtility.singleLineHeight + 2f) * 6f;
            }
            return height;
        }
    }
}
