using System.Collections.Generic;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    [CreateAssetMenu(fileName = "DialogueTreeAsset", menuName = "Dialogue Tree", order = 0)]
    public class DialogueTreeAsset : ScriptableObject
    {
        [SerializeReference] private List<DialogueGraphNode> _nodes;
        
        public List<DialogueGraphNode> Nodes => _nodes;

        public DialogueTreeAsset()
        {
            _nodes = new List<DialogueGraphNode>();
        }
    }
}