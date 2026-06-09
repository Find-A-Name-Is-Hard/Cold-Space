using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyConversionData", menuName = "Scriptable Objects/DifficultyConversionData")]
public class DifficultyConversionData : ScriptableObject
{
    [Tooltip("Starting Difficulty float changes based on menu choice. Easy Difficult is x=0 on curves.")]
    public float InitDifficultyEasy = 0;
    public float InitDifficultyMedium = 10;
    public float InitDifficultyHard = 20;
    [Tooltip("Max Difficulty is translated to x=1 on the curves")]
    public float MaxDifficulty = 30;
    [Tooltip("Increase difficulty per minute played")]
    public float DifficultyPerMinutePlayed = 1;
    [Tooltip("Increase difficulty when test attack start")]
    public float DifficultyUpPerTestAttack = 4;
    [Tooltip("Decrease difficulty when test attack over")]
    public float DifficultyDownAfterTestAttack = 2;
    [Tooltip("Increase difficulty when cure attack start")]
    public float DifficultyUpPerCureAttack = 6;
    [Tooltip("Decrease difficulty when cure attack finished")]
    public float DifficultyDownAfterCureAttack = 3;

    [Tooltip("Multiplicatively adjusts bullet spawn interval from x=0 to x=1")]
    public AnimationCurve SpawnIntervalConversion = AnimationCurve.Constant(0f,1f,1f);
    [Tooltip("Multiplicatively adjusts bullet count from x=0 to x=1")]
    public AnimationCurve CountConversion = AnimationCurve.Constant(0f, 1f, 1f);
    [Tooltip("Multiplicatively adjusts rotational angular acceleration from x=0 to x=1")]
    public AnimationCurve RotationAngularAccelerationConversion = AnimationCurve.Constant(0f, 1f, 1f);
    [Tooltip("Multiplicatively adjusts rotational angular max velocity from x=0 to x=1")]
    public AnimationCurve RotationAngularVelocityMaxConversion = AnimationCurve.Constant(0f, 1f, 1f);
    [Tooltip("Multiplicatively adjusts AngleIterval from x=0 to x=1")]
    public AnimationCurve AngleIntervalConversion = AnimationCurve.Constant(0f, 1f, 1f);

}
