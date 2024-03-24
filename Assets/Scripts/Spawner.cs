using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Component.Spawning;
using FishNet.Managing;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public class SpawnController : MonoBehaviour
    {
        private NetworkManager _networkManager;
        private readonly List<Transform> _spawnPoints = new();
        [SerializeField]
        private bool _isCurrentScene;

        private void Awake()
        {
            _networkManager = InstanceFinder.NetworkManager;
            for (var i = 0; i < transform.childCount; ++i)
            {
                _spawnPoints.Add(transform.GetChild(i).transform);
            }

            if (_isCurrentScene)
            {
                _networkManager.GetComponent<PlayerSpawner>().Spawns = _spawnPoints.ToArray();
            }
        }

        public Transform GetRandomSpawnPoint()
        {
            var randomIdx = Random.Range(0, _spawnPoints.Count);
            return _spawnPoints[randomIdx];   
        }
    }
}
