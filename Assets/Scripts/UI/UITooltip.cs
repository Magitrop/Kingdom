using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    public RectTransform rectTransform;
    [SerializeField]
    private Text tooltipText;

    public void SetTooltipText(string newText)
    {
        tooltipText.text = newText;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tooltipText.preferredWidth + 50);
    }

    /*
    void Update()
    {
        transform.position = Input.mousePosition + new Vector3(10, -10);
    }
    */
}