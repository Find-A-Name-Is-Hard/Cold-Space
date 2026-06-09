using Ami.BroAudio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MuteButton : MonoBehaviour,IPointerClickHandler
{
    private Image m_image;
    public BroAudioType audioType;
    private bool toggle;
    private void Start()
    {
        m_image = this.GetComponent<Image>();
        
        
        if (audioType == BroAudioType.SFX)
        {
            toggle = SoundEffectManager.Instance.muted;
        }
        else if (audioType == BroAudioType.Music)
        {
            toggle = MusicManager.Instance.muted;
        }

        if (toggle)
        {
            m_image.color = Color.white;
        }
        else
        {
            m_image.color = Color.clear;
        }

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
        Debug.Log("Mute Button Clicked");
        toggle = !toggle;
        BroAudio.SetVolume(audioType, toggle ? 0f : 1f);
        if (audioType == BroAudioType.SFX)
        {
            SoundEffectManager.Instance.toggleMute();
        }
        else if (audioType == BroAudioType.Music)
        {
            MusicManager.Instance.toggleMute();
        }
        

        if (toggle)
        {
            m_image.color = Color.white;
        }
        else
        {
            m_image.color = Color.clear;
        }

        //Color tempColor = Color.white;

        // Modify only the alpha component
        //tempColor.a = InputHandle.m_Instance.m_isSettingMenuOpened ? 1f : 0f;

        // Reassign the modified color back to the image
        //m_image.color = tempColor;


    }
}
