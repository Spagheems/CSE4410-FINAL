using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class SceneController : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 5f;

    private int waveNumber = 1;
    private int enemiesRemaining;

    [Header("UI Elements")]
    public TextMeshProUGUI waveText;       // Reference to Wave UI Text
    public TextMeshProUGUI countdownText;  // Reference to countdown UI Text

    void Start()
    {
        UpdateWaveText();
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(2f); // Initial delay before wave starts
        SpawnEnemies(enemiesPerWave);
    }

    void SpawnEnemies(int count)
    {
        enemiesRemaining = count;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<EnemyAI>().OnEnemyDeath += EnemyDefeated;
        }
    }

    void EnemyDefeated()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            StartCoroutine(NextWave());
        }
    }

    IEnumerator NextWave()
    {
        float countdown = timeBetweenWaves;

        // Display countdown timer
        while (countdown > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = "Next Wave In: " + countdown.ToString("F0");
            }

            countdown -= Time.deltaTime;
            yield return null;
        }

        if (countdownText != null)
        {
            countdownText.text = ""; // Clear after countdown
        }

        waveNumber++;
        enemiesPerWave += 2;

        UpdateWaveText();
        SpawnEnemies(enemiesPerWave);
    }

    void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + waveNumber;
        }
        else
        {
            Debug.LogError("WaveText UI is not assigned in SceneController!");
        }
    }
}
