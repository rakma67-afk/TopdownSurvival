//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ flash
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WeaponSpreadVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private float range = 15f; // ระยะความยาวเส้นมองเห็น
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3; // 3 จุด: ปลายซ้าย -> ปากกระบอก -> ปลายขวา
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    // ฟังก์ชันอัปเดตเส้นตามตำแหน่งปากกระบอกและมุม Spread
    public void UpdateSpreadLines(Vector3 firePoint, Vector3 forwardDir, float currentSpreadAngle)
    {
        // คำนวณเวกเตอร์ปลายซ้ายและปลายขวาตามองศา Spread
        Quaternion leftRot = Quaternion.AngleAxis(-currentSpreadAngle, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(currentSpreadAngle, Vector3.up);

        Vector3 leftDir = leftRot * forwardDir;
        Vector3 rightDir = rightRot * forwardDir;

        Vector3 leftTarget = firePoint + leftDir * range;
        Vector3 rightTarget = firePoint + rightDir * range;

        // ดึงตำแหน่งปลายเส้นให้แนบพื้น
        lineRenderer.SetPosition(0, leftTarget);
        lineRenderer.SetPosition(1, firePoint);
        lineRenderer.SetPosition(2, rightTarget);
    }

    public void SetRange(float newRange)
    {
        range = newRange;
    }
}