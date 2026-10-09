using Unity.Netcode;
using UnityEngine;

// Повесь на объект двери. Добавь NetworkObject на этот же объект.
public class LevelExitDoor : NetworkBehaviour
{
    [Header("Компоненты двери")]
    [Tooltip("Коллайдер, который блокирует проход, пока дверь закрыта")]
    public Collider2D doorCollider;
    [Tooltip("Необязательно: если есть анимация открытия двери")]
    public Animator doorAnimator;
    [Tooltip("Запасной вариант, если анимации нет — просто смена спрайта")]
    public SpriteRenderer doorSprite;

    [Header("Спрайты (если без аниматора)")]
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Звук (необязательно)")]
    public AudioClip openSound;

    private NetworkVariable<bool> isOpen = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        isOpen.OnValueChanged += OnOpenChanged;
        ApplyState(isOpen.Value);
    }

    private void OnOpenChanged(bool oldVal, bool newVal)
    {
        ApplyState(newVal);

        if (newVal && openSound != null)
            AudioSource.PlayClipAtPoint(openSound, transform.position);
    }

    private void ApplyState(bool open)
    {
        if (doorCollider != null)
            doorCollider.enabled = !open;

        if (doorAnimator != null)
            doorAnimator.SetBool("IsOpen", open);
        else if (doorSprite != null && (closedSprite != null || openSprite != null))
            doorSprite.sprite = open ? openSprite : closedSprite;
    }

    // Вызывай только с сервера (например, из LevelManager)
    public void Open()
    {
        if (!IsServer) return;
        isOpen.Value = true;
    }

    public override void OnDestroy()
    {
        isOpen.OnValueChanged -= OnOpenChanged;
    }
}