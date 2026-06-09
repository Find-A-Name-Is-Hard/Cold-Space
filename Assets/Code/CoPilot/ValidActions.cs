using UnityEngine;

[System.Serializable]
public enum validActions
{
    OnTestAtkStart = 0,
    OnTestFinish = 1,
    OnCureHappen = 2,
    OnPlayerSpawn = 3,
    OnPlayerMovementUpdate = 4,
    OnPlayerPositionUpdate = 5,
    OnPlayerHealthUpdate = 6,
    OnPlayerEnergyUpdate = 7,
    OnPlayerWeaponChange = 8,
    OnPlayerTestWeaponChange = 9,
    OnPlayerSlowDownStateChange = 10,
    OnPlayerFireStateUpdate = 11,
    OnPlayerHitBoss = 12,
    OnTimeSinceGameStart = 13,
    OnMakeDeduction = 14,
}