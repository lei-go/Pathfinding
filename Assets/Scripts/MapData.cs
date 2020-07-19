using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class MapData : MonoBehaviour
{
    public int width = 10;
    public int height = 5;
    public TextAsset textAsset;
    public Texture2D textureMap;
    public string resourcePath = "MapData";

    public List<string> GetMapFromTextFile(TextAsset tAsset)
    {
        List<string> lines = new List<string>();

        if (tAsset != null)
        {
            string textData = tAsset.text;
            string[] delimiters = {"\r\n","\n"};
            lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None));

            lines.Reverse();
        }
        else
        {
            Debug.LogWarning("MAP Invalid text asset");
        }

        return lines;
    }

    public List<string> GetTextFromTextFile()
    {
        if (textAsset == null)
        {
            string levelName = SceneManager.GetActiveScene().name;
            textAsset = Resources.Load(resourcePath + "/" + levelName) as TextAsset; //unity special resources grabbing method
        }
        return GetMapFromTextFile(textAsset);
    }

    public List<string> GetMapFromTexture(Texture2D tAsset)
    {
        List<string> lines = new List<string>();

        for (int y = 0; y < tAsset.height; y++)
        {
            string newLine = "";

            for (int x = 0; x < tAsset.width; x++)
            {
                if (tAsset.GetPixel(x,y) == Color.black) newLine += '1';
                else if (tAsset.GetPixel(x,y) == Color.white) newLine += '0';
                else newLine += ' ';
            }

            lines.Add(newLine);
        }

        return lines;
    }
    public List<string> GetMapFromTexture()
    {
        if (textureMap == null)
        {
            string levelName = SceneManager.GetActiveScene().name;
            textureMap = Resources.Load(resourcePath + "/" + levelName) as Texture2D; //unity special resources grabbing method
        }
        return GetMapFromTexture(textureMap);
    }
    public void SetDimensions(List<string> textLines)
    {
        height = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > width)
            {
                width = line.Length;
            }
        }
    }

    public int[,] MakeMap()
    {
        List<string> lines = new List<string>();
        if (textureMap != null) lines = GetMapFromTexture();
        else lines = GetTextFromTextFile();
        SetDimensions(lines);

        int[,] map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (lines[y].Length > x) //why
                {
                    map[x,y] = (int) Char.GetNumericValue(lines[y][x]);
                }
            }
        }


        return map;
    }
}