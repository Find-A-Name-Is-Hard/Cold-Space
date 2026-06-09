using System.Collections.Generic;

public class AttackPatternLog
{
    public string m_RecordTime;
    public string m_LogName;
    public string m_Damage;
    public string m_ENCharged;
    public string m_Difficulty;
    public string m_IsCauseDeath = "false";
    public List<AttackPatternTrackingData> m_TrackingDatas = new();
}
