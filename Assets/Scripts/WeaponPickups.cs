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
    public override void OnStartServer()
    {
        base.OnStartServer();
        _audioPlayer = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        yield return new WaitForSeconds(3.0f);
        ServerManager.Despawn(_weapon);
    }

}
