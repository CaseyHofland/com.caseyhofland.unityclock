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
    }
}
