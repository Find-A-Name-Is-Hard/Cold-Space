using UnityEngine;
using UnityEngine.UI;

public enum CopilotDifficulty
{
    Easy, 
    Medium,
    Hard
}

public class CopilotToggleController : MonoBehaviour
{
    [Header("Toggles for each difficulty")]
    public Toggle m_EasyToggle;
    //public Toggle m_MediumToggle;
    public Toggle m_HardToggle;

    private const string PREF_KEY = "CopilotDifficulty";

    void Start()
    {
        // Bind Listener
        m_EasyToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, CopilotDifficulty.Easy));
        //m_MediumToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, CopilotDifficulty.Medium));
        m_HardToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, CopilotDifficulty.Hard));

        // Get the saved data from playerprefs
        int savedValue = PlayerPrefs.GetInt(PREF_KEY, (int)CopilotDifficulty.Easy);
        CopilotDifficulty savedDifficulty = (CopilotDifficulty)savedValue;

        SetInitialToggle(savedDifficulty);
    }

    private void OnToggleChanged(bool isOn, CopilotDifficulty difficulty)
    {
        if (!isOn) return;

        PlayerPrefs.SetInt(PREF_KEY, (int)difficulty);
        PlayerPrefs.Save();

        Debug.Log($"Now the copilot difficulty is: {difficulty}");
    }

    private void SetInitialToggle(CopilotDifficulty difficulty)
    {
        // Remove listener
        m_EasyToggle.onValueChanged.RemoveAllListeners();
        //m_MediumToggle.onValueChanged.RemoveAllListeners();
        m_HardToggle.onValueChanged.RemoveAllListeners();

        m_EasyToggle.isOn = (difficulty == CopilotDifficulty.Easy);
        //m_MediumToggle.isOn = (difficulty == CopilotDifficulty.Medium);
        m_HardToggle.isOn = (difficulty == CopilotDifficulty.Hard);


        m_EasyToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, CopilotDifficulty.Easy));
        //m_MediumToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, CopilotDifficulty.Medium));
        m_HardToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, CopilotDifficulty.Hard));
    }
}
