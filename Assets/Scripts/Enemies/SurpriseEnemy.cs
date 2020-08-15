using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseEnemy : MonoBehaviour
{
    private const float TIME_BEFORE_DESTROY = 3.0f;

    [SerializeField] private float speed;
    [SerializeField] private Explosion death;

    private void Start()
    {
        Destroy(this.gameObject, TIME_BEFORE_DESTROY);
    }

    private void FixedUpdate()
    {
        move();
    }

    private void move()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.SendMessage("hit");
            Destroy(this.gameObject);
            Instantiate(death, transform.position, Quaternion.identity);
        }
    }
}
