﻿using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using ZeroProgress.Common;
using ZeroProgress.Common.Collections;

public class RefRect2DMouseEvents : RefRect2D, IMouseTrackerCBs {

    //[SerializeField]
    //private Vector2ParamEvent mouseMoveEventIn;
    //[SerializeField]
    //private Vector2ParamEvent mouseDownEventIn;
    //[SerializeField]
    //private Vector2ParamEvent mouseUpEventIn;    

    [SerializeField]
    private ComponentSet mouseTrackers;

    private bool containsPointCurr;

    [SerializeField]
    private bool linkMouseUpToDown;
    private bool mouseDownWithinBounds;

    [SerializeField]
    private UnityEvent mouseDownEvent;
    [SerializeField]
    private UnityEvent mouseUpEvent;
    [SerializeField]
    private UnityEvent mouseEnterEvent;
    [SerializeField]
    private UnityEvent mouseExitEvent;

    private IMouseDownDetector[] mouseDownScripts;
    private IMouseUpDetector[] mouseUpScripts;
    private IMouseEnterDetector[] mouseEnterScripts;
    private IMouseExitDetector[] mouseExitScripts;

    private void Awake() {
        mouseDownWithinBounds = false;

        mouseDownScripts = GetComponents<IMouseDownDetector>();
        mouseUpScripts = GetComponents<IMouseUpDetector>();
        mouseEnterScripts = GetComponents<IMouseEnterDetector>();
        mouseExitScripts = GetComponents<IMouseExitDetector>();
    }

    private void OnEnable() {
        mouseTrackers.Add(this);
        //mouseMoveEventIn.RegisterListener(this);
        //mouseDownEventIn.RegisterListener(this);
        //mouseUpEventIn.RegisterListener(this);
    }

    private void OnDisable() {
        mouseTrackers.Remove(this);
        //mouseMoveEventIn.UnregisterListener(this);
        //mouseDownEventIn.UnregisterListener(this);
        //mouseUpEventIn.UnregisterListener(this);
    }

    public void OnEventRaised(Vector2 Param) {
        // Same callback cannot work for all three event types
        Debug.Log("Mouse down event raised on object: " + gameObject.name);
    }

    public void MouseDownCB(Vector2 mousePos) {
        if (ContainsPoint(mousePos)) {
            if (linkMouseUpToDown)
                mouseDownWithinBounds = true;

            foreach (IMouseDownDetector script in mouseDownScripts) {
                script.MouseDownCB();
            }
            mouseDownEvent.Invoke();
        }        
    }

    public void MouseUpCB(Vector2 mousePos) {
        if (linkMouseUpToDown) {
            // even if option is checked, the mouse being clicked and dragged into these bounds should still do mouse up,
            // even without the down connection
            if (mouseDownWithinBounds) {
                mouseDownWithinBounds = false;
                foreach (IMouseUpDetector script in mouseUpScripts) {
                    script.MouseUpCB();
                }
                mouseUpEvent.Invoke();
                return;
            }
        }

        if (ContainsPoint(mousePos)) {
            foreach (IMouseUpDetector script in mouseUpScripts) {
                script.MouseUpCB();
            }
            mouseUpEvent.Invoke();
        }
    }

    public void MouseMoveCB(Vector2 mousePos) {
        if (containsPointCurr) {
            if(!ContainsPoint(mousePos)) {
                foreach (IMouseExitDetector script in mouseExitScripts) {
                    script.MouseExitCB();
                }
                mouseExitEvent.Invoke();
                containsPointCurr = false;
            }
        }
        else {
            if (ContainsPoint(mousePos)) {
                foreach (IMouseEnterDetector script in mouseEnterScripts) {
                    script.MouseEnterCB();
                }
                mouseEnterEvent.Invoke();
                containsPointCurr = true;
            }
        }
    }
}

[CustomEditor(typeof(RefRect2DMouseEvents))]
[CanEditMultipleObjects]
public class RefRect2DMouseEventsEditor : Editor {

    SerializedProperty offsetX;
    SerializedProperty offsetY;
    SerializedProperty width;
    SerializedProperty height;

    SerializedProperty linkMouseUpToDown;

    SerializedProperty mouseTrackers;
    SerializedProperty mouseDownEvent;
    SerializedProperty mouseUpEvent;
    SerializedProperty mouseEnterEvent;
    SerializedProperty mouseExitEvent;

    bool useEvents;

    private void OnEnable() {
        offsetX = serializedObject.FindProperty("offsetX");
        offsetY = serializedObject.FindProperty("offsetY");
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");

        linkMouseUpToDown = serializedObject.FindProperty("linkMouseUpToDown");

        mouseTrackers = serializedObject.FindProperty("mouseTrackers");
        mouseDownEvent = serializedObject.FindProperty("mouseDownEvent");
        mouseUpEvent = serializedObject.FindProperty("mouseUpEvent");
        mouseEnterEvent = serializedObject.FindProperty("mouseEnterEvent");
        mouseExitEvent = serializedObject.FindProperty("mouseExitEvent");

        useEvents = false;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(offsetX);
        EditorGUILayout.PropertyField(offsetY);
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(mouseTrackers);

        EditorGUILayout.PropertyField(linkMouseUpToDown);

        useEvents = EditorGUILayout.Foldout(useEvents, "Public events", true);
        if (useEvents) {
            EditorGUILayout.PropertyField(mouseDownEvent);
            EditorGUILayout.PropertyField(mouseUpEvent);
            EditorGUILayout.PropertyField(mouseEnterEvent);
            EditorGUILayout.PropertyField(mouseExitEvent);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
