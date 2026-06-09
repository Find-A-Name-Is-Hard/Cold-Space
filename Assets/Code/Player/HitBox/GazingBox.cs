using UnityEngine;

public class GazingBox : MonoBehaviour
{
    private PlayerController m_controller;
    [SerializeField] private string m_enemyBulletsHitBoxTag = "EnemyBullets";

    private void Start()
    {
        m_controller = LevelManager.m_Instance.CurrentPlayer.GetComponent<PlayerController>();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_enemyBulletsHitBoxTag)
        {
            m_controller.HandleEnergyChangeInput();
        }
    }

}
