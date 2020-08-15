using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private List<Transform> stopPositions;

    private bool canMove;
    private int currentStop;

    private void Start()
    {
        canMove = true;
        currentStop = 0;
    }

    private void FixedUpdate()
    {
        move();

        stop();
    }



    private void move()
    {
        if (canMove)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
        }
    }

    private void stop()
    {
        if (transform.position.y >= stopPositions[currentStop].position.y)
        {
            canMove = false;
            currentStop++;
        }
    }

    public void go()
    {
        canMove = true;
    }

    public bool isMoving()
    {
        return canMove;
    }

    public int getCurrentStop()
    {
        return currentStop;
    }
}
