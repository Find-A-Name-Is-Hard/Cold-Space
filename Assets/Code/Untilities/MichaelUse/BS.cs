using UnityEngine;

public class BS : MonoBehaviour
{
    public Transform p0; // 起点
    public Transform p1; // 控制点1
    public Transform p2; // 控制点2
    public Transform p3; // 终点
    public int segmentCount = 50; // 线段数量

    private LineRenderer line;

    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = segmentCount + 1;
        line.widthMultiplier = 0.05f;
    }

    void Update()
    {
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 pos = CalculateCubicBezierPoint(t, p0.position, p1.position, p2.position, p3.position);
            line.SetPosition(i, pos);
        }
    }

    // 计算三次贝塞尔曲线上的点
    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        return uuu * p0 +
               3 * uu * t * p1 +
               3 * u * tt * p2 +
               ttt * p3;
    }
}
