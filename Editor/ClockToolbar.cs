using UnityClock.UIElements;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.Editor
{
    [Overlay(typeof(SceneView), nameof(ClockToolbar), "Clock Tools", defaultDisplay = true, defaultDockPosition = DockPosition.Bottom, defaultDockZone = DockZone.TopToolbar)]
    [Icon("UnityEditor.AnimationWindow")]
    public class ClockToolbar : ToolbarOverlay
    {
        public readonly ClockPreviewControl clockPreviewControl = new()
        {
            time = new(12, 0)
        };

        public override void OnCreated()
        {
            clockPreviewControl.OnCreate();
        }

        public override void OnWillBeDestroyed()
        {
            clockPreviewControl.previewing = false;
            clockPreviewControl.OnDestroy();
        }

        public ClockToolbar() : base(nameof(SceneClockElement)) { }
    }

    [EditorToolbarElement(nameof(SceneClockElement), typeof(SceneView))]
    public class SceneClockElement : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        internal const string elementName = "Scene Clock";

        public EditorWindow containerWindow { get; set; }
        public SceneView sceneView => (SceneView)containerWindow;
        public ClockToolbar clockToolbar => (sceneView.TryGetOverlay(nameof(ClockToolbar), out var overlay) ? (ClockToolbar)overlay : null)!;
        public ClockPreviewControl clockPreviewControl => clockToolbar.clockPreviewControl;

        public SceneClockElement()
        {
            // Set display values.
            name = elementName;
            icon = (Texture2D)EditorGUIUtility.IconContent("UnityEditor.AnimationWindow").image;
            //tooltip = "Specify a time to preview in the scene view.";

            // Add the DropdownClicked event.
            dropdownClicked += () => UnityEditor.PopupWindow.Show(worldBound, new ClockPreviewContent(clockPreviewControl));

            // Add callbacks.
            SceneView.duringSceneGui += DrawEnabledIfNotInAnimationMode;
            this.RegisterValueChangedCallback(evt => clockPreviewControl.previewing = evt.newValue);
        }

        private void DrawEnabledIfNotInAnimationMode(SceneView sceneView)
        {
            if (sceneView != containerWindow)
            {
                return;
            }

            this.Q<Button>().SetEnabled(clockPreviewControl.canPreview);
            SetValueWithoutNotify(clockPreviewControl.previewing);
        }

        private class ClockPreviewContent : PopupWindowContent
        {
            public readonly ClockPreviewControl clockPreviewControl;

            public ClockPreviewContent(ClockPreviewControl clockPreviewControl)
            {
                this.clockPreviewControl = clockPreviewControl;
            }

            public override Vector2 GetWindowSize() => new(280f, 18f * editorWindow.rootVisualElement.childCount + (editorWindow.rootVisualElement.childCount - 1));

            public override void OnGUI(Rect rect) { } // Intentionally left empty.

            public override void OnOpen()
            {
                base.OnOpen();

                var sceneClockLabel = new Label(elementName);
                sceneClockLabel.AddToClassList(alignedFieldUssClassName);
                sceneClockLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

                var timeField = new TimeOnlyField("Time")
                {
                    value = clockPreviewControl.time
                };
                timeField.RegisterValueChangedCallback(timeChanged => clockPreviewControl.time = timeChanged.newValue);

                var daySpanField = new TimeSpanField("Day Span")
                {
                    value = clockPreviewControl.daySpan,
                    isDelayed = true
                };
                daySpanField.RegisterValueChangedCallback(daySpanChanged => clockPreviewControl.daySpan = daySpanChanged.newValue);

                editorWindow.rootVisualElement.Add(sceneClockLabel);
                editorWindow.rootVisualElement.Add(timeField);
                editorWindow.rootVisualElement.Add(daySpanField);
            }
        }
    }
}