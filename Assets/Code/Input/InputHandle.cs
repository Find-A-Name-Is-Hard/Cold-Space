using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputHandle : MonoBehaviour
{
    #region InputHandle Details
    public InputMapping m_input;
    public bool m_isSettingMenuOpened = false;

    public Action<Vector2> MovementEvent = delegate { };
    public Action ShootingPerformedEvent = delegate { };
    public Action ShootingCanceledEvent = delegate { };
    public Action SlowDownPerformedEvent = delegate { };
    public Action SlowDownCanceledEvent = delegate { };
    /// <summary>
    /// Bool is enable or not of test atk toggle
    /// </summary>
    public Action<bool> TestAtkToggleEvent = delegate { };
    /// <summary>
    /// Bool is enable or not of cure atk toggle
    /// </summary>
    public Action<bool> CureAtkToggleEvent = delegate { };
    public Action<int> TestAtkButtonPerfomedEvent = delegate { };
    public Action<int> CureAtkButtonPerfomedEvent = delegate { };
    public Action SettingMenuPerformedEvent = delegate { };

    public static InputHandle m_Instance { get; private set; }

    #region Action Toggle
    public void EnableTestAtkAction()
    {
        m_input.TestAtk.Enable();
        m_input.TestAtk.ATKButton.performed += OnTestAtkButtonPerformed;
    }

    public void DisableTestAtkAction()
    {
        m_input.TestAtk.ATKButton.performed -= OnTestAtkButtonPerformed;
        m_input.TestAtk.Disable();        
    }

    public void EnablePlayerAction()
    {
        m_input.Player.Enable();
        m_input.Player.NormalAtk.performed += OnNormalAtkPerformed;
        m_input.Player.NormalAtk.canceled += OnNormalAtkCancleed;
        m_input.Player.SlowDown.performed += OnSlowDownPerformed;
        m_input.Player.SlowDown.canceled += OnSlowDownCanceled;
    }

    public void DisablePlayerAction()
    {
        m_input.Player.Disable();
        m_input.Player.NormalAtk.performed -= OnNormalAtkPerformed;
        m_input.Player.NormalAtk.canceled -= OnNormalAtkCancleed;
        m_input.Player.SlowDown.performed -= OnSlowDownPerformed;
        m_input.Player.SlowDown.canceled -= OnSlowDownCanceled;
    }

    public void EnableCureAtkAction()
    {
        m_input.CureAtk.Enable();
        m_input.CureAtk.ATKButton.performed += OnCureAtkButtonPerformed;
    }

    public void DisableCureAtkAction()
    {
        m_input.CureAtk.ATKButton.performed -= OnCureAtkButtonPerformed;
        m_input.CureAtk.Disable();
    }

    public void EnableTestToggleAction()
    {
        m_input.AttackToggle.Enable();
        m_input.AttackToggle.TestAtkToggle.performed += OnTestAtkTogglePerformed;
        
    }

    public void DisableTESTToggleAction()
    {
        m_input.AttackToggle.Disable();
        m_input.AttackToggle.TestAtkToggle.performed -= OnTestAtkTogglePerformed;
    }

    public void EnableCureToggleAction()
    {
        m_input.CureToggle.Enable();
        m_input.CureToggle.CureAtkToggle.performed += OnCureAtkTogglePerformed;
    }

    public void DisableCureToggleAction()
    {
        m_input.CureToggle.Disable();
        m_input.CureToggle.CureAtkToggle.performed -= OnCureAtkTogglePerformed;
    }

    public void EnableSettingMenuAction()
    {
        m_input.SettingMenu.Enable();
        m_input.SettingMenu.ESC.performed += OnSettingMenuPerformed;
    }

    public void DisableSettingMenuAction()
    {
        m_input.SettingMenu.Disable();
        m_input.SettingMenu.ESC.performed -= OnSettingMenuPerformed;
    }

    #endregion

    #region Event 
    private void OnMoveEventPerformed(Vector2 input)
    {
        if (input == Vector2.zero) return;

        MovementEvent?.Invoke(input);
        //UnityEngine.Debug.Log(input);
    }

    private void OnNormalAtkCancleed(InputAction.CallbackContext context)
    {
        ShootingCanceledEvent?.Invoke();
        //UnityEngine.Debug.Log(m_input.Player.NormalAtk.phase);
    }

    private void OnNormalAtkPerformed(InputAction.CallbackContext context)
    {
        ShootingPerformedEvent?.Invoke();
        //UnityEngine.Debug.Log(m_input.Player.NormalAtk.phase);
    }

    private void OnTestAtkTogglePerformed(InputAction.CallbackContext context)
    {

        if (m_input.TestAtk.enabled)
        {
            BattleInputConfig();
            if(Time.timeScale <= 0)
                Time.timeScale = 1;
        }
        else
        {
            TestAtkSelectionConfig();
            if (Time.timeScale > 0)
                Time.timeScale = 0;
        }


        TestAtkToggleEvent?.Invoke(m_input.TestAtk.enabled);

        //UnityEngine.Debug.Log(m_input.TestAtk.enabled);
    }

    private void OnCureAtkTogglePerformed(InputAction.CallbackContext context)
    {
        if (m_input.CureAtk.enabled)
        {
            BattleInputConfig();
            if (Time.timeScale <= 0)
                Time.timeScale = 1;
        }
        else
        {
            CureAtkSelectionConfig();
            if (Time.timeScale > 0)
                Time.timeScale = 0;
        }

        CureAtkToggleEvent?.Invoke(m_input.CureAtk.enabled);

        //UnityEngine.Debug.Log(m_input.CureAtk.enabled);
    }

    private void OnTestAtkButtonPerformed(InputAction.CallbackContext context)
    {
        string keyName = context.control.name;

        if (int.TryParse(keyName, out int value))
        {
            if (Time.timeScale <= 0)
                Time.timeScale = 1;
            TestAtkButtonPerfomedEvent?.Invoke(value);
        }
        else
        {
            UnityEngine.Debug.LogError("Cannot Sent test atk button performed event");
        }

        BattleInputConfig();

        //UnityEngine.Debug.Log( "Test" + keyName);
    }

    private void OnCureAtkButtonPerformed(InputAction.CallbackContext context)
    {
        string keyName = context.control.name;

        if (int.TryParse(keyName, out int value))
        {
            if (Time.timeScale <= 0)
                Time.timeScale = 1;
            CureAtkButtonPerfomedEvent?.Invoke(value);
        }
        else
        {
            UnityEngine.Debug.LogError("Cannot Sent cure atk button performed event");
        }

        BattleInputConfig();
        //UnityEngine.Debug.Log("Cure" + keyName);
    }

    public void OnSettingMenuPerformed(InputAction.CallbackContext context)
    {
        if(m_input.SettingMenu.enabled == false) return;

        m_isSettingMenuOpened = !m_isSettingMenuOpened;

        if (m_isSettingMenuOpened) SettingMenuConfig();
        else BattleInputConfig();

        Time.timeScale = (m_isSettingMenuOpened) ? 0f : 1f;
        SettingMenuPerformedEvent?.Invoke();
        
    }

    public void OnSettingMenuPerformed()
    {
        if(m_input.SettingMenu.enabled == false) return;
        
        m_isSettingMenuOpened = !m_isSettingMenuOpened;

        if (m_isSettingMenuOpened) SettingMenuConfig();
        else BattleInputConfig();

        Time.timeScale = m_isSettingMenuOpened ? 0f : 1f;
        SettingMenuPerformedEvent?.Invoke();
        
    }

    private void OnSlowDownPerformed(InputAction.CallbackContext context)
    {
        SlowDownPerformedEvent?.Invoke();
    }

    private void OnSlowDownCanceled(InputAction.CallbackContext context)
    {
        SlowDownCanceledEvent?.Invoke();
    }

    #endregion

    #region Input Configs
    private void BattleInputConfig()
    {
        EnablePlayerAction();
        EnableTestToggleAction();
        EnableCureToggleAction();
        DisableTestAtkAction();
        DisableCureAtkAction();
        EnableSettingMenuAction();
    }

    private void TestAtkSelectionConfig()
    {
        DisablePlayerAction();
        EnableTestToggleAction();
        EnableTestAtkAction();
        DisableCureToggleAction();
        DisableCureAtkAction();
        DisableSettingMenuAction();
    }

    private void CureAtkSelectionConfig()
    {
        DisablePlayerAction();
        DisableTESTToggleAction();
        EnableCureToggleAction();
        DisableTestAtkAction();
        EnableCureAtkAction();
        DisableSettingMenuAction();
    }

    private void SettingMenuConfig()
    {
        DisablePlayerAction();
        DisableTESTToggleAction();
        DisableTestAtkAction();
        DisableCureToggleAction();
        DisableCureAtkAction();
        EnableSettingMenuAction();
    }

    #endregion

    #endregion

    #region MonoBehavior
    protected void Awake()
    {
        m_Instance = this;
        m_input = new InputMapping();
    }

    private void OnEnable()
    {
        m_input.Enable();

        BattleInputConfig();

    }

    private void OnDisable()
    {
        m_input.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 v = m_input.Player.Move.ReadValue<Vector2>();

        if (m_input.Player.enabled && m_input.Player.Move.IsPressed())
        {
            OnMoveEventPerformed(v);
        }

    }
    #endregion
}
