using UnityEngine;

public class ChargerBullet : MonoBehaviour
{
    public string m_EnemyTag = "Enemy";
    public int m_ChargeEnergy = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == m_EnemyTag)
        {
            PlayerController controller = LevelManager.m_Instance.CurrentPlayer.GetComponent<PlayerController>();
            controller.HandleChargeEvent(m_ChargeEnergy);
            LevelManager.m_Instance.NotifyPlyaerHitHappen(PlayerHitEvent.NormalAtk);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        // If the bullet is not recyclable, destroy it. 
        if (GetComponent<SelfRecycledHelper>() == null)
        {
            Destroy(gameObject);
        }

        // If it is recycleable, disable it and trigger the recycle method in self recyced helper
        else
        {
            gameObject.SetActive(false);
        }
    }
}
