using UnityEngine;
using System.Collections;
using UnityEngine.Events;
//using System.Collections.Generic;

public class EnemyDifficulty : MonoBehaviour
{
    public DifficultyConversionData diffData;
    [Range(0, 100)]
    public float difficulty = 0;
    public AttackPatternData apdClone;
    //public AttackPatternData preModified;
    public bool difficultyUpOverTime = false;


    //Event Registration
    public bool m_isRegisterTestEnd = false;
    public bool m_isRegisterTestStart = false;
    public bool m_isRegisterCureEnd = false;
    public bool m_isRegisterCureStart = false;
    //private List<int> usedAttacks = new List<int>();
    private bool currentSpecialAttack = false;

    private IEnumerator TryRegisterTestEnd()
    {
        while (!m_isRegisterTestEnd)
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestFinish += TestFinishEffect;
                m_isRegisterTestEnd = true;
            }
            yield return null;
        }
        yield break;
    }

    private void TestFinishEffect(bool isTestCorrect, HardClues clueName)
    {
        
        difficulty -= diffData.DifficultyDownAfterTestAttack;
        currentSpecialAttack = false;
        Debug.Log("Difficulty down by test end: " + diffData.DifficultyDownAfterTestAttack);
    }


    private IEnumerator TryRegisterTestStart()
    {
        while (!m_isRegisterTestStart)
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestAtkStart += TestStartAtkEffect;
                m_isRegisterTestEnd = true;
            }
            yield return null;
        }
        yield break;
    }

    //not sure what the int is
    private void TestStartAtkEffect(int someInt)
    {
        if (currentSpecialAttack==false)
        {
            //usedAttacks.Add(someInt);
            currentSpecialAttack = true;
            difficulty += diffData.DifficultyUpPerTestAttack;
            Debug.Log("Difficulty up by test start:" + diffData.DifficultyUpPerTestAttack);
            Debug.Log("Used this int: " + someInt);
        }
        
    }



    public void PassAndConvertAttackData(AttackPatternData attackPatternData, SimpleBoss simpleBoss)
    {
        apdClone = Object.Instantiate(attackPatternData);
        



        float EvaluateCurveAt = Mathf.Clamp(difficulty / diffData.MaxDifficulty,0,1);
        Debug.Log("Difficult Evaluated at: " + EvaluateCurveAt);



        for (int i = 0; i < apdClone.enemyAimedBulletDatas.Length; i++)
        {
            apdClone.enemyAimedBulletDatas[i] = Object.Instantiate(apdClone.enemyAimedBulletDatas[i]);
        }
        for (int i = 0; i < apdClone.enemyOriginBulletDatas.Length; i++)
        {
            apdClone.enemyOriginBulletDatas[i] = Object.Instantiate(apdClone.enemyOriginBulletDatas[i]);
        }
        for (int i = 0; i < apdClone.lineOriginBulletDatas.Length; i++)
        {
            apdClone.lineOriginBulletDatas[i] = Object.Instantiate(apdClone.lineOriginBulletDatas[i]);
        }



        foreach (fromEnemyAimedBulletData bulletData in apdClone.enemyAimedBulletDatas)
        {
            
            //Debug.Log("Original Bullet Data Count was:" + bulletData.Count);
            bulletData.Count = Mathf.FloorToInt(bulletData.Count * diffData.CountConversion.Evaluate(EvaluateCurveAt));
            //Debug.Log("Aimed Bullet Count Evaluated at" + diffData.CountConversion.Evaluate(EvaluateCurveAt));
            //Debug.Log("New Bullet Data Count is:" + bulletData.Count);
            bulletData.SpawnInterval *= diffData.SpawnIntervalConversion.Evaluate(EvaluateCurveAt);
            bulletData.AngleInterval *= diffData.AngleIntervalConversion.Evaluate(EvaluateCurveAt);

        }
        foreach (fromEnemyOriginBulletData bulletData in apdClone.enemyOriginBulletDatas)
        {
            //Debug.Log("Original Bullet Data Count was:" + bulletData.Count);
            bulletData.Count = Mathf.FloorToInt(bulletData.Count * diffData.CountConversion.Evaluate(EvaluateCurveAt));
            //Debug.Log("Enemy Bullet Count Evaluated at" + diffData.CountConversion.Evaluate(EvaluateCurveAt));
            //Debug.Log("New Bullet Data Count is:" + bulletData.Count);
            bulletData.SpawnInterval *= diffData.SpawnIntervalConversion.Evaluate(EvaluateCurveAt);
            bulletData.AngularAcceleration *= diffData.RotationAngularAccelerationConversion.Evaluate(EvaluateCurveAt);
            bulletData.MaxAngularVelocity *= diffData.RotationAngularVelocityMaxConversion.Evaluate(EvaluateCurveAt);
            bulletData.AngleInterval *= diffData.AngleIntervalConversion.Evaluate(EvaluateCurveAt);
        }
        foreach (fromLineOriginBulletData bulletData in apdClone.lineOriginBulletDatas)
        {
            //Debug.Log("Original Bullet Data Count was:" + bulletData.Count);
            bulletData.Count = Mathf.FloorToInt(bulletData.Count * diffData.CountConversion.Evaluate(EvaluateCurveAt));
            //Debug.Log("Line Bullet Count Evaluated at" + diffData.CountConversion.Evaluate(EvaluateCurveAt));
            //Debug.Log("New Bullet Data Count is:" + bulletData.Count);
            bulletData.SpawnInterval *= diffData.SpawnIntervalConversion.Evaluate(EvaluateCurveAt);

        }
        simpleBoss.UpdateSpawnerConfig(apdClone);

        // Broadcast modified data
        if(LevelManager.m_Instance != null)
        {
            LevelManager.m_Instance.OnEnemyAtkPatternUpdate.Invoke(apdClone);
        }
    }


    private void OnEnable()
    {
        StartCoroutine(TryRegisterTestEnd());
        StartCoroutine(TryRegisterTestStart());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.GetInt("GameDifficulty", 0) == 0)
        {
            difficulty = diffData.InitDifficultyEasy;
        }
        else if (PlayerPrefs.GetInt("GameDifficulty", 0) == 1)
        {
            difficulty = diffData.InitDifficultyMedium;
        }
        else
        {
            difficulty = diffData.InitDifficultyHard;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
        
    private void FixedUpdate()
    {
        if (difficultyUpOverTime)
        {
            difficulty += Time.deltaTime * diffData.DifficultyPerMinutePlayed * (1f / 60);
        }
    }
}
