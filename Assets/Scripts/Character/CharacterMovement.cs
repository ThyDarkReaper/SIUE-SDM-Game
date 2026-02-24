using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //public Camera cam;
    //public GameObject character;
    public GlobalVariables GV;
    public float speed = 5.0f;

    private Rigidbody rb;
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        if(!GV.isTalking() && !GV.isPaused())
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized;

            rb.velocity = movement * speed * Time.fixedDeltaTime * 100;
        }
    }
}
