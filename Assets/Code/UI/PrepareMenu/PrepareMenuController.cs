using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PrepareMenuController : MonoBehaviour
{
    public bool m_IsActiveAtBeginning = false;

    private CanvasGroup m_ConvasGroup;
    private EventBus m_eb;

    private IEnumerator TryRegister()
    {
        yield return new WaitUntil(() => m_eb != null);

        m_eb.Subscribe<OnStartPressed>(HandleStartPressedEvent);
        m_eb.Subscribe<OnBackToMainPressed>(HandleBackToMainEvent);

        yield break;
    }

    private void UndoRegister()
    {
        if (m_eb != null)
        {
            m_eb.Unsubscribe<OnStartPressed>(HandleStartPressedEvent);
            m_eb.Unsubscribe<OnBackToMainPressed>(HandleBackToMainEvent);
        }
    }

    private void HandleBackToMainEvent(OnBackToMainPressed pressed)
    {
        SetConvasGroupActive(false);
    }

    private void HandleStartPressedEvent(OnStartPressed pressed)
    {
        SetConvasGroupActive(true);
    }

    private void OnEnable()
    {
        StartCoroutine(TryRegister());
    }

    private void OnDisable()
    {
        UndoRegister();
    }

    private void SetConvasGroupActive(bool value)
    {
        m_ConvasGroup.alpha = value ? 1 : 0;
        m_ConvasGroup.interactable = value;
        m_ConvasGroup.blocksRaycasts = value;
    }
    private void Start()
    {
        m_eb = FindFirstObjectByType<EventBus>();
        m_ConvasGroup = GetComponent<CanvasGroup>();
        SetConvasGroupActive(m_IsActiveAtBeginning);
    }
}
