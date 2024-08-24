using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System;

public class WeaponPickups : NetworkBehaviour
{
    [SerializeField]
    private AudioSource _audioPlayer;
    [SerializeField]
    private GameObject _weapon;
    [SerializeField]
    private bool _consumable = true;
    [SerializeField]
    private int _weaponNumber;
    [SerializeField]
    private int _amount = 50;
    [SerializeField]
    private GameObject _spawnPoint;
    [SerializeField]
    private GameObject _spawnManager;
    private bool _consumed;
    [SerializeField]
    private MeshRenderer _mesh;
    [SerializeField]
    private GameObject _meshObject;
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    // Start is called before the first frame update
    void Start()
    {
        _audioPlayer = GetComponent<AudioSource>();
        _spawnManager = GameObject.Find("SpawnManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpawnPoint(GameObject point)
    {
        _spawnPoint = point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _consumed) return;
        _consumed = true;
        if (_mesh != null)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            _meshObject.SetActive(false);
        }
        AddAmmo(other.GetComponent<WeaponChange>());
        PlayPickupAudioServer();
        TurnOffServer();
    }

    void AddAmmo(WeaponChange weapon)
    {
        weapon.AddAmmo(_weaponNumber, _amount);
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayPickupAudioServer()
    {
        PlayPickupAudio();
    }

    [ObserversRpc]
    void PlayPickupAudio()
    {
        if (_consumable)
        {
            Destroy(GetComponent<Renderer>());
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        Destroy(GetComponent<Collider>());
        _audioPlayer.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    void TurnOffServer()
    {
        StartCoroutine(TurnOffRoutine());
    }

    IEnumerator TurnOffRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        _spawnManager.GetComponent<WeaponSpawnObject>().spawnPoint.Add(_spawnPoint);
        ServerManager.Despawn(_weapon);
    }

}
