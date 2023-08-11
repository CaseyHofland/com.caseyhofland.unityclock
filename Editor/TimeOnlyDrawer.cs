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
    [CustomPropertyDrawer(typeof(TimeOnlyAttribute), true)]
    public class TimeOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Check if property is of type long.
            if (property.numericType != SerializedPropertyNumericType.Int64)
            {
                return base.CreatePropertyGUI(property);
            }

            // Create the TimeOnlyField.
            var timeOnlyField = new TimeOnlyField(preferredLabel)
            {
                isDelayed = fieldInfo.GetCustomAttribute<DelayedAttribute>(true) != null,
            };

            // Bind the property.
            timeOnlyField.RegisterCallback<BlurEvent>(OnBlur);
            timeOnlyField.TrackPropertyValue(property, PropertyChanged);
            PropertyChanged(property);

            // Return the TimeSpanField.
            timeOnlyField.AddToClassList(TimeOnlyField.alignedFieldUssClassName);
            return timeOnlyField;

            // Bind property methods.
            void OnBlur(BlurEvent blurEvent)
            {
                property.longValue = timeOnlyField.value.Ticks;
                property.serializedObject.ApplyModifiedProperties();
            }

            void PropertyChanged(SerializedProperty property)
            {
                timeOnlyField.SetValueWithoutNotify(new TimeOnly(property.longValue));
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
            var timeOnly = new TimeOnly(property.longValue);
            position.height = EditorGUIUtility.singleLineHeight;

            // Draw foldout and text field.
            {
                var foldoutLabel = new GUIContent(label);
                var textFieldPosition = new Rect(position);
                textFieldPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
                textFieldPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f;

                EditorGUI.BeginChangeCheck();
                var timeString = (fieldInfo.GetCustomAttribute<DelayedAttribute>(true) != null)
                    ? EditorGUI.DelayedTextField(position, label, timeOnly.ToString("g"))
                    : EditorGUI.TextField(position, label, timeOnly.ToString("g"));
                if (!timeString.Contains(":"))
                {
                    timeString += ":00";
                }
                if (EditorGUI.EndChangeCheck() && TimeSpan.TryParse("0." + timeString, out var result))
                {
                    timeOnly = TimeOnly.MinValue.Add(result);
                }

                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, foldoutLabel, true);
            }

            // Draw sliders.
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Draw hour, minute, second and millisecond.
                {
                    EditorGUI.BeginChangeCheck();

                    timeOnly.Deconstruct(out int hour, out int minute, out int second, out int millisecond);
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        hour = EditorGUI.IntSlider(position, nameof(timeOnly.Hour), hour, 0, 23);
                    }
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        minute = EditorGUI.IntSlider(position, nameof(timeOnly.Minute), minute, 0, 59);
                    }
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2f;
                        second = EditorGUI.IntSlider(position, nameof(timeOnly.Second), second, 0, 59);
                    }
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
            var height = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.isExpanded)
            {
                height += (EditorGUIUtility.singleLineHeight + 2f) * 5f;
            }
            return height;
        }
    }
}
