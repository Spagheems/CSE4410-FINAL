using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3.0f;
    private Transform player;
    public int health = 3;
    public delegate void EnemyDeathEvent();
    public event EnemyDeathEvent OnEnemyDeath;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();

        if (player == null)
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        if (animator == null)
            Debug.LogError("Animator not found on enemy!");
    }

    void Update()
    {
        if (isDead || player == null) return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.LookAt(player);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Optional: Add a "GetHit" animation trigger here
            animator?.SetTrigger("Hit");
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator?.SetTrigger("IsDead");

        // Notify wave system
        OnEnemyDeath?.Invoke();

        // Start a delay before destroying the enemy
        StartCoroutine(DelayedDestroy());
    }

    IEnumerator DelayedDestroy()
    {
        // Adjust this based on the length of your death animation
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Projectile"))
    {
        TakeDamage(1);
        Destroy(collision.gameObject);
    }
    else if (collision.gameObject.CompareTag("Player"))
    {
        PlayerCharacter playerChar = collision.gameObject.GetComponent<PlayerCharacter>();
        if (playerChar != null)
        {
            playerChar.Hurt(1);
        }
    }
}
}