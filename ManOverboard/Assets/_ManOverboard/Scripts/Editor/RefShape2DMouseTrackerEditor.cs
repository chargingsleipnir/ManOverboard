using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RefShape2DMouseTracker))]
[CanEditMultipleObjects]
public class RefShape2DMouseTrackerEditor : Editor {

    SerializedProperty refShape;
    SerializedProperty clickThrough;
    SerializedProperty linkMouseUpToDown;

    SerializedProperty mouseDownEvent;
    SerializedProperty mouseUpEvent;
    SerializedProperty mouseEnterEvent;
    SerializedProperty mouseExitEvent;

    bool useEvents;

    private void OnEnable() {

        refShape = serializedObject.FindProperty("refShape");
        clickThrough = serializedObject.FindProperty("clickThrough");
        linkMouseUpToDown = serializedObject.FindProperty("linkMouseUpToDown");

        mouseDownEvent = serializedObject.FindProperty("mouseDownEvent");
        mouseUpEvent = serializedObject.FindProperty("mouseUpEvent");
        mouseEnterEvent = serializedObject.FindProperty("mouseEnterEvent");
        mouseExitEvent = serializedObject.FindProperty("mouseExitEvent");

        useEvents = false;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RefShape2DMouseTracker)target), typeof(RefShape2DMouseTracker), false);
        GUI.enabled = true;

        EditorGUILayout.LabelField("Only 1 instance of this component is req'd to activate all IMouseDetector callbacks on this GameObject");

        EditorGUILayout.PropertyField(refShape);
        EditorGUILayout.PropertyField(clickThrough);
        EditorGUILayout.PropertyField(linkMouseUpToDown);

        useEvents = EditorGUILayout.Foldout(useEvents, "Public event responses", true);
        if (useEvents) {
            EditorGUILayout.PropertyField(mouseDownEvent);
            EditorGUILayout.PropertyField(mouseUpEvent);
            EditorGUILayout.PropertyField(mouseEnterEvent);
            EditorGUILayout.PropertyField(mouseExitEvent);
        }

        serializedObject.ApplyModifiedProperties();
    }
}