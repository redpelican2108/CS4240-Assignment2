using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialReplacer : MonoBehaviour
{
    private Material[] originalMaterials;
    private Renderer[] renderers;

    void Awake()
    {
        // Store a reference to the original materials
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }
    }
    
    public void ResetMaterials()
    {
        // Reset the materials to their original state
        ReplaceMaterials(originalMaterials);
    }
    
    public void ReplaceMaterials(Material newMaterial)
    {
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = newMaterial;
            }
            renderer.materials = materials;
        }
    }
    
    private void ReplaceMaterials(Material[] newMaterials)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = newMaterials[i];
        }
    }
}
