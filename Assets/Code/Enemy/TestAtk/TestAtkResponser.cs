using System.Collections;
using UnityEngine;

/// <summary>
/// This class will enable shield when detect test attack start.
/// And disable shield when end.
/// </summary>
public class TestAtkResponser : MonoBehaviour
{
    public GameObject m_Shield;
    public System.Collections.Generic.List<HardClues> type = new();
    public EnemyAiManager m_EnemyAiManager;
    private bool m_isBindTestAtk = false;
    private bool m_isRegister = false;
    private bool m_isFindEnemyManager = false;

    private IEnumerator TryBindTestAtkEvent()
    {
        while (m_isBindTestAtk == false)
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestAtkStart += (keyNum) => { m_Shield.SetActive(true); };
                LevelManager.m_Instance.OnTestFinish += (isClueCorrect, enemyType) => { m_Shield.SetActive(false); };
                m_isBindTestAtk = true;
                break;
            }
            yield return null;
        }
        yield break;
    }
    private IEnumerator TryRegisterSelf()
    {
        while (!m_isRegister)
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.CurretEnemy = gameObject;
                m_isRegister = true;
                break;
            }
            yield return null;
        }
        yield break;
    }

    private IEnumerator TryFindEnemyManager()
    {
        while(true)
        {
            if(m_EnemyAiManager == null)
            {
                m_EnemyAiManager = FindFirstObjectByType<EnemyAiManager>();
                yield return null;
                continue;
            }

            if (m_EnemyAiManager.enemAttributes != null && m_EnemyAiManager.enemAttributes.clues != null)
            {
                foreach (var value in m_EnemyAiManager.enemAttributes.clues)
                {
                    type.Add(value.hardClue);
                }
                m_isFindEnemyManager = true;
                break;
            }

            yield return null;
        }

        yield break;
    }

    private void OnEnable()
    {
        StartCoroutine(TryBindTestAtkEvent());
        StartCoroutine(TryRegisterSelf());
        StartCoroutine(TryFindEnemyManager());
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        m_Shield.SetActive(false);
    }
}
