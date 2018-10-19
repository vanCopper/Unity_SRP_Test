using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
namespace SRPTest_01
{
    public class DevRenderPipeline : RenderPipeline
    {

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            BeginFrameRendering(cameras);

            var clearCmd = new CommandBuffer { name = "Clear(SRP-CommandBuffer)" };
            clearCmd.ClearRenderTarget(true, true, Color.yellow);
            renderContext.ExecuteCommandBuffer(clearCmd);
            clearCmd.Release();//dispose
            //renderContext.Submit();
            foreach (var camera in cameras){

                BeginCameraRendering(camera);
                ScriptableCullingParameters cullingParameters;
                if (!CullResults.GetCullingParameters(camera, out cullingParameters))
                    continue;
                CullResults cullResults = new CullResults();
                CullResults.Cull(ref cullingParameters, renderContext, ref cullResults);

                renderContext.SetupCameraProperties(camera);


                var filterSettings = new FilterRenderersSettings(true);
                var drawSettings = new DrawRendererSettings(camera, new ShaderPassName("BasicLightMode"));
                drawSettings.rendererConfiguration = RendererConfiguration.PerObjectLightProbe | RendererConfiguration.PerObjectLightmaps;


                // 不透明物件
                filterSettings.renderQueueRange = RenderQueueRange.opaque;
                drawSettings.sorting.flags = SortFlags.CommonOpaque;
                renderContext.DrawRenderers(cullResults.visibleRenderers, ref drawSettings, filterSettings);

                // skybox
                if( camera.clearFlags == CameraClearFlags.Skybox){
                    renderContext.DrawSkybox(camera);
                }

                // 透明物件
                filterSettings.renderQueueRange = RenderQueueRange.transparent;
                drawSettings.sorting.flags = SortFlags.CommonTransparent;
                renderContext.DrawRenderers(cullResults.visibleRenderers, ref drawSettings, filterSettings);

                renderContext.Submit();

            }
        }
    }
}
