using System.Collections.Generic;
using DTNE.DialogueTreeNodeEditor.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeGraphView : GraphView
    {
        private DialogueTreeAsset _dialogueTreeAsset;
        private SerializedObject _serializedObject;
        private DialogueTreeEditorWindow _window;

        public List<DialogueTreeGraphEditorNode> GraphNodes;
        public Dictionary<string, DialogueTreeGraphEditorNode> GraphNodesDictionary;
        public DialogueTreeEditorWindow Window => _window;
        
        private DialogueTreeGraphWindowSearchProvider _searchProvider;
        
        public DialogueTreeGraphView(SerializedObject serializedObject, DialogueTreeEditorWindow window)
        {
            _serializedObject = serializedObject;
            _dialogueTreeAsset = (DialogueTreeAsset)_serializedObject.targetObject;
            _window = window;
            
            GraphNodes = new List<DialogueTreeGraphEditorNode>();
            GraphNodesDictionary = new Dictionary<string, DialogueTreeGraphEditorNode>();

            _searchProvider = ScriptableObject.CreateInstance<DialogueTreeGraphWindowSearchProvider>();
            _searchProvider.Graph = this;
            
            this.nodeCreationRequest = ShowSearchWindow;
            
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/DialogueTreeNodeEditor/Editor/USS/DialogueTreeEditor.uss");
            styleSheets.Add(styleSheet);
            
            GridBackground background = new GridBackground();
            background.name = "Grid";
            Add(background);
            background.SendToBack();
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new RectangleSelector());
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            _searchProvider.Target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), _searchProvider);
        }

        public void Add(DialogueGraphNode node)
        {
            Undo.RecordObject(_serializedObject.targetObject, "Add Dialogue Node");
            
            _dialogueTreeAsset.Nodes.Add(node);
            _serializedObject.Update();
            
            AddNodeToGraph(node);
        }

        private void AddNodeToGraph(DialogueGraphNode node)
        {
            node.typeName = node.GetType().AssemblyQualifiedName;

            DialogueTreeGraphEditorNode editorNode = new DialogueTreeGraphEditorNode();
            editorNode.SetPosition(node.Position);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.id, editorNode);
            
            AddElement(editorNode);
        }
    }
}