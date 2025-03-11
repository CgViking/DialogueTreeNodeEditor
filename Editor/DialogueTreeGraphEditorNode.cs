using System;
using System.Reflection;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public class DialogueTreeGraphEditorNode : Node
    {
        private DialogueGraphNode _node;
        public DialogueGraphNode   Node => _node;
        public DialogueTreeGraphEditorNode(DialogueGraphNode node)
        {
            this.AddToClassList("dialogue-node");

            _node = node;
            
            Type typeInfo = node.GetType();
            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

            title = info.Title;
            
            this.name = typeInfo.Name;

            string[] depths = info.MenuItem.Split("/");
            foreach (var depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(' ', '-'));
            }

        }
    }
}