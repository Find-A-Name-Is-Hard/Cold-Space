using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    private Dictionary<string, Stack<GameObject>> m_unusedPool = new();

    [SerializeField] private int m_maxCapacityPerItem = 1000;
    public int MaxCapacityPerItem { get { return m_maxCapacityPerItem; } }

    [SerializeField] private int m_AsyncPreloadNumberPerFrame = 10;

    public static GameObjectPool m_instance;

    private void Awake()
    {
        m_instance = this;
    }

    /// <summary>
    /// <para>Get GameObject. The gameobject is reponsible to disable it by itself</para>
    /// <para>Return value is unreliable. It might be null in next frame, plase validate it! </para>
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObject GetSelfManagedGameObject(GameObject go, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string name = "")
    {
        if(go == null)
        {
            Debug.LogError($"Trying to get game object with null reference. {Environment.NewLine}" +
                $"From {Path.GetFileName(file)}.{name}, line: {line}" +
                $"(at {file}:{line})");
            throw new System.ArgumentNullException();
        }
        string goName = go.name;

        // If the gameobject is not registered in the pool, adding new mapping relationship at first
        if (!m_unusedPool.ContainsKey(goName))
        {
            Stack<GameObject> stack = new Stack<GameObject>();
            m_unusedPool.Add(goName, stack);
        }

        // If there is no available gameobject in stack, preloading it, popping last GO and return it
        if (m_unusedPool[goName].Count <= 0)
        {
            SyncPreloadSelfManagedGO(go, m_unusedPool[goName], 10, file, line, name);
            if (m_unusedPool[goName].Count <= m_maxCapacityPerItem)
                AsyncPreloadSelfManagedGO(go, m_unusedPool[goName], 50, file, line, name);
        }

        // Get last valid game object
        GameObject temp = null;
        while(temp == null && m_unusedPool[goName].Count > 0)
        {
            temp = m_unusedPool[goName].Pop();
        }

        // If there is no valid game object, recurse to get a valid gameobject
        if(temp == null)
        {
            temp = GetSelfManagedGameObject(go, file, line, name);
        }

        temp.SetActive(true);

        return temp;
    }

    public void AsyncPreloadSelfManagedGameObject(GameObject go, int preloadCount, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string name = "")
    {
        if (go == null)
        {
            Debug.LogError($"Trying to preload game object with null reference. {Environment.NewLine}" +
                $"From {Path.GetFileName(file)}.{name}, line: {line}" +
                $"(at {file}:{line})");
            throw new System.ArgumentNullException();
        }
        string goName = go.name;

        // If the gameobject is not registered in the pool, adding new mapping relationship at first
        if (!m_unusedPool.ContainsKey(goName))
        {
            Stack<GameObject> stack = new Stack<GameObject>();
            m_unusedPool.Add(goName, stack);
        }

        if (m_unusedPool[goName].Count <= m_maxCapacityPerItem)
            AsyncPreloadSelfManagedGO(go, m_unusedPool[goName], preloadCount, file, line, name);
    }

    /// <summary>
    /// Return GO to the unused pool. But if the stack size is over max capacity, destroy it. 
    /// <para>The return value is true, if the object is recycled successfully, or it is destoryed by this recycle method</para>
    /// </summary>
    /// <param name="go"></param>
    public bool Recycle(GameObject go)
    {
        if(go == null)
        {
            return false;
        }

        string goName = go.name;

        if(!m_unusedPool.ContainsKey(goName))
        {
            return false;
        }

        if (m_unusedPool[goName].Contains(go))
        {
            return true;
        }

        if(m_unusedPool[goName].Count > m_maxCapacityPerItem)
        {
            Destroy(go);
            return true;
        }

        m_unusedPool[goName].Push(go);
        return true;
    }

    public void CountPoolObjects()
    {
        string str = $"{Environment.NewLine}We have {m_unusedPool.Count} bullets in pool {Environment.NewLine}";
        foreach (var kv in m_unusedPool)
        {
            str += $"{kv.Key} : {kv.Value.Count} {Environment.NewLine}";
        }
        Debug.Log(str);
        //Debug.Log();
    }

    /// <summary>
    /// Preload GO and inject self managed helper. The preload number is set in argument
    /// </summary>
    /// <param name="go"></param>
    private void SyncPreloadSelfManagedGO(GameObject go, Stack<GameObject> targetContainer, int count,
        [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string name = "")
    {
        if (go == null || targetContainer == null || count <= 0)
        {
            Debug.LogError($"Detect invalid sync preload arguments.Click me jumping to caller" +
                $"go={go?.name ?? "null"}, " +
                $"targetContainer={targetContainer?.GetType().Name ?? "null"}, " +
                $"count={count}" +
                $"Called from {Path.GetFileName(file)}.{name}, line: {line}" +
                $"(at {file}:{line})");

            throw new System.ArgumentException("Detect invalid sync preload arguments" +
                $"go={go?.name ?? "null"}, " +
                $"targetContainer={targetContainer?.GetType().Name ?? "null"}, " +
                $"count={count}" +
                $"Called from {Path.GetFileName(file)}.{name}, line: {line}");
        }

        for(int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(go);
            temp.name = go.name;
            temp.SetActive(false);
            if (temp.GetComponent<SelfRecycledHelper>() == null)
                temp.AddComponent<SelfRecycledHelper>();
            targetContainer.Push(temp);
        }
    }

    private void AsyncPreloadSelfManagedGO(GameObject go, Stack<GameObject> targetContainer, int count, 
        [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string name = "")
    {
        StartCoroutine(AsyncPreloadCoroutine(go, targetContainer,count, file, line, name));
    }

    private IEnumerator AsyncPreloadCoroutine(GameObject go, Stack<GameObject> targetContainer, int targetCount,
        [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string name = "")
    {
        if (this == null) yield break;
        int totalCount = 0;
        int index = 0;

        while (totalCount < targetCount)
        {
            if (this == null) yield break;
            SyncPreloadSelfManagedGO(go, targetContainer, 1, file, line, name);
            
            index++;
            totalCount++;

            if(index >= m_AsyncPreloadNumberPerFrame)
            {
                yield return null;
                index = 0;
            }
        }

        yield break;
    }
}
