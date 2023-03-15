using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Melee))]
[RequireComponent(typeof(Projectile))]

public class PlayerAttack : MonoBehaviour
{
    public InputActionAsset playerActions;
    public LayerMask mask;

    public float meleeCoolDownInSeconds;
    public float projectileCoolDownInSeconds;
    private float nextAttackTime;
    private float nextProjectileTime;

    public float projectileVelocity;
    public float projectileDistance;

    private InputAction attack;
    private Melee melee;

    private InputAction projectileInput;
    private Projectile projectileScript;

    // Start is called before the first frame update
    void Start()
    {
        melee = gameObject.GetComponent<Melee>();
        projectileScript = gameObject.GetComponent<Projectile>();

        //set up melee and projectile scripts
        melee.setLayerMask(mask);

        projectileScript.setLayerMask(mask);
        projectileScript.setVelocity(projectileVelocity);
        projectileScript.SetDespawnDistance(projectileDistance);
    }
    private void OnEnable()
    {
        //set up button keyboard attack events
        attack = playerActions.FindAction("Attack");

        attack.Enable();
        attack.performed += MeleeAttack;

        projectileInput = playerActions.FindAction("Projectile");
        projectileInput.Enable();
        projectileInput.performed += projectileAttack;
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
        if (Time.time > nextAttackTime)
        {
            Debug.Log("attacked");
            melee.Cast();
            nextAttackTime = Time.time + meleeCoolDownInSeconds;
        }
    }
    private void projectileAttack(InputAction.CallbackContext context)
    {
        if (Time.time > nextProjectileTime)
        {
            Debug.Log("shot");
            projectileScript.Cast();
            nextProjectileTime = Time.time + projectileCoolDownInSeconds;
        }
    }
}
