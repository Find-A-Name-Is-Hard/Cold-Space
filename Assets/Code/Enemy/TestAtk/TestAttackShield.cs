using UnityEngine;

[RequireComponent (typeof(TestAttackReceiver))]
public class TestAttackShield : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    private TestAttackReceiver m_receiver; 

    private void SetSpriteAlpha()
    {
        if (m_renderer != null)
        {
            float alpha = (m_receiver.m_testShieldCurrentHP / m_receiver.m_testShieldMaxHP) * 0.9f + 0.1f;
            Color color = new Color(m_renderer.color.r, m_renderer.color.g, m_renderer.color.b, alpha);

            m_renderer.color = color;
        }
    }

    private void Start()
    {
        m_receiver = GetComponent<TestAttackReceiver>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        SetSpriteAlpha();
    }
}
