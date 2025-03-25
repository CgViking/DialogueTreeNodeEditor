using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Event Trigger", "Actions/Event Trigger", true, false)]
    public class EventTriggerNode : DialogueGraphNode
    {
        [ExposedProperty()]
        public UnityEvent EventTrigger;

        public override string OnProcess(DialogueTreeAsset currentGraph)
        {
            Debug.Log("EventTrigger");
            EventTrigger!.Invoke();
            return base.OnProcess(currentGraph);
        }
    }
}