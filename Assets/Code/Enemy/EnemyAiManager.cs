using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAiManager : MonoBehaviour
{
    public EnemyAttrtibutes[] allPossibleEnemies;
    public EnemyAttrtibutes enemAttributes;
    public float timeTillNextAttack = float.MaxValue;
    public float defaultTimeBetweenAttacks = 10f;
    public float m_remainingTimeBetweenAttacks = 3; 

    public List<AttackPatternData>  attackQueue;
    public AttackPatternData lastUsedAttack;
    //public BulletsSpawner bulletsSpawner;
    public SimpleBoss simpleBoss;
    public EnemyDifficulty enemDifficulty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //bulletsSpawner = FindFirstObjectByType<BulletsSpawner>();
        pickSecretEnemy();
        //SetupAttackDeck();
        timeTillNextAttack = 0;
        StartCoroutine(StartAttackPattern());   
    }

    // Update is called once per frame
    void Update()
    {
        //timeTillNextAttack -= Time.deltaTime;
        //if (timeTillNextAttack <= 0)
        //{
            

        //    m_remainingTimeBetweenAttacks -= Time.deltaTime;
        //    if(m_remainingTimeBetweenAttacks <= 0)
        //    {
        //        executeNewAttack();
        //    }            
        //}
    }

    private IEnumerator StartAttackPattern()
    {
        while(true)
        {
            yield return new WaitUntil(() => { return simpleBoss.m_BulletsSpawner != null; });
            simpleBoss.m_BulletsSpawner.DisableShooting();
            yield return new WaitForSeconds(m_remainingTimeBetweenAttacks);
            executeNewAttack();
            yield return new WaitForSeconds(timeTillNextAttack);
            simpleBoss.StopMovement();
        }
    }

    void executeNewAttack()
    {
        //Repeat until there is a new queue to pick stuff from
        while (attackQueue.Count==0)
        {
            SetupAttackDeck();
        }
        
        lastUsedAttack = attackQueue[0];
        attackQueue.RemoveAt(0);
        
        timeTillNextAttack = lastUsedAttack.attackLength;

        enemDifficulty.PassAndConvertAttackData(lastUsedAttack, simpleBoss);
        //simpleBoss.UpdateSpawnerConfig(lastUsedAttack);
        
        Debug.Log(lastUsedAttack);
        //Send a message to a bullet manager.
        //bulletsSpawner.setAttackPattern(lastUsedAttack);
        m_remainingTimeBetweenAttacks = lastUsedAttack.TimeBetweenAttack;
    }

    void pickSecretEnemy()
    {
        enemAttributes = allPossibleEnemies[Random.Range(0, allPossibleEnemies.Length)];
        CopilotDialogueManager.Instance.populateEnemRemainingList(this);

        Debug.Log("we picked the enemy: " + (enemAttributes.EnemyName));
    }

    void SetupAttackDeck()
    {
        attackQueue.Clear();
        float randomCountUp = 0f;
        //evaluate where we want to pick the from the curve at based on difficulty
        float EvaluateCurveAt = Mathf.Clamp(enemDifficulty.difficulty / enemDifficulty.diffData.MaxDifficulty, 0, 1);
        foreach (AttackWithProbability attackData in enemAttributes.deckData.attackDeck)
        {
            //don't repeat attacks
            if (attackData.attackPattern!=lastUsedAttack)
            {
                // For each miss increase chance of picking the next one.
                randomCountUp += Random.Range(0f, 1f);
                //if roll over 1 - the chance of the attack pick the attack
                if (randomCountUp > (1f - attackData.attackChanceAtDifficulty.Evaluate(EvaluateCurveAt)))
                {
                    attackQueue.Add(attackData.attackPattern);
                    randomCountUp = 0;
                    Debug.Log("Picked Attack" + attackData.attackPattern);
                }
            }
            
        }

        // fisher�yates shuffle     
        for (int i = 0; i < attackQueue.Count; i++)
        {
            // Pick random Element
            int j = Random.Range(i, attackQueue.Count);

            // Swap Elements
            AttackPatternData tmp = attackQueue[i];
            attackQueue[i] = attackQueue[j];
            attackQueue[j] = tmp;
        }
    }


}
