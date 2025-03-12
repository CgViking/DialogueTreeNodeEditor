using System.Collections.Generic;
using System.Linq;
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
            
            string path = "Packages/com.bearlygames.dialogue-tree-editor/Editor/USS/DialogueTreeEditor.uss";
            if (!System.IO.File.Exists(path))
            {
                path = "Assets/Plugins/DialogueTreeNodeEditor/Editor/USS/DialogueTreeEditor.uss"; //Failsafe for the Developer.
            }
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

            styleSheets.Add(styleSheet);
            
            GridBackground background = new GridBackground();
            background.name = "Grid";
            Add(background);
            background.SendToBack();
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
            this.AddManipulator(new ContentZoomer());

            DrawNodes();

            graphViewChanged += OnGraphViewChangedEvent;
        }

        //This chooses what can be plugged into what.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> ports = new List<Port>();

            foreach (var node in GraphNodes)
            {
                allPorts.AddRange(node.Ports);
            }

            foreach (Port p in allPorts)
            {
                if (p == startPort) { continue; }
                if (p.node == startPort.node) { continue; }
                if (p.direction == startPort.direction) { continue; }
                if (p.portType == startPort.portType)
                {
                    ports.Add(p);
                }
            }
            
            return ports;
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Moved Elements");
                foreach (DialogueTreeGraphEditorNode editorNode in graphViewChange.movedElements.OfType<DialogueTreeGraphEditorNode>())
                {
                    editorNode.SavePosition();
                }
            }
            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Remove Stuff from Graph");
                
                List<DialogueTreeGraphEditorNode> nodes = graphViewChange.elementsToRemove.OfType<DialogueTreeGraphEditorNode>().ToList();
                if (nodes.Count > 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodes[i]);
                    }
                }
            }
            
            return graphViewChange;
        }

        private void RemoveNode(DialogueTreeGraphEditorNode editorNode)
        {
            _dialogueTreeAsset.Nodes.Remove(editorNode.Node);
            GraphNodesDictionary.Remove(editorNode.Node.id);
            GraphNodes.Remove(editorNode);
            _serializedObject.Update();
        }

        private void DrawNodes()
        {
            foreach (DialogueGraphNode node in _dialogueTreeAsset.Nodes)
            {
                AddNodeToGraph(node);
            }
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

            DialogueTreeGraphEditorNode editorNode = new DialogueTreeGraphEditorNode(node);
            editorNode.SetPosition(node.Position);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.id, editorNode);
            
            AddElement(editorNode);
        }
    }
}