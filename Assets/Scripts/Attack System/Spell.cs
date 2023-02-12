using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public List<Spell_Effects> effects;
    public abstract void Cast();
}
