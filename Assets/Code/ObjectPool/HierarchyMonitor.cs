using UnityEngine;
using UnityEngine.SceneManagement;

public class HierarchyMonitor : MonoBehaviour
{
    public float interval = 1f;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            //Debug.Log($"Object pool: {GameObjectPool.m_instance.CountPoolObjects()}");
            GameObjectPool.m_instance.CountPoolObjects();
        }
    }

    //private int CountAllObjects()
    //{
    //    //int count = ;        

    //    //return count;
    //}

    private int CountRecursive(GameObject go)
    {
        int total = 1;
        foreach (Transform c in go.transform)
            total += CountRecursive(c.gameObject);
        return total;
    }
}
