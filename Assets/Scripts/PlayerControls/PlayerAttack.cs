using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public InputActionAsset playerActions;
    public LayerMask mask;

    public float meleeCoolDownInSeconds;
    private float nextAttackTime;

    private InputAction attack;
    private Melee melee;
    // Start is called before the first frame update
    void Start()
    {
        melee = gameObject.GetComponent<Melee>();
        melee.setLayerMask(mask);
    }
    private void OnEnable()
    {
        attack = playerActions.FindAction("Attack");

        attack.Enable();
        attack.performed += MeleeAttack;
    }
    private void OnDisable()
    {
        attack.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void MeleeAttack(InputAction.CallbackContext context)
    {
        Debug.Log("we hit");
        if (Time.time > nextAttackTime)
        {
            melee.Cast();
            nextAttackTime = Time.time + meleeCoolDownInSeconds;
        }
    }
}
