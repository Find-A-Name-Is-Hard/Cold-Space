using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ESCMenuController : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;
    private bool m_isShowing = false;

    public void PressShowingToggle()
    {
        HandleSettingMenuPerformedEvent();
    }

    private void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        ControlCanvasExhibition(false);
        InputHandle.m_Instance.SettingMenuPerformedEvent += HandleSettingMenuPerformedEvent;
    }

    private void HandleSettingMenuPerformedEvent()
    {
        m_isShowing = !m_isShowing;
        ControlCanvasExhibition(m_isShowing);
    }

    private void ControlCanvasExhibition(bool isToShow)
    {
        m_canvasGroup.alpha = isToShow ? 1 : 0;
        m_canvasGroup.interactable = isToShow;
        m_canvasGroup.blocksRaycasts = isToShow;
    }
}
