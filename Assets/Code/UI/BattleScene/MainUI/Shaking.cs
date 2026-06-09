using System.Collections;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class Shaking : MonoBehaviour
{
    public float m_ShakingTime = 0.5f;
    public float m_ShakingDistance = 20f;
    private RectTransform m_rectTransform;
    private bool m_IsShaking = false;
    private Vector3 m_OriginalPos; 

    public void StartShaking(int current, int maxHP)
    {
        //if (m_IsShaking) return;

        if (m_rectTransform == null)
        {
            m_rectTransform = GetComponent<RectTransform>();
        }

        StopAllCoroutines();
        //m_OriginalPos = m_rectTransform.position;
             
        StartCoroutine(SimpleShakingAnim());
    }

    private IEnumerator SimpleShakingAnim()
    {
        //m_IsShaking = true;

        float timer = 0f;

        if (m_rectTransform == null) yield break;

        // Shaking loop
        while (timer < m_ShakingTime)
        {
            timer += Time.deltaTime;

            // Sin shaking
            float yOffset = Mathf.Sin(timer * 50f) * m_ShakingDistance * (1 - timer / m_ShakingTime);

            m_rectTransform.localPosition = m_OriginalPos + new Vector3(0, yOffset, 0);

            yield return null;
        }

        m_rectTransform.localPosition = m_OriginalPos;

        //m_IsShaking = false;
        yield break;
    }

    private IEnumerator TryRegisterLevelManager()
    {
        yield return new WaitUntil(() => { return (LevelManager.m_Instance != null); } );
        LevelManager.m_Instance.OnPlayerHealthUpdate += StartShaking;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCoroutine(TryRegisterLevelManager());  
        m_OriginalPos = new Vector3(m_rectTransform.position.x, 0, m_rectTransform.position.z);   
    }

    private void OnDisable()
    {
        LevelManager.m_Instance.OnPlayerHealthUpdate -= StartShaking;
    }

}
