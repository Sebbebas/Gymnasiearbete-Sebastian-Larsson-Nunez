using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField, Range(0f, 1f)] float parallaxEffectPower;

    private float imageLength;
    private float startpos;
    private float posY;

    void Awake()
    {
        mainCamera = FindFirstObjectByType<Camera>();
        imageLength = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        startpos = transform.position.x;
        posY = transform.position.y;
    }
    void FixedUpdate()
    {
        float tempLength = mainCamera.transform.position.x * (1 - parallaxEffectPower);
        float distance = mainCamera.transform.position.x * parallaxEffectPower;

        transform.position = new Vector3(startpos + distance, posY);

        if (tempLength > startpos + imageLength) { startpos += imageLength; } 
        else if (tempLength < startpos - imageLength) { startpos -= imageLength; }
    }
}