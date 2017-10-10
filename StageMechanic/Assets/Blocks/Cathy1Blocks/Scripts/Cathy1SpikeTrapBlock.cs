﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a normal Cath1-style spike trap. Upon the player or enemy stepping on this
/// block it will trigger after the specified trigger time, allowing the player or enemy
/// a certain amount of time to move before the trap enters the IsTriggerd state before
/// moving into the disarned state.
/// </summary>
public class Cathy1SpikeTrapBlock : Cathy1AbstractTrapBlock
{
    /// <summary>
    /// Indicate to the Cathy1 game rules that this is a spike trap
    /// </summary>
    public sealed override TrapBlockType TrapType
    {
        get
        {
            return TrapBlockType.Spike;
        }
    }

    /// <summary>
    /// Returns the capsule collider associated with the block
    /// </summary>
    public override sealed Collider PlayerTriggerCollider
    {
        get
        {
            return GetComponent<CapsuleCollider>();
        }
        set { }
    }

    /// <summary>
    /// Items cannot trigger spike traps in Cathy1 style
    /// </summary>
    public override sealed Collider ItemTriggerCollider
    {
        get
        {
            return null;
        }
        set { }
    }

    /// <summary>
    /// Blocks cannot trigget spike traps in Cathy1 style
    /// </summary>
    public override sealed Collider BlockTriggerCollider
    {
        get
        {
            return null;
        }
        set { }
    }

    /// <summary>
    /// Add the spike trap specific properties.
    /// </summary>
    public override Dictionary<string, string> Properties
    {
        get
        {
            //TODO
            return base.Properties;
        }

        set
        {
            //TODO
            base.Properties = value;
        }
    }

    /// <summary>
    /// Sets the trigger time of the spike trap
    /// </summary>
    public virtual void Start()
    {
        TriggerTime = 1000;

    }
}
