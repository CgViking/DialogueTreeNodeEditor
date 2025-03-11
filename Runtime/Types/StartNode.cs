using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Start", "Process/Start")]
    public class StartNode : DialogueGraphNode
    {
    }

    [NodeInfo("Dialogue", "Dialogue/Dialogue")]
    public class DialogueNode : DialogueGraphNode
    {
        [SerializeField]
        private string _dialogue;
    }
    
    [NodeInfo("Actor", "Items/Actor")]
    public class ActorNode : DialogueGraphNode
    {
        
    }
}