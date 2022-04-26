using System;
using UnityEngine;

namespace Wellmor.PerRendererData
{
    [Serializable]
    public abstract class PropertyData : IEquatable<PropertyData>
    {
        [SerializeField]
        private string _name;
        public string Name => _name;
        
        protected PropertyData(string name) => _name = name;
        public abstract void Set(MaterialPropertyBlock block);

        public bool Equals(PropertyData other)
        {
            if (other == null || string.IsNullOrWhiteSpace(other._name)) return false;
            return _name == other._name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PropertyData)obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public static bool operator ==(PropertyData left, PropertyData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyData left, PropertyData right)
        {
            return !Equals(left, right);
        }
    }

    [Serializable]
    public class FloatPropertyData : PropertyData
    {
        public FloatPropertyData(string name, float value) : base(name) => _value = value;

        [SerializeField]
        private float _value = default;

        public override void Set(MaterialPropertyBlock block) => block.SetFloat(Name, _value);
    }

    [Serializable]
    public class ColorPropertyData : PropertyData
    {
        [SerializeField]
        private Color _value = default;

        public ColorPropertyData(string name, Color value) : base(name) => _value = value;
        
        public override void Set(MaterialPropertyBlock prop) => prop.SetColor(Name, _value);
    }

    [Serializable]
    public class VectorPropertyData : PropertyData
    {
        [SerializeField]
        private Vector4 _value = default;
        
        public VectorPropertyData(string name, Vector4 value) : base(name) => _value = value;
        
        public override void Set(MaterialPropertyBlock prop) => prop.SetVector(Name, _value);
    }

    [Serializable]
    public class MatrixPropertyData : PropertyData
    {
        [SerializeField]
        private Matrix4x4 _value = Matrix4x4.identity;
        
        public MatrixPropertyData(string name, Matrix4x4 value) : base(name) => _value = value;
        
        public override void Set(MaterialPropertyBlock prop) => prop.SetMatrix(Name, _value);
    }

    [Serializable]
    public class IntPropertyData : PropertyData
    {
        [SerializeField]
        private int _value = default;
        
        public IntPropertyData(string name, int value) : base(name) => _value = value;

        public override void Set(MaterialPropertyBlock prop) => prop.SetInt(Name, _value);
    }

    [Serializable]
    public class TexturePropertyData : PropertyData
    {
        [SerializeField]
        private Texture _value = default;

        [SerializeField]
        private Vector4 _scaleOffset = default;

        [SerializeField]
        private bool _useScaleOffset = false;
        
        public TexturePropertyData(string name, Texture value) : base(name) => _value = value;

        public TexturePropertyData(string name, Texture value, Vector4 scaleOffset) : base(name)
        {
            _value = value;
            _scaleOffset = scaleOffset;
            _useScaleOffset = true;
        }
        
        public override void Set(MaterialPropertyBlock block)
        {
            if (_value != null) block.SetTexture(Name, _value);
            if (_useScaleOffset) block.SetVector(Name + "_ST", _scaleOffset);
        }
    }
}