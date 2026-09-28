using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game")]
    public float gameDuration = 60f;

    [Header("UI")]
    public TMP_Text scoreText;
    //public TMP_Text timeText;
    public TMP_Text waveText;
    public TMP_Text healthText;
    public TMP_Text messageText;
    private int score = 0;
    //private float timeLeft;
    public int currentWave = 1;
    public int enemiesAlive = 0;
    public bool isGameOver = false;

    public bool IsGameOver { get { return isGameOver; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //timeLeft = gameDuration;
        score = 0;
        currentWave = 1;
        enemiesAlive = 0;
        isGameOver = false;

        if (messageText != null) messageText.gameObject.SetActive(false);

        UpdateScoreText();
        UpdateWaveText();
        //UpdateTimeText();
    }

    private void Update()
    {
        if (isGameOver)
        {
            CheckRestartInput();
        }


        //UpdateTimer();
    }
    /*
    private void UpdateTimer()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            WinGame();
        }

        UpdateTimeText();
    }
    
    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            timeText.text =
                "Time: " +
                Mathf.CeilToInt(timeLeft);
        }
    }*/

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
    public void UpdateWaveText()
    {
        if (waveText != null) waveText.text = "Wave: " + currentWave;
    }
    public void EnemyDied()
    {
        enemiesAlive--;
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (healthText != null) healthText.text = "HP: " + currentHealth + "/" + maxHealth;
    }

    

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        ShowMessage("GAME OVER\nScore: " + score + "\nWave Reached: " + currentWave + "\nPress SPACE to Restart");
        CheckRestartInput();
    }

    private void WinGame()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        ShowMessage(
            "YOU SURVIVED!\n" +
            "Score: " + score +
            "\nPress R to Restart"
        );
    }

    private void ShowMessage(string message)
    {
        if (messageText == null) return;
        messageText.gameObject.SetActive(true);
        messageText.text = message;
    }

    private void CheckRestartInput()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.spaceKey.wasPressedThisFrame) RestartGame();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

