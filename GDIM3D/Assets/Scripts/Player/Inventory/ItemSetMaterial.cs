using System;
using UnityEngine;

public class ItemSetMaterial : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private ItemData item;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        item = GetComponentInParent<Item>().itemData;
        
    }

    private void Start()
    {
        Material mat = new Material(meshRenderer.sharedMaterial);
        mat.mainTexture = item.itemSprite.texture;
        meshRenderer.material = mat;
    }
}
