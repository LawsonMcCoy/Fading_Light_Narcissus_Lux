using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell_Effects : ScriptableObject
{
    public abstract void Process(GameObject entity);
}
