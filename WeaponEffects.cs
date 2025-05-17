using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    [Header("Screen Shake (Recoil) Settings")]
    public float horizontalRecoil = 0.2f;  // Horizontal recoil effect
    public float verticalRecoil = 0.2f;    // Vertical recoil effect
    public float recoilDuration = 0.1f;    // Duration of the recoil effect
    private Vector3 originalCameraPosition;

    [Header("Weapon Sway Settings")]
    public float swayAmount = 0.05f;  // How much the weapon sways
    public float swaySpeed = 3.0f;    // Speed of weapon sway
    private Vector3 initialWeaponPosition;

    private Camera playerCamera;
    private Transform weaponTransform;

    private void Start()
    {
        playerCamera = Camera.main;
        weaponTransform = GetComponent<Transform>();
        originalCameraPosition = playerCamera.transform.localPosition;
        initialWeaponPosition = weaponTransform.localPosition;
    }

    private void Update()
    {
        // Handle weapon sway
        HandleWeaponSway();

        // Add screen shake (recoil) when the player fires the weapon
        if (Input.GetMouseButtonDown(0)) // Fire button (left-click)
        {
            StartCoroutine(ApplyRecoil());
        }
    }

    // Method to handle screen shake (recoil)
    private IEnumerator ApplyRecoil()
    {
        float elapsed = 0.0f;

        // Apply recoil during the fire action
        while (elapsed < recoilDuration)
        {
            float horizontalOffset = Random.Range(-horizontalRecoil, horizontalRecoil);
            float verticalOffset = Random.Range(-verticalRecoil, verticalRecoil);

            // Apply the recoil to the camera position
            playerCamera.transform.localPosition = originalCameraPosition + new Vector3(horizontalOffset, verticalOffset, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset camera position after recoil
        playerCamera.transform.localPosition = originalCameraPosition;
    }

    // Method to add weapon sway effect
    private void HandleWeaponSway()
    {
        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        Vector3 swayOffset = new Vector3(moveX, moveY, 0);
        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, initialWeaponPosition + swayOffset, Time.deltaTime * swaySpeed);
    }
}
