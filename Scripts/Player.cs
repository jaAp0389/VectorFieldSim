using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 worldPosition;
    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        //mLineList = new List<LineSegment>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MoveToPosition(GetMousPosition());
        }
    }

    private void FixedUpdate()
    {

        Direction();
        this.transform.position += velocity * Time.deltaTime * Lib.sSpeed;
    }
    public Vector3 GetMousPosition()
    {
        Plane plane = new Plane(Vector3.forward, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
    void Direction()
    {
        if (!Lib.sIsSlowDirChange) GetDirection();
        else GetDirectionSlow();
    }
    void GetDirection()
    {
        if(Lib.sIsFlowField)
        {
            velocity = Lib.sFlowField.GetDirection(this.transform.position);
            return;
        }
        velocity = Lib.sVectorField.GetDirection(this.transform.position);
    }
    void GetDirectionSlow()
    {
        if(!Lib.sIsFlowField)
        {
            velocity += Lib.sVectorField.GetDirection
                (this.transform.position) * Lib.sDirChangeRate;
            velocity = velocity.normalized;
            return;
        }

        velocity += Lib.sFlowField.GetDirection
            (this.transform.position) * Lib.sDirChangeRate;
        velocity = velocity.normalized;
    }
    void MoveToPosition(Vector3 _position)
    {
        this.transform.position = _position;
    }

    
}
