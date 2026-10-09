using Unity.Netcode;
using UnityEngine;
using TMPro;

// Повесь на пустой GameObject в КАЖДОЙ сцене уровня (например "LevelManagerObject").
// Обязательно добавь на этот же объект компонент NetworkObject —
// Netcode автоматически заспавнит его как объект сцены при старте.
public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    [Header("Настройки уровня")]
    [Tooltip("Сколько всего предметов нужно собрать на этом уровне")]
    public int totalCollectibles = 5;

    [Header("Таймер (необязательно)")]
    [Tooltip("Если больше 0 — включается обратный отсчёт. Если время истекло, а не всё собрано — уровень провален")]
    public float timeLimit = 0f;
    private NetworkVariable<float> timeLeft = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("UI")]
    public TextMeshProUGUI counterText; // покажет "2 / 5"
    public TextMeshProUGUI timerText;   // покажет "01:23", можно оставить пустым

    [Header("Дверь выхода")]
    public LevelExitDoor exitDoor;

    private NetworkVariable<int> collectedCount = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool levelFailed = false;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        collectedCount.OnValueChanged += OnCountChanged;
        timeLeft.OnValueChanged += OnTimeChanged;

        UpdateCounterUI(collectedCount.Value);
        UpdateTimerUI(timeLeft.Value);

        if (IsServer && timeLimit > 0f)
            timeLeft.Value = timeLimit;
    }

    private void Update()
    {
        // Таймер тикает только на сервере, клиенты просто отображают синхронизированное значение
        if (!IsServer) return;
        if (timeLimit <= 0f || levelFailed) return;
        if (collectedCount.Value >= totalCollectibles) return; // уровень уже пройден, таймер не нужен

        if (timeLeft.Value > 0f)
        {
            timeLeft.Value = Mathf.Max(0f, timeLeft.Value - Time.deltaTime);
            if (timeLeft.Value <= 0f)
            {
                levelFailed = true;
                OnTimeUp();
            }
        }
    }

    private void OnCountChanged(int oldVal, int newVal)
    {
        UpdateCounterUI(newVal);

        if (IsServer && newVal >= totalCollectibles && exitDoor != null)
            exitDoor.Open();
    }

    private void OnTimeChanged(float oldVal, float newVal) => UpdateTimerUI(newVal);

    private void UpdateCounterUI(int value)
    {
        if (counterText != null)
            counterText.text = $"{value} / {totalCollectibles}";
    }

    private void UpdateTimerUI(float value)
    {
        if (timerText == null || timeLimit <= 0f) return;
        int minutes = Mathf.FloorToInt(value / 60f);
        int seconds = Mathf.FloorToInt(value % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Вызывается сервером из CollectibleItem при подборе
    public void AddCollected()
    {
        if (!IsServer) return;
        collectedCount.Value++;
    }

    // Переопредели/дополни под свой геймдизайн: рестарт уровня, экран поражения и т.п.
    private void OnTimeUp()
    {
        Debug.Log("Время вышло! Уровень провален.");
        // TODO: например SceneManager.LoadScene(текущая сцена) через NetworkManager,
        // или показать UI поражения через ClientRpc
    }

    public override void OnDestroy()
    {
        collectedCount.OnValueChanged -= OnCountChanged;
        timeLeft.OnValueChanged -= OnTimeChanged;
    }
}