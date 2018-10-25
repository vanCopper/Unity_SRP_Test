using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
namespace SRPTest_01
{
    public class DevRenderPipeline : RenderPipeline
    {
        CommandBuffer _lightCb = new CommandBuffer();
        public override void Dispose()
        {
            base.Dispose();
            if(_lightCb != null){
                _lightCb.Dispose();
                _lightCb = null;
            }
        }

        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);

            BeginFrameRendering(cameras);

            var clearCmd = new CommandBuffer { name = "Clear(SRP-CommandBuffer)" };
            clearCmd.ClearRenderTarget(true, true, Color.gray);
            renderContext.ExecuteCommandBuffer(clearCmd);
            clearCmd.Release();//dispose

            var _WorldSpaceLightPos0 =  Shader.PropertyToID("_WorldSpaceLightPos0");
            var _LightColor0 = Shader.PropertyToID("_LightColor0");

            //renderContext.Submit();
            foreach (var camera in cameras){

                BeginCameraRendering(camera);
                ScriptableCullingParameters cullingParameters;
                if (!CullResults.GetCullingParameters(camera, out cullingParameters))
                    continue;
                CullResults cullResults = new CullResults();
                CullResults.Cull(ref cullingParameters, renderContext, ref cullResults);

                // 设置全局第一个平行光
                var lights = cullResults.visibleLights;
                foreach(var light in lights){
                    if(light.lightType != LightType.Directional) continue;

                    Vector4 lightPos = light.localToWorld.GetColumn(2);
                    Vector4 lightDir = new Vector4(-lightPos.x, -lightPos.y, -lightPos.z, 0);
                    
                    Color lightColor = light.finalColor;

                    _lightCb.SetGlobalVector(_WorldSpaceLightPos0, lightDir);
                    _lightCb.SetGlobalVector(_LightColor0, lightColor);
                    _lightCb.SetGlobalVector("ambientLightSky", RenderSettings.ambientSkyColor);
                    // RenderSettings.

                    renderContext.ExecuteCommandBuffer(_lightCb);
                    _lightCb.Clear();

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
                        //renderContext.DrawSkybox(camera);
                    }

                    // 透明物件
                    filterSettings.renderQueueRange = RenderQueueRange.transparent;
                    drawSettings.sorting.flags = SortFlags.CommonTransparent;
                    renderContext.DrawRenderers(cullResults.visibleRenderers, ref drawSettings, filterSettings);
                }

                renderContext.Submit();

            }
        }
    }
}
