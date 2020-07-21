using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapData : MonoBehaviour
{
    public int width = 10;
    public int height = 5;
    public TextAsset textAsset;
    public Texture2D textureMap;
    public string resourcePath = "MapData";

    public Color32 openColor = Color.white;
    public Color32 blockedColor = Color.black;
    public Color32 lightTerrainColor = new Color32(124, 194, 78, 255);
    public Color32 mediumTerrainColor = new Color32(252, 255, 52, 255);
    public Color32 heavyTerrainColor = new Color32(255, 129, 12, 255);

    static Dictionary<Color32,NodeType> terrainLookupTable = new Dictionary<Color32, NodeType>();

    void Awake()
    {
        SetupLookupTable();
    }
//todo: add a start function to pull texture
    void Start()
    {
        string levelName = SceneManager.GetActiveScene().name;
        if (textureMap == null)
        {
            textureMap = Resources.Load(resourcePath + "/" + levelName) as Texture2D;
        }
        if (textAsset == null)
        {
            textAsset = Resources.Load(resourcePath + "/" + levelName) as TextAsset;
        }
    }

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
                Color32 pixelColor = textureMap.GetPixel(x,y);

                if (terrainLookupTable.ContainsKey(pixelColor))
                {
                    NodeType nodeType = terrainLookupTable[pixelColor];
                    int nodeTypeNum = (int) nodeType;
                    newLine += nodeTypeNum;
                }
                else
                {
                    newLine += 0;
                }
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

    void SetupLookupTable()
    {
        terrainLookupTable.Add(openColor, NodeType.Open);
        terrainLookupTable.Add(blockedColor,NodeType.Blocked);
        terrainLookupTable.Add(lightTerrainColor,NodeType.LightTerrain);
        terrainLookupTable.Add(mediumTerrainColor,NodeType.MediumTerrain);
        terrainLookupTable.Add(heavyTerrainColor,NodeType.HeavyTerrain);
    }

    public static Color32 GetColorFromNodeType(NodeType nodeType)
    {
        if (terrainLookupTable.ContainsValue(nodeType))
        {
            Color colorKey = terrainLookupTable.FirstOrDefault(x => x.Value == nodeType).Key;
            return colorKey;
        }
        return Color.white;
    }

}