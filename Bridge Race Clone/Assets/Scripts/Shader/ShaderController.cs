using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.Shader
{
    public class ShaderController : MonoBehaviour
    {
        [SerializeField] private Material _outlineMaterial;
        [SerializeField] private float _outlineScaleFactor;
        [SerializeField] private Color _outlineColor;

        private Renderer _outlineRenderer;

        private void Start()
        {
            _outlineRenderer = CreateOutline(_outlineMaterial, _outlineScaleFactor, _outlineColor);
            _outlineRenderer.enabled = true;
        }

        Renderer CreateOutline(Material outlineMaterial, float scaleFactor, Color outlinecolor)
        {
            GameObject outlineObject = Instantiate(this.gameObject, transform.position, transform.rotation, transform);
            Renderer rend = outlineObject.GetComponent<Renderer>();

            rend.material = outlineMaterial;
            rend.material.SetColor("_OutlineColor", outlinecolor);
            rend.material.SetFloat("_Scale", scaleFactor);
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            outlineObject.GetComponent<ShaderController>().enabled = false;
            //outlineObject.GetComponent<Collider>().enabled = false;

            rend.enabled = false;

            return rend;
        }
    }    
}
