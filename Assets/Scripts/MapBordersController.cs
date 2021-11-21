using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapBordersController : MonoBehaviour
{
#if (UNITY_EDITOR)
    public MapController map;
    public TextMesh leftUpper, rightUpper, rightBottom;
    public LineRenderer line;

    private void Start()
    {
        line.positionCount = 5;
    }

    private void Update()
    {
        if (EditorApplication.isPlaying == false)
        {
            if (leftUpper != null && rightBottom != null && rightUpper != null)
            {
                rightUpper.transform.position = new Vector3((float)Math.Truncate(rightUpper.transform.position.x) + 0.5f, (float)Math.Truncate(rightUpper.transform.position.y) + 0.5f, -1f);
                leftUpper.transform.position = new Vector3(0.5f, (float)Math.Truncate(rightUpper.transform.position.y) - 0.5f, -1f);
                rightBottom.transform.position = new Vector3((float)Math.Truncate(rightUpper.transform.position.x) - 0.5f, 0.5f, -1f);

                rightUpper.text = "(" + (rightUpper.transform.position.x - 1.5f) + "; " + (rightUpper.transform.position.y - 1.5f) + ")";
                leftUpper.text = "(" + (leftUpper.transform.position.x - 0.5f) + "; " + (leftUpper.transform.position.y - 0.5f) + ")";
                rightBottom.text = "(" + (rightBottom.transform.position.x - 0.5f) + "; " + (rightBottom.transform.position.y - 0.5f) + ")";

                map.mapSizeX = (int)(rightUpper.transform.position.x - 1.5f);
                map.mapSizeY = (int)(rightUpper.transform.position.y - 1.5f);

                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, leftUpper.transform.position + new Vector3(-0.5f, 0.5f, -1f));
                line.SetPosition(2, rightUpper.transform.position + new Vector3(-0.5f, -0.5f, -1f));
                line.SetPosition(3, rightBottom.transform.position + new Vector3(0.5f, -0.5f, -1f));
                line.SetPosition(4, Vector3.zero);
            }
        }
    }
#endif
}