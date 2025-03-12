using System;
using DTNE.DialogueTreeNodeEditor.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeEditorWindow : GraphViewEditorWindow
    {
        public static void Open(DialogueTreeAsset target)
        {
            DialogueTreeEditorWindow[] windows = Resources.FindObjectsOfTypeAll<DialogueTreeEditorWindow>();
            foreach (var w in windows)
            {
                if (w.CurrentGraph == target)
                {
                    w.Focus();
                    return;
                }
            }
            DialogueTreeEditorWindow window = CreateWindow<DialogueTreeEditorWindow>(typeof(DialogueTreeEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null, typeof(DialogueTreeAsset)).image);
            window.Load(target);
        }



        [SerializeField] private DialogueTreeAsset dialogueTreeAsset;
        [SerializeField] private SerializedObject  _serializedObject;
        [SerializeField] private DialogueTreeGraphView _currentView;
        
        public DialogueTreeAsset CurrentGraph => dialogueTreeAsset;

        private void OnEnable()
        {
            if (dialogueTreeAsset != null)
            {
                DrawGraph();
            }
        }

        private void OnGUI()
        {
            if (dialogueTreeAsset != null)
            {
                if(EditorUtility.IsDirty(dialogueTreeAsset))
                {
                    this.hasUnsavedChanges = true;
                }
                else
                {
                    this.hasUnsavedChanges = false;
                }
            }
        }

        private void Load(DialogueTreeAsset target)
        {
            dialogueTreeAsset = target;
            DrawGraph();
        }

        private void DrawGraph()
        {
            _serializedObject =  new SerializedObject(dialogueTreeAsset);
            _currentView = new DialogueTreeGraphView(_serializedObject, this);
            _currentView.graphViewChanged += OnChange;
            rootVisualElement.Add(_currentView);
        }

        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            this.hasUnsavedChanges = true;
            EditorUtility.SetDirty(dialogueTreeAsset);
            return graphViewChange;
        }
    }
}