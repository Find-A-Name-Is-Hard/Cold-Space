using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBullet : MonoBehaviour
{
    [Header("Bezier Phase Arguments")]
    public Vector3 m_Start;
    public Vector3 m_End;
    public Vector3 m_ControlPoint;

    /// <summary>
    /// How long does it takes when bullet hits end point
    /// </summary>
    public float m_BezierDuration = 2;
    private float m_runTime = 0;
    public float m_LifeTime = 5;

    [Header("Bullet Arugments")]
    public string m_EnermyLayer = "Enemy";
    public string m_EnemyBulletLayer = "EnemyBullets";
    public GameObject m_HitEffect;
    public float m_Damage = 5;
    public bool m_IsReady2Go = false;
    private int enemyLayer;
    private int enemyBulletLayer;


    private Vector3 GetQuadraticBezierPoint (float weight, Vector3 start, Vector3 end, Vector3 controlPoint1)
    {
        float a = 1 - weight;
        Vector3 result = a * a * start + 2 * a * weight * controlPoint1 + weight * weight * end;
        return result;
    }

    private Vector3 GetQuadraticBezierDerivative(float weight, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return 2 * (1 - weight) * (p1 - p0) + 2 * weight * (p2 - p1);
    }

    private void ManageLifeCycle()
    {
        m_runTime += Time.fixedDeltaTime;

        if(m_runTime > m_LifeTime)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        // If the bullet is not recyclable, destroy it. 
        if (GetComponent<SelfRecycledHelper>() == null)
        {
            Destroy(gameObject);
        }

        // If it is recycleable, disable it and trigger the recycle method in self recyced helper
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateTransform()
    {
        float weight = Mathf.Clamp01(m_runTime / m_BezierDuration);
        Vector3 velocity = GetQuadraticBezierDerivative(weight, m_Start, m_ControlPoint, m_End);
        Vector3 direction = velocity.normalized;
        transform.up = direction;
        transform.Translate(velocity * (Time.fixedDeltaTime / m_BezierDuration), Space.World);
    }

    private IEnumerator StartMovement()
    {
        yield return new WaitUntil(() => { return m_IsReady2Go; });

        m_runTime = 0;
        transform.position = m_Start;
        enemyLayer = LayerMask.NameToLayer(m_EnermyLayer);
        enemyBulletLayer = LayerMask.NameToLayer(m_EnemyBulletLayer);

        while(true)
        {
            ManageLifeCycle();
            UpdateTransform();
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    #region MonoBehavior
    private void OnEnable()
    {
        StartCoroutine(StartMovement());
    }

    private void OnDisable()
    {
        m_IsReady2Go = false;
    }

    private void FixedUpdate()
    {
        
        //UpdateTransform();
        //if(m_runTime <= m_BezierDuration)
        //{
        //    Vector3 newPosition = GetQuadraticBezierPoint(weight, m_Start, m_End, m_ControlPoint);
        //    Vector3 velocity = GetQuadraticBezierDerivative(weight, m_Start, m_ControlPoint, m_End);
        //    Vector3 direction = velocity.normalized;
        //    transform.up = direction;
        //    transform.position = newPosition;
        //}
        //else
        //{
        //    Vector3 velocity = GetQuadraticBezierDerivative(weight, m_Start, m_ControlPoint, m_End);
        //    transform.Translate(velocity * (Time.fixedDeltaTime / m_BezierDuration), Space.World);
        //}
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        // Clear enemy bullets
        if (collision.gameObject.layer == enemyBulletLayer)
        {
            Destroy(collision.gameObject);
            return;
        }

        // Apply damage 
        if (collision.gameObject.layer == enemyLayer)
        {
            if (!collision.TryGetComponent<IAtkReceiver>(out var rec)) return;

            rec.GetDamge(this.gameObject, m_Damage, HardClues.Explosive);
            Instantiate(m_HitEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    #endregion
}
