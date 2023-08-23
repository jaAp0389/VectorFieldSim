using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 movDir = new Vector3(0, 0, 0);
    [SerializeField] float speed = 10;
    private void Update()
    {
        movDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) movDir += Vector3.up;
        if (Input.GetKey(KeyCode.S)) movDir += Vector3.down;
        if (Input.GetKey(KeyCode.A)) movDir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movDir += Vector3.right;

        movDir = movDir.normalized;
    }
    private void FixedUpdate()
    {
        this.transform.position += movDir * Time.deltaTime * speed;
    }
}
