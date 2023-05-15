#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
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
            set => ResampleAnimation(_time = value);
        }

        private Lookup<GameObject, AnimationClip>? _graph;

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

            AnimationMode.StartAnimationMode(driver);
            DestroyGraph();
            ResampleAnimation(time);
        }

        public void StopPreview()
        {
            if (!previewing)
            {
                return;
            }

            DestroyGraph();
            AnimationMode.StopAnimationMode(driver);
        }

        private void DestroyGraph()
        {
            _graph = null;

            //if (!_graph.IsValid())
            //    return;

            //_graph.Destroy();
            //_graphRoot = Playable.Null;
        }

        private void RebuildGraph()
        {
            Dictionary<GameObject, IEnumerable<AnimationClip>> clipsByGameObjects = new();
            foreach (var clipSource in Object.FindObjectsOfType<MonoBehaviour>().OfType<ITemporal>().OfType<IAnimationClipSource>())
            {
                var clips = new List<AnimationClip>(1);
                var gameObject = ((MonoBehaviour)clipSource).gameObject;
                clipSource.GetAnimationClips(clips);
                clipsByGameObjects.Add(gameObject, clips);
            }
            _graph = (Lookup<GameObject, AnimationClip>)clipsByGameObjects.SelectMany(clipsByGameObject => clipsByGameObject.Value.Select(clip => new KeyValuePair<GameObject, AnimationClip>(clipsByGameObject.Key, clip))).ToLookup(pair => pair.Key, pair => pair.Value);
        }

        private void ResampleAnimation(TimeOnly time)
        {
            if (!previewing)
            {
                return;
            }

            Clock.time = time;

            _resampleAnimationMarker.Begin();

            if (_graph == null)
            {
                RebuildGraph();
            }

            AnimationMode.BeginSampling();
            //Undo.FlushUndoRecordObjects();

            var sampleTime = time.Ticks * Clock.dayMultiplier;
            foreach (var clipsByGameObject in _graph!)
            {
                foreach (var clip in clipsByGameObject)
                {
                    AnimationMode.SampleAnimationClip(clipsByGameObject.Key, clip, sampleTime * clip.length);
                }
            }

            AnimationMode.EndSampling();

            SceneView.RepaintAll();

            _resampleAnimationMarker.End();
        }
    }
}