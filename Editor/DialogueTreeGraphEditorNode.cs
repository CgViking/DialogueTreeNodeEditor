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
        
        private List<Port> _outputPorts = new List<Port>();
        private List<Port> _inputPorts = new List<Port>();
        public List<Port> Ports => _inputPorts.Concat(_outputPorts).ToList();
        
        private SerializedProperty _serializedProperty;
        public DialogueGraphNode Node => _node;
        
        private SerializedObject _serializedObject;
        private SerializedProperty _choicesProp;
        private int _lastChoiceCount;


        public DialogueTreeGraphEditorNode(DialogueGraphNode node, SerializedObject dialogueGraphObject)
        {
            
            this.AddToClassList("dialogue-node");

            _serializedObject = dialogueGraphObject;
            _node = node;
            
            Type typeInfo = node.GetType();
            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

            title = info.Title;
            this.name = typeInfo.Name;

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
            
            SetupBranches();
            GetActor();
            RefreshExpandedState();
        }

        private void SetupBranches()
        {
            if (!(_node is DialogueBranch db)) return;
            
            // Check if serialized properties are valid
            if (_serializedProperty == null)
            {
                Debug.LogError("SerializedProperty is null!");
                return;
            }

            _choicesProp = _serializedProperty.FindPropertyRelative("_choices");
            if (_choicesProp == null)
            {
                Debug.LogError("Could not find _choices property!");
                return;
            }

            VisualElement branchView = new VisualElement();
            branchView.AddToClassList("branchView");
            
            ListView choicesList = new ListView
            {
                bindingPath = "_choices",
                showFoldoutHeader = false,
                showBoundCollectionSize = false,
                showBorder = true,
                showAddRemoveFooter = true,
                reorderable = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                fixedItemHeight = 32,
                style = { flexGrow = 1 }
            };
            
            //Remove ability to delete.
            choicesList.RegisterCallback<ContextualMenuPopulateEvent>(evt => 
            {
                evt.menu.ClearItems();
            });

            choicesList.BindProperty(_choicesProp);
      
            choicesList.makeItem = () => 
            {
                var textField = new TextField
                {
                    label = "Choice ",
                    maxLength = 64
                };
                textField.AddToClassList("choice-text-field");
                return textField;
            };

            choicesList.bindItem = (element, index) =>
            {
                var textField = (TextField)element;
                textField.label = "Choice " + index.ToString();
                var itemProp = _choicesProp.GetArrayElementAtIndex(index);
                textField.BindProperty(itemProp); // Direct property binding
            };
            
            choicesList.itemsAdded += indices =>
            {
                _serializedObject.Update();
                _serializedObject.ApplyModifiedProperties();
                foreach (int index in indices)
                {
                    CreateFlowOutputPort(index);
                }
                MarkDirtyRepaint();
            };

            choicesList.itemsRemoved += indices =>
            {
                _serializedObject.Update();
                _serializedObject.ApplyModifiedProperties();
                foreach (int index in indices.OrderByDescending(i => i))
                {
                    RemoveFlowOutput(index);
                }
                MarkDirtyRepaint();
            };
            
            branchView.Add(choicesList);
            extensionContainer.Add(branchView);
        }

        private void RemoveFlowOutput(int index) {
            if (index < 0 || index >= _outputPorts.Count) return;

            _outputPorts.RemoveAt(index);
            outputContainer.RemoveAt(index);
        }

        private void GetActor()
        {
            var actorField = new ObjectField("Actor");
            actorField.objectType = typeof(Actor);
            actorField.tooltip = "The associated Actor\nIf this is the Start node, then it is whom initiated the conversation.";
            actorField.AddToClassList("actorField");
            
            SerializedProperty actorProp = _serializedProperty?.FindPropertyRelative("_actor");

            // Check if the property exists
            if (actorProp != null)
            {
                actorField.BindProperty(actorProp);
                
                actorProp.serializedObject.Update();
                actorField.RegisterValueChangedCallback(evt =>
                {
                    _serializedObject.ApplyModifiedProperties();
                    UpdateActorDisplay(actorField, evt.newValue as  Actor);
                    RefreshExpandedState();
                });

                UpdateActorDisplay(actorField, _node.Actor);
            }
            else
            {
                Debug.LogWarning("Actor property not found!");
            }
            
            extensionContainer.Insert(0 ,actorField); //Insert to make sure it's first in the list.
            RefreshExpandedState();
        }

        private void UpdateActorDisplay(ObjectField actorField,  Actor actor)
        {
            var icon = actorField.Q<VisualElement>("actorIcon");
            
            if (icon != null) actorField.Remove(icon);
            
            if (actor == null) return;
            
            if (actor.name != string.Empty) {
                actorField.name = actor.name;
            }
            actorField.style.backgroundColor = new StyleColor(actor.actorColor);

            if (icon == null) // Make new icon if null.
            {
                icon = new VisualElement();
                icon.name = "actorIcon";
                icon.AddToClassList("actorIcon");
                actorField.Add(icon);
            }
            
            icon.style.backgroundImage = actor.actorIcon != null 
                ? new StyleBackground(actor.actorIcon) 
                : StyleKeyword.Null;
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
            _inputPorts.Add(inputPort);
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

            _outputPorts.Add(outputPort);
            outputContainer.Add(outputPort);
        }
        
        public void SavePosition()
        {
            _node.SetPosition(GetPosition());
        }
        
    }
}