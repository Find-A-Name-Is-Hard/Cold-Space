using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    private Dictionary<Type, Delegate> m_eventMapping = new();

    public void Subscribe<T>(Action<T> callback) where T : EventBase
    {
        var type = typeof(T);

        if(m_eventMapping.ContainsKey(type))
        {
            m_eventMapping[type] = Delegate.Combine(m_eventMapping[type], callback);
        }
        else
        {
            m_eventMapping.Add(type, callback);
        }

#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        PrintEventBusLog($"\"{caller.DeclaringType}. {caller.Name}\" subscribes \"{type}\" event");
#endif
    }

    public void Unsubscribe<T>(Action<T> callBack) where T : EventBase
    {
        var type = typeof(T);

        if(m_eventMapping.ContainsKey(type))
        {
            var current = Delegate.Remove(m_eventMapping[type], callBack);
            if (current == null) { m_eventMapping.Remove(type); }
            else { m_eventMapping[type] = current; }

#if UNITY_EDITOR
            var stackTrace = new StackTrace();
            var caller = stackTrace.GetFrame(1).GetMethod();
            PrintEventBusLog($"\"{caller.DeclaringType}. {caller.Name}\" unsubscribes \"{type.Name}\" event");
#endif
        }
    }

    public void Publish<T>(T eventData) where T : EventBase
    {
        var type = typeof(T);

#if UNITY_EDITOR
        var stackTrace = new StackTrace();
        var caller = stackTrace.GetFrame(1).GetMethod();
        PrintEventBusLog($"\"{caller.DeclaringType}. {caller.Name}\" publishes \"{type.Name}\" event");
#endif

        if (m_eventMapping.TryGetValue(type, out var delegates))
        {
            // (action as Action<T>)?.Invoke(eventData);
            foreach(var delegateMethod in delegates.GetInvocationList())
            {
#if UNITY_EDITOR
                var method = delegateMethod.Method;

                if (method != null)
                {
                    PrintEventBusLog($"{method.DeclaringType}. {method.Name} gets event \"{type.Name}\" ");
                }
#endif

                (delegateMethod as Action<T>).Invoke(eventData);
            }
        }
    }

    private void PrintEventBusLog(string log)
    {
        UnityEngine.Debug.Log($"Event bus logging: {log}");
    }

}
