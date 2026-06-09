using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MannualPageController : MonoBehaviour
{
    public int m_PageNumber = 0;

    private EventBus m_eb;
    private CanvasGroup m_canvasGroup;

    private IEnumerator TryRegister()
    {
        yield return new WaitUntil(() => m_eb != null);

        m_eb.Subscribe<OnMannualPressed>(HandleMannualPressedEvent);

        yield break;
    }

    private void UndoRegister()
    {
        if (m_eb != null)
        {
            m_eb.Unsubscribe<OnMannualPressed>(HandleMannualPressedEvent);
        }
    }

    private void HandleMannualPressedEvent(OnMannualPressed eventData)
    {
        if (eventData.pageNumber == m_PageNumber) SetConvasGroupActive(true);
        else SetConvasGroupActive(false);
    }

    private void SetConvasGroupActive(bool value)
    {
        m_canvasGroup.alpha = value ? 1 : 0;
        m_canvasGroup.interactable = value;
        m_canvasGroup.blocksRaycasts = value;
    }


    #region MonoBehavior
    private void OnEnable()
    {
        StartCoroutine(TryRegister());
    }

    private void OnDisable()
    {
        UndoRegister();
    }

    private void Start()
    {
        m_eb = FindFirstObjectByType<EventBus>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }
    #endregion
}
