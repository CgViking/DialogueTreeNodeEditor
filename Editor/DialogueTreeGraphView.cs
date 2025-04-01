using System.Collections.Generic;
using System.Linq;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeGraphView : GraphView
    {
        private DialogueTreeAsset _dialogueTreeAsset;
        private SerializedObject _serializedObject;
        private DialogueTreeEditorWindow _window;
        public DialogueTreeEditorWindow Window => _window;

        public List<DialogueTreeGraphEditorNode> GraphNodes;
        public Dictionary<string, DialogueTreeGraphEditorNode> GraphNodesDictionary;
        public Dictionary<Edge, DialogueTreeGraphConnection> ConnectionDictionary;
        
        private DialogueTreeGraphWindowSearchProvider _searchProvider;
        
        public DialogueTreeGraphView(SerializedObject serializedObject, DialogueTreeEditorWindow window)
        {
            _serializedObject = serializedObject;
            _dialogueTreeAsset = (DialogueTreeAsset)_serializedObject.targetObject;
            _window = window;
            
            GraphNodes = new List<DialogueTreeGraphEditorNode>();
            GraphNodesDictionary = new Dictionary<string, DialogueTreeGraphEditorNode>();
            ConnectionDictionary = new Dictionary<Edge, DialogueTreeGraphConnection>();

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
            DrawConnections();

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
                Undo.RecordObject(_serializedObject.targetObject, "Removed Stuff from Graph");
                
                List<DialogueTreeGraphEditorNode> nodes = graphViewChange.elementsToRemove.OfType<DialogueTreeGraphEditorNode>().ToList();
                if (nodes.Count > 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodes[i]);
                    }
                }

                foreach (Edge edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(edge);   
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, "Added connections");
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    CreateEdge(edge);
                }
            }
            
            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            DialogueTreeGraphEditorNode inputNode = (DialogueTreeGraphEditorNode) edge.input.node;
            int inputIndex = inputNode.Ports.IndexOf(edge.input);
            
            DialogueTreeGraphEditorNode outputNode = (DialogueTreeGraphEditorNode) edge.output.node;
            int outputIndex = outputNode.Ports.IndexOf(edge.output);
            
            DialogueTreeGraphConnection connection = new DialogueTreeGraphConnection(inputNode.Node.id, inputIndex, outputNode.Node.id, outputIndex);
            _dialogueTreeAsset.Connections.Add(connection);
        }

        private void RemoveConnection(Edge e)
        {
            if(ConnectionDictionary.TryGetValue(e, out DialogueTreeGraphConnection connection))
            {
                _dialogueTreeAsset.Connections.Remove(connection);
                ConnectionDictionary.Remove(e);
            }
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
            
            Bind();
        }
        private void DrawConnections()
        {
            if(_dialogueTreeAsset.Connections == null) { return; }

            foreach (DialogueTreeGraphConnection connection in _dialogueTreeAsset.Connections)
            {
                DrawConnection(connection);
            }
            
        }

        private void DrawConnection(DialogueTreeGraphConnection connection)
        {
            DialogueTreeGraphEditorNode inputNode = GetNode(connection.inputPort.nodeId);
            DialogueTreeGraphEditorNode outputNode = GetNode(connection.outputPort.nodeId);

            if (inputNode == null || outputNode == null)
            {
                _dialogueTreeAsset.Connections.Remove(connection);
                return;
            }
            
            Port inPort = inputNode.Ports[connection.inputPort.portIndex];
            Port outPort = outputNode.Ports[connection.outputPort.portIndex];

            if (inPort == null || outPort == null)
            {
                _dialogueTreeAsset.Connections.Remove(connection);
                return;
            }

            // Ensure ports have opposite directions
            if (inPort.direction == outPort.direction)
            {
                Debug.LogError($"Invalid connection: Port directions must differ (Input: {inPort.direction}, Output: {outPort.direction})");
                _dialogueTreeAsset.Connections.Remove(connection); // Remove invalid connection
                return;
            }
            
            Edge edge = inPort.ConnectTo(outPort);
            
            if (edge == null) { return; }
            
            AddElement(edge);
            
            ConnectionDictionary.Add(edge, connection);
        }

        private DialogueTreeGraphEditorNode GetNode(string nodeId)
        {
            DialogueTreeGraphEditorNode node = null;
            GraphNodesDictionary.TryGetValue(nodeId, out node);
            return node;
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
            Bind();
        }

        private void AddNodeToGraph(DialogueGraphNode node)
        {
            node.typeName = node.GetType().AssemblyQualifiedName;

            DialogueTreeGraphEditorNode editorNode = new DialogueTreeGraphEditorNode(node, _serializedObject);
            editorNode.SetPosition(node.Position);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.id, editorNode);
            
            AddElement(editorNode);
        }

        private void Bind()
        {
            _serializedObject.Update();
            this.Bind(_serializedObject);
        }
        
    }
}