using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private const float HIT_COLLIDER_TIMER = 0.4f;
    private const float TIME_BEFORE_HIT = 0.6f;

    [SerializeField] private MeleeCollider meleeHit;

    private bool attacking;

    private void OnEnable()
    {
        attacking = false;
        attackTimer = 0;
    }

    private void FixedUpdate()
    {
        move();

        attack();

        rotateTowardPlayer();
    }


    protected override void attack()
    {
        if (attackTimer <= 0)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 2.0f);

            if (hit && hit.tag == "Player" && !attacking)
            {
                StartCoroutine(meleeAttack());
            }
        }else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    protected override void move()
    {
        Vector2 distance = (target.transform.position - transform.position).normalized;
        rg.velocity = distance * Time.deltaTime * speed;
    }

    private void rotateTowardPlayer()
    {
        Vector2 distance = transform.position - target.transform.position;
        float angle = Mathf.Atan2(distance.x, distance.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

    private IEnumerator meleeAttack()
    {
        anim.SetTrigger("attack");
        attacking = true;

        yield return new WaitForSeconds(TIME_BEFORE_HIT);

        meleeHit.gameObject.SetActive(true);

        yield return new WaitForSeconds(HIT_COLLIDER_TIMER);

        meleeHit.gameObject.SetActive(false);
        attacking = false;
        attackTimer = Random.Range(minAttackCooldown, attackCooldown);
    }
}
