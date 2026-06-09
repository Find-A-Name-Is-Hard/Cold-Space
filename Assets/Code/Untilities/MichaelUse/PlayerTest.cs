using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private InputAction m_moveAction;

    private void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_moveAction = m_playerInput.actions["Move"];
    }

    void Update()
    {
        // ∂¡»° ‰»Î÷µ
        Vector2 moveInput = m_moveAction.ReadValue<Vector2>();
        transform.Translate(new Vector3(moveInput.x, moveInput.y, transform.position.z) * Time.deltaTime * 7f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        int enemyBulletsLayer = LayerMask.NameToLayer("EnemyBullets");

        if (other.gameObject.layer == enemyBulletsLayer)
        {
            Debug.Log("Player injured by enemy bullet!");
        }
    }
}