using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class DifficultyToggleController : MonoBehaviour
{
    [Header("Toggles for each difficulty")]
    public Toggle m_EasyToggle;
    public Toggle m_MediumToggle;
    public Toggle m_HardToggle;

    private const string PREF_KEY = "GameDifficulty";

    void Start()
    {
        // Bind Listener
        m_EasyToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, Difficulty.Easy));
        m_MediumToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, Difficulty.Medium));
        m_HardToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, Difficulty.Hard));

        // Get the saved data from playerprefs
        int savedValue = PlayerPrefs.GetInt(PREF_KEY, (int)Difficulty.Medium);
        Difficulty savedDifficulty = (Difficulty)savedValue;

        SetInitialToggle(savedDifficulty);
    }

    private void OnToggleChanged(bool isOn, Difficulty difficulty)
    {
        if (!isOn) return;

        PlayerPrefs.SetInt(PREF_KEY, (int)difficulty);
        PlayerPrefs.Save();

        Debug.Log($"Now the difficulty is: {difficulty}");
    }

    private void SetInitialToggle(Difficulty difficulty)
    {
        // Remove listener
        m_EasyToggle.onValueChanged.RemoveAllListeners();
        m_MediumToggle.onValueChanged.RemoveAllListeners();
        m_HardToggle.onValueChanged.RemoveAllListeners();

        m_EasyToggle.isOn = (difficulty == Difficulty.Easy);
        m_MediumToggle.isOn = (difficulty == Difficulty.Medium);
        m_HardToggle.isOn = (difficulty == Difficulty.Hard);


        m_EasyToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, Difficulty.Easy));
        m_MediumToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, Difficulty.Medium));
        m_HardToggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, Difficulty.Hard));
    }
}
