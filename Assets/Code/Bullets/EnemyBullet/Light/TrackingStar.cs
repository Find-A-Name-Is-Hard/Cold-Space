using System.Collections;
using UnityEngine;

public class TrackingStar : MonoBehaviour
{
    private Coroutine m_MainCoroutine;
    private GameObject m_player;

    private void OnEnable()
    {
        StartCoroutine(MainCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Start()
    {
        
    }

    private IEnumerator MainCoroutine()
    {
        while(m_player == null)
        {
            if(LevelManager.m_Instance != null && LevelManager.m_Instance.CurrentPlayer != null)
            {
                m_player = LevelManager.m_Instance.CurrentPlayer;
                break;
            }
            yield return null;
        }

        float currentPositionX, currentPositionY;
        float leftLevelBoundary = LevelManager.m_Instance.LeftLevelBoundary,
            rightLevelBoundary = LevelManager.m_Instance.RightLevelBoundary,
            upLevelBoundary = LevelManager.m_Instance.UpperLevelBoundary,
            bottomLevelBoundary = LevelManager.m_Instance.LowerLevelBoundary;


        while (true)
        {
            currentPositionX = transform.position.x;
            currentPositionY = transform.position.y;
            if (currentPositionX < leftLevelBoundary
                || currentPositionX > rightLevelBoundary
                || currentPositionY < bottomLevelBoundary
                || currentPositionY > upLevelBoundary)
            {
                Vector3 direction = (m_player.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

                transform.rotation = Quaternion.Euler(0, 0 ,transform.rotation.z + angle);
            }
            yield return null;
        }
    }
}
