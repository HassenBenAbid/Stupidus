using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitDamage : MonoBehaviour
{
    [SerializeField] private MovingCamera mainCamera;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && mainCamera.isMoving())
        {
            collision.collider.gameObject.SendMessage("hit");
        }
    }
}
