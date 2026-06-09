using UnityEngine;

public class CurrentButtonPressed : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectAndKeyCode
    {
        public GameObject obj;
        public KeyCode key;
    }

    public ObjectAndKeyCode[] objKeyList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (ObjectAndKeyCode objkey in objKeyList)
        {
            objkey.obj.SetActive(Input.GetKey(objkey.key));
        }
    }

}
