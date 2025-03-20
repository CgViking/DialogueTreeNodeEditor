using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Dialogue Branch", "Dialogue/Branch", true, true, 2)]
    public class DialogueBranch : DialogueGraphNode
    {
        [ExposedProperty]
        public string Dialogue;
        [ExposedProperty]
        public string[] Branches;
    }
}