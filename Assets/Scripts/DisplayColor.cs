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
using Random = UnityEngine.Random;

public class DisplayColor : NetworkBehaviour
{
    public Color32[] Colors;

    private GameObject _namesObject;

    private int? _namesAt;

    public AudioClip[] GunShotSounds;
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Death = Animator.StringToHash("Death");
    
    public bool IsDead;
    private bool _respawned;

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

    private void Update()
    {
        if (GetComponent<Animator>().GetBool(Hit))
        {
            StartCoroutine(Recover());
        }

        if (IsDead && !_respawned)
        {
            _respawned = true;
            StartCoroutine(RespawnWait());
        }
    }

    public void Respawn()
    {
                GetComponent<Animator>().SetBool(Death, false);
    }

    public void ResetForReplay(string playerName)
    {
        // var myName = InstanceFinder.NetworkManager.GetComponent<GameManager>().PlayerNickName;
        for (var i = 0; i < _namesObject.GetComponent<NickNamesScript>().Names.Length; i++)
        {
            if (playerName != _namesObject.GetComponent<NickNamesScript>().Names[i].text) continue;

            IsDead = false;
            gameObject.GetComponent<PlayerMovement>().IsDead = false;
            gameObject.GetComponent<WeaponChange>().IsDead = false;
            _namesObject.GetComponent<NickNamesScript>().HealthBars[i].gameObject.GetComponent<Image>()
                .fillAmount = 1;
            // na
            if (IsOwner)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    [ServerRpc]
    public void ResetForReplayServer(string playerName)
    {
        ResetForReplayObserver(playerName);
    }

    [ObserversRpc]
    private void ResetForReplayObserver(string playerName)
    {
        ResetForReplay(playerName);
    }
    
    IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.03f);
        if (IsOwner)
        {
            GetComponent<Animator>().SetBool(Hit, false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeliverDamage(string name, float damageAmt)
    {
        // GunDamage(name, damageAmt);
        GunDamageObserver(name, damageAmt);
    }

    [ObserversRpc(ExcludeServer = true)]
    private void GunDamageObserver(string name, float damageAmt)
    {
        GunDamage(name, damageAmt);
    }

    public void GunDamage(string name, float damageAmt)
    {
        for (var i = 0; i < _namesObject.GetComponent<NickNamesScript>().Names.Length; i++)
        {
            if (name != _namesObject.GetComponent<NickNamesScript>().Names[i].text) continue;
            var healthBar = _namesObject.GetComponent<NickNamesScript>().HealthBars[i].gameObject.GetComponent<Image>();
            var damageResult = healthBar.fillAmount - damageAmt;
            if (damageResult>0)
            {
                if (IsOwner)
                {
                    GetComponent<Animator>().SetBool(Hit, true);
                }
                healthBar.fillAmount = damageResult;
            }
            else
            {
                healthBar.fillAmount = 0;
                if (IsOwner)
                {
                    GetComponent<Animator>().SetBool(Death, true);
                }

                gameObject.GetComponent<PlayerMovement>().IsDead = true;
                IsDead = true;
                gameObject.GetComponent<WeaponChange>().IsDead = true;
            }
        }
    }
    

    IEnumerator RespawnWait()
    {
        yield return new WaitForSeconds(3);
        IsDead = false;
        gameObject.GetComponent<PlayerMovement>().IsDead = false;
        gameObject.GetComponent<WeaponChange>().IsDead = false;
        _respawned = false;
        var spawnPoints = GameObject.Find("SpawnPoints");
        transform.position = spawnPoints.transform.GetChild(Random.Range(0, spawnPoints.transform.childCount)).position;
        GetComponent<DisplayColor>().Respawn();
        var myName = InstanceFinder.NetworkManager.GetComponent<GameManager>().PlayerNickName;
        ResetForReplayServer(myName);
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

    public void PlayGunShot(int weaponNumber)
    {
        GetComponent<AudioSource>().clip = GunShotSounds[weaponNumber];
        GetComponent<AudioSource>().Play();
    }

    [ServerRpc]
    public void PlayGunShotServer(int weaponNumber)
    {
        PlayGunShotObserver(weaponNumber);
    }

    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void PlayGunShotObserver(int weaponNumber)
    {
        PlayGunShot(weaponNumber);
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
                    a.text).ToArray(),
                _namesObject.GetComponent<NickNamesScript>().HealthBars.Select(a => a.color).ToArray());
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