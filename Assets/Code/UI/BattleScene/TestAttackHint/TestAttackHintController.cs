using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TestAttackHintController : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;
    [SerializeField]private TextMeshProUGUI m_hintText;
    private float m_playerEnergy = 0;
    private float m_playerMaxEnergy = 999;

    private void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        ControlCanvasExhibition(false);
        InputHandle.m_Instance.TestAtkToggleEvent += HandleTestToggleEvent;
        InputHandle.m_Instance.TestAtkButtonPerfomedEvent += HandleTestAttackStart;
        LevelManager.m_Instance.OnPlayerEnergyUpdate += UpdatePlayerEnergyTrackingData;
    }

    private void HandleTestAttackStart(int weaponIndex)
    {
        ControlCanvasExhibition(false);
    }

    private void HandleTestToggleEvent(bool isToShow)
    {
        UpdateHintTextInfo();
        ControlCanvasExhibition(isToShow);
    }

    private void UpdatePlayerEnergyTrackingData(int current, int max)
    {
        m_playerEnergy = current;
        m_playerMaxEnergy = max;
    }

    private void UpdateHintTextInfo()
    {
        if(m_playerEnergy < m_playerMaxEnergy)
        {
            m_hintText.text = $"You don't have full energy to start test attack{Environment.NewLine}" +
                $"Press X to return battle";
        }
        else
        {
            m_hintText.text = $"Test Attack Weapons List {Environment.NewLine}" +
                $"1 : Light  2 : Fire  3 : Magnetic  4 : Explosive {Environment.NewLine}" +
                $"Press number to select weapon Or Press X to return battle";
        }
    }

    private void ControlCanvasExhibition(bool isToShow)
    {
        m_canvasGroup.alpha = isToShow ? 1 : 0;
        m_canvasGroup.interactable = isToShow;
        m_canvasGroup.blocksRaycasts = isToShow;
    }
}
