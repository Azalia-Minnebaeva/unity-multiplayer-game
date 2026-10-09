using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }

    [Header("API Settings")]
    [SerializeField] private string baseUrl = "http://localhost:5231";

    public string AuthToken { get; private set; }
    public int UserID { get; private set; }
    public string Username { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task<bool> SignUp(string username, string password)
    {
        var json = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
        using (var request = new UnityWebRequest(baseUrl + "/auth/signup", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<SimpleResponse>(request.downloadHandler.text);
                if (response != null && response.status)
                {
                    return await SignIn(username, password);
                }
            }
            Debug.LogError($"SignUp error: {request.downloadHandler.text}");
            return false;
        }
    }

    public async Task<bool> SignIn(string username, string password)
    {
        var json = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
        using (var request = new UnityWebRequest(baseUrl + "/auth/signin", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<SignInResponse>(request.downloadHandler.text);
                if (response != null && response.status)
                {
                    AuthToken = response.token;
                    UserID = response.userID;
                    Username = response.username;
                    Debug.Log($"Успешный вход: {Username}, токен {AuthToken}");
                    return true;
                }
            }
            Debug.LogError($"SignIn error: {request.downloadHandler.text}");
            return false;
        }
    }

    public async Task<string> GetLevels()
    {
        if (string.IsNullOrEmpty(AuthToken))
        {
            Debug.LogError("Нет токена авторизации");
            return null;
        }

        using (var request = UnityWebRequest.Get(baseUrl + "/levels/getall"))
        {
            request.SetRequestHeader("Authorization", "Bearer " + AuthToken);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                return request.downloadHandler.text;
            else
                Debug.LogError($"GetLevels error: {request.downloadHandler.text}");
            return null;
        }
    }

    [System.Serializable]
    private class SimpleResponse
    {
        public bool status;
        public string message;
    }

    [System.Serializable]
    private class SignInResponse
    {
        public bool status;
        public string token;
        public int userID;
        public string username;
    }
}
