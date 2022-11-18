using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a script of static functions to be used
//by various purposes
public class Utilities
{
    static public bool ObjectInLayer(GameObject gameObject, LayerMask mask)
    {
        //create the layer mask for the gameObject's layer, by shift 1 into the correct spot
        LayerMask objectLayer = 1 << gameObject.layer;

        //perform bitwise and to check in the object is in the mask
        //it will be nonzero if it is, and zero if it isn't
        return (objectLayer.value & mask.value) != 0; 
    }
}
