using UnityEngine;
using Ami.BroAudio;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class MusicManager : MonoBehaviour
{
    
    public Ami.BroAudio.IAudioPlayer audioPlayer;
    public Dictionary<validActions, bool> registrationDictionary = new Dictionary<validActions, bool>();
    public float volumeDefault = 80f;
    public float fadeTimeDefault = .25f;
    public float highHealthPercentThreshold = .3f;
    public float midEnergyPercentThreshold = .4f;
    public float highEnergyPercentThreshold = .9f;
    


    [System.Serializable]
    public enum lowMedHigh
    {
        Low = 0,
        Med = 1,
        High = 2
    }

    [System.Serializable]
    public struct HealthEnergyStates
    {
        public lowMedHigh health;
        public lowMedHigh energy;
    }

    [System.Serializable]
    public struct HealthEnergyMusic
    {
        public HealthEnergyStates healthEnergy;
        public Ami.BroAudio.SoundID soundId;
    }

    [System.Serializable]
    public struct ClueMusic
    {
        public HardClues clueName;
        public Ami.BroAudio.SoundID soundId;
    }

    public HealthEnergyMusic[] healthEnergyMusicArray;
    public ClueMusic[] clueMusicArray;

    public HealthEnergyStates playerStatus;
    public HealthEnergyStates musicStatus;
    public Dictionary<HardClues, bool> clueStatus = new Dictionary<HardClues, bool>();
    public Dictionary<HardClues, bool> clueMusicStatus = new Dictionary<HardClues, bool>();

    public Dictionary<HealthEnergyStates, Ami.BroAudio.IAudioPlayer> audioHealthEnergyDict = new Dictionary<HealthEnergyStates, Ami.BroAudio.IAudioPlayer>();
    public Dictionary<HardClues, Ami.BroAudio.IAudioPlayer> audioClueDict = new Dictionary<HardClues, IAudioPlayer>();

    public bool muted = false;

    private void OnEnable()
    {
        //StartCoroutine(TryRegisterHealthChange());
        //StartCoroutine(TryRegisterEnergyChange());
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public void toggleMute()
    {
        muted = ! muted;
        BroAudio.SetVolume(BroAudioType.Music, muted ? 0f : 1f);
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        playerStatus.health = lowMedHigh.High;
        playerStatus.energy = lowMedHigh.Low;
        registrationDictionary = new Dictionary<validActions, bool>();
        clueStatus = new Dictionary<HardClues, bool>();
        StartCoroutine(TryRegisterHealthChange());
        StartCoroutine(TryRegisterEnergyChange());
        StartCoroutine(TryRegisterTestEnd());
    }

    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        


        
        playerStatus.health = lowMedHigh.High;
        playerStatus.energy = lowMedHigh.Low;
        musicStatus.health = playerStatus.health;
        musicStatus.energy = playerStatus.energy;
        foreach (HealthEnergyMusic hem in healthEnergyMusicArray)
        {
            Ami.BroAudio.IAudioPlayer tempPlayer = BroAudio.Play(hem.soundId);
            tempPlayer.SetVolume(volumeDefault * Convert.ToInt32(playerStatus.energy == hem.healthEnergy.energy && playerStatus.health == hem.healthEnergy.health));
            audioHealthEnergyDict.Add(hem.healthEnergy, tempPlayer);
        }

        foreach (ClueMusic cm in clueMusicArray)
        {
            Ami.BroAudio.IAudioPlayer tempPlayer = BroAudio.Play(cm.soundId);
            tempPlayer.SetVolume(0f);
            audioClueDict.Add(cm.clueName, tempPlayer);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerStatus.energy!=musicStatus.energy||playerStatus.health!=musicStatus.health)
        {
            audioHealthEnergyDict[musicStatus].SetVolume(0f, fadeTimeDefault);
            audioHealthEnergyDict[playerStatus].SetVolume(volumeDefault, fadeTimeDefault);

            musicStatus.health = playerStatus.health;
            musicStatus.energy = playerStatus.energy;
            Debug.Log("Music Score Changed");
        }

        //Keys in clueStatus but not in clueMusicStatus
        var keysInDict1Only = clueStatus.Keys.Except(clueMusicStatus.Keys);

        
        foreach (var key in keysInDict1Only)
        {
            Debug.Log("Playing new clue music");
            clueMusicStatus.Add(key, true);
            audioClueDict[key].SetVolume(volumeDefault, fadeTimeDefault);
        }

        //Keys in clueMusicStatus not in clue status
        var keysInDict2Only = clueMusicStatus.Keys.Except(clueStatus.Keys);

        //Collection was modified enumeration operation may not execute.
        foreach (var key in keysInDict2Only)
        {
            Debug.Log("Muting clue music");
            clueMusicStatus.Remove(key);
            audioClueDict[key].SetVolume(0f, fadeTimeDefault);
        }
    }



    private IEnumerator TryRegisterHealthChange()
    {
        while (registrationDictionary.ContainsKey(validActions.OnPlayerHealthUpdate) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerHealthUpdate += PlayerHealthUpdate;
                registrationDictionary[validActions.OnPlayerHealthUpdate] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void PlayerHealthUpdate(int newHealthValue, int max)
    {
        if (newHealthValue > max*highHealthPercentThreshold)
        {
            playerStatus.health = lowMedHigh.High;
        }
        else
        {
            playerStatus.health = lowMedHigh.Low;
        }

        if (newHealthValue <= 0)
        {
            
        }
    }


    private IEnumerator TryRegisterEnergyChange()
    {
        while (registrationDictionary.ContainsKey(validActions.OnPlayerEnergyUpdate) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerEnergyUpdate += PlayerEnergyUpdate;
                registrationDictionary[validActions.OnPlayerEnergyUpdate] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void PlayerEnergyUpdate(int newEnergyValue, int max)
    {
        //Check if @ max energy

        if (newEnergyValue >= max*highEnergyPercentThreshold)//max) //just hardcoded to test
        {
            playerStatus.energy = lowMedHigh.High;
            //Debug.Log("Max Energy in music manager");
        }
        else if (newEnergyValue >= max*midEnergyPercentThreshold)
        {
            playerStatus.energy = lowMedHigh.Med;
        }
        else
        {
            playerStatus.energy = lowMedHigh.Low;
        }
    }

    private IEnumerator TryRegisterTestEnd()
    {
        while (registrationDictionary.ContainsKey(validActions.OnTestFinish) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestFinish += TestFinishEffect;
                registrationDictionary[validActions.OnTestFinish] = true;
            }
            yield return null;
        }
        yield break;
    }


    private void TestFinishEffect(bool isTestCorrect, HardClues clueName)
    {
        if (isTestCorrect)
        {
            clueStatus.Add(clueName, true);
        }
        else
        {
            
        }

        Debug.Log("Test Finished In Music Manager");
    }





}
