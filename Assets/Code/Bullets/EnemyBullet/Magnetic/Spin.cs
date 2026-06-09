using System.Collections;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float m_RotatingAngle = 180;
    public float m_RotatingTime = 0.5f;
    public float m_TremblingAngle = 5;
    public float m_TremblingTime = 0.2f;

    private float bigRotateTimer = 0f;

    private void OnEnable()
    {
        StartCoroutine(SimpleAnim());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SimpleAnim()
    {
        while (true)
        {
            // --- Trembling around current angle ---
            float baseAngle = transform.localEulerAngles.z;
            float jitter = Random.Range(0, 2) == 0 ? m_TremblingAngle : -m_TremblingAngle;
            transform.localRotation = Quaternion.Euler(0, 0, baseAngle + jitter);

            // Timer for the big rotation
            bigRotateTimer += m_TremblingTime;
            if (bigRotateTimer >= 2f)
            {
                bigRotateTimer = 0f;
                yield return StartCoroutine(RotateBig());
            }

            yield return new WaitForSeconds(m_TremblingTime);
        }
    }

    private IEnumerator RotateBig()
    {
        float timer = 0f;
        float start = transform.localEulerAngles.z;
        float end = start + m_RotatingAngle;

        while (timer < m_RotatingTime)
        {
            timer += Time.deltaTime;
            float t = timer / m_RotatingTime;

            float angle = Mathf.Lerp(start, end, t);
            transform.localRotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }
    }
}
