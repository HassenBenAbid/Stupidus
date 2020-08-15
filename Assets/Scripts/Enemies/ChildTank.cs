﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTank : Enemy
{
    private const int BULLET_NUMBER = 20;

    [SerializeField] private GameObject weapon;
    [SerializeField] private Transform firePos;
    [SerializeField] private Bullet bullet;

    private AudioSource audio;
    private Vector2 destination;
    private bool moving;
    static List<GameObject> bulletList;

    private void OnEnable()
    {
        if (bulletList == null)
        {
            bulletList = new List<GameObject>();
            for (int i = 0; i < BULLET_NUMBER; i++)
            {
                bulletList.Add(Instantiate(bullet.gameObject));
                bulletList[i].SetActive(false);
            }
        }

        attackTimer = attackCooldown;
        destination = target.transform.position;
        moving = true;

        audio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        rotate();

        move();

    }

    private void Update()
    {
        attack();
    }

    protected override void attack()
    {
        if (attackTimer <= 0)
        {
            GameObject currentBullet1 = useObject<GameObject>(bulletList);
            currentBullet1.transform.position = firePos.position;
            currentBullet1.transform.rotation = weapon.transform.rotation;
            currentBullet1.SetActive(true);
            audio.Play();

            attackTimer = Random.Range(minAttackCooldown, attackCooldown);
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    protected override void move()
    {
        if (moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if ((Vector2)transform.position == destination)
            {
                moving = false;
            }
        }

    }


    private void rotate()
    {
        Vector2 distance = target.transform.position - transform.position;
        float angle = Mathf.Atan2(distance.x, distance.y) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0, 0, -angle);

        if (moving)
        {
            distance = destination - (Vector2)transform.position;
            angle = Mathf.Atan2(distance.x, distance.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}
