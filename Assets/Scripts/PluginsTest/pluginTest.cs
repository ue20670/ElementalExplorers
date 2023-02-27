using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class pluginTest : MonoBehaviour
{
    // Import and expose native c++ functions
    [DllImport("TESTPLUGIN", EntryPoint = "SimpleReturn")] public static extern int SimpleReturn();
    // [DllImport("testUnityPlugin", EntryPoint = "getRandom")] public static extern int getRandom();
    // [DllImport("testUnityPlugin", EntryPoint = "displaySum")] public static extern int displaySum();
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SimpleReturn());
    }

}
