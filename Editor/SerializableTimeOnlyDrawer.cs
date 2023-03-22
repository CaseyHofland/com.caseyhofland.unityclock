using System;
using UnityEditor;
using UnityEngine;

namespace UnityClock.Editor
{
    [CustomPropertyDrawer(typeof(SerializableTimeOnly))]
    public class SerializableTimeOnlyDrawer : PropertyDrawer
    {
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    return new TimeOnlyField(property.displayName);
        //}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty _hour = property.FindPropertyRelative(nameof(_hour));
            SerializedProperty _minute = property.FindPropertyRelative(nameof(_minute));
            SerializedProperty _second = property.FindPropertyRelative(nameof(_second));
            SerializedProperty _millisecond = property.FindPropertyRelative(nameof(_millisecond));

            var timeOnly = new TimeOnly(_hour.intValue, _minute.intValue, _second.intValue, _millisecond.intValue);

            var timeStringPosition = new Rect(position);
            timeStringPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
            timeStringPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f;
            timeStringPosition.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            var timeString = EditorGUI.DelayedTextField(timeStringPosition, GUIContent.none, timeOnly.ToString("g"));
            if (EditorGUI.EndChangeCheck())
            {
                if (TimeSpan.TryParse(timeString, out var result))
                {
                    timeOnly = TimeOnly.MinValue.Add(result);
                }
            }

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var scalePosition = new Rect(position);
                scalePosition.y += position.height - EditorGUIUtility.singleLineHeight;
                scalePosition.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.BeginChangeCheck();
                var scale = Clock.InverseLerp(TimeOnly.MinValue, TimeOnly.MaxValue, timeOnly);
                scale = EditorGUI.Slider(scalePosition, "Scale", scale, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    timeOnly = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(TimeOnly.MaxValue.Ticks) * scale);
                }

                EditorGUI.indentLevel--;
            }

            _hour.intValue = timeOnly.Hour;
            _minute.intValue = timeOnly.Minute;
            _second.intValue = timeOnly.Second;
            _millisecond.intValue = timeOnly.Millisecond;

            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName), true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) 
                + (property.isExpanded ? EditorGUIUtility.singleLineHeight + 2f : 0f);
        }
    }
}
