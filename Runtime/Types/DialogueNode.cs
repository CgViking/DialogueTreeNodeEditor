using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [System.Serializable]
    [NodeInfo("Dialogue", "Dialogue/Dialogue")]
    public class DialogueNode : DialogueGraphNode
    {
        [ExposedProperty()]
        public string Dialogue;

        public override string OnProcess(DialogueTreeAsset currentGraph, int choice)
        {
            RaiseDialogueUpdated(new DialogueEventArgs(this.Dialogue, this.Actor));
            return base.OnProcess(currentGraph);
        }
        
    }
}