using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerHitEvent
{
    NormalAtk, 
    FlammableTest, 
    LightningTest,
    MagneticTest,
    ExplosiveTest,
    CureAtk
}
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Instance Mode
    /// </summary>
    public static LevelManager m_Instance;

    /// <summary>
    /// The timer will start in "Start" of "Level Manager"
    /// </summary>
    public float LevelTimer { get; private set; }

    #region Level Boundary
    /// <summary>
    /// Level boundary configs
    /// </summary>
    [SerializeField] private BoxCollider2D LevelBoundary;
    public float UpperLevelBoundary { get; private set; }
    public float LowerLevelBoundary { get; private set; }
    public float LeftLevelBoundary { get; private set; }
    public float RightLevelBoundary { get; private set; }

    public GameObject m_PlayerPrefab;
    public Transform m_PlayerSpawnPosition;


    private void CalculateLevelBoundary()
    {
        if (LevelBoundary == null)
        {
            UnityEngine.Debug.LogError($"Cannot find level boundary collider in {this.name}");
        }

        Vector2 center = new Vector2(LevelBoundary.transform.position.x, LevelBoundary.transform.position.y);
        float height = LevelBoundary.size.y;
        float width = LevelBoundary.size.x;

        UpperLevelBoundary = center.y + (height / 2f);
        LowerLevelBoundary = center.y - (height / 2f);
        RightLevelBoundary = center.x + (width / 2f);
        LeftLevelBoundary = center.x - (width / 2f);
    }

    private void SpawnPlayer()
    {
        if (m_PlayerPrefab == null && m_PlayerSpawnPosition == null)
            UnityEngine.Debug.LogError("Trying to spawn player without prefab or position transform");

        CurrentPlayer = Instantiate(m_PlayerPrefab);

        OnPlayerSpawn?.Invoke();
    }

    private IEnumerator TryRegisterPlayerEvents()
    {
        PlayerModel pm;

        yield return new WaitUntil(() => CurrentPlayer != false);

        while (!CurrentPlayer.TryGetComponent<PlayerModel>(out pm))
        {
            yield return null;
        }

        pm.OnMovementUpdate += NotifyPlayerMovementUpdate;
        pm.OnPositionUpdate += NotifyPlayerPositionUpdate;
        pm.OnHealthUpdate += NotifyPlayerHealthUpdate;
        pm.OnEnergyUpdate += NotifyPlayerEnergyUpdate;
        pm.OnWeaponChange += NotifyPlayerWeaponChange;
        pm.OnChangeTestWeapon += NotifyPlayerTestWeaponChange;
        pm.OnSlowDownStateChange += NotifyPlayerSlowDownSateChange;
        pm.OnFireStateUpdate += NotifyPlayerFireStateUpdate;

        yield break;
    }

    private void NotifyPlayerFireStateUpdate(bool isFiring)
    {
        OnPlayerFireStateUpdate?.Invoke(isFiring);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerSlowDownSateChange(bool isSlowDown)
    {
        OnPlayerSlowDownStateChange?.Invoke(isSlowDown);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerTestWeaponChange()
    {
        OnPlayerTestWeaponChange?.Invoke();
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerWeaponChange(WeaponController controller, HardClues weaponType)
    {
        OnPlayerWeaponChange?.Invoke(controller, weaponType);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerEnergyUpdate(int currentEN, int oldValue,int MaxEN)
    {
        OnPlayerEnergyUpdate?.Invoke(currentEN, MaxEN);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerHealthUpdate(int CurrentHP, int oldValue, int MaxHP)
    {
        OnPlayerHealthUpdate?.Invoke(CurrentHP, MaxHP);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerPositionUpdate(Vector3 vector)
    {
        OnPlayerPositionUpdate?.Invoke(vector);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }

    private void NotifyPlayerMovementUpdate(Vector2 vector)
    {
        OnPlayerMovementUpdate?.Invoke(vector);
//#if UNITY_EDITOR
//        var stackTrace = new StackTrace();
//        var current = stackTrace.GetFrame(0).GetMethod();
//        UnityEngine.Debug.Log(current.Name);
//#endif
    }
    #endregion

    #region LevelManager API
    public GameObject CurrentPlayer { get; private set; }
    public GameObject CurretEnemy;

    // Level relevant event
    /// <summary>
    /// Int is the 1~4 key number that player pressed
    /// </summary>
    public Action<int> OnTestAtkStart = delegate { };
    [Obsolete("This is obsolete, use 'OnTestFinish' ")]
    public Action OnTestAtkFinished = delegate { };
    /// <summary>
    /// When boolean is true, test succeeds. 
    /// HardClues is the test atk type
    /// </summary>
    public Action<bool, HardClues> OnTestFinish = delegate { };
    /// <summary>
    /// When boolean is true, cure succeeds and boss is killed
    /// </summary>
    public Action<bool, HardClues[]> OnCureHappen = delegate { };

    // Player relevant event
    public Action OnPlayerSpawn = delegate { };
    /// <summary>
    /// Invoke when player press any key of up/down/left/right. 
    /// Vector2 is moving direction
    /// </summary>
    public Action<Vector2> OnPlayerMovementUpdate = delegate { };
    /// <summary>
    /// Invoke when player position is changing. 
    /// Vector3 is the new position
    /// </summary>
    public Action<Vector3> OnPlayerPositionUpdate = delegate { };
    /// <summary>
    /// Invoke when player health is changing. 
    /// First int is the new health value, second int is the max energy value
    /// </summary>
    public Action<int, int> OnPlayerHealthUpdate = delegate { };
    /// <summary>
    /// Invoke when player energy is changing 
    /// First int is the new energy value, second int is the max energy value
    /// </summary>
    public Action<int, int> OnPlayerEnergyUpdate = delegate { };
    /// <summary>
    /// Invoke when player changes test weapon or normal attack weapon. 
    /// HardClues is weapon type
    /// Now it is invoked when player enter and finish test attack
    /// </summary>
    public Action<WeaponController, HardClues> OnPlayerWeaponChange = delegate { };
    /// <summary>
    /// Invoke when player enter test atk
    /// </summary>
    public Action OnPlayerTestWeaponChange = delegate { };
    /// <summary>
    /// Invoke when player press or cancel shift.
    /// True is moving in slow mode
    /// </summary>
    public Action<bool> OnPlayerSlowDownStateChange = delegate { };
    /// <summary>
    /// Invoke when player press or cancel fire button, whatever in normal or test atk mode
    /// Pressing button is true, cancel is false
    /// </summary>
    public Action<bool> OnPlayerFireStateUpdate = delegate { };

    public Action<PlayerHitEvent> OnPlayerHitBoss = delegate { };

    // Enemy part
    public Action<AttackPatternData> OnEnemyAtkPatternUpdate = delegate { };

    public void NotifyPlyaerHitHappen(PlayerHitEvent eventData)
    {
        //UnityEngine.Debug.Log(eventData.ToString());
        OnPlayerHitBoss?.Invoke(eventData);
    }

    public void NotifyChangeTestWeapon(int weaponNum)
    {
        OnTestAtkStart?.Invoke(weaponNum);
    }

    [Obsolete("This function is deprecated, please use the Notify test atk end (EEnemyType)")]
    public void NotifyTestAtkEnd()
    {
        OnTestAtkFinished?.Invoke();
    }

    public void NotifyTestAtkEnd(bool isClueCorrect, HardClues clue)
    {
        OnTestFinish?.Invoke(isClueCorrect, clue);
    }


    #endregion


    public void retMain(bool killPlayer)
    {
        StartCoroutine(ReturnMainMenu(killPlayer));
    }

    public IEnumerator ReturnMainMenu(bool killPlayer)
    {
        if (killPlayer)
        {
            CurrentPlayer.SetActive(false);
        }
        UnityEngine.Debug.Log("returnMainMenuCalled");
        yield return new WaitForSeconds(5f);
        UnityEngine.Debug.Log("WaitOver");
        //SceneManager.LoadSceneAsync(0);
        SceneManager.LoadScene(0);
        yield return null;
    }

    private void Awake()
    {
        m_Instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(TryRegisterPlayerEvents());
    }

    private void OnDisable()
    {
        PlayerModel pm;
        
        if(CurrentPlayer != null && CurrentPlayer.TryGetComponent<PlayerModel>(out pm))
        {
            pm.OnMovementUpdate -= NotifyPlayerMovementUpdate;
            pm.OnPositionUpdate -= NotifyPlayerPositionUpdate;
            pm.OnHealthUpdate -= NotifyPlayerHealthUpdate;
            pm.OnEnergyUpdate -= NotifyPlayerEnergyUpdate;
            pm.OnWeaponChange -= NotifyPlayerWeaponChange;
            pm.OnChangeTestWeapon -= NotifyPlayerTestWeaponChange;
            pm.OnSlowDownStateChange -= NotifyPlayerSlowDownSateChange;
            pm.OnFireStateUpdate -= NotifyPlayerFireStateUpdate;
        }
        
    }

    private void Start()
    {
        CalculateLevelBoundary();
        SpawnPlayer();
    }

    private void Update()
    {
        LevelTimer += Time.deltaTime;
    }
}
