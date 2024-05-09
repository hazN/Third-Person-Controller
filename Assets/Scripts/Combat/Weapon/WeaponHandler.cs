using System.Collections;
using System.Collections.Generic;
using TPC.Combat;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private Collider weaponLogic;
    [SerializeField] private WeaponDamage weaponDamage;

    public void EnableWeapon()
    {
        weaponLogic.enabled = true;
        weaponDamage.enabled = true;
    }

    public void DisableWeapon()
    {
        weaponDamage.enabled = false;
        weaponLogic.enabled = false;
    }
}
