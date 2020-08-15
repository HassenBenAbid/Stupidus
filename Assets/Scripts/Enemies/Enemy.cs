using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    [SerializeField] protected List<GameObject> deathAnimations;
    [SerializeField] protected float minAttackCooldown;
    protected static Player target;

    protected override void Start()
    {
        base.Start();

        if (target == null)
        {
            target = GameObject.FindWithTag("Player").GetComponent<Player>();
        }
        gameObject.tag = "Enemy";
    }

    protected override void hit()
    {
        base.hit();

        if (isDead)
        {
            if (deathAnimations.Count > 0)
            {
                int randomDeath = Random.Range(0, deathAnimations.Count);
                Instantiate(deathAnimations[randomDeath], transform.position, Quaternion.identity);
                //gameObject.SetActive(false);
                Destroy(this.gameObject);
            }

           
        }
    }

}
