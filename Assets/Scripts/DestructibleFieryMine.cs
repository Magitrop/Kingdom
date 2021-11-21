using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleFieryMine : Destructible
{
    public override void OnTurnStart()
    {
        base.OnTurnStart();

        target = null;
        //Creature _target = GetPlayerTarget();
        Creature _target = null;
        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if (_x < 0 || _x >= map.mapSizeX || _y < 0 || _y >= map.mapSizeY || (_x == x && _y == y))
                    continue;
                if (map.cells[_x, _y].cellObject != null && map.cells[_x, _y].cellObject.GetComponent<Creature>() != null && map.cells[_x, _y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false && map.cells[_x, _y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                    _target = map.cells[_x, _y].cellObject.GetComponent<Creature>();
            }
        }
        if (_target != null)
            target = _target;

        if (target == null)
        {
            if (turnIsCompleted == false)
                Wait();
            return;
        }

        /*
        if (currentPath == null || currentPath.Length == 0 || currentPath.Length > GetStateValue("view_dist"))
        {
            if (turnIsCompleted == false)
                Wait();
            return;
        }
        */

        AIController();
    }

    public override void CorrelateLevelWithStats() { }

    public override void AIController()
    {
        base.AIController();

        StartCoroutine(Explosion());

        if (turnIsCompleted == false)
        {
            Wait();
            return;
        }
    }

    public IEnumerator Explosion()
    {
        animator.Play("fiery_mine_explosion");
        yield return new WaitForSeconds(0.33f);
        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if (_x < 0 || _x >= map.mapSizeX || _y < 0 || _y >= map.mapSizeY || (_x == x && _y == y))
                    continue;
                if (map.cells[_x, _y].cellObject != null && map.cells[_x, _y].cellObject.GetComponent<Creature>() != null && map.cells[_x, _y].cellObject.GetComponent<PlayerController>() == null && map.cells[_x, _y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    map.cells[_x, _y].cellObject.GetComponent<Creature>().GetDamage(GetStateValue("attack_damage"), DamageType.scalding, this);
            }
        }
        yield return new WaitForSeconds(0.1f);
        Death();
    }

    public override void Death()
    {
        base.Death();
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
            animator.Play("destroyed");
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