using System.Collections;
using UnityEngine;
using TMPro; // Add this for UI

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject primaryWeaponPrefab;
    public Transform weaponHolder;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Aiming Positions")]
    public Transform hipPosition;
    public Transform adsPosition;

    [Header("Settings")]
    public float aimSpeed = 10.0f;

    [Header("Recoil Settings")]
    public float recoilZ = -0.1f;
    public float recoilX = 1.0f;
    public float recoilY = 0.5f;
    public float recoilReturnSpeed = 10f;
    public float recoilSnappiness = 6f;

    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("Reload Sounds")]
    public AudioClip partialReloadSound;
    public AudioClip emptyReloadSound;

    [Header("UI Elements")]
    public TextMeshProUGUI ammoText;

    private GameObject currentWeapon;
    private AudioSource audioSource;
    private Animator animator;

    private Vector3 targetRecoil;
    private Vector3 currentRecoil;
    private Vector3 originalWeaponPosition;

    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        EquipPrimaryWeapon();
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        if (weaponHolder != null)
            originalWeaponPosition = weaponHolder.localPosition;
    }

    void Update()
    {
        if (currentWeapon == null)
        {
            Debug.LogError("Weapon is missing! Trying to reattach.");
            EquipPrimaryWeapon();
            return;
        }

        HandleADS();

        if (isReloading) return;

        if (currentAmmo <= 0 && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Out of ammo! Reload!");
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            FireWeapon();
        }

        HandleRecoil();
    }

    void FireWeapon()
    {
        Debug.Log("Playing fire sound");
        if (fireSound != null && audioSource != null)
            audioSource.PlayOneShot(fireSound);
            if (audioSource == null)
        {
            Debug.LogError("AudioSource not found in weapon prefab!");
            }
            else
            {
            Debug.Log("AudioSource successfully assigned.");
}

        if (animator != null)
        {
            animator.SetTrigger("Fire");
            animator.SetBool("IsFiring", true);
            StartCoroutine(ResetFireAnimation());
        }

        currentAmmo--;
        UpdateAmmoUI();

        bool isAiming = Input.GetMouseButton(1);
        float adsMultiplier = isAiming ? 0.5f : 1f;

        targetRecoil += new Vector3(
            recoilX * adsMultiplier,
            Random.Range(-recoilY, recoilY) * adsMultiplier,
            recoilZ * adsMultiplier
        );
    }

    IEnumerator ResetFireAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        if (animator != null)
            animator.SetBool("IsFiring", false);
    }

    IEnumerator Reload()
{
    if (isReloading || currentAmmo == maxAmmo) yield break;

    isReloading = true;

    bool isPartial = currentAmmo > 0;

    if (animator != null)
    {
        if (isPartial)
            animator.SetTrigger("ReloadPartial");
        else
            animator.SetTrigger("ReloadEmpty");
    }

    // Play correct reload sound
    if (audioSource != null)
    {
        AudioClip reloadClip = isPartial ? partialReloadSound : emptyReloadSound;
        if (reloadClip != null)
            audioSource.PlayOneShot(reloadClip);
        else
            Debug.LogWarning("Missing reload sound clip!");
    }

    yield return new WaitForSeconds(reloadTime); // Adjust based on animation

    currentAmmo = maxAmmo;
    isReloading = false;
}


    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
    }

    public void ReplaceCurrentWeapon(GameObject newWeaponPrefab)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(newWeaponPrefab, weaponHolder);
        SetupWeapon(currentWeapon);
        Debug.Log("New weapon equipped!");
    }

    void EquipPrimaryWeapon()
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        if (primaryWeaponPrefab != null)
        {
            currentWeapon = Instantiate(primaryWeaponPrefab, weaponHolder);
            SetupWeapon(currentWeapon);
            Debug.Log("Primary weapon equipped!");
        }
        else
        {
            Debug.LogError("Primary weapon prefab is not assigned!");
        }
    }

    void SetupWeapon(GameObject weapon)
    {
        if (weapon == null) return;

        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.Euler(0, 180, 0);

        animator = weapon.GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator not found on weapon prefab!");

        originalWeaponPosition = weaponHolder.localPosition;
    }

    void HandleADS()
    {
        if (hipPosition == null || adsPosition == null || weaponHolder == null)
        {
            Debug.LogError("One of the position references is missing!");
            return;
        }

        bool isAiming = Input.GetMouseButton(1);

        Vector3 targetPosition = isAiming ? adsPosition.localPosition : hipPosition.localPosition;
        Quaternion targetRotation = isAiming ? adsPosition.localRotation : hipPosition.localRotation;

        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetPosition, Time.deltaTime * aimSpeed);
        weaponHolder.localRotation = Quaternion.Lerp(weaponHolder.localRotation, targetRotation, Time.deltaTime * aimSpeed);
    }

    void HandleRecoil()
    {
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRecoil = Vector3.Slerp(currentRecoil, targetRecoil, recoilSnappiness * Time.deltaTime);

        weaponHolder.localRotation *= Quaternion.Euler(-currentRecoil.x, currentRecoil.y, 0f);
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalWeaponPosition + new Vector3(0f, 0f, targetRecoil.z), Time.deltaTime * recoilReturnSpeed);
    }
}
