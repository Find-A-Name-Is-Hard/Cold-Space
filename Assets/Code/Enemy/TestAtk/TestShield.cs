using System;
using UnityEngine;

[Obsolete("This class is out of date, please use test attack shield")]
public class TestShield : MonoBehaviour, IAtkReceiver
{
    public Action OnShieldBreak = delegate { };
    public string m_PlayerTestAtkTag = "PlayerTestAttack";
    public float m_MaxHP = 200;
    public float m_CurrentHP = 200;
    private SpriteRenderer m_Renderer;

    public void GetDamge(GameObject caster, float value, HardClues atkType)
    {
        if(caster.CompareTag(m_PlayerTestAtkTag))
        {
            m_CurrentHP -= value;
            if(m_CurrentHP < 0)
            {
                OnShieldBreak?.Invoke();
                LevelManager.m_Instance.NotifyTestAtkEnd();
                gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        m_CurrentHP = m_MaxHP;
    }

    private void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        SetSpriteAlpha();
    }

    private void SetSpriteAlpha()
    {
        if (m_Renderer != null)
        {
            float alpha = (m_CurrentHP / m_MaxHP) * 0.3f + 0.1f;
            Color color = new Color(m_Renderer.color.r, m_Renderer.color.g, m_Renderer.color.b, alpha);

            m_Renderer.color = color;
        }
    }
}
