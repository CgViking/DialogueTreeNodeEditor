using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Start", "Process/Start", false, true)]
    public class StartNode : DialogueGraphNode
    {
        public override string OnProcess(DialogueTreeAsset currentGraph)
        {
            Debug.Log("StartNode");
            return base.OnProcess(currentGraph);
        }
    }

    [NodeInfo("Dialogue", "Dialogue/Dialogue")]
    public class DialogueNode : DialogueGraphNode
    {
        [ExposedProperty()]
        public string Dialogue;
    }
    
    [NodeInfo("Dialogue Branch", "Dialogue/Branch", true, true)]
    public class DialogueBranch : DialogueGraphNode
    {
        public string Dialogue;
        public string[] Branches;
    }
}