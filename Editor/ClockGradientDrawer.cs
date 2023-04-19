#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.Editor
{
    [System.Obsolete]
    [CustomPropertyDrawer(typeof(ClockGradient))]
    public class ClockGradientDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var zeroToTwelveProperty = property.FindPropertyRelative($"<{nameof(ClockGradient.zeroToTwelve)}>k__BackingField");
            var twelveToZeroProperty = property.FindPropertyRelative($"<{nameof(ClockGradient.twelveToZero)}>k__BackingField");
            var pingPongProperty = property.FindPropertyRelative($"<{nameof(ClockGradient.pingPong)}>k__BackingField");

            var label = new Label(property.displayName);
            label.AddToClassList(BaseField<object>.labelUssClassName);

            var zeroToTwelve = new GradientField()
            {
                bindingPath = zeroToTwelveProperty.propertyPath,
                tooltip = Capitalize(zeroToTwelveProperty.displayName),
                colorSpace = zeroToTwelveProperty.gradientValue.colorSpace,
            };

            var twelveToZero = new GradientField()
            {
                bindingPath = twelveToZeroProperty.propertyPath,
                tooltip = Capitalize(twelveToZeroProperty.displayName),
                colorSpace = twelveToZeroProperty.gradientValue.colorSpace,
            };
            ContextualGradientMenuManipulator(zeroToTwelve);
            ContextualGradientMenuManipulator(twelveToZero);

            var pingPong = new PropertyField(pingPongProperty, string.Empty);
            pingPong.RegisterValueChangeCallback(change => twelveToZero.style.display = change.changedProperty.boolValue ? DisplayStyle.None : DisplayStyle.Flex);

            var root = new VisualElement();
            root.AddToClassList(BaseField<object>.alignedFieldUssClassName);
            root.style.flexDirection = FlexDirection.Row;
            root.style.flexGrow = zeroToTwelve.style.flexGrow = twelveToZero.style.flexGrow = 1f;

            root.Add(label);
            root.Add(zeroToTwelve);
            root.Add(twelveToZero);
            root.Add(pingPong);

            return root;

            void ContextualGradientMenuManipulator(GradientField gradientField)
            {
                var manipulator = new ContextualMenuManipulator(@event =>
                {
                    var useHdrStatus = gradientField.hdr ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;

                    var isUsingLinearColorSpace = gradientField.colorSpace == ColorSpace.Linear;
                    var useLinearColorSpaceStatus = isUsingLinearColorSpace ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
                    var newColorSpace = isUsingLinearColorSpace ? ColorSpace.Gamma : ColorSpace.Linear;

                    @event.menu.AppendAction("Use HDR", action => zeroToTwelve.hdr = twelveToZero.hdr = !gradientField.hdr, useHdrStatus);
                    @event.menu.AppendAction("Use Linear Color Space", action => zeroToTwelve.colorSpace = twelveToZero.colorSpace = newColorSpace, useLinearColorSpaceStatus);
                })
                {
                    target = gradientField
                };
            }
        }

        private string Capitalize(string s) => char.ToUpper(s[0]) + s[1..];
    }
}
