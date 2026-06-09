using System.Collections;
using UnityEngine;

public class FlreWeaponController : WeaponController
{
    #region Fire Weapon Controller
    [SerializeField] private GameObject m_bullet;
    [SerializeField] private GameObject m_hint;
    [SerializeField] private float m_flickingTime = 0.1f;
    [SerializeField] private float m_flickingTimes = 3;
    [SerializeField] private float m_flickingTimeInerval = 0.1f;
    [SerializeField] private float m_existTime = 10;
    [SerializeField] private float m_cooldDownTime = 10;
    
    private Coroutine m_fireCoroutine;

    private IEnumerator FireCoroutine()
    {
        SpriteRenderer m_fireRnderer = m_bullet.GetComponent<SpriteRenderer>();
        float restTime = 0;

        // Initiate
        yield return new WaitUntil(() => m_bullet != null);
        m_bullet.SetActive(false);
        yield return new WaitUntil(() => m_hint != null);
        m_hint.SetActive(false);

        while (true)
        {
            // If cannot shoot, end coroutine
            if (!m_isAbleToShoot) { yield return new WaitUntil(() => m_isAbleToShoot); }

            // Hint flicking visual effect 
            for (int i = 0; i < m_flickingTimes; i++)
            {
                m_hint.SetActive(true);
                yield return new WaitForSeconds(m_flickingTime);
                m_hint.SetActive(false) ;
                yield return new WaitForSeconds(m_flickingTimeInerval);
            }

            // Start Fire
            m_bullet?.SetActive(true);
            m_fireRnderer.color = new Color(m_fireRnderer.color.r, m_fireRnderer.color.g, m_fireRnderer.color.b, 1);
            restTime = m_existTime;

            SoundEffectManager.Instance.PlayerContinuousAttack(PlayerHitEvent.FlammableTest, true);

            while (restTime > 0)
            {
                m_fireRnderer.color = new Color(m_fireRnderer.color.r, m_fireRnderer.color.g, m_fireRnderer.color.b, restTime / m_existTime);
                yield return null;
                restTime -= Time.deltaTime;
            }

            

            // Stop Fire and enter cool down
            SoundEffectManager.Instance.PlayerContinuousAttack(PlayerHitEvent.FlammableTest, false);
            m_bullet?.SetActive(false);
            yield return new WaitForSeconds(m_cooldDownTime);
        }     
    }

    public override void HandleShootingStartInput()
    {
        base.HandleShootingStartInput();
        
    }

    public override void HandleShootingEndInput()
    {
        base.HandleShootingEndInput();
        //m_bullet?.SetActive(false);
        //m_hint?.SetActive(false);
    }

    public override void HandleWeaponChangeEvent(/*WeaponController controller*/)
    {
        //if (controller != null && controller != this)
        //{
            
        //}
        base.HandleWeaponChangeEvent(/*controller*/);

        StopCoroutine(m_fireCoroutine);
        m_bullet?.SetActive(false);
        m_hint?.SetActive(false);
        m_fireCoroutine = StartCoroutine(FireCoroutine());
    }
    #endregion

    #region MonoBhv
    protected override void OnEnable()
    {
        base.OnEnable();

        m_bullet?.SetActive(false);
        m_hint?.SetActive(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();


        m_bullet?.SetActive(false);
        m_hint?.SetActive(false);
    }

    protected void Start()
    {
        m_bullet?.SetActive(false);
        m_hint?.SetActive(false);

        m_fireCoroutine = StartCoroutine(FireCoroutine());
    }

    #endregion
}
