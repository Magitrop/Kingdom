using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFloatingText : MonoBehaviour
{
    public Vector2 floatDirection;
    public float floatSpeed;
    public float fadeSpeed;

    public TextMesh textComponent;

    private void Start()
    {
        textComponent = GetComponent<TextMesh>();
    }

    void Update()
    {
        transform.position += (Vector3)floatDirection * floatSpeed * Time.deltaTime;
        if (textComponent.color.a >= 0.025f)
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, textComponent.color.a - fadeSpeed * Time.deltaTime);
        else Destroy(gameObject);
    }
}