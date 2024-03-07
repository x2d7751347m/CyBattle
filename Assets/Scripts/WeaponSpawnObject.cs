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
    private List<GameObject> _spawnPoint;
    [SerializeField]
    private List<GameObject> _spawnedPoints;
    [SerializeField]
    private float _canSpawn = 0, _spawnRate = 30;
    public override void OnStartClient()
    {
        base.OnStartClient();
        gameObject.GetComponent<WeaponSpawnObject>().enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > _canSpawn && _spawnPoint.Count != 0)
        {
            _canSpawn = Time.time + _spawnRate;
            var spawnPointNumber = Random.Range(0, _spawnPoint.Count);
            _spawnedPoints.Add(_spawnPoint[spawnPointNumber]);
            GameObject weaponSpawned = Instantiate(_objWeaponsToSpawn[Random.Range(0, _objWeaponsToSpawn.Length)], _spawnPoint[spawnPointNumber].transform.position, Quaternion.identity);
            _spawnPoint.RemoveAt(spawnPointNumber);
            ServerManager.Spawn(weaponSpawned);
        }
    }

    [ServerRpc]
    void SpawnWeapon()
    {
        _canSpawn = Time.time + _spawnRate;
        var spawnPointNumber = Random.Range(0, _spawnPoint.Count);
        _spawnedPoints.Add(_spawnPoint[spawnPointNumber]);
        GameObject weaponSpawned = Instantiate(_objWeaponsToSpawn[Random.Range(0, _objWeaponsToSpawn.Length)], _spawnPoint[spawnPointNumber].transform.position, Quaternion.identity);
        _spawnPoint.RemoveAt(spawnPointNumber);
        ServerManager.Spawn(weaponSpawned);
    }
}
