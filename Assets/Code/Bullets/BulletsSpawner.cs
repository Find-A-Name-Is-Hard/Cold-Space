using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BulletsSpawner : MonoBehaviour
{
    #region BulletsSpawnerImplement

    [Header("Spawner Configuration")]
    //public fromEnemyOriginBulletData m_Config;
    public AttackPatternData m_Config;
    public GameObject playerTarget;
    private bool hasFromEnemyData = false;
    private bool hasFromAimedData = false;
    private bool hasFromLineData = false;
    private float m_currentTime = 0;

    //Maximum of 10 concurrant attacks this is arbitrary
    private float[] m_LastSpawnTime = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private float[] m_currentRotation = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private float[] m_currentAngularVelocity = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //For the aimed attack/ Theoretically we don't need this but I got confused.
    private Vector3[] m_currentRotationVector = { new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0),
                                                  new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0)};

    private Vector2[] m_startPos = { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
                                    Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};
    private Vector2[] m_endPos = { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
                                    Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};

    private bool m_isAllowedToShoot = false;

    private void UpdatePrivateVariables()
    {
        int attackIterator = 0;

        if (hasFromEnemyData)
        {
            

            foreach (fromEnemyOriginBulletData bulletData in m_Config.enemyOriginBulletDatas)
            {
                m_currentAngularVelocity[attackIterator] = bulletData.InitAngularVelocity;
                m_currentRotation[attackIterator] = bulletData.InitRotation;

                attackIterator++;
            }
        }

        if (hasFromAimedData)
        {
            foreach (fromEnemyAimedBulletData bulletData in m_Config.enemyAimedBulletDatas)
            {
                //m_currentRotation[attackIterator] = bulletData.InitRotation;
                m_currentRotationVector[attackIterator] = new Vector3(0, -1, 0);

                attackIterator++;
            }
        }

        if (hasFromLineData)
        {
            foreach (fromLineOriginBulletData bulletData in m_Config.lineOriginBulletDatas)
            {
                m_startPos[attackIterator] = bulletData.startPos;
                m_endPos[attackIterator] = bulletData.endPos;

                attackIterator++;
            }
        }
                 
    }

    private void UpdateBulletSpawnerTransform()
    {

        int attackIterator = 0;

        if (hasFromEnemyData)
        {
            //to change this to accept multiple attacks at once we need to iterate over each fromEnemy attack instead
            

            foreach (fromEnemyOriginBulletData bulletData in m_Config.enemyOriginBulletDatas)
            {
                float targetAngularVelocity = Mathf.Clamp(m_currentAngularVelocity[attackIterator] + bulletData.AngularAcceleration * Time.fixedDeltaTime, -bulletData.MaxAngularVelocity, bulletData.MaxAngularVelocity);
                m_currentAngularVelocity[attackIterator] = targetAngularVelocity;

                m_currentRotation[attackIterator] += m_currentAngularVelocity[attackIterator] * Time.fixedDeltaTime;
                if (Mathf.Abs(m_currentRotation[attackIterator]) >= 720f)
                {
                    m_currentRotation[attackIterator] -= Mathf.Sign(m_currentRotation[attackIterator]) * 360f;
                }

                //gameObject.transform.rotation = Quaternion.Euler(0, 0, m_currentRotation[attackIterator]);

                attackIterator++;
            }

            
        }

        if (hasFromAimedData)
        {
            foreach (fromEnemyAimedBulletData bulletData in m_Config.enemyAimedBulletDatas)
            {
                // Determine which direction to rotate towards
                Vector3 targetDirection = playerTarget.transform.position - transform.position;
                Vector3 currentDirection = m_currentRotationVector[attackIterator];
                //UnityEngine.Debug.Log("Target Direction: " + targetDirection + " Current Direction: " + currentDirection);

                // The step size is equal to speed times frame time.
                float singleStep = bulletData.AimingSpeed * Time.fixedDeltaTime;

                // Rotate the forward vector towards the target direction by one step
                Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, singleStep, 0.0f);

               

                m_currentRotationVector[attackIterator] = newDirection;

                //m_currentRotation[attackIterator] = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                m_currentRotation[attackIterator] = Vector3.SignedAngle(new Vector3 (0, -1), newDirection, new Vector3(0,0,1));

                //UnityEngine.Debug.Log("Firing in new Direction: " + newDirection + " Current Rotation angle: " + m_currentRotation[attackIterator]);

                attackIterator++;
            }
        }

        if (hasFromLineData)
        {
            foreach (fromLineOriginBulletData bulletData in m_Config.lineOriginBulletDatas)
            {
                if (bulletData.aimSpeed!=0)
                {
                    Vector2 midPoint = (m_startPos[attackIterator] + m_endPos[attackIterator]) / 2;
                    //Vector3 aimVector = Quaternion.Euler(0, 0, bulletData.projectileAngle).eulerAngles;

                    Vector3 aimVector = bulletData.projectileAngleVector;
                    Vector3 perpendicularAim = Vector2.Perpendicular(new Vector2(aimVector.x,aimVector.y));
                    Vector2 angleToTargetFromMidpoint = new Vector2 (playerTarget.transform.position.x, playerTarget.transform.position.y) - midPoint;

                    //print("aim Vector= " + aimVector+ "perpendicularAim = " + perpendicularAim + "positionWantToMoveTo= " + positionWantToMoveTo);
                    Vector2 targetDirectionVector = Vector3.Project(angleToTargetFromMidpoint, perpendicularAim);

                    m_startPos[attackIterator] = Vector3.MoveTowards(m_startPos[attackIterator], m_startPos[attackIterator] + targetDirectionVector, bulletData.aimSpeed * Time.fixedDeltaTime);
                    m_endPos[attackIterator] = Vector3.MoveTowards(m_endPos[attackIterator], m_endPos[attackIterator] + targetDirectionVector, bulletData.aimSpeed * Time.fixedDeltaTime);

                    //m_startPos[attackIterator] += targetDirectionVector;
                    //m_endPos[attackIterator] += targetDirectionVector;




                }

                attackIterator++;
            }
        }

    }

    private void SpawnBullets()
    {
        if (! m_isAllowedToShoot) return;

        //if (m_currentRow >= m_Config.Row && m_isAllowedToShoot == true)
        //{
        //    StartCoroutine(ExhibitionCoroutine());            
        //}

        int attackIterator = 0;
        if (hasFromEnemyData)
        {
            //Can later change this to a for each enemy originbulletdata in enemy originbulletdatas

            foreach (fromEnemyOriginBulletData bulletData in m_Config.enemyOriginBulletDatas)
            {
                if (m_currentTime - m_LastSpawnTime[attackIterator] >= bulletData.SpawnInterval)
                {
                    float shootingAngle = m_currentRotation[attackIterator];
                    shootingAngle += bulletData.AngleWaveRandomOffset * Random.Range(-.5f, .5f);


                    if ((bulletData.Count % 2 == 0))
                    {
                        shootingAngle += bulletData.AngleInterval / 2;
                    }


                    for (int i = 0; i < bulletData.Count; i++)
                    {
                        shootingAngle += Mathf.Pow(-1, i) * i * bulletData.AngleInterval;
                        float tempShootingAngle = shootingAngle + bulletData.AngleBulletRandomOffset * Random.Range(-.5f, .5f);
                        //print("Shooting Angle= " + shootingAngle + "TempShootingAngle =" + tempShoortingAngle);

                        GameObject bullet = GetBullet(bulletData.BulletPrefab);
                        bullet.transform.rotation = Quaternion.Euler(0, 0, tempShootingAngle);
                        Vector3 spawnDirection = bullet.transform.up;

                        bullet.transform.position = transform.position + (spawnDirection * bulletData.SpawnOffset);

                    }

                    // m_currentRow += 1;
                    m_currentTime = LevelManager.m_Instance.LevelTimer;
                    m_LastSpawnTime[attackIterator] = m_currentTime;
                }


                attackIterator++;
            }

        }

        if (hasFromAimedData)
        {
            foreach (fromEnemyAimedBulletData bulletData in m_Config.enemyAimedBulletDatas)
            {
                if (m_currentTime - m_LastSpawnTime[attackIterator] >= bulletData.SpawnInterval)
                {
                    float shootingAngle = m_currentRotation[attackIterator];
                    shootingAngle += bulletData.AngleWaveRandomOffset * Random.Range(-.5f, .5f);


                    if ((bulletData.Count % 2 == 0))
                    {
                        shootingAngle += bulletData.AngleInterval / 2;
                    }


                    for (int i = 0; i < bulletData.Count; i++)
                    {
                        shootingAngle += Mathf.Pow(-1, i) * i * bulletData.AngleInterval;
                        float tempShootingAngle = shootingAngle + bulletData.AngleBulletRandomOffset * Random.Range(-.5f, .5f);

                        GameObject bullet = GetBullet(bulletData.BulletPrefab);
                        bullet.transform.rotation = Quaternion.Euler(0, 0, tempShootingAngle);
                        Vector3 spawnDirection = bullet.transform.up;

                        bullet.transform.position = transform.position + (spawnDirection * bulletData.SpawnOffset);

                    }

                    // m_currentRow += 1;
                    m_currentTime = LevelManager.m_Instance.LevelTimer;
                    m_LastSpawnTime[attackIterator] = m_currentTime;
                }


                attackIterator++;
            }
        }

        if (hasFromLineData)
        {
            foreach (fromLineOriginBulletData bulletData in m_Config.lineOriginBulletDatas)
            {
                if (m_currentTime - m_LastSpawnTime[attackIterator] >= bulletData.SpawnInterval)
                {
                    //UnityEngine.Debug.Log("Spawn Line from " + bulletData.startPos + " to  " + bulletData.endPos);

                    //Pseudo code for more complicated stuff
                    Vector2 randomOffset = new Vector2( Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * bulletData.lineNoise;
                    Vector2 templineStart = m_startPos[attackIterator] + randomOffset;
                    Vector2 templineEnd = m_endPos[attackIterator] + randomOffset;
                    //lineStart[attackIterator] += lineDrift * spawnInterval
                    m_startPos[attackIterator] += bulletData.lineDrift * bulletData.SpawnInterval;
                    //lineEnd[attackIterator] += lineDrift * spawnInterval
                    m_endPos[attackIterator] += bulletData.lineDrift * bulletData.SpawnInterval;
                    for (int i = 0; i < bulletData.Count; i++)
                    {
                        GameObject bullet = GetBullet(bulletData.BulletPrefab);
                        //bullet.transform.rotation = Quaternion.Euler(0, 0, bulletData.projectileAngle); 
                        //bullet.transform.rotation = Quaternion.Euler(0,0,Vector3.SignedAngle(new Vector3(0, -1), bulletData.projectileAngleVector, new Vector3(0, 0, 1)));
                        bullet.transform.up = bulletData.projectileAngleVector;
                        //Vector3 spawnDirection = bullet.transform.up;
                        //Vector3 spawnDirection = bulletData.projectileAngleVector;

                        Vector2 randomPositionOffset = new Vector2(Random.Range(-bulletData.placementNoise, bulletData.placementNoise), Random.Range(-bulletData.placementNoise, bulletData.placementNoise));
                        bullet.transform.position = Vector2.Lerp(templineStart, templineEnd, (float)i / (bulletData.Count-1)) + randomPositionOffset;
                        //UnityEngine.Debug.Log($"Bullet{(float)i / bulletData.Count} Position : {Vector2.Lerp(templineStart, templineEnd, (float)i / bulletData.Count) + randomPositionOffset}, Random: {randomPositionOffset}");
                        //bullet.transform.position = Vector2.Lerp(templineStart, templineEnd, ((float)i+Random.Range(0f,bulletData.placementNoise ))/ bulletData.Count);
                        //UnityEngine.Debug.Log("At Location " + Vector2.Lerp(bulletData.startPos, bulletData.endPos, i / bulletData.Count));
                    }
                    m_currentTime = LevelManager.m_Instance.LevelTimer;
                    m_LastSpawnTime[attackIterator] = m_currentTime;
                }

                attackIterator++;
            }
        }

        
    }

    // This function will be used to get bullets from object pools

    
    private GameObject GetBullet(GameObject bulletType)
    {
        if (bulletType == null)
        {
            throw new System.ArgumentNullException();
        }

        GameObject bullet = null;

        // Game Object Pool might provide null object
        for (int i = 0; i < 5; i++)
        {
            bullet = GameObjectPool.m_instance.GetSelfManagedGameObject(bulletType);
            if (bullet != null) { break; }
        }

        return bullet;
        //return Instantiate(bulletType);
    }

    private IEnumerator ExhibitionCoroutine()
    {
        DisableShooting();
        yield return new WaitForSeconds(5);
        EnableShooting();
        yield break;
    }

    #endregion

    #region Bullets Spawner API
    // Spawner API 
    // If you want to generate bullets
    // The calling order is:
    // UpdateSpawnerConfig -> Enable shooting -> Disable Shooting

    public void UpdateSpawnerConfig(AttackPatternData config, bool isResetRotation = true)
    {
        // Validate null ref
        //var traceStack = new StackTrace();
        //var method = traceStack.GetFrame(0).GetMethod();
        //var caller = traceStack.GetFrame(1).GetMethod();

        //if (config == null)
        //    UnityEngine.Debug.LogError($"Trying to {method.Name} in {method.DeclaringType} with null, " +
        //        $"called from {caller.DeclaringType}.{caller.Name}");

        // Deliever config to config attributes in spawner
        m_Config = config;
        hasFromEnemyData = (0 != m_Config.enemyOriginBulletDatas.Length);
        hasFromLineData = (0 != m_Config.lineOriginBulletDatas.Length);
        hasFromAimedData = (0 != m_Config.enemyAimedBulletDatas.Length);

        if (playerTarget == null)
        {
            playerTarget = FindFirstObjectByType<PlayerController>().gameObject;
        }

        if (isResetRotation)
            UpdatePrivateVariables();
    }

    public void EnableShooting()
    {
        m_currentTime = LevelManager.m_Instance.LevelTimer;

        for (int i = 0; i < m_LastSpawnTime.Length; i++)
        {
            m_LastSpawnTime[i] = m_currentTime;
        }
        m_isAllowedToShoot = true;
    }

    public void DisableShooting()
    {
        m_isAllowedToShoot = false;
    }

    #endregion

    #region MonoBehavior

    private void Awake()
    {
        
        
    }

    private void Start()
    {
        //UpdateSpawnerConfig(m_Config);
        //EnableShooting();
    }

    private void FixedUpdate()
    {
        m_currentTime = LevelManager.m_Instance.LevelTimer;
        UpdateBulletSpawnerTransform();
        SpawnBullets();

    }

    #endregion
}
