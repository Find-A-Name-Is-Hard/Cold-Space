using UnityEngine;
using UnityEngine.EventSystems;

public class StartController : MonoBehaviour, IPointerClickHandler
{
    private EventBus m_EB;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(m_EB != null)
        {
            m_EB.Publish<OnStartPressed>(new OnStartPressed());
        }
    }

    private void Start()
    {
        m_EB = FindFirstObjectByType<EventBus>();
    }
}
