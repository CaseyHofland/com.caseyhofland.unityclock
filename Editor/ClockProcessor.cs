using System;
using UnityEditor;

namespace UnityClock.Editor
{
    public class ClockProcessor : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            if (Clock.time != TimeOnly.MinValue)
            {
                var tmp = Clock.time;
                EditorApplication.delayCall += () => Clock.time = tmp;
            }

            Clock.time = TimeOnly.MinValue;
            return paths;
        }
    }
}
