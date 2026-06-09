using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Image))]
public class ScreenSizeButtonController : MonoBehaviour, IPointerClickHandler
{
    /*
    [SerializeField] private Sprite m_enlargeImg;
    [SerializeField] private Sprite m_shrinkImg;
    */
    private Image m_image;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool IsDocumentInFullScreen();
#endif

    private void Start()
    {
        m_image = GetComponent<Image>();
    }

    void Update()
    {
        UpdateButtonIcon();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SwitchZoomToggle();
    }

    public void SwitchZoomToggle()
    {
        SetWindowSize();
    }

    public void UpdateButtonIcon()
    {
        if (m_image == null)
            m_image = GetComponent<Image>();

        bool isFullScreen = IsFullScreen();

        //m_image.enabled =  isFullScreen;

        
        if (isFullScreen)
        {
            m_image.color = Color.clear;
        }
        else
        {
            m_image.color = Color.white;
        }
    }

    public void SetWindowSize()
    {
        SetFullScreen(!IsFullScreen());
    }

    private bool IsFullScreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL 环境：使用 JavaScript 检测浏览器全屏状态
        return IsDocumentInFullScreen();
#else
        // PC/Editor 环境：使用 Unity Screen API
        return Screen.fullScreen;
#endif
    }

    private void SetFullScreen(bool fullscreen)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL 环境：使用 JavaScript 调用浏览器全屏 API
        if (fullscreen)
        {
            Application.ExternalEval(
                @"
                var canvas = document.querySelector('#unity-canvas');
                if (canvas.requestFullscreen) {
                    canvas.requestFullscreen();
                } else if (canvas.webkitRequestFullscreen) {
                    canvas.webkitRequestFullscreen();
                } else if (canvas.mozRequestFullScreen) {
                    canvas.mozRequestFullScreen();
                } else if (canvas.msRequestFullscreen) {
                    canvas.msRequestFullscreen();
                }
                "
            );
        }
        else
        {
            Application.ExternalEval(
                @"
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                } else if (document.webkitExitFullscreen) {
                    document.webkitExitFullscreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.msExitFullscreen) {
                    document.msExitFullscreen();
                }
                "
            );
        }
#else
        // PC 环境：使用 Unity 的 Screen API
        if (fullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1920, 1080, false);
        }
#endif
    }
}
