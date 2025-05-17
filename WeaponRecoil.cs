using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Hip Fire Recoil")]
    public Vector3 hipRecoilRotation = new Vector3(-5f, 2f, 0f);
    public float hipRecoilReturnSpeed = 10f;
    public float hipRecoilSnapiness = 10f;

    [Header("ADS Recoil")]
    public Vector3 adsRecoilRotation = new Vector3(-2f, 1f, 0f);
    public float adsRecoilReturnSpeed = 15f;
    public float adsRecoilSnapiness = 15f;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [HideInInspector] public bool isAiming;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, GetReturnSpeed() * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, GetSnapiness() * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void FireRecoil()
    {
        Vector3 recoil = isAiming ? adsRecoilRotation : hipRecoilRotation;
        targetRotation += recoil;
    }

    private float GetReturnSpeed()
    {
        return isAiming ? adsRecoilReturnSpeed : hipRecoilReturnSpeed;
    }

    private float GetSnapiness()
    {
        return isAiming ? adsRecoilSnapiness : hipRecoilSnapiness;
    }
}
