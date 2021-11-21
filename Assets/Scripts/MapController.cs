using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class MapCell
{
    public int x, y;
    public bool isPassable;
    public int pathNumber = -1; // -1 means unnumbered value
    public int zoneNumber = -1;
    public int pathCost = 1;
    public CellObject cellObject;

    public MapCell(int _x, int _y, bool _passable)
    {
        x = _x;
        y = _y;
        isPassable = _passable;
    }
}

public enum CellCheckFlags
{
    ToExistance,
    ToPassability,
    ToZone,
    ToPathNumber,
    ToVisibility
}

public enum FacingDirection
{
    Up,
    Right,
    Down,
    Left
}

public class MapController : MonoBehaviour
{
    // Level name itself to identify whether player is loading this level
    public string levelKey;
    public int currentLevelIndex;
    public CreaturesContainer creaturesContainer;

    public MapCell[,] cells;
    public Tilemap cellsColliders;

    public Tilemap fogOfWar;
    public TileBase tileClearFogOfWar;
    public List<MapCell> clearedCellsFogOfWar;

    public int mapSizeX, mapSizeY;
    public SpriteMask levelMaskExemplar;

    public GameObject finishLevelWindow;
    public Vector2Int levelExitPosition;
    public Vector2Int playerStartPosition;

    // index in positions' list must match with index in texts' list
    public Vector2Int[] signsPositions;
    public string[] signsTextIDs;

    public PlayerController player;
    public List<Creature> creaturesToMove;
    public int currentCreatureIndex;
    public List<Creature> creaturesToDestroy;

    public bool isLevelLoadedNotFromFile;

    private int currentMaxCellObjectID;

    public bool CheckLevelKey(string inputKey)
    {
        // player is loading this level from file and it have to load creatures from player's save file
        if (inputKey == levelKey)
        {
            for (int i = 0; i < creaturesToMove.Count; i++)
            {
                if (creaturesToMove[i] != null)
                {
                    creaturesToMove[i].isDestroyed = true;
                    Destroy(creaturesToMove[i].gameObject);
                }
            }
            creaturesToMove.Clear();
            return true;
        }
        // player is loading this level first time or once again (not from file) and creatures on this level will be placed by standard
        else
        {
            return false;
        }
    }
    
    public void InitializeMap()
    {
        SpriteMask mask = Instantiate(levelMaskExemplar);
        mask.transform.position = new Vector3(0.5f * (mapSizeX + 1), 0.5f * (mapSizeY + 1));
        mask.transform.localScale = new Vector3(mapSizeX + 1, mapSizeY + 1);

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                cells[x, y] = new MapCell(x, y, !cellsColliders.HasTile(new Vector3Int(x, y, 0)));
            }
        }

        SeparateZones();
        creaturesToMove = FindObjectsOfType<Creature>().Where(c => c.isDestroyed == false).ToList();
        Creature player = creaturesToMove.Find(c => c.GetComponent<PlayerController>() != null);
        creaturesToMove.Remove(player);
        creaturesToMove.Insert(0, player);

        for (int i = 0; i < creaturesToMove.Count; i++)
        {
            if (creaturesToMove[i].cellObjectID == -1)
            {
                creaturesToMove[i].cellObjectID = currentMaxCellObjectID;
                currentMaxCellObjectID++;
            }
            creaturesToMove[i].map = this;
            cells[creaturesToMove[i].x, creaturesToMove[i].y].cellObject = creaturesToMove[i];
            creaturesToMove[i].turnIsCompleted = true;
            creaturesToMove[i].CorrelateLevelWithStats();
            creaturesToMove[i].RecalculateStates();
            if (creaturesToMove[i].levelText != null)
                if (creaturesToMove[i].showLevelText == true)
                    creaturesToMove[i].levelText.text = creaturesToMove[i].GetStateValue("level").ToString();
                else creaturesToMove[i].levelText.gameObject.SetActive(false);
            creaturesToMove[i].SetCreatureInformation();
        }

        if (isLevelLoadedNotFromFile == false)
            ReapplyEffects();
        creaturesToMove[currentCreatureIndex].turnIsCompleted = false;
        creaturesToMove[currentCreatureIndex].OnTurnStart();
        creaturesToMove[currentCreatureIndex].connectedInventory.InventoryTick();

        if (File.Exists(Application.dataPath + "/Saves/fog.xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/fog.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "cell")
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("value") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                clearedCellsFogOfWar.Add(cells[int.Parse(s[0]), int.Parse(s[1])]);
                            }
                        }
                    }
                }
            }
        }

        MapCell cell;
        for (int i = 0; i < clearedCellsFogOfWar.Count; i++)
        {
            cell = clearedCellsFogOfWar[i];
            fogOfWar.SetTile(new Vector3Int(cell.x, cell.y, 0), tileClearFogOfWar);

            // light the walls too
            if (cell.y + 1 < mapSizeY && cells[cell.x, cell.y + 1] != null && cells[cell.x, cell.y + 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x, cell.y + 1, 0), tileClearFogOfWar);
            if (cell.y - 1 >= 0 && cells[cell.x, cell.y - 1] != null && cells[cell.x, cell.y - 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x, cell.y - 1, 0), tileClearFogOfWar);
            if (cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y] != null && cells[cell.x + 1, cell.y].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x + 1, cell.y, 0), tileClearFogOfWar);
            if (cell.x - 1 >= 0 && cells[cell.x - 1, cell.y] != null && cells[cell.x - 1, cell.y].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x - 1, cell.y, 0), tileClearFogOfWar);

            if (cell.y + 1 < mapSizeY && cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y + 1] != null
                && cells[cell.x + 1, cell.y + 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x + 1, cell.y + 1, 0), tileClearFogOfWar);
            if (cell.y + 1 < mapSizeY && cell.x - 1 >= 0 && cells[cell.x - 1, cell.y + 1] != null
                 && cells[cell.x - 1, cell.y + 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x - 1, cell.y + 1, 0), tileClearFogOfWar);
            if (cell.y - 1 >= 0 && cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y - 1] != null
                && cells[cell.x + 1, cell.y - 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x + 1, cell.y - 1, 0), tileClearFogOfWar);
            if (cell.y - 1 >= 0 && cell.x - 1 >= 0 && cells[cell.x - 1, cell.y - 1] != null
                && cells[cell.x - 1, cell.y - 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x - 1, cell.y - 1, 0), tileClearFogOfWar);
        }
    }

    public void NextTurn()
    {
        if (creaturesToMove[currentCreatureIndex].turnIsCompleted == false)
            return;
        creaturesToMove[currentCreatureIndex].OnTurnEnd();
        for (int i = 0; i < creaturesToMove.Count; i++)
        {
            if (creaturesToMove[i] == null)
            {
                creaturesToMove.RemoveAt(i);
                continue;
            }
            if (creaturesToMove[i].isAlive == false && creaturesToMove[i].creatureNameID != "hero" && creaturesToDestroy.Contains(creaturesToMove[i]) == false)
                DestroyCreature(creaturesToMove[i]);
        }

        currentCreatureIndex++;
        while (currentCreatureIndex <= creaturesToMove.Count - 1 && creaturesToMove[currentCreatureIndex] != null && creaturesToMove[currentCreatureIndex].isAlive == false)
        {
            currentCreatureIndex++;
        }

        if (currentCreatureIndex > creaturesToMove.Count - 1)
        {
            for (int i = 0; i < creaturesToDestroy.Count; i++)
            {
                creaturesToMove.Remove(creaturesToMove.Find(c => c.x == creaturesToDestroy[i].x && c.y == creaturesToDestroy[i].y && c.creatureNameID != "hero" && c.isAlive == false));
                if (creaturesToDestroy[i] != null)
                    Destroy(creaturesToDestroy[i].gameObject);
            }
            creaturesToDestroy.Clear();
            currentCreatureIndex = 0;
            player.bestiary.SaveBestiary();
            creaturesToMove[currentCreatureIndex].turnIsCompleted = false;
            creaturesToMove[currentCreatureIndex].OnTurnStart();
            //SaveGame();
        }
        else
        {
            creaturesToMove[currentCreatureIndex].turnIsCompleted = false;
            creaturesToMove[currentCreatureIndex].OnTurnStart();
        }

        creaturesToMove[currentCreatureIndex].connectedInventory.InventoryTick();
    }

    public Creature SpawnCreature(Creature creatureToSpawn, int _x, int _y)
    {
        Creature creature = Instantiate(creatureToSpawn);
        creature.x = _x;
        creature.y = _y;
        creature.map = this;
        creature.transform.position = new Vector3(_x, _y);
        creature.RecalculateStates();
        creature.OnSpawn();
        creature.connectedInventory.Initialize();
        creature.Initialize();
        
        creaturesToMove.Add(creature);
        cells[_x, _y].cellObject = creature;

        return creature;
    }

    public void DestroyCreature(Creature creatureToDestroy)
    {
        creaturesToDestroy.Add(creatureToDestroy);
        //creaturesToMove.Remove(creaturesToMove.Find(c => c.x == creatureToDestroy.x && c.y == creatureToDestroy.y));
        //Destroy(creatureToDestroy.gameObject);
    }

    public void ShowNotification(string notificationTextID, params string[] args)
    {
        string result = Translate.TranslateText(notificationTextID).Replace("@", System.Environment.NewLine);
        while (result.Contains("{") && result.Contains("}"))
        {
            int firstBracket = result.IndexOf("{") + 1;
            int secondBracket = result.IndexOf("}") + 1;
            int number = int.Parse(result.Substring(firstBracket, secondBracket - firstBracket - 1));
            if (number >= args.Length)
            {
                Debug.LogWarning("Failed to show notification: " + notificationTextID);
                return;
            }
            result = result.Replace(result.Substring(firstBracket - 1, secondBracket - firstBracket + 1), args[number]);
        }
        player.notificationText.text = result;
        player.notificationPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, player.notificationText.preferredHeight + 150);
        player.notificationPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Returns true if cell is corresponding to condition.
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool CheckCell(int _x, int _y, CellCheckFlags flag)
    {
        bool result = false;
        if (_x < mapSizeX && _x >= 0 &&
            _y < mapSizeY && _y >= 0)
        {
            switch (flag)
            {
                case CellCheckFlags.ToExistance:
                    {
                        if (cells[_x, _y] != null)
                            result = true;
                        break;
                    }

                case CellCheckFlags.ToZone:
                    {
                        if (cells[_x, _y] != null && cells[_x, _y].isPassable == true && cells[_x, _y].zoneNumber == -1)
                            result = true;
                        break;
                    }

                case CellCheckFlags.ToPassability:
                    {
                        if (cells[_x, _y] != null && cells[_x, _y].isPassable == true && (cells[_x, _y].cellObject == null || (cells[_x, _y].cellObject != null && cells[_x, _y].cellObject.GetComponent<Creature>() != null && cells[_x, _y].cellObject.GetComponent<Creature>().isAlive == false)))
                            result = true;
                        break;
                    }

                case CellCheckFlags.ToVisibility:
                    {
                        if (cells[_x, _y] != null && clearedCellsFogOfWar.Any(c => c.x == _x && c.y == _y) == true)
                            result = true;
                        break;
                    }
            }
        }

        return result;
    }
    /// <summary>
    /// Returns true if at least one of cell's neighbors is corresponding to condition.
    /// </summary>
    /// <param name="_cell"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool CheckNeighbors(MapCell _cell, CellCheckFlags flag)
    {
        bool result = true;
        switch (flag)
        {
            case CellCheckFlags.ToExistance:
                {
                    if ((_cell.y + 1 < mapSizeY && cells[_cell.x, _cell.y + 1] == null) &&
                        (_cell.y - 1 >= 0 && cells[_cell.x, _cell.y - 1] == null) &&
                        (_cell.x + 1 < mapSizeX && cells[_cell.x + 1, _cell.y] == null) &&
                        (_cell.x - 1 >= 0 && cells[_cell.x - 1, _cell.y] == null))
                        result = false;
                    break;
                }

            case CellCheckFlags.ToZone:
                {
                    if ((_cell.y + 1 < mapSizeY && cells[_cell.x, _cell.y + 1] != null && cells[_cell.x, _cell.y + 1].isPassable == true && cells[_cell.x, _cell.y + 1].zoneNumber == -1) ||
                        (_cell.y - 1 >= 0 && cells[_cell.x, _cell.y - 1] != null && cells[_cell.x, _cell.y - 1].isPassable == true && cells[_cell.x, _cell.y - 1].zoneNumber == -1) ||
                        (_cell.x + 1 < mapSizeX && cells[_cell.x + 1, _cell.y] != null && cells[_cell.x + 1, _cell.y].isPassable == true && cells[_cell.x + 1, _cell.y].zoneNumber == -1) ||
                        (_cell.x - 1 >= 0 && cells[_cell.x - 1, _cell.y] != null && cells[_cell.x - 1, _cell.y].isPassable == true && cells[_cell.x - 1, _cell.y].zoneNumber == -1))
                        result = false;
                    break;
                }

            case CellCheckFlags.ToPassability:
                {
                    if ((_cell.y + 1 < mapSizeY && cells[_cell.x, _cell.y + 1] != null && cells[_cell.x, _cell.y + 1].isPassable == true && (cells[_cell.x, _cell.y + 1].cellObject == null || (cells[_cell.x, _cell.y + 1].cellObject != null && cells[_cell.x, _cell.y + 1].cellObject.GetComponent<Creature>() != null && cells[_cell.x, _cell.y + 1].cellObject.GetComponent<Creature>().isAlive == false))) ||
                        (_cell.y - 1 >= 0 && cells[_cell.x, _cell.y - 1] != null && cells[_cell.x, _cell.y - 1].isPassable == true && (cells[_cell.x, _cell.y - 1].cellObject == null || (cells[_cell.x, _cell.y - 1].cellObject != null && cells[_cell.x, _cell.y - 1].cellObject.GetComponent<Creature>() != null && cells[_cell.x, _cell.y - 1].cellObject.GetComponent<Creature>().isAlive == false))) ||
                        (_cell.x + 1 < mapSizeX && cells[_cell.x + 1, _cell.y] != null && cells[_cell.x + 1, _cell.y].isPassable == true && (cells[_cell.x + 1, _cell.y].cellObject == null || (cells[_cell.x + 1, _cell.y].cellObject != null && cells[_cell.x + 1, _cell.y].cellObject.GetComponent<Creature>() != null && cells[_cell.x + 1, _cell.y].cellObject.GetComponent<Creature>().isAlive == false))) ||
                        (_cell.x - 1 >= 0 && cells[_cell.x - 1, _cell.y] != null && cells[_cell.x - 1, _cell.y].isPassable == true && (cells[_cell.x - 1, _cell.y].cellObject == null || (cells[_cell.x - 1, _cell.y].cellObject != null && cells[_cell.x - 1, _cell.y].cellObject.GetComponent<Creature>() != null && cells[_cell.x - 1, _cell.y].cellObject.GetComponent<Creature>().isAlive == false))))
                        result = false;
                    break;
                }

            case CellCheckFlags.ToPathNumber:
                {
                    if ((_cell.y + 1 < mapSizeY && cells[_cell.x, _cell.y + 1] != null && cells[_cell.x, _cell.y + 1].isPassable == true /* && cells[_cell.x, _cell.y + 1].cellObject == null */ && cells[_cell.x, _cell.y + 1].pathNumber == -1) ||
                        (_cell.y - 1 >= 0 && cells[_cell.x, _cell.y - 1] != null && cells[_cell.x, _cell.y - 1].isPassable == true /* && cells[_cell.x, _cell.y - 1].cellObject == null */ && cells[_cell.x, _cell.y - 1].pathNumber == -1) ||
                        (_cell.x + 1 < mapSizeX && cells[_cell.x + 1, _cell.y] != null && cells[_cell.x + 1, _cell.y].isPassable == true /* && cells[_cell.x + 1, _cell.y].cellObject == null */ && cells[_cell.x + 1, _cell.y].pathNumber == -1) ||
                        (_cell.x - 1 >= 0 && cells[_cell.x - 1, _cell.y] != null && cells[_cell.x - 1, _cell.y].isPassable == true /* && cells[_cell.x - 1, _cell.y].cellObject == null */ && cells[_cell.x - 1, _cell.y].pathNumber == -1))
                        result = false;
                    break;
                }
        }

        return result;
    }
    /// <summary>
    /// Returns true if at least one of neighbors of any cell from list is corresponding to condition.
    /// </summary>
    /// <param name="_cell"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool CheckNeighbors(List<MapCell> _cells, CellCheckFlags flag)
    {
        bool result = true;
        switch (flag)
        {
            case CellCheckFlags.ToExistance:
                {
                    for (int i = 0; i < _cells.Count; i++)
                    {
                        if ((_cells[i].y + 1 < mapSizeY && cells[_cells[i].x, _cells[i].y + 1] == null) &&
                            (_cells[i].y - 1 >= 0 && cells[_cells[i].x, _cells[i].y - 1] == null) &&
                            (_cells[i].x + 1 < mapSizeX && cells[_cells[i].x + 1, _cells[i].y] == null) &&
                            (_cells[i].x - 1 >= 0 && cells[_cells[i].x - 1, _cells[i].y] == null))
                            result = false;
                    }

                    break;
                }

            case CellCheckFlags.ToZone:
                {
                    for (int i = 0; i < _cells.Count; i++)
                    {
                        if ((_cells[i].y + 1 < mapSizeY && cells[_cells[i].x, _cells[i].y + 1] != null && cells[_cells[i].x, _cells[i].y + 1].isPassable == true && cells[_cells[i].x, _cells[i].y + 1].zoneNumber == -1) ||
                        (_cells[i].y - 1 >= 0 && cells[_cells[i].x, _cells[i].y - 1] != null && cells[_cells[i].x, _cells[i].y - 1].isPassable == true && cells[_cells[i].x, _cells[i].y - 1].zoneNumber == -1) ||
                        (_cells[i].x + 1 < mapSizeX && cells[_cells[i].x + 1, _cells[i].y] != null && cells[_cells[i].x + 1, _cells[i].y].isPassable == true && cells[_cells[i].x + 1, _cells[i].y].zoneNumber == -1) ||
                        (_cells[i].x - 1 >= 0 && cells[_cells[i].x - 1, _cells[i].y] != null && cells[_cells[i].x - 1, _cells[i].y].isPassable == true && cells[_cells[i].x - 1, _cells[i].y].zoneNumber == -1))
                            result = false;
                    }
                    break;
                }
        }

        return result;
    }
    /// <summary>
    /// Separates unlinked cells by giving them different zone numbers.
    /// </summary>
    public void SeparateZones()
    {
        int zoneNumber = 0;
        MapCell f = cells.Cast<MapCell>().ToList().Find(m => m != null && m.zoneNumber == -1);
        List<MapCell> markedCells = new List<MapCell>();
        if (f != null)
            markedCells.Add(f);
        
        while (cells.Cast<MapCell>().ToList().Any(m => m != null && m.isPassable == true && m.zoneNumber == -1) == true)
        {
            if (markedCells.Count == 0)
            {
                f = cells.Cast<MapCell>().ToList().Find(m => m != null && m.isPassable == true && m.zoneNumber == -1);
                if (f != null)
                {
                    markedCells.Add(f);
                    if (CheckNeighbors(f, CellCheckFlags.ToZone) == true)
                    {
                        f.zoneNumber = zoneNumber;
                    }
                }
                else break;
            }

            while (CheckNeighbors(f, CellCheckFlags.ToZone) == false)
            {
                for (int i = 0; i < markedCells.Count; i++)
                {
                    f = markedCells[i];
                    if (f.y + 1 < mapSizeY && cells[f.x, f.y + 1] != null && cells[f.x, f.y + 1].isPassable == true && cells[f.x, f.y + 1].zoneNumber == -1)
                    {
                        cells[f.x, f.y + 1].zoneNumber = zoneNumber;
                        markedCells.Add(cells[f.x, f.y + 1]);
                    }

                    if (f.y - 1 >= 0 && cells[f.x, f.y - 1] != null && cells[f.x, f.y - 1].isPassable == true && cells[f.x, f.y - 1].zoneNumber == -1)
                    {
                        cells[f.x, f.y - 1].zoneNumber = zoneNumber;
                        markedCells.Add(cells[f.x, f.y - 1]);
                    }

                    if (f.x + 1 < mapSizeX && cells[f.x + 1, f.y] != null && cells[f.x + 1, f.y].isPassable == true && cells[f.x + 1, f.y].zoneNumber == -1)
                    {
                        cells[f.x + 1, f.y].zoneNumber = zoneNumber;
                        markedCells.Add(cells[f.x + 1, f.y]);
                    }

                    if (f.x - 1 >= 0 && cells[f.x - 1, f.y] != null && cells[f.x - 1, f.y].isPassable == true && cells[f.x - 1, f.y].zoneNumber == -1)
                    {
                        cells[f.x - 1, f.y].zoneNumber = zoneNumber;
                        markedCells.Add(cells[f.x - 1, f.y]);
                    }
                }
            }

            zoneNumber++;
            markedCells.Clear();
        }
    }
    public void ClearPath()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int z = 0; z < mapSizeY; z++)
            {
                if (cells[x, z] != null)
                {
                    cells[x, z].pathNumber = -1;
                }
            }
        }
    }
    public MapCell[] FindPath(MapCell from, MapCell to, Creature sender)
    {
        if (from.x == to.x && from.y == to.y)
            return null;

        if (from.isPassable == false || to.isPassable == false)
            return null;

        if (from.zoneNumber != to.zoneNumber)
            return null;

        /*
        if ((to.y + 1 > mapSizeY || cells[to.x, to.y + 1] == null || cells[to.x, to.y + 1].isPassable == false || cells[to.x, to.y + 1].cellObject != null) ||
            (to.y - 1 > mapSizeY || cells[to.x, to.y - 1] == null || cells[to.x, to.y - 1].isPassable == false || cells[to.x, to.y - 1].cellObject != null) ||
            (to.x + 1 > mapSizeX || cells[to.x + 1, to.y] == null || cells[to.x + 1, to.y].isPassable == false || cells[to.x + 1, to.y].cellObject != null) ||
            (to.x - 1 > mapSizeX || cells[to.x - 1, to.y] == null || cells[to.x - 1, to.y].isPassable == false || cells[to.x - 1, to.y].cellObject != null) ||
            (from.y + 1 > mapSizeY || cells[from.x, from.y + 1] == null || cells[from.x, from.y + 1].isPassable == false || cells[from.x, from.y + 1].cellObject != null) ||
            (from.y - 1 > mapSizeY || cells[from.x, from.y - 1] == null || cells[from.x, from.y - 1].isPassable == false || cells[from.x, from.y - 1].cellObject != null) ||
            (from.x + 1 > mapSizeX || cells[from.x + 1, from.y] == null || cells[from.x + 1, from.y].isPassable == false || cells[from.x + 1, from.y].cellObject != null) ||
            (from.x - 1 > mapSizeX || cells[from.x - 1, from.y] == null || cells[from.x - 1, from.y].isPassable == false || cells[from.x - 1, from.y].cellObject != null))*/
        //if (CheckNeighbors(to, CellCheckFlags.ToPassability) == true || CheckNeighbors(from, CellCheckFlags.ToPassability) == true)
            //return null;

        List<MapCell> path = new List<MapCell>();
        to.pathNumber = 0;

        MapCell f = to;
        List<MapCell> markedCells = new List<MapCell>
        {
            to
        };

        int iterationsLeft;
        if (sender != null)
            iterationsLeft = sender.GetStateValue("view_dist");
        else iterationsLeft = mapSizeX * mapSizeY;

        //while (markedCells.Contains(from) == false)
        while (CheckNeighbors(f, CellCheckFlags.ToPathNumber) == false && iterationsLeft > 0) 
        {
            iterationsLeft--;
            for (int i = 0; i < markedCells.Count; i++)
            {
                f = markedCells[i];
                if (f.y + 1 < mapSizeY && cells[f.x, f.y + 1] != null && cells[f.x, f.y + 1].isPassable == true && cells[f.x, f.y + 1].pathNumber == -1 && (cells[f.x, f.y + 1].cellObject == null || cells[f.x, f.y + 1].cellObject == sender) && markedCells.Contains(cells[f.x, f.y + 1]) == false)
                {
                    cells[f.x, f.y + 1].pathNumber = f.pathNumber + cells[f.x, f.y + 1].pathCost;
                    markedCells.Add(cells[f.x, f.y + 1]);
                }

                if (f.y - 1 >= 0 && cells[f.x, f.y - 1] != null && cells[f.x, f.y - 1].isPassable == true && cells[f.x, f.y - 1].pathNumber == -1 && (cells[f.x, f.y - 1].cellObject == null || cells[f.x, f.y - 1].cellObject == sender) && markedCells.Contains(cells[f.x, f.y - 1]) == false)
                {
                    cells[f.x, f.y - 1].pathNumber = f.pathNumber + cells[f.x, f.y - 1].pathCost;
                    markedCells.Add(cells[f.x, f.y - 1]);
                }

                if (f.x + 1 < mapSizeX && cells[f.x + 1, f.y] != null && cells[f.x + 1, f.y].isPassable == true && cells[f.x + 1, f.y].pathNumber == -1 && (cells[f.x + 1, f.y].cellObject == null || cells[f.x + 1, f.y].cellObject == sender) && markedCells.Contains(cells[f.x + 1, f.y]) == false)
                {
                    cells[f.x + 1, f.y].pathNumber = f.pathNumber + cells[f.x + 1, f.y].pathCost;
                    markedCells.Add(cells[f.x + 1, f.y]);
                }

                if (f.x - 1 >= 0 && cells[f.x - 1, f.y] != null && cells[f.x - 1, f.y].isPassable == true && cells[f.x - 1, f.y].pathNumber == -1 && (cells[f.x - 1, f.y].cellObject == null || cells[f.x - 1, f.y].cellObject == sender) && markedCells.Contains(cells[f.x - 1, f.y]) == false)
                {
                    cells[f.x - 1, f.y].pathNumber = f.pathNumber + cells[f.x - 1, f.y].pathCost;
                    markedCells.Add(cells[f.x - 1, f.y]);
                }
            }

            if (markedCells.Contains(from))
            {
                //print("Path is built");
                break;
            }

            //markedCells.Clear();
        }

        if (markedCells.Contains(from) == false)
        {
            ClearPath();
            return null;
        }

        MapCell toGo = null;
        f = from;
        int minNumber = f.pathNumber;

        while (f.x != to.x || f.y != to.y)
        {
            if (f.y + 1 < mapSizeY && cells[f.x, f.y + 1] != null && cells[f.x, f.y + 1].pathNumber != -1 && cells[f.x, f.y + 1].pathNumber < minNumber)
            {
                minNumber = cells[f.x, f.y + 1].pathNumber;
                toGo = cells[f.x, f.y + 1];
            }

            if (f.y - 1 >= 0 && cells[f.x, f.y - 1] != null && cells[f.x, f.y - 1].pathNumber != -1 && cells[f.x, f.y - 1].pathNumber < minNumber)
            {
                minNumber = cells[f.x, f.y - 1].pathNumber;
                toGo = cells[f.x, f.y - 1];
            }

            if (f.x + 1 < mapSizeX && cells[f.x + 1, f.y] != null && cells[f.x + 1, f.y].pathNumber != -1 && cells[f.x + 1, f.y].pathNumber < minNumber)
            {
                minNumber = cells[f.x + 1, f.y].pathNumber;
                toGo = cells[f.x + 1, f.y];
            }

            if (f.x - 1 >= 0 && cells[f.x - 1, f.y] != null && cells[f.x - 1, f.y].pathNumber != -1 && cells[f.x - 1, f.y].pathNumber < minNumber)
            {
                minNumber = cells[f.x - 1, f.y].pathNumber;
                toGo = cells[f.x - 1, f.y];
            }

            path.Add(toGo);
            f = toGo;
        }

        markedCells.Clear();
        path.Reverse();
        ClearPath();
        return path.ToArray();
    }

    public int FindDistance(MapCell from, MapCell to)
    {
        if (from.x == to.x && from.y == to.y)
            return -1;

        if (from.isPassable == false || to.isPassable == false)
            return -1;

        if (from.zoneNumber != to.zoneNumber)
            return -1;

        /*
        if ((to.y + 1 > mapSizeY || cells[to.x, to.y + 1] == null || cells[to.x, to.y + 1].isPassable == false || cells[to.x, to.y + 1].cellObject != null) ||
            (to.y - 1 > mapSizeY || cells[to.x, to.y - 1] == null || cells[to.x, to.y - 1].isPassable == false || cells[to.x, to.y - 1].cellObject != null) ||
            (to.x + 1 > mapSizeX || cells[to.x + 1, to.y] == null || cells[to.x + 1, to.y].isPassable == false || cells[to.x + 1, to.y].cellObject != null) ||
            (to.x - 1 > mapSizeX || cells[to.x - 1, to.y] == null || cells[to.x - 1, to.y].isPassable == false || cells[to.x - 1, to.y].cellObject != null) ||
            (from.y + 1 > mapSizeY || cells[from.x, from.y + 1] == null || cells[from.x, from.y + 1].isPassable == false || cells[from.x, from.y + 1].cellObject != null) ||
            (from.y - 1 > mapSizeY || cells[from.x, from.y - 1] == null || cells[from.x, from.y - 1].isPassable == false || cells[from.x, from.y - 1].cellObject != null) ||
            (from.x + 1 > mapSizeX || cells[from.x + 1, from.y] == null || cells[from.x + 1, from.y].isPassable == false || cells[from.x + 1, from.y].cellObject != null) ||
            (from.x - 1 > mapSizeX || cells[from.x - 1, from.y] == null || cells[from.x - 1, from.y].isPassable == false || cells[from.x - 1, from.y].cellObject != null))*/
        //if (CheckNeighbors(to, CellCheckFlags.ToPassability) == true || CheckNeighbors(from, CellCheckFlags.ToPassability) == true)
        //return null;

        List<MapCell> path = new List<MapCell>();
        to.pathNumber = 0;

        MapCell f = to;
        List<MapCell> markedCells = new List<MapCell>
        {
            to
        };

        int iterationsLeft = mapSizeX * mapSizeY;

        //while (markedCells.Contains(from) == false)
        while (CheckNeighbors(f, CellCheckFlags.ToPathNumber) == false && iterationsLeft > 0)
        {
            iterationsLeft--;
            for (int i = 0; i < markedCells.Count; i++)
            {
                f = markedCells[i];
                if (f.y + 1 < mapSizeY && cells[f.x, f.y + 1] != null && cells[f.x, f.y + 1].isPassable == true && cells[f.x, f.y + 1].pathNumber == -1 && markedCells.Contains(cells[f.x, f.y + 1]) == false)
                {
                    cells[f.x, f.y + 1].pathNumber = f.pathNumber + cells[f.x, f.y + 1].pathCost;
                    markedCells.Add(cells[f.x, f.y + 1]);
                }

                if (f.y - 1 >= 0 && cells[f.x, f.y - 1] != null && cells[f.x, f.y - 1].isPassable == true && cells[f.x, f.y - 1].pathNumber == -1 && markedCells.Contains(cells[f.x, f.y - 1]) == false)
                {
                    cells[f.x, f.y - 1].pathNumber = f.pathNumber + cells[f.x, f.y - 1].pathCost;
                    markedCells.Add(cells[f.x, f.y - 1]);
                }

                if (f.x + 1 < mapSizeX && cells[f.x + 1, f.y] != null && cells[f.x + 1, f.y].isPassable == true && cells[f.x + 1, f.y].pathNumber == -1 && markedCells.Contains(cells[f.x + 1, f.y]) == false)
                {
                    cells[f.x + 1, f.y].pathNumber = f.pathNumber + cells[f.x + 1, f.y].pathCost;
                    markedCells.Add(cells[f.x + 1, f.y]);
                }

                if (f.x - 1 >= 0 && cells[f.x - 1, f.y] != null && cells[f.x - 1, f.y].isPassable == true && cells[f.x - 1, f.y].pathNumber == -1 && markedCells.Contains(cells[f.x - 1, f.y]) == false)
                {
                    cells[f.x - 1, f.y].pathNumber = f.pathNumber + cells[f.x - 1, f.y].pathCost;
                    markedCells.Add(cells[f.x - 1, f.y]);
                }
            }

            if (markedCells.Contains(from))
            {
                //print("Path is built");
                break;
            }

            //markedCells.Clear();
        }

        if (markedCells.Contains(from) == false)
        {
            ClearPath();
            return -1;
        }

        MapCell toGo = null;
        f = from;
        int minNumber = f.pathNumber;

        while (f.x != to.x || f.y != to.y)
        {
            if (f.y + 1 < mapSizeY && cells[f.x, f.y + 1] != null && cells[f.x, f.y + 1].pathNumber != -1 && cells[f.x, f.y + 1].pathNumber < minNumber)
            {
                minNumber = cells[f.x, f.y + 1].pathNumber;
                toGo = cells[f.x, f.y + 1];
            }

            if (f.y - 1 >= 0 && cells[f.x, f.y - 1] != null && cells[f.x, f.y - 1].pathNumber != -1 && cells[f.x, f.y - 1].pathNumber < minNumber)
            {
                minNumber = cells[f.x, f.y - 1].pathNumber;
                toGo = cells[f.x, f.y - 1];
            }

            if (f.x + 1 < mapSizeX && cells[f.x + 1, f.y] != null && cells[f.x + 1, f.y].pathNumber != -1 && cells[f.x + 1, f.y].pathNumber < minNumber)
            {
                minNumber = cells[f.x + 1, f.y].pathNumber;
                toGo = cells[f.x + 1, f.y];
            }

            if (f.x - 1 >= 0 && cells[f.x - 1, f.y] != null && cells[f.x - 1, f.y].pathNumber != -1 && cells[f.x - 1, f.y].pathNumber < minNumber)
            {
                minNumber = cells[f.x - 1, f.y].pathNumber;
                toGo = cells[f.x - 1, f.y];
            }

            path.Add(toGo);
            f = toGo;
        }

        markedCells.Clear();
        ClearPath();
        return path.Count;
    }

    public void RecalculateFogOfWar(MapCell cellFrom, int viewDistance)
    {
        MapCell cell = cellFrom;
        List<MapCell> markedCells = new List<MapCell>
        {
            cellFrom
        };
        while (CheckNeighbors(cell, CellCheckFlags.ToPassability) == false && viewDistance > 0)
        {
            viewDistance--;
            int markedCellsCount = markedCells.Count;
            for (int i = 0; i < markedCells.Count; i++)
            {
                cell = markedCells[i];
                if (cell.y + 1 < mapSizeY && cells[cell.x, cell.y + 1] != null && markedCells.Contains(cells[cell.x, cell.y + 1]) == false 
                    //&& clearedCellsFogOfWar.Contains(cells[cell.x, cell.y + 1]) == false 
                    && cells[cell.x, cell.y + 1].isPassable == true)
                {
                    markedCells.Add(cells[cell.x, cell.y + 1]);
                    if (clearedCellsFogOfWar.Contains(cells[cell.x, cell.y + 1]) == false)
                        clearedCellsFogOfWar.Add(cells[cell.x, cell.y + 1]);
                }

                if (cell.y - 1 >= 0 && cells[cell.x, cell.y - 1] != null && markedCells.Contains(cells[cell.x, cell.y - 1]) == false 
                    //&& clearedCellsFogOfWar.Contains(cells[cell.x, cell.y - 1]) == false 
                    && cells[cell.x, cell.y - 1].isPassable == true)
                {
                    markedCells.Add(cells[cell.x, cell.y - 1]);
                    if (clearedCellsFogOfWar.Contains(cells[cell.x, cell.y - 1]) == false)
                        clearedCellsFogOfWar.Add(cells[cell.x, cell.y - 1]);
                }

                if (cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y] != null && markedCells.Contains(cells[cell.x + 1, cell.y]) == false 
                    //&& clearedCellsFogOfWar.Contains(cells[cell.x + 1, cell.y]) == false 
                    && cells[cell.x + 1, cell.y].isPassable == true)
                {
                    markedCells.Add(cells[cell.x + 1, cell.y]);
                    if (clearedCellsFogOfWar.Contains(cells[cell.x + 1, cell.y]) == false)
                        clearedCellsFogOfWar.Add(cells[cell.x + 1, cell.y]);
                }

                if (cell.x - 1 >= 0 && cells[cell.x - 1, cell.y] != null && markedCells.Contains(cells[cell.x - 1, cell.y]) == false
                    //&& clearedCellsFogOfWar.Contains(cells[cell.x - 1, cell.y]) == false 
                    && cells[cell.x - 1, cell.y].isPassable == true)
                {
                    markedCells.Add(cells[cell.x - 1, cell.y]);
                    if (clearedCellsFogOfWar.Contains(cells[cell.x - 1, cell.y]) == false)
                        clearedCellsFogOfWar.Add(cells[cell.x - 1, cell.y]);
                }

                if (i >= markedCellsCount)
                    break;
            }
        }

        for (int i = 0; i < markedCells.Count; i++)
        {
            cell = markedCells[i];
            fogOfWar.SetTile(new Vector3Int(cell.x, cell.y, 0), tileClearFogOfWar);

            // light the walls too
            if (cell.y + 1 < mapSizeY && cells[cell.x, cell.y + 1] != null && markedCells.Contains(cells[cell.x, cell.y + 1]) == false && clearedCellsFogOfWar.Contains(cells[cell.x, cell.y + 1]) == false && cells[cell.x, cell.y + 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x, cell.y + 1, 0), tileClearFogOfWar);
            if (cell.y - 1 >= 0 && cells[cell.x, cell.y - 1] != null && markedCells.Contains(cells[cell.x, cell.y - 1]) == false && clearedCellsFogOfWar.Contains(cells[cell.x, cell.y - 1]) == false && cells[cell.x, cell.y - 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x, cell.y - 1, 0), tileClearFogOfWar);
            if (cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y] != null && markedCells.Contains(cells[cell.x + 1, cell.y]) == false && clearedCellsFogOfWar.Contains(cells[cell.x + 1, cell.y]) == false && cells[cell.x + 1, cell.y].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x + 1, cell.y, 0), tileClearFogOfWar);
            if (cell.x - 1 >= 0 && cells[cell.x - 1, cell.y] != null && markedCells.Contains(cells[cell.x - 1, cell.y]) == false && clearedCellsFogOfWar.Contains(cells[cell.x - 1, cell.y]) == false && cells[cell.x - 1, cell.y].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x - 1, cell.y, 0), tileClearFogOfWar);

            if (cell.y + 1 < mapSizeY && cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y + 1] != null
                && markedCells.Contains(cells[cell.x + 1, cell.y + 1]) == false && clearedCellsFogOfWar.Contains(cells[cell.x + 1, cell.y + 1]) == false && cells[cell.x + 1, cell.y + 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x + 1, cell.y + 1, 0), tileClearFogOfWar);
            if (cell.y + 1 < mapSizeY && cell.x - 1 >= 0 && cells[cell.x - 1, cell.y + 1] != null
                 && markedCells.Contains(cells[cell.x - 1, cell.y + 1]) == false && clearedCellsFogOfWar.Contains(cells[cell.x - 1, cell.y + 1]) == false && cells[cell.x - 1, cell.y + 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x - 1, cell.y + 1, 0), tileClearFogOfWar);
            if (cell.y - 1 >= 0 && cell.x + 1 < mapSizeX && cells[cell.x + 1, cell.y - 1] != null 
                && markedCells.Contains(cells[cell.x + 1, cell.y - 1]) == false && clearedCellsFogOfWar.Contains(cells[cell.x + 1, cell.y - 1]) == false && cells[cell.x + 1, cell.y - 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x + 1, cell.y - 1, 0), tileClearFogOfWar);
            if (cell.y - 1 >= 0 && cell.x - 1 >= 0 && cells[cell.x - 1, cell.y - 1] != null
                && markedCells.Contains(cells[cell.x - 1, cell.y - 1]) == false && clearedCellsFogOfWar.Contains(cells[cell.x - 1, cell.y - 1]) == false && cells[cell.x - 1, cell.y - 1].isPassable == false)
                fogOfWar.SetTile(new Vector3Int(cell.x - 1, cell.y - 1, 0), tileClearFogOfWar);
        }
    }

    public Creature FindCreatureByID(int creatureID)
    {
        //if (player.cellObjectID == creatureID)
            //return player;
        for (int i = 0; i < creaturesToMove.Count; i++)
            if (creaturesToMove[i].cellObjectID == creatureID)
                return creaturesToMove[i];
        return null;
    }

    public void SaveGame()
    {
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        XmlNode userNode;
        XmlElement element;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("save");
        xmlDoc.AppendChild(rootNode);

        player.statistics.SaveStatistics();
        player.SetStateValue("cur_health_percent", Mathf.RoundToInt((float)player.GetStateValue("max_health") / player.GetStateValue("cur_health") * 100));
        userNode = xmlDoc.CreateElement("Creatures");
        {
            for (int i = 0; i < creaturesToMove.Count; i++)
            {
                element = xmlDoc.CreateElement(creaturesToMove[i].creatureNameID);
                element.SetAttribute("value", creaturesToMove[i].ToString());
                for (int j = 0; j < creaturesToMove[i].statesHandler.creatureStates.Count; j++)
                {
                    element.SetAttribute("state" + j, creaturesToMove[i].statesHandler.creatureStates[j].ToString());
                }
                for (int j = 0; j < creaturesToMove[i].connectedInventory.inventorySlotsCount; j++)
                {
                    element.SetAttribute("item" + j, creaturesToMove[i].connectedInventory.ItemToString(j));
                }
                for (int j = 0; j < creaturesToMove[i].currentEffects.Count; j++)
                {
                    element.SetAttribute("effect" + j, creaturesToMove[i].currentEffects[j].ToString());
                }
                userNode.AppendChild(element);
            }
        }

        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/save.xml");

        // fog of war
        xmlDoc = new XmlDocument();
        rootNode = xmlDoc.CreateElement("fog");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("fog");
        {
            for (int i = 0; i < clearedCellsFogOfWar.Count; i++)
            {
                element = xmlDoc.CreateElement("cell");
                element.SetAttribute("value", clearedCellsFogOfWar[i].x + ";" + clearedCellsFogOfWar[i].y);
                userNode.AppendChild(element);
            }
        }

        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/fog.xml");

        // learnt recipes
        xmlDoc = new XmlDocument();
        rootNode = xmlDoc.CreateElement("recipes");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("recipes");
        {
            for (int i = 0; i < player.exploredRecipes.Count; i++)
            {
                element = xmlDoc.CreateElement("recipe");
                element.SetAttribute("value", player.exploredRecipes[i].ToString());
                userNode.AppendChild(element);
            }
        }

        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/recipes.xml");

        // gained and prepared skills
        xmlDoc = new XmlDocument();
        rootNode = xmlDoc.CreateElement("skills");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("skills");
        {
            for (int i = 0; i < player.gainedSkills.Count; i++)
            {
                element = xmlDoc.CreateElement("skill");
                element.SetAttribute("value", player.gainedSkills[i].ToString());
                userNode.AppendChild(element);
            }

            element = xmlDoc.CreateElement("unused_skill_points");
            element.SetAttribute("value", player.levelupPanelController.unusedSkillPoints.ToString());
            userNode.AppendChild(element);
        }
        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/skills.xml");
        SavePreparedSkills();
    }

    public void SavePreparedSkills()
    {
        XmlNode userNode;
        XmlElement element;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("skills");
        xmlDoc.AppendChild(rootNode);

        userNode = xmlDoc.CreateElement("prepared_skills");
        {
            for (int i = 0; i < player.preparedSkillsIDs.Count; i++)
            {
                element = xmlDoc.CreateElement("skill");
                element.SetAttribute("value", player.preparedSkillsIDs[i].ToString());
                userNode.AppendChild(element);
            }
        }

        rootNode.AppendChild(userNode);
        xmlDoc.Save(Application.dataPath + "/Saves/prepared_skills.xml");
    }

    public void ReapplyEffects()
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
                    if (childnode.Name == "hero")
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("effect") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                Effect.ApplyEffect(
                                    EffectsDatabase.GetEffectByID(int.Parse(s[0])),
                                    FindCreatureByID(int.Parse(s[1])),
                                    FindCreatureByID(int.Parse(s[2])),
                                    int.Parse(s[3]) + 1,
                                    int.Parse(s[4]));
                            }
                        }
                    }
                    else if (isLevelLoadedNotFromFile == false)
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("effect") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                Effect.ApplyEffect(
                                    EffectsDatabase.GetEffectByID(int.Parse(s[0])),
                                    FindCreatureByID(int.Parse(s[1])),
                                    FindCreatureByID(int.Parse(s[2])),
                                    int.Parse(s[3]),
                                    int.Parse(s[4]));
                            }
                        }
                    }
                }
            }
        }
    }

    public void LoadGame()
    {
        creaturesContainer = FindObjectOfType<CreaturesContainer>();
        cells = new MapCell[mapSizeX, mapSizeY];
        player.statistics.LoadStatistics();
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
                    if (childnode.Name == "hero")
                    {
                        player.FromString(childnode.Attributes.GetNamedItem("value").Value);
                        player.experienceToNextLevel = 120;
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("state") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                player.GetStateByName(s[0]).startStateValue = int.Parse(s[1]);
                                if (s[0] != "level")
                                {
                                    if (s[0] != "exp")
                                    {
                                        if (bool.Parse(s[2]) == true)
                                            player.SetStateValue(s[0], int.Parse(s[3]));
                                        else player.RecalculateState(s[0]);
                                    }
                                    else
                                    {
                                        player.AddExperience(int.Parse(s[3]), false);
                                    }
                                }

                                //player.RecalculateState(s[0]);
                                //player.SetStateValue(state.stateName, state.valueAddends);
                            }
                            else if (childnode.Attributes[i].Name.StartsWith("item") == true)
                            {
                                string s = childnode.Attributes[i].Value;
                                if (s.Length < 1) s = "0";
                                player.connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(int.Parse(s)));
                            }
                        }

                        string[] r = PlayerPrefs.GetString("explored_recipes").Split(';');
                        for (int i = 0; i < r.Length; i++)
                            if (r[i].Length > 0)
                                player.exploredRecipes.Add(int.Parse(r[i]));
                    }
                    else if (isLevelLoadedNotFromFile == false)
                    {
                        Creature creature = Instantiate(creaturesContainer.GetCreatureByName(childnode.Name));
                        creature.map = this;
                        creature.FromString(childnode.Attributes.GetNamedItem("value").Value);
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("state") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                creature.GetStateByName(s[0]).startStateValue = int.Parse(s[1]);
                                if (bool.Parse(s[2]) == true)
                                    creature.SetStateValue(s[0], int.Parse(s[3]));
                                else creature.RecalculateState(s[0]);

                                //creature.SetStateValue(state.stateName, state.valueAddends);
                            }
                            else if (childnode.Attributes[i].Name.StartsWith("item") == true)
                            {
                                string s = childnode.Attributes[i].Value;
                                if (s.Length < 1) s = "0";
                                creature.connectedInventory.AddItemToInventory(ItemsDatabase.GetItemByID(int.Parse(s)));
                            }
                        }

                        creature.SetCreatureInformation();
                    }
                }
            }
        }

        if (File.Exists(Application.dataPath + "/Saves/recipes.xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/recipes.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "recipe")
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("value") == true)
                            {
                                player.exploredRecipes.Add(int.Parse(childnode.Attributes[i].Value));
                            }
                        }
                    }
                }
            }
        }

        if (File.Exists(Application.dataPath + "/Saves/skills.xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/skills.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "skill")
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("value") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                player.GainSkill(int.Parse(s[0]), int.Parse(s[1]));
                            }
                        }
                    }
                    if (childnode.Name == "unused_skill_points")
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("value") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                player.levelupPanelController.unusedSkillPoints = (int.Parse(s[0]));
                            }
                        }
                    }
                }
            }
        }

        if (File.Exists(Application.dataPath + "/Saves/prepared_skills.xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/prepared_skills.xml");
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "skill")
                    {
                        for (int i = 0; i < childnode.Attributes.Count; i++)
                        {
                            if (childnode.Attributes[i].Name.StartsWith("value") == true)
                            {
                                string[] s = childnode.Attributes[i].Value.Split(';');
                                player.preparedSkillsIDs.Add(int.Parse(s[0]));
                                //player.GainSkill(int.Parse(s[0]), int.Parse(s[1]));
                            }
                        }
                    }
                }
            }
        }

        /*
        if (useHealthPercent == true)
            player.SetStateValue("cur_health", Mathf.RoundToInt((float)player.GetStateValue("max_health") / player.GetStateValue("cur_health_percent") * 100));
            */

        if (player.preparedSkillsIDs.Count == 0)
        {
            player.levelupPanelController.PrepareSkills();
            SavePreparedSkills();
        }

        if (player.levelupPanelController.unusedSkillPoints > 0)
        {
            //player.levelupPanelController.CheckSkills();
            player.levelupPanelController.ShowSkills();
            player.levelupPanelController.gameObject.SetActive(true);
        }
    }
}