using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitching : MonoBehaviour
{
    [SerializeField] private string m_nextLevel; 
    public void ChangeLevel()
    {
        SceneManager.LoadScene(m_nextLevel);
    }
}
