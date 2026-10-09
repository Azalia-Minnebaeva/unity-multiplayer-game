using UnityEngine;
using Unity.Netcode;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(4f, 1f, 0f);

    private Transform player;

    void LateUpdate()
    {
        if (player == null)
        {
            foreach (var netObj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
            {
                if (netObj.IsOwner)
                {
                    player = netObj.transform;
                    break;
                }
            }
            return;
        }

        Vector3 desiredPosition = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}