using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ComplexCharaHPBar : MonoBehaviour
{
    private PlayerModel m_playerModel;
    public RectTransform obscuringObject;
    public float maxPosX;
    public float minPosX;
    private float travelDistanceX;
    public RectTransform obscuredObject;
    private Vector3 obscuredInitLocation;
    private Quaternion obscuredInitRotation;
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

        m_playerModel.OnHealthUpdate += UpdateValue;
    }


    private void UpdateValue(int value,int oldValue, int maxEN)
    {
        float maxHP = m_playerModel.m_currentAttributes.playerAttributes.MaxHP;
        print("Update value in complex health bar");
        obscuringObject.localPosition = new Vector3(Mathf.Lerp(minPosX, maxPosX, (float)value / maxHP), obscuringObject.localPosition.y, obscuringObject.localPosition.z);

        obscuredObject.localPosition = new Vector3(obscuredInitLocation.x + (1 * Mathf.Lerp(0, travelDistanceX, (float)value / maxEN)), obscuredInitLocation.y, obscuredInitLocation.z);
        //obscuredObject.SetPositionAndRotation(obscuredInitLocation, obscuredInitRotation);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_hpBar = GetComponent<UnityEngine.UI.Slider>();
        StartCoroutine(TryGetPlayerModel());

        Vector3 obscuredInitGlobal = obscuringObject.transform.position;
        Quaternion obscuredInitRotation = obscuringObject.transform.rotation;

        obscuringObject.localPosition = new Vector3(minPosX, obscuringObject.localPosition.y, obscuringObject.localPosition.z);

        obscuredObject.SetPositionAndRotation(obscuredInitGlobal, obscuredInitRotation);
        obscuredInitLocation = obscuredObject.localPosition;
        travelDistanceX = Mathf.Abs(minPosX - maxPosX);
        obscuringObject.localPosition = new Vector3(maxPosX, obscuringObject.localPosition.y, obscuringObject.localPosition.z);
        obscuredObject.localPosition = new Vector3(obscuredInitLocation.x + travelDistanceX, obscuredInitLocation.y, obscuredInitLocation.z);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
