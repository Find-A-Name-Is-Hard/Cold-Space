using UnityEngine;

public class AttackPatternLogManager : USCG.Core.Telemetry.TelemetryManager
{
    /// <summary>
    /// Access the singleton instance of the TelemetryManager.
    /// </summary>
    public static AttackPatternLogManager instance { get => _instance; }
    private static AttackPatternLogManager _instance = default;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }
    protected override void Update()
    {
        base.Update();
    }
}
