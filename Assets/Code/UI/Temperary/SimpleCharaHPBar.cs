using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
[RequireComponent(typeof(Slider))]
public class SimpleCharaHPBar : MonoBehaviour
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

        m_playerModel.OnHealthUpdate += UpdateValue;
    }


    private void UpdateValue(int value,int oldValue, int maxEN)
    {
        float maxHP = m_playerModel.m_currentAttributes.playerAttributes.MaxHP;
        m_hpBar.value = (float)value / maxHP;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_hpBar = GetComponent<UnityEngine.UI.Slider>();
        StartCoroutine(TryGetPlayerModel());
    }


    // Update is called once per frame
    void Update()
    {

    }
}
