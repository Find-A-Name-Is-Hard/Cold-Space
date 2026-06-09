using UnityEngine;

/// <summary>
/// Being called on disable time. Recycle it self to gameobject pool
/// </summary>
public class SelfRecycledHelper : MonoBehaviour
{
    private void OnDisable()
    {
        if(GameObjectPool.m_instance == null)
        {
            Destroy(gameObject);
            return;
        }
        
        bool isRecycledSuccess = GameObjectPool.m_instance.Recycle(gameObject);
        if(!isRecycledSuccess)
        {
            Destroy(gameObject);
        }
    }
}
