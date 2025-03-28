using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Start", "Process/Start", false, true, 1, false)]
    public class StartNode : DialogueGraphNode
    {
        public override string OnProcess(DialogueTreeAsset currentGraph)
        {
            Debug.Log("StartNode");
            return base.OnProcess(currentGraph);
        }
    }
}