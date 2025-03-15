using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Dialogue Branch", "Dialogue/Branch", true, true, 2)]
    public class DialogueBranch : DialogueGraphNode
    {
        public string Dialogue;
        public string[] Branches;
    }
}