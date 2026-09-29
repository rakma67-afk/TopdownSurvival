using UnityEngine;
using UnityEngine.Audio;

public class PlayerShooting : MonoBehaviour
{
    [Header("Bullet & Fire Point")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;     // จุดปล่อยกระสุน (เช่น ปลายปืน)
    [SerializeField] private float fireRate = 0.15f;  // ยิงได้ทุกๆ กี่วินาที

    [Header("Spread Settings (องศา)")]
    [SerializeField] private float hipFireSpread = 22f; // กระจายมากเมื่อไม่เล็ง
    [SerializeField] private float aimSpread = 3.5f;    // กระจายแคบลงมากเมื่อเล็ง
    [SerializeField] private float spreadSmoothSpeed = 10f; // ความนุ่มนวลตอนกรอบขยาย/หด

    [Header("Movement Speed Modifier")]
    [SerializeField] private float aimWalkSpeed = 3f;   // ความเร็วเดินขณะเล็ง

    [Header("Visualizer")]
    [SerializeField] private WeaponSpreadVisualizer visualizer;

    private float nextTimeToFire = 0f;
    private float currentSpread;
    private bool isAiming;
    private PlayerController playerController;

    public bool IsAiming => isAiming;
    public float AimWalkSpeed => aimWalkSpeed;
    public AudioSource GunSound;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        currentSpread = hipFireSpread;
        GunSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // 1. ตรวจสอบการเล็ง (คลิกขวา M2 ค้าง) ตาม GDD
        isAiming = Input.GetMouseButton(1);

        // 2. ค่อยๆ ปรับขนาด Spread ปัจจุบันเข้าหาเป้าหมาย
        float targetSpread = isAiming ? aimSpread : hipFireSpread;
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, spreadSmoothSpeed * Time.deltaTime);

        // 3. วาดเส้นกรอบแรงดีด/ระยะยิง
        if (visualizer != null && firePoint != null)
        {
            visualizer.UpdateSpreadLines(firePoint.position, transform.forward, currentSpread);
        }

        // 4. การยิง (คลิกซ้าย M1) ตาม GDD
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        GunSound.Play();
        if (bulletPrefab == null || firePoint == null) return;

        // สุ่มมุมกระจายตาม currentSpread บนแกน Y
        float randomAngle = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);

        // ทิศทางสุดท้ายของกระสุน
        Vector3 shootDirection = spreadRotation * firePoint.forward;
        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);

        // Spawn กระสุน
        Instantiate(bulletPrefab, firePoint.position, bulletRotation);
    }
}