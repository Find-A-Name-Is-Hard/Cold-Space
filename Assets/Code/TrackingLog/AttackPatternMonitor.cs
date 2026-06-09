using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using USCG.Core.Telemetry;

public class AttackPatternMonitor : MonoBehaviour
{
    private AttackPatternLog m_log;
    private Dictionary<string, MetricId> m_idMapping = new Dictionary<string, MetricId>();

    private bool m_isSubscribed = false;
    /// <summary>
    /// This value will be changed by player health update event
    /// It is going to keep tracking how much damage is taken by player
    /// </summary>
    private float m_playerCurrentHealth = -999;
    private float m_damage = 0;

    private float m_playerCurrentEN = -999;
    private float m_chargedEN = 0;

    private IEnumerator TrySubscribeEvent()
    {
        float timer = 0;
        while (timer <= 3 && !m_isSubscribed)
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnEnemyAtkPatternUpdate += HandleAtkPatternUpdate;
                LevelManager.m_Instance.OnPlayerHealthUpdate += HandlePlayerHealthUpdate;
                LevelManager.m_Instance.OnCureHappen += HandleCureEvent;
                LevelManager.m_Instance.OnPlayerEnergyUpdate += HandleChargedEvent;
                m_isSubscribed = true;
                //Debug.Log($"<color=Red>isSubcribed? : {m_isSubscribed}</color>");
            }

            yield return null;
        }

        yield break;
    }

    private void UnsubscribeEvent()
    {
        if (m_isSubscribed && LevelManager.m_Instance != null)
        {
            LevelManager.m_Instance.OnEnemyAtkPatternUpdate -= HandleAtkPatternUpdate;
            LevelManager.m_Instance.OnPlayerHealthUpdate -= HandlePlayerHealthUpdate;
            LevelManager.m_Instance.OnCureHappen -= HandleCureEvent;
            LevelManager.m_Instance.OnPlayerEnergyUpdate -= HandleChargedEvent;
        }
        m_isSubscribed = false;
        //Debug.Log($"<color=Red>isSubcribed? : {m_isSubscribed}</color>");
    }

    private void OnSceneUnloaded(Scene arg0)
    {
        UpdateLogDamageAndEN();
        TryUploadAtkPatternLog();
        UnsubscribeEvent();
        m_playerCurrentHealth = -999;
        m_damage = 0;
        m_playerCurrentEN = -999;
        m_chargedEN = 0;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(TrySubscribeEvent());
        m_playerCurrentHealth = -999;
        m_damage = 0;
        m_playerCurrentEN = -999;
        m_chargedEN = 0;
    }

    private void HandleCureEvent(bool isSuccess, HardClues[] clues)
    {
        if(isSuccess)
        {
            UpdateLogDamageAndEN();
            TryUploadAtkPatternLog();
        }        
    }

    private void HandlePlayerHealthUpdate(int currentHP, int maxHP)
    {
        if(m_playerCurrentHealth == -999)
        {
            m_playerCurrentHealth = maxHP;
        }

        m_damage += m_playerCurrentHealth - currentHP;
        m_playerCurrentHealth = currentHP;

        if(currentHP <= 0)
        {
            m_log.m_IsCauseDeath = "True";
            UpdateLogDamageAndEN();
            TryUploadAtkPatternLog();
        }
    }

    private void HandleChargedEvent(int currentEN, int MaxEN)
    {
        if (m_playerCurrentEN == -999)
        {
            m_playerCurrentEN = 0;
        }

        // When player use cure / test attack, they consume all EN
        // But that's not what we want to track, we should ignore it
        if((currentEN -  m_playerCurrentEN) != -MaxEN)
        {
            m_chargedEN += currentEN - m_playerCurrentEN;
        }
        
        m_playerCurrentEN = currentEN;
    }

    private void HandleAtkPatternUpdate(AttackPatternData data)
    {
        UpdateLogDamageAndEN();
        TryUploadAtkPatternLog();
        m_log = StartNewEnemyAttackRecord(data);
    }

    private void TryUploadAtkPatternLog()
    {
        if (m_log != null && AttackPatternLogManager.instance != null)
        {
            MetricId id;

            if (!m_idMapping.TryGetValue(m_log.m_LogName, out id))
            {
                id = AttackPatternLogManager.instance.CreateAtkPatternLogMetric(m_log.m_LogName);
                m_idMapping.Add(m_log.m_LogName , id);
            }

            AttackPatternLogManager.instance.AddAtkPatternLogMetric(m_idMapping[m_log.m_LogName], m_log);
            m_log = null;
        }
    }

    private void UpdateLogDamageAndEN()
    {
        if(m_log == null) return;

        m_log.m_Damage = (m_damage).ToString();
        m_log.m_ENCharged = (m_chargedEN).ToString();
        m_chargedEN = 0;
        m_damage = 0;   
    }

    private AttackPatternLog StartNewEnemyAttackRecord(AttackPatternData AtkPatternData)
    {
        AttackPatternLog log = new AttackPatternLog();

        // Get current system time
        DateTime now = DateTime.Now;
        int year = now.Year;
        int month = now.Month;
        int day = now.Day;

        int hour = now.Hour;
        int minute = now.Minute;
        int second = now.Second;

        log.m_RecordTime = $"{year}-{month}-{day} {hour}:{minute}:{second}";
        log.m_LogName = AtkPatternData.name.Replace(',', ' ');
        log.m_Damage = "Failed to track damage";
        log.m_ENCharged = "Faild to track EN status";

        string difficulty = "Failed to find difficulty";
        EnemyDifficulty enemyDifficulty = FindFirstObjectByType<EnemyDifficulty>();
        if ((enemyDifficulty != null))
        {
            difficulty = enemyDifficulty.difficulty.ToString();
        }
        log.m_Difficulty = difficulty;
        log.m_IsCauseDeath = "False";

        List<AttackPatternTrackingData> dataSet = new List<AttackPatternTrackingData>();

        foreach (fromEnemyAimedBulletData bulletData in AtkPatternData.enemyAimedBulletDatas)
        {
            AttackPatternTrackingData trackingData = new AttackPatternTrackingData();
            trackingData.m_BulletDataName = bulletData.name;
            trackingData.m_SpawnInterval = bulletData.SpawnInterval.ToString().Replace(',',' ');
            trackingData.m_Count = bulletData.Count.ToString().Replace(',', ' ');
            trackingData.m_AngularAcceleration = "-";
            trackingData.m_MaxAngularVelocity = "-";
            trackingData.AngelInterval = bulletData.AngleInterval.ToString().Replace(",", " ");
            dataSet.Add(trackingData);
        }
        foreach (fromEnemyOriginBulletData bulletData in AtkPatternData.enemyOriginBulletDatas)
        {
            AttackPatternTrackingData trackingData = new AttackPatternTrackingData();
            trackingData.m_BulletDataName = bulletData.name;
            trackingData.m_SpawnInterval = bulletData.SpawnInterval.ToString().Replace(',', ' ');
            trackingData.m_Count = bulletData.Count.ToString().Replace(',', ' ');
            trackingData.m_AngularAcceleration = bulletData.AngularAcceleration.ToString().Replace(",", " ");
            trackingData.m_MaxAngularVelocity = bulletData.MaxAngularVelocity.ToString().Replace(",", " ");
            trackingData.AngelInterval = bulletData.AngleInterval.ToString().Replace(",", " ");
            dataSet.Add(trackingData);
        }
        foreach (fromLineOriginBulletData bulletData in AtkPatternData.lineOriginBulletDatas)
        {
            AttackPatternTrackingData trackingData = new AttackPatternTrackingData();
            trackingData.m_BulletDataName = bulletData.name;
            trackingData.m_SpawnInterval = bulletData.SpawnInterval.ToString().Replace(',', ' ');
            trackingData.m_Count = bulletData.Count.ToString().Replace(',', ' ');
            trackingData.m_AngularAcceleration = "-";
            trackingData.m_MaxAngularVelocity = "-";
            trackingData.AngelInterval = "-";
            dataSet.Add(trackingData);
        }

        log.m_TrackingDatas = dataSet;

        return log;
    }

    #region MonoBehavior

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        m_playerCurrentHealth = -999;
        m_damage = 0;
        m_playerCurrentEN = -999;
        m_chargedEN = 0;
    }
    #endregion
}
