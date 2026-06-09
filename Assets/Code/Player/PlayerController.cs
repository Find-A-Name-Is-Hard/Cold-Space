using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerModel))]
public class PlayerController : MonoBehaviour
{
    #region Player Controller
    private PlayerModel m_model;
    private bool m_isInputBound = false;

    // A public static reference to the single instance of the class
    public static PlayerController Instance { get; private set; }

    void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this one
            Destroy(this.gameObject);
        }
        else
        {
            // If not, set this as the instance
            Instance = this;
            // Optional: keep the object alive across scene loads
            // DontDestroyOnLoad(this.gameObject); 
        }
    }

    private void HandleMoveInput(Vector2 move)
    {
        m_model?.UpdatePlayerMovement(move);        
    }

    private IEnumerator TryBindInput()
    {
        float runTime = 0;

        while(true)
        {
            // Terminate coroutine if cannot bind after 5 sec
            if(runTime > 5)
            {
                UnityEngine.Debug.LogError($"{this.name} cannot bind input");
                yield break;
            }

            if (InputHandle.m_Instance != null && LevelManager.m_Instance != null &&!m_isInputBound)
            {
                LevelManager.m_Instance.OnTestAtkFinished += HandleTestAtkEndEvent;
                LevelManager.m_Instance.OnTestFinish += HandleTestAtkEndEvent;

                InputHandle.m_Instance.MovementEvent += HandleMoveInput;
                InputHandle.m_Instance.TestAtkButtonPerfomedEvent += HandleTestAtkInput;
                InputHandle.m_Instance.ShootingPerformedEvent += HandleShootingStartInput;
                InputHandle.m_Instance.ShootingCanceledEvent += HandleShootingStopInput;
                InputHandle.m_Instance.CureAtkButtonPerfomedEvent += HandleCureAtkInput;
                InputHandle.m_Instance.SlowDownPerformedEvent += HandleSlowDownEnterEvent;
                InputHandle.m_Instance.SlowDownCanceledEvent += HandleSlowDownEndEvent;

#if UNITY_EDITOR
                InputHandle.m_Instance.m_input.Debug.FillEnergy.performed += FillEN;
                InputHandle.m_Instance.m_input.Debug.FillHP.performed += FillHP;
#endif

                m_isInputBound = true;
                yield break;
            }

            yield return null;

            runTime += Time.deltaTime;
        }
    }

    private IEnumerator TryUnbindInput()
    {
        float runTime = 0;

        while (true)
        {
            // Terminate coroutine if cannot bind after 5 sec
            if (runTime > 5)
            {
                UnityEngine.Debug.LogError($"{this.name} cannot unbind input");
                yield break;
            }

            if (InputHandle.m_Instance != null && LevelManager.m_Instance != null && m_isInputBound)
            {
                LevelManager.m_Instance.OnTestAtkFinished -= HandleTestAtkEndEvent;

                InputHandle.m_Instance.MovementEvent -= HandleMoveInput;
                InputHandle.m_Instance.TestAtkButtonPerfomedEvent -= HandleTestAtkInput;
                InputHandle.m_Instance.ShootingPerformedEvent -= HandleShootingStartInput;
                InputHandle.m_Instance.ShootingCanceledEvent -= HandleShootingStopInput;
                InputHandle.m_Instance.CureAtkButtonPerfomedEvent -= HandleCureAtkInput;
#if UNITY_EDITOR
                InputHandle.m_Instance.m_input.Debug.FillEnergy.performed -= FillEN;
                InputHandle.m_Instance.m_input.Debug.FillHP.performed -= FillHP;
#endif
                m_isInputBound = false;
                yield break;
            }

            yield return null;

            runTime += Time.deltaTime;
        }
    }

#if UNITY_EDITOR
    private void FillEN(InputAction.CallbackContext context)
    {
        m_model.UpdatePlayerEnergy(m_model.m_currentAttributes.playerAttributes.MaxEnergy);
    }

    private void FillHP(InputAction.CallbackContext context)
    {
        m_model.UpdatePlayerHealth(m_model.m_currentAttributes.playerAttributes.MaxHP);
    }
#endif

    #endregion

    #region Controller API
    /// <summary>
    /// If you want to heal, the parameter should be positive.
    /// If you want to hurt, the parameter should be negative.
    /// </summary>
    /// <param name="hpChange"></param>
    public void HandleDamageEvent(int hpChange)
    {
        int currentHP = m_model.m_currentAttributes.playerAttributes.CurrentHP;
        int health = currentHP + hpChange;

        m_model.UpdatePlayerHealth(health);
    }

    public void HandlePositionInput(Vector3 position)
    {
        m_model.UpdatePlayerPosition(position);
    }

    /// <summary>
    /// If you want to reduce energy, the parameter should be negative.
    /// If you want to increase it, that should be positive
    /// </summary>
    /// <param name="energyChange"></param>
    public void HandleEnergyChangeInput(int energyChange)
    {
        int currentEN = m_model.m_currentAttributes.playerAttributes.CurrentEnergy;
        int targetEN = currentEN + energyChange;
        m_model.UpdatePlayerEnergy(targetEN);
    }

    public void HandleEnergyChangeInput()
    {
        int currentEN = m_model.m_currentAttributes.playerAttributes.CurrentEnergy;
        int defaultDodgeEN = m_model.m_currentAttributes.playerAttributes.DodgeEnergy;
        int targetEN = currentEN + defaultDodgeEN;
        m_model.UpdatePlayerEnergy(targetEN);
    }

    public void HandleTestAtkInput(int testAtk)
    {
        if (m_model.m_currentAttributes.playerAttributes.CurrentEnergy != m_model.m_currentAttributes.playerAttributes.MaxEnergy) return;

        m_model.UpdatePlayerEnergy(0);

        switch (testAtk)
        {
            case 2:
                m_model.currentWeapon.HandleWeaponChangeEvent();
                m_model.UpdateCurrentWeapon(m_model.FireWeapon.GetComponent<WeaponController>());
                LevelManager.m_Instance.NotifyChangeTestWeapon(testAtk);
                break;
            case 1:
                m_model.currentWeapon.HandleWeaponChangeEvent();
                m_model.UpdateCurrentWeapon(m_model.LightWeapon.GetComponent<WeaponController>());
                LevelManager.m_Instance.NotifyChangeTestWeapon(testAtk);
                break;
            case 3:
                m_model.currentWeapon.HandleWeaponChangeEvent();
                m_model.UpdateCurrentWeapon(m_model.MagneticWeapon.GetComponent<WeaponController>());
                LevelManager.m_Instance.NotifyChangeTestWeapon(testAtk);
                break;
            case 4:
                m_model.currentWeapon.HandleWeaponChangeEvent();
                m_model.UpdateCurrentWeapon(m_model.ProjectileWeapon.GetComponent<WeaponController>());
                LevelManager.m_Instance.NotifyChangeTestWeapon(testAtk);
                break;
            default:
                Debug.LogError($"{this.name} gets unknown weapon number {testAtk}");
                return;
        }
        
    }

    public void HandleShootingStartInput()
    {
        //Debug.Log("press z");
        m_model.currentWeapon.HandleShootingStartInput();
        m_model.UpdateFiringState(true);
    }

    public void HandleShootingStopInput()
    {
        //Debug.Log("cancel z");
        m_model.currentWeapon.HandleShootingEndInput();
        m_model.UpdateFiringState(false);
    }

    public void HandleCureAtkInput(int keyNum)
    {
        if (m_model.m_currentAttributes.playerAttributes.CurrentEnergy != m_model.m_currentAttributes.playerAttributes.MaxEnergy) return;

        m_model.UpdatePlayerEnergy(0);

        if (keyNum >= 1 && keyNum <= 6)
        {
            Instantiate(m_model.m_currentAttributes.CureAtkWeapon[keyNum - 1]);
        }
    }

    public void HandleChargeEvent(int chargeNumber)
    {
        m_model.UpdatePlayerEnergy(m_model.m_currentAttributes.playerAttributes.CurrentEnergy + chargeNumber);
    }

    public void HandleTestAtkEndEvent()
    {
        m_model.currentWeapon.HandleWeaponChangeEvent();
        m_model.UpdateCurrentWeapon(m_model.DefaultWeapon.GetComponent<WeaponController>());
    }

    public void HandleTestAtkEndEvent(bool isClueCorrect, HardClues enemyType)
    {
        m_model.currentWeapon.HandleWeaponChangeEvent();
        m_model.UpdateCurrentWeapon(m_model.DefaultWeapon.GetComponent<WeaponController>());
    }

    public void HandleSlowDownEnterEvent()
    {
        m_model.UpdateSlowDownState(true);
    }

    public void HandleSlowDownEndEvent()
    {
        m_model.UpdateSlowDownState(false);
    }

    #endregion

    #region MonoBehavior

    private void OnEnable()
    {
        StartCoroutine(TryBindInput());
    }

    private void OnDisable()
    {
        if (InputHandle.m_Instance != null && m_isInputBound)
        {
            LevelManager.m_Instance.OnTestFinish -= HandleTestAtkEndEvent;

            InputHandle.m_Instance.MovementEvent -= HandleMoveInput;
            InputHandle.m_Instance.TestAtkButtonPerfomedEvent -= HandleTestAtkInput;
            InputHandle.m_Instance.ShootingPerformedEvent -= HandleShootingStartInput;
            InputHandle.m_Instance.ShootingCanceledEvent -= HandleShootingStopInput;
            InputHandle.m_Instance.CureAtkButtonPerfomedEvent -= HandleCureAtkInput;
            InputHandle.m_Instance.SlowDownPerformedEvent -= HandleSlowDownEnterEvent;
            InputHandle.m_Instance.SlowDownCanceledEvent -= HandleSlowDownEndEvent;

#if UNITY_EDITOR
            InputHandle.m_Instance.m_input.Debug.FillEnergy.performed -= FillEN;
            InputHandle.m_Instance.m_input.Debug.FillHP.performed -= FillHP;
#endif

            m_isInputBound = false;
        }
    }

    private void Start()
    {
        m_model = GetComponent<PlayerModel>();
    }

    #endregion
}
