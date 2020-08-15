using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    private const int BULLET_NUMBER = 20;

    [SerializeField] private GameObject weapon;
    [SerializeField] private Transform firePos1, firePos2;
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
            for(int i=0; i < BULLET_NUMBER; i++)
            {
                bulletList.Add(Instantiate(bullet.gameObject));
                bulletList[i].SetActive(false);
                DontDestroyOnLoad(bulletList[i]);
            }
        }

        attackTimer = attackCooldown;
        destination = chooseRandomDestination();
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
            currentBullet1.transform.position = firePos1.position;
            currentBullet1.transform.rotation = weapon.transform.rotation;
            currentBullet1.SetActive(true);

            GameObject currentBullet2 = useObject<GameObject>(bulletList);
            currentBullet2.transform.position = firePos2.position;
            currentBullet2.transform.rotation = weapon.transform.rotation;
            currentBullet2.SetActive(true);

            audio.Play();
            attackTimer = Random.Range(minAttackCooldown, attackCooldown);
        }else
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

    private Vector2 chooseRandomDestination()
    {
        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = (cameraHeight * Camera.main.aspect) / 2.0f;

        float cameraPosX = Random.Range(-cameraWidth, cameraWidth);
        float cameraPosY = Random.Range(-cameraHeight, cameraHeight) + Camera.main.transform.position.y;

        return new Vector2(cameraPosX, cameraPosY);
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
