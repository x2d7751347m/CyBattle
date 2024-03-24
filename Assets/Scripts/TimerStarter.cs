using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using JetBrains.Annotations;
using TMPro;

public class TimerStarter : NetworkBehaviour
{
    private Timer _timer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            _timer = GameObject.Find("namesBG").GetComponent<Timer>();
            SetCountServer();
            BeginTimer();
        }
        else
        {
            gameObject.GetComponent<TimerStarter>().enabled = false;
        }
    }

    private void Start()
    {
        _timer = GameObject.Find("namesBG").GetComponent<Timer>();
    }

    public void BeginTimer()
    {
        Count();
    }

    [ServerRpc]
    private void Count()
    {
        if (_timer.Minutes is 0 or 5 && _timer.Seconds == 0)
        {
            _timer.Minutes = 5;
            _timer.Seconds = 0;
            _timer.BeginCounting();
            BeginCountingTarget(Owner);
        }
        else
        {
            BeginCountingTarget(Owner);
        }
    }
    
    [ServerRpc]
    private void SetCountServer()
    {
        SetCount(Owner, _timer.Minutes, _timer.Seconds);
    }
    
    [TargetRpc]
    private void SetCount(NetworkConnection conn, int minutes, int seconds)
    {
        _timer.Minutes = minutes;
        _timer.Seconds = seconds;
        _timer.MinutesText.text = minutes.ToString();
        if (seconds > 9)
        {
            _timer.SecondsText.text = seconds.ToString();
        }
        else
        {
            _timer.SecondsText.text = "0" + seconds;
        }
    }
    
    [TargetRpc]
    private void BeginCountingTarget(NetworkConnection conn)
    {
        _timer.BeginCounting();
    }
}
