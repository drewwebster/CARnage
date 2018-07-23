using UnityEngine;
using System.Collections.Generic;

namespace DestroyIt
{
    public class FadeTreeSwaying : MonoBehaviour
    {
        private List<Material> materials;

        void Start()
        {
            materials = new List<Material>();
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach(MeshRenderer meshRenderer in meshRenderers)
            {
                foreach(Material mat in meshRenderer.materials)
                {
                    if (mat != null && mat.HasProperty("_Sway") && mat.HasProperty("_Wind"))
                        materials.Add(mat);
                }
            }

            if (materials.Count > 0)
                InvokeRepeating("FadeOut", .1f, .1f);
        }

        void FadeOut()
        {
            foreach(Material mat in materials)
            {
                float wind = mat.GetFloat("_Wind");
                float sway = mat.GetFloat("_Sway");
                if (wind > 0)
                {
                    wind -= .005f;
                    if (wind < 0) wind = 0;
                    mat.SetFloat("_Wind", wind);
                }
                if (sway > 0)
                {    
                    sway -= .005f;
                    if (sway < 0) sway = 0;
                    mat.SetFloat("_Sway", sway);
                }
                if (wind <= 0 && sway <= 0)
                    Destroy(this);
            }
        }
    }
}