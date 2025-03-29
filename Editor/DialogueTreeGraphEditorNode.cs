using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using DTNE.DialogueTreeNodeEditor.Runtime.Types;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeGraphEditorNode : Node
    {
        private DialogueGraphNode _node;
        private Port _outputPort;
        private List<Port> _ports;
        private SerializedProperty _serializedProperty;
        public DialogueGraphNode Node => _node;
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

            if (info.Deletable == false) //Make sure nodes like start node is not deletable.
            {
                capabilities &= ~Capabilities.Deletable;
            }

            // We do this first so that output is always index 0;
            if (node.HasFlowOutput)
            {
                for (int i = 0; i < node.FlowOutputCount; i++)
                {
                    CreateFlowOutputPort(i); //TODO: This actually works, but they have the same index, so they need to iterate the index.
                }
            }
            if (node.HasFlowInput)
            {
                for (int i = 0; i < node.FlowInputCount; i++)
                {
                    CreateFlowInputPort(i);
                }
            }
            foreach (FieldInfo property in typeInfo.GetFields() )
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() is { } exposedProperty)
                {
                    PropertyField field = DrawProperty(property.Name);
                    //field.RegisterValueChangeCallback(OnFieldChangedCallback);
                }
            }
            
            FetchSerializedProperty();
            
            GetActor();
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
            if (_serializedObject == null)
            {
                Debug.LogError("SerializedObject is null!");
                return;
            }

            SerializedProperty nodes = _serializedObject.FindProperty("_nodes");
            if (nodes == null || !nodes.isArray) return;

            for (int i = 0; i < nodes.arraySize; i++)
            {
                var element = nodes.GetArrayElementAtIndex(i);
                var elementId = element.FindPropertyRelative("_guid");
                if (elementId != null && elementId.stringValue == _node.id)
                {
                    _serializedProperty = element;
                    break;
                }
            }

            if (_serializedProperty == null)
            {
                Debug.LogError($"Failed to find serialized property for node {_node.id}");
            }
        }

        private void CreateFlowInputPort(int index)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            inputPort.portName = $"In {index + 1}";
            inputPort.tooltip = $"Input port {index + 1}";
            _ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }
        private void CreateFlowOutputPort(int index)
        {
            var outputPort = InstantiatePort(
                Orientation.Horizontal,
                Direction.Output,
                Port.Capacity.Single,
                typeof(PortTypes.FlowPort)
            );
    
            outputPort.portName = _node.GetOutputPortName(index);
            outputPort.tooltip = $"Output port {index}";
    
            if (!_ports.Contains(outputPort))
            {
                _ports.Add(outputPort);
                outputContainer.Add(outputPort);
            }
        }
        
        public void SavePosition()
        {
            _node.SetPosition(GetPosition());
        }
    }
}