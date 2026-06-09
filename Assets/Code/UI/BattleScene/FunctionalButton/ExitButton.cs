using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ExitButton : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private ESCMenuController m_escMenuController;
    private Image m_image;


    private void Start()
    {
        m_image = this.GetComponent<Image>();
        
        m_image.color = Color.clear;
        /*
        m_image.enabled = false;
        Color tempColor = m_image.color;
        
        // Modify only the alpha component
        tempColor.a = 0f;

        // Reassign the modified color back to the image
        m_image.color = tempColor;*/
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        InputHandle.m_Instance.OnSettingMenuPerformed();
        Debug.Log("exit Button Clicked");
        //Color tempColor = Color.white;

        // Modify only the alpha component
        //tempColor.a = InputHandle.m_Instance.m_isSettingMenuOpened ? 1f : 0f;

        // Reassign the modified color back to the image
        //m_image.color = tempColor;


    }

    //This is hacky but whatever
    public void Update()
    {
        if (Time.timeScale == 0)
        {
            m_image.color = Color.white;
        }
        else
        {
            m_image.color = Color.clear;
        }
    }
}
