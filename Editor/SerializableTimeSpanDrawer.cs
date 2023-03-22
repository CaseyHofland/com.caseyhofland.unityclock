using System;
using UnityEditor;
using UnityEngine;

namespace UnityClock.Editor
{
    [CustomPropertyDrawer(typeof(SerializableTimeSpan))]
    public class SerializableTimeSpanDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty _days = property.FindPropertyRelative(nameof(_days));
            SerializedProperty _hours = property.FindPropertyRelative(nameof(_hours));
            SerializedProperty _minutes = property.FindPropertyRelative(nameof(_minutes));
            SerializedProperty _seconds = property.FindPropertyRelative(nameof(_seconds));
            SerializedProperty _milliseconds = property.FindPropertyRelative(nameof(_milliseconds));

            var timeSpan = new TimeSpan(_days.intValue, _hours.intValue, _minutes.intValue, _seconds.intValue, _milliseconds.intValue);

            var timeStringPosition = new Rect(position);
            timeStringPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
            timeStringPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f;
            timeStringPosition.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            var timeString = EditorGUI.DelayedTextField(timeStringPosition, GUIContent.none, timeSpan.ToString("g"));
            if (EditorGUI.EndChangeCheck())
            {
                if (TimeSpan.TryParse(timeString, out var result))
                {
                    timeSpan = result;
                }
            }

            _days.intValue = timeSpan.Days;
            _hours.intValue = timeSpan.Hours;
            _minutes.intValue = timeSpan.Minutes;
            _seconds.intValue = timeSpan.Seconds;
            _milliseconds.intValue = timeSpan.Milliseconds;

            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName), true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
