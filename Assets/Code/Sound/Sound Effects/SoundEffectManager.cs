using UnityEngine;
using Ami.BroAudio;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class SoundEffectManager : MonoBehaviour
{
    public SoundID playerHitSound;
    public SoundID enemyHitNormalSound;
    public SoundID enemyhitExplosiveSound;
    public SoundID enemyHitMagneticSound;
    public SoundID EnemyHitFireSound;
    public SoundID EnemyHitLightSound;
    public float enemyHitLightTime = -1f;
    public float enemyHitFireTime = -1f;
    public Ami.BroAudio.IAudioPlayer enemyHitFireSource;
    public Ami.BroAudio.IAudioPlayer enemyHitLightSource;
    public bool lightPlaying = false;
    public bool firePlaying = false;
    public float attackFadeTime = .2f;

    public float attackFireTime = -1f;
    public PlayerHitEvent recentHit = PlayerHitEvent.NormalAtk;
    public bool currentlyFiring = false;
    public Ami.BroAudio.IAudioPlayer playerFireSource;
    public SoundID playerFireSound;

    public Dictionary<validActions, bool> registrationDictionary = new Dictionary<validActions, bool>();

    public Ami.BroAudio.IAudioPlayer playerFireSourceFireAttack;
    public SoundID playerFireSoundFireAttack;
    public Ami.BroAudio.IAudioPlayer playerFireSourceLightAttack;
    public SoundID playerFireSoundLightAttack;

    public SoundID bossEntry;
    public SoundID bossDeath;
    public SoundID fullEnergy;
    private int avoidDoubleEnergyHits = 0;
    public SoundID positiveResult;

    public string mainMenu;

    public bool muted = false;
    public static SoundEffectManager Instance { get; private set; }

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
        mainMenu = SceneManager.GetActiveScene().name;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHitFireSource = BroAudio.Play(EnemyHitFireSound);
        enemyHitFireSource.SetVolume(0);
        enemyHitLightSource = BroAudio.Play(EnemyHitLightSound);
        enemyHitLightSource.SetVolume(0);
        playerFireSource = BroAudio.Play(playerFireSound);
        playerFireSource.SetVolume(0);

        playerFireSourceFireAttack = BroAudio.Play(playerFireSoundFireAttack);
        playerFireSourceFireAttack.SetVolume(0);
        playerFireSourceLightAttack = BroAudio.Play(playerFireSoundLightAttack);
        playerFireSourceLightAttack.SetVolume(0);

    }

    // Update is called once per frame
    void Update()
    {
        enemyHitFireTime -= Time.deltaTime;
        enemyHitLightTime -= Time.deltaTime;
        attackFireTime -= Time.deltaTime;

        
        if (Time.timeScale==0)
        {
            Debug.Log("game paused in sfx manager");
            playerFireSource.SetVolume(0f);
        }
        else if (currentlyFiring)
        {
            playerFireSource.SetVolume(80f);
        }

        if (enemyHitFireTime>=0 && firePlaying==false) 
        {
            firePlaying = true;
            enemyHitFireSource.SetVolume(80f, attackFadeTime);
        }
        else if (enemyHitFireTime<=0 && firePlaying)
        {
            firePlaying = false;
            enemyHitFireSource.SetVolume(0, attackFadeTime);
        }

        if (enemyHitLightTime >= 0 && lightPlaying == false)
        {
            lightPlaying = true;
            enemyHitLightSource.SetVolume(80f, attackFadeTime);
        }
        else if (enemyHitLightTime <= 0 && lightPlaying)
        {
            lightPlaying = false;
            enemyHitLightSource.SetVolume(0, attackFadeTime);
        }

        if (attackFireTime >= 0 && currentlyFiring == false)
        {
            currentlyFiring = true;
            playerFireSource.SetVolume(80f);
            
        }
        else if (attackFireTime <= 0 && currentlyFiring)
        {
            currentlyFiring = false;
            playerFireSource.SetVolume(0);
        }
    }

    private void OnEnable()
    {
        //StartCoroutine(TryRegisterHealthChange());
        //StartCoroutine(TryRegisterEnergyChange());
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public void toggleMute()
    {
        muted = ! muted;
        BroAudio.SetVolume(BroAudioType.SFX, muted ? 0f : 1f);
    }


    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        registrationDictionary = new Dictionary<validActions, bool>();
        StartCoroutine(TryRegisterHealthChange());
        StartCoroutine(TryRegisterEnemyHit());
        enemyHitFireSource.SetVolume(0);
        enemyHitLightSource.SetVolume(0);
        enemyHitLightTime = -1f;
        enemyHitFireTime = -1f;
        lightPlaying = false;
        firePlaying = false;

        playerFireSource.SetVolume(0);
        attackFireTime = -1f;
        lightPlaying = false;

        playerFireSourceFireAttack.SetVolume(0);
        playerFireSourceLightAttack.SetVolume(0);


        StartCoroutine(TryRegisterCureEnd());
        StartCoroutine(TryRegisterEnergyChange());
        StartCoroutine(TryRegisterTestEnd());
        if (mainMenu != arg1.name)
        {
            PlayerSpawnEffect();
        }
        
    }

    private IEnumerator TryRegisterTestEnd()
    {
        while (registrationDictionary.ContainsKey(validActions.OnTestFinish)==false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
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
            BroAudio.Play(positiveResult);
        }
        Debug.Log("Test Finished In SFX Manager");
    }


    private IEnumerator TryRegisterCureEnd()
    {
        while (registrationDictionary.ContainsKey(validActions.OnCureHappen) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnCureHappen += CureFinishEffect;
                registrationDictionary[validActions.OnCureHappen] = true;
            }
            yield return null;
        }
        yield break;
    }


    private void CureFinishEffect(bool isCureCorrect, HardClues[] clues)
    {
        if (isCureCorrect)
        {
            BroAudio.Play(bossDeath);
            
        }

        Debug.Log("Cure Finished In sfx Manager");
    }

    private void PlayerSpawnEffect()
    {
        BroAudio.Play(bossEntry);
        //Debug.Log("Game Start in sfx Manager");
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
        
        if (newEnergyValue >= max && avoidDoubleEnergyHits< max)
        {
            BroAudio.Play(fullEnergy);
            Debug.Log("Max Energy in SFX manager");
        }
        avoidDoubleEnergyHits = newEnergyValue;
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

        if (newHealthValue <= 0)
        {
            //play player death sound
        }
        else
        {
            //play player hit sound
            BroAudio.Play(playerHitSound);
        }
    }

    private IEnumerator TryRegisterEnemyHit()
    {
        while (registrationDictionary.ContainsKey(validActions.OnPlayerHitBoss) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerHitBoss += EnemyHit;
                registrationDictionary[validActions.OnPlayerHitBoss] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void EnemyHit(PlayerHitEvent eventData)
    {

        switch (eventData)
        {
            case PlayerHitEvent.CureAtk:
                
                // Code to execute if expression matches value1
                break;
            case PlayerHitEvent.ExplosiveTest:
                BroAudio.Play(enemyhitExplosiveSound);
                break;
            case PlayerHitEvent.FlammableTest:
                //hardcoded to three because this doesn't constantly trigger hmm
                enemyHitFireTime = 1f;
                break;
            case PlayerHitEvent.LightningTest:
                enemyHitLightTime = 1f;
                break;
            case PlayerHitEvent.MagneticTest:
                BroAudio.Play(enemyHitMagneticSound);
                break;
            case PlayerHitEvent.NormalAtk:
                BroAudio.Play(enemyHitNormalSound);
                break;
        }
        Debug.Log("enemyHitInSfxManager");
    }

    public void PlayerShoot(PlayerHitEvent eventData)
    {
        recentHit = eventData;
        attackFireTime = .25f;
        /*
        switch (eventData)
        {
            case PlayerHitEvent.CureAtk:

                // Code to execute if expression matches value1
                break;
            case PlayerHitEvent.ExplosiveTest:
                //BroAudio.Play(enemyhitExplosiveSound);
                break;
            case PlayerHitEvent.FlammableTest:
                //hardcoded to three because this doesn't constantly trigger hmm
                attackFireTime = .25f;
                break;
            case PlayerHitEvent.LightningTest:
                attackFireTime = .25f;
                break;
            case PlayerHitEvent.MagneticTest:
                attackFireTime = .25f;
                break;
            case PlayerHitEvent.NormalAtk:
                attackFireTime = .25f;
                
                break;
        }*/
    }

    public void PlayerContinuousAttack(PlayerHitEvent eventData, bool Active)
    {
        Debug.Log("continuous attack start");
        if (eventData == PlayerHitEvent.LightningTest)
        {
            if (Active)
            {
                
                playerFireSourceLightAttack.SetVolume(80f);
            }
            else
            {
                playerFireSourceLightAttack.SetVolume(0);
            }
        }
        else if (eventData == PlayerHitEvent.FlammableTest)
        {
            if (Active)
            {
                playerFireSourceFireAttack.SetVolume(80f);
            }
            else
            {
                playerFireSourceFireAttack.SetVolume(0);
            }
        }
        
    }

}
