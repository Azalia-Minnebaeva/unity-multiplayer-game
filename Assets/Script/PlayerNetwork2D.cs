using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork2D : NetworkBehaviour
{
    // Имя игрока над головой
    private NetworkVariable<int> playerIndex =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    [Header("UI над головой")]
    public TMPro.TextMeshPro nameLabel;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerIndex.Value = IsHost ? 0 : 1;
        }

        playerIndex.OnValueChanged += OnIndexChanged;
        ApplyIndex(playerIndex.Value);
    }

    void OnIndexChanged(int old, int newVal) => ApplyIndex(newVal);

    void ApplyIndex(int index)
    {
        if (nameLabel != null)
            nameLabel.text = index == 0 ? "Player 1" : "Player 2";
    }

    public override void OnDestroy()
    {
        playerIndex.OnValueChanged -= OnIndexChanged;
    }
}