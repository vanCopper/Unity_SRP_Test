using System;
using UnityEngine.Experimental.Rendering;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.ProjectWindowCallback;

#endif

namespace SRPTest_01
{
    public class DevRenderPipelineAsset : RenderPipelineAsset
    {

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Render Pipeline/SRPTest_01/Pipeline Asset")]
        static void CreateSRPTestPipeline()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateSRPTest_01_PipelineAsset>(), "SRPTest_01 Pipeline.asset", null, null);
        }

        class CreateSRPTest_01_PipelineAsset : EndNameEditAction{

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<DevRenderPipelineAsset>();
                AssetDatabase.CreateAsset(instance, pathName);
            }

        }

#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            return new DevRenderPipeline();
        }
    }
}
