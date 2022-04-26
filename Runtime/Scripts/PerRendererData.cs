using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wellmor.PerRendererData
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class PerRendererData : MonoBehaviour
    {
        [Serializable]
        private enum ApplyEvent
        {
            Awake,
            Start,
            Manual
        }
        
        [SerializeField]
        private ApplyEvent _event = ApplyEvent.Awake;

        [SerializeField]
        private Renderer _renderer;

        [SerializeReference]
        private List<PropertyData> _properties = new List<PropertyData>();

        public List<PropertyData> Properties => _properties;

        private void Awake()
        {
            if (_event == ApplyEvent.Awake) Apply();
        }

        private void Start()
        {
            if (_event == ApplyEvent.Start) Apply();
        }
        
        private void OnValidate()
        {
            Apply();
        }

        private void Reset()
        {
            if (_renderer != null) _renderer.SetPropertyBlock(new MaterialPropertyBlock());
        }

        public void Apply()
        {
            if (_properties.Count < 1) return;
            
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_renderer == null) return;
            
            var block = new MaterialPropertyBlock();

            _renderer.GetPropertyBlock(block);
            
            for (var i = 0; i < _properties.Count; i++)
            {
                _properties[i].Set(block);
            }

            _renderer.SetPropertyBlock(block);
        }
    }
}