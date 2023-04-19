#nullable enable
using System;
using UnityEngine;

namespace UnityClock
{
    [Serializable]
    [Obsolete]
    public class ClockGradient : ITemporal<Color>
    {
        [field: SerializeField] public Gradient zeroToTwelve { get; set; } = new()
        {
            alphaKeys = new GradientAlphaKey[] { new(1f, 0f), new(1f, 1f) },
            colorKeys = new GradientColorKey[] { new(Color.black, 0f), new(Color.white, 1f) },
            colorSpace = ColorSpace.Linear,
            mode = GradientMode.PerceptualBlend
        };

        [field: SerializeField] public Gradient twelveToZero { get; set; } = new()
        {
            alphaKeys = new GradientAlphaKey[] { new(1f, 0f), new(1f, 1f) },
            colorKeys = new GradientColorKey[] { new(Color.white, 0f), new(Color.black, 1f) },
            colorSpace = ColorSpace.Linear,
            mode = GradientMode.PerceptualBlend
        };

        [field: SerializeField, Tooltip("Ping Pong the gradient from 12:00:00 to 23:59:59.")] public bool pingPong { get; set; } = true;

        public Color Evaluate(TimeOnly time)
        {
            var t = time.Ticks * Clock.pingPongMultiplier;
            return pingPong
                ? zeroToTwelve.Evaluate(1f - Mathf.Abs(t - 1f))
                : t > 1f
                    ? twelveToZero.Evaluate(t - 1f)
                    : zeroToTwelve.Evaluate(t);
        }
    }
}
