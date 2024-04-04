using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject cameraObject; // Ganti "Camera" menjadi "cameraObject" untuk menghindari konflik dengan kelas bawaan Unity
    public float parallaxEffect;
    private float width, positionX;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x; // Perbaiki penulisan "SpriteRendere" menjadi "SpriteRenderer"
        positionX = transform.position.x;
    }

    void Update()
    {
        float parallaxDistance = cameraObject.transform.position.x * parallaxEffect;
        float remainingDistance = cameraObject.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(positionX + parallaxDistance, transform.position.y, transform.position.z); // Perbaiki penulisan "Vector#" menjadi "Vector3"

        if (remainingDistance > positionX + width)
        {
            positionX += width;
        }
    }
}
