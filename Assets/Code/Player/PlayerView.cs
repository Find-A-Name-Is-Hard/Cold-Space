using UnityEngine;
[RequireComponent(typeof(PlayerModel))]
public class PlayerView : MonoBehaviour
{
    #region Player View
    private PlayerModel m_model;
    #endregion

    #region MonoBehavior

    private void Start()
    {
        m_model = GetComponent<PlayerModel>();
    }

    private void FixedUpdate()
    {
        transform.position = m_model.Position;
    }
    #endregion
}
