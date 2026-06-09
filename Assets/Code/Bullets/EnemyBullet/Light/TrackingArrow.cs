using System.Collections;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackingArrow : BulletsBehavior
{
    private GameObject m_player;
    [Header("Tracking Config")]
    public float m_MaxRotationPerSec = 90;
    public float m_TrackingTime = 2; 
    private IEnumerator AimingPlayer()
    {
        while(m_player == null)
        {
            if(LevelManager.m_Instance != null && LevelManager.m_Instance.CurrentPlayer != null)
            {
                m_player = LevelManager.m_Instance.CurrentPlayer;
            }
            yield return null;
        }

        float timer = 0;
        Vector3 targetDirection = Vector3.zero;
        Vector3 expectedRotation = Vector3.zero;
        Quaternion expectedQuaternion = Quaternion.identity;
        Quaternion targetQuaternion = Quaternion.identity;

        float angle = 0;
        
        while (timer < m_TrackingTime)
        {
            targetDirection = m_player.transform.position - transform.position;
            angle = -Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
            expectedRotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle);
            expectedQuaternion = Quaternion.Euler(expectedRotation);
            targetQuaternion = Quaternion.RotateTowards(transform.rotation, expectedQuaternion, m_MaxRotationPerSec * Time.deltaTime);


            transform.rotation = targetQuaternion;

            timer += Time.deltaTime;
            yield return null;
        }      
        
        yield break;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(AimingPlayer());
    }
}
