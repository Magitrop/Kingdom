using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureVisualController : MonoBehaviour
{
    public Creature owner;

    public void Death()
    {
        owner.Death();
    }

    public void ReturnToCurrentAnimation()
    {
        owner.ReturnToCurrentAnimation();
    }

    public void AIController()
    {
        owner.AIController();
    }

    public void Destroy()
    {
        owner.Destroy();
    }
}