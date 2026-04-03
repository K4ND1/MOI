using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundcontroller : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float parallaxEffect;

    [SerializeField] private static float parallaxEffectMultiplier = 0.25f;

    void Start()
    {
        parallaxEffect *= parallaxEffectMultiplier;  

        startPos = transform.position.x - parallaxEffect * cam.transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        /// Parallax effect and infinite scrolling should 
        /// be placed in a seperete method to clean the code up

        // Move the background in different speeds in relation to the camera movement
        float dist = cam.transform.position.x * parallaxEffect;
        float move = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // Infinite scrolling effect
        if (move > startPos + length)
        {
            startPos += length;
        }
        else if (move < startPos - length)
        {
            startPos -= length;
        }
    }
}
