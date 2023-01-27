using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MovementUpdateReciever
{
    public void FlyingUpdate(MovementFlying movement);

    public void HoverUpdate(MovementHovering movement);
    
    public void WalkingUpdate(MovementWalking movement);
}
