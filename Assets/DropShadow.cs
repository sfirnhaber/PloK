using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DropShadow : MonoBehaviour
{
    public Vector2 ShadowOffset;
    public Material ShadowMaterial;

    SpriteRenderer spriteRenderer;
    GameObject shadowGameObject;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        shadowGameObject = new GameObject("Shadow");

        SpriteRenderer shadowSpriteRenderer = shadowGameObject.AddComponent<SpriteRenderer>();

        shadowSpriteRenderer.sprite = spriteRenderer.sprite;
        shadowSpriteRenderer.material = ShadowMaterial;

        shadowGameObject.transform.localScale = transform.localScale;
        shadowSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    // Update is called once per frame

    private void LateUpdate() {
        shadowGameObject.transform.localPosition = transform.localPosition + (Vector3)ShadowOffset;
        shadowGameObject.transform.localRotation = transform.localRotation;
    }

    void Update()
    {
        
    }
}
