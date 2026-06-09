using UnityEngine;

public class ChargingBulletHitEffect : MonoBehaviour
{
    [SerializeField] private GameObject m_HitEffect;

    private void OnDisable()
    {
        if (!Application.isPlaying) return;
        GameObject go = Instantiate(m_HitEffect, transform.position, Quaternion.identity);
    }
}
