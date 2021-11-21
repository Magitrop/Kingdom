using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWickedAltar : Destructible
{
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        if (target == null)
        {
            //Creature _target = GetPlayerTarget();
            Creature _target = map.player;
            if (_target != null)
                target = _target;
        }

        if (target != null)
        {
            BuildPath(target.x, target.y);
        }
        else
        {
            if (turnIsCompleted == false)
                Wait();
            return;
        }

        if (currentPath == null || currentPath.Length == 0 || currentPath.Length > GetStateValue("view_dist"))
        {
            if (turnIsCompleted == false)
                Wait();
            return;
        }

        AIController();
    }

    public override void CorrelateLevelWithStats() { }

    public override void AIController()
    {
        base.AIController();

        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(2), target, this, 1, Mathf.Abs(target.GetStateValue("level") + 2));
        Effect.ApplyEffect(EffectsDatabase.GetEffectByID(4), target, this, 1, 3);

        if (turnIsCompleted == false)
        {
            Wait();
            return;
        }
    }

    public override void Death()
    {
        base.Death();
        animator.Play("wicked_altar_death");
        SetStateValue("cur_health", 0);
        RefreshHealthbar();
        isAlive = false;
        if (turnIsCompleted == false)
            Wait();
    }

    public override void PlayIdleAnimation() { }
    public override void PlayWalkAnimation() { }
    public override void PlayAttackAnimation() { }
    public override void PlayDeathAnimation() { }
    public override void PlaySpawnAnimation() { }

    public override void GetDamage(int _damage, DamageType damageType, Creature sender)
    {
        if (canBeDestroyed == true && isAlive == true) // important condition
        {
            _damage = 1;
            int damageReduction = OnDamage(sender, damageType, _damage);
            if (damageReduction >= _damage)
                return;
            _damage -= damageReduction;
            Death();
        }
    }

    public override void GetHealing(int _healing) { }

    public override int OnDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        int result = base.OnDamage(_sender, _damageType, _damage);
        PlaySound("get_damage");
        return result;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        PlaySound("death");
    }

    public override void Destroy()
    {
        base.Destroy();
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return creatureNameID.ToString() + ";"
                + x + ";"
                + y + ";"
                + isAlive.ToString() + ";"
                + isSummonedByPlayer.ToString();
    }

    public override void FromString(string str)
    {
        string[] s = str.Split(';');
        creatureName = s[0];
        x = int.Parse(s[1]);
        y = int.Parse(s[2]);
        //SetStateValue("cur_health", int.Parse(s[3]));
        isAlive = bool.Parse(s[3]);
        isSummonedByPlayer = bool.Parse(s[4]);
        transform.position = new Vector3(x, y, 0);

        //RecalculateStates();

        //isAlive = GetStateValue("cur_health") > 0;
        if (isAlive == false)
            Death();
        RefreshHealthbar();
    }
}