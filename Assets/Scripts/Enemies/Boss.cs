using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    private const int MAIN_BULLET_NUMBER = 8;
    private const int SECONDARY_BULLET_NUMBER = 30;
    private const float SECONDARY_ANGLE = 50.0f;
    private const int BARRAGE_NUMBER = 14;
    private const float BOSS_MOVE_SPEED = 4.0f;
    private const int CHILDREN_NUMBER = 8;

    [SerializeField] private Transform firePos1, firePos2, secondaryFirePos1, secondaryFirePos2;
    [SerializeField] private GameObject mainTurret, secondaryTurret1, secondaryTurret2;
    [SerializeField] private Bullet secondaryBullet;
    [SerializeField] private BossBullet mainBullet;
    [SerializeField] private Transform spawner1, spawner2, childrenParent;
    [SerializeField] private ChildTank childTank;
    [SerializeField] private Vector2 mainAttackCooldown, secondaryAttackCooldown, backCooldown;

    private bool secondaryOn, backMove, invincibility;
    private Quaternion turretAngle;
    private List<GameObject> mainBulletList, secondaryBulletList;
    private Vector2 startPosition, endPosition;
    private int childrenCount;
    private float mainAttackTimer, secondaryAttackTimer, backTimer;

    protected override void Start()
    {
        base.Start();

        mainBulletList = new List<GameObject>();
        for(int i=0; i < MAIN_BULLET_NUMBER; i++)
        {
            mainBulletList.Add(Instantiate(mainBullet.gameObject));
            mainBulletList[i].SetActive(false);
        }

        secondaryBulletList = new List<GameObject>();
        for(int i=0; i< SECONDARY_BULLET_NUMBER; i++)
        {
            secondaryBulletList.Add(Instantiate(secondaryBullet.gameObject));
            secondaryBulletList[i].SetActive(false);
        }

        startPosition = transform.position;
        endPosition = new Vector2(startPosition.x, startPosition.y + 3.0f);
        transform.position = endPosition;
        backMove = false;

        childrenCount = CHILDREN_NUMBER;
        invincibility = false;

        attackTimer = 0;
        mainAttackTimer = Random.Range(mainAttackCooldown.x, mainAttackCooldown.y);
        secondaryAttackTimer = Random.Range(secondaryAttackCooldown.x, secondaryAttackCooldown.y);
        backTimer = Random.Range(backCooldown.x, backCooldown.y);

    }

    private void Update()
    {
        if (!isDead) attack();
    }

    private void FixedUpdate()
    {
        if (!isDead) move();
    }


    protected override void attack()
    {
        if (!backMove)
        {
            if (mainAttackTimer <= 0)
            {
                rotateMainTurret();
                mainAttackTimer = Random.Range(mainAttackCooldown.x, mainAttackCooldown.y);
            }
            else
            {
                mainAttackTimer -= Time.deltaTime;
            }

            if (secondaryAttackTimer <= 0)
            {
                activateSecondaryAttack();
                secondaryAttackTimer = Random.Range(secondaryAttackCooldown.x, secondaryAttackCooldown.y);
            }else
            {
                secondaryAttackTimer -= Time.deltaTime;
            }

            if (backTimer <= 0)
            {
                backMove = true;
                invincibility = true;
            }
            else
            {
                backTimer -= Time.deltaTime;
            }
        }


        mainTurretAttack();

        secondaryTurretAttack(secondaryTurret1);
        secondaryTurretAttack(secondaryTurret2);

    }

    protected override void move()
    {
        if (backMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosition, BOSS_MOVE_SPEED * Time.deltaTime);
            if ((Vector2)transform.position == endPosition) spawnChildren();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, BOSS_MOVE_SPEED * Time.deltaTime);
        }
    }

    private void spawnChildren()
    {
        if (childrenCount > 0)
        {
            if (attackTimer <= 0)
            {
                ChildTank child = Instantiate(childTank, spawner1.position, Quaternion.identity);
                child.transform.parent = childrenParent;

                child = Instantiate(childTank, spawner2.position, Quaternion.identity);
                child.transform.parent = childrenParent;

                childrenCount -= 2;

                attackTimer = Random.Range(minAttackCooldown, attackCooldown);
            }else
            {
                attackTimer -= Time.deltaTime;
            }
        }

        if (childrenCount == 0 && childrenParent.childCount == 0)
        {
            backMove = false;
            childrenCount = CHILDREN_NUMBER;
            attackTimer = 0;
            invincibility = false;
            backTimer = Random.Range(backCooldown.x, backCooldown.y);
        }
    }

    private void rotateMainTurret()
    {
        Vector3 targetPosition = target.transform.position;
        Vector2 distance = transform.position - targetPosition;

        float angle = Mathf.Atan2(distance.x, distance.y) * Mathf.Rad2Deg;

        turretAngle = Quaternion.Euler(0, 0, -angle);
    }

    private void rotateTo(GameObject rotatingObject, Quaternion angle, float change = 0)
    {
        Vector3 vectorChange = new Vector3(0, 0, change);
        float currentSpeed = (Time.deltaTime * speed) / Vector3.Distance(rotatingObject.transform.localRotation.eulerAngles - vectorChange, angle.eulerAngles);
        rotatingObject.transform.localRotation = Quaternion.Lerp(rotatingObject.transform.localRotation, angle, currentSpeed);
    }

    private void mainTurretAttack()
    {

        if (Mathf.Abs(mainTurret.transform.localRotation.eulerAngles.z - turretAngle.eulerAngles.z) > 1.0f)
        {
            rotateTo(mainTurret, turretAngle);

            if (Mathf.Abs(mainTurret.transform.localRotation.eulerAngles.z - turretAngle.eulerAngles.z) <= 1.0f)
            {
                anim.SetTrigger("attack");

                GameObject currentBullet1 = useObject<GameObject>(mainBulletList);
                currentBullet1.transform.position = firePos1.position;
                currentBullet1.transform.rotation = mainTurret.transform.rotation;

                GameObject currentBullet2 = useObject<GameObject>(mainBulletList);
                currentBullet2.transform.position = firePos2.position;
                currentBullet2.transform.rotation = mainTurret.transform.rotation;

                currentBullet1.SetActive(true);
                currentBullet2.SetActive(true);
            }
        }
    }

    private void activateSecondaryAttack()
    {
        secondaryOn = true;
        StartCoroutine(secondaryBarrage());
    }

    private void secondaryTurretAttack(GameObject currentTurret)
    {
        if (secondaryOn)
        {
            Quaternion angle = Quaternion.Euler(0, 0, SECONDARY_ANGLE * Mathf.Sign(currentTurret.transform.position.x));
            rotateTo(currentTurret, angle);
            if (currentTurret.transform.localRotation == angle) secondaryOn = false;
        }else if (currentTurret.transform.localRotation.eulerAngles.z != 0)
        {
            Quaternion angle = Quaternion.Euler(0, 0, 0);

            if (Mathf.Sign(currentTurret.transform.position.x) > 0) rotateTo(currentTurret, angle);
            else rotateTo(currentTurret, angle, 370.0f);
        }
    }

    private IEnumerator secondaryBarrage()
    {
        for(int i=0; i < BARRAGE_NUMBER; i++)
        {
            GameObject currentBullet1 = useObject<GameObject>(secondaryBulletList);
            currentBullet1.transform.position = secondaryFirePos1.position;
            currentBullet1.transform.rotation = secondaryTurret1.transform.rotation;

            GameObject currentBullet2 = useObject<GameObject>(secondaryBulletList);
            currentBullet2.transform.position = secondaryFirePos2.position;
            currentBullet2.transform.rotation = secondaryTurret2.transform.rotation;

            currentBullet1.SetActive(true);
            currentBullet2.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
    }

    protected override void hit()
    {
        if (!invincibility)
        {
            if (health > 0)
            {
                health--;
                if (health <= 0)
                {
                    isDead = true;
                    Instantiate(deathAnimations[0], transform.position, Quaternion.identity);
                    Destroy(this.gameObject, 0.5f);
                    Level.Instance.gameWon();
                }
            }
        }

        Debug.Log(health);
    }
}
