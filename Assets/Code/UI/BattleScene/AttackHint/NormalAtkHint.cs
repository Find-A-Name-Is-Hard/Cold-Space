using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class NormalAtkHint : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;
    private bool m_isHintShowing = true;

    private void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_isHintShowing = true;
        ControlCanvasExhibition(m_isHintShowing);
        InputHandle.m_Instance.TestAtkToggleEvent += HandleTestToggleEvent;
        InputHandle.m_Instance.TestAtkButtonPerfomedEvent += HandleTestAttackStart;
        InputHandle.m_Instance.CureAtkToggleEvent += HandleCureToggleEvent;
        InputHandle.m_Instance.CureAtkButtonPerfomedEvent += HandleCureAttackStart;
    }

    private void HandleCureAttackStart(int obj)
    {
        HintShowingToggle();
    }

    private void HandleCureToggleEvent(bool obj)
    {
        HintShowingToggle();
    }

    private void HandleTestAttackStart(int obj)
    {
        HintShowingToggle();
    }

    private void HandleTestToggleEvent(bool obj)
    {
        HintShowingToggle();
    }

    private void HintShowingToggle()
    {
        m_isHintShowing = ! m_isHintShowing;
        ControlCanvasExhibition(m_isHintShowing);
    }

    private void ControlCanvasExhibition(bool isToShow)
    {
        m_canvasGroup.alpha = isToShow ? 1 : 0;
        m_canvasGroup.interactable = isToShow;
        m_canvasGroup.blocksRaycasts = isToShow;
    }
}
