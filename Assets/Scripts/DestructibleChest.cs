using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PossibleLoot
{
    public int lootID;
    public float lootRate;
}

public class DestructibleChest : Destructible
{
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        turnIsCompleted = true;
        Wait();
        return;
    }

    public override void Death()
    {
        base.Death();
        animator.Play("destroyed");
        SetStateValue("cur_health", 0);
        RefreshHealthbar();
        //isAlive = false;
        if (turnIsCompleted == false)
            Wait();
        Destroy(gameObject);
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
            SetStateValue("cur_health", (int)Mathf.MoveTowards(GetStateValue("cur_health"), 0, _damage));
            animator.SetInteger("hits", GetStateValue("max_health") - GetStateValue("cur_health"));
            RefreshHealthbar();
            if (GetStateValue("cur_health") < GetStateValue("max_health"))
                canBeLootedWhileAlive = false;
            if (GetStateValue("cur_health") <= 0)
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

    public override void CorrelateLevelWithStats() { }

    public override void Initialize()
    {
        base.Initialize();
        if (animator != null)
            animator.SetInteger("hits", GetStateValue("max_health") - GetStateValue("cur_health"));
        if (GetStateValue("cur_health") < GetStateValue("max_health"))
            canBeLootedWhileAlive = false;
        if (lootIsInitialized == false)
        {
            for (int i = 0; i < possibleLoots.Count; i++)
                if (Random.Range(0f, 100f) < possibleLoots[i].lootRate)
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(possibleLoots[i].lootID));
            lootIsInitialized = true;
        }
    }

    public override string ToString()
    {
        return creatureNameID.ToString() + ";"
                + x + ";"
                + y + ";"
                //+ GetStateValue("cur_health").ToString() + ";"
                + isAlive.ToString() + ";"
                + lootIsInitialized.ToString() + ";"
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
        transform.position = new Vector3(x, y, 0);
        lootIsInitialized = bool.Parse(s[4]);
        isSummonedByPlayer = bool.Parse(s[5]);

        //RecalculateStates();

        //isAlive = GetStateValue("cur_health") > 0;
        if (isAlive == false)
            Death();
        RefreshHealthbar();
    }
}