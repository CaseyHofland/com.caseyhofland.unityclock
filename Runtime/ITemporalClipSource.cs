using UnityEngine;

namespace UnityClock
{
    public interface ITemporalClipSource : IAnimationClipSource
    {
        GameObject gameObject { get; }
    }
}