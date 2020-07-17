using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public int width = 10;
    public int height = 5;

    public int[,] MakeMap()
    {
        int[,] map = new int[width, height];
        /* //at this point for loop is not necessary as default for [,] are 0s
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x,y] = 0; //will add more stuff in here
            }
        }
        */ 

        for (int y = 0; y < 3; y++)
        {
            for (int x = 4; x < 6; x++)
            {
                map[x,y] = 1;
            }
        }

        for (int y = 2; y < height; y++)
        {
            for (int x = 7; x < 9; x++)
            {
                map[x,y] = 1;
            }
        }

        return map;
    }
}