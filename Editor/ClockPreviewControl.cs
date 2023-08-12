// AnimationWindowControl Reference: https://github.com/Unity-Technologies/UnityCsReference/blob/7765d52c6cc13c363796ca00437f1a3209943991/Editor/Mono/Animation/AnimationWindow/AnimationWindowControl.cs#L512

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace UnityClock.Editor
{
    [Serializable]
    public class ClockPreviewControl
    {
        private static readonly ProfilerMarker _resampleAnimationMarker = new($"{nameof(ClockPreviewControl)}.{nameof(ResampleAnimation)}");

        [SerializeField] private AnimationModeDriver? _driver;
        private AnimationModeDriver driver
        {
            get
            {
                if (_driver == null)
                {
                    _driver = ScriptableObject.CreateInstance<AnimationModeDriver>();
                    _driver.hideFlags = HideFlags.HideAndDontSave;
                    _driver.name = "ClockPreviewDriver";
                }

                return _driver;
            }
        }

        public bool canPreview => previewing || !AnimationMode.InAnimationMode();
        public bool previewing
        {
            get => _driver != null && AnimationMode.InAnimationMode(_driver);
            set
            {
                if (value)
                {
                    StartPreview();
                }
                else
                {
                    StopPreview();
                }
            }
        }

        [SerializeField] private TimeOnly _time;
        public TimeOnly time
        {
            get => _time;
            set
            {
                timePassed = TimeSpan.Zero;
                ResampleAnimation(_time = value);
            }
        }

        [field: SerializeField] public TimeSpan daySpan;

        private TimeSpan timePassed;

        private IEnumerable<ICycleSource> cycleSources = Enumerable.Empty<ICycleSource>();

        public void OnCreate()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            if (_driver != null)
            {
                Object.DestroyImmediate(_driver);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode ||
                state == PlayModeStateChange.ExitingEditMode)
            {
                StopPreview();
            }
        }

        public void StartPreview()
        {
            if (previewing || !canPreview)
            {
                return;
            }

            cycleSources = Object.FindObjectsByType<Object>(FindObjectsSortMode.None).OfType<ICycleSource>();
            foreach (var cycleSource in cycleSources)
            {
                cycleSource.Create(Clock.playableGraph);
                cycleSource.OnPlay();
            }

            AnimationMode.StartAnimationMode(driver);
            EditorApplication.update += UpdateTime;
        }

        public void StopPreview()
        {
            if (!previewing)
            {
                return;
            }

            EditorApplication.update -= UpdateTime;
            AnimationMode.StopAnimationMode(driver);

            foreach (var cycleSource in cycleSources)
            {
                cycleSource.OnStop();
                cycleSource.Destroy();
            }
            cycleSources = Enumerable.Empty<ICycleSource>();
        }

        private double realtimeSinceStartPreviewAsDouble;

        private void UpdateTime()
        {
            if (!previewing)
            {
                EditorApplication.update -= UpdateTime;
                return;
            }

            var deltaTimeAsDouble = Time.realtimeSinceStartupAsDouble - realtimeSinceStartPreviewAsDouble;
            if (daySpan != TimeSpan.Zero)
            {
                timePassed += TimeSpan.FromSeconds(deltaTimeAsDouble / daySpan.TotalDays);
                ResampleAnimation(time.Add(timePassed));
            }
            realtimeSinceStartPreviewAsDouble = Time.realtimeSinceStartupAsDouble;
        }

        private void ResampleAnimation(TimeOnly time)
        {
            if (!previewing)
            {
                return;
            }

            _resampleAnimationMarker.Begin();

            AnimationMode.BeginSampling();
            Undo.FlushUndoRecordObjects();

            for (int i = 1; i < Clock.playableGraph.GetOutputCount(); i++)
            {
                if (((AnimationPlayableOutput)Clock.playableGraph.GetOutput(i)).IsOutputValid())
                {
                    AnimationMode.SamplePlayableGraph(Clock.playableGraph, i, Clock.Day(time));
                }
            }

            AnimationMode.EndSampling();
            SceneView.RepaintAll();

            _resampleAnimationMarker.End();
        }
    }
}