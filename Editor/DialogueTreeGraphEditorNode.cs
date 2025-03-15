using System;
using System.Collections.Generic;
using System.Reflection;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeGraphEditorNode : Node
    {
        private DialogueGraphNode _node;
        private Port _outputPort;
        private List<Port> _ports;
        private SerializedProperty _serializedProperty;
        public DialogueGraphNode   Node => _node;
        public List<Port> Ports => _ports;
        
        private SerializedObject _serializedObject;
        
        public DialogueTreeGraphEditorNode(DialogueGraphNode node, SerializedObject dialogueGraphObject)
        {
            this.AddToClassList("dialogue-node");

            _serializedObject = dialogueGraphObject;
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

            // We do this so that output is always index 0;
            if (info.HasFlowOutput)
            {
                for (int i = 0; i < info.Outputs; i++)
                {
                    CreateFlowOutputPort(); //TODO: This actually works, but they have the same index, so they need to iterate the index.
                }
            }
            if (info.HasFlowInput)
            {
                CreateFlowInputPort();
            }

            foreach (FieldInfo property in typeInfo.GetFields() )
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty)
                {
                    PropertyField field = DrawProperty(property.Name);
                    //field.RegisterValueChangeCallback(OnFieldChangedCallback);
                }
            }
            
            RefreshExpandedState();
        }

        private PropertyField DrawProperty(string propertyName)
        {
            if (_serializedProperty == null)
            {
                FetchSerializedProperty();
            }
            
            SerializedProperty prop = _serializedProperty.FindPropertyRelative(propertyName);
            
            PropertyField field = new PropertyField(prop);
            field.bindingPath = prop.propertyPath;
            extensionContainer.Add(field);
            return field;
        }

        private void FetchSerializedProperty()
        {
            SerializedProperty nodes = _serializedObject.FindProperty("_nodes");
            if (nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    var element = nodes.GetArrayElementAtIndex(i);
                    var elementId = element.FindPropertyRelative("guid"); // The same as in DialogueGraphNode.cs
                    if (elementId.stringValue == _node.id)
                    {
                        _serializedProperty = element;
                    }
                }
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