using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private const int BULLET_NUMBER = 30;
    private const KeyCode FIRE_BUTTON = KeyCode.Mouse0;
    private const KeyCode DASH_BUTTON = KeyCode.Space;
    private const float ACTIVATED_DASH_TIME = 0.1f;
    private const float INVINCIBILITY_TIMER = 0.2f;

    [SerializeField] private Transform firePos;
    [SerializeField] private Bullet bullet;
    [SerializeField] private float dashCooldown, dashSpeed;
    [SerializeField] private HealthUI healthUI;


    private AudioSource audio;
    private List<GameObject> bulletList;
    private SpriteRenderer playerSprite;
    private float dashTimer, activatedDashTimer;
    private bool dashActivated, moving, invincibility;


    override protected void Start()
    {
        base.Start();

        isDead = false;
        moving = false;
        dashActivated = false;
        invincibility = false;
        dashTimer = 0;

        bulletList = new List<GameObject>();
        createList(ref bulletList, bullet.gameObject, BULLET_NUMBER);

        playerSprite = GetComponent<SpriteRenderer>();

        healthUI.setHearts(health);
        audio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!isDead && !Level.Instance.gameEnd())
        {
            move();

            rotate();

            dash();
        }
    }

    private void Update()
    {
        if (!isDead && !Level.Instance.gameEnd())
        {
            attack();

            if (dashTimer > 0)
            {
                dashTimer -= Time.deltaTime;
            }

            if (dashActivated)
            {
                activatedDashTimer -= Time.deltaTime;
            }
        }
    }

    protected override void attack()
    {
        if (Input.GetKeyUp(FIRE_BUTTON))
        {
            if (attackTimer <= 0)
            {
                anim.Play("attack", -1, 0);
                GameObject currentBullet = useObject<GameObject>(bulletList);
                currentBullet.transform.position = firePos.position;
                currentBullet.transform.rotation = transform.rotation;
                if (currentBullet.activeSelf) currentBullet.SetActive(false);
                currentBullet.SetActive(true);
                attackTimer = attackCooldown;
                audio.Play();
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    protected override void move()
    {
        float deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float deltaY = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        moving = (deltaX != 0 || deltaY != 0);
        rg.velocity = new Vector2(deltaX, deltaY);
    }

    private void rotate()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 distance = mousePosition - playerPos;
        float angle = Mathf.Atan2(distance.x, distance.y * -1) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void dash()
    {
        if (dashTimer <= 0)
        {
            if (Input.GetKeyUp(DASH_BUTTON) && !dashActivated && moving)
            {
                dashActivated = true;
                activatedDashTimer = ACTIVATED_DASH_TIME;
                dashTimer = dashCooldown;
            }
        }

        if (dashActivated)
        {

            rg.velocity *= dashSpeed * Time.deltaTime;

            if (activatedDashTimer <= 0) dashActivated = false;
        }
    }

    protected override void hit()
    {
        if (!invincibility)
        {
            base.hit();
            healthUI.heartHit(1);
        }
        if (health > 0) StartCoroutine(hitInvincibility());

        if (health <= 0)
        {
            Level.Instance.gameover();
            gameObject.SetActive(false);
        }
    }

    private IEnumerator hitInvincibility()
    {
        invincibility = true;
        for (int i = 0; i < 4; i++)
        {
            playerSprite.enabled = false;
            yield return new WaitForSeconds(INVINCIBILITY_TIMER);
            playerSprite.enabled = true;
            yield return new WaitForSeconds(INVINCIBILITY_TIMER);
        }
        invincibility = false;
    }
}
