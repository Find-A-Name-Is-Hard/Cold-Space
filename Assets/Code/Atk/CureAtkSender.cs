using UnityEngine;
using System.Collections.Generic;

public class CureAtkSender : MonoBehaviour
{
    [SerializeField] public List<HardClues> m_CureTypes = new();
    public string m_EnemyBulletTag = "EnemyBullets";
    public string m_EnemyType = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == m_EnemyBulletTag)
            Destroy(collision.gameObject);

        if(collision.gameObject.tag == m_EnemyType)
        {
            CureAtkReceiver car;
            if(collision.gameObject.TryGetComponent<CureAtkReceiver>(out car))
            {
                car.GetCure(m_CureTypes);
            }
            else
            {
                // Destroy(collision.gameObject);
            }
        }
    }
}
