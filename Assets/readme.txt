# Wrinkle Maps

Wrinkle Maps Driver is a rendering component developed for The Blacksmith to blend character normal and occlusion maps based on blendshape influences.


### How does it work?

The Wrinkle Maps Driver targets a single skinned mesh renderer. It works by setting custom keywords and properties on the renderer's materials. This has the unfortunate side effect of occasionally modifying those materials, even in non-play scene view mode. This can get slightly annoying, but since these modifications are non-destructive, we decided it was worth the minor inconvenience to have wrinkle maps working all the time, everywhere, even in non-play scene view.


### Setting up your own

It's recommended to examine to included example scene to help you get started with your own wrinkles setup. There's few gotchas to be aware of, though, so we'll mention those explicitly here. Whereas **Target** specifies the skinned mesh renderer you'd like to blend wrinkles for, **Target Bone** is used for culling distance calculations, and would normally be set to the animated bone that moves around in the world (skinned meshes can have origins that don't move). Selecting which objects to render when blending wrinkles uses a combination of two criteria: the specified **Culling Mask** needs to match, and the object needs to have a material with a shader that includes the tag "Special"="Wrinkles". The included Standard shader variant matches the latter criterion.


### Preview mode

At the bottom of the wrinkle maps driver inspector, there's a blendshape preview panel. This allows you to explicitly set blendshape influence weights, and is very handy when setting up and tweaking the wrinkle map layers. Keep in mind that this only changes the influences on the target skinned mesh renderer. If you have additional meshes for hair or facial hair these will remain unaffected by this preview, but work normally when driven by proper animations.


### All the other options

The component contains quite a few configuration fields, here's a quick description of what they all mean.

**Use Wrinkle Normals**: Disable wrinkle normals. Note that this is not a performance optimization but just a quick debug visualization toggle.

**Use Wrinkle Occlusion**: Disable wrinkle occlusion. Note that this is not a performance optimization but just a quick debug visualization toggle.

**Target**: The target gameobject for which to enable wrinkle maps on the attached skinned mesh renderer.

**Target Bone**: The transform to use for calculating distance to target.

**Culling Mask**: Which layers to include when composing the screen-space wrinkles blend. Note that renderers also need a shader tagged with "Special"="Wrinkles" to be included in the composition.

**Max Distance**: How far away from the rendering camera wrinkle maps should updated and used in rendering. As objects move further away from the camera, they become smaller on screen, and at some distance it's just a waste of performance generating and rendering with wrinkle maps.

**Wrinkle Mask**: A texture mask enabling face-part masking. Use one color channel per face part, these correspond to the mask weights in the wrinkle map layers. Use a white texture if you don't need any specific masking.

**Wrinkle Map/Normal Map**: Tangent space normal map for the layer.

**Wrinkle Map/Occlusion Map**: Occlusion map for the layer.

**Wrinkle Map/Bump Scale**: Normal map strength.

**Wrinkle Map/Occlusion Strength**: Occlusion map strength.

**Wrinkle Map/Mask Weight**: Masking weights matching the channels of the **Wrinkle Mask** texture. Set all component to 1 if you don't care about masking.
