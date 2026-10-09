using UnityEngine;
using TMPro;

public class ScarecrowInteract : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public string message = "Осторожно! Здесь опасно...";

    [Header("Аудио")]
    public AudioClip clip;
    private AudioSource audioSource;

    [Header("Плавность")]
    public float fadeSpeed = 2f;
    private float targetAlpha = 0f;
    private bool playerNear = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
            SetAlpha(0f);
        }
    }

    void Update()
    {
        PlayerController2D player = FindAnyObjectByType<PlayerController2D>();
        if (player == null) return;

        // Плавно меняем прозрачность текста
        if (dialogueText != null)
        {
            float currentAlpha = dialogueText.color.a;
            float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            SetAlpha(newAlpha);
        }

        // Плавно меняем громкость звука
        if (audioSource != null)
        {
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume, targetAlpha, fadeSpeed * Time.deltaTime);

            if (playerNear && !audioSource.isPlaying && clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    void SetAlpha(float alpha)
    {
        Color c = dialogueText.color;
        c.a = alpha;
        dialogueText.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = true;
        targetAlpha = 1f;
        dialogueText.text = message;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNear = false;
        targetAlpha = 0f;
    }
}