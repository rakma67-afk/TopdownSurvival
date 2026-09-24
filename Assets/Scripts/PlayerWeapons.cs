using UnityEngine;
using System.Collections; // จำเป็นสำหรับการใช้ Coroutine (IEnumerator)

[RequireComponent(typeof(AudioSource))] // บังคับให้ GameObject นี้ต้องมี AudioSource เสมอ
public class PlayerWeapon : MonoBehaviour
{
    [Header("Inventory (ใส่ Weapon Data 4 ชิ้น)")]
    [SerializeField] private WeaponData[] weaponSlots = new WeaponData[4];
    private int[] currentAmmos = new int[4]; // เก็บจำนวนกระสุนปัจจุบันของช่อง 1-4
    private WeaponData currentWeapon;
    private int currentSlot = 0;

    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponSpreadVisualizer visualizer;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Settings")]
    [SerializeField] private float reloadTime = 1.5f; // ใช้เวลาเปลี่ยนกระสุนกี่วินาที

    private float nextTimeToFire = 0f;
    private float currentSpread;
    private bool isAiming;
    private bool isReloading = false; // เช็คว่ากำลังรีโหลดอยู่หรือไม่

    private AudioSource audioSource;
    private PlayerController playerController;

    public float AimWalkSpeed = 3f;
    public bool IsAiming => isAiming;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        // เติมกระสุนให้เต็มทุกกระบอกในตอนเริ่มเกม
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null && !weaponSlots[i].isMelee)
            {
                currentAmmos[i] = weaponSlots[i].maxAmmo;
            }
        }

        EquipWeapon(0);
    }

    private void Update()
    {
        // ถ้ากำลังรีโหลดอยู่ จะไม่สามารถยิง เล็ง หรือสลับอาวุธได้
        if (isReloading) return;

        HandleWeaponSwitching();

        if (currentWeapon == null) return;

        // 1. การเล็งและปรับเส้น Visualizer
        isAiming = Input.GetMouseButton(1);
        float targetSpread = isAiming ? currentWeapon.aimSpread : currentWeapon.hipSpread;
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, 15f * Time.deltaTime);

        if (visualizer != null && firePoint != null)
        {
            visualizer.UpdateSpreadLines(firePoint.position, transform.forward, currentSpread);
            visualizer.SetRange(currentWeapon.range);
        }

        // 2. ระบบเปลี่ยนกระสุนแบบกดเอง (กด R)
        // ทำงานเมื่อ ไม่ใช่อาวุธประชิด และ กระสุนในแมกกาซีนยังไม่เต็ม
        if (Input.GetKeyDown(KeyCode.R) && !currentWeapon.isMelee && currentAmmos[currentSlot] < currentWeapon.maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        // 3. ระบบโจมตี (คลิกซ้าย)
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            // ถ้ายิงปืนและกระสุนหมด ให้ทำการรีโหลดอัตโนมัติ
            if (!currentWeapon.isMelee && currentAmmos[currentSlot] <= 0)
            {
                StartCoroutine(Reload());
            }
            else
            {
                nextTimeToFire = Time.time + currentWeapon.fireRate;
                Attack();
            }
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EquipWeapon(3);
    }

    private void EquipWeapon(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Length && weaponSlots[slotIndex] != null)
        {
            currentSlot = slotIndex;
            currentWeapon = weaponSlots[currentSlot];
            currentSpread = currentWeapon.hipSpread;
            Debug.Log($"Equipped: {currentWeapon.weaponName} | Ammo: {currentAmmos[currentSlot]} / {currentWeapon.maxAmmo}");
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // เล่นเสียงรีโหลดถ้ามีการใส่ไฟล์เสียงไว้
        if (currentWeapon.reloadSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.reloadSound);
        }

        // รอเวลาให้รีโหลดเสร็จ
        yield return new WaitForSeconds(reloadTime);

        // เติมกระสุนให้เต็มและปลดล็อคสถานะ
        currentAmmos[currentSlot] = currentWeapon.maxAmmo;
        isReloading = false;

        Debug.Log("Reload Complete!");
    }

    private void Attack()
    {
        // เล่นเสียงโจมตี (ปรับ Pitch แบบสุ่มเล็กน้อยให้เสียงปืนไม่น่าเบื่อ)
        if (currentWeapon.attackSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(currentWeapon.attackSound);
        }

        if (currentWeapon.isMelee)
        {
            MeleeAttack();
        }
        else
        {
            ShootRanged();
            currentAmmos[currentSlot]--; // ลดยอดกระสุนลง 1 นัดเมื่อยิง
        }
    }

    private void ShootRanged()
    {
        if (bulletPrefab == null || firePoint == null) return;
        float randomAngle = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Vector3 shootDirection = spreadRotation * firePoint.forward;
        Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
    }

    private void MeleeAttack()
    {
        // ค้นหาวัตถุทั้งหมดในระยะอาวุธ ที่อยู่ใน Layer ของศัตรู
        Collider[] hitEnemies = Physics.OverlapSphere(firePoint.position, currentWeapon.range, enemyLayer);

        foreach (Collider hitCollider in hitEnemies)
        {
            // คำนวณทิศทางจากตัวเราไปยังศัตรู
            Vector3 directionToEnemy = (hitCollider.transform.position - firePoint.position).normalized;

            // ตรวจสอบว่าศัตรูอยู่ในกรอบองศาการฟันด้านหน้าหรือไม่
            float angleToEnemy = Vector3.Angle(firePoint.forward, directionToEnemy);

            if (angleToEnemy <= currentSpread)
            {
                // ใช้ TryGetComponent เพื่อตรวจสอบว่าสิ่งที่ฟันโดนมีสคริปต์ EnemyController หรือไม่
                if (hitCollider.TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    // ส่งค่า Damage จากอาวุธปัจจุบัน ไปให้ฟังก์ชัน TakeDamage ของศัตรู
                    enemy.TakeDamage(currentWeapon.damage);

                    Debug.Log($"ฟันโดน {hitCollider.name} เข้าไป {currentWeapon.damage} ดาเมจ!");
                }
            }
        }
    }
}