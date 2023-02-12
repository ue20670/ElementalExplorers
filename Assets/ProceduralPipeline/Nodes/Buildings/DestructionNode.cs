using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using XNode;

[CreateNodeMenu("Buildings/Add Building Destruction")]

public class DestructionNode : ExtendedNode
{
    [Input] public GameObject[] inputBuildings;
    [Input] public float quantity;
    [Input] public float scale;
    [Output] public GameObject[] outputBuildings;

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {

        if (port.fieldName == "outputBuildings")
        {
            return inputBuildings;
        }
        return null;
    }

    public override void CalculateOutputs(Action<bool> callback)
    {
        // setup inputs
        GameObject[] buildings = GetInputValue("buildingData", inputBuildings);
        float q = GetInputValue("buildingData", quantity);
        float s = GetInputValue("buildingData", scale);

        // setup outputs
        List<GameObject> gameObjects = new List<GameObject>();



        foreach (GameObject building in buildings)
        {
            GameObject buildingGO = AddBuildingDestruction(building, q, s);
            gameObjects.Add(buildingGO);
        }

        outputBuildings = gameObjects.ToArray();
        callback.Invoke(true);
    }

    private GameObject AddBuildingDestruction(GameObject building, float q, float s)
    {
        return building;
    }
}
