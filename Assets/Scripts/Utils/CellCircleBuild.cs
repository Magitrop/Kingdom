using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCircleBuild : MonoBehaviour
{
    public int xCenter, yCenter;
    public int radius;
    public void Build()
    {
        int _x = 0;
        int _y = radius;
        int d = 3 - 2 * _y;
        while (_x <= _y)
        {
            /*
            points:
            (_x + xCenter, _y + yCenter)
            (_x + xCenter, -_y + yCenter)
            (-_x + xCenter, -_y + yCenter)
            (-_x + xCenter, _y + yCenter)
            (_y + xCenter, _x + yCenter)
            (_y + xCenter, -_x + yCenter)
            (-_y + xCenter, -_x + yCenter)
            (-_y + xCenter, _x + yCenter)
            */
            if (d < 0)
                d = d + 4 * _x + 6;
            else
            {
                d = d + 4 * (_x - _y) + 10;
                _y--;
            }
            _x++;
        }
    }
}