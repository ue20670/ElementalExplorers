﻿using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using XNode;

[CreateNodeMenu("Terrain/Elevation Open Topography (High Resolution)")]
public class OpenTopographyElevationNode : ExtendedNode {

    public const string APIKey = "AtK3XHD1AaSGDXOTdtiNlf24CbNMdvGM6fRpHynP6a4RHuc3m7goqqxgunAXuEI3";
    private const int Width = 32;
    [Input] public GlobeBoundingBox boundingBox;
    [Input] public string dem = "NASADEM";
    [Output] public ElevationData elevationData;

    private Texture2D preview;

    // Use this for initialization
    protected override void Init()
    {
        base.Init();

    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "elevationData")
        {
            return elevationData;
        }
        return null;
    }


    //Converts ASCII DEM file to a 2D array of normalised heights
    //Ready for Unity terrain
    public ElevationData ASCToHeightmap(string file, GlobeBoundingBox box)
    {
        string[] lines = file.Split('\n');
        //meta-data
        //assume non-square resolution, force square resolution later
        int resX = int.Parse(lines[0].Substring(5));
        int resY = int.Parse(lines[1].Substring(5));

        float[,] inputHeights = new float[resY, resX];

        double maxHeight = double.MinValue;
        double minHeight = double.MaxValue;

        //read each row
        for (int y = 0; y < resY; y++)
        {
            string[] xValues = lines[y + 6].Split(' ');

            //read each column
            for (int x = 0; x < resX; x++)
            {
                //there is a whitespace at the beginning of each row, so offsetted to exclude this
                double h = float.Parse(xValues[x + 1]);

                //update constraints
                if (h > maxHeight)
                {
                    maxHeight = h;
                }
                else if (h < minHeight)
                {
                    minHeight = h;
                }

                inputHeights[resY - y - 1, x] = (float)h;
            }
        }

        return new ElevationData(inputHeights, box, minHeight - 5.0, maxHeight);
    }

    public override void CalculateOutputs(Action<bool> callback)
    {
        GlobeBoundingBox box = GetInputValue("boundingBox", boundingBox);
        string demType = GetInputValue("dem", dem);
        string url = $"https://portal.opentopography.org/API/globaldem?demtype={demType}&south={box.south}&north={box.north}&west={box.west}&east={box.east}&outputFormat=AAIGrid&API_Key=057f5757f717ae10f196355c3f3cc29b";
        UnityWebRequest request = UnityWebRequest.Get(url);

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        operation.completed += (AsyncOperation operation) =>
        {
            //Failure
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                callback.Invoke(false);
            }
            else
            {
                //Convert into TerrainImport and send to callback function
                elevationData = ASCToHeightmap(request.downloadHandler.text, box);

                preview = new Texture2D(elevationData.height.GetLength(0), elevationData.height.GetLength(1));
                for (int i = 0; i < elevationData.height.GetLength(0); i++)
                {
                    for (int j = 0; j < elevationData.height.GetLength(1); j++)
                    {
                        float h = elevationData.height[i, j];
                        preview.SetPixel(i, j, new Color(h, h, h));
                    }
                }
                callback.Invoke(true);
            }
            preview.Apply();

            request.Dispose();
        };
    }

#if UNITY_EDITOR
    public override void ApplyGUI()
    {
        base.ApplyGUI();

        EditorGUILayout.LabelField(new GUIContent(preview), GUILayout.Width(128), GUILayout.Height(128));
    }
#endif

    public override void Release()
    {
        elevationData = null;
        preview = null;
    }
}