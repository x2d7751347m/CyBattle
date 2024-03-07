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
        Destroy(GetComponent<Renderer>());
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
