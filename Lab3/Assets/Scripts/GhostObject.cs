using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObject : MonoBehaviour
{
    public Material ghostMaterial;
    public Material errorMaterial;

    private MaterialReplacer materialReplacer;
    private int collisionCount;

    void Start()
    {
        collisionCount = 0;
        materialReplacer = GetComponent<MaterialReplacer>();
        materialReplacer.ReplaceMaterials(ghostMaterial);
    }

    public bool IsPlaceable()
    {
        return collisionCount <= 0;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "PlaceableObject")
        {
            collisionCount++;
            //print(gameObject.name + " collided with " + col.gameObject.name + " collision count: " + collisionCount);
            if (!IsPlaceable())
            {
                materialReplacer.ReplaceMaterials(errorMaterial);
            }
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "PlaceableObject")
        {
            collisionCount--;
            //print(gameObject.name + " stopped colliding with " + col.gameObject.name + " collision count: " + collisionCount);
            if (IsPlaceable())
            {
                materialReplacer.ReplaceMaterials(ghostMaterial);
            }
        }
    }
}
