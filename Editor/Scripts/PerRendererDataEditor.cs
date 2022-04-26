using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Wellmor.PerRendererData.PerRendererDataEditorUtils;

namespace Wellmor.PerRendererData
{
    [CustomEditor(typeof(PerRendererData))]
    public class PerRendererDataEditor : Editor
    {
        private Material[] _materials;
        private Renderer _renderer;
        private PerRendererData _data;

        private SerializedPropertyList<PropertyData> _properties;
        private SerializedProperty _eventProp;

        private void OnEnable()
        {
            _data = target as PerRendererData;
            if (_data == null) return;

            _renderer = _data.GetComponent<Renderer>();
            _materials = _renderer.sharedMaterials;
            _properties = new SerializedPropertyList<PropertyData>(serializedObject.FindProperty(Constants.PropertiesPropertyName));
            _eventProp = serializedObject.FindProperty(Constants.EventNameProp);

            var rendProp = serializedObject.FindProperty(Constants.RendererPropertyName);
            if (rendProp.objectReferenceValue == _renderer) return;

            rendProp.objectReferenceValue = _renderer;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(_eventProp);
                EditorGUILayout.Space(5f);
                
                Print(_properties, out var removeIndex);

                if (_materials.Length > 0)
                {
                    if (GUILayout.Button(new GUIContent("Add")))
                    {
                        DrawAddButton();
                    }
                }

                if (removeIndex >= 0) Remove(removeIndex);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawAddButton()
        {
            GetPropertyArrays(out var instanced, out var common);
            var menu = new GenericMenu();

            Draw(menu, instanced, "Per Renderer");
            Draw(menu, common, "Common");

            menu.ShowAsContext();
        }

        private void Draw(GenericMenu menu, List<MaterialPropertyInfo> info, string header)
        {
            if (info.Count < 1) return;

            menu.AddSeparator(string.Empty);
            menu.AddDisabledItem(new GUIContent(header), false);
            menu.AddSeparator(string.Empty);

            foreach (var p in info)
            {
                menu.AddItem(new GUIContent(p.Name + " | " + p.Type), false, Add, p);
            }
        }

        private void Add(object context)
        {
            var info = (MaterialPropertyInfo)context;
            var data = CreatePropertyData(info.Property);
            if (data == null) return;
            _properties.Add(data);
            serializedObject.ApplyModifiedProperties();
        }

        private void Remove(int index)
        {
            Clear();
            _properties.RemoveAt(index);
            serializedObject.ApplyModifiedProperties();
        }

        private void Clear()
        {
            _renderer.SetPropertyBlock(new MaterialPropertyBlock());
        }

        private void GetPropertyArrays(out List<MaterialPropertyInfo> instanced, out List<MaterialPropertyInfo> common)
        {
            var materialProperties = GetPropertiesInfoFrom(_materials);
            var existPropsNames = _properties.Select(p => p.Name);
            var fullList = materialProperties.FindAll(info => !existPropsNames.Contains(info.Name));

            fullList.Split(out instanced, out common,
                p => (p.Property.flags & MaterialProperty.PropFlags.PerRendererData) != 0 ||
                     p.Attributes.Contains(Constants.AttributeName));
        }

        private static void Print(SerializedPropertyList<PropertyData> list, out int removeIndex)
        {
            removeIndex = -1;
            for (var i = 0; i < list.Count; i++)
            {
                var parent = list.GetRaw(i);
                var nameProp = parent.FindPropertyRelative(Constants.PropertyDataNamePropertyName);
                var valueProp = parent.FindPropertyRelative(Constants.ValuePropertyName);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.PropertyField(valueProp, new GUIContent(nameProp.stringValue));

                        var scaleOffsetProp = parent.FindPropertyRelative(Constants.ScaleOffsetPropertyName);
                        if (scaleOffsetProp != null)
                        {
                            var useScaleOffsetProp = parent.FindPropertyRelative(Constants.UseScaleOffsetPropertyName);
                            if (useScaleOffsetProp != null && useScaleOffsetProp.boolValue)
                            {
                                EditorGUILayout.PropertyField(scaleOffsetProp,
                                    new GUIContent(nameProp.stringValue + "_ST"));
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    if (GUILayout.Button("-", GUILayout.Width(50)))
                    {
                        removeIndex = i;
                        continue;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (list.Count > 0) EditorGUILayout.Space(15);
        }
    }
}