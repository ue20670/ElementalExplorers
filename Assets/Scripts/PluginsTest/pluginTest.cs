using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class pluginTest : MonoBehaviour
{
    // Import and expose native c++ functions
    [DllImport("mcut-wrapper", EntryPoint = "TestDLLWorking")]
    public static extern int TestDLLWorking();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(TestDLLWorking());
    }
}