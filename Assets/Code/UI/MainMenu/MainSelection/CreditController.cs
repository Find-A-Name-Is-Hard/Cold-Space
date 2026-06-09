using UnityEngine;
using UnityEngine.EventSystems;

public class CreditController : MonoBehaviour, IPointerClickHandler
{
    private EventBus m_EB;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_EB != null)
        {
            m_EB.Publish<OnCreditPressed>(new OnCreditPressed());
        }
    }

    private void Start()
    {
        m_EB = FindFirstObjectByType<EventBus>();
    }
}
