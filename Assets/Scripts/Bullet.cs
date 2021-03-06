﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed, deathTimer;
    [SerializeField] private int direction;
    [SerializeField] private GameObject bulletExplosion;

    private Rigidbody2D rg;
    private float timer;

    private void Start()
    {
        rg = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        timer = deathTimer;
    }

    private void FixedUpdate()
    {
        move();

        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }else
        {
            timer -= Time.deltaTime;
        }
    }

    private void move()
    {
        rg.velocity =  Mathf.Sign(direction) * transform.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Player")
        {
            collision.SendMessage("hit");
        }

        if (bulletExplosion)
        {
            Instantiate(bulletExplosion, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false);
    }
}
