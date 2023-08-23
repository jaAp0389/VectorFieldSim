using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Options))]
class DecalMeshHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("FlowField: Expand"))
        {
            Lib.sFlowField.ExpandSegments();
            Lib.sFlowField.UpdateSegmentList();
        }
        if (GUILayout.Button("FlowField: Smooth"))
        {
            Lib.sFlowField.SmoothFlowField();
            Lib.sFlowField.UpdateSegmentList();
        }
        if (GUILayout.Button("VectorField : Redraw"))
        {
            Lib.sVectorField.DrawField();
        }
    }
}
public class Options : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] bool IsFlowfield = true,
                          IsSlowDirChange = true;
    [SerializeField][Range(0f, 1f)] float mDirChangeRate = 1f;
    [SerializeField][Range(-10f, 10f)] float mSpeed = 5;

    [Header("FlowField")]
    [SerializeField] bool mDrawFField = true;
    [SerializeField] int mFlowFieldDim = 50;

    [Header("VectorField")]
    [SerializeField] bool mDrawVField = true;
    [SerializeField] int mVectorFieldDim = 50;
    [SerializeField][Range(0, 3)] int mVFieldType = 0;

    void Awake()
    {
        Lib.sFlowField = FindObjectOfType<FlowField>();
        Lib.sVectorField = FindObjectOfType<VectorField>();
    }
    void OnValidate()
    {
        Lib.sFlowFieldDim = mFlowFieldDim;
        Lib.sDirChangeRate = mDirChangeRate;
        Lib.sSpeed = mSpeed;
        Lib.sIsFlowField = IsFlowfield;
        Lib.sVDim = mVectorFieldDim;
        Lib.sVFieldType = mVFieldType;
        Lib.sDrawFField = mDrawFField;
        Lib.sDrawVField = mDrawVField;
        Lib.sIsSlowDirChange = IsSlowDirChange;
    }
}
public static class Lib
{
    public static bool sIsFlowField, 
                       sDrawFField, 
                       sDrawVField,
                       sIsSlowDirChange;
    public static int sFlowFieldDim,
                      sVDim,
                      sVFieldType;
    public static float sDirChangeRate, 
                        sSpeed;

    public static FlowField sFlowField;
    public static VectorField sVectorField;
}
