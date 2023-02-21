using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell_Effects", menuName = "Spell_Effects")]
public abstract class Spell_Effects : ScriptableObject
{
    public abstract void Process(GameObject entity);
}
