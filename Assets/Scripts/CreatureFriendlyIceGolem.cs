using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureFriendlyIceGolem : CreatureEnemy
{
    public Creature GetCreatureTarget()
    {
        for (int i = 1; i < GetStateValue("view_dist"); i++)
        {
            var _x = i - 1;
            var _y = 0;
            var dx = 1;
            var dy = 1;
            var diameter = i * 2;
            var decisionOver2 = dx - diameter;

            while (_x >= _y)
            {
                if (CheckCellToNonPlayer(_x + x, _y + y) == true && map.cells[_x + x, _y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[_x + x, _y + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(_x + x, -_y + y) == true && map.cells[_x + x, -_y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[_x + x, -_y + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(-_x + x, -_y + y) == true && map.cells[-_x + x, -_y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[-_x + x, -_y + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(-_x + x, _y + y) == true && map.cells[-_x + x, _y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[-_x + x, _y + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(_y + x, _x + y) == true && map.cells[_y + x, _x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[_y + x, _x + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(_y + x, -_x + y) == true && map.cells[_y + x, -_x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[_y + x, -_x + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(-_y + x, -_x + y) == true && map.cells[-_y + x, -_x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[-_y + x, -_x + y].cellObject.GetComponent<Creature>();
                if (CheckCellToNonPlayer(-_y + x, _x + y) == true && map.cells[-_y + x, _x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == false)
                    return map.cells[-_y + x, _x + y].cellObject.GetComponent<Creature>();

                if (decisionOver2 <= 0)
                {
                    _y++;
                    decisionOver2 += dy;
                    dy += 2;
                }
                if (decisionOver2 > 0)
                {
                    _x--;
                    dx += 2;
                    decisionOver2 += (-diameter) + dx;
                }
            }
        }

        return null;
    }

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
        if (target == null)
        {
            Creature _target = GetCreatureTarget();
            if (_target != null)
                target = _target;
            else if (lastAttackedCreature != null && lastAttackedCreature.GetComponent<PlayerController>() == null)
            {
                target = lastAttackedCreature;
            }
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

    public override void CorrelateLevelWithStats() { }

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
                Attack();
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
                if (Random.Range(0, 100) < 0.33f)
                {
                    Effect.ApplyEffect(EffectsDatabase.GetEffectByID(6), creatureToAttack, this, 1, 1);
                }
                lastAttackedCreature = creatureToAttack;
                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                    connectedInventory.GetItemBySlotIndex(i).OnAttack(this, creatureToAttack, i);
                nextTurnDelay = 1.0f;
            }
            //print(creatureName + " attacked " + creatureToAttack.creatureName + ";\n" + creatureToAttack.creatureName + " now have " + creatureToAttack.currentHealth);
        }
        attackedThisTurn = true;
        PlayAttackAnimation();
        PlaySound("attack");

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

    public override void Death()
    {
        base.Death();
        if (turnIsCompleted == false)
            Wait();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        PlaySound("death");
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
            animator.Play("ice_golem_idle_right");
        }
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play("ice_golem_idle_left");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play("ice_golem_idle_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play("ice_golem_idle_down");
        }
    }
    public override void PlayWalkAnimation()
    {
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play("ice_golem_walk_right");
        }
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play("ice_golem_walk_left");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play("ice_golem_walk_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play("ice_golem_walk_down");
        }
    }
    public override void PlayAttackAnimation()
    {
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play("ice_golem_attack_left");
        }
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play("ice_golem_attack_right");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play("ice_golem_attack_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play("ice_golem_attack_down");
        }
    }
    public override void PlayDeathAnimation()
    {
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play("ice_golem_death_left");
        }
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play("ice_golem_death_right");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play("ice_golem_death_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play("ice_golem_death_down");
        }
    }
    public override void PlaySpawnAnimation()
    {

    }

    public override void GetDamage(int _damage, DamageType damageType, Creature sender)
    {
        if (isAlive == true) // important condition
        {
            if (damageType != DamageType.freezing)
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
                    Die(sender);
                if (sender != null && sender.GetComponent<PlayerController>() == null)
                    target = sender;
            }
        }
    }
    public override void GetHealing(int _healing)
    {
        OnHeal();
        SetStateValue("cur_health", (int)Mathf.MoveTowards(GetStateValue("cur_health"), GetStateValue("max_health"), _healing));
        RefreshHealthbar();
    }

    public override void OnCreatureKill(Creature creature)
    {
        base.OnCreatureKill(creature);
        PlayerController player = map.player;
        player.AddExperience(creature.experienceFromKilling, true);
        player.AddCreatureToBestiary(creature);
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
        transform.position = new Vector3(x, y, 0);
        lootIsInitialized = bool.Parse(s[5]);
        cellObjectID = int.Parse(s[6]);
        experienceFromKilling = int.Parse(s[7]);
        isSummonedByPlayer = bool.Parse(s[8]);

        //RecalculateStates();

        //isAlive = GetStateValue("cur_health") > 0;
        if (isAlive == true)
            PlayIdleAnimation();
        else Die(null);
        RefreshHealthbar();
    }
}