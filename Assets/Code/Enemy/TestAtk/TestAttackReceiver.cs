using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// This script focuses on handling input, event and updating data. 
/// It will not change any visual elements directly, such as the shield animation.
/// But this scrip will provide some action delegates for visual scripts to subscribe.
/// </summary>

public class TestAttackReceiver : MonoBehaviour, IAtkReceiver
{
    #region TestAttack Receiver
    //public System.Collections.Generic.List<EEnenmyType> m_EnemyTypes = new();
    public float m_testShieldMaxHP = 200f;
    public float m_testShieldCurrentHP = 200f;
    public string m_playerTestAtkTag = "PlayerTestAttack";

    private EnemyAiManager m_EnemyManager;

    private void HandleTestAtkStartEvent(int keyNum)
    {
        m_testShieldCurrentHP = m_testShieldMaxHP;
    }

    private IEnumerator TryBindTestAtkStart()
    {
        while (true)
        {
            if(LevelManager.m_Instance!=null)
            {
                LevelManager.m_Instance.OnTestAtkStart += HandleTestAtkStartEvent;
                break;
            }
            yield return null;
        }
        yield break;
    }

    private bool ValidateClue(HardClues clue)
    {
        

        foreach (var value in m_EnemyManager.enemAttributes.clues)
        {
            if(value.hardClue == clue) return true;
        }

        return false;
    }
    #endregion

    #region MonoBehavior
    private void OnEnable()
    {
        m_testShieldCurrentHP = m_testShieldMaxHP;
    }

    private void Start()
    {
        m_EnemyManager = FindFirstObjectByType<EnemyAiManager>();
    }

    #endregion

    #region IAtkReceiver
    public void GetDamge(GameObject caster, float value, HardClues attackType)
    {
        if (caster.CompareTag(m_playerTestAtkTag))
        {
            if(LevelManager.m_Instance != null)
            {
                if (attackType == HardClues.Flammable)
                    LevelManager.m_Instance.NotifyPlyaerHitHappen(PlayerHitEvent.FlammableTest);
                if (attackType == HardClues.LightSensitive)
                    LevelManager.m_Instance.NotifyPlyaerHitHappen(PlayerHitEvent.LightningTest);
                if (attackType == HardClues.Magnetic)
                    LevelManager.m_Instance.NotifyPlyaerHitHappen(PlayerHitEvent.MagneticTest);
                if (attackType == HardClues.Explosive)
                    LevelManager.m_Instance.NotifyPlyaerHitHappen(PlayerHitEvent.ExplosiveTest);
            }

            m_testShieldCurrentHP -= value;
            if(m_testShieldCurrentHP <= 0)
            {
                m_testShieldCurrentHP = 0;

                Debug.Log(attackType);

                if (ValidateClue(attackType)) LevelManager.m_Instance.NotifyTestAtkEnd(true, attackType);
                else LevelManager.m_Instance.NotifyTestAtkEnd(false, attackType);


            }
        }        
    }

    #endregion
}
