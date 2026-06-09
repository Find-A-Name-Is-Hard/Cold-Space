using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EngageButtonController : MonoBehaviour, IPointerClickHandler
{
    public string NextLevelName;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(NextLevelName);
        //Debug.Log(NextLevelName);
    }
}
