using UnityEngine;
using System.Collections.Generic;

public class PreloadHelper : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_preloadItems;
    [SerializeField] private int m_preloadCount;

    void Start()
    {
        foreach (var item in m_preloadItems)
        {
            GameObjectPool.m_instance.AsyncPreloadSelfManagedGameObject(item, m_preloadCount);
        }
    }
}
