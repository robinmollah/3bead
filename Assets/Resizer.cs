using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resizer : MonoBehaviour
{
    void Start()
    {
        float nScale = 3f;
        float cWidth = Screen.width;
        float cDpi = 416;
        if(Screen.orientation == ScreenOrientation.Portrait)
        {
            nScale = (cWidth / 250f) / (cDpi / 160f);
        }
        transform.localScale = new Vector3(nScale, nScale, 1);
    }

    void Update()
    {
        
    }
}
