using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f; // จำนวนเลือดที่เพิ่มต่อ 1 ชิ้น

    private void OnTriggerEnter(Collider other)
    {
        // เช็คว่าคนที่มาเหยียบคือผู้เล่น
        if (other.CompareTag("Player"))
        {
            // ดึงสคริปต์ PlayerHealth มาและสั่งเพิ่มเลือด
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject); // เก็บแล้วลบยาทิ้ง
            }
        }
    }
}