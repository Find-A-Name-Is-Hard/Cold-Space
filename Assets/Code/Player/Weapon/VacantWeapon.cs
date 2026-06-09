using UnityEngine;

public class VacantWeapon : WeaponController
{
    public override void HandleShootingEndInput()
    {
        base.HandleShootingEndInput();
        Debug.Log($"Stop from {gameObject.name}");
    }

    public override void HandleShootingStartInput()
    {
        base.HandleShootingStartInput();
        Debug.Log($"Shoot from {gameObject.name}");
    }
}
