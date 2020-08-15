using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackCooldown;

    protected bool isDead;
    protected float attackTimer;

    protected Animator anim;
    protected Rigidbody2D rg;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rg = GetComponent<Rigidbody2D>();
    }

    protected virtual void hit()
    {
        if (health > 0)
        {
            health--;
            if (health <= 0)
            {
                isDead = true;
            }
        }
    }

    protected abstract void attack();
    protected abstract void move();

    protected void createList(ref List<GameObject> list, GameObject item, int size)
    {
        for(int i=0; i < size; i++)
        {
            list.Add(Instantiate(item));
            list[i].SetActive(false);

        }
    }

    protected T useObject<T>(List<T> list)
    {
        T currentObject = list[0];
        list.Add(currentObject);
        list.Remove(currentObject);

        return currentObject;
    }


}
