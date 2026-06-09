using UnityEngine;
using UnityEngine.EventSystems;

public class BackToMainController : MonoBehaviour, IPointerClickHandler
{
    private EventBus m_EB;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_EB != null)
        {
            m_EB.Publish<OnBackToMainPressed>(new OnBackToMainPressed());
        }
    }

    private void Start()
    {
        m_EB = FindFirstObjectByType<EventBus>();
    }
}
