using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class GlobalVolume : MonoBehaviour
{
    private Slider m_volumeSlider;
    private const string PREF_KEY = "GlobalVolume";

    public Slider.SliderEvent HandleSliderValueChaning { get; private set; }

    private void Start()
    {
        m_volumeSlider = GetComponent<Slider>();
        m_volumeSlider.value = PlayerPrefs.GetFloat(PREF_KEY, 1);

        m_volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    //private void OnEnable()
    //{
    //    StartCoroutine(RegisterValueChange());
    //}

    //private void OnDisable()
    //{
    //    m_volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    //}

    //private IEnumerator RegisterValueChange()
    //{
    //    yield return new WaitUntil(() => {return (m_volumeSlider != null); });
    //}

    public void OnSliderValueChanged(float value)
    {
        PlayerPrefs.SetFloat(PREF_KEY, value);
        Debug.Log(PlayerPrefs.GetFloat(PREF_KEY, 1));
    }



}
