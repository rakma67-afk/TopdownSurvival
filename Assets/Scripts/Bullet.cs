/*
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 2f;
    public int damage = 1;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity =
            transform.forward * speed;

        Destroy(
            gameObject,
            lifeTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        // ป้องกัน Bullet ชน Player ที่ยิงมันออกมา
        if (other.GetComponent<PlayerController>() != null)
        {
            return;
        }

        // ตรวจว่าเป็น Enemy หรือไม่
        if (other.TryGetComponent<EnemyController>(
            out EnemyController enemy))
        {
            enemy.TakeDamage(damage);
        }

        // Bullet หายเมื่อชนวัตถุ
        Destroy(gameObject);
    }
}
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ flash
*/

using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifetime = 2.5f;
    public int damage = 1;

    private void Start()
    {
        // ทำลายกระสุนอัตโนมัติตามเวลา ป้องกัน Object ค้างใน Memory
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // พุ่งตรงไปข้างหน้าตามทิศทางหัวกระสุน
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        // ไม่ชนกับตัวผู้เล่นเอง
        if (other.CompareTag("Player")) return;

        if (other.TryGetComponent<EnemyController>(
            out EnemyController enemy))
        {
            enemy.TakeDamage(damage);
        }

        // ในอนาคตใส่คำสั่งเช็คดาเมจศัตรู/กำแพง ตรงนี้ได้
        Destroy(gameObject);
    }
}