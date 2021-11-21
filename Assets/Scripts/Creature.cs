using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DamageType
{
    blunt,
    pricking,
    cutting,
    hewing,
    lashing,
    suffocative,
    scalding,
    freezing,
    electric,
    poison,
    pure,
    none
}

public enum CreatureRace
{
    Living,
    Undead,
    Demon,
    Light,
    Animal,
    Reptile,
    Human,
    Amphibian,
    Mechanism,
    Elemental,
    Insect
}

[Serializable]
public class CreatureInformation
{
    public string creatureNameID;
    public int creatureLevel;
    public int traitsCount;
    public int creatureHealth;
    public int creatureEnergy;
    public int creatureDamage;
    public int[] creatureProtections;
    public CreatureRace[] creatureRaces;
    /*
    public int creatureBluntProtection;
    public int creaturePrickingProtection;
    public int creatureCuttingProtection;
    public int creatureHewingProtection;
    public int creatureLashingProtection;
    public int creatureSuffocativeProtection;
    public int creatureScaldingProtection;
    public int creatureFreezingProtection;
    public int creatureElectricProtection;
    public int creaturePoisonProtection;
    */

    public DamageType creatureAttackType;

    public CreatureInformation(string _nameID, int _level, int _traitsCount, int _health, int _energy, int _damage, DamageType _attackType, int[] _protections, int _racesCount, CreatureRace[] _races)
    {
        creatureNameID = _nameID;
        creatureLevel = _level;
        traitsCount = _traitsCount;
        creatureHealth = _health;
        creatureEnergy = _energy;
        creatureDamage = _damage;
        creatureAttackType = _attackType;
        creatureProtections = _protections;
        creatureRaces = _races;
        /*
        creatureBluntProtection = _protections[0];
        creaturePrickingProtection = _protections[1];
        creatureCuttingProtection = _protections[2];
        creatureHewingProtection = _protections[3];
        creatureLashingProtection = _protections[4];
        creatureSuffocativeProtection = _protections[5];
        creatureScaldingProtection = _protections[6];
        creatureFreezingProtection = _protections[7];
        creatureElectricProtection = _protections[8];
        creaturePoisonProtection = _protections[9];
        */
    }

    public override string ToString()
    {
        string result = creatureNameID + ";" 
            + creatureLevel + ";"
            + traitsCount + ";"
            + creatureHealth + ";"
            + creatureEnergy + ";"
            + creatureDamage + ";"
            + ((int)creatureAttackType).ToString() + ";"
            + creatureProtections[0] + ";"
            + creatureProtections[1] + ";"
            + creatureProtections[2] + ";"
            + creatureProtections[3] + ";"
            + creatureProtections[4] + ";"
            + creatureProtections[5] + ";"
            + creatureProtections[6] + ";"
            + creatureProtections[7] + ";"
            + creatureProtections[8] + ";"
            + creatureProtections[9] + ";"
            + creatureRaces.Length;
        for (int i = 0; i < creatureRaces.Length; i++)
            result += ";" + ((int)creatureRaces[i]).ToString();
        return result;
    }

    public static CreatureInformation FromString(string input)
    {
        string[] s = input.Split(';');
        int _racesCount = int.Parse(s[17]);
        CreatureRace[] _races = new CreatureRace[_racesCount];
        for (int i = 0; i < _racesCount; i++)
        {
            _races[i] = (CreatureRace)int.Parse(s[i + 18]);
        }
        CreatureInformation inf = new CreatureInformation(
            s[0], 
            int.Parse(s[1]),
            int.Parse(s[2]),
            int.Parse(s[3]),
            int.Parse(s[4]),
            int.Parse(s[5]),
            (DamageType)(int.Parse(s[6])),
            new int[10] 
            {
                int.Parse(s[7]),
                int.Parse(s[8]),
                int.Parse(s[9]),
                int.Parse(s[10]),
                int.Parse(s[11]),
                int.Parse(s[12]),
                int.Parse(s[13]),
                int.Parse(s[14]),
                int.Parse(s[15]),
                int.Parse(s[16])
            },
            _racesCount,
            _races);
        return inf;
    }
}

[Serializable]
public class CreatureState
{
    public string stateName;
    public int startStateValue;
    public int valueAddends;
    public bool shouldBeChangedDirectly;
    public int totalStateValue; //{ get; protected set; }

    public CreatureState(string name)
    {
        stateName = name;
        startStateValue = 0;
    }

    public CreatureState(string name, int value)
    {
        stateName = name;
        startStateValue = value;
    }

    public void Calculate()
    {
        if (shouldBeChangedDirectly == false)
            totalStateValue = startStateValue + valueAddends;
    }

    public override string ToString()
    {
        return stateName + ";" + startStateValue + ";" + shouldBeChangedDirectly + ";" + totalStateValue;
    }
}

public abstract class Creature : CellObject
{
    public string creatureName;
    public string creatureNameID;

    //public List<CreatureState> creatureStates;
    public CreatureStatesHandler statesHandler;
    public List<Effect> currentEffects = new List<Effect>();
    public VisualEffectsContainer effectsContainer;

    public CreatureInformation information;

    public DamageType autoattackDamageType;

    public CreatureRace[] creatureRaces;
    public int traitsCount;

    public MapCell[] currentPath;
    public FacingDirection facingDirection;
    public Animator animator;

    public float nextTurnDelay;
    public Creature lastAttackedCreature;
    public CreatureVisualController visualController;

    public bool canBeTargetedByAI;
    public bool isSummonedByPlayer;
    public bool turnIsCompleted;
    public bool isAlive;
    public bool canBeLootedWhileAlive;
    public bool canBeAffectedByEffects;
    public bool showHealthbar;
    public bool showLevelText;
    public bool canBeAddedToBestiary;

    public Inventory connectedInventory;
    /// <summary>
    /// Each item (even the same) must be specified separately.
    /// </summary>
    public List<PossibleLoot> possibleLoots;

    public List<int> itemsGivingBonusesIDs;

    public int minExperienceFromKilling, maxExperienceFromKilling;
    public int experienceFromKilling;

    public void Awake()
    {
        base.Initialize();

        // must be applied to each creature's prefab manually
        // creature may have additional states besides these
        /*
        creatureStates.Add(new CreatureState("max_health"));
        creatureStates.Add(new CreatureState("cur_health"));
        creatureStates.Add(new CreatureState("max_energy"));
        creatureStates.Add(new CreatureState("cur_energy"));
        creatureStates.Add(new CreatureState("attack_damage"));
        creatureStates.Add(new CreatureState("view_dist"));
        creatureStates.Add(new CreatureState("protection"));
        */

        effectsContainer = FindObjectOfType<VisualEffectsContainer>();
        animator = visualObject.GetComponent<Animator>();
        visualController.owner = this;
        RefreshHealthbar();

        PlayIdleAnimation();
        //currentHealth = maximumHealth;
        //isAlive = true;     
    }

    public abstract void CorrelateLevelWithStats();

    public void SetCreatureInformation()
    {
        information = new CreatureInformation(
            creatureNameID,
            GetStateValue("level"),
            traitsCount,
            GetStateByName("max_health").startStateValue,
            GetStateByName("max_energy").startStateValue,
            GetStateByName("attack_damage").startStateValue,
            autoattackDamageType,
            new int[10]
            {
                GetStateByName("protection_blunt").startStateValue,
                GetStateByName("protection_pricking").startStateValue,
                GetStateByName("protection_cutting").startStateValue,
                GetStateByName("protection_hewing").startStateValue,
                GetStateByName("protection_lashing").startStateValue,
                GetStateByName("protection_suffocative").startStateValue,
                GetStateByName("protection_scalding").startStateValue,
                GetStateByName("protection_freezing").startStateValue,
                GetStateByName("protection_electric").startStateValue,
                GetStateByName("protection_poison").startStateValue,
            },
            creatureRaces.Length,
            creatureRaces);
    }

    public void RecalculateStates()
    {
        for (int i = 0; i < statesHandler.creatureStates.Count; i++)
            statesHandler.creatureStates[i].Calculate();
        RefreshHealthbar();
    }

    public void RecalculateState(string stateName)
    {
        for (int i = 0; i < statesHandler.creatureStates.Count; i++)
            if (statesHandler.creatureStates[i].stateName == stateName)
            {
                statesHandler.creatureStates[i].Calculate();
                return;
            }
        RefreshHealthbar();
    }

    public void ChangeStateBonus(string stateName, int value)
    {
        for (int i = 0; i < statesHandler.creatureStates.Count; i++)
            if (statesHandler.creatureStates[i].stateName == stateName)
            {
                statesHandler.creatureStates[i].valueAddends += value;
                statesHandler.creatureStates[i].Calculate();
                RefreshHealthbar();
                return;
            }
    }

    public void SetStateValue(string stateName, int value)
    {
        for (int i = 0; i < statesHandler.creatureStates.Count; i++)
            if (statesHandler.creatureStates[i].stateName == stateName)
            {
                statesHandler.creatureStates[i].totalStateValue = value;
                RefreshHealthbar();
                //creatureStates[i].Calculate();
                return;
            }
    }

    /// <summary>
    /// Returns -1 if the state with this name does not exist.
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public int GetStateValue(string stateName)
    {
        for (int i = 0; i < statesHandler.creatureStates.Count; i++)
            if (statesHandler.creatureStates[i].stateName == stateName)
            {
                return statesHandler.creatureStates[i].totalStateValue;
            }
        return -1;
    }

    public CreatureState GetStateByName(string stateName)
    {
        for (int i = 0; i < statesHandler.creatureStates.Count; i++)
            if (statesHandler.creatureStates[i].stateName == stateName)
            {
                return statesHandler.creatureStates[i];
            }
        return new CreatureState("none");
    }

    public Creature GetCreatureInFront()
    {
        if (facingDirection == FacingDirection.Up)
        {
            if (map.CheckCell(x, y + 1, CellCheckFlags.ToExistance) == true
                && map.cells[x, y + 1].cellObject != null
                && map.cells[x, y + 1].cellObject.GetComponent<Creature>() != null)
            {
                return map.cells[x, y + 1].cellObject.GetComponent<Creature>();
            }
        }
        else if (facingDirection == FacingDirection.Down)
        {
            if (map.CheckCell(x, y - 1, CellCheckFlags.ToExistance) == true
                && map.cells[x, y - 1].cellObject != null
                && map.cells[x, y - 1].cellObject.GetComponent<Creature>() != null)
            {
                return map.cells[x, y - 1].cellObject.GetComponent<Creature>();
            }
        }
        else if (facingDirection == FacingDirection.Right)
        {
            if (map.CheckCell(x + 1, y, CellCheckFlags.ToExistance) == true
                && map.cells[x + 1, y].cellObject != null
                && map.cells[x + 1, y].cellObject.GetComponent<Creature>() != null)
            {
                return map.cells[x + 1, y].cellObject.GetComponent<Creature>();
            }
        }
        else if (facingDirection == FacingDirection.Left)
        {
            if (map.CheckCell(x - 1, y, CellCheckFlags.ToExistance) == true
                && map.cells[x - 1, y].cellObject != null
                && map.cells[x - 1, y].cellObject.GetComponent<Creature>() != null)
            {
                return map.cells[x - 1, y].cellObject.GetComponent<Creature>();
            }
        }

        return null;
    }

    public virtual void OnTurnStart()
    {
        nextTurnDelay = 0.0f;
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (canBeAffectedByEffects == true)
                currentEffects[i].OnTurnStart();
            currentEffects[i].effectDuration--;
            if (currentEffects[i].effectDuration < 0)
            {
                if (canBeAffectedByEffects == true)
                    currentEffects[i].OnEffectEnd();
                currentEffects[i].hasEnded = true;
            }
        }

        currentEffects = currentEffects.Where(e => e.hasEnded == false).ToList();
    }
    public virtual void OnTurnEnd()
    {
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                currentEffects[i].OnTurnEnd();
            }
    }
    public virtual void OnCreatureKill(Creature creature)
    {
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                currentEffects[i].OnCreatureKill(creature);
            }
    }
    public virtual void OnSpawn() { }
    public virtual void OnDeath()
    {
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                currentEffects[i].OnCreatureDeath();
            }
    }
    /// <summary>
    /// Returns damage that was canceled during this function.
    /// </summary>
    /// <param name="_sender"></param>
    /// <param name="_damageType"></param>
    /// <param name="_damage"></param>
    /// <returns></returns>
    public virtual int OnDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        int result = 0;
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                result += currentEffects[i].OnCreatureDamage(_sender, _damageType, _damage);
            }
        return result;
    }
    public virtual void OnHeal()
    {
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                currentEffects[i].OnCreatureHeal();
            }
    }
    public virtual void OnCastSpell(Spell spell)
    {
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                currentEffects[i].OnCreatureCastSpell(spell);
            }
    }
    public virtual void OnCreatureMove()
    {
        if (canBeAffectedByEffects == true)
            for (int i = 0; i < currentEffects.Count; i++)
            {
                currentEffects[i].OnCreatureMove();
            }
    }

    public virtual void AIController() { }
    public void RefreshHealthbar()
    {
        if (healthbar != null)
        {
            if (showHealthbar == true)
            {
                healthbar.gameObject.SetActive(true);
                if (GetStateValue("max_health") > 0)
                {
                    healthbar.size = new Vector2((float)GetStateValue("cur_health") / GetStateValue("max_health"), 0.125f); //healthbar.size.y);
                    if (healthbar.size.x > 1)
                        healthbar.size = new Vector2(1, 0.125f);
                }
                else healthbar.size = new Vector2(0, 0.125f); //healthbar.size.y);
            }
            else healthbar.gameObject.SetActive(false);
        }
    }

    public void ReturnToCurrentAnimation()
    {
        PlayIdleAnimation();
    }

    public VisualEffect PlayVisualEffect(string effectName)
    {
        if (effectsContainer.GetVisualEffect(effectName) != null)
        {
            VisualEffect visualEffect = Instantiate(effectsContainer.GetVisualEffect(effectName), visualController.transform.position, Quaternion.identity);
            visualEffect.transform.SetParent(visualController.transform);
            return visualEffect;
        }
        else return null;
    }

    public VisualEffect PlayVisualEffect(string effectName, Vector3 position)
    {
        if (effectsContainer.GetVisualEffect(effectName) != null)
        {
            VisualEffect visualEffect = Instantiate(effectsContainer.GetVisualEffect(effectName), position, Quaternion.identity);
            visualEffect.transform.SetParent(visualController.transform);
            return visualEffect;
        }
        else return null;
    }

    public virtual void Wait()
    {
        turnIsCompleted = true;
        StartCoroutine(EndTurn(nextTurnDelay));
    }
    public virtual void Attack() { }
    public virtual void Move() { }
    public virtual void Death()
    {
        //isAlive = false;
        //map.DestroyCreature(this);
    }

    // basically for Destructibles
    public virtual void Destroy() { }

    public abstract void GetDamage(int _damage, DamageType damageType, Creature sender);
    public abstract void GetHealing(int _healing);

    /*
    public void ApplyEffect(Effect effectToApply, Creature effectSender, int duration, int level)
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (currentEffects[i].effectID == effectToApply.effectID)
            {
                if (currentEffects[i].effectLevel < level)
                {
                    currentEffects[i].effectLevel = level;
                    currentEffects[i].effectSender = effectSender;
                    currentEffects[i].effectHandler = this;
                }
                if (currentEffects[i].effectLevel == level && currentEffects[i].effectDuration < duration)
                {
                    currentEffects[i].effectDuration = duration;
                    currentEffects[i].effectSender = effectSender;
                    currentEffects[i].effectHandler = this;
                }
                return;
            }
        }

        effectToApply.effectDuration = duration;
        effectToApply.effectLevel = level;
        effectToApply.effectSender = effectSender;
        effectToApply.effectHandler = this;
        currentEffects.Add(effectToApply);
        currentEffects[currentEffects.Count - 1].OnEffectBegin();
    }
    */

    // NOT FOR USE TO END THE TURN
    public virtual IEnumerator EndTurn(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        map.NextTurn();
    }

    public abstract void PlayIdleAnimation();
    public abstract void PlayWalkAnimation();
    public abstract void PlayAttackAnimation();
    public abstract void PlayDeathAnimation();
    public abstract void PlaySpawnAnimation();

    public abstract override string ToString();
    public abstract void FromString(string s);
}