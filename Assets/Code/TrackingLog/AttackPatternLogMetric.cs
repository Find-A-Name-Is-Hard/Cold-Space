using System.Collections.Generic;
using System.Linq;
using System.Text;
using USCG.Core.Telemetry;
using USCG.Core.Telemetry.Internal;
using UnityEngine;
using Unity.Properties;

public class AttackPatternLogMetric : Metric
{
    public List<AttackPatternLog> m_List = new();

    public void AddLog(AttackPatternLog log)
    {
        if (log == null) return;
        m_List.Add(log);
    }

    public override void Reset()
    {
        m_List.Clear();
    }

    public override string ToCsv(MetricId metricId)
    {
        // Don't print anything if the list is empty
        if(m_List == null)
        {
            return null;
        }

        // Add table name
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{metricId.metricName},");

        // Add table header
        if (m_List == null || m_List.Count == 0)
        {
            stringBuilder.AppendLine("Error: Attack pattern log list is empty!");
            return stringBuilder.ToString();
        }

        stringBuilder.Append("Record Time,Damage,ENCharged,Difficulty,Is Cause Death,");

        foreach(var data in m_List[0].m_TrackingDatas)
        {
            int count = data.GetType().GetFields().Length;
            stringBuilder.Append($"{data.m_BulletDataName}");
            for(int i =0;  i < count; i++)
            {
                stringBuilder.Append(',');
            }
        }
        stringBuilder.AppendLine();

        // Add table subheader
        stringBuilder.Append(",,,,");

        foreach (var data in m_List[0].m_TrackingDatas)
        {
            string name;
            var fieldInfo = data.GetType().GetFields().OrderBy(f => f.MetadataToken);
            foreach (var field in fieldInfo)
            {
                name = Sanitize(field.Name, ' ').Replace("m_", "");
                stringBuilder.Append($",{name}");
            }            
        }
        stringBuilder.AppendLine();

        // Add data
        foreach(var log in m_List)
        {
            // Add simple data
            stringBuilder.Append($"{Sanitize(log.m_RecordTime, ' ')},{Sanitize(log.m_Damage, ' ')}," +
                $"{Sanitize(log.m_ENCharged, ' ')},{Sanitize(log.m_Difficulty, ' ')},{Sanitize(log.m_IsCauseDeath, ' ')}");

            // Add data in tracking data list
            foreach(var data in log.m_TrackingDatas)
            {
                var fieldInfo = data.GetType().GetFields().OrderBy(f => f.MetadataToken);
                foreach (var field in fieldInfo)
                {
                    stringBuilder.Append($",{Sanitize(field.GetValue(data).ToString(), ' ')}");
                }
            }

            // Go to next row
            stringBuilder.AppendLine();
        }
        

        return stringBuilder.ToString();
    }
}
