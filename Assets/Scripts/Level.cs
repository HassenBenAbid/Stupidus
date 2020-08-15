using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : Singleton<Level>
{
    private const int ENEMIES_NUMBER = 15;
    private const int ENEMIES_MAX_ALIVE = 6;
    private const KeyCode PAUSE_BUTTON = KeyCode.Escape;

    [SerializeField] private Enemy meleeEnemy, rangedEnemy;
    [SerializeField] private SurpriseEnemy surprise;
    [SerializeField] private MovingCamera mainCamera;
    [SerializeField] private float spawnCooldown, surpriseMinCooldown, surpriseMaxCooldown;
    [SerializeField] private GameObject enemiesParent, gameoverScreen;
    [SerializeField] private Transform spawner;
    [SerializeField] private Boss finalBoss;
    [SerializeField] private Player player;
    [SerializeField] private GameObject gamewonScreen, finalBossText, pauseScreen;


    private int currentEnemies;
    private float cameraWidth, cameraHeight, spawnTimer, surpriseTimer;
    private bool bossSpawned, gameEnded, paused;

    private void Start()
    {
        Time.timeScale = 1.0f;
        paused = false;
        currentEnemies = ENEMIES_NUMBER;
        bossSpawned = false;
        mainCamera.transform.position = new Vector3(0, 0, -10.0f);
        player.transform.position = new Vector3(0, 0, 0);

        cameraHeight = Camera.main.orthographicSize * 2.0f;
        spawnTimer = 0;
        surpriseTimer = surpriseMinCooldown;
        gameEnded = false;
    }

    private void Update()
    {
        spawnEnemies();

        spawnSurprise();

        pause();
    }

    private void spawnEnemies()
    {
        if (!mainCamera.isMoving())
        {
            if (mainCamera.getCurrentStop() < 4)
            {
                if (currentEnemies > 0)
                {
                    if (spawnTimer <= 0)
                    {
                        if (enemiesParent.transform.childCount < 5)
                        {
                            int enemyType = Random.Range(0, 2);

                            if (enemyType == 0)
                            {
                                spawnMelee();
                            }
                            else if (enemyType == 1)
                            {
                                spawnRanged();
                            }

                            currentEnemies--;
                            spawnTimer = spawnCooldown;
                        }

                    }
                    else
                    {
                        spawnTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    if (enemiesParent.transform.childCount == 0)
                    {
                        mainCamera.go();
                        currentEnemies = ENEMIES_NUMBER;
                    }
                }
            }else if (!bossSpawned)
            {
                bossSpawned = true;
                finalBoss.gameObject.SetActive(true);
                finalBossText.SetActive(true);
                Destroy(finalBossText, 1.5f);
            }
            

        }
    }

    private void spawnRanged()
    {
        float spawnPosX = Random.Range(-spawner.position.x, spawner.position.x);
        float spawnPosY = spawner.position.y + (Random.Range(0, 2) * cameraHeight);

        Enemy currentEnemy = Instantiate(rangedEnemy, new Vector2(spawnPosX, spawnPosY), Quaternion.identity);
        currentEnemy.transform.parent = enemiesParent.transform;
    }

    private void spawnMelee()
    {

        float spawnPosY = Random.Range(spawner.position.y, spawner.position.y + cameraHeight);
        float spawnPosX = Mathf.Sign(Random.Range(-1, 2)) * spawner.position.x;

        Enemy currentEnemy = Instantiate(meleeEnemy, new Vector2(spawnPosX, spawnPosY), Quaternion.identity);
        currentEnemy.transform.parent = enemiesParent.transform;
    }

    private void spawnSurprise()
    {
        if (mainCamera.isMoving())
        {
            if (surpriseTimer <= 0)
            {
                float spawnPosY = Random.Range(spawner.position.y, spawner.position.y + cameraHeight);
                float spawnPosX = spawner.position.x * Mathf.Sign(Random.Range(-1, 1));
                Instantiate(surprise, new Vector2(spawnPosX, spawnPosY), Quaternion.Euler(0, 0, 90.0f * Mathf.Sign(spawnPosX)));

                surpriseTimer = Random.Range(surpriseMinCooldown, surpriseMaxCooldown);
            }else
            {
                surpriseTimer -= Time.deltaTime;
            }
        }
    }

    public void gameover()
    {
        gameoverScreen.SetActive(true);
    }

    public void gameWon()
    {
        gamewonScreen.SetActive(true);
        gameEnded = true;
    }

    public void restart()
    {
        SceneManager.LoadScene(1);
    }

    public void quit()
    {
        Application.Quit();
    }

    public bool gameEnd()
    {
        return gameEnded;
    }

    private void pause()
    {
        if (Input.GetKeyUp(PAUSE_BUTTON))
        {
            if (paused)
            {
                Time.timeScale = 1.0f;
                pauseScreen.SetActive(false);
                paused = false;
            }else
            {
                pauseScreen.SetActive(true);
                paused = true;
                Time.timeScale = 0.0f;
            }
        }

    }


}
