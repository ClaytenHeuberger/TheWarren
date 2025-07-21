using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject Player;
    [SerializeField] private float spawnRange = 200;
    [SerializeField] private int startNum = 10;
    [SerializeField] private float spawnRate = 0.1f;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Material scoreMat;

    [HideInInspector]
    public int enemyCount = 0;
    List<GameObject> enemies = new List<GameObject>();
    float timer = 0;

    int currentScore;
    [HideInInspector]
    public bool scoreIsUpdating = false;

    bool started = false;
    GameHandler gameHandler;
    AudioManager audioManager;

    private void Start()
    {
        gameHandler = FindObjectOfType<GameHandler>();
        audioManager = FindObjectOfType<AudioManager>();
    }


    private void Update()
    {
        if (gameHandler.started && !started)
        {
            started = true;
            Begin();
        }


        if (started)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                enemyCount = 0;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].activeInHierarchy)
                    {
                        enemyCount++;
                    }
                }
                timer = 1 / spawnRate;

                if (enemyCount < maxEnemies)
                {

                    int numNewEnemies = Random.Range(0, (maxEnemies - enemyCount) + 1);
                    if (numNewEnemies > 3)
                    {
                        numNewEnemies = 3;
                    }

                    for (int i = 0; i < numNewEnemies; i++)
                        SpawnEnemy();

                }
            }
        }
    }
    private void Begin()
    {
        timer = 1 / spawnRate;
        for (int i = 0; i < startNum; i++)
        {
            SpawnEnemy();
        }
    }
    void SpawnEnemy()
    {
        Vector3 newPos = new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange));
        Vector3 newRot = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
        GameObject newEnemy = Instantiate(Enemy, newPos, Quaternion.Euler(newRot));
        newEnemy.GetComponent<EnemyShip>().player = Player.transform;
        newEnemy.GetComponent<EnemyShip>().enemyHandler = this;
        enemies.Add(newEnemy);

        
    }


    public void AddScore(int scoreToAdd, Color color)
    {
        StartCoroutine(AddScoreOverTime(scoreToAdd, color));
    }

    private IEnumerator AddScoreOverTime(int scoreToAdd, Color color)
    {

        scoreText.fontSize *= 2;

        scoreIsUpdating = true;

        currentScore = int.Parse(scoreText.text);
        int tempScore = 0;

        

        Color tempFontColor = scoreText.color;
        scoreText.color = color;
        while (Mathf.Abs(tempScore) < Mathf.Abs(scoreToAdd))
        {

            if (scoreToAdd < 0)
                tempScore--;
            else if (scoreToAdd > 0)
                tempScore++;

            scoreText.text = (tempScore + currentScore).ToString();

            float ratio = Mathf.Abs(tempScore) / Mathf.Abs(scoreToAdd);
            scoreMat.SetFloat("_GlowPower", Mathf.Lerp(0.3f, 0.05f, ratio));

            audioManager.Play("ScoreTick");

            yield return new WaitForSeconds(0.05f);
        }

        currentScore += scoreToAdd;
        scoreText.text = currentScore.ToString();
        scoreText.fontSize /= 2;
        scoreText.color = new Color(0.5f, 0.5f, 0);
        scoreMat.SetFloat("_GlowPower", 0.05f);
        scoreIsUpdating = false;

    }

}
