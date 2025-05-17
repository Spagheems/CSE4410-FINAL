using System.Collections;
using UnityEngine;
using TMPro; // For UI display

public class PlayerCharacter : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public GameObject gameOverPanel;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void Hurt(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Health: {currentHealth}");
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);


    }
}
