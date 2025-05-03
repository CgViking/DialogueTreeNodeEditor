using System;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Event Trigger", "Actions/Event Trigger")]
    public class EventTriggerNode : DialogueGraphNode
    {
        [ExposedProperty, SerializeField]
        public string EventTriggerName;

        public override string OnProcess(DialogueTreeAsset currentGraph, int choice)
        {
            var graphObject = currentGraph.gameObject?.GetComponent<DialogueTreeGraphObject>();
            if (graphObject != null)
            {
                graphObject.TriggerEvent(EventTriggerName);
            }
            else
            {
                Debug.LogError("Failed to find DialogueTreeGraphObject.");
            }
            
            return base.OnProcess(currentGraph);
        }
    }
}