using NUnit.Framework;
using System.Reflection;
using UnityEngine;
using USCG.Core.Telemetry;
using USCG.Core.Telemetry.Internal;
using System.Collections.Generic;

public class PlayerLog 
{
    public string m_RecordTime;
    public float m_PlayTime;
    public bool m_IsWin;
    public string m_Difficulty;
    public string m_LastEnemyAttack;
    public Vector3 m_LastPlayerPosition;
}
