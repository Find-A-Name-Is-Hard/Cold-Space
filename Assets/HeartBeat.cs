using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeartBeat : MonoBehaviour
{
    public Sprite[] heartSprites;
    private PlayerModel m_playerModel;
    public float slowFps;
    public float fastFps;

    public Image m_image;
    private float m_resetTime;
    private float m_timeTillNextSprite;
    private int m_numberOfFrames;
    private int m_currentFrame = 0;
    private bool m_goingUp = true;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TryGetPlayerModel());
        m_image = GetComponent<Image>();
        m_numberOfFrames = heartSprites.Length;
        m_timeTillNextSprite = 1f/slowFps;
    }

    // Update is called once per frame
    void Update()
    {
        m_resetTime-=Time.deltaTime;
        if (m_resetTime<=0)
        {
            m_resetTime = m_timeTillNextSprite;
            if (m_currentFrame==0)
            {
                m_goingUp = true;
            }
            else if (m_currentFrame == m_numberOfFrames - 1)
            {
                m_goingUp = false;
            }
            m_currentFrame += m_goingUp ? 1 : -1;
            m_image.sprite = heartSprites[m_currentFrame];
        }
    }



    private IEnumerator TryGetPlayerModel()
    {
        // �ȴ� LevelManager ��ʼ��
        while (LevelManager.m_Instance == null)
            yield return null;

        // �ȴ� CurrentPlayer ׼����
        while (LevelManager.m_Instance.CurrentPlayer == null)
            yield return null;

        // �ȴ� PlayerModel ���
        while (m_playerModel == null)
        {
            LevelManager.m_Instance.CurrentPlayer.TryGetComponent<PlayerModel>(out m_playerModel);
            yield return null;
        }

        m_playerModel.OnHealthUpdate += UpdateValue;
    }

    private void UpdateValue(int value,int oldValue, int maxEN)
    {
        float maxHP = m_playerModel.m_currentAttributes.playerAttributes.MaxHP;
        m_timeTillNextSprite = 1f/UnityEngine.Mathf.Lerp(fastFps,slowFps,value/maxHP);
        
        print("UpdateValueInHeartDisplay");
    
    
    }

}
