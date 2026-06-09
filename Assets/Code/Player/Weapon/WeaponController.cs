using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    public HardClues m_WeaponType;

    protected bool m_isAbleToShoot = false;

    public virtual void HandleShootingStartInput() { m_isAbleToShoot = true; }
    public virtual void HandleShootingEndInput() { m_isAbleToShoot= false; }

    public virtual void HandleWeaponChangeEvent() { m_isAbleToShoot = false;}

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

}
