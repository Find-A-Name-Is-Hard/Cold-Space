using UnityEngine;
using System;
using System.Collections;

public class ComplexENBar : MonoBehaviour
{
    //private UnityEngine.UI.Slider m_hpBar;
    private PlayerModel m_playerModel;
    public RectTransform obscuringObject;
    public float maxPosX;
    public float minPosX;
    private float travelDistanceX;
    public RectTransform obscuredObject;
    private Vector3 obscuredInitLocation;
    //private Quaternion obscuredInitRotation;
    public GameObject ENNotFull;
    public GameObject ENFull;

    private IEnumerator TryGetPlayerModel()
    {
        // 等待 LevelManager 初始化
        while (LevelManager.m_Instance == null)
            yield return null;

        // 等待 CurrentPlayer 准备好
        while (LevelManager.m_Instance.CurrentPlayer == null)
            yield return null;

        // 等待 PlayerModel 组件
        while (m_playerModel == null)
        {
            LevelManager.m_Instance.CurrentPlayer.TryGetComponent<PlayerModel>(out m_playerModel);
            yield return null;
        }

        m_playerModel.OnEnergyUpdate += UpdateValue;
    }


    private void UpdateValue(int value, int oldValue, int MaxHP)
    {
        float maxEN = m_playerModel.m_currentAttributes.playerAttributes.MaxEnergy;
        Vector3 origPosition = obscuringObject.localPosition;
        obscuringObject.localPosition = new Vector3(Mathf.Lerp(minPosX, maxPosX, (float)value / maxEN), obscuringObject.localPosition.y, obscuringObject.localPosition.z);
        

        obscuredObject.localPosition = new Vector3(obscuredInitLocation.x + (1*Mathf.Lerp(0, travelDistanceX, (float)value / maxEN)), obscuredInitLocation.y, obscuredInitLocation.z);

        bool enFull = (value == maxEN);
        ENNotFull.SetActive(!enFull);
        ENFull.SetActive(enFull);
        
        //obscuredObject.SetPositionAndRotation(obscuredInitLocation, obscuredInitRotation);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TryGetPlayerModel());
        
        //obscuredInitRotation = obscuredObject.transform.rotation;
        Vector3 obscuredInitGlobal = obscuringObject.transform.position;
        Quaternion obscuredInitRotation = obscuringObject.transform.rotation;

        obscuringObject.localPosition = new Vector3(minPosX, obscuringObject.localPosition.y, obscuringObject.localPosition.z);

        obscuredObject.SetPositionAndRotation(obscuredInitGlobal, obscuredInitRotation);
        obscuredInitLocation = obscuredObject.localPosition;
        travelDistanceX = Mathf.Abs(minPosX - maxPosX);
        ENNotFull.SetActive(true);
        ENFull.SetActive(false);

        //obscuredObject.localPosition = new Vector3(obscuredInitLocation.x - minPosX, obscuredInitLocation.y, obscuredInitLocation.z);

        //obscuredObject.SetPositionAndRotation(obscuredInitLocation, obscuredInitRotation);
    }
}
