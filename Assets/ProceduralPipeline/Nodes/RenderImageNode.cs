﻿using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using XNode;

public class RenderImageNode : ExtendedNode {
	[Input] public GlobeBoundingBox boundingBox;
	[Input] public LayerMask renderMask;
	[Input] public int resolution = 256;
	[Input] public GameObject[] stage;
	[Output] public Texture2D render;
	[Output] public GameObject[] stagePassthrough;

	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		if (port.fieldName == "render")
		{
			return render;
		}

		if (port.fieldName == "stagePassthrough")
		{
			return stagePassthrough;
		}
		return null; // Replace this
	}

	public override void CalculateOutputs(Action<bool> callback)
	{
		GlobeBoundingBox bbox = GetInputValue("boundingBox", boundingBox);
		LayerMask mask = GetInputValue("renderMask", renderMask);
		int res = GetInputValue("resolution", resolution);
		GameObject cameraGO = new GameObject();
		float width = (float)GlobeBoundingBox.LatitudeToMeters(bbox.north - bbox.south);
		cameraGO.transform.position = new Vector3(width / 2, 1000, width / 2);
		cameraGO.transform.eulerAngles = new Vector3(90, 0, 0);

        Camera cam = cameraGO.AddComponent<Camera>();
        cam.farClipPlane = 2000;
		cam.orthographic = true;
		cam.orthographicSize = width / 2;
		cam.cullingMask = mask.value;
        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = Color.black;

		RenderTexture tex = RenderTexture.active;
		RenderTexture temp = RenderTexture.GetTemporary(res, res);
		cam.targetTexture = temp;
		RenderTexture.active = cam.targetTexture;

		cam.Render();

		render = new Texture2D(res, res);
        render.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        render.Apply();

		RenderTexture.active = tex;

		RenderTexture.ReleaseTemporary(temp);
		DestroyImmediate(cameraGO);

		stagePassthrough = GetInputValue("stage", stage);

		callback.Invoke(true);
    }

#if UNITY_EDITOR
    public override void ApplyGUI()
	{
		base.ApplyGUI();

        EditorGUILayout.LabelField(new GUIContent(render), GUILayout.Width(128), GUILayout.Height(128));
    }
#endif

	public override void Release()
	{
		render = null;
		stage = null;
		stagePassthrough = null;
	}
}