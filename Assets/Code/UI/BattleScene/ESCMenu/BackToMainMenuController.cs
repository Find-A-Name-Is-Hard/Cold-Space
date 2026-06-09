using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BackToMainMenuController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string m_NextLevelName;

    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(m_NextLevelName);
    }
}
