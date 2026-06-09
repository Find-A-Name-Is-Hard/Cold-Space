using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MainSelectionController : MonoBehaviour
{
    public bool m_IsActiveAtBeginning = true;

    private EventBus m_EB;
    private CanvasGroup m_ConvasGroup;

    private void OnEnable()
    {
        StartCoroutine(TrySubscribeEvents());
    }

    private void OnDisable()
    {
        if(m_EB != null)
        {
            m_EB.Unsubscribe<OnStartPressed>(HandleStartPressedEvent);
            m_EB.Unsubscribe<OnCreditPressed>(HandleCreditPressedEvent);
            m_EB.Unsubscribe<OnMannualPressed>(HandleMannualPressedEvent);
            m_EB.Unsubscribe<OnBackToMainPressed>(HandleBackToMainPressedEvent);
        }
    }

    private void Start()
    {
        m_ConvasGroup = GetComponent<CanvasGroup>();
        m_EB = FindFirstObjectByType<EventBus>();
        SetConvasGroupActive(m_IsActiveAtBeginning);
    }

    private IEnumerator TrySubscribeEvents()
    {
        yield return new WaitUntil(() => m_EB != null);

        m_EB.Subscribe<OnStartPressed>(HandleStartPressedEvent);
        m_EB.Subscribe<OnCreditPressed>(HandleCreditPressedEvent);
        m_EB.Subscribe<OnMannualPressed>(HandleMannualPressedEvent);
        m_EB.Subscribe<OnBackToMainPressed>(HandleBackToMainPressedEvent);

        yield break;
    }
    private void HandleBackToMainPressedEvent(OnBackToMainPressed pressed)
    {
        SetConvasGroupActive(true);
    }

    private void HandleStartPressedEvent(OnStartPressed eventData)
    {
         SetConvasGroupActive(false);
    }

    private void HandleMannualPressedEvent(OnMannualPressed eventData)
    {
        SetConvasGroupActive(false);
    }

    private void HandleCreditPressedEvent(OnCreditPressed eventData)
    {
        SetConvasGroupActive(false);
    }

    private void SetConvasGroupActive(bool value)
    {
        m_ConvasGroup.alpha = value ? 1 : 0;
        m_ConvasGroup.interactable = value;
        m_ConvasGroup.blocksRaycasts = value;
    }
}
