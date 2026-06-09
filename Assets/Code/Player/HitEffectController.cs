using UnityEngine;

public class HitEffectController : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.6f); 
    }
}
