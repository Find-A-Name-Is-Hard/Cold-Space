using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EyeMovement))]
public class DebugEllipse : Editor
{
    
    void OnSceneGUI() {

        EyeMovement ey = target as EyeMovement;
        DrawCircle(ey.gameObject.transform.position.x, ey.gameObject.transform.position.y, ey.xRadius, ey.yRadius, ey.DebugSegments, 0f);

        
    }

    void DrawCircle(float x, float y, float xradius, float yradius, int segments, float angle) {

        float xo = 0;
        float yo = 0;
        float xn = 0;
        float yn = 0;
        for (int i = 0; i < (segments + 1); i++) {
            xn = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            yn = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;                
            if (i > 0)
            {
                Handles.DrawLine(new Vector2(x + xn, y + yn), new Vector2(x + xo, y + yo));
            }
            xo = xn;
            yo = yn;
            angle += (360f / segments);
        }
    }
}
