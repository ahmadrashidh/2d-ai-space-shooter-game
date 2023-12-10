using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_detection : MonoBehaviour
{

    public float speed = 1.0f; // Speed of the enemy movement
    public float amplitude = 3.0f; // Amplitude of the sine wave
    public float frequency = 1.0f; // Frequency of the sine wave
    public float patrolRange = 10.0f; // Range for patrolling (left to right)
    public float minY = -4.0f; // Minimum Y position
    public float maxY = 4.0f; // Maximum Y position
    public float leftBoundary = -8.0f; // Left boundary of movement
    public float rightBoundary = 9.0f; // Right boundary of movement

    public float detectionRadius = 4f;
    public LayerMask playerLayer; // Assign the layer where the player is placed
    public float movementSpeed = 2f; // Speed at which the enemy moves towards the player

    private float startXPos;
    private bool movingLeft = true; // Initial movement direction
    private float timer = 0.0f;

    public Transform playerTransform; // Reference to the player's Transform
    private SpriteRenderer enemyRenderer; // Reference to the SpriteRenderer component
    private Color defaultColor; // Store the default color of the enemy
    private bool isChasing = false; // Flag to indicate whether the enemy is chasing the player

    private void Start()
    {
        startXPos = transform.position.x;
        enemyRenderer = GetComponent<SpriteRenderer>();
        defaultColor = enemyRenderer.color;
    }

    private void Update()
    {
        if (!isChasing)
        {
            MoveEnemy();
            CheckForPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    void MoveEnemy()
    {
        float patrolX = CalculatePatrolX();
        float yOffset = Mathf.Sin(timer * frequency) * amplitude;
        float newXPos = Mathf.Clamp(patrolX, leftBoundary, rightBoundary);
        float newYPos = Mathf.Clamp(yOffset, minY, maxY);
        transform.position = new Vector2(newXPos, newYPos);
        timer += Time.deltaTime * speed;

        if (HasReachedPatrolBoundary(patrolX))
        {
            movingLeft = !movingLeft;
        }
    }

    float CalculatePatrolX()
    {
        float direction = movingLeft ? -1.0f : 1.0f;
        return startXPos + direction * Mathf.PingPong(Time.time * speed, patrolRange);
    }

    bool HasReachedPatrolBoundary(float patrolX)
    {
        return (patrolX <= startXPos - patrolRange && movingLeft) || (patrolX >= startXPos && !movingLeft);
    }

    void CheckForPlayer()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);

        foreach (Collider2D obj in detectedObjects)
        {
            if (obj.CompareTag("Player"))
            {
                StartChasing();
                break;
            }


        }
    }

    void StartChasing()
    {
        enemyRenderer.color = Color.green; // Change the enemy color to green when chasing
        isChasing = true; // Set the chasing flag to true
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Get player's transform
    }

    void ChasePlayer()
    {
        if (playerTransform != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, movementSpeed * Time.deltaTime);
            // Change the 'movementSpeed' value as needed for the speed of movement
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy Collision:" + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {

            Death();

        }
        if (collision.gameObject.CompareTag("Enemy"))
        {


        }
    }

    public void Death()
    {
        // Implement your death logic here
        // For example, destroy the enemy object or play a death animation
        Destroy(this.gameObject);
    }
}
