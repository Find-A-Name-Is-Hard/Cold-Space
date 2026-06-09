using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using USCG.Core.Telemetry;

public class PlayerMonitor : MonoBehaviour
{
    [SerializeField] private string m_metricKeyWord;

    /// <summary>
    /// This value will be update in player position update broadcast
    /// </summary>
    private Vector3 m_playerPosition; 
    private MetricId m_metricId;
    private bool m_isSubscribed = false;

    

    private IEnumerator TrySubscribeEvent()
    {
        float timer = 0;
        while(timer <= 3 && !m_isSubscribed)
        {
            if(LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerHealthUpdate += HandlePlayerHealthUpdate;
                LevelManager.m_Instance.OnCureHappen += HandleCureEvent;
                LevelManager.m_Instance.OnPlayerPositionUpdate += HandlePlayerPositionUpdate;
                m_isSubscribed = true;
                //Debug.Log($"<color=Red>isSubcribed? : {m_isSubscribed}</color>");
            }

            yield return null;
        }
        
        yield break;
    }

    private void UnsubscribeEvent()
    {
        if(m_isSubscribed && LevelManager.m_Instance != null)
        {
            LevelManager.m_Instance.OnPlayerHealthUpdate -= HandlePlayerHealthUpdate;
            LevelManager.m_Instance.OnCureHappen -= HandleCureEvent;
            LevelManager.m_Instance.OnPlayerPositionUpdate -= HandlePlayerPositionUpdate;
        }
        m_isSubscribed = false;
    }

    private void OnSceneUnloaded(Scene arg0)
    {
        UnsubscribeEvent();
        m_playerPosition = new Vector3(-999, -999, -999);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(TrySubscribeEvent());
    }

    private void HandleCureEvent(bool doesCureSucceed, HardClues[] clues)
    {
        if (doesCureSucceed && PlayerLoggingManager.instance != null)
        {
            PlayerLog log = GeneratePlayerLog(true);

            PlayerLoggingManager.instance.AddPlayerLogMetric(m_metricId, log);
            log = null;
        }
    }

    private void HandlePlayerHealthUpdate(int currentHP, int MaxHP)
    {
        if (currentHP <= 0 && PlayerLoggingManager.instance != null)
        {
            PlayerLog log = GeneratePlayerLog(false);

            PlayerLoggingManager.instance.AddPlayerLogMetric(m_metricId, log);
            log = null;
        }
    }

    private void HandlePlayerPositionUpdate(Vector3 positionr)
    {
        m_playerPosition = positionr;
    }

    private PlayerLog GeneratePlayerLog(bool isWin)
    {
        // Get current system time
        DateTime now = DateTime.Now;
        int year = now.Year;
        int month = now.Month;
        int day = now.Day;

        int hour = now.Hour;
        int minute = now.Minute;
        int second = now.Second;

        PlayerLog log = new PlayerLog();

        log.m_RecordTime = $"{year}-{month}-{day} {hour}:{minute}:{second}";
        log.m_PlayTime = LevelManager.m_Instance.LevelTimer;
        log.m_IsWin = isWin;

        string difficulty = "Failed to find difficulty";
        EnemyDifficulty enemyDifficulty = FindFirstObjectByType<EnemyDifficulty>();
        if ((enemyDifficulty != null))
        {
            difficulty = enemyDifficulty.difficulty.ToString();
        }
        log.m_Difficulty = difficulty;

        string lastUsedAttack = "Failed to find attack value";
        EnemyAiManager m_enemyAiManager = FindFirstObjectByType<EnemyAiManager>();
        if (m_enemyAiManager != null && m_enemyAiManager.lastUsedAttack != null)
        {
            lastUsedAttack = m_enemyAiManager.lastUsedAttack.name;
        }
        log.m_LastEnemyAttack = lastUsedAttack;
        log.m_LastPlayerPosition = (m_playerPosition == null) ? new Vector3(-999, -999, -999) : m_playerPosition;

        return log;
    }

    #region MonoBehavior

    private void Start()
    {
        m_metricId = PlayerLoggingManager.instance.CreatePlayerLogMetric(m_metricKeyWord);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }    
    #endregion
}
