/*
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 0.2f;
    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 moveDirection;
    private Quaternion targetRotation;
    private float nextShootTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        // ถ้าเกมจบแล้ว ไม่รับ Input
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            moveDirection = Vector3.zero;
            return;
        }

        ReadMovementInput();
        AimAtMouse();
        ReadShootingInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.wKey.isPressed)
            vertical += 1f;

        if (Keyboard.current.sKey.isPressed)
            vertical -= 1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal += 1f;

        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1f;

        Vector3 input =
            new Vector3(horizontal, 0f, vertical);

        moveDirection = input.normalized;
    }

    private void MovePlayer()
    {
        Vector3 velocity = moveDirection * moveSpeed;

        rb.linearVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );
    }

    private void AimAtMouse()
    {
        if (Mouse.current == null ||
            mainCamera == null)
        {
            return;
        }

        // ตำแหน่ง Mouse บน Screen
        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        // ยิง Ray จากกล้อง
        Ray ray =
            mainCamera.ScreenPointToRay(mousePosition);

        // สร้าง Plane สมมติในระดับเดียวกับ Player
        Plane groundPlane =
            new Plane(
                Vector3.up,
                transform.position
            );

        // ตรวจว่า Ray ตัด Plane หรือไม่
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint =
                ray.GetPoint(distance);

            Vector3 lookDirection =
                hitPoint - transform.position;

            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                targetRotation =
                    Quaternion.LookRotation(
                        lookDirection
                    );
            }
        }
    }

    private void RotatePlayer()
    {
        rb.MoveRotation(targetRotation);
    }

    private void ReadShootingInput()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.isPressed &&
            Time.time >= nextShootTime)
        {
            Shoot();

            nextShootTime =
                Time.time + shootCooldown;
        }
    }

    private void Shoot()
    {
        Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );
    }
}

// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ flash
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f;       // ความเร็วเดินปกติ
    [SerializeField] private float sprintSpeed = 10f;    // ความเร็วเมื่อกด Shift

    [Header("Aim Settings")]
    [SerializeField] private LayerMask groundLayer;      // Layer ของพื้น เพื่อให้ Raycast ยิงโดน

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 moveInput;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // รับค่าการเดิน WASD
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        // เช็คการกดปุ่ม Shift เพื่อวิ่งตาม GDD
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // หันหน้าตามตำแหน่ง Cursor เมาส์บนพื้น 3D
        RotateTowardsMouse();
    }

    private void FixedUpdate()
    {
        // เคลื่อนที่ตามแกน X, Z โดยล็อกแกน Y ไม่ให้กระโดดหรือลอย
        Vector3 targetVelocity = moveInput * currentSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void RotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
        {
            Vector3 targetPoint = hitInfo.point;
            targetPoint.y = transform.position.y; // ล็อกแกน Y ไม่ให้ตัวละครก้มหรือเงย

            Vector3 lookDirection = targetPoint - transform.position;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ flash
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;

    [Header("Acceleration / Deceleration")]
    [SerializeField] private float acceleration = 25f;  // อัตราการเร่งความเร็ว
    [SerializeField] private float deceleration = 35f;  // อัตราการเบรก/หยุด

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 367f; // องศาต่อวินาทีในการหัน
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 moveInput;
    private Vector3 currentMoveVelocity;

    private PlayerShooting playerShooting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        playerShooting = GetComponent<PlayerShooting>(); // ดึงคอมโพเนนต์มาใช้
    }

    private void Update()
    {
        // 1. รับ Input เดิน WASD
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. หมุนตัวแบบนุ่มนวลตามตำแหน่ง Cursor
        SmoothRotateTowardsMouse();
    }

    private void FixedUpdate()
    {
        // คำนวณความเร็วเป้าหมาย (เดิน/วิ่ง/หยุด)
        float targetSpeed = 0f;
        if (moveInput.sqrMagnitude > 0.001f)
        {
            if (playerShooting != null && playerShooting.IsAiming)
            {
                targetSpeed = playerShooting.AimWalkSpeed;
            }
            else
            {
                bool isSprinting = Input.GetKey(KeyCode.LeftShift);
                targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
            }
            

        }

        Vector3 targetVelocity = moveInput * targetSpeed;

        // เลือกว่าตอนนี้กำลังเร่งความเร็วหรือกำลังเบรกหยุด
        float rate = (moveInput.sqrMagnitude > 0.001f) ? acceleration : deceleration;

        // ค่อยๆ ปรับความเร็วปัจจุบันเข้าหาความเร็วเป้าหมายอย่างนุ่มนวล
        currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, targetVelocity, rate * Time.fixedDeltaTime);

        // ใส่ความเร็วให้ Rigidbody โดยยังคงรักษาแรงโน้มถ่วงแกน Y ไว้
        rb.linearVelocity = new Vector3(currentMoveVelocity.x, rb.linearVelocity.y, currentMoveVelocity.z);
    }

    private void SmoothRotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
        {
            Vector3 targetPoint = hitInfo.point;
            targetPoint.y = transform.position.y; // ระนาบเดียวกับตัวละคร

            Vector3 lookDirection = targetPoint - transform.position;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                // คำนวณมุมหมุนเป้าหมาย
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

                // ค่อยๆ หมุนตัวละครเข้าหามุมเป้าหมายด้วยความเร็ว rotationSpeed (องศา/วินาที)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}