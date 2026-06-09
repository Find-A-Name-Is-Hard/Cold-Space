using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRegister : MonoBehaviour
{
    private bool m_isRegister = false;
    private IEnumerator TryRegisterSelfToLevel()
    {
        float timer = 0;


        while (true)
        {
            if (timer > 5)
            {
                Debug.LogError($"Cannot register enemy {gameObject.name} to level manager");
            }
            if (!m_isRegister && LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.CurretEnemy = gameObject;
                m_isRegister = true;
                break;
            }
            yield return null;
            timer += Time.deltaTime;
        }
        yield break;        
    }

    private void OnEnable()
    {
        StartCoroutine(TryRegisterSelfToLevel());
    }

    private void OnDisable()
    {
        if (m_isRegister 
            && LevelManager.m_Instance != null 
            && LevelManager.m_Instance.CurretEnemy == gameObject)
        {
            LevelManager.m_Instance.CurretEnemy = null;
            m_isRegister = false;
        }
    }
}
