using System;
using UnityEngine;
using UnityEngine.UI;

public enum IsSkipTutorial
{
    No,
    Yes
}


public class SkipTutorialToggleController : MonoBehaviour
{
    public Toggle m_Toggle;
    private const string PREF_KEY = "IsSkipTutorial";

    void Start()
    {
        // Bind Listener
        m_Toggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn));

        // Get the saved data from playerprefs
        int savedValue = PlayerPrefs.GetInt(PREF_KEY, (int)IsSkipTutorial.No);
        IsSkipTutorial savedData = (IsSkipTutorial)savedValue;

        SetInitialToggle(savedData);
    }

    private void SetInitialToggle(IsSkipTutorial savedData)
    {
        m_Toggle.onValueChanged.RemoveAllListeners();

        m_Toggle.isOn = (savedData == IsSkipTutorial.Yes) ? true : false;

        m_Toggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn));
    }

    private void OnToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(PREF_KEY,(int)(isOn ? IsSkipTutorial.Yes : IsSkipTutorial.No));
        PlayerPrefs.Save();

        Debug.Log($"Is skip tutorial? : {isOn}");
    }
}
