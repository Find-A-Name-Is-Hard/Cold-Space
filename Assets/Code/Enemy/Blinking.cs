using System.Runtime.CompilerServices;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Blinking : MonoBehaviour
{
    public float eyeOpenTime;
    public float eyeClosedTime;
    [Range(0f, 1f)]
    public float variance;
    public bool isOpen = false;
    private float m_timeLeft = 0f;
    private SpriteRenderer m_spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_timeLeft-=Time.deltaTime;

        if (m_timeLeft<=0)
        {
            isOpen=!isOpen;
            if (isOpen)
            {
                m_spriteRenderer.color=Color.clear;
                m_timeLeft = eyeOpenTime * (1-Random.Range(-variance, variance));
            }
            else
            {
                m_spriteRenderer.color=Color.white;
                m_timeLeft = eyeClosedTime * (1-Random.Range(-variance, variance));
            }
        }

    }
}
