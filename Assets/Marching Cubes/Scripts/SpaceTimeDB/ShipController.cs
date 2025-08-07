using System;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class ShipController : EntityController
{

    private PlayerServerController Owner;
    
   
    
    public void Spawn(Ship playerShip, PlayerServerController owner)
    {
        base.Spawn(playerShip.EntityId);

        this.Owner = owner;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = owner.Username;
    }

    public override void OnDelete(EventContext context)
    {
        base.OnDelete(context);
        Owner.OnShipDeleted(this);
    }
    
    
}
