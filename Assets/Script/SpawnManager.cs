using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Script
{
    public class SpawnManager : MonoBehaviour
    {
        public Transform hostSpawnPoint;
        public Transform clientSpawnPoint;

        void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            var client = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (client == null) return;

            bool isHost = clientId == NetworkManager.Singleton.LocalClientId;
            client.transform.position = isHost
                ? hostSpawnPoint.position
                : clientSpawnPoint.position;
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}
