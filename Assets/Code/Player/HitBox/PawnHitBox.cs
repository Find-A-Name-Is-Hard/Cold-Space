using UnityEngine;

public class PawnHitBox : MonoBehaviour
{
    private PlayerController m_controller;
    [SerializeField] private string m_enemyBulletsHitBoxTag = "EnemyBullets";
    [SerializeField] private GameObject m_hitEffect;
    [SerializeField] private float m_hitEffectOffsetRange = 0.1f;
    [SerializeField] private float m_hitEffectAngleOffsetRange = 360;

    private void Start()
    {
        m_controller = LevelManager.m_Instance.CurrentPlayer.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == m_enemyBulletsHitBoxTag)
        //{
        //    m_controller.HandleDamageEvent(-10);
        //}
    }

    public void GetDamage(int damage)
    {
        m_controller.HandleDamageEvent(-damage);
        if(m_hitEffect != null)
        {
            Vector3 randamOffset = new() /*= new Vector3(Random.Range(0, m_hitEffectOffsetRange),
                                            Random.Range(0, m_hitEffectOffsetRange), 
                                            Random.Range(0, m_hitEffectOffsetRange))*/;
            randamOffset += -0.05f * Vector3.up + 0.0f * Vector3.right;
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, m_hitEffectAngleOffsetRange));
            Instantiate(m_hitEffect, transform.position + randamOffset, rotation, gameObject.transform);
        }
    }
}
