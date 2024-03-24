using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DisplayColor : NetworkBehaviour
{
    public Color32[] Colors;

    private GameObject _namesObject;

    private int? _namesAt;
    public override void OnStartClient()
    {
        base.OnStartClient();
        _namesObject = GameObject.Find("namesBG");
        if (!IsOwner)
        {
            gameObject.GetComponent<DisplayColor>().enabled = false;
        }
        else
        {
            var gameManager = InstanceFinder.NetworkManager.GetComponent<GameManager>();
            FetchNamesServer();
            ChooseColor(gameManager.PlayerNickName, gameManager.PlayerColor);
        }
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        _namesObject = GameObject.Find("namesBG");
    }

    private void ChooseColor(string nickname, int playerColor)
    {
        AssignColor(playerColor);
        AssignColorServer(nickname, playerColor);
    }

    [ServerRpc]
    private void AssignColorServer(string nickname, int playerColor)
    {
        FillNames(nickname, playerColor);
        FillNamesObserver(nickname, playerColor);
        AssignColorObserver(playerColor);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true, BufferLast = true)]
    private void AssignColorObserver(int playerColor)
    {
        AssignColor(playerColor);
    }

    private void AssignColor(int playerColor)
    {
        transform.GetChild(1).GetComponent<Renderer>().material.color = Colors[playerColor];
    }

    [ObserversRpc]
    private void FillNamesObserver(string nickname, int playerColor)
    {
        FillNames(nickname, playerColor);
    }

    private void FillNames(string nickname, int playerColor)
    {
        for (var i = 0; i < 6; i++)
        {
            if (_namesObject.GetComponent<NickNamesScript>().Names[i].gameObject.activeSelf) continue;
            _namesAt = i;
            _namesObject.GetComponent<NickNamesScript>().Names[i].gameObject.SetActive(true);
            _namesObject.GetComponent<NickNamesScript>().HealthBars[i].gameObject.SetActive(true);
            _namesObject.GetComponent<NickNamesScript>().Names[i].text = nickname;
            _namesObject.GetComponent<NickNamesScript>().HealthBars[i].color = Colors[playerColor];
            var color = _namesObject.GetComponent<NickNamesScript>().HealthBars[i].color;
            color.a = 255f;
            _namesObject.GetComponent<NickNamesScript>().HealthBars[i].color = color;
            break;
        }
    }

    [ServerRpc]
    private void FetchNamesServer()
    {
        if (_namesObject.GetComponent<NickNamesScript>().Names.Any(tmpText => tmpText.enabled))
        {
            FetchNames(Owner, _namesObject.GetComponent<NickNamesScript>().Names.Select(a =>
                a.text).ToArray(), _namesObject.GetComponent<NickNamesScript>().HealthBars.Select(a => a.color).ToArray());
        }
    }

    [TargetRpc]
    private void FetchNames(NetworkConnection conn, string[] names, Color[] healthBars)
    {
        for (var i = 0; i < 6; i++)
        {
            if (names[i] == "") continue;
            _namesObject.GetComponent<NickNamesScript>().Names[i].gameObject.SetActive(true);
            _namesObject.GetComponent<NickNamesScript>().HealthBars[i].gameObject.SetActive(true);
            _namesObject.GetComponent<NickNamesScript>().Names[i].text = names[i];
            _namesObject.GetComponent<NickNamesScript>().HealthBars[i].color = healthBars[i];
        }
    }

    private void OnDestroy()
    {
        if (!_namesAt.HasValue) return;
        _namesObject.GetComponent<NickNamesScript>().Names[_namesAt.Value].gameObject.SetActive(false);
        _namesObject.GetComponent<NickNamesScript>().HealthBars[_namesAt.Value].gameObject.SetActive(false);
        _namesObject.GetComponent<NickNamesScript>().Names[_namesAt.Value].text = "";
        _namesObject.GetComponent<NickNamesScript>().HealthBars[_namesAt.Value].gameObject.SetActive(false);
    }
}
