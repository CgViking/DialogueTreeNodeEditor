using System;
using System.Collections.Generic;
using System.Reflection;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeGraphEditorNode : Node
    {
        private DialogueGraphNode _node;
        private Port _outputPort;
        private List<Port> _ports;
        public DialogueGraphNode   Node => _node;
        public List<Port> Ports => _ports;
        public DialogueTreeGraphEditorNode(DialogueGraphNode node)
        {
            this.AddToClassList("dialogue-node");

            _node = node;
            
            Type typeInfo = node.GetType();
            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

            title = info.Title;
            this.name = typeInfo.Name;
            
            _ports = new List<Port>();

            string[] depths = info.MenuItem.Split("/");
            foreach (var depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(' ', '-'));
            }

            if (info.HasFlowInput)
            {
                CreateFlowInputPort();
            }

            if (info.HasFlowOutput)
            {
                CreateFlowOutputPort();
            }

        }

        private void CreateFlowInputPort()
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            inputPort.portName = "In";
            inputPort.tooltip = "Input port";
            _ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }
        private void CreateFlowOutputPort()
        {
            _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            _outputPort.portName = "Out";
            _outputPort.tooltip = "Output port";
            _ports.Add(_outputPort);
            outputContainer.Add(_outputPort);
        }


        public void SavePosition()
        {
            _node.SetPosition(GetPosition());
        }
    }
}