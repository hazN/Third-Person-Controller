using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.Combat
{
    [Serializable]
    public class Attack
    {
        [field: SerializeField] public string AnimationName { get; private set; }
        [field: SerializeField] public float TransitionDuration { get; private set; } = 0.1f;
        [field: SerializeField] public int ComboStateIndex { get; private set; } = -1;
        [field: SerializeField] public float ComboAttackTime { get; private set; } = 0.8f;
        [field: SerializeField] public float ForceTime { get; private set; } = 0.35f;
        [field: SerializeField] public float Force { get; private set; } = 2f;
        [field: SerializeField] public float Damage { get; private set; } = 10f;
        [field: SerializeField] public float Knockback { get; private set; } = 2f;
    }
}