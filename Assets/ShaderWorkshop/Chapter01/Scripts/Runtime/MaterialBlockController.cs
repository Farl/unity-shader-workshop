using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderWorkshop
{
    [ExecuteAlways]
    public class MaterialBlockController : MonoBehaviour
    {
        public enum PropertyType
        {
            Vector,
            Float,
            Color,
            World_Position,
        }

        public enum UpdateMethod
        {
            Enable,
            Update,
        }

        [System.Serializable]
        public class Data
        {
            public UpdateMethod updateMethod;
            public string propertyName;
            public PropertyType propertyType;
            public Vector4 vectorValue;
            public float floatValue;
            public Color colorValue;
            public Transform targetTransform;
        }

        [SerializeField] private bool showDebugInfo;
        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        [SerializeReference, SerializeField] private Data[] datas = new Data[0];

        private Dictionary<Renderer, MaterialPropertyBlock> materialBlocks = new Dictionary<Renderer, MaterialPropertyBlock>();

        private void OnValidate()
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i] == null)
                {
                    datas[i] = new Data();
                }
            }
        }

        public void CollectRenderers()
        {
            var rendererArray = GetComponentsInChildren<Renderer>();
            renderers.Clear();
            renderers.AddRange(rendererArray);
        }

        private void UpdateMaterialBlock(UpdateMethod updateMethod)
        {
            for (var i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (data == null)
                {
                    datas[i] = new Data();
                    continue;
                }
                if (data.updateMethod != updateMethod || string.IsNullOrEmpty(data.propertyName))
                {
                    continue;
                }
                if (data.propertyType == PropertyType.World_Position && data.targetTransform == null)
                {
                    Debug.LogError($"Target Transform ({data.propertyName}) is null", this);
                    continue;
                }

                foreach (var renderer in renderers)
                {
                    if (renderer == null)
                    {
                        continue;
                    }

                    if (materialBlocks.TryGetValue(renderer, out var materialBlock) == false)
                    {
                        materialBlock = new MaterialPropertyBlock();
                        materialBlocks.Add(renderer, materialBlock);
                    }
                    renderer.GetPropertyBlock(materialBlock);
                    switch (data.propertyType)
                    {
                        case PropertyType.Vector:
                            materialBlock.SetVector(data.propertyName, data.vectorValue);
                            break;
                        case PropertyType.Float:
                            materialBlock.SetFloat(data.propertyName, data.floatValue);
                            break;
                        case PropertyType.Color:
                            materialBlock.SetColor(data.propertyName, data.colorValue);
                            break;
                        case PropertyType.World_Position:
                            materialBlock.SetVector(data.propertyName, data.targetTransform.position);
                            break;
                    }
                    renderer.SetPropertyBlock(materialBlock);
                }
            }
        }

        void OnEnable()
        {
            UpdateMaterialBlock(UpdateMethod.Enable);
        }

        void OnDisable()
        {

        }

        void Update()
        {
            UpdateMaterialBlock(UpdateMethod.Update);
        }

        private void OnDestroy()
        {
            for (var i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (data == null)
                {
                    continue;
                }
                foreach (var kvp in materialBlocks)
                {
                    kvp.Value.Clear();
                    kvp.Key.SetPropertyBlock(null);
                }
            }
        }
    }
}
