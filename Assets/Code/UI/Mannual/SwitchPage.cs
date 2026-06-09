using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchPage : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// If value is true, clicking this buttong will go to next page
    /// , or clicking will go to previous page
    /// </summary>
    public bool m_IsGoingToNextPage = true;

    private int m_nextTargetPage = 0;
    public int m_MaxPage = 0;

    private EventBus m_eb;

    // Register events in event bus
    private IEnumerator TryRegister()
    {
        yield return new WaitUntil(() => m_eb != null);

        m_eb.Subscribe<OnMannualPressed>(HandleMannualPressedEvent);

        yield break;
    }

    private void UndoRegister()
    {
        if(m_eb != null)
        {
            m_eb.Unsubscribe<OnMannualPressed>(HandleMannualPressedEvent);
        }
    }

    // Handle Mannual pressed event from main menu and self
    private void HandleMannualPressedEvent(OnMannualPressed eventData)
    {
        int pageNumber = eventData.pageNumber;
        m_nextTargetPage = m_IsGoingToNextPage ? pageNumber + 1 : pageNumber - 1;
        if (m_nextTargetPage <= 0) m_nextTargetPage = m_MaxPage;
        if (m_nextTargetPage > m_MaxPage) m_nextTargetPage = 1;
        Debug.Log(m_nextTargetPage);
    }

    // Handle user click input
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnMannualPressed eventData = new OnMannualPressed();
        eventData.pageNumber = m_nextTargetPage;

        m_eb.Publish<OnMannualPressed>(eventData);
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
    }
    #endregion
}
