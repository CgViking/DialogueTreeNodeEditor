using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Dialogue", "Dialogue/Dialogue")]
    public class DialogueNode : DialogueGraphNode
    {
        [ExposedProperty()]
        public string Dialogue;

        public override string OnProcess(DialogueTreeAsset currentGraph, int choice)
        {
            DisplayDialogue?.Invoke(Dialogue);
            return base.OnProcess(currentGraph);
        }
    }
}