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
    public List<GameObject> spawnPoint;
    [SerializeField]
    private float _spawnRate = 30;
    public override void OnStartClient()
    {
        base.OnStartClient();
        gameObject.GetComponent<WeaponSpawnObject>().enabled = false;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        StartSpawning();
    }
    
    public void StartSpawning()
    {
        StartCoroutine(SpawnWeaponRoutine());
    }

    IEnumerator SpawnWeaponRoutine()
    {
        while (true)
        {
            var spawnPointNumber = Random.Range(0, spawnPoint.Count);
            GameObject weaponSpawned = Instantiate(_objWeaponsToSpawn[Random.Range(0, _objWeaponsToSpawn.Length)], spawnPoint[spawnPointNumber].transform.position, Quaternion.identity);
            ServerManager.Spawn(weaponSpawned);
            weaponSpawned.GetComponent<WeaponPickups>().SetSpawnPoint(spawnPoint[spawnPointNumber]);
            spawnPoint.RemoveAt(spawnPointNumber);
            yield return new WaitForSeconds(_spawnRate);
        }
    }

    [ServerRpc]
    void SpawnWeapon()
    {
        var spawnPointNumber = Random.Range(0, spawnPoint.Count);
        GameObject weaponSpawned = Instantiate(_objWeaponsToSpawn[Random.Range(0, _objWeaponsToSpawn.Length)], spawnPoint[spawnPointNumber].transform.position, Quaternion.identity);
        ServerManager.Spawn(weaponSpawned);
        weaponSpawned.GetComponent<WeaponPickups>().SetSpawnPoint(spawnPoint[spawnPointNumber]);
        spawnPoint.RemoveAt(spawnPointNumber);
    }
}
