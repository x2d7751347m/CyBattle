using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object.Synchronizing;
using JetBrains.Annotations;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text MinutesText;
    public TMP_Text SecondsText;
    public int Minutes = 5;
    public int Seconds = 0;

    public void BeginCounting()
    {
        CancelInvoke();
        InvokeRepeating(nameof(TimeCountDown), 1, 1);
    }

    private void TimeCountDown()
    {
        switch (Seconds)
        {
            case > 10:
                Seconds -= 1;
                SecondsText.text = Seconds.ToString();
                break;
            case > 0:
                Seconds -= 1;
                SecondsText.text = "0" + Seconds;
                break;
            case 0 when Minutes > 0:
                Minutes -= 1;
                Seconds = 59;
                MinutesText.text = Minutes.ToString();
                SecondsText.text = Seconds.ToString();
                break;
        }
    }
}
