using NUnit.Framework;
using UnityEngine;
using System;

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "Scriptable Objects/PlayerAttributes")]
public class PlayerAttributes : ScriptableObject
{
    public SPlayerAttributes playerAttributes;

    public GameObject DefaultWeapon;
    public GameObject FireWeapon;
    public GameObject MegneticWeapon;
    public GameObject LightWeapon;
    public GameObject ProjectileWeapon;

    public System.Collections.Generic.List<GameObject> CureAtkWeapon;

    public PlayerAttributes ValueClone()
    {
        PlayerAttributes clone = ScriptableObject.CreateInstance<PlayerAttributes>();
        clone.playerAttributes = this.playerAttributes;
        clone.DefaultWeapon = this.DefaultWeapon;
        clone.FireWeapon = this.FireWeapon;
        clone.MegneticWeapon = this.MegneticWeapon;
        clone.LightWeapon = this.LightWeapon;
        clone.ProjectileWeapon = this.ProjectileWeapon;
        clone.CureAtkWeapon = this.CureAtkWeapon;
        return clone;
    }
}

