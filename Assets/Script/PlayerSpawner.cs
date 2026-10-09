using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject player1Prefab; // рыцарь
    public GameObject player2Prefab; // маг

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Спавним всех подключившихся игроков
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Спавним хоста сразу
        SpawnPlayer(NetworkManager.Singleton.LocalClientId);
    }

    void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        SpawnPlayer(clientId);
    }

    private System.Collections.Generic.HashSet<ulong> spawnedClients = new();

    void SpawnPlayer(ulong clientId)
    {
        if (spawnedClients.Contains(clientId)) return;
        spawnedClients.Add(clientId);

        bool isHost = clientId == NetworkManager.Singleton.LocalClientId && IsHost;
        GameObject prefab = isHost ? player1Prefab : player2Prefab;
        Transform spawnPoint = isHost ? spawnPoint1 : spawnPoint2;

        GameObject player = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}