## Per Renderer Data
This tool is designed for convenient use of [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) from the Unity editor.
Think of it as overriding a material for a specific Renderer.

Use it in cases when you don't want to create new material every time or for optimization purposes ([GPU Instancing](https://docs.unity3d.com/Manual/GPUInstancing.html)).

### Base case:
1. Add the component to the GameObject.
2. Click the `Add` button
3. Select the property that you want to override.

The `Per Renderer` block - all properties that have the **PerRenderer** or **[PerRendererData](https://docs.unity3d.com/ScriptReference/MaterialProperty.PropFlags.PerRendererData.html)** attribute fall here.

> If you want to override these properties without breaking gpu instancing - you should add gpu instancing support for shader properties yourself, as it stated [here] (https://docs.unity3d.com/Manual/GPUInstancing.html).

By the way, I add samples that have simple shader example with instancing support.


![2](https://user-images.githubusercontent.com/15890881/165573750-072dd1f5-0294-4469-8f03-9002ae06d041.png)
![2](https://user-images.githubusercontent.com/15890881/165573114-98b3a9e7-13f8-4bcc-bc15-6a12ff3b429c.png)


### License
The software is released under the terms of the MIT license.

No personal support or any guarantees.
