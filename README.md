### Per Renderer Data
This tool is designed for convenient use of [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) from the Unity editor.
Think of it as overriding a material for a specific Renderer.

Use it in cases when you don't want to create new material every time or for optimization purposes ([GPU Instancing](https://docs.unity3d.com/Manual/GPUInstancing.html)).

### Base case:
- Add the component to the GameObject.
- Click the `Add` button
- Select the property that you want to override.

The `Per Renderer` block - all properties that have the **PerRenderer** or **[PerRendererData](https://docs.unity3d.com/ScriptReference/MaterialProperty.PropFlags.PerRendererData.html)** attribute fall here.

> If you want to override these properties without breaking gpu instancing - you should add gpu instancing support for shader properties yourself, as it stated [here] (https://docs.unity3d.com/Manual/GPUInstancing.html).

By the way, I add samples that have simple shader example with instancing support.