/*using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetHealth(
                currentHealth,
                maxHealth
            );
        }
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

}

*/
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // ฟังก์ชันรับความเสียหาย (EnemyController จะเรียกใช้ฟังก์ชันนี้ตอนเดินชน)
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"ผู้เล่นถูกโจมตี! พลังชีวิตเหลือ: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ฟังก์ชันเพิ่มเลือดสำหรับเก็บยา
    public void Heal(float amount)
    {
        currentHealth += amount;

        // ป้องกันไม่ให้เลือดเกินหลอด
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log($"ฮีล! พลังชีวิตปัจจุบัน: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log("ผู้เล่นตาย! Game Over");
        // ซ่อนผู้เล่นไปก่อน (ในอนาคตสามารถเรียก GameManager เพื่อโชว์หน้า Game Over ได้)
        gameObject.SetActive(false);
    }
}