using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Spell
{
    private bool m_Started;
    private LayerMask layer;
    public void Start()
    {
        m_Started = true;
        if(effects.Count < 1)
        {
            InstantDamage insta = ScriptableObject.CreateInstance<InstantDamage>();
            insta.damage = 10;
            effects.Add(insta);
        }
        
    }
    public override void Cast()
    {
        //find objects hit in a box
        //used transform.forward to place box in front of gameobject
        Collider[] hits = Physics.OverlapBox(gameObject.transform.position + transform.forward *3, Vector3.one * 3, Quaternion.identity, layer);

        for(int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];
            GameObject entity = hit.gameObject;
            for(int j = 0; j < effects.Count; j++)
            {
                ApplySpellEffect(entity, effects[j]);
            }
        }
        //apply spell effects
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Debug.Log("draw box");
            Gizmos.DrawWireCube(transform.position + transform.forward * 3, Vector3.one * 3);
    }
    public void setLayerMask(LayerMask mask)
    {
        layer = mask;
    }
    private void ApplySpellEffect(GameObject entity, Spell_Effects effect)
    {
        effect.Process(entity);
    }
    
}
