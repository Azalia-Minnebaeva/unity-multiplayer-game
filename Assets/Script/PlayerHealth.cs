using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Здоровье")]
    public float maxHealth = 100f;
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public Slider healthBar;
    public string damageTag = "Enemy";
    public float damageAmount = 10f;

    private void Awake()
    {
        healthBar = GameObject.Find("HealthBar")?.GetComponent<Slider>();
    }

    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += OnHealthChanged;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth.Value;
        }
    }

    void OnHealthChanged(float oldVal, float newVal)
    {
        if (healthBar != null)
            healthBar.value = newVal;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(damageTag))
            TakeDamage(damageAmount);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(damageTag))
            TakeDamage(damageAmount);
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            currentHealth.Value = Mathf.Clamp(
                currentHealth.Value - damage, 0, maxHealth);
            if (currentHealth.Value <= 0)
                Die();
        }
        else
        {
            TakeDamageServerRpc(damage);
        }
    }

    [ServerRpc]
    void TakeDamageServerRpc(float damage)
    {
        currentHealth.Value = Mathf.Clamp(
            currentHealth.Value - damage, 0, maxHealth);
        if (currentHealth.Value <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Игрок умер!");
    }

    public override void OnDestroy()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }
}