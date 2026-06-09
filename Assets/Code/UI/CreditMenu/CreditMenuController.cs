using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CreditMenuController : MonoBehaviour
{
    private CanvasGroup m_ConvasGroup;
    private EventBus m_EB;
    public bool m_IsActiveAtBeginning = false;

    private void OnEnable()
    {
        StartCoroutine(TrySubscribeEvents());
    }

    private void OnDisable()
    {
        m_EB.Unsubscribe<OnCreditPressed>(HandleCreditPressedEvent);
        m_EB.Unsubscribe<OnBackToMainPressed>(HandleBackToMainPressedEvent);
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

        m_EB.Subscribe<OnCreditPressed>(HandleCreditPressedEvent);
        m_EB.Subscribe<OnBackToMainPressed>(HandleBackToMainPressedEvent);

        yield break;
    }

    private void HandleBackToMainPressedEvent(OnBackToMainPressed pressed)
    {
        SetConvasGroupActive(false);
    }

    private void HandleCreditPressedEvent(OnCreditPressed pressed)
    {
        SetConvasGroupActive(true);
    }

    private void SetConvasGroupActive(bool value)
    {
        m_ConvasGroup.alpha = value ? 1 : 0;
        m_ConvasGroup.interactable = value;
        m_ConvasGroup.blocksRaycasts = value;
    }
}
