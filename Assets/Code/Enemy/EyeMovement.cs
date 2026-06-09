using UnityEngine;
using UnityEditor;
using Unity.Collections;
using Unity.VisualScripting;

public class EyeMovement : MonoBehaviour
{


    public float xRadius;
    public float yRadius;
    public int DebugSegments = 25;
    public float maxDistance = 4.5f;
    public Vector2 startingPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition=this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector2.Distance(this.transform.position, PlayerController.Instance.gameObject.transform.position);
        Debug.Log ("eye distance from player : " + distance);
        Vector2 heading = this.transform.position - PlayerController.Instance.gameObject.transform.position;
        Vector2 directionUnitVector = heading.normalized;
        Debug.Log("Direction Unit Vector: " + directionUnitVector);
        this.transform.localPosition = startingPosition - ((directionUnitVector*Mathf.Lerp(0,maxDistance,distance))*new Vector2(xRadius,yRadius)*.1f); 


    }

}
