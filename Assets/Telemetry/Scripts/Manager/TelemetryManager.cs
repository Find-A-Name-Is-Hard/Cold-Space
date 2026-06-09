namespace USCG.Core.Telemetry
{
    using USCG.Core.Telemetry.Internal;

    using UnityEngine;

    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    public class TelemetryManager : MonoBehaviour
    {
        [Header("Telemetry Manager Settings")]

        [Tooltip("Whether or not metrics will be printed when the application quits.")]
        [SerializeField] private bool _printMetricsOnQuit = true;

        [Tooltip("Whether or not to print metrics while running in the editor.")]
        [SerializeField] private bool _printMetricsWhileInEditor = false;

        [SerializeField] private string m_tableCategory;

        [Tooltip("Pressing the combination of these keys will write all current metrics values to a file and reset them.")]
        [SerializeField]
        private List<KeyCode> _printAndResetKeyCodes = new()
        {
            //KeyCode.LeftCommand,
            KeyCode.LeftControl,
            KeyCode.LeftShift,
            KeyCode.M
        };

        private Dictionary<MetricId, object> _metrics = new Dictionary<MetricId, object>();
        private bool _bIsPrintingMetrics = false;

        protected virtual void Update()
        {
            if (ArePrintAndResetKeyCodesPressed())
            {
                PrintMetricsImmediate(true);
            }
            else
            {
                _bIsPrintingMetrics = false;
            }
        }

        protected virtual void OnApplicationQuit()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Do nothing in WebGL
            return;
#else
            // Save CSV file on other platform
            if (_printMetricsOnQuit)
            {
                PrintMetricsImmediate();
            }
#endif
        }

        #region Original Metric Part
        /// <summary>
        /// Creates a new accumulated metric with the given name. The returned value should be used with
        /// the AccumulateMetric method.
        /// </summary>
        /// <param name="inMetricName"></param>
        /// <returns></returns>
        public virtual MetricId CreateAccumulatedMetric(string inMetricName)
        {
            MetricId metricId = new MetricId(inMetricName);

            if (_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {inMetricName} was not created because a metric with that name already exists.");
                return default;
            }

            _metrics.Add(metricId, new AccumulatedMetric());
            return metricId;
        }

        /// <summary>
        /// Accumulates the specified metric with the given MetricId by the provided amount.
        /// </summary>
        /// <param name="metricId"></param>
        /// <param name="value"></param>
        public virtual void AccumulateMetric(MetricId metricId, float value)
        {
            if (!_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {metricId.metricName} does not exist!");
                return;
            }

            try
            {
                AccumulatedMetric accumulatedMetric = (AccumulatedMetric)_metrics[metricId];
                accumulatedMetric.Accumulate(value);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        /// <summary>
        /// Creates a new sampled metric with the given name. The returned value should be used with
        /// the AddMetricSample method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inMetricName"></param>
        /// <returns></returns>
        public virtual MetricId CreateSampledMetric<T>(string inMetricName)
        {
            MetricId metricId = new MetricId(inMetricName);

            if (_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {inMetricName} was not created because a metric with that name already exists.");
                return default;
            }

            _metrics.Add(metricId, new TSampledMetric<T>());
            return metricId;
        }

        /// <summary>
        /// Adds a sample for the specified metric with the given MetricId. The type of value must
        /// match the type used to create the original metric.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="metricId"></param>
        /// <param name="value"></param>
        public virtual void AddMetricSample<T>(MetricId metricId, T value)
        {
            if (!_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {metricId.metricName} does not exist!");
                return;
            }

            try
            {
                TSampledMetric<T> sampledMetric = (TSampledMetric<T>)_metrics[metricId];
                sampledMetric.AddSample(value);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        #endregion 

        // Player Log Metric
        public virtual MetricId CreatePlayerLogMetric(string inMetricName)
        {
            MetricId metricId = new MetricId(inMetricName);

            if (_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {inMetricName} was not created because a metric with that name already exists.");
                return default;
            }

            _metrics.Add(metricId, new PlayerLogMetric());
            return metricId;
        }

        public virtual void AddPlayerLogMetric(MetricId metricId, PlayerLog value)
        {
            if (!_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {metricId.metricName} does not exist!");
                return;
            }

            try
            {
                PlayerLogMetric playerLogMetric = (PlayerLogMetric)_metrics[metricId];
                playerLogMetric.AddLog(value);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        // Attack Pattern Log Metric
        public virtual MetricId CreateAtkPatternLogMetric(string inMetricName)
        {
            MetricId metricId = new MetricId(inMetricName);

            if (_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {inMetricName} was not created because a metric with that name already exists.");
                return default;
            }

            _metrics.Add(metricId, new AttackPatternLogMetric());
            return metricId;
        }

        public virtual void AddAtkPatternLogMetric(MetricId metricId, AttackPatternLog value)
        {
            if (!_metrics.ContainsKey(metricId))
            {
                Debug.LogWarning($"Metric {metricId.metricName} does not exist!");
                return;
            }

            try
            {
                AttackPatternLogMetric AttackPatternLogMetric = (AttackPatternLogMetric)_metrics[metricId];
                AttackPatternLogMetric.AddLog(value);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        // Prints all metrics to a file. The optional parameter can be used to reset all
        // current metrics, which will clear their values.
        #region backup
        //private void PrintMetricsImmediate(bool bResetMetrics = false)
        //{
        //    // Don't print anything if metrics are still printing. This gets reset in Update().
        //    if (_bIsPrintingMetrics)
        //    {
        //        return;
        //    }

        //    //Don't print anything while running in editor unless it's explicitly allowed.
        //    if (Application.isEditor && !_printMetricsWhileInEditor)
        //    {
        //        return;
        //    }

        //    // Always flip this value to be true. In Update() we check for whether or not the
        //    // keys have been released and turn this value false again. Without this safeguard,
        //    // users could "print" multiple files each frame while the keys are held down.
        //    _bIsPrintingMetrics = true;

        //    StringBuilder stringBuilder = new StringBuilder();
        //    foreach (KeyValuePair<MetricId, object> kvp in _metrics)
        //    {
        //        Metric metric = (Metric)kvp.Value;
        //        stringBuilder.AppendLine(metric.ToCsv(kvp.Key));
        //        if (bResetMetrics)
        //        {
        //            metric.Reset();
        //        }
        //    }

        //    // Sanitize the file name to make sure it doesn't contain bad characters.
        //    string dateTime = DateTime.Now.ToString();
        //    dateTime = dateTime.Replace("/", "_");
        //    dateTime = dateTime.Replace(":", "_");
        //    dateTime = dateTime.Replace(" ", "___");

        //    string filePath = $"{Application.persistentDataPath}/{Application.productName}-Metrics-{dateTime}.csv";
        //    File.WriteAllText(filePath, stringBuilder.ToString());
        //    Debug.Log($"Metrics written to {filePath}");
        //}
        #endregion

        protected virtual void PrintMetricsImmediate(bool bResetMetrics = false)
        {
            // Prevent repeated print
            if (_bIsPrintingMetrics)
                return;

            // Enable or not the print in editor
            if (Application.isEditor && !_printMetricsWhileInEditor)
                return;

            _bIsPrintingMetrics = true;

            // Construct CSV content
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<MetricId, object> kvp in _metrics)
            {
                Metric metric = (Metric)kvp.Value;
                string temp = metric.ToCsv(kvp.Key);
                if (temp != null && temp != "") { stringBuilder.AppendLine(temp); }                

                if (bResetMetrics)
                    metric.Reset();
            }

            // Generate file name
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"{Application.productName}-{m_tableCategory}-Metrics-{dateTime}.csv";
            string csvContent = stringBuilder.ToString();

#if UNITY_WEBGL && !UNITY_EDITOR
    // In the WebGL, we should use JavaScript to download the CSV file
    string safeContent = csvContent.Replace("`", "\\`"); // ±ÜÃâ·´ÒýºÅ³åÍ»
    Application.ExternalEval($@"
        var blob = new Blob([`{safeContent}`], {{ type: 'text/csv;charset=utf-8' }});
        var a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = '{fileName}';
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    ");
    Debug.Log($"[TelemetryManager] CSV download triggered in browser: {fileName}");
#else
            // Writting file in PC / Editor platform environment
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(filePath, csvContent);
            Debug.Log($"[TelemetryManager] Metrics written to: {filePath}");
#endif
        }

        // Helper method to check if the keys are pressed.
        protected virtual bool ArePrintAndResetKeyCodesPressed()
        {
            bool bArePrintAndResetKeyCodesPressed = _printAndResetKeyCodes.Count > 0;

            foreach (KeyCode key in _printAndResetKeyCodes)
            {
                bArePrintAndResetKeyCodesPressed &= Input.GetKey(key);
            }
            return bArePrintAndResetKeyCodesPressed;
        }

    }
}
