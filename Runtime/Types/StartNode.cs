using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Start", "Process/Start", false)]
    public class StartNode : DialogueGraphNode
    {
        public override bool HasFlowInput => false;

        public override string OnProcess(DialogueTreeAsset currentGraph, int choice)
        {
            RaiseDialogueUpdated(new DialogueEventArgs(string.Empty, this.Actor));
            return base.OnProcess(currentGraph);
        }
    }
}