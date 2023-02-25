using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class OptimisationBenchmarker : EditorWindow
{
    // -----------------------------------------------------------------------------------------------------------------
    // GENERAL VARIABLES
    // -----------------------------------------------------------------------------------------------------------------

    // methods shown in toolbar
    private readonly string[] _optimisationMethod =
    {
        "Static Batching",
        "GPU Instancing",
        "GPU Instancing Indirect"
    };

    // currently selected method
    private int _toolbarCurrentlySelected = 0;

    // culling implementations
    private enum CullingImplementation
    {
        CPU,
        ComputeShader,
        JobSystem,
        JobSystemWithBurst
    }

    // -----------------------------------------------------------------------------------------------------------------
    // STATIC BATCHING VARIABLES
    // -----------------------------------------------------------------------------------------------------------------
    private Object _staticBatchingPrefab;
    private int _staticBatchingCount;
    private Object _staticBatchingPlane;
    private int _staticBatchingMethod;

    private readonly string[] _staticBatchingMethods =
    {
        "Mesh.CombineMeshes()",
        "StaticBatchingUtility.Combine()"
    };

    // -----------------------------------------------------------------------------------------------------------------
    // GPU INSTANCING VARIABLES
    // -----------------------------------------------------------------------------------------------------------------
    private Object _gpuInstancingMesh;
    private Object _gpuInstancingMaterial;
    private int _gpuInstancingCount;
    private Object _gpuInstancingPlane;
    private bool _gpuInstancingUseFrustumCulling;
    private bool _gpuInstancingUseOcclusionCulling;
    private CullingImplementation _gpuInstancingCullingImplementation;


    // -----------------------------------------------------------------------------------------------------------------
    // GPU INSTANCING INDIRECT VARIABLES
    // -----------------------------------------------------------------------------------------------------------------
    private Object _gpuInstancingIndirectMesh;
    private Object _gpuInstancingIndirectMaterial;
    private int _gpuInstancingIndirectCount;
    private Object _gpuInstancingIndirectPlane;
    private bool _gpuInstancingIndirectUseFrustumCulling;
    private bool _gpuInstancingIndirectUseOcclusionCulling;
    private CullingImplementation _gpuInstancingIndirectCullingImplementation;

    [MenuItem("Optimisation/Optimisation Benchmarker")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        OptimisationBenchmarker window =
            (OptimisationBenchmarker) EditorWindow.GetWindow(typeof(OptimisationBenchmarker));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(10f);
        _toolbarCurrentlySelected = GUILayout.Toolbar(_toolbarCurrentlySelected, _optimisationMethod);
        GUILayout.Space(10f);

        // change options shown based on selected tab
        switch (_toolbarCurrentlySelected)
        {
            case 0:
                StaticBatchingTab();
                break;
            case 1:
                GPUInstancingTab();
                break;
            case 2:
                GPUInstancingIndirectTab();
                break;
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run", GUILayout.Height(50f)))
        {
            Debug.Log("ran");
        }

        if (GUILayout.Button("Clear", GUILayout.Height(50f)))
        {
            Debug.Log("cleared");
        }

        GUILayout.EndHorizontal();
    }

    private void StaticBatchingTab()
    {
        GUILayout.Label("General Settings", EditorStyles.boldLabel);
        _staticBatchingPrefab = EditorGUILayout.ObjectField("Prefab", _staticBatchingPrefab, typeof(GameObject), true);
        _staticBatchingCount = EditorGUILayout.IntField("Object Count", _staticBatchingCount);
        _staticBatchingPlane =
            EditorGUILayout.ObjectField("Plane to Instantiate on", _staticBatchingPlane, typeof(GameObject), true);

        GUILayout.Space(10f);
        GUILayout.Label("Static Batching Settings", EditorStyles.boldLabel);
        _staticBatchingMethod = GUILayout.Toolbar(_staticBatchingMethod, _staticBatchingMethods);
    }

    private void GPUInstancingTab()
    {
        GUILayout.Label("General Settings", EditorStyles.boldLabel);
        _gpuInstancingMesh = EditorGUILayout.ObjectField("Mesh", _gpuInstancingMesh, typeof(Mesh), true);
        _gpuInstancingMaterial =
            EditorGUILayout.ObjectField("Material", _gpuInstancingMaterial, typeof(Material), true);
        _gpuInstancingCount = EditorGUILayout.IntField("Object Count", _gpuInstancingCount);
        _gpuInstancingPlane =
            EditorGUILayout.ObjectField("Plane to Instantiate on", _gpuInstancingPlane, typeof(GameObject), true);

        GUILayout.Space(10f);
        GUILayout.Label("GPU Instancing Settings", EditorStyles.boldLabel);
        _gpuInstancingUseFrustumCulling = EditorGUILayout.Toggle("Frustum Culling", _gpuInstancingUseFrustumCulling);
        _gpuInstancingUseOcclusionCulling = EditorGUILayout.Toggle("Occlusion Culling", _gpuInstancingUseOcclusionCulling);
        _gpuInstancingCullingImplementation =
            (CullingImplementation) EditorGUILayout.EnumPopup("Culling Method", _gpuInstancingCullingImplementation);
    }

    private void GPUInstancingIndirectTab()
    {
        GUILayout.Label("General Settings", EditorStyles.boldLabel);
        _gpuInstancingIndirectMesh = EditorGUILayout.ObjectField("Mesh", _gpuInstancingIndirectMesh, typeof(Mesh), true);
        _gpuInstancingIndirectMaterial =
            EditorGUILayout.ObjectField("Material", _gpuInstancingIndirectMaterial, typeof(Material), true);
        _gpuInstancingIndirectCount = EditorGUILayout.IntField("Object Count", _gpuInstancingIndirectCount);
        _gpuInstancingIndirectPlane =
            EditorGUILayout.ObjectField("Plane to Instantiate on", _gpuInstancingIndirectPlane, typeof(GameObject), true);

        GUILayout.Space(10f);
        GUILayout.Label("GPU Instancing Indirect Settings", EditorStyles.boldLabel);
        _gpuInstancingIndirectUseFrustumCulling = EditorGUILayout.Toggle("Frustum Culling", _gpuInstancingIndirectUseFrustumCulling);
        _gpuInstancingIndirectUseOcclusionCulling = EditorGUILayout.Toggle("Occlusion Culling", _gpuInstancingIndirectUseOcclusionCulling);
        _gpuInstancingIndirectCullingImplementation =
            (CullingImplementation) EditorGUILayout.EnumPopup("Culling Method", _gpuInstancingIndirectCullingImplementation);
    }
}