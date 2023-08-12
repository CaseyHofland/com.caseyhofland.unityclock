#nullable enable
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityClock
{
    [Serializable]
    public struct AnimationClipCycle : ICycle, ISerializationCallbackReceiver
    {
        [field: SerializeField] public AnimationClip clip { get; set; }
        [field: SerializeField] public int loops { get; set; }
        [SerializeField, TimeOnly] private long _start;
        [SerializeField, TimeOnly] private long _duration;

        public TimeOnly start { get; set; }
        public TimeOnly duration { get; set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _start = start.Ticks;
            _duration = duration.Ticks;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            start = new(_start);
            duration = new(_duration);
            loops = Mathf.Max(loops, 1);
        }

        public static AnimationClipPlayable Create(PlayableGraph graph, AnimationClipCycle cycle)
        {
            if (cycle.clip == null)
            {
                return (AnimationClipPlayable)Playable.Null;
            }

            var playable = AnimationClipPlayable.Create(graph, cycle.clip);
            playable.SetCycle(cycle, cycle.clip.length);
            return playable;
        }
    }
}
