using UnityEngine;

public class CureAtkManager : MonoBehaviour
{
    public Vector3 m_SpawnLocation;

    private void Awake()
    {
        transform.position = m_SpawnLocation;
    }
}
