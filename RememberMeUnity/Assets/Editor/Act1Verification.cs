using System;
using UnityEditor;
using UnityEngine;

namespace RememberMe.Editor
{
    public static class Act1Verification
    {
        public static void Verify()
        {
            if (EditorSettings.defaultBehaviorMode != EditorBehaviorMode.Mode2D)
                throw new InvalidOperationException("Project must default to 2D.");
            var pipeline = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            if (!pipeline || !pipeline.GetType().Name.Contains("UniversalRenderPipeline"))
                throw new InvalidOperationException("The Unity 2D URP pipeline is not assigned.");
            var controller = Type.GetType("RememberMe.Act1Game, Assembly-CSharp");
            if (controller == null)
                throw new InvalidOperationException("The Act I playable controller is not implemented.");
            Debug.Log("Validated 2D defaults, URP and Act I controller.");
        }
    }
}
