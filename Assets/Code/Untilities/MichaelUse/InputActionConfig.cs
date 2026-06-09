using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionConfig", menuName = "Scriptable Objects/InputActionConfig")]
public class InputActionConfig : ScriptableObject
{
    [System.Serializable]
    public class InputEventMapping
    {
        public InputActionReference inputAction;
        public InputPhase phase = InputPhase.Performed;

        [SerializeReference]
        public AbstractEvent eventToDispatch;

        [Tooltip("如果为空，则立即分发事件；否则在指定延迟后分发")]
        public float dispatchDelay = 0f;
    }

    public List<InputEventMapping> mappings = new List<InputEventMapping>();
}

public enum InputPhase
{
    Started,
    Performed,
    Canceled
}


