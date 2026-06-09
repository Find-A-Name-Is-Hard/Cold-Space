using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T m_Instance { get; private set; }

    protected virtual void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(m_Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
