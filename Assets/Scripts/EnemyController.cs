using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Combat")]
    // ปรับเป็น float ให้สอดคล้องกับระบบอาวุธและ GDD
    public float maxHealth = 100f; 
    public float armor = 0f;       
    public float touchDamage = 20f;
    public int scoreValue = 10;
    
    private float currentHealth;
    private Rigidbody rb;
    private Transform player;
    private bool isDead = false;

    // อัปเดตฟังก์ชัน SetStats ให้รองรับเกราะ (armor)
    public void SetStats(float health, float speed, float enemyArmor)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        moveSpeed = speed;
        armor = enemyArmor;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (player == null) return;

        MoveTowardPlayer();
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        direction.Normalize();

        Vector3 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        Quaternion rotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(rotation);
    }

    // เปลี่ยนจาก int เป็น float เพื่อรับค่าจากกระสุนและมีด
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // คำนวณความเสียหายโดยหักลบด้วยเกราะ (หักลบแล้วต้องโดนอย่างน้อย 1 ดาเมจ)
        float finalDamage = Mathf.Max(damage - armor, 1f); 
        currentHealth -= finalDamage;

        Debug.Log($"{gameObject.name} โดนโจมตี {finalDamage} ดาเมจ | เลือดเหลือ: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            // หาก PlayerHealth.TakeDamage() ของคุณยังใช้ int อยู่ ให้ใส่ (int) ครอบ touchDamage ไว้
            playerHealth.TakeDamage((int)touchDamage); 

            Destroy(gameObject);
        }
    }
}