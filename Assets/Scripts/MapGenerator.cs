using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum JointDirection
{
    Up,
    Right,
    Down,
    Left
}

[Serializable]
public class GenerationMapTile
{
    public int x, y;
    // -1 - unidentified (empty)
    // 0 - floor
    // 1 - ceiling
    // 2 - wall
    public int floorType = -1;
    public int roomNumber = -1;

    // -1 - declined
    // 0 - unchecked
    public int enemyLevel = 0;

    public JointDirection jointDirection;
    public bool isJointFinished;
    public bool isJointCanceled;
    public bool frontSideIsConnected, rearSideIsConnected;
    public int frontSideRoomNumber, rearSideRoomNumber;
}

[Serializable]
public class MapRoom
{
    public List<int> connections = new List<int>();
    public int pathIndex = -1;
    public int roomNumber;
    public Vector2Int leftBottomCorner, rightTopCorner;

    public MapRoom(Vector2Int _leftBottomCorner, Vector2Int _rightTopCorner, int _roomNumber)
    {
        leftBottomCorner = _leftBottomCorner;
        rightTopCorner = _rightTopCorner;
        roomNumber = _roomNumber;
    }
}

public class MapGenerator : MonoBehaviour
{
    public int maxAttemptsCount;
    int attemptsLeft;
    public GenerationMapTile[,] tiles;
    public List<GenerationMapTile> joints;
    public int maxMapSizeX = 200,
               maxMapSizeY = 200;
    public int mapMinX = -1,
               mapMinY = -1,
               mapMaxX = -1,
               mapMaxY = -1;
    public GenerationMapTile currentJoint;
    public int currentJointIndex = 0;
    public int currentRoomNumber = 0;

    public Tilemap tilemap, jointsTilemap;
    public TileBase floorTile, ceilingTile, wallTile;
    public TileBase freeJoint, occupiedJoint;

    public TextMesh tileText;

    public int jointsToTopCount, jointsToRightCount, jointsToBottomCount, jointsToLeftCount;

    public List<MapRoom> rooms = new List<MapRoom>();

    private void Start()
    {
        tiles = new GenerationMapTile[maxMapSizeX, maxMapSizeY];
        for (int _x = 0; _x < maxMapSizeX; _x++)
            for (int _y = 0; _y < maxMapSizeY; _y++)
                tiles[_x, _y] = new GenerationMapTile()
                {
                    x = _x,
                    y = _y,
                    floorType = -1,
                    roomNumber = -1
                };

        /*
        bool result = RoomGenerationWave();
        while (result == false && attemptsLeft > 0)
            result = RoomGenerationWave();
            */
        Random.InitState(Mathf.RoundToInt(DateTime.Now.Ticks * Time.deltaTime * DateTime.Now.Millisecond * 0.000000000001f));

        tiles[100, 100].jointDirection = JointDirection.Right;
        currentJoint = tiles[100, 100];

        StartCoroutine(Generate(12));
    }

    public IEnumerator Generate(int maxRooms)
    {
        for (int j = 0; j < maxRooms; j++)
        {
            for (int i = 0; i < maxAttemptsCount;)
            {
                if (RoomGenerationWave(i) == true) break;
                i++;
                if (i == maxAttemptsCount)
                {
                    currentJoint.isJointFinished = true;
                    currentJoint.frontSideIsConnected = true;
                    currentJoint.rearSideIsConnected = true;
                    jointsTilemap.SetTile(new Vector3Int(currentJoint.x, currentJoint.y, 0), occupiedJoint);
                }
            }
        }
        FixUnusedJoints();
        FixWallsAbsence();
        SmoothenDeadEnds();
        SeparateRooms();
        RenderRooms();
        yield break;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < maxAttemptsCount;)
            {
                Random.InitState(Mathf.RoundToInt(DateTime.Now.Ticks * Time.deltaTime * DateTime.Now.Millisecond * 0.000000000001f) + currentRoomNumber);
                if (RoomGenerationWave(i) == true) break;
                i++;
                if (i == maxAttemptsCount)
                {
                    currentJoint.isJointFinished = true;
                    currentJoint.frontSideIsConnected = true;
                    currentJoint.rearSideIsConnected = true;
                    jointsTilemap.SetTile(new Vector3Int(currentJoint.x, currentJoint.y, 0), occupiedJoint);
                }
            }
            RenderRooms();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SmoothenDeadEnds();
            FixUnusedJoints();
            FixWallsAbsence();
            SeparateRooms();
            RenderRooms();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GenerationMapTile tile = tiles[(int)Math.Truncate(mousePos.x), (int)Math.Truncate(mousePos.y)];
            print(tile.x + " " + tile.y + " | " + tile.jointDirection + " | " + tile.roomNumber + " | " + tile.isJointCanceled);
        }
    }

    public void RenderRooms()
    {
        for (int x = 0; x < maxMapSizeX; x++)
            for (int y = 0; y < maxMapSizeY; y++)
            {
                if (tiles[x, y].floorType == 0)
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                if (tiles[x, y].floorType == 1)
                    tilemap.SetTile(new Vector3Int(x, y, 0), ceilingTile);
                if (tiles[x, y].floorType == 2)
                    tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
            }
    }

    public int GetCountOfJointsUnconnected(JointDirection requiredDirection)
    {
        Random.InitState(Mathf.RoundToInt(DateTime.Now.Ticks * Time.deltaTime * 0.00000000001f) - 915 * currentRoomNumber);
        return joints.FindAll(j => j.jointDirection == requiredDirection && (j.rearSideIsConnected == false || j.frontSideIsConnected == false)).Count;
    }

    public int GetCountOfJointsOfType(JointDirection requiredDirection)
    {
        Random.InitState(Mathf.RoundToInt(DateTime.Now.Ticks * Time.deltaTime * 0.00000000001f) + 916 * currentRoomNumber);
        return joints.FindAll(j => j.jointDirection == requiredDirection).Count;
    }

    public void SmoothenDeadEnds()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            GenerationMapTile curDeadEnd = joints[i];
            int x = curDeadEnd.x;
            int y = curDeadEnd.y;
            if (curDeadEnd.jointDirection == JointDirection.Up)
            {
                if (tiles[x, y + 1].floorType > 0)
                {
                    joints[i].isJointFinished = true;
                    joints[i].frontSideIsConnected = true;
                    joints[i].rearSideIsConnected = true;
                    joints[i].isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(joints[i].x, joints[i].y, 0), occupiedJoint);
                    tiles[x, y].floorType = 1;
                    tiles[x, y - 1].floorType = 2;
                }
            }
            else if (curDeadEnd.jointDirection == JointDirection.Down)
            {
                if (tiles[x, y - 1].floorType > 0)
                {
                    joints[i].isJointFinished = true;
                    joints[i].frontSideIsConnected = true;
                    joints[i].rearSideIsConnected = true;
                    joints[i].isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(joints[i].x, joints[i].y, 0), occupiedJoint);
                    tiles[x, y].floorType = 1;
                }
            }
            else if (curDeadEnd.jointDirection == JointDirection.Right)
            {
                if (tiles[x + 1, y].floorType > 0)
                {
                    joints[i].isJointFinished = true;
                    joints[i].frontSideIsConnected = true;
                    joints[i].rearSideIsConnected = true;
                    joints[i].isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(joints[i].x, joints[i].y, 0), occupiedJoint);
                    tiles[x, y].floorType = 1;
                    tiles[x, y + 1].floorType = 1;
                }
            }
            else if (curDeadEnd.jointDirection == JointDirection.Left)
            {
                if (tiles[x - 1, y].floorType > 0)
                {
                    joints[i].isJointFinished = true;
                    joints[i].frontSideIsConnected = true;
                    joints[i].rearSideIsConnected = true;
                    joints[i].isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(joints[i].x, joints[i].y, 0), occupiedJoint);
                    tiles[x, y].floorType = 1;
                    tiles[x, y + 1].floorType = 1;
                }
            }
        }
    }

    public void FixUnusedJoints()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            GenerationMapTile curJoint = joints[i];
            int x = curJoint.x;
            int y = curJoint.y;
            if (curJoint.jointDirection == JointDirection.Up)
            {
                if (tiles[x, y + 1].floorType != 0)
                {
                    tiles[x, y].floorType = 1;
                    tiles[x, y - 1].floorType = 2;
                    curJoint.isJointCanceled = true;
                }
            }
            else if (curJoint.jointDirection == JointDirection.Down)
            {
                if (tiles[x, y - 1].floorType != 0)
                {
                    tiles[x, y].floorType = 1;
                    curJoint.isJointCanceled = true;
                }
            }
            else if (curJoint.jointDirection == JointDirection.Right)
            {
                if (tiles[x + 1, y].floorType != 0)
                {
                    tiles[x, y].floorType = 1;
                    tiles[x, y + 1].floorType = 1;
                    curJoint.isJointCanceled = true;
                }
            }
            else if (curJoint.jointDirection == JointDirection.Left)
            {
                if (tiles[x - 1, y].floorType != 0)
                {
                    tiles[x, y].floorType = 1;
                    tiles[x, y + 1].floorType = 1;
                    curJoint.isJointCanceled = true;
                }
            }
        }
    }

    public void SeparateRooms()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            if (joints[i].isJointCanceled == true) continue;

            GenerationMapTile curJoint = joints[i];
            int x = curJoint.x;
            int y = curJoint.y;
            if (curJoint.jointDirection == JointDirection.Up)
            {
                if (tiles[x, y + 1].floorType == 0 && tiles[x, y - 1].floorType == 0)
                {
                    curJoint.frontSideRoomNumber = tiles[x, y + 1].roomNumber;
                    curJoint.rearSideRoomNumber = tiles[x, y - 1].roomNumber;
                }
            }
            else if (curJoint.jointDirection == JointDirection.Down)
            {
                if (tiles[x, y + 1].floorType == 0 && tiles[x, y - 1].floorType == 0)
                {
                    curJoint.frontSideRoomNumber = tiles[x, y - 1].roomNumber;
                    curJoint.rearSideRoomNumber = tiles[x, y + 1].roomNumber;
                }
            }
            else if (curJoint.jointDirection == JointDirection.Right)
            {
                if (tiles[x + 1, y].floorType == 0 && tiles[x - 1, y].floorType == 0)
                {
                    curJoint.frontSideRoomNumber = tiles[x + 1, y].roomNumber;
                    curJoint.rearSideRoomNumber = tiles[x - 1, y].roomNumber;
                }
            }
            else if (curJoint.jointDirection == JointDirection.Left)
            {
                if (tiles[x + 1, y].floorType == 0 && tiles[x - 1, y].floorType == 0)
                {
                    curJoint.frontSideRoomNumber = tiles[x - 1, y].roomNumber;
                    curJoint.rearSideRoomNumber = tiles[x + 1, y].roomNumber;
                }
            }
        }

        for (int i = 0; i < joints.Count; i++)
        {
            if (joints[i].isJointCanceled == true) continue;

            rooms[joints[i].frontSideRoomNumber].pathIndex = -1;
            if (!rooms[joints[i].frontSideRoomNumber].connections.Contains(joints[i].rearSideRoomNumber))
            {
                rooms[joints[i].frontSideRoomNumber].connections.Add(joints[i].rearSideRoomNumber);
                rooms[joints[i].frontSideRoomNumber].roomNumber = joints[i].frontSideRoomNumber;
            }
            if (!rooms[joints[i].rearSideRoomNumber].connections.Contains(joints[i].frontSideRoomNumber))
            {
                rooms[joints[i].rearSideRoomNumber].connections.Add(joints[i].frontSideRoomNumber);
                rooms[joints[i].rearSideRoomNumber].roomNumber = joints[i].rearSideRoomNumber;
            }
        }

        int maxLength = 0;
        int startRoomIndex = -1, // room for the player
            endRoomIndex = -1;   // room for the exit
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < rooms.Count; j++)
                rooms[j].pathIndex = -1;

            rooms[i].pathIndex = 0;
            int curPathIndex;
            for (curPathIndex = 1; curPathIndex < rooms.Count; curPathIndex++)
            {
                if (!rooms.Any(n => n.pathIndex == -1)) break;
                foreach (var n in rooms.Where(n => n.pathIndex == -1 && n.connections.Any(_n => rooms[_n].pathIndex == curPathIndex - 1)))
                    n.pathIndex = curPathIndex;
            }
            if (curPathIndex > maxLength)
            {
                maxLength = curPathIndex;
                startRoomIndex = i;
                endRoomIndex = rooms.Where(n => n.pathIndex == curPathIndex - 1).ToList()[0].roomNumber;
            }
        }

        /*
                Possible arrangements:

                1) weight = 1
                . . .
                . e .
                . . .

                2) weight = 2
                e . .
                . . .
                . . e

                3) weight = 2
                . . .
                e . e
                . . .

                4) weight = 3
                . . e
                . e .
                e . .

                5) weight = 3
                e . e
                . . .
                . e .

                6) weight = 4
                e . e
                . . .
                e . e

                7) weight = 5
                e . e
                . e .
                . e .

                8) weight = 6
                e . e
                . e .
                e . e

                9) weight = 6
                . e .
                e e e
                . e .

                If middle enemy is surrounded by more than 2 another enemies he shall gain +1 to his level and to his weight
                Room's maximum weight depends on its area and difficulty
        */

        // placing enemies, player and the exit
        for (int i = 0; i < rooms.Count; i++)
        {
            int currentRoomWeight = 0,
                maxRoomWeight = -1;
            int roomArea = (rooms[i].rightTopCorner.x - rooms[i].leftBottomCorner.x) * (rooms[i].rightTopCorner.y - rooms[i].leftBottomCorner.y); // S = W * H

            //getting different connections count
            List<int> connectsWithRooms = new List<int>();
            for (int j = 0; j < rooms[i].connections.Count; j++)
                if (!connectsWithRooms.Contains(rooms[i].connections[j]))
                    connectsWithRooms.Add(rooms[i].connections[j]);

            float roomDifficulty; // room weight coefficient
            if (i == startRoomIndex)
                roomDifficulty = 0; // player's rooms does not contain enemies
            else if (connectsWithRooms.Count == 1)
                roomDifficulty = 2f; // dead end (treasury) rooms are the most dangerous
            else if (connectsWithRooms.Count == 2)
                roomDifficulty = 1;
            else if (connectsWithRooms.Count == 3)
                roomDifficulty = 1.25f;
            else roomDifficulty = 1.5f;

            maxRoomWeight = Mathf.RoundToInt(roomArea / 8 * roomDifficulty);
            /*
            List<GenerationMapTile> possibleTiles = new List<GenerationMapTile>(); // tiles around which there is not any obstacle or occupied cell
            for (int x = rooms[i].leftBottomCorner.x; x < rooms[i].rightTopCorner.x; x++)
            {
                for (int y = rooms[i].leftBottomCorner.y; y < rooms[i].rightTopCorner.y; y++)
                {
                    if (CheckIsTilePossibleForEnemySpawning(x, y, rooms[i].roomNumber))
                        possibleTiles.Add(tiles[x, y]);
                }
            }
            */

            List<GenerationMapTile> roomTiles = tiles.Cast<GenerationMapTile>().
                Where(t => t.x >= rooms[i].leftBottomCorner.x &&
                           t.y >= rooms[i].leftBottomCorner.y &&
                           t.x <= rooms[i].rightTopCorner.x &&
                           t.y <= rooms[i].rightTopCorner.y).ToList();
            while (roomTiles.Any(t => t.enemyLevel == 0) && currentRoomWeight < maxRoomWeight)
            {
                GenerationMapTile tileToSpawn = roomTiles[Random.Range(0, roomTiles.Count)];
                int x = tileToSpawn.x,
                    y = tileToSpawn.y;
                for (int _x = x - 1; _x <= x + 1; _x++)
                    for (int _y = y - 1; _y <= y + 1; _y++)
                        tiles[_x, _y].enemyLevel = -1;

                tiles[x, y].enemyLevel = 1;
                currentRoomWeight++;
                Instantiate(tileText, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity).text = "E";

                roomTiles = roomTiles.Where(t => t.enemyLevel == 0).ToList();
            }

            /*
            GenerationMapTile curTileToSpawn = 
                tiles[Random.Range(rooms[i].leftBottomCorner.x, rooms[i].rightTopCorner.x + 1), 
                      Random.Range(rooms[i].leftBottomCorner.y, rooms[i].rightTopCorner.y + 1)]; //possibleTiles[Random.Range(0, possibleTiles.Count)];
            */

            /*
            int minEnemiesCount = Mathf.RoundToInt(roomArea / 9 * roomDifficulty);
            int maxEnemiesCount = roomArea / 7 * roomDifficulty > 0 ? Mathf.RoundToInt(roomArea / 6 * roomDifficulty - 1) : 0;

            int enemiesCount = Random.Range(minEnemiesCount, maxEnemiesCount);
            Vector2Int placeToSpawn;
            List<Vector2Int> occupiedPlaces = new List<Vector2Int>();
            for (int j = 0; j < enemiesCount; j++)
            {
                do
                {
                    placeToSpawn = new Vector2Int(
                    Random.Range(rooms[i].leftBottomCorner.x, rooms[i].rightTopCorner.x + 1),
                    Random.Range(rooms[i].leftBottomCorner.y, rooms[i].rightTopCorner.y + 1));
                }
                while (occupiedPlaces.Contains(placeToSpawn));
                Instantiate(tileText, placeToSpawn + new Vector2(0.5f, 0.5f), Quaternion.identity).text = "e";
            }
            */

            //Instantiate(tileText, new Vector2(rooms[i].leftBottomCorner.x + 2.5f, rooms[i].leftBottomCorner.y + 2.5f), Quaternion.identity).text = minEnemiesCount + "-" + maxEnemiesCount;
        }

        print(maxLength + ": " + startRoomIndex + "; " + endRoomIndex);
    }

    private bool CheckIsTilePossibleForEnemySpawning(int x, int y, int roomNumber)
    {
        for (int _x = x - 1; _x < x + 1; _x++)
            for (int _y = y - 1; _y < y + 1; _y++)
                if (tiles[_x, _y].floorType != 0 ||
                    tiles[_x, _y].roomNumber != roomNumber ||
                    tiles[_x, _y].enemyLevel != 0)
                    return false;
        return true;
    }

    public void FixWallsAbsence()
    {
        for (int x = 0; x < maxMapSizeX; x++)
        {
            for (int y = 0; y < maxMapSizeY; y++)
            {
                if (tiles[x, y].floorType == 1 && tiles[x, y - 1].floorType != 1)
                    tiles[x, y - 1].floorType = 2;
            }
        }
    }

    /// <summary>
    /// Returns true if room is generated successfully.
    /// </summary>
    /// <returns></returns>
    public bool RoomGenerationWave(int attemptNumber)
    {
        int maxRoomHeight = 8;
        int maxRoomWidth = 8;

        int _jointsToTopCountCur = 0, 
            _jointsToRightCountCur = 0, 
            _jointsToBottomCountCur = 0, 
            _jointsToLeftCountCur = 0;

        Vector2Int leftBottomCorner = new Vector2Int(), 
                   rightTopCorner = new Vector2Int();

        Random.InitState(Mathf.RoundToInt(DateTime.Now.Ticks * Time.deltaTime * DateTime.Now.Millisecond * 0.000000000001f) + currentRoomNumber + attemptNumber);
        if (currentJoint.isJointFinished == false)
        {
            if (currentJoint.jointDirection == JointDirection.Up)
            {
                int x = currentJoint.x;
                int y = currentJoint.y;
                bool shouldRejectJoint = false;
                if (tiles[x, y + 1].floorType != -1)
                    shouldRejectJoint = true;
                else
                {
                    int spreadToLeft = Random.Range(1, maxRoomWidth);
                    for (int i = 1; i <= spreadToLeft; i++)
                    {
                        if (tiles[x - i, y + 1].floorType != -1)
                        {
                            spreadToLeft = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    int spreadToRight = spreadToLeft <= 2 ? Random.Range(maxRoomWidth - spreadToLeft - 2, maxRoomWidth - spreadToLeft) : Random.Range(1, maxRoomWidth);
                    for (int i = 1; i <= spreadToRight; i++)
                    {
                        if (tiles[x + i, y + 1].floorType != -1)
                        {
                            spreadToRight = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    //   spread to left 
                    // + spread to right 
                    // + one tile above the joint }
                    // - one left wall            } 1 - 1 - 1 = -1
                    // - one right wall           }
                    if (spreadToLeft + spreadToRight - 1 < 4)
                    {
                        //print("Too narrow room at (" + x + "; " + y + ")");
                        return false;
                    }
                    else
                    {
                        int chestsCount = 0;
                        int creaturesCount = 0;

                        // + upper wall
                        int height = Random.Range(maxRoomHeight - 3, maxRoomHeight) + 1;
                        int maxHeight = height;
                        for (int k = -spreadToLeft + 1; k < spreadToRight; k++)
                        {
                            for (int i = 1; i <= height; i++)
                            {
                                if (tiles[x + k, y + i].floorType != -1)
                                {
                                    maxHeight = i - 1 >= 0 ? i - 1 : 0;
                                    break;
                                }
                            }
                            if (height > maxHeight)
                                height = maxHeight;
                        }

                        if (height < 4)
                        {
                            //print("Too low room at (" + x + "; " + y + "): height = " + height);
                            return false;
                        }
                        else
                        {
                            for (int _x = -spreadToLeft; _x <= spreadToRight; _x++)
                            {
                                for (int _y = 0; _y <= height; _y++)
                                {
                                    if (x + _x < mapMinX || mapMinX == -1) mapMinX = x + _x;
                                    if (y + _y < mapMinY || mapMinY == -1) mapMinY = y + _y;
                                    if (x + _x > mapMaxX || mapMaxX == -1) mapMaxX = x + _x;
                                    if (y + _y > mapMaxY || mapMaxY == -1) mapMaxY = y + _y;
                                    if (tiles[x + _x, y + _y].roomNumber == -1)
                                        tiles[x + _x, y + _y].roomNumber = currentRoomNumber;
                                    if (_y == 0)
                                    {
                                        if (_x == -spreadToLeft)
                                            leftBottomCorner = new Vector2Int(x + _x + 1, y + 1);

                                        if (tiles[x + _x, y].floorType == -1)
                                            tiles[x + _x, y].floorType = 1;
                                        else if (tiles[x + _x, y].floorType == 0)
                                        {
                                            tiles[x + _x, y].frontSideIsConnected = true;
                                            tiles[x + _x, y].rearSideIsConnected = true;
                                            tiles[x + _x, y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_x == -spreadToLeft)
                                    {
                                        if (_y == height)
                                        {
                                            tiles[x + _x, y + _y + 1].floorType = 1;
                                            if (tiles[x + _x, y + _y].floorType == -1)
                                                tiles[x + _x, y + _y].floorType = 1;
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == -1)
                                        {
                                            if (tiles[x + _x, y - _y + 3].floorType == 1 && Random.Range(0f, 100f) < 25f - jointsToLeftCount - _jointsToLeftCountCur * 10f)
                                            {
                                                tiles[x + _x, y + _y].floorType = 0;
                                                tiles[x + _x, y + _y + 1].floorType = 2;
                                                tiles[x + _x, y + _y + 2].floorType = 1;
                                                tiles[x + _x, y + _y].jointDirection = JointDirection.Left;
                                                tiles[x + _x, y + _y].rearSideIsConnected = true;
                                                jointsToLeftCount++;
                                                _jointsToLeftCountCur++;
                                                joints.Add(tiles[x + _x, y + _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x + _x, y + _y].floorType = 1;
                                            }
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == 0)
                                        {
                                            tiles[x + _x, y + _y].frontSideIsConnected = true;
                                            tiles[x + _x, y + _y].rearSideIsConnected = true;
                                            tiles[x + _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_x == spreadToRight)
                                    {
                                        if (_y == height)
                                        {
                                            rightTopCorner = new Vector2Int(x + _x - 1, y + _y - 1);

                                            tiles[x + _x, y + _y + 1].floorType = 1;
                                            if (tiles[x + _x, y + _y].floorType == -1)
                                                tiles[x + _x, y + _y].floorType = 1;
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == -1)
                                        {
                                            if (tiles[x + _x, y - _y + 3].floorType == 1 && Random.Range(0f, 100f) < 25f - jointsToRightCount - _jointsToRightCountCur * 10f)
                                            {
                                                tiles[x + _x, y + _y].floorType = 0;
                                                tiles[x + _x, y + _y + 1].floorType = 2;
                                                tiles[x + _x, y + _y + 2].floorType = 1;
                                                tiles[x + _x, y + _y].jointDirection = JointDirection.Right;
                                                tiles[x + _x, y + _y].rearSideIsConnected = true;
                                                jointsToRightCount++;
                                                _jointsToRightCountCur++;
                                                joints.Add(tiles[x + _x, y + _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x + _x, y + _y].floorType = 1;
                                            }
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == 0)
                                        {
                                            tiles[x + _x, y + _y].frontSideIsConnected = true;
                                            tiles[x + _x, y + _y].rearSideIsConnected = true;
                                            tiles[x + _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else
                                    {
                                        if (_y < height)
                                        {
                                            tiles[x + _x, y + _y].floorType = 0;
                                        }
                                        else
                                        {
                                            if (tiles[x + _x, y + _y + 1].floorType == 0)
                                            {
                                                tiles[x + _x, y + _y + 1].frontSideIsConnected = true;
                                                tiles[x + _x, y + _y + 1].rearSideIsConnected = true;
                                                tiles[x + _x, y + _y + 1].isJointFinished = true;
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y + 1, 0), occupiedJoint);
                                            }
                                            else
                                            {
                                                if (tiles[x + _x, y + _y + 1].floorType == -1 && Random.Range(0f, 100f) < 25f - jointsToTopCount - _jointsToTopCountCur * 10f)
                                                {
                                                    tiles[x + _x, y + _y].floorType = 0;
                                                    tiles[x + _x, y + _y + 1].floorType = 0;
                                                    tiles[x + _x, y + _y + 1].jointDirection = JointDirection.Up;
                                                    tiles[x + _x, y + _y].rearSideIsConnected = true;
                                                    jointsToTopCount++;
                                                    _jointsToTopCountCur++;
                                                    joints.Add(tiles[x + _x, y + _y + 1]);
                                                    jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y + 1, 0), freeJoint);
                                                }
                                                else
                                                {
                                                    tiles[x + _x, y + _y].floorType = 2;
                                                    tiles[x + _x, y + _y + 1].floorType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (shouldRejectJoint == true)
                {
                    currentJoint.isJointFinished = true;
                    currentJoint.frontSideIsConnected = true;
                    currentJoint.rearSideIsConnected = true;
                    currentJoint.isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(currentJoint.x, currentJoint.y, 0), occupiedJoint);
                }
                else
                {
                    rooms.Add(new MapRoom(leftBottomCorner, rightTopCorner, currentRoomNumber));
                    currentRoomNumber++;
                }
            }
            else if (currentJoint.jointDirection == JointDirection.Down)
            {
                int x = currentJoint.x;
                int y = currentJoint.y;
                bool shouldRejectJoint = false;
                if (tiles[x, y - 1].floorType != -1)
                    shouldRejectJoint = true;
                else
                {
                    int spreadToLeft = Random.Range(1, maxRoomWidth);
                    for (int i = 1; i <= spreadToLeft; i++)
                    {
                        if (tiles[x - i, y - 1].floorType != -1)
                        {
                            spreadToLeft = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    int spreadToRight = spreadToLeft <= 2 ? Random.Range(maxRoomWidth - spreadToLeft - 2, maxRoomWidth - spreadToLeft) : Random.Range(1, maxRoomWidth);
                    for (int i = 1; i <= spreadToRight; i++)
                    {
                        if (tiles[x + i, y - 1].floorType != -1)
                        {
                            spreadToRight = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    //   spread to left 
                    // + spread to right 
                    // + one tile beneath the joint }
                    // - one left wall              } 1 - 1 - 1 = -1
                    // - one right wall             }
                    if (spreadToLeft + spreadToRight - 1 < 4)
                    {
                        //print("Too narrow room at (" + x + "; " + y + ")");
                        return false;
                    }
                    else
                    {
                        int chestsCount = 0;
                        int creaturesCount = 0;

                        // + lower wall
                        int height = Random.Range(maxRoomHeight - 3, maxRoomHeight) + 1;
                        int maxHeight = height;
                        for (int k = -spreadToLeft + 1; k < spreadToRight; k++)
                        {
                            for (int i = 1; i <= height; i++)
                            {
                                if (tiles[x + k, y - i].floorType != -1)
                                {
                                    maxHeight = i - 1 >= 0 ? i - 1 : 0;
                                    break;
                                }
                            }
                            if (height > maxHeight)
                                height = maxHeight;
                        }

                        if (height < 4)
                        {
                            //print("Too low room at (" + x + "; " + y + "): height = " + height);
                            return false;
                        }
                        else
                        {
                            for (int _x = -spreadToLeft; _x <= spreadToRight; _x++)
                            {
                                for (int _y = 0; _y <= height; _y++)
                                {
                                    if (x + _x < mapMinX || mapMinX == -1) mapMinX = x + _x;
                                    if (y - _y < mapMinY || mapMinY == -1) mapMinY = y - _y;
                                    if (x + _x > mapMaxX || mapMaxX == -1) mapMaxX = x + _x;
                                    if (y - _y > mapMaxY || mapMaxY == -1) mapMaxY = y - _y;
                                    if (tiles[x + _x, y - _y].roomNumber == -1)
                                        tiles[x + _x, y - _y].roomNumber = currentRoomNumber;
                                    if (_y == 0)
                                    {
                                        if (_x == spreadToRight)
                                            rightTopCorner = new Vector2Int(x + _x - 1, y - _y - 2);
                                        if (tiles[x + _x, y].floorType == -1)
                                        {
                                            tiles[x + _x, y].floorType = 1;
                                        }
                                        if (tiles[x + _x, y].floorType == 0)
                                        {
                                            tiles[x + _x, y].frontSideIsConnected = true;
                                            tiles[x + _x, y].rearSideIsConnected = true;
                                            tiles[x + _x, y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_x == -spreadToLeft)
                                    {
                                        if (_y == height)
                                        {
                                            leftBottomCorner = new Vector2Int(x + _x + 1, y - _y + 1);
                                            tiles[x + _x, y - _y].floorType = 1;
                                        }
                                        else if (tiles[x + _x, y - _y].floorType == -1)
                                        {
                                            if (_y > 2 && tiles[x + _x, y - _y + 3].floorType == 1 && Random.Range(0f, 100f) < 25f - jointsToLeftCount - _jointsToLeftCountCur * 10f)
                                            {
                                                tiles[x + _x, y - _y].floorType = 0;
                                                tiles[x + _x, y - _y + 1].floorType = 2;
                                                tiles[x + _x, y - _y + 2].floorType = 1;
                                                tiles[x + _x, y - _y].jointDirection = JointDirection.Left;
                                                tiles[x + _x, y - _y].rearSideIsConnected = true;
                                                jointsToLeftCount++;
                                                _jointsToLeftCountCur++;
                                                joints.Add(tiles[x + _x, y - _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y - _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x + _x, y - _y].floorType = 1;
                                            }
                                        }
                                        else if (tiles[x + _x, y - _y].floorType == 0)
                                        {
                                            tiles[x + _x, y - _y].frontSideIsConnected = true;
                                            tiles[x + _x, y - _y].rearSideIsConnected = true;
                                            tiles[x + _x, y - _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y - _y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_x == spreadToRight)
                                    {
                                        if (_y == height)
                                        {
                                            tiles[x + _x, y - _y].floorType = 1;
                                        }
                                        else if (tiles[x + _x, y - _y].floorType == -1)
                                        {
                                            if (_y > 2 && tiles[x + _x, y - _y + 3].floorType == 1 && Random.Range(0f, 100f) < 25f - jointsToRightCount - _jointsToRightCountCur * 10f)
                                            {
                                                tiles[x + _x, y - _y].floorType = 0;
                                                tiles[x + _x, y - _y + 1].floorType = 2;
                                                tiles[x + _x, y - _y + 2].floorType = 1;
                                                tiles[x + _x, y - _y].jointDirection = JointDirection.Right;
                                                tiles[x + _x, y - _y].rearSideIsConnected = true;
                                                jointsToRightCount++;
                                                _jointsToRightCountCur++;
                                                joints.Add(tiles[x + _x, y - _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y - _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x + _x, y - _y].floorType = 1;
                                            }
                                        }
                                        else if (tiles[x + _x, y - _y].floorType == 0)
                                        {
                                            tiles[x + _x, y - _y].frontSideIsConnected = true;
                                            tiles[x + _x, y - _y].rearSideIsConnected = true;
                                            tiles[x + _x, y - _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y - _y, 0), occupiedJoint);
                                        }
                                    }
                                    else
                                    {
                                        if (_y < height)
                                        {
                                            tiles[x + _x, y - _y].floorType = 0;
                                            if (tiles[x + _x, y].floorType != 0)
                                                tiles[x + _x, y - 1].floorType = 2;
                                        }
                                        else
                                        {
                                            if (tiles[x + _x, y - _y].floorType == 0)
                                            {
                                                tiles[x + _x, y - _y].frontSideIsConnected = true;
                                                tiles[x + _x, y - _y].rearSideIsConnected = true;
                                                tiles[x + _x, y - _y].isJointFinished = true;
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y - _y, 0), occupiedJoint);
                                            }
                                            else
                                            {
                                                if (tiles[x + _x, y - _y].floorType == -1 && Random.Range(0f, 100f) < 25f - jointsToBottomCount - _jointsToBottomCountCur * 10f)
                                                {
                                                    tiles[x + _x, y - _y].floorType = 0;
                                                    tiles[x + _x, y - _y].jointDirection = JointDirection.Down;
                                                    tiles[x + _x, y - _y].rearSideIsConnected = true;
                                                    jointsToBottomCount++;
                                                    _jointsToBottomCountCur++;
                                                    joints.Add(tiles[x + _x, y - _y]);
                                                    jointsTilemap.SetTile(new Vector3Int(x + _x, y - _y, 0), freeJoint);
                                                }
                                                else
                                                {
                                                    tiles[x + _x, y - _y].floorType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (shouldRejectJoint == true)
                {
                    currentJoint.isJointFinished = true;
                    currentJoint.frontSideIsConnected = true;
                    currentJoint.rearSideIsConnected = true;
                    currentJoint.isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(currentJoint.x, currentJoint.y, 0), occupiedJoint);
                }
                else
                {
                    rooms.Add(new MapRoom(leftBottomCorner, rightTopCorner, currentRoomNumber));
                    currentRoomNumber++;
                }
            }
            else if (currentJoint.jointDirection == JointDirection.Right)
            {
                int x = currentJoint.x;
                int y = currentJoint.y;
                bool shouldRejectJoint = false;
                if (tiles[x + 1, y].floorType != -1)
                    shouldRejectJoint = true;
                else
                {
                    int spreadToBottom = Random.Range(1, maxRoomHeight);
                    for (int i = 1; i <= spreadToBottom; i++)
                    {
                        if (tiles[x + 1, y - i].floorType != -1)
                        {
                            spreadToBottom = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    int spreadToTop = spreadToBottom <= 2 ? Random.Range(maxRoomHeight - spreadToBottom - 2, maxRoomHeight - spreadToBottom) : Random.Range(2, maxRoomHeight);
                    for (int i = 1; i <= spreadToTop; i++)
                    {
                        if (tiles[x + 1, y + i].floorType != -1)
                        {
                            spreadToTop = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    //   spread to bottom 
                    // + spread to top 
                    // + one tile beneath the joint  }
                    // - one top wall                } 1 - 1 - 1 = -1
                    // - one bottom wall             }
                    if (spreadToBottom + spreadToTop - 1 < 4)
                    {
                        //print("Too low room at (" + x + "; " + y + ")");
                        return false;
                    }
                    else
                    {
                        int chestsCount = 0;
                        int creaturesCount = 0;

                        // + lower wall
                        int width = Random.Range(maxRoomHeight - 3, maxRoomHeight) + 1;
                        int maxWidth = width;
                        for (int k = -spreadToBottom + 1; k < spreadToTop; k++)
                        {
                            for (int i = 1; i <= width; i++)
                            {
                                if (tiles[x + i, y + k].floorType != -1)
                                {
                                    maxWidth = i - 1 >= 0 ? i - 1 : 0;
                                    break;
                                }
                            }
                            if (width > maxWidth)
                                width = maxWidth;
                        }

                        if (width < 4)
                        {
                            //print("Too narrow room at (" + x + "; " + y + "): width = " + width);
                            return false;
                        }
                        else
                        {
                            for (int _y = -spreadToBottom; _y <= spreadToTop; _y++)
                            {
                                for (int _x = 0; _x <= width; _x++)
                                {
                                    if (x + _x < mapMinX || mapMinX == -1) mapMinX = x + _x;
                                    if (y + _y < mapMinY || mapMinY == -1) mapMinY = y + _y;
                                    if (x + _x > mapMaxX || mapMaxX == -1) mapMaxX = x + _x;
                                    if (y + _y > mapMaxY || mapMaxY == -1) mapMaxY = y + _y;
                                    if (tiles[x + _x, y + _y].roomNumber == -1)
                                        tiles[x + _x, y + _y].roomNumber = currentRoomNumber;
                                    if (_x == 0)
                                    {
                                        if (_y == -spreadToBottom)
                                            leftBottomCorner = new Vector2Int(x + 1, y + _y + 1);
                                        if (tiles[x + _x, y + _y].floorType == -1)
                                        {
                                            tiles[x + _x, y + _y].floorType = 1;
                                        }
                                        if (tiles[x + _x, y + _y].floorType == 0)
                                        {
                                            tiles[x + _x, y + _y].frontSideIsConnected = true;
                                            tiles[x + _x, y + _y].rearSideIsConnected = true;
                                            tiles[x + _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_y == -spreadToBottom)
                                    {
                                        if (_x == width)
                                        {
                                            tiles[x + _x, y + _y].floorType = 1;
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == -1)
                                        {
                                            if (_x > 0 && Random.Range(0f, 100f) < 25f - jointsToBottomCount - _jointsToBottomCountCur * 10f)
                                            {
                                                tiles[x + _x, y + _y].floorType = 0;
                                                tiles[x + _x, y + _y].jointDirection = JointDirection.Down;
                                                tiles[x + _x, y + _y].rearSideIsConnected = true;
                                                jointsToLeftCount++;
                                                _jointsToLeftCountCur++;
                                                joints.Add(tiles[x + _x, y + _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x + _x, y + _y].floorType = 1;
                                            }
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == 0)
                                        {
                                            tiles[x +_x, y + _y].frontSideIsConnected = true;
                                            tiles[x + _x, y + _y].rearSideIsConnected = true;
                                            tiles[x + _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_y == spreadToTop)
                                    {
                                        if (_x == width)
                                        {
                                            rightTopCorner = new Vector2Int(x + _x - 1, y + _y - 2);
                                            tiles[x + _x, y + _y].floorType = 1;
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == -1)
                                        {
                                            if (_x > 0 && Random.Range(0f, 100f) < 25f - jointsToTopCount - _jointsToTopCountCur * 10f)
                                            {
                                                tiles[x + _x, y + _y].floorType = 0;
                                                tiles[x + _x, y + _y].jointDirection = JointDirection.Up;
                                                tiles[x + _x, y + _y].rearSideIsConnected = true;
                                                jointsToRightCount++;
                                                _jointsToRightCountCur++;
                                                joints.Add(tiles[x + _x, y + _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x + _x, y + _y].floorType = 1;
                                                if (_x > 0)
                                                    tiles[x + _x, y + _y - 1].floorType = 2;
                                            }
                                        }
                                        else if (tiles[x + _x, y + _y].floorType == 0)
                                        {
                                            tiles[x + _x, y + _y].frontSideIsConnected = true;
                                            tiles[x + _x, y + _y].rearSideIsConnected = true;
                                            tiles[x + _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else
                                    {
                                        if (_x < width)
                                        {
                                            tiles[x + _x, y + _y].floorType = 0;
                                        }
                                        else
                                        {
                                            if (tiles[x + _x, y + _y].floorType == 0)
                                            {
                                                tiles[x + _x, y + _y].frontSideIsConnected = true;
                                                tiles[x + _x, y + _y].rearSideIsConnected = true;
                                                tiles[x + _x, y + _y].isJointFinished = true;
                                                jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y, 0), occupiedJoint);
                                            }
                                            else
                                            {
                                                if (_y > spreadToBottom && tiles[x + _x, y + _y + 1].floorType == 1 && tiles[x + _x, y + _y].floorType == -1 && Random.Range(0f, 100f) < 25f - jointsToRightCount - _jointsToRightCountCur * 10f)
                                                {
                                                    tiles[x + _x, y + _y].floorType = 2;
                                                    tiles[x + _x, y + _y - 1].floorType = 0;
                                                    tiles[x + _x, y + _y - 1].jointDirection = JointDirection.Right;
                                                    tiles[x + _x, y + _y - 1].rearSideIsConnected = true;
                                                    jointsToBottomCount++;
                                                    _jointsToBottomCountCur++;
                                                    joints.Add(tiles[x + _x, y + _y - 1]);
                                                    jointsTilemap.SetTile(new Vector3Int(x + _x, y + _y - 1, 0), freeJoint);
                                                }
                                                else
                                                {
                                                    tiles[x + _x, y + _y].floorType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (shouldRejectJoint == true)
                {
                    currentJoint.isJointFinished = true;
                    currentJoint.frontSideIsConnected = true;
                    currentJoint.rearSideIsConnected = true;
                    currentJoint.isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(currentJoint.x, currentJoint.y, 0), occupiedJoint);
                }
                else
                {
                    rooms.Add(new MapRoom(leftBottomCorner, rightTopCorner, currentRoomNumber));
                    currentRoomNumber++;
                }
            }
            else if (currentJoint.jointDirection == JointDirection.Left)
            {
                int x = currentJoint.x;
                int y = currentJoint.y;
                bool shouldRejectJoint = false;
                if (tiles[x - 1, y].floorType != -1)
                    shouldRejectJoint = true;
                else
                {
                    int spreadToBottom = Random.Range(1, maxRoomHeight);
                    for (int i = 1; i <= spreadToBottom; i++)
                    {
                        if (tiles[x - 1, y - i].floorType != -1)
                        {
                            spreadToBottom = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    int spreadToTop = spreadToBottom <= 2 ? Random.Range(maxRoomHeight - spreadToBottom - 2, maxRoomHeight - spreadToBottom) : Random.Range(2, maxRoomHeight);
                    for (int i = 1; i <= spreadToTop; i++)
                    {
                        if (tiles[x - 1, y + i].floorType != -1)
                        {
                            spreadToTop = i >= 1 ? i : 1;
                            break;
                        }
                    }

                    //   spread to bottom 
                    // + spread to top 
                    // + one tile beneath the joint  }
                    // - one top wall                } 1 - 1 - 1 = -1
                    // - one bottom wall             }
                    if (spreadToBottom + spreadToTop - 1 < 4)
                    {
                        //print("Too low room at (" + x + "; " + y + ")");
                        return false;
                    }
                    else
                    {
                        int chestsCount = 0;
                        int creaturesCount = 0;

                        // + lower wall
                        int width = Random.Range(maxRoomHeight - 3, maxRoomHeight) + 1;
                        int maxWidth = width;
                        for (int k = -spreadToBottom + 1; k < spreadToTop; k++)
                        {
                            for (int i = 1; i <= width; i++)
                            {
                                if (tiles[x - i, y + k].floorType != -1)
                                {
                                    maxWidth = i - 1 >= 0 ? i - 1 : 0;
                                    break;
                                }
                            }
                            if (width > maxWidth)
                                width = maxWidth;
                        }

                        if (width < 4)
                        {
                            //print("Too narrow room at (" + x + "; " + y + "): width = " + width);
                            return false;
                        }
                        else
                        {
                            for (int _y = -spreadToBottom; _y <= spreadToTop; _y++)
                            {
                                for (int _x = 0; _x <= width; _x++)
                                {
                                    if (x - _x < mapMinX || mapMinX == -1) mapMinX = x - _x;
                                    if (y + _y < mapMinY || mapMinY == -1) mapMinY = y + _y;
                                    if (x - _x > mapMaxX || mapMaxX == -1) mapMaxX = x - _x;
                                    if (y + _y > mapMaxY || mapMaxY == -1) mapMaxY = y + _y;
                                    if (tiles[x - _x, y + _y].roomNumber == -1)
                                        tiles[x - _x, y + _y].roomNumber = currentRoomNumber;
                                    if (_x == 0)
                                    {
                                        if (tiles[x - _x, y + _y].floorType == -1)
                                        {
                                            tiles[x - _x, y + _y].floorType = 1;
                                        }
                                        if (tiles[x - _x, y + _y].floorType == 0)
                                        {
                                            tiles[x - _x, y + _y].frontSideIsConnected = true;
                                            tiles[x - _x, y + _y].rearSideIsConnected = true;
                                            tiles[x - _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_y == -spreadToBottom)
                                    {
                                        if (_x == width)
                                        {
                                            leftBottomCorner = new Vector2Int(x - _x + 1, y + _y + 1);
                                            tiles[x - _x, y + _y].floorType = 1;
                                        }
                                        else if (tiles[x - _x, y + _y].floorType == -1)
                                        {
                                            if (_x > 0 && Random.Range(0f, 100f) < 25f - jointsToBottomCount - _jointsToBottomCountCur * 10f)
                                            {
                                                tiles[x - _x, y + _y].floorType = 0;
                                                tiles[x - _x, y + _y].jointDirection = JointDirection.Down;
                                                tiles[x - _x, y + _y].rearSideIsConnected = true;
                                                jointsToLeftCount++;
                                                _jointsToLeftCountCur++;
                                                joints.Add(tiles[x - _x, y + _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x - _x, y + _y].floorType = 1;
                                            }
                                        }
                                        else if (tiles[x - _x, y + _y].floorType == 0)
                                        {
                                            tiles[x - _x, y + _y].frontSideIsConnected = true;
                                            tiles[x - _x, y + _y].rearSideIsConnected = true;
                                            tiles[x - _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else if (_y == spreadToTop)
                                    {
                                        if (_x == width)
                                        {
                                            rightTopCorner = new Vector2Int(x - 1, y + _y - 2);
                                            tiles[x - _x, y + _y].floorType = 1;
                                        }
                                        else if (tiles[x - _x, y + _y].floorType == -1)
                                        {
                                            if (_x > 0 && Random.Range(0f, 100f) < 25f - jointsToTopCount - _jointsToTopCountCur * 10f)
                                            {
                                                tiles[x - _x, y + _y].floorType = 0;
                                                tiles[x - _x, y + _y].jointDirection = JointDirection.Up;
                                                tiles[x - _x, y + _y].rearSideIsConnected = true;
                                                jointsToRightCount++;
                                                _jointsToRightCountCur++;
                                                joints.Add(tiles[x - _x, y + _y]);
                                                jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y, 0), freeJoint);
                                            }
                                            else
                                            {
                                                tiles[x - _x, y + _y].floorType = 1;
                                                if (_x > 0)
                                                    tiles[x - _x, y + _y - 1].floorType = 2;
                                            }
                                        }
                                        else if (tiles[x - _x, y + _y].floorType == 0)
                                        {
                                            tiles[x - _x, y + _y].frontSideIsConnected = true;
                                            tiles[x - _x, y + _y].rearSideIsConnected = true;
                                            tiles[x - _x, y + _y].isJointFinished = true;
                                            jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y, 0), occupiedJoint);
                                        }
                                    }
                                    else
                                    {
                                        if (_x < width)
                                        {
                                            tiles[x - _x, y + _y].floorType = 0;
                                        }
                                        else
                                        {
                                            if (tiles[x - _x, y + _y].floorType == 0)
                                            {
                                                tiles[x - _x, y + _y].frontSideIsConnected = true;
                                                tiles[x - _x, y + _y].rearSideIsConnected = true;
                                                tiles[x - _x, y + _y].isJointFinished = true;
                                                jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y, 0), occupiedJoint);
                                            }
                                            else
                                            {
                                                if (_y > spreadToBottom && tiles[x - _x, y + _y + 1].floorType == 1 && tiles[x - _x, y + _y].floorType == -1 && Random.Range(0f, 100f) < 25f - jointsToRightCount - _jointsToRightCountCur * 10f)
                                                {
                                                    tiles[x - _x, y + _y].floorType = 2;
                                                    tiles[x - _x, y + _y - 1].floorType = 0;
                                                    tiles[x - _x, y + _y - 1].jointDirection = JointDirection.Left;
                                                    tiles[x - _x, y + _y - 1].rearSideIsConnected = true;
                                                    jointsToBottomCount++;
                                                    _jointsToBottomCountCur++;
                                                    joints.Add(tiles[x - _x, y + _y - 1]);
                                                    jointsTilemap.SetTile(new Vector3Int(x - _x, y + _y - 1, 0), freeJoint);
                                                }
                                                else
                                                {
                                                    tiles[x - _x, y + _y].floorType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (shouldRejectJoint == true)
                {
                    currentJoint.isJointFinished = true;
                    currentJoint.frontSideIsConnected = true;
                    currentJoint.rearSideIsConnected = true;
                    currentJoint.isJointCanceled = true;
                    jointsTilemap.SetTile(new Vector3Int(currentJoint.x, currentJoint.y, 0), occupiedJoint);
                }
                else
                {
                    rooms.Add(new MapRoom(leftBottomCorner, rightTopCorner, currentRoomNumber));
                    currentRoomNumber++;
                }
            }
        }

        SmoothenDeadEnds();
        List<GenerationMapTile> possibleJoints = joints.Where(j => j.isJointFinished == false).ToList();
        if (possibleJoints.Count > 0)
            currentJoint = possibleJoints[Random.Range(0, possibleJoints.Count)];

        return true;
    }
}