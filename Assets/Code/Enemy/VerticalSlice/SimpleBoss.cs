using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class SimpleBoss : MonoBehaviour
{
    //public TestShield m_Shield;
    public GameObject m_PositiveResult;
    public GameObject m_NegativeResult; 
    public AttackPatternData m_AttackPatternData;

    public BulletsSpawner m_BulletsSpawner;
    private SplineAnimate m_SplineAnimator;
    public EnemyDifficulty m_enemDifficulty;

    private bool m_isRegisterTestEnd = false;
    private bool canMove = false;

    // We can add different test reaction anim accroding to different clue name and test result boolean
    private void StartTestEndEffect(bool isTestCorrect, HardClues clueName)
    {
        Debug.Log(isTestCorrect);
        StartCoroutine(SimpleTestResultCoroutine(isTestCorrect));
    }

    private IEnumerator SimpleTestResultCoroutine(bool isTestCorrect)
    {
        GameObject temp = isTestCorrect ? m_PositiveResult : m_NegativeResult;
        temp.SetActive(true);
        yield return new WaitForSeconds(10);
        temp.SetActive(false);
        yield break;
    }

    private IEnumerator TryRegisterTestEnd()
    {
        while (!m_isRegisterTestEnd)
        {
            if(LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestFinish += StartTestEndEffect;
                m_isRegisterTestEnd = true;
            }
            yield return null;
        }
        yield break;
    }

    private IEnumerator SimpleAnim()
    {
        yield return new WaitForSeconds(3);

        Vector3 targetPos = new Vector3(transform.position.x, 2f, transform.position.z);
        float speed = 2f;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        //m_BulletsSpawner.UpdateSpawnerConfig(m_AttackPatternData);
        //m_BulletsSpawner.EnableShooting();
        //enable spline animator
        m_SplineAnimator.enabled = true;
        canMove = true;
        m_enemDifficulty.difficultyUpOverTime = true;
        yield return new WaitForSeconds(2f);

        // bool isClockWise = false;
        //float angularAcceleration = -30f;

        //m_BulletsSpawnerAttributes.AngularAcceleration = angularAcceleration;
        //m_BulletsSpawner.UpdateSpawnerConfig(m_AttackPatternData);
        yield return new WaitForSeconds(3);

        //while (true)
        //{
            //angularAcceleration = (isClockWise = !isClockWise) ? -30f : 30f;

            //m_BulletsSpawnerAttributes.AngularAcceleration = angularAcceleration;
            //m_BulletsSpawner.UpdateSpawnerConfig(m_BulletsSpawnerAttributes, false);
            //yield return new WaitForSeconds(6);

        //}
    }

    public void UpdateSpawnerConfig(AttackPatternData apd)
    {
        m_AttackPatternData = apd;
        m_BulletsSpawner.UpdateSpawnerConfig(m_AttackPatternData);
        m_BulletsSpawner.EnableShooting();

        //animation
        if (apd.splineMovementData.Length!=0)
        {
            m_SplineAnimator.Container = apd.splineMovementData[Random.Range(0, apd.splineMovementData.Length)];
            m_SplineAnimator.Restart(true);
            m_SplineAnimator.Duration = apd.attackLength;
            m_SplineAnimator.enabled = canMove;
        }
        else
         {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        m_SplineAnimator.enabled = false;
    }

    private void OnEnable()
    {
        StartCoroutine(TryRegisterTestEnd());
    }

    private void OnDisable()
    {
        LevelManager.m_Instance.OnTestFinish -= StartTestEndEffect;
        m_isRegisterTestEnd = false;
    }

    private void Start()
    {
        m_BulletsSpawner = GetComponent<BulletsSpawner>();
        m_SplineAnimator = GetComponent<SplineAnimate>();
        m_PositiveResult.SetActive(false);
        m_NegativeResult.SetActive(false);
        m_BulletsSpawner.DisableShooting();

        StartCoroutine(SimpleAnim());
    }


}
