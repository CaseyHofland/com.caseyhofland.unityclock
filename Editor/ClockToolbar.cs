using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityClock.Editor
{
    [Overlay(typeof(SceneView), nameof(ClockToolbar), "Clock Tools", true, defaultDockPosition = DockPosition.Bottom, defaultDockZone = DockZone.TopToolbar)]
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

        public ClockToolbar() : base(nameof(PreviewTimeElement), nameof(PreviewTimeLapseElement)) { }
    }

    [EditorToolbarElement(nameof(PreviewTimeElement), typeof(SceneView))]
    public class PreviewTimeElement : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        private class Dummy : ScriptableObject
        {
            [TimeOnly] public long ticks = TimeSpan.TicksPerDay / 2;
        }

        public EditorWindow containerWindow { get; set; }
        public SceneView sceneView => (SceneView)containerWindow;
        public ClockToolbar clockToolbar => (sceneView.TryGetOverlay(nameof(ClockToolbar), out var overlay) ? (ClockToolbar)overlay : null)!;
        public ClockPreviewControl clockPreviewControl => clockToolbar.clockPreviewControl;

        public PreviewTimeElement()
        {
            // Set display values.
            name = "Preview Time";
            text = "Preview";
            icon = (Texture2D)EditorGUIUtility.IconContent("UnityEditor.AnimationWindow").image;
            tooltip = "Specify a time to preview in the scene view.";

            // Add the DropdownClicked event.
            dropdownClicked += () =>
            {
                var dummy = ScriptableObject.CreateInstance<Dummy>();
                var so = new SerializedObject(dummy);
                var timeProperty = so.FindProperty(nameof(Dummy.ticks));

                LongPopup longPopup = null;
                longPopup = new LongPopup(timeProperty
                    , ticks => clockPreviewControl.time = new(ticks)
                    , () => EditorApplication.update += UpdateTimeProperty
                    , () =>
                    {
                        EditorApplication.update -= UpdateTimeProperty;
                        Object.DestroyImmediate(dummy);
                    });
                UpdateTimeProperty();
                UnityEditor.PopupWindow.Show(worldBound, longPopup);

                void UpdateTimeProperty()
                {
                    timeProperty.longValue = clockPreviewControl.time.Ticks;
                    if (longPopup != null && longPopup.editorWindow != null)
                    {
                        EditorUtility.SetDirty(longPopup.editorWindow);
                        longPopup.editorWindow.Repaint();
                    }
                }
            };

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

            //SetEnabled(clockPreviewControl.canPreview);
            this.Q<Button>().SetEnabled(clockPreviewControl.canPreview);
            SetValueWithoutNotify(clockPreviewControl.previewing);
        }
    }

    [EditorToolbarElement(nameof(PreviewTimeLapseElement), typeof(SceneView))]
    public class PreviewTimeLapseElement : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        private class Dummy : ScriptableObject
        {
            [TimeSpan] public long ticks = TimeSpan.TicksPerSecond * 24;
        }

        public EditorWindow containerWindow { get; set; }
        public SceneView sceneView => (SceneView)containerWindow;
        public ClockToolbar clockToolbar => (sceneView.TryGetOverlay(nameof(ClockToolbar), out var overlay) ? (ClockToolbar)overlay : null)!;
        public ClockPreviewControl clockPreviewControl => clockToolbar.clockPreviewControl;

        private double realtimeAtAnimStart;
        private TimeSpan daySpan = new(0, 0, 24);

        public PreviewTimeLapseElement()
        {
            // Set display values.
            name = "Preview Time Lapse";
            tooltip = "Specify a time lapse to preview in the scene view.";
            icon = (Texture2D)EditorGUIUtility.IconContent("Animation.Play").image;

            // Add the DropdownClicked event.
            dropdownClicked += () =>
            {
                var dummy = ScriptableObject.CreateInstance<Dummy>();
                var so = new SerializedObject(dummy);
                var timeLapseProperty = so.FindProperty(nameof(Dummy.ticks));
                timeLapseProperty.longValue = daySpan.Ticks;

                var longPopup = new LongPopup(timeLapseProperty, ticks => daySpan = new(ticks), onClose: () => Object.DestroyImmediate(dummy));
                UnityEditor.PopupWindow.Show(worldBound, longPopup);
            };

            // Add callbacks.
            SceneView.duringSceneGui += DrawEnabledIfNotInAnimationMode;
            this.RegisterValueChangedCallback(evt => Animate(evt.newValue));
        }

        private void Animate(bool value)
        {
            EditorApplication.update -= UpdateTime;

            if (value)
            {
                realtimeAtAnimStart = Time.realtimeSinceStartupAsDouble;
                EditorApplication.update += UpdateTime;
            }
        }

        private void UpdateTime()
        {
            if (!clockPreviewControl.previewing)
            {
                EditorApplication.update -= UpdateTime;
                return;
            }

            var timeAsDouble = Time.realtimeSinceStartupAsDouble - realtimeAtAnimStart;
            var elapsedTime = daySpan == TimeSpan.Zero ? TimeSpan.Zero : TimeSpan.FromSeconds(timeAsDouble / daySpan.TotalDays);
            clockPreviewControl.time = clockPreviewControl.time.Add(elapsedTime);
            realtimeAtAnimStart = Time.realtimeSinceStartupAsDouble;
        }

        private void DrawEnabledIfNotInAnimationMode(SceneView view)
        {
            if (view != containerWindow)
            {
                return;
            }

            this.Q<Button>().SetEnabled(clockPreviewControl.canPreview);
        }
    }

    internal class LongPopup : PopupWindowContent
    {
        public readonly SerializedProperty property;
        public readonly Action<long> onChanged;
        public readonly Action onOpen;
        public readonly Action onClose;

        public LongPopup(SerializedProperty property, Action<long> onChanged, Action onOpen = null, Action onClose = null)
        {
            this.property = property;
            this.onChanged = onChanged;
            this.onOpen = onOpen;
            this.onClose = onClose;
        }

        public override Vector2 GetWindowSize() => new(280f, EditorGUI.GetPropertyHeight(property, true));

        public override void OnGUI(Rect rect)
        {
            EditorGUIUtility.labelWidth = 100f;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, property, true);
            if (EditorGUI.EndChangeCheck())
            {
                onChanged.Invoke(property.longValue);
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            onOpen?.Invoke();
        }

        public override void OnClose()
        {
            base.OnClose();
            onClose?.Invoke();
        }
    }
}