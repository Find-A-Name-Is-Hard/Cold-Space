using UnityEngine;
using UnityEngine.EventSystems;

public class MannualController : MonoBehaviour, IPointerClickHandler
{
    private EventBus m_EB;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (m_EB != null)
        {
            OnMannualPressed eventData = new OnMannualPressed();
            eventData.pageNumber = 1;

            m_EB.Publish<OnMannualPressed>(eventData);
        }
    }

    private void Start()
    {
        m_EB = FindFirstObjectByType<EventBus>();
    }
}
