using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveSequence", menuName = "SaveSequence")]

public class SaveSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessSave(this);
    }

    //data
    public SaveScrtiptableObject saveData;
}
