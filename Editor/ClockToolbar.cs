#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClock.Editor
{
    [Overlay(typeof(SceneView), nameof(ClockToolbar), "Clock Tools", true, defaultDockPosition = DockPosition.Bottom, defaultDockZone = DockZone.TopToolbar)]
    [Icon("UnityEditor.AnimationWindow")]
    public class ClockToolbar : ToolbarOverlay
    {
        public ClockToolbar() : base(nameof(AnimateTimeElement), nameof(PreviewTimeElement), nameof(PreviewTimeLapseElement)) { }
    }

    [EditorToolbarElement(nameof(AnimateTimeElement), typeof(SceneView))]
    public class AnimateTimeElement : EditorToolbarToggle
    {
        [SerializeField] private AnimationModeDriver driver = ScriptableObject.CreateInstance<AnimationModeDriver>();

        private static readonly string savedValueKey = typeof(SceneView).AssemblyQualifiedName + typeof(AnimateTimeElement).AssemblyQualifiedName;
        private static bool savedValue
        {
            get => EditorPrefs.GetBool(savedValueKey);
            set => EditorPrefs.SetBool(savedValueKey, value);
        }

        public AnimateTimeElement()
        {
            name = "Animate Time";
            tooltip = "Toggle time-based view.";
            icon = (Texture2D)EditorGUIUtility.IconContent("UnityEditor.AnimationWindow").image;

            this.RegisterValueChangedCallback(evt => SetActive(savedValue = evt.newValue));
            value = savedValue;

            RegisterCallback<AttachToPanelEvent>(_ => SetActive(value));
            RegisterCallback<DetachFromPanelEvent>(_ => SetActive(false));
        }

        public void SetActive(bool value)
        {
            if (value && !AnimationMode.InAnimationMode(driver))
            {
                AnimationMode.StartAnimationMode(driver);
                Clock.timeChanged += AnimateTime;
                AnimateTime(Clock.time);
            }
            else if (!value && AnimationMode.InAnimationMode(driver))
            {
                Clock.timeChanged -= AnimateTime;
                AnimationMode.StopAnimationMode(driver);
            }
        }

        public void AnimateTime(TimeOnly time)
        {
            if (EditorApplication.isPlaying || !AnimationMode.InAnimationMode(driver))
            {
                return;
            }

            AnimationMode.BeginSampling();

            var temporalClipSources = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ITemporalClipSource>();
            var clips = new List<AnimationClip>(1);
            var sampleTime = time.Ticks * Clock.dayMultiplier;
            foreach (var clipSource in temporalClipSources)
            {
                clipSource.GetAnimationClips(clips);

                foreach (var clip in clips)
                {
                    AnimationMode.SampleAnimationClip(clipSource.gameObject, clip, sampleTime * clip.length);
                }
            }

            AnimationMode.EndSampling();
        }
    }

    [EditorToolbarElement(nameof(PreviewTimeElement), typeof(SceneView))]
    public class PreviewTimeElement : EditorToolbarDropdownToggle
    {
        public PreviewTimeElement()
        {
            name = "Preview Time";
            text = "Preview";
            tooltip = "Specify a time to preview in the scene view.";
        }
    }

    [EditorToolbarElement(nameof(PreviewTimeLapseElement), typeof(SceneView))]
    public class PreviewTimeLapseElement : EditorToolbarDropdownToggle
    {
        public PreviewTimeLapseElement()
        {
            name = "Preview Time Lapse";
            tooltip = "Specify a time lapse to preview in the scene view.";
            icon = (Texture2D)EditorGUIUtility.IconContent("Animation.Play").image;
        }
    }
}