using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSkeletonWarrior : CreatureEnemy
{
    public int circularAttackCharges;

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        if (isAlive == false)
        {
            if (turnIsCompleted == false)
                Wait();
            return;
        }
        SetStateValue("cur_energy", GetStateValue("max_energy"));
        attackedThisTurn = false;
        Creature _target = GetPlayerOrSummonedByPlayerTarget();
        if (_target != null)
            target = _target;
        else if (lastAttackedCreature != null)
        {
            target = lastAttackedCreature;
        }

        if (isTryingToFlee == true)
        {
            MapCell cellToFlee = GetCellToFlee();
            BuildPath(cellToFlee.x, cellToFlee.y);
        }
        else
        {
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
        }

        if (currentPath == null || currentPath.Length == 0 || currentPath.Length > GetStateValue("view_dist"))
        {
            //print("Path is null, its length is 0 or bigger than view distance.");
            if (turnIsCompleted == false)
                Wait();
            return;
        }

        currentPathIndex = currentPath.Length - 1;
        AIController();
    }

    protected int CorrelateHealth(int _level)
    {
        int result = 400;
        for (int i = 0; i <= _level; i++)
            result = Mathf.RoundToInt(result * (i > 0 ? (float)System.Math.Round((i + 6) / (i * 12f) + 1, 2) : 1));
        return result;
    }

    protected int CorrelateAttackDamage(int _level)
    {
        int result = 100;
        for (int i = 0; i <= _level; i++)
            result = Mathf.RoundToInt(result * (i > 0 ? (float)System.Math.Round((i + 6f) / (i * 9f) + 1, 2) : 1));
        return result;
    }

    public override void CorrelateLevelWithStats()
    {
        int _level = GetStateValue("level") - 1;
        GetStateByName("max_health").startStateValue = CorrelateHealth(_level);
        SetStateValue("cur_health", GetStateByName("max_health").startStateValue);
        GetStateByName("attack_damage").startStateValue = CorrelateAttackDamage(_level);
        GetStateByName("max_energy").startStateValue = _level / 8 + 2;
        SetStateValue("cur_energy", GetStateByName("max_energy").startStateValue);
        GetStateByName("view_dist").startStateValue = _level / 20 + 6;
        GetStateByName("protection_blunt").startStateValue = _level / 2 + 1;
        GetStateByName("protection_pricking").startStateValue = _level / 2 + 3;
        GetStateByName("protection_cutting").startStateValue = _level / 2 + 1;
        GetStateByName("protection_hewing").startStateValue = _level / 2 + 1;
        GetStateByName("protection_lashing").startStateValue = _level / 2 + 2;
        GetStateByName("protection_suffocative").startStateValue = _level / 2 + 3;
        GetStateByName("protection_scalding").startStateValue = _level / 2 + 1;
        GetStateByName("protection_freezing").startStateValue = _level / 2 + 1;
        GetStateByName("protection_electric").startStateValue = _level / 2 + 1;
        GetStateByName("protection_poison").startStateValue = _level / 2 + 2;
        minExperienceFromKilling = Mathf.RoundToInt(map.player.GetExperienceDiff(_level) * (_level > 0 ? ((_level + 2) / (_level * 68f)) : 1)) + 10;
        maxExperienceFromKilling = Mathf.RoundToInt(map.player.GetExperienceDiff(_level) * (_level > 0 ? ((_level + 2) / (_level * 56f)) : 1)) + 15;
    }

    int currentPathIndex;
    bool attackedThisTurn;
    public override void AIController()
    {
        base.AIController();
        if (GetStateValue("cur_energy") > 0 && attackedThisTurn == false)
        {
            SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            if (currentPathIndex > 0)
            {
                Move();
                return;
            }
            else
            {
                if (circularAttackCharges >= 2)
                {
                    CircularAttack();
                }
                else
                {
                    Attack();
                }

                return;
            }
        }

        //PlayIdleAnimation();
        if (turnIsCompleted == false)
        {
            Wait();
            return;
        }
    }

    public override void Attack()
    {
        base.Attack();
        nextTurnDelay = 0f;
        int xToAttack = x;
        int yToAttack = y;
        if (currentPath[currentPathIndex].x - x > 0 && currentPath[currentPathIndex].y - y == 0)
        {
            facingDirection = FacingDirection.Right;
            xToAttack++;
        }
        if (currentPath[currentPathIndex].x - x < 0 && currentPath[currentPathIndex].y - y == 0)
        {
            facingDirection = FacingDirection.Left;
            xToAttack--;
        }
        if (currentPath[currentPathIndex].y - y > 0 && currentPath[currentPathIndex].x - x == 0)
        {
            facingDirection = FacingDirection.Up;
            yToAttack++;
        }
        if (currentPath[currentPathIndex].y - y < 0 && currentPath[currentPathIndex].x - x == 0)
        {
            facingDirection = FacingDirection.Down;
            yToAttack--;
        }

        if (map.cells[xToAttack, yToAttack].cellObject != null && map.cells[xToAttack, yToAttack].cellObject.GetComponent<Creature>() != null)
        {
            Creature creatureToAttack = map.cells[xToAttack, yToAttack].cellObject.GetComponent<Creature>();
            if (creatureToAttack.isAlive == true && creatureToAttack.GetStateValue("cur_health") > 0)
            {
                creatureToAttack.GetDamage(GetStateValue("attack_damage"), autoattackDamageType, this);
                lastAttackedCreature = creatureToAttack;
                //print(creatureName + " attacked " + creatureToAttack.creatureName + ";\n" + creatureToAttack.creatureName + " now have " + creatureToAttack.currentHealth);
                circularAttackCharges++;
                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                    connectedInventory.GetItemBySlotIndex(i).OnAttack(this, creatureToAttack, i);
                nextTurnDelay = 0.25f;
            }
        }

        attackedThisTurn = true;
        PlayAttackAnimation();
        PlaySound("attack");

        //AIController();
    }

    public void CircularAttack()
    {
        circularAttackCharges = 0;
        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if ((_x == x && _y == y) == false)
                {
                    if (_x >= 0 && _y >= 0 && _x < map.mapSizeX && _y < map.mapSizeY)
                    {
                        if (map.cells[_x, _y].cellObject != null && map.cells[_x, _y].cellObject.GetComponent<Creature>() != null)
                        {
                            Creature creatureToAttack = map.cells[_x, _y].cellObject.GetComponent<Creature>();
                            creatureToAttack.GetDamage(GetStateValue("attack_damage"), DamageType.cutting, this);
                            lastAttackedCreature = creatureToAttack;
                            for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                                connectedInventory.GetItemBySlotIndex(i).OnAttack(this, creatureToAttack, i);
                            //print(creatureName + " attacked " + creatureToAttack.creatureName + ";\n" + creatureToAttack.creatureName + " now have " + creatureToAttack.currentHealth);
                        }
                    }
                }
            }
        }
        attackedThisTurn = true;
        PlaySound("circular_attack");
        PlayCircularAttackAnimation();

        //AIController();
    }

    public override void Move()
    {
        if (currentPath[currentPathIndex].x - x > 0 && currentPath[currentPathIndex].y - y == 0)
        {
            facingDirection = FacingDirection.Right;
        }
        if (currentPath[currentPathIndex].x - x < 0 && currentPath[currentPathIndex].y - y == 0)
        {
            facingDirection = FacingDirection.Left;
        }
        if (currentPath[currentPathIndex].y - y > 0 && currentPath[currentPathIndex].x - x == 0)
        {
            facingDirection = FacingDirection.Up;
        }
        if (currentPath[currentPathIndex].y - y < 0 && currentPath[currentPathIndex].x - x == 0)
        {
            facingDirection = FacingDirection.Down;
        }

        PlayWalkAnimation();
        map.cells[x, y].cellObject = null;
        x = currentPath[currentPathIndex].x;
        y = currentPath[currentPathIndex].y;
        map.cells[x, y].cellObject = this;
        PlaySound("move");
        StartCoroutine(MovePathVisual());
        for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
            connectedInventory.GetItemBySlotIndex(i).OnMove(this, i);
        currentPathIndex--;
        OnCreatureMove();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        PlaySound("death");
    }

    public override void Death()
    {
        base.Death();
        if (turnIsCompleted == false)
            Wait();
    }

    private IEnumerator MovePathVisual()
    {
        animator.SetBool("isMoving", true);
        while (Vector3.Distance(transform.position, new Vector3(x, y)) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, y), 3f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(x, y);
        //PlayIdleAnimation(facingDirection);
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(0.25f);
        AIController();
    }

    public override void PlayIdleAnimation()
    {
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play(creatureNameID + "_idle_right");
        }
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play(creatureNameID + "_idle_left");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play(creatureNameID + "_idle_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play(creatureNameID + "_idle_down");
        }
    }
    public override void PlayWalkAnimation()
    {
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play(creatureNameID + "_walk_right");
        }
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play(creatureNameID + "_walk_left");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play(creatureNameID + "_walk_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play(creatureNameID + "_walk_down");
        }
    }
    public override void PlayAttackAnimation()
    {
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play(creatureNameID + "_attack_left");
        }
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play(creatureNameID + "_attack_right");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play(creatureNameID + "_attack_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play(creatureNameID + "_attack_down");
        }
    }
    public override void PlayDeathAnimation()
    {
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play(creatureNameID + "_death_left");
        }
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play(creatureNameID + "_death_right");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play(creatureNameID + "_death_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play(creatureNameID + "_death_down");
        }
    }
    public void PlayCircularAttackAnimation()
    {
        animator.Play(creatureNameID + "_circular_attack");
    }
    public override void PlaySpawnAnimation()
    {

    }

    public override void GetDamage(int _damage, DamageType damageType, Creature sender)
    {
        if (isAlive == true) // important condition
        {
            if (GetStateValue("protection_" + damageType.ToString()) > 0)
                _damage = Mathf.RoundToInt(75 * _damage / (GetStateValue("protection_" + damageType.ToString()) + 75));
            else _damage += -GetStateValue("protection_" + damageType.ToString()) * 15;
            if (_damage < 1)
                _damage = 1;
            int damageReduction = OnDamage(sender, damageType, _damage);
            if (damageReduction >= _damage)
                return;
            _damage -= damageReduction;
            SetStateValue("cur_health", (int)Mathf.MoveTowards(GetStateValue("cur_health"), 0, _damage));
            RefreshHealthbar();
            if (GetStateValue("cur_health") <= 0)
            {
                Die(sender);
            }
            if (sender != null)
                target = sender;
        }
    }
    public override void GetHealing(int _healing)
    {
        OnHeal();
        SetStateValue("cur_health", (int)Mathf.MoveTowards(GetStateValue("cur_health"), GetStateValue("max_health"), _healing));
        RefreshHealthbar();
    }

    public void Die(Creature causedBy)
    {
        SetStateValue("cur_health", 0);
        RefreshHealthbar();
        isAlive = false;
        if (causedBy != null)
        {
            if (causedBy.GetComponent<PlayerController>() != null)
            {
                PlayerController player = map.player;
                player.AddExperience(experienceFromKilling, true);
                player.AddCreatureToBestiary(this);
                player.OnCreatureKill(this);
            }
            else causedBy.OnCreatureKill(this);
        }
        OnDeath();
        PlayDeathAnimation();
    }

    public override int OnDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        int result = base.OnDamage(_sender, _damageType, _damage);
        PlaySound("get_damage");
        return result;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (lootIsInitialized == false)
        {
            for (int i = 0; i < possibleLoots.Count; i++)
                if (Random.Range(0, 100) < possibleLoots[i].lootRate)
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(possibleLoots[i].lootID));
            experienceFromKilling = Random.Range(minExperienceFromKilling, maxExperienceFromKilling);
            lootIsInitialized = true;
        }
    }

    public override string ToString()
    {
        return creatureNameID.ToString() + ";"
                + x + ";"
                + y + ";"
                //+ GetStateValue("cur_health").ToString() + ";"
                //+ GetStateValue("cur_energy").ToString() + ";"
                + isAlive.ToString() + ";"
                + ((int)facingDirection).ToString() + ";"
                + circularAttackCharges.ToString() + ";"
                + lootIsInitialized.ToString() + ";"
                + cellObjectID.ToString() + ";"
                + experienceFromKilling.ToString() + ";"
                + isSummonedByPlayer.ToString();
    }

    public override void FromString(string str)
    {
        string[] s = str.Split(';');
        creatureName = s[0];
        x = int.Parse(s[1]);
        y = int.Parse(s[2]);
        //SetStateValue("cur_health", int.Parse(s[3]));
        //SetStateValue("cur_energy", int.Parse(s[4]));
        isAlive = bool.Parse(s[3]);
        facingDirection = (FacingDirection)int.Parse(s[4]);
        circularAttackCharges = int.Parse(s[5]);
        transform.position = new Vector3(x, y, 0);
        lootIsInitialized = bool.Parse(s[6]);
        cellObjectID = int.Parse(s[7]);
        experienceFromKilling = int.Parse(s[8]);
        isSummonedByPlayer = bool.Parse(s[9]);

        //RecalculateStates();

        //isAlive = GetStateValue("cur_health") > 0;
        if (isAlive == true)
            PlayIdleAnimation();
        else Die(null);
        RefreshHealthbar();
    }
}