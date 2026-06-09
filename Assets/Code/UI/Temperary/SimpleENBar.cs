using UnityEngine;
using System;
using System.Collections;

public class SimpleENBar : MonoBehaviour
{
    private UnityEngine.UI.Slider m_hpBar;
    private PlayerModel m_playerModel;

    private IEnumerator TryGetPlayerModel()
    {
        // 等待 LevelManager 初始化
        while (LevelManager.m_Instance == null)
            yield return null;

        // 等待 CurrentPlayer 准备好
        while (LevelManager.m_Instance.CurrentPlayer == null)
            yield return null;

        // 等待 PlayerModel 组件
        while (m_playerModel == null)
        {
            LevelManager.m_Instance.CurrentPlayer.TryGetComponent<PlayerModel>(out m_playerModel);
            yield return null;
        }

        m_playerModel.OnEnergyUpdate += UpdateValue;
    }


    private void UpdateValue(int value, int oldValue, int MaxHP)
    {
        float maxEN = m_playerModel.m_currentAttributes.playerAttributes.MaxEnergy;
        m_hpBar.value = (float)value / maxEN;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_hpBar = GetComponent<UnityEngine.UI.Slider>();
        m_hpBar.value = 0;
        StartCoroutine(TryGetPlayerModel());
    }
}
