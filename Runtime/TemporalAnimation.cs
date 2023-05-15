#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClock
{
    [AddComponentMenu("Unity Time/Temporal Animation")]
    [DisallowMultipleComponent]
    public class TemporalAnimation : MonoBehaviour, IAnimationClipSource, ITemporal
    {
        [field: SerializeField, Tooltip("The clips used to animate the object. Clips are sampled based on time so that the start and end of the clip correspond to 0:00 on the clock.")] 
        public List<AnimationClip> clips { get; set; } = new List<AnimationClip>();

        [field: SerializeField, Tooltip("If clips should ping pong between midnight and midday so that the start of the clip corresponds to 0:00 on the clock and the end of the clip corresponds to 12:00 on the clock.")]
        public bool pingPong { get; set; }

        [field: SerializeField, Tooltip("If the Animation Component should be destroyed at runtime. The Animation Component is required for editing animations in the editor, but usually you don't want these taking resources during gameplay. Changing this value during runtime has no effect.")]
        public bool destroyAnimationComponentAtRuntime { get; set; } = true;

        private void Reset()
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(this) && !gameObject.GetComponent<Animation>())
            {
                var animation = UnityEditor.Undo.AddComponent<Animation>(gameObject);
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(animation, false);
            }
#endif   
        }

        private void OnEnable()
        {
            Clock.timeChanged += Evaluate;
            Evaluate(Clock.time);
        }

        private void OnDisable()
        {
            Clock.timeChanged -= Evaluate;
        }

        public void Evaluate(TimeOnly time)
        {
            var lengthMultiplier = pingPong
                ? Clock.PingPong(time)
                : time.Ticks * Clock.dayMultiplier;

            foreach (var clip in clips)
            {
                clip.SampleAnimation(gameObject, lengthMultiplier * clip.length);
            }
        }

        void IAnimationClipSource.GetAnimationClips(List<AnimationClip> results)
        {
            results.AddRange(clips);
        }
    }
}