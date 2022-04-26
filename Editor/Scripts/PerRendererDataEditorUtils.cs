using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Wellmor.PerRendererData;

namespace Wellmor.PerRendererData
{
    internal static class PerRendererDataEditorUtils
    {
        internal static class Constants
        {
            public const string RendererPropertyName = "_renderer";
            public const string PropertiesPropertyName = "_properties";
            public const string PropertyDataNamePropertyName = "_name";
            public const string ValuePropertyName = "_value";
            public const string EventNameProp = "_event";
            
            public const string ScaleOffsetPropertyName = "_scaleOffset";
            public const string UseScaleOffsetPropertyName = "_useScaleOffset";
            public const string AttributeName = "PerRenderer";
        }
        
        internal class MaterialPropertyInfo
        {
            public string Name;
            public ShaderUtil.ShaderPropertyType Type;
            public MaterialProperty Property;
            public string[] Attributes;
        }

        internal static void Split<T>(this List<T> source, out List<T> first, out List<T> second, Predicate<T> match)
        {
            first = new List<T>();
            second = new List<T>();

            foreach (var item in source)
            {
                if (match(item)) first.Add(item);
                else second.Add(item);
            }
        }

        internal static List<MaterialPropertyInfo> GetPropertiesInfoFrom(Material[] materials)
        {
            var result = new List<MaterialPropertyInfo>();

            foreach (var mat in materials)
            {
                var shader = mat.shader;
                var count = ShaderUtil.GetPropertyCount(shader);
                for (var i = 0; i < count; i++)
                {
                    var info = new MaterialPropertyInfo
                    {
                        Name = ShaderUtil.GetPropertyName(shader, i),
                        Type = ShaderUtil.GetPropertyType(shader, i),
                        Attributes = shader.GetPropertyAttributes(i)
                    };

                    info.Property = MaterialEditor.GetMaterialProperty(new[] { mat }, info.Name);

                    result.Add(info);
                }
            }

            return result;
        }

        internal static PropertyData CreatePropertyData(MaterialProperty property)
        {
            if (property == null) return null;

            PropertyData result;
            switch (property.type)
            {
                case MaterialProperty.PropType.Color:
                    result = new ColorPropertyData(property.name, property.colorValue);
                    break;
                case MaterialProperty.PropType.Vector:
                    result = new VectorPropertyData(property.name, property.vectorValue);
                    break;
                case MaterialProperty.PropType.Int:
                    result = new IntPropertyData(property.name, property.intValue);
                    break;
                case MaterialProperty.PropType.Float:
                case MaterialProperty.PropType.Range:
                    result = new FloatPropertyData(property.name, property.floatValue);
                    break;
                case MaterialProperty.PropType.Texture:
                    result = (property.flags & MaterialProperty.PropFlags.NoScaleOffset) == 0
                        ? new TexturePropertyData(property.name, property.textureValue, property.textureScaleAndOffset)
                        : new TexturePropertyData(property.name, property.textureValue);
                    break;
                default:
                    return null;
            }

            return result;
        }
    }
}