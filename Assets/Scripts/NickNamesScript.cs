using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NickNamesScript : MonoBehaviour
{
    public TMP_Text[] Names;

    public Image[] HealthBars;

    private void Start()
    {
        for (var i = 0; i < Names.Length; i++)
        {
            Names[i].text = "";
            Names[i].gameObject.SetActive(false);
            HealthBars[i].gameObject.SetActive(false);
        }
    }
}
