using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeftClickEventPublisher : MonoBehaviour
{
    [SerializeReference]
    public EventBase m_Event;
    private EventBus m_EB;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_EB != null)
        {
            //m_EB.Publish(m_Event);
        }
    }

    private void Start()
    {
        m_EB = FindFirstObjectByType<EventBus>();
    }
}
