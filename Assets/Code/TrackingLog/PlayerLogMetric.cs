using System.Collections.Generic;
using System.Text;
using USCG.Core.Telemetry;
using USCG.Core.Telemetry.Internal;

public class PlayerLogMetric : Metric
{
    private List<PlayerLog> m_logs = new();

    public void AddLog(PlayerLog log)
    {
        if (log == null) return;
        m_logs.Add(log);
    }

    public override void Reset()
    {
        m_logs.Clear();
    }

    public override string ToCsv(MetricId metricId)
    {
        // Add table name
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{metricId.metricName},");

        // Add table header
        stringBuilder.AppendLine("Record Time,Play Time,Is Win,Difficulty,Last Enemy Attack,Last Player Position");

        // Add table data
        foreach(PlayerLog log in m_logs)
        {
            string lastPlayerPosition = Sanitize(log.m_LastPlayerPosition.ToString(), ' ');
            string recordTime = Sanitize(log.m_RecordTime, ' ');
            string playTime = Sanitize(log.m_PlayTime.ToString(), ' ');
            string isWin = Sanitize(log.m_IsWin.ToString(), ' ');
            string difficulty = Sanitize(log.m_Difficulty, ' ');
            string lastEnemyAttack = Sanitize(log.m_LastEnemyAttack, ' ');
            stringBuilder.AppendLine($"{recordTime},{playTime},{isWin},{difficulty},{lastEnemyAttack},{lastPlayerPosition}");
        }

        return stringBuilder.ToString();
    }

}