using System;
using System.Collections;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    #region Player Model
    public PlayerAttributes m_ExceptedAttributes;
    public PlayerAttributes m_currentAttributes { get; private set; }

    public WeaponController currentWeapon { get; private set; }
    public GameObject DefaultWeapon { get; private set; }
    public GameObject FireWeapon { get; private set; }
    public GameObject MagneticWeapon { get; private set; }
    public GameObject LightWeapon { get; private set; }
    public GameObject ProjectileWeapon { get; private set; }

    private Vector2 m_movement;
    public Vector2 Movement
    {
        get => m_movement;
        private set
        {
            m_movement = value;
            OnMovementUpdate?.Invoke(m_movement);
        }
    }

    private Vector3 m_position;
    public Vector3 Position
    {
        get => m_position;
        private set
        {
            m_position = value;
            OnPositionUpdate?.Invoke(m_position);
        }
    }

    private bool m_isSlowDown = false;
    private bool m_isFiring = false;

    public Action<Vector2> OnMovementUpdate = delegate { };
    public Action<Vector3> OnPositionUpdate = delegate { };
    public Action<int, int, int> OnHealthUpdate = delegate { };
    public Action<int, int, int> OnEnergyUpdate = delegate { };
    public Action<WeaponController, HardClues> OnWeaponChange = delegate { };
    public Action OnChangeTestWeapon = delegate { };
    public Action<bool> OnFireStateUpdate = delegate { };
    public Action<bool> OnSlowDownStateChange = delegate { };
    public Action OnPlayerDead = delegate { };


    /// <summary>
    /// Check player position, if it is out of boundary, move it back
    /// </summary>
    private void BoundaryCheck()
    {
        float xPosition = Mathf.Clamp(Position.x, LevelManager.m_Instance.LeftLevelBoundary, LevelManager.m_Instance.RightLevelBoundary);
        float yPosition = Mathf.Clamp(Position.y, LevelManager.m_Instance.LowerLevelBoundary, LevelManager.m_Instance.UpperLevelBoundary);

        Position = new Vector3(xPosition, yPosition, Position.z);
    }

    private void GoToLevelSpawnPosition()
    {
        UpdatePlayerPosition(LevelManager.m_Instance.m_PlayerSpawnPosition.position);
        m_currentAttributes = m_ExceptedAttributes.ValueClone();
    }

    private void GenerateWeapon()
    {
        DefaultWeapon = Instantiate(m_currentAttributes.DefaultWeapon, transform);
        FireWeapon = Instantiate(m_currentAttributes.FireWeapon, transform);
        MagneticWeapon = Instantiate(m_currentAttributes.MegneticWeapon, transform);
        LightWeapon = Instantiate(m_currentAttributes.LightWeapon, transform);
        ProjectileWeapon = Instantiate(m_currentAttributes.ProjectileWeapon, transform);
    }
    #endregion

    #region Player Model API
    public void UpdatePlayerMovement(Vector2 moveData)
    {
        Movement = m_isSlowDown ? moveData * m_currentAttributes.playerAttributes.SlowDownRatio : moveData;        
    }

    public void UpdatePlayerPosition(Vector2 move2D)
    {
        if (m_currentAttributes == null)
        {
            Debug.LogError($"{this.name} needs attributes config");
            return;
        }

        Vector3 moveDistance = new Vector3(
            move2D.x * m_currentAttributes.playerAttributes.Speed * Time.fixedDeltaTime,
            move2D.y * m_currentAttributes.playerAttributes.Speed * Time.fixedDeltaTime,
            Position.z
        );

        Position += moveDistance;

        BoundaryCheck();
    }

    public void UpdatePlayerPosition(Vector3 position)
    {
        Position = position;
    }

    public void UpdatePlayerHealth(int health)
    {
        int oldValue = m_currentAttributes.playerAttributes.CurrentHP;
        m_currentAttributes.playerAttributes.CurrentHP = Mathf.Clamp(health, 0, m_currentAttributes.playerAttributes.MaxHP);
        OnHealthUpdate?.Invoke(m_currentAttributes.playerAttributes.CurrentHP, oldValue, m_currentAttributes.playerAttributes.MaxHP);

        if(health <= 0)
        {
            LevelManager.m_Instance.retMain(true);
            OnPlayerDead?.Invoke();
            //Debug.Log("You Lose!");
        }
    }

    public void UpdateCurrentWeapon(WeaponController controller)
    {
        if (controller.gameObject != DefaultWeapon)
        {
            OnChangeTestWeapon?.Invoke();
        }

        currentWeapon = controller;
        OnWeaponChange?.Invoke(currentWeapon, currentWeapon.m_WeaponType);
    }

    public void UpdatePlayerEnergy(int energy)
    {
        int oldValue = m_currentAttributes.playerAttributes.CurrentEnergy;
        m_currentAttributes.playerAttributes.CurrentEnergy = Mathf.Clamp(energy, 0, m_currentAttributes.playerAttributes.MaxEnergy);
        OnEnergyUpdate?.Invoke(m_currentAttributes.playerAttributes.CurrentEnergy, oldValue, m_currentAttributes.playerAttributes.MaxEnergy);
    }

    public void UpdateSlowDownState(bool slowDown)
    {
        m_isSlowDown = slowDown;
        OnSlowDownStateChange?.Invoke(slowDown);
    }

    public void UpdateFiringState(bool isFiring)
    {
        m_isFiring = isFiring;
        OnFireStateUpdate?.Invoke(isFiring);
    }

    #endregion

    #region MonoBehavior
    private void Awake()
    {
        GoToLevelSpawnPosition();
        GenerateWeapon();
    }

    private void OnEnable()
    {
        OnMovementUpdate += UpdatePlayerPosition;
    }

    private void OnDisable()
    {
        OnMovementUpdate -= UpdatePlayerPosition;
    }

    private void Start()
    {
        currentWeapon = DefaultWeapon.GetComponent<WeaponController>();
    }
    #endregion
}
