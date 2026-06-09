using UnityEngine;

public class PlayerLoggingManager : USCG.Core.Telemetry.TelemetryManager
{
    /// <summary>
    /// Access the singleton instance of the TelemetryManager.
    /// </summary>
    public static PlayerLoggingManager instance { get => _instance; }
    private static PlayerLoggingManager _instance = default;

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
