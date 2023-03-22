#nullable enable
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityClock.Editor
{
    public class TemporalAnimationProcessor : IProcessSceneWithReport
    {
        private List<GameObject> rootGameObjects = new();
        private List<TemporalAnimation> temporalAnimations = new();

        int IOrderedCallback.callbackOrder => default;
        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            scene.GetRootGameObjects(rootGameObjects);
            rootGameObjects.ForEach(OnProcessGameObject);
        }

        private void OnProcessGameObject(GameObject gameObject)
        {
            gameObject.GetComponentsInChildren(true, temporalAnimations);
            temporalAnimations.ForEach(OnProcessTemporalAnimation);
        }

        private void OnProcessTemporalAnimation(TemporalAnimation temporalAnimation)
        {
            if (!temporalAnimation.destroyAnimationComponentAtRuntime)
            {
                return;
            }

            var animation = temporalAnimation.GetComponent<Animation>();
            Object.DestroyImmediate(animation);
        }
    }
}