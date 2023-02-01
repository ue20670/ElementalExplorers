﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XNode;

public class BingWaterMaskNode : ExtendedNode
{
	public const string APIKey = "AtK3XHD1AaSGDXOTdtiNlf24CbNMdvGM6fRpHynP6a4RHuc3m7goqqxgunAXuEI3";
	public const string MapType = "CanvasGray";
	[Input] public GlobeBoundingBox boundingBox;
	[Input] public int resolution = 512;
	[Output] public Texture2D waterMask;

	// Use this for initialization
	protected override void Init()
	{
		base.Init();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		if (port.fieldName == "waterMask")
		{
			return waterMask;
		}
		return null;
	}

	public override void CalculateOutputs(Action<bool> callback)
	{
		int res = GetInputValue("resolution", resolution);
		GlobeBoundingBox box = GetInputValue("boundingBox", boundingBox);
		string url = $"https://dev.virtualearth.net/REST/v1/Imagery/Map/{MapType}?mapArea={box.south},{box.west},{box.north},{box.east}&mapSize={resolution},{resolution}&style=me|lv:0_ar|v:0_trs|v:0_cr|bsc:444444;boc:00000000;fc:888888;v:1_ad|bv:0_wt|fc:ffffff_pt|v:0&format=png&mapMetadata=0&key={APIKey}";
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

		UnityWebRequestAsyncOperation operation = request.SendWebRequest();
		operation.completed += (AsyncOperation operation) =>
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(request.error);
				callback.Invoke(false);
			}
			else
			{
				waterMask = DownloadHandlerTexture.GetContent(request);
				callback.Invoke(true);
			}

			request.Dispose();
		};
	}
}