using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PushableBox : MonoBehaviour
{
    public float mass = 3f;
    public float linearDrag = 5f;
    public PhysicsMaterial2D bounceMaterial;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.mass = mass;
        rb.linearDamping = linearDrag;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (bounceMaterial != null)
            GetComponent<Collider2D>().sharedMaterial = bounceMaterial;
    }
}