using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsoluteWindManager : MonoBehaviour
{
    [Tooltip("Layer mask for the wind tunnels")]
    [SerializeField] private LayerMask windTunnelMask;

    private List<WindTunnel> windTunnels = new List<WindTunnel>(); //A list of all wind tunnels the player is currently in
    public Vector3 absoluteWind //The absolute wind the player feels
    {
        private set;
        get;
    }

    // Update is called once per frame
    void Update()
    {
        //Compute absoluteWind by resetting it to zero and adding to it for each wind tunnel
        absoluteWind = Vector3.zero;
        for (int tunnelIndex = 0; tunnelIndex < windTunnels.Count; tunnelIndex++)
        {
            absoluteWind += windTunnels[tunnelIndex].getWindValue();
        }
        Debug.Log($"absolute wind {absoluteWind}");
    }

    private void OnTriggerEnter(Collider triggered)
    {
        if (Utilities.ObjectInLayer(triggered.gameObject, windTunnelMask))
        {
            //entered a wind tunnel, add its wind to the absolute wind
            windTunnels.Add(triggered.GetComponent<WindTunnel>());
        }
    }

        private void OnTriggerExit(Collider triggered)
    {
        if (Utilities.ObjectInLayer(triggered.gameObject, windTunnelMask))
        {
            //exited a wind tunnel, subtract its wind to the absolute wind
            windTunnels.Remove(triggered.GetComponent<WindTunnel>());
        }
    }
}
