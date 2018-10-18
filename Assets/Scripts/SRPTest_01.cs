using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
namespace SRPTest_01
{
    public class DevRenderPipeline : RenderPipeline
    {
        CommandBuffer commandBuffer;

        public override void Dispose()
        {
            base.Dispose();

            if(commandBuffer != null){
                commandBuffer.Dispose();
                commandBuffer = null;
            }
        }

        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            BeginFrameRendering(cameras);

            if (commandBuffer == null) commandBuffer = new CommandBuffer();


            foreach(var camera in cameras){

                BeginCameraRendering(camera);
                ScriptableCullingParameters cullingParameters;
                if (!CullResults.GetCullingParameters(camera, out cullingParameters))
                    continue;
                CullResults cullResults = new CullResults();
                CullResults.Cull(ref cullingParameters, renderContext, ref cullResults);

                renderContext.SetupCameraProperties(camera);

                var clearCmd = new CommandBuffer { name = "Clear(SRP-CommandBuffer)" };
                clearCmd.ClearRenderTarget(true, true, Color.gray);
                renderContext.ExecuteCommandBuffer(clearCmd);

                clearCmd.Release();//dispose

                //Draw opaque objects using BasicLightMode shader pass
                var filterSettings = new FilterRenderersSettings(true);
                var drawSettings = new DrawRendererSettings(camera, new ShaderPassName("BasicLightMode"));
                drawSettings.rendererConfiguration = RendererConfiguration.PerObjectLightProbe | RendererConfiguration.PerObjectLightmaps;


                // draw opaque objects
                filterSettings.renderQueueRange = RenderQueueRange.opaque;
                drawSettings.sorting.flags = SortFlags.CommonOpaque;
                renderContext.DrawRenderers(cullResults.visibleRenderers, ref drawSettings, filterSettings);

                // draw skybox
                if( camera.clearFlags == CameraClearFlags.Skybox){
                    renderContext.DrawSkybox(camera);
                }

                // draw transparent objects

                filterSettings.renderQueueRange = RenderQueueRange.transparent;
                drawSettings.sorting.flags = SortFlags.CommonTransparent;
                renderContext.DrawRenderers(cullResults.visibleRenderers, ref drawSettings, filterSettings);

                renderContext.Submit();

                //// 设置上下文为当前相机的上下文
                //renderContext.SetupCameraProperties(camera);
                ////  渲染至相机的backbuffer
                //commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                //commandBuffer.ClearRenderTarget(true, true, Color.black);
                ////执行指令
                //renderContext.ExecuteCommandBuffer(commandBuffer);
                //commandBuffer.Clear();

                //renderContext.DrawSkybox(camera);

                //// 执行裁剪
                //var culled = new CullResults();
                //CullResults.Cull(camera, renderContext, out culled);

                //// Filtering
                //var fs = new FilterRenderersSettings(true);
                //// 设置只绘制不透明物体
                //fs.renderQueueRange = RenderQueueRange.opaque;
                //// 绘制所有层
                //fs.layerMask = ~0;

                //var rs = new DrawRendererSettings(camera, new ShaderPassName("BasicLightMode"));
                //rs.sorting.flags = SortFlags.None;

                ////绘制物体
                //renderContext.DrawRenderers(culled.visibleRenderers, ref rs, fs);

                //renderContext.Submit();
            }
        }
    }
}
