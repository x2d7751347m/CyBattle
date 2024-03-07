using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine.Animations.Rigging;

public class WeaponSpawnObject : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] _objWeaponsToSpawn;
    [SerializeField]
    private GameObject[] _spawnedWeaponObjects;
    [SerializeField]
    private GameObject[] _spawnPoint;
    [SerializeField]
    private float _canSpawn = 0, _spawnRate = 30;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            gameObject.GetComponent<WeaponSpawnObject>().enabled = false;
        }
        else
        {
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > _canSpawn)
        {
            _canSpawn = Time.time + _spawnRate;
            GameObject weaponSpawned = Instantiate(_objWeaponsToSpawn[Random.Range(0, _objWeaponsToSpawn.Length)], _spawnPoint[Random.Range(0, _spawnPoint.Length)].transform.position, Quaternion.identity);
            ServerManager.Spawn(weaponSpawned);
        }
    }

    [ServerRpc]
    void SpawnWeapon()
    {
        _canSpawn = Time.time + _spawnRate;
        GameObject weaponSpawned = Instantiate(_objWeaponsToSpawn[Random.Range(0, _objWeaponsToSpawn.Length)], _spawnPoint[Random.Range(0, _spawnPoint.Length)].transform.position, Quaternion.identity);
        ServerManager.Spawn(weaponSpawned);
        SetSpawnWeapon(weaponSpawned, this);
    }

    [ObserversRpc]
    void SetSpawnWeapon(GameObject spawned, WeaponSpawnObject script)
    {
        //script._spawnedWeaponObjects[_spawnedWeaponObjects.Length] = spawned;
    }
}
