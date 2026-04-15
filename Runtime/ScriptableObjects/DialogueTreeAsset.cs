using System.Collections.Generic;
using System.Linq;
using DTNE.DialogueTreeNodeEditor.Runtime.Types;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DialogueTreeAsset", menuName = "Dialogue/Dialogue Tree", order = 0)]
    public class DialogueTreeAsset : ScriptableObject
    {
        [SerializeReference] private List<DialogueGraphNode> _nodes;
        [SerializeField] private List<DialogueTreeGraphConnection> _connections; //TODO: Apparently can't reference types.
        
        public GameObject gameObject;
        public List<DialogueGraphNode> Nodes => _nodes;
        public List<DialogueTreeGraphConnection> Connections => _connections;
        private Dictionary<string, DialogueGraphNode> _nodeDictionary;

        public DialogueTreeAsset()
        {
            _nodes = new List<DialogueGraphNode>();
            _connections = new List<DialogueTreeGraphConnection>();
            _nodeDictionary = new Dictionary<string, DialogueGraphNode>();
            
            if (!Nodes.OfType<StartNode>().Any())
            {
                StartNode startNode = new StartNode();
                _nodes.Add(startNode);
                _nodeDictionary.Add(startNode.id, startNode);
            }
        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            // Constructor seeded the dict before Unity deserialized _nodes — rebuild from the real list.
            _nodeDictionary = new Dictionary<string, DialogueGraphNode>();
            foreach (DialogueGraphNode node in Nodes)
            {
                _nodeDictionary[node.id] = node;
            }
        }

        public DialogueGraphNode GetStartNode()
        {
             StartNode[] startNodes = _nodes.OfType<StartNode>().ToArray();
            if (startNodes.Length == 0)
            {
                Debug.LogError("There are no start node in this graph.");
                return null;
            }
            return startNodes[0];
            /*
            var startNode = _nodes.FirstOrDefault(node => node is StartNode);
            if (startNode == null)
            {
                Debug.LogError("There are no start nodes in this graph.");
                return null;
            }
            return (DialogueGraphNode)startNode;
            */
        }

        public DialogueGraphNode GetNode(string nextNodeId)
        {
            return _nodeDictionary.GetValueOrDefault(nextNodeId);
        }

        public DialogueGraphNode GetNodeFromOutput(string outputNodeId, int index) //TODO: Add functionality for multi-connections.
        {
            foreach (DialogueTreeGraphConnection connection in _connections)
            {
                if (connection.outputPort.nodeId == outputNodeId && connection.outputPort.portIndex == index) //BUG: This is what is causing it. Index is wrong.
                {
                    string nodeId = connection.inputPort.nodeId;
                    DialogueGraphNode inputNode = _nodeDictionary[nodeId];
                    return inputNode;
                }
            }
            return null;
        }
    }
}