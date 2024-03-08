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
    private int _weaponType = 1;
    [SerializeField]
    private GameObject _spawnPoint;
    [SerializeField]
    private GameObject _spawnManager;
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
        if (other.CompareTag("Player"))
        {
            PlayPickupAudioServer();
            TurnOffServer();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayPickupAudioServer()
    {
        PlayPickupAudio();
    }

    [ObserversRpc]
    void PlayPickupAudio()
    {
        if (_weaponType == 1)
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
