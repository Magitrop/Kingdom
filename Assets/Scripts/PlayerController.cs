using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class SpellBonus
{
    public SpellType spellType;
    public int spellPowerBonus;
}

public class PlayerController : Creature
{
    //public int heroLevel;
    public TextMesh pathMark;
    List<TextMesh> pathMarks = new List<TextMesh>();

    public MapBordersController bordersController;

    public Image playerHealthbar;
    public Image energybar, xpbar;
    public Text
        healthText,
        energyText,
        xpText,
        attackDamageText,
        nameText,
        attackTypeText,
        spellPowerText;
    public Text[] protectionTexts;

    public UIScrollController scroll;
    public Bestiary bestiary;
    public GameObject menu;
    public DeathMenu deathMenu;
    public GameObject visualInventory, lootInventory;
    public UIInventorySlot[] visualSlots;

    public UIInventoryLootButton lootButton;

    public GameObject effectIconsContainer;
    public EffectIconsController iconsController;
    public EffectIcon effectIconExemplar;

    public KeyCode keyMoveUp;
    public KeyCode keyMoveRight;
    public KeyCode keyMoveDown;
    public KeyCode keyMoveLeft;
    public KeyCode keyShowScroll;
    public KeyCode keyShowBestiary;
    public KeyCode keyShowInventory;
    public KeyCode keySaveGame;
    public KeyCode keyLoot;

    public PlayerStatistics statistics;
    // list that contains recipes results' IDs
    public List<int> exploredRecipes;

    public List<Skill> gainedSkills;
    public LevelupPanelController levelupPanelController;

    public bool isMoving { get; protected set; }

    public int experienceToNextLevel;

    public bool inExploreMode;

    public bool inSpellMode;
    public Spell currentSpellToCast;
    public SpellsDatabase spellsDatabase;
    public Animator currentSpellSelection;
    int currentSpellRequirementIndex;
    List<object> currentSpellArguments = new List<object>();
    int spellSourceItemSlotIndex = -1; // if spell is casting from source item (like scroll)

    public UIInventoryStepButton stepButton;
    public UIInventoryAttackButton attackButton;
    public UIInventoryBestiaryButton bestiaryButton;
    public UIInventoryInvButton invButton;
    public UIInventoryWaitButton waitButton;

    public RectTransform notificationPanel;
    public Text notificationText;

    public List<int> preparedSkillsIDs;

    public int criticalStrikesInThisTurn;
    public List<Vector2Int> creaturesKilledInThisTurnPositions;

    public int spellsCastInThisTurn;

    public void InitializePlayer()
    {
        base.Initialize();
        map = FindObjectOfType<MapController>();
        map.LoadGame();
        int energy = GetStateValue("cur_energy");
        bestiary.Initialize();
        RefreshStatsVisual();
        map.InitializeMap();

        transform.position = new Vector3(x, y, 0);
        if (map.isLevelLoadedNotFromFile == false)
            SetStateValue("cur_energy", energy);
        else SetStateValue("cur_energy", GetStateValue("max_energy"));
        isAlive = GetStateValue("cur_health") > 0;
        if (isAlive == true)
            PlayIdleAnimation();
        else
        {
            PlayDeathAnimation();
            deathMenu.ShowStatistics();
            deathMenu.gameObject.SetActive(true);
        }
        
        statistics.maxLevelThisTime = GetStateValue("level");
        if (PlayerPrefs.GetInt("maxLevelEver") < statistics.maxLevelThisTime)
            statistics.maxLevelEver = statistics.maxLevelThisTime;
        map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
        bestiary.SaveBestiary();
        //map.SaveGame();
        currentSpellToCast = SpellsDatabase.GetSpellByID(-1);
    }

    public override void CorrelateLevelWithStats() { }

    public void DeselectVisualSlots()
    {
        for (int i = 0; i < visualSlots.Length; i++)
            visualSlots[i].selectionVisual.gameObject.SetActive(false);
        connectedInventory.selectedSlotIndex = -1;
    }

    public void StartCastingSpell(Spell spellToCast)
    {
        visualInventory.SetActive(false);
        lootInventory.SetActive(false);
        bestiary.gameObject.SetActive(false);
        currentSpellToCast = spellToCast;
        currentSpellSelection.gameObject.SetActive(currentSpellToCast.spellRequirements.Length > 0);
        currentSpellRequirementIndex = 0;
        currentSpellArguments.Clear();
        inSpellMode = true;
    }

    public void StartCastingSpell(Spell spellToCast, int sourceItemSlotIndex)
    {
        visualInventory.SetActive(false);
        lootInventory.SetActive(false);
        bestiary.gameObject.SetActive(false);
        spellSourceItemSlotIndex = sourceItemSlotIndex;
        currentSpellToCast = spellToCast;
        currentSpellSelection.gameObject.SetActive(currentSpellToCast.spellRequirements.Length > 0);
        currentSpellRequirementIndex = 0;
        currentSpellArguments.Clear();
        inSpellMode = true;
    }

    public void ExploreItem(InventoryItem itemToExplore, int itemSlotIndex)
    {
        InventoryItem item = ItemsDatabase.GetItemByID(itemToExplore.recipeExplorationResultID);
        if (item.recipeShouldBeExplored == true && exploredRecipes.Contains(item.itemID) == false)
        {
            exploredRecipes.Add(item.itemID);
            connectedInventory.PlaceItemInSlot(ItemsDatabase.GetItemByID(0), itemSlotIndex);
            map.ShowNotification("notifications_recipe_is_explored", Translate.TranslateText(itemToExplore.itemNameRaw), Translate.TranslateText(item.itemNameRaw));
            //print(ItemsDatabase.GetItemByID(itemToExplore.itemID).itemName + " is explored and recipe of " + ItemsDatabase.GetItemByID(item.itemID).itemName + " is learnt");
        }
    }

    public void DisableSpellMode()
    {
        currentSpellSelection.gameObject.SetActive(false);
        currentSpellToCast = SpellsDatabase.GetSpellByID(-1);
        currentSpellRequirementIndex = 0;
        currentSpellArguments.Clear();
        spellSourceItemSlotIndex = -1;
        inSpellMode = false;
    }

    public override void Update()
    {
        base.Update();
        //if (Input.GetKeyDown(KeyCode.LeftAlt))
        //bordersController.gameObject.SetActive(!bordersController.gameObject.activeSelf);
        if (Input.GetMouseButtonDown(1))
        {
            DisableSpellMode();
            inExploreMode = false;
        }

        if (isAlive == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menu.SetActive(!menu.activeSelf);
                visualInventory.SetActive(false);
                lootInventory.SetActive(false);
                bestiary.gameObject.SetActive(false);
                DisableSpellMode();
            }

            if (Input.GetKeyDown(keyShowInventory))
            {
                /*
                visualInventory.SetActive(!visualInventory.activeSelf);
                lootInventory.SetActive(false);
                */
                invButton.OnPointerDown(null);
            }

            if (Input.GetKeyDown(keyLoot) && isAlive == true)
            {
                //lootButton.Loot();
                lootButton.OnPointerDown(null);
            }

            /*
            if (Input.GetKeyDown(key_saveGame) && isAlive == true)
            {
                bestiary.SaveBestiary();
                SaveGame();
            }
            */

            bool availabilityCondition =
                    (isMoving == false &&
                    bestiary.gameObject.activeSelf == false &&
                    menu.activeSelf == false &&
                    visualInventory.activeSelf == false && 
                    notificationPanel.gameObject.activeSelf == false &&
                    levelupPanelController.gameObject.activeSelf == false);
            if (turnIsCompleted == false)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(30));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(32));
                    //connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(37));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(42));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(44));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(46));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(48));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(50));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(52));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(54));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(56));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(58));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(60));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(62));
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(29));
                    connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(25));
                }

                if (inSpellMode == false)
                {
                    //if (turnIsCompleted == false && isMoving == false && bestiary.gameObject.activeSelf == false && menu.activeSelf == false && visualInventory.activeSelf == false)

                    // "else if" constructions are preventing diagonal moves
                    if (Input.GetKeyDown(keyMoveUp))
                    {
                        if (facingDirection == FacingDirection.Up)
                        {
                            /*
                            if (y + 1 < map.mapSizeY && map.CheckCell(x, y + 1, CellCheckFlags.ToPassability) && turnIsCompleted == false && availabilityCondition == true && GetStateValue("cur_energy") > 0)
                            {
                                isMoving = true;
                                map.cells[x, y].cellObject = null;
                                y++;
                                //transform.position = new Vector3(x, y, 0);
                                map.cells[x, y].cellObject = this;
                                StartCoroutine(MoveVisual());
                                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                                RefreshEnergybar();
                                PlaySound("move");
                                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                                    connectedInventory.GetItemBySlotIndex(i).OnMove(this);
                                map.finishLevelWindow.SetActive((map.levelExitPosition == new Vector2Int(x, y)));
                                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                                OnCreatureMove();
                            }
                            */
                            stepButton.OnPointerDown(null);
                        }
                        else if (availabilityCondition == true)
                        {
                            facingDirection = FacingDirection.Up;
                            PlayIdleAnimation();
                        }
                    }
                    else if (Input.GetKeyDown(keyMoveDown))
                    {
                        if (facingDirection == FacingDirection.Down)
                        {
                            /*
                            if (y - 1 >= 0 && map.CheckCell(x, y - 1, CellCheckFlags.ToPassability) && turnIsCompleted == false && availabilityCondition == true && GetStateValue("cur_energy") > 0)
                            {
                                isMoving = true;
                                map.cells[x, y].cellObject = null;
                                y--;
                                //transform.position = new Vector3(x, y, 0);
                                map.cells[x, y].cellObject = this;
                                StartCoroutine(MoveVisual());
                                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                                RefreshEnergybar();
                                PlaySound("move");
                                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                                    connectedInventory.GetItemBySlotIndex(i).OnMove(this);
                                map.finishLevelWindow.SetActive((map.levelExitPosition == new Vector2Int(x, y)));
                                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                                OnCreatureMove();
                            }
                            */
                            stepButton.OnPointerDown(null);
                        }
                        else if (availabilityCondition == true)
                        {
                            facingDirection = FacingDirection.Down;
                            PlayIdleAnimation();
                        }
                    }
                    else if (Input.GetKeyDown(keyMoveRight))
                    {
                        if (facingDirection == FacingDirection.Right)
                        {
                            /*
                            if (x + 1 < map.mapSizeX && map.CheckCell(x + 1, y, CellCheckFlags.ToPassability) && turnIsCompleted == false && availabilityCondition == true && GetStateValue("cur_energy") > 0)
                            {
                                isMoving = true;
                                map.cells[x, y].cellObject = null;
                                x++;
                                //transform.position = new Vector3(x, y, 0);
                                map.cells[x, y].cellObject = this;
                                StartCoroutine(MoveVisual());
                                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                                RefreshEnergybar();
                                PlaySound("move");
                                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                                    connectedInventory.GetItemBySlotIndex(i).OnMove(this);
                                map.finishLevelWindow.SetActive((map.levelExitPosition == new Vector2Int(x, y)));
                                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                                OnCreatureMove();
                            }
                            */
                            stepButton.OnPointerDown(null);
                        }
                        else if (availabilityCondition == true)
                        {
                            facingDirection = FacingDirection.Right;
                            PlayIdleAnimation();
                        }
                    }
                    else if (Input.GetKeyDown(keyMoveLeft))
                    {
                        if (facingDirection == FacingDirection.Left)
                        {
                            /*
                            if (x - 1 >= 0 && map.CheckCell(x - 1, y, CellCheckFlags.ToPassability) && turnIsCompleted == false && availabilityCondition == true && GetStateValue("cur_energy") > 0)
                            {
                                isMoving = true;
                                map.cells[x, y].cellObject = null;
                                x--;
                                //transform.position = new Vector3(x, y, 0);
                                map.cells[x, y].cellObject = this;
                                StartCoroutine(MoveVisual());
                                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                                RefreshEnergybar();
                                PlaySound("move");
                                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                                    connectedInventory.GetItemBySlotIndex(i).OnMove(this);
                                map.finishLevelWindow.SetActive((map.levelExitPosition == new Vector2Int(x, y)));
                                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                                OnCreatureMove();
                            }
                            */
                            stepButton.OnPointerDown(null);
                        }
                        else if (availabilityCondition == true)
                        {
                            facingDirection = FacingDirection.Left;
                            PlayIdleAnimation();
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.A) && availabilityCondition == true && GetStateValue("cur_energy") > 0)
                    {
                        //Attack();
                        attackButton.OnPointerDown(null);
                    }

                    if (Input.GetKeyDown(KeyCode.W) && availabilityCondition == true)
                    {
                        waitButton.OnPointerDown(null);
                    }

                    /*
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
                        if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                            mouse.y >= 0 && mouse.y < map.mapSizeY &&
                            moving == false)
                        {
                            for (int i = 0; i < pathMarks.Count; i++)
                                if (pathMarks[i] != null)
                                    Destroy(pathMarks[i].gameObject);
                            pathMarks.Clear();
                            map.ClearPath();
                            currentPath = map.FindPath(map.cells[x, y], map.cells[mouse.x, mouse.y], this);
                            if (currentPath != null)
                            {
                                ShowPath(currentPath);
                                StartCoroutine(MovePath());
                            }
                        }
                    }
                    */

                }
                else
                {
                    if (currentSpellToCast.spellID != -1)
                    {
                        if (currentSpellRequirementIndex < currentSpellToCast.spellRequirements.Length)
                        {
                            switch (currentSpellToCast.spellRequirements[currentSpellRequirementIndex])
                            {
                                case (SpellRequirement.ChooseAnyCell):
                                    {
                                        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                        Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
                                        if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                                            mouse.y >= 0 && mouse.y < map.mapSizeY)
                                        {
                                            currentSpellSelection.transform.position = new Vector3(mouse.x + 0.5f, mouse.y + 0.5f);
                                            int path = map.FindDistance(map.cells[x, y], map.cells[mouse.x, mouse.y]);
                                            if ((currentSpellToCast.isRanged == false 
                                                || (map.cells[mouse.x, mouse.y].cellObject != null && map.cells[mouse.x, mouse.y].cellObject.GetComponent<PlayerController>() != null) 
                                                || (path >= 0 && path <= currentSpellToCast.spellRange)) && map.CheckCell(mouse.x, mouse.y, CellCheckFlags.ToVisibility))
                                            {
                                                currentSpellSelection.Play("tiles_selection_suitable");
                                                if (Input.GetMouseButtonDown(0))
                                                {
                                                    currentSpellArguments.Add(mouse);
                                                    currentSpellRequirementIndex++;
                                                    currentSpellSelection.gameObject.SetActive(false);
                                                    //currentSpellSelection = Instantiate(currentSpellToCast.);
                                                }
                                            }
                                            else
                                            {
                                                currentSpellSelection.Play("tiles_selection_unsuitable");
                                            }
                                        }
                                        break;
                                    }
                                case (SpellRequirement.ChoosePassableCell):
                                    {
                                        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                        Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
                                        if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                                            mouse.y >= 0 && mouse.y < map.mapSizeY)
                                        {
                                            currentSpellSelection.transform.position = new Vector3(mouse.x + 0.5f, mouse.y + 0.5f);
                                            int path = map.FindDistance(map.cells[x, y], map.cells[mouse.x, mouse.y]);
                                            if (map.cells[mouse.x, mouse.y].isPassable == true &&
                                                (currentSpellToCast.isRanged == false 
                                                || (map.cells[mouse.x, mouse.y].cellObject != null && map.cells[mouse.x, mouse.y].cellObject.GetComponent<PlayerController>() != null) 
                                                || (path >= 0 && path <= currentSpellToCast.spellRange)) && map.CheckCell(mouse.x, mouse.y, CellCheckFlags.ToVisibility))
                                            {
                                                currentSpellSelection.Play("tiles_selection_suitable");
                                                if (Input.GetMouseButtonDown(0))
                                                {
                                                    currentSpellArguments.Add(mouse);
                                                    currentSpellRequirementIndex++;
                                                    currentSpellSelection.gameObject.SetActive(false);
                                                    //currentSpellSelection = Instantiate(currentSpellToCast.);
                                                }
                                            }
                                            else
                                            {
                                                currentSpellSelection.Play("tiles_selection_unsuitable");
                                            }
                                        }
                                        break;
                                    }
                                case (SpellRequirement.ChooseEmptyCell):
                                    {
                                        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                        Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
                                        if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                                            mouse.y >= 0 && mouse.y < map.mapSizeY)
                                        {
                                            currentSpellSelection.transform.position = new Vector3(mouse.x + 0.5f, mouse.y + 0.5f);
                                            int path = map.FindDistance(map.cells[x, y], map.cells[mouse.x, mouse.y]);
                                            if (map.cells[mouse.x, mouse.y].isPassable == true &&
                                                map.cells[mouse.x, mouse.y].cellObject == null &&
                                                (currentSpellToCast.isRanged == false 
                                                || (path >= 0 && path <= currentSpellToCast.spellRange)) && map.CheckCell(mouse.x, mouse.y, CellCheckFlags.ToVisibility))
                                            {
                                                currentSpellSelection.Play("tiles_selection_suitable");
                                                if (Input.GetMouseButtonDown(0))
                                                {
                                                    currentSpellArguments.Add(mouse);
                                                    currentSpellRequirementIndex++;
                                                    currentSpellSelection.gameObject.SetActive(false);
                                                    //currentSpellSelection = Instantiate(currentSpellToCast.);
                                                }
                                            }
                                            else
                                            {
                                                currentSpellSelection.Play("tiles_selection_unsuitable");
                                            }
                                        }
                                        break;
                                    }
                                case (SpellRequirement.ChooseTargetOrSelf):
                                    {
                                        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                        Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
                                        if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                                            mouse.y >= 0 && mouse.y < map.mapSizeY)
                                        {
                                            currentSpellSelection.transform.position = new Vector3(mouse.x + 0.5f, mouse.y + 0.5f);
                                            int path = map.FindDistance(map.cells[x, y], map.cells[mouse.x, mouse.y]);
                                            if (map.cells[mouse.x, mouse.y].isPassable == true
                                            && (currentSpellToCast.isRanged == false || (path >= 0 && path <= currentSpellToCast.spellRange) || (x == mouse.x && y == mouse.y)) && map.CheckCell(mouse.x, mouse.y, CellCheckFlags.ToVisibility)
                                            && map.cells[mouse.x, mouse.y].cellObject != null && map.cells[mouse.x, mouse.y].cellObject.GetComponent<Creature>() != null
                                            && (currentSpellToCast.canBeAppliedOnDeadCreature == true || map.cells[mouse.x, mouse.y].cellObject.GetComponent<Creature>().isAlive == true))
                                            {
                                                currentSpellSelection.Play("tiles_selection_suitable");
                                                if (Input.GetMouseButtonDown(0))
                                                {
                                                    currentSpellArguments.Add(map.cells[mouse.x, mouse.y].cellObject.GetComponent<Creature>());
                                                    currentSpellRequirementIndex++;
                                                    currentSpellSelection.gameObject.SetActive(false);
                                                    //currentSpellSelection = Instantiate(currentSpellToCast.);
                                                }
                                            }
                                            else
                                            {
                                                currentSpellSelection.Play("tiles_selection_unsuitable");
                                            }
                                        }
                                        break;
                                    }
                                case (SpellRequirement.ChooseTarget):
                                    {
                                        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                        Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
                                        if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                                            mouse.y >= 0 && mouse.y < map.mapSizeY)
                                        {
                                            currentSpellSelection.transform.position = new Vector3(mouse.x + 0.5f, mouse.y + 0.5f);
                                            int path = map.FindDistance(map.cells[x, y], map.cells[mouse.x, mouse.y]);
                                            if (map.cells[mouse.x, mouse.y].isPassable == true
                                            && (currentSpellToCast.isRanged == false || (path >= 0 && path <= currentSpellToCast.spellRange)) && map.CheckCell(mouse.x, mouse.y, CellCheckFlags.ToVisibility)
                                            && map.cells[mouse.x, mouse.y].cellObject != null && map.cells[mouse.x, mouse.y].cellObject.GetComponent<Creature>() != null 
                                            && (currentSpellToCast.canBeAppliedOnDeadCreature == true || map.cells[mouse.x, mouse.y].cellObject.GetComponent<Creature>().isAlive == true))
                                            {
                                                currentSpellSelection.Play("tiles_selection_suitable");
                                                if (Input.GetMouseButtonDown(0))
                                                {
                                                    currentSpellArguments.Add(map.cells[mouse.x, mouse.y].cellObject.GetComponent<Creature>());
                                                    currentSpellRequirementIndex++;
                                                    currentSpellSelection.gameObject.SetActive(false);
                                                    //currentSpellSelection = Instantiate(currentSpellToCast.);
                                                }
                                            }
                                            else
                                            {
                                                currentSpellSelection.Play("tiles_selection_unsuitable");
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            if (GetStateValue("cur_energy") > 0)
                            {
                                currentSpellToCast.EmbedRequirements(currentSpellArguments);
                                for (int i = 0; i < gainedSkills.Count; i++)
                                    gainedSkills[i].OnCreatureCastSpell(currentSpellToCast);
                                OnCastSpell(currentSpellToCast);
                                StartCoroutine(currentSpellToCast.SpellAction(this, currentSpellToCast.spellLevel));
                                if (spellSourceItemSlotIndex != -1)
                                    connectedInventory.PlaceItemInSlot(ItemsDatabase.GetItemByID(0), spellSourceItemSlotIndex);
                                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                                spellsCastInThisTurn++;
                                RefreshEnergybar();
                            }
                            DisableSpellMode();
                        }
                    }
                    else
                    {
                        DisableSpellMode();
                    }
                }
            }

            if (Input.GetKeyDown(keyShowScroll))
            {
                RefreshStatsVisual();
                scroll.Reverse();
            }

            if (Input.GetKeyDown(keyShowBestiary))
            {
                bestiaryButton.OnPointerDown(null);
                /*
                if (bestiary.knownCreatures.Count > 0)
                {
                    if (bestiary.gameObject.activeSelf == false)
                    {
                        bestiary.gameObject.SetActive(true);
                        bestiary.LoadCurrentCreature();
                    }
                    else bestiary.gameObject.SetActive(false);
                }
                */
            }
        }

        /*
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mouse = new Vector2Int((int)Math.Truncate(m.x), (int)Math.Truncate(m.y));
            if (mouse.x >= 0 && mouse.x < map.mapSizeX &&
                mouse.y >= 0 && mouse.y < map.mapSizeY)
            {
                print("Position: " + "(" + mouse.x + ":" + mouse.y + ")" + " / " +
                      "Is passable: " + map.cells[mouse.x, mouse.y].isPassable + " / " +
                      "Zone number: " + map.cells[mouse.x, mouse.y].zoneNumber + " / " +
                      "Object: " + (map.cells[mouse.x, mouse.y].cellObject != null ? map.cells[mouse.x, mouse.y].cellObject.ToString() : "null"));
            }
        }
        */
    }

    public int GetExperienceToNextLevel(int _level)
    {
        int result = GetExperienceDiff(_level) + GetExperienceDiff(_level - 1);
        return result;
    }

    public int GetExperienceDiff(int _level)
    {
        int result = _level > 0 ? 120 : 0;
        for (int i = 1; i < _level; i++)
            result = Mathf.RoundToInt(result * (float)Math.Round((2f - i / (i + 3f)), 2));
        return result;
    }

    public void AddExperience(int experience, bool withGainingSkills)
    {
        ChangeStateBonus("exp", experience);
        while (GetStateValue("exp") >= experienceToNextLevel)
        {
            experienceToNextLevel = GetExperienceToNextLevel(GetStateValue("level") + 1);
            SetStateValue("level", GetStateValue("level") + 1);
            statistics.maxLevelThisTime = GetStateValue("level");
            statistics.maxLevelEver = GetStateValue("level") > statistics.maxLevelEver ? GetStateValue("level") : statistics.maxLevelEver;
            // Attack damage increases by 5 every level
            ChangeStateBonus("attack_damage", 5);
            // Max energy increases by 1 every 10 levels
            ChangeStateBonus("max_energy", (GetStateValue("level") % 10 == 0) ? 1 : 0);
            // Max health increases by 20 every level
            ChangeStateBonus("max_health", 20);
            // Protections increase by 1 every 4 levels
            ChangeStateBonus("protection_blunt", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_pricking", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_cutting", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_hewing", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_lashing", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_suffocative", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_scalding", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_freezing", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_electric", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            ChangeStateBonus("protection_poison", (GetStateValue("level") % 4 == 0) ? 1 : 0);
            // Spell power increases by 1 every level
            ChangeStateBonus("spell_power", 1);
            if (withGainingSkills == true)
                levelupPanelController.unusedSkillPoints++;
        }

        if (levelupPanelController.unusedSkillPoints > 0)
        {
            levelupPanelController.ShowSkills();
            levelupPanelController.gameObject.SetActive(true);
        }

        RefreshStatsVisual();
    }

    public void GainSkill(int _skillID, int _heroLevel)
    {
        if (gainedSkills.Any(s => s.skillID == _skillID) == true)
            return;
        Skill skill = SkillsDatabase.GetSkillByID(_skillID);
        skill.player = this;
        skill.OnSkillGain(_heroLevel);
        gainedSkills.Add(skill);
    }

    public override void Move()
    {
        base.Move();
        if (facingDirection == FacingDirection.Up)
        {
            if (y + 1 < map.mapSizeY && map.CheckCell(x, y + 1, CellCheckFlags.ToPassability) && turnIsCompleted == false && GetStateValue("cur_energy") > 0)
            {
                isMoving = true;
                map.cells[x, y].cellObject = null;
                y++;
                //transform.position = new Vector3(x, y, 0);
                map.cells[x, y].cellObject = this;
                StartCoroutine(MoveVisual());
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                RefreshEnergybar();
                PlaySound("move");
                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                    connectedInventory.GetItemBySlotIndex(i).OnMove(this, i);
                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                for (int i = 0; i < gainedSkills.Count; i++)
                    gainedSkills[i].OnCreatureMove();
                OnCreatureMove();
            }
        }
        else if (facingDirection == FacingDirection.Down)
        {
            if (y - 1 >= 0 && map.CheckCell(x, y - 1, CellCheckFlags.ToPassability) && turnIsCompleted == false && GetStateValue("cur_energy") > 0)
            {
                isMoving = true;
                map.cells[x, y].cellObject = null;
                y--;
                //transform.position = new Vector3(x, y, 0);
                map.cells[x, y].cellObject = this;
                StartCoroutine(MoveVisual());
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                RefreshEnergybar();
                PlaySound("move");
                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                    connectedInventory.GetItemBySlotIndex(i).OnMove(this, i);
                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                for (int i = 0; i < gainedSkills.Count; i++)
                    gainedSkills[i].OnCreatureMove();
                OnCreatureMove();
            }
        }
        else if (facingDirection == FacingDirection.Right)
        {
            if (x + 1 < map.mapSizeX && map.CheckCell(x + 1, y, CellCheckFlags.ToPassability) && turnIsCompleted == false && GetStateValue("cur_energy") > 0)
            {
                isMoving = true;
                map.cells[x, y].cellObject = null;
                x++;
                //transform.position = new Vector3(x, y, 0);
                map.cells[x, y].cellObject = this;
                StartCoroutine(MoveVisual());
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                RefreshEnergybar();
                PlaySound("move");
                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                    connectedInventory.GetItemBySlotIndex(i).OnMove(this, i);
                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                for (int i = 0; i < gainedSkills.Count; i++)
                    gainedSkills[i].OnCreatureMove();
                OnCreatureMove();
            }
        }
        else if (facingDirection == FacingDirection.Left)
        {
            if (x - 1 >= 0 && map.CheckCell(x - 1, y, CellCheckFlags.ToPassability) && turnIsCompleted == false && GetStateValue("cur_energy") > 0)
            {
                isMoving = true;
                map.cells[x, y].cellObject = null;
                x--;
                //transform.position = new Vector3(x, y, 0);
                map.cells[x, y].cellObject = this;
                StartCoroutine(MoveVisual());
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
                RefreshEnergybar();
                PlaySound("move");
                for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                    connectedInventory.GetItemBySlotIndex(i).OnMove(this, i);
                map.RecalculateFogOfWar(map.cells[x, y], GetStateValue("view_dist"));
                for (int i = 0; i < gainedSkills.Count; i++)
                    gainedSkills[i].OnCreatureMove();
                OnCreatureMove();
            }
        }
    }

    public override void Attack()
    {
        Creature creatureToAttack = GetCreatureInFront();
        if (creatureToAttack != null && creatureToAttack.isAlive == true)
        {
            statistics.damageDealtThisTime += GetStateValue("attack_damage") > 0 ? GetStateValue("attack_damage") : 1;
            statistics.damageDealtTotal += GetStateValue("attack_damage") > 0 ? GetStateValue("attack_damage") : 1;
            if (UnityEngine.Random.Range(0f, 100f) < GetStateValue("crit_chance"))
            {
                criticalStrikesInThisTurn++;
                creatureToAttack.GetDamage(GetStateValue("attack_damage"), autoattackDamageType, this);
                //if (gainedSkills.Any(s => s.skillID == skill_frenzy_ID))
                    SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            }
            else
            {
                creatureToAttack.GetDamage(Mathf.RoundToInt(GetStateValue("attack_damage") * (1 + GetStateValue("crit_damage_percent") * 0.01f)), autoattackDamageType, this);
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            }
            if (creatureToAttack.isAlive == false && 
                creatureToAttack.GetComponent<CreatureFriendlyAwakenedSpirit>() == null && 
                creatureToAttack.GetComponent<Destructible>() == null)
                creaturesKilledInThisTurnPositions.Add(new Vector2Int(creatureToAttack.x, creatureToAttack.y));
            PlaySound("attack");
            for (int i = 0; i < gainedSkills.Count; i++)
                gainedSkills[i].OnCreatureAttack(creatureToAttack, autoattackDamageType, GetStateValue("attack_damage"));
            for (int i = 0; i < connectedInventory.inventorySlotsCount; i++)
                connectedInventory.GetItemBySlotIndex(i).OnAttack(this, creatureToAttack, i);
        }
        /*
        if (facingDirection == FacingDirection.Up)
        {
            if (map.CheckCell(x, y + 1, CellCheckFlags.ToExistance) == true
                && map.cells[x, y + 1].cellObject != null
                && map.cells[x, y + 1].cellObject.GetComponent<Creature>() != null)
            {
                creatureToAttack = map.cells[x, y + 1].cellObject.GetComponent<Creature>();
                creatureToAttack.GetDamage(GetStateValue("attack_damage"), autoattackDamageType, this);
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            }
        }
        else if (facingDirection == FacingDirection.Down)
        {
            if (map.CheckCell(x, y - 1, CellCheckFlags.ToExistance) == true
                && map.cells[x, y - 1].cellObject != null
                && map.cells[x, y - 1].cellObject.GetComponent<Creature>() != null)
            {
                creatureToAttack = map.cells[x, y - 1].cellObject.GetComponent<Creature>();
                creatureToAttack.GetDamage(GetStateValue("attack_damage"), autoattackDamageType, this);
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            }
        }
        else if (facingDirection == FacingDirection.Right)
        {
            if (map.CheckCell(x + 1, y, CellCheckFlags.ToExistance) == true
                && map.cells[x + 1, y].cellObject != null
                && map.cells[x + 1, y].cellObject.GetComponent<Creature>() != null)
            {
                creatureToAttack = map.cells[x + 1, y].cellObject.GetComponent<Creature>();
                creatureToAttack.GetDamage(GetStateValue("attack_damage"), autoattackDamageType, this);
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            }
        }
        else if (facingDirection == FacingDirection.Left)
        {
            if (map.CheckCell(x - 1, y, CellCheckFlags.ToExistance) == true
                && map.cells[x - 1, y].cellObject != null
                && map.cells[x - 1, y].cellObject.GetComponent<Creature>() != null)
            {
                creatureToAttack = map.cells[x - 1, y].cellObject.GetComponent<Creature>();
                creatureToAttack.GetDamage(GetStateValue("attack_damage"), autoattackDamageType, this);
                SetStateValue("cur_energy", GetStateValue("cur_energy") - 1);
            }
        }
        */

        RefreshEnergybar();
        /*
        if (creatureToAttack != null)
            print(gameObject + " attacked " + creatureToAttack + ";\n" + creatureToAttack + " now have " + creatureToAttack.currentHealth);
            */
    }

    public void OnEffectBegin(Effect effect)
    {
        EffectIcon icon = Instantiate(effectIconExemplar);
        icon.icon.sprite = effect.effectIcon;
        icon.frame.sprite = 
            effect.effectType == EffectType.Buff 
            ? icon.possibleFrames[0] 
            : icon.possibleFrames[1];
        icon.durationText.text = effect.effectDuration.ToString();
        icon.effectLevel = effect.effectLevel;
        icon.iconsController = iconsController;
        icon.effectName = effect.effectName;
        icon.effectDescription = effect.TranslateEffectDescription(effect.effectName + "_desc");
        icon.transform.SetParent(effectIconsContainer.transform);
        effect.connectedIcon = icon;
    }

    public void OnEffectEnd(Effect effect)
    {
        if (effect.connectedIcon != null)
            Destroy(effect.connectedIcon.gameObject);
        effect.connectedIcon = null;
        iconsController.gameObject.SetActive(false);
    }

    public override void OnTurnStart()
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (canBeAffectedByEffects == true)
                currentEffects[i].OnTurnStart();
            currentEffects[i].effectDuration--;
            if (currentEffects[i].connectedIcon != null)
                currentEffects[i].connectedIcon.durationText.text = (currentEffects[i].effectDuration + 1).ToString();
            if (currentEffects[i].effectDuration < 0)
            {
                OnEffectEnd(currentEffects[i]);
                if (canBeAffectedByEffects == true)
                    currentEffects[i].OnEffectEnd();
                currentEffects[i].hasEnded = true;
            }
        }

        for (int i = 0; i < gainedSkills.Count; i++)
            gainedSkills[i].OnTurnStart();

        criticalStrikesInThisTurn = 0;
        spellsCastInThisTurn = 0;
        creaturesKilledInThisTurnPositions.Clear();
        currentEffects = currentEffects.Where(e => e.hasEnded == false).ToList();
        SetStateValue("cur_energy", GetStateValue("max_energy"));
        OnTurnStartLate();
        RefreshEnergybar();
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
        for (int i = 0; i < gainedSkills.Count; i++)
            gainedSkills[i].OnTurnEnd();
    }

    /// <summary>
    /// Is calling when all effects' and skills' actions were committed.
    /// </summary>
    public void OnTurnStartLate()
    {
        for (int i = 0; i < gainedSkills.Count; i++)
            gainedSkills[i].OnTurnStartLate();
    }

    public void ShowPath(MapCell[] _path)
    {
        for (int i = 0; i < _path.Length; i++)
        {
            TextMesh t = Instantiate(pathMark, new Vector3(_path[i].x + 0.5f, _path[i].y + 0.5f, -1f), Quaternion.identity);
            t.text = (_path.Length - i).ToString();
            pathMarks.Add(t);
        }
    }

    private IEnumerator MoveVisual()
    {
        animator.SetBool("isMoving", true);
        PlayWalkAnimation();
        while (Vector3.Distance(transform.position, new Vector3(x, y)) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, y), 2f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(x, y);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isMoving", false);
        map.finishLevelWindow.SetActive((map.levelExitPosition == new Vector2Int(x, y)));
        if (map.signsPositions != null)
            for (int i = 0; i < map.signsPositions.Length; i++)
                if (map.signsPositions[i] == new Vector2Int(x, y))
                    map.ShowNotification(map.signsTextIDs[i]);
        //PlayIdleAnimation(facingDirection);
        isMoving = false;
    }

    public override void PlayIdleAnimation()
    {
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play("hero_idle_right");
        }
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play("hero_idle_left");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play("hero_idle_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play("hero_idle_down");
        }
    }
    public override void PlayWalkAnimation()
    {
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play("hero_walk_right");
        }
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play("hero_walk_left");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play("hero_walk_up");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play("hero_walk_down");
        }
    }
    public override void PlayAttackAnimation()
    {

    }
    public override void PlayDeathAnimation()
    {
        if (facingDirection == FacingDirection.Left)
        {
            animator.Play(creatureNameID + "_left_death");
        }
        if (facingDirection == FacingDirection.Right)
        {
            animator.Play(creatureNameID + "_right_death");
        }
        if (facingDirection == FacingDirection.Up)
        {
            animator.Play(creatureNameID + "_up_death");
        }
        if (facingDirection == FacingDirection.Down)
        {
            animator.Play(creatureNameID + "_down_death");
        }
    }
    public override void PlaySpawnAnimation()
    {

    }

    public void Die(Creature causedBy)
    {
        statistics.deathsCount++;
        SetStateValue("cur_health", 0);
        RefreshHealthbar();
        isAlive = false;
        visualInventory.SetActive(false);
        lootInventory.SetActive(false);
        bestiary.gameObject.SetActive(false);
        for (int i = 0; i < gainedSkills.Count; i++)
            gainedSkills[i].OnCreatureDeath();
        OnDeath();
        PlayDeathAnimation();
        statistics.SaveStatistics();
        deathMenu.ShowStatistics();
        StartCoroutine(VisualDeath());
    }

    public IEnumerator VisualDeath()
    {
        yield return new WaitForSeconds(1.0f);
        deathMenu.gameObject.SetActive(true);
    }

    public override void OnCreatureKill(Creature creature)
    {
        base.OnCreatureKill(creature);
        for (int i = 0; i < gainedSkills.Count; i++)
            gainedSkills[i].OnCreatureKill(creature);
        statistics.killedEnemiesThisTime++;
        statistics.killedEnemiesTotal++;
    }

    public override void GetDamage(int _damage, DamageType damageType, Creature sender)
    {
        if (isAlive == true) // important condition
        {
            //_damage = Mathf.RoundToInt(_damage * Mathf.Pow(GetStateValue("protection_" + damageType.ToString()), -(GetStateValue("protection" + damageType.ToString()) / _damage)));
            if (GetStateValue("protection_" + damageType.ToString()) > 0)
                _damage = Mathf.RoundToInt(75 * _damage / (GetStateValue("protection_" + damageType.ToString()) + 75));
            else _damage += -GetStateValue("protection_" + damageType.ToString()) * 15;
            if (_damage < 1)
                _damage = 1;
            int damageReduction = OnDamage(sender, damageType, _damage);
            if (damageReduction >= _damage)
                return;
            _damage -= damageReduction;
            for (int i = 0; i < gainedSkills.Count; i++)
                gainedSkills[i].OnCreatureDamage(sender, damageType, _damage);
            SetStateValue("cur_health", (int)Mathf.MoveTowards(GetStateValue("cur_health"), 0, _damage));
            statistics.damageReceivedThisTime += _damage;
            statistics.damageReceivedTotal += _damage;
            RefreshHealthbar();
            if (GetStateValue("cur_health") <= 0)
            {
                Die(sender);
            }
        }
    }
    public override void GetHealing(int _healing)
    {
        for (int i = 0; i < gainedSkills.Count; i++)
            gainedSkills[i].OnCreatureHeal(_healing);
        OnHeal();
        statistics.healingReceivedThisTime += _healing;
        statistics.healingReceivedTotal += _healing;
        SetStateValue("cur_health", (int)Mathf.MoveTowards(GetStateValue("cur_health"), GetStateValue("max_health"), _healing));
        RefreshHealthbar();
    }

    public override int OnDamage(Creature _sender, DamageType _damageType, int _damage)
    {
        int result = base.OnDamage(_sender, _damageType, _damage);
        PlaySound("get_damage");
        return result;
    }

    public void AddCreatureToBestiary(Creature creatureToAdd)
    {
        if (creatureToAdd.canBeAddedToBestiary == false)
            return;
        /*
        CreatureInformation information = new CreatureInformation(
            creatureToAdd.creatureNameID,
            creatureToAdd.traitsCount,
            creatureToAdd.GetStateValue("max_health"),
            creatureToAdd.GetStateValue("max_energy"),
            creatureToAdd.GetStateValue("attack_damage"),
            creatureToAdd.autoattackDamageType,
            new int[10]
            {
                creatureToAdd.GetStateValue("protection_blunt"),
                creatureToAdd.GetStateValue("protection_pricking"),
                creatureToAdd.GetStateValue("protection_cutting"),
                creatureToAdd.GetStateValue("protection_hewing"),
                creatureToAdd.GetStateValue("protection_lashing"),
                creatureToAdd.GetStateValue("protection_suffocative"),
                creatureToAdd.GetStateValue("protection_scalding"),
                creatureToAdd.GetStateValue("protection_freezing"),
                creatureToAdd.GetStateValue("protection_electric"),
                creatureToAdd.GetStateValue("protection_poison"),
            },
            creatureToAdd.creatureRaces.Length,
            creatureToAdd.creatureRaces);
            */
        CreatureInformation _information = creatureToAdd.information;
        if (bestiary.knownCreatures.Any(i => i.creatureNameID == _information.creatureNameID && i.creatureLevel == _information.creatureLevel) == false)
        {
            bestiary.knownCreatures.Add(_information);
        }
    }

    new public void RefreshHealthbar()
    {
        playerHealthbar.rectTransform.sizeDelta = new Vector2((float)GetStateValue("cur_health") / GetStateValue("max_health") * 100f, playerHealthbar.rectTransform.sizeDelta.y);
        if (playerHealthbar.rectTransform.sizeDelta.x > 100)
            playerHealthbar.rectTransform.sizeDelta = new Vector2(100, playerHealthbar.rectTransform.sizeDelta.y);
        healthText.text = GetStateValue("cur_health").ToString() + " / " + GetStateValue("max_health").ToString();
    }

    public void RefreshEnergybar()
    {
        energybar.rectTransform.sizeDelta = new Vector2((float)GetStateValue("cur_energy") / GetStateValue("max_energy") * 100f, energybar.rectTransform.sizeDelta.y);
        if (energybar.rectTransform.sizeDelta.x > 100)
            energybar.rectTransform.sizeDelta = new Vector2(100, energybar.rectTransform.sizeDelta.y);
        energyText.text = GetStateValue("cur_energy").ToString() + " / " + GetStateValue("max_energy").ToString();
    }

    public void RefreshXPbar()
    {
        xpbar.rectTransform.sizeDelta = new Vector2(((float)GetStateValue("exp") - GetExperienceToNextLevel(GetStateValue("level") - 1)) / (experienceToNextLevel - GetExperienceToNextLevel(GetStateValue("level") - 1)) * 100f, xpbar.rectTransform.sizeDelta.y);
        if (xpbar.rectTransform.sizeDelta.x > 100f)
            xpbar.rectTransform.sizeDelta = new Vector2(100f, xpbar.rectTransform.sizeDelta.y);
        xpText.text = GetStateValue("exp").ToString() + " / " + experienceToNextLevel.ToString();
    }

    public void RefreshStatsVisual()
    {
        RefreshHealthbar();
        RefreshEnergybar();
        RefreshXPbar();
        for (int i = 0; i < protectionTexts.Length; i++)
            protectionTexts[i].text = GetStateValue("protection_" + Enum.GetName(typeof(DamageType), i)).ToString();
        attackDamageText.text = GetStateValue("attack_damage").ToString();
        attackTypeText.text = Translate.TranslateText(autoattackDamageType.ToString() + "_attack_type");
        spellPowerText.text = GetStateValue("spell_power").ToString();
        nameText.text = creatureName + " (lvl " + GetStateValue("level") + ")";
    }

    /*
    public void SaveGame()
    {
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        XmlNode userNode;
        XmlElement element;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("save");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("Creatures");
        {
            element = xmlDoc.CreateElement(creatureNameID);
            element.SetAttribute("value", ToString());
            userNode.AppendChild(element);

            for (int i = 0; i < map.creaturesToMove.Count; i++)
            {
                element = xmlDoc.CreateElement(map.creaturesToMove[i].creatureNameID);
                element.SetAttribute("value", map.creaturesToMove[i].ToString());
                userNode.AppendChild(element);
            }
        }

        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/save.xml");
    }

    public void LoadGame()
    {
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        if (File.Exists(Application.dataPath + "/Saves/save.xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/save.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == creatureNameID)
                        FromString(childnode.Attributes.GetNamedItem("value").Value);
                    else if (isLevelLoadedNotFromFile == false)
                    {
                        Creature creature = Instantiate(creaturesContainer.GetCreatureByName(childnode.Name));
                        creature.map = map;
                        creature.FromString(childnode.Attributes.GetNamedItem("value").Value);
                    }
                }
            }
        }
    }
    */

    public override string ToString()
    {
        return map.levelKey + ";"
                + creatureName.ToString() + ";"
                + x + ";"
                + y + ";"
                //+ GetStateValue("level").ToString() + ";"
                //+ GetStateValue("cur_health").ToString() + ";"
                //+ GetStateValue("max_health").ToString() + ";"
                //+ GetStateValue("cur_energy").ToString() + ";"
                //+ GetStateValue("max_energy").ToString() + ";"
                //+ GetStateValue("protection").ToString() + ";"
                //+ GetStateValue("attack_damage").ToString() + ";"
                + ((int)facingDirection).ToString() + ";"
                + ((int)autoattackDamageType).ToString() + ";"
                + cellObjectID.ToString() + ";"
                + GetStateValue("level").ToString();
    }

    public override void FromString(string str)
    {
        string[] s = str.Split(';');
        cellObjectID = int.Parse(s[6]);
        creatureName = s[1];
        autoattackDamageType = (DamageType)int.Parse(s[5]);
        experienceToNextLevel = 10;
        //SetStateValue("level", int.Parse(s[4]));
        //SetStateValue("max_health", int.Parse(s[6]));
        //SetStateValue("max_energy", int.Parse(s[8]));
        //SetStateValue("protection", int.Parse(s[9]));
        //SetStateValue("attack_damage", int.Parse(s[10]));
        if (map.CheckLevelKey(s[0]) == false)
        {
            map.isLevelLoadedNotFromFile = true;
            x = map.playerStartPosition.x;
            y = map.playerStartPosition.y;
            //SetStateValue("cur_energy", GetStateValue("max_energy"));
            //SetStateValue("cur_health", GetStateValue("max_health"));
            facingDirection = FacingDirection.Down;
        }
        else
        {
            x = int.Parse(s[2]);
            y = int.Parse(s[3]);
            //SetStateValue("cur_energy", int.Parse(s[7]));
            //SetStateValue("cur_health", int.Parse(s[5]));
            facingDirection = (FacingDirection)int.Parse(s[4]);
            PlayIdleAnimation();
        }

        //RecalculateStates();
    }
}