using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Selectable : MonoBehaviour
{
    // Start is called before the first frame update
    private Color startColor;
    void Start()
    {
        startColor = GetComponent<Renderer>().materials[0].color;
    }

    public void Select(Color color)
    {
        GetComponent<Renderer>().materials[0].color = color; 
    }

    public void Deselect()
    {
        GetComponent<Renderer>().materials[0].color = startColor;
    }
}
