using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CureAttackHintControllre : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;
    [SerializeField] private TextMeshProUGUI m_hintText;
    private float m_playerEnergy = 0;
    private float m_playerMaxEnergy = 999;

    private void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        ControlCanvasExhibition(false);
        InputHandle.m_Instance.CureAtkToggleEvent += HandleCureToggleEvent;
        InputHandle.m_Instance.CureAtkButtonPerfomedEvent += HandleCureAttackStart;
        LevelManager.m_Instance.OnPlayerEnergyUpdate += UpdatePlayerEnergyTrackingData;
    }

    private void HandleCureAttackStart(int weaponIndex)
    {
        ControlCanvasExhibition(false);
    }

    private void HandleCureToggleEvent(bool isToShow)
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
        if (m_playerEnergy < m_playerMaxEnergy)
        {
            m_hintText.text = $"You don't have full energy to start cure attack{Environment.NewLine}" +
                $"Press C to return battle";
        }
        else
        {
            m_hintText.text = $"Weaknesses Mapping Table {Environment.NewLine}" +
                $"1 : Fire+Magnetic  2 : Fire+Explosive  3 : Fire+Light  {Environment.NewLine}" +
                $"4 : Light+Magnetic 5 : Light+Explosive  6 : Explosive+Magnetic {Environment.NewLine}" +
                $"Press number to select weapon Or Press C to return battle";
        }
    }

    private void ControlCanvasExhibition(bool isToShow)
    {
        m_canvasGroup.alpha = isToShow ? 1 : 0;
        m_canvasGroup.interactable = isToShow;
        m_canvasGroup.blocksRaycasts = isToShow;
    }
}
