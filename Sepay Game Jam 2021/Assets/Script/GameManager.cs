using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject bubble;
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject heart;

    [SerializeField] private GameObject audioManager;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;

    [SerializeField] private GameObject gameOverPanel;

    private Vector2 minPosCamera;
    private Vector2 maxPosCamera;

    private int score;
    private int highScore;

    private int live;
    [SerializeField] private GameObject[] heartLive;

    private float coolDownSpawnTime;
    private float coolDownTime;

    private int maxBubbleSpawn;

    SaveData theData;

    void Start()
    {
        // Find screen area
        minPosCamera = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        maxPosCamera = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));

        // Set data
        score = 0;
        scoreText.text = "0";
        maxBubbleSpawn = 3;

        coolDownSpawnTime = 3f;
        coolDownTime = 0f;

        // Find Audio
        if (FindObjectOfType<AudioManager>() == null)
        {
            Instantiate(audioManager, transform.position, Quaternion.identity);
        }

        // Load data
        theData = SaveGame.LoadData();

        // Set score
        highScore = theData.GetHighScore();
        highScoreText.text = highScore.ToString();

        // Set live
        live = heartLive.Length;

        // Audio
        FindObjectOfType<AudioManager>().Play("BGM");
    }

    
    void Update()
    {
        // Spawn Bubble
        BubbleSpawner();
    }

    private void BubbleSpawner()
    {
        // Check cool down time
        if (coolDownTime > 0)
        {
            // If not over
            coolDownTime -= Time.deltaTime;
        }
        else if (coolDownTime <= 0)
        {
            // If it's over
            // Instantiate bubble
            int spawnTotal = Random.Range(1, maxBubbleSpawn);
            for (int i = 0; i < spawnTotal; i++)
            {
                // Random Y position
                float yPos;
                if (Random.Range(0, 2) == 0)
                {
                    yPos = minPosCamera.y - Random.Range(2f, 3f);
                }
                else
                {
                    yPos = maxPosCamera.y + Random.Range(2f, 3f);
                }

                // Random bomb
                GameObject temp;
                int randTemp = Random.Range(0, 10);
                if (randTemp == 0 || randTemp == 1 || randTemp == 3)
                {
                    temp = bomb;
                }
                else if (randTemp == 5)
                {
                    temp = heart;
                }
                else
                {
                    temp = bubble;
                }

                // Spawn it
                Vector3 spawnPos = new Vector3(Random.Range(minPosCamera.x, maxPosCamera.x), yPos, 0);
                Instantiate(temp, spawnPos, Quaternion.identity);
            }
           
            // Reset time
            coolDownTime = coolDownSpawnTime;
        }
    }


    public Vector2 GetMinPosCamera()
    {
        return minPosCamera;
    }
    public Vector2 GetMaxPosCamera()
    {
        return maxPosCamera;
    }

    public void BubblePopped(int id)
    {
        // Bubble
        if (id == 0)
        {
            // SFX
            FindObjectOfType<AudioManager>().Play("BubblePop");

            // Set score
            IncreaseScore(10);            
        }
        // Bomb
        else if (id == 1)
        {
            // SFX
            FindObjectOfType<AudioManager>().Play("BubbleBomb");

            // Set Live
            SetHeartLive(-1);
        }
        // Heart
        else if (id == 2)
        {
            // SFX
            FindObjectOfType<AudioManager>().Play("BubbleHeart");

            SetHeartLive(1);
        }
    }

    private void IncreaseScore(int value)
    {
        // Increase score
        score += value;
        scoreText.text = score.ToString();

        // Check HighScore
        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();
            theData.SetHighScore(score);
        }

        // Increase level
        if (score % 100 == 0)
        {
            if (maxBubbleSpawn < 10)
            {
                maxBubbleSpawn += 1;
            }

            if (coolDownSpawnTime >= 1.5f)
            {
                coolDownSpawnTime -= 0.5f;
            }
        }
    }

    private void SetHeartLive(int value)
    {
        if (value > 0)
        {
            if(live + value <= heartLive.Length)
            {
                live += value;
            }
            else
            {
                IncreaseScore(20);
            }
        }
        else if(value < 0)
        {
            live += value;

            if(live <= 0)
            {
                StartCoroutine(GameOverDelay());
            }
        }

        // Set the UI
        for (int i = 0; i < heartLive.Length; i++)
        {
            if (i <= live - 1)
            {
                heartLive[i].SetActive(true);
            }
            else
            {
                heartLive[i].SetActive(false);
            }
        }
    }

    private IEnumerator GameOverDelay()
    {
        // Sound
        FindObjectOfType<AudioManager>().Stop("BGM");
        FindObjectOfType<AudioManager>().Play("GameOver");

        // Save game
        SaveHighScore();

        yield return new WaitForSeconds(2);

        // Game over
        gameOverPanel.SetActive(true);
        MenuManager.MenuIsActive = true;
        // Stop the time
        Time.timeScale = 0;
    }

    public void SaveHighScore()
    {
        if (score >= highScore)
        {
            SaveGame.SaveProgress(theData);
        }
    }
}
