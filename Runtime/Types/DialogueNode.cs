using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Dialogue", "Dialogue/Dialogue")]
    public class DialogueNode : DialogueGraphNode
    {
        [ExposedProperty()]
        public string Dialogue;
        
        public override string OnProcess(DialogueTreeAsset currentGraph)
        {
            Debug.Log(Dialogue);
            return base.OnProcess(currentGraph);
        }
    }
}