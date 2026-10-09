using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    public static string PlayerRole { get; private set; }

    public GameObject hostPlayerPrefab;
    public GameObject clientPlayerPrefab;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    public void SetRoleAndLoadLevelSelection(string role)
    {
        PlayerRole = role;
        SceneManager.LoadScene("LevelSelectionFour");
    }

    public void LoadLevel(string levelName)
    {
        if (string.IsNullOrEmpty(PlayerRole)) { Debug.LogError("Роль не выбрана!"); return; }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(levelName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (!scene.name.StartsWith("Level")) return;

        var nm = NetworkManager.Singleton;
        if (nm.IsListening) return;

        if (PlayerRole == "Host")
        {
            nm.ConnectionApprovalCallback = ApproveConnection;
            nm.OnClientConnectedCallback += OnClientConnected;
            nm.StartHost();
        }
        else
        {
            nm.StartClient();
        }
    }

    private void ApproveConnection(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false; 
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        bool isHost = clientId == NetworkManager.Singleton.LocalClientId;

        GameObject prefab = isHost ? hostPlayerPrefab : clientPlayerPrefab;

        var spawnName = isHost ? "SpawnPoint_Host" : "SpawnPoint_Client";
        var spawnObj = GameObject.Find(spawnName);
        var spawnPos = spawnObj != null ? spawnObj.transform.position : Vector3.zero;

        var player = Instantiate(prefab, spawnPos, Quaternion.identity);
        var netObj = player.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId); 

        Debug.Log($"Заспавнен игрок для clientId={clientId}, isHost={isHost}");
    }
}