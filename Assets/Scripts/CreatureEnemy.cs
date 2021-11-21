using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureEnemy : Creature
{
    public Creature target;
    public bool lootIsInitialized;
    public bool isTryingToFlee;
    //public int viewDistance;

    //public GameObject go;

    public bool CheckCellToNonPlayer(int cellX, int cellY)
    {
        bool result = false;

        if (cellX >= 0
            && cellY >= 0
            && cellX < map.mapSizeX
            && cellY < map.mapSizeY
            && map.cells[cellX, cellY].zoneNumber == map.cells[x, y].zoneNumber
            && map.cells[cellX, cellY].cellObject != null
            && map.cells[cellX, cellY].cellObject.GetComponent<PlayerController>() == null
            && map.cells[cellX, cellY].cellObject.GetComponent<Creature>() != null)
            result = true;
        return result;
    }

    public bool CheckCellToPlayer(int cellX, int cellY)
    {
        bool result = false;

        if (cellX >= 0 
            && cellY >= 0 
            && cellX < map.mapSizeX 
            && cellY < map.mapSizeY
            && map.cells[cellX, cellY].zoneNumber == map.cells[x, y].zoneNumber
            && map.cells[cellX, cellY].cellObject != null
            && map.cells[cellX, cellY].cellObject.GetComponent<PlayerController>() != null)
            result = true;
        return result;
    }

    public MapCell GetCellToFlee()
    {
        MapCell result = map.cells[x, y];
        for (int i = GetStateValue("view_dist") - 1; i > 1; i--)
        {
            var _x = i - 1;
            var _y = 0;
            var dx = 1;
            var dy = 1;
            var diameter = i * 2;
            var decisionOver2 = dx - diameter;

            while (_x >= _y)
            {
                if (map.CheckCell(_x + x, _y + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[_x + x, _y + y];
                if (map.CheckCell(_x + x, -_y + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[_x + x, -_y + y];
                if (map.CheckCell(-_x + x, -_y + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[-_x + x, -_y + y];
                if (map.CheckCell(-_x + x, _y + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[-_x + x, _y + y];
                if (map.CheckCell(_y + x, _x + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[_y + x, _x + y];
                if (map.CheckCell(_y + x, -_x + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[_y + x, -_x + y];
                if (map.CheckCell(-_y + x, -_x + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[-_y + x, -_x + y];
                if (map.CheckCell(-_y + x, _x + y, CellCheckFlags.ToPassability) == true)
                    return map.cells[-_y + x, _x + y];

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
        return result;
    }

    public Creature GetPlayerTarget()
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
                if (CheckCellToPlayer(_x + x, _y + y) == true)
                    return map.cells[_x + x, _y + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(_x + x, -_y + y) == true)
                    return map.cells[_x + x, -_y + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(-_x + x, -_y + y) == true)
                    return map.cells[-_x + x, -_y + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(-_x + x, _y + y) == true)
                    return map.cells[-_x + x, _y + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(_y + x, _x + y) == true)
                    return map.cells[_y + x, _x + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(_y + x, -_x + y) == true)
                    return map.cells[_y + x, -_x + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(-_y + x, -_x + y) == true)
                    return map.cells[-_y + x, -_x + y].cellObject.GetComponent<PlayerController>();
                if (CheckCellToPlayer(-_y + x, _x + y) == true)
                    return map.cells[-_y + x, _x + y].cellObject.GetComponent<PlayerController>();

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
    public Creature GetPlayerOrSummonedByPlayerTarget()
    {
        Creature result = GetPlayerTarget();
        if (GetPlayerTarget() == null)
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
                    if (CheckCellToNonPlayer(_x + x, _y + y) == true && map.cells[_x + x, _y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true 
                        && map.cells[_x + x, _y + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[_x + x, _y + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(_x + x, -_y + y) == true && map.cells[_x + x, -_y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true 
                        && map.cells[_x + x, -_y + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[_x + x, -_y + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(-_x + x, -_y + y) == true && map.cells[-_x + x, -_y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true 
                        && map.cells[-_x + x, -_y + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[-_x + x, -_y + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(-_x + x, _y + y) == true && map.cells[-_x + x, _y + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true
                         && map.cells[-_x + x, _y + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[-_x + x, _y + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(_y + x, _x + y) == true && map.cells[_y + x, _x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true
                        && map.cells[_y + x, _x + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[_y + x, _x + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(_y + x, -_x + y) == true && map.cells[_y + x, -_x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true
                        && map.cells[_y + x, -_x + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[_y + x, -_x + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(-_y + x, -_x + y) == true && map.cells[-_y + x, -_x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true
                         && map.cells[-_y + x, -_x + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
                        return map.cells[-_y + x, -_x + y].cellObject.GetComponent<Creature>();
                    if (CheckCellToNonPlayer(-_y + x, _x + y) == true && map.cells[-_y + x, _x + y].cellObject.GetComponent<Creature>().isSummonedByPlayer == true
                        && map.cells[-_y + x, _x + y].cellObject.GetComponent<Creature>().canBeTargetedByAI == true)
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
        }
        else return result;

        return null;
    }

    /*
    public void GetPathToTarget()
    {
        if (target != null)
            currentPath = map.FindPath(map.cells[x, y], map.cells[target.x, target.y], this);
        if (currentPath == null || currentPath.Length == 0 || currentPath.Length > viewDistance)
            Wait();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        movesRemaining = maximumMoves;

        if (target == null)
        {
            Creature _target = GetPlayerTarget();
            if (_target != null)
                target = _target;
            else if (lastAttackedCreature != null)
            {
                target = lastAttackedCreature;
            }
        }

        GetPathToTarget();
    }

    public bool moving;
    public IEnumerator MovePath()
    {
        int index = currentPath.Length - 1;
        moving = true;
        bool reachTarget = true;

        if (currentPath.Length > 1)
        {
            while (Vector3.Distance(transform.position, new Vector3(currentPath[1].x, currentPath[1].y)) > 0.01f)
            {
                PlayAnimationByPath(index);

                while (Vector3.Distance(transform.position, new Vector3(currentPath[index].x, currentPath[index].y)) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentPath[index].x, currentPath[index].y), 3f * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }

                if (currentPath[index - 1].cellObject != null && currentPath[index - 1].cellObject != target)
                {
                    reachTarget = false;
                    map.cells[x, y].cellObject = null;
                    x = currentPath[index].x;
                    y = currentPath[index].y;
                    map.cells[x, y].cellObject = this;
                    transform.position = new Vector3(x, y);
                    Wait();
                    yield break;
                }

                movesRemaining--;
                if (movesRemaining <= 0)
                {
                    reachTarget = false;
                    break;
                }

                if (index > 1)
                {
                    index--;
                }
            }

            moving = false;

            map.cells[x, y].cellObject = null;
            x = currentPath[index].x;
            y = currentPath[index].y;
            map.cells[x, y].cellObject = this;
            transform.position = new Vector3(x, y);
            moving = false;
        }
        else
        {
            PlayAnimationByPath(index);
            //yield return new WaitForSeconds(autoattackAnimationDelay);
            moving = false;
        }

        if (reachTarget == true && movesRemaining > 0)
            Autoattack();
        else StartCoroutine(EndTurn(nextTurnDelay));
    }
    */

    public virtual void BuildPath(int destinationX, int destinationY)
    {
        currentPath = map.FindPath(map.cells[x, y], map.cells[destinationX, destinationY], this);
    }

    /*
    public override void Autoattack()
    {
        base.Autoattack();
        movesRemaining--;
        lastAttackedCreature = target;
    }
    */
}