using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CureAtkReceiver : MonoBehaviour
{
    public EnemyAiManager m_EnemyAiManager;
    #region MonoBehavior
    private void Start()
    {
        m_EnemyAiManager = FindFirstObjectByType<EnemyAiManager>();
    }
    #endregion

    #region CureAtkReceiver
    public void GetCure(List<HardClues> clues)
    {
        if(LevelManager.m_Instance != null)
        {
            LevelManager.m_Instance.NotifyPlyaerHitHappen(PlayerHitEvent.CureAtk);
        }

        if (ValidateClues(clues))
        {
            LevelManager.m_Instance.OnCureHappen(true, clues.ToArray());
            //Debug.Log($"You win");
            //Return Main
            LevelManager.m_Instance.retMain(false);
            Destroy(gameObject);
        }
        else
        {
            LevelManager.m_Instance.OnCureHappen(false, clues.ToArray());
            //Debug.Log($"Not correct");
        }

        //Debug.Log("You Win!");
        //Destroy(gameObject);
    }

    private bool ValidateClues(List<HardClues> clues)
    {
        int count = 0;
        int targetValue = m_EnemyAiManager.enemAttributes.clues.Length;

        foreach (var value in m_EnemyAiManager.enemAttributes.clues)
        {
            if(clues.Contains(value.hardClue))
            {
                count++;
            }
        }

        if (count == targetValue)
        {
            return true;
        }

        return false;
    }

    #endregion
}
