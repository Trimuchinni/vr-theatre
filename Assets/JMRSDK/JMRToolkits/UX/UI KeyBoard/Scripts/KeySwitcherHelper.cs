// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeySwitcherHelper : MonoBehaviour
{
    [SerializeField]
    private GameObject specialKeyHolder, alphabetKeyHolder;

    [SerializeField]
    private Toggle toggleBtn;

    public void KeyToggle(bool state)
    {
        specialKeyHolder.SetActive(state);
        alphabetKeyHolder.SetActive(!state);
        toggleBtn.isOn = state;
    }
}
