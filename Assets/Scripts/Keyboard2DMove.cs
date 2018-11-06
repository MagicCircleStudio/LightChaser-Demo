using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard2DMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    // Use this for initialization
    void Start ()
    {
        
    }
    
    // Update is called once per frame
    void Update ()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 normalizedMove = new Vector3(horizontal, 0f, vertical);
        transform.position += normalizedMove * moveSpeed * Time.deltaTime;
    }
}
