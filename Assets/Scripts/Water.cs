using UnityEngine;

public class Water : MonoBehaviour
{
    public Transform WaterSplashPrefab;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(WaterSplashPrefab, other.transform.position, WaterSplashPrefab.rotation);
    }
}
