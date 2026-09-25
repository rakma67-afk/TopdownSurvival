using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("ข้อมูลพื้นฐาน")]
    public string weaponName;
    public bool isMelee;

    [Header("โมเดลอาวุธ (3D Model)")]
    public GameObject weaponPrefab;

    [Header("ค่าสถิติ (Stats)")]
    public float damage;
    public float fireRate;
    public float range;

    [Header("ระบบเล็งและแรงดีด (Spread)")]
    public float hipSpread;
    public float aimSpread;

    [Header("ลูกซอง (Shotgun Settings)")]
    public int bulletsPerShot = 1;

    [Header("กระสุน")]
    public int maxAmmo; // ความจุต่อ 1 แมกกาซีน

    [Header("เสียง (Audio)")]
    public AudioClip attackSound; // เสียงปืนยิง หรือ เสียงมีดฟัน
    public AudioClip reloadSound; // เสียงตอนเปลี่ยนกระสุน
}