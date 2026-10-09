using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.2f;

    private Camera cam;
    private float startPosX;
    private float startPosY;
    private float spriteWidth;

    void Start()
    {
        cam = Camera.main;
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        float dist = cam.transform.position.x * parallaxSpeed;
        transform.position = new Vector3(startPosX + dist, startPosY, transform.position.z);

        float camX = cam.transform.position.x;
        if (camX - startPosX > spriteWidth) startPosX += spriteWidth;
        else if (startPosX - camX > spriteWidth) startPosX -= spriteWidth;
    }
}