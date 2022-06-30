// Copyright (c) 2020 JioGlass. All Rights Reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JMRSliderTint : MonoBehaviour
{
    private Image _Renderer;
    public Color Color;
    
    void Update()
    {
        _Renderer = this.GetComponent<Image>();  
        _Renderer.material.SetColor("_Color", Color);
    }
}
