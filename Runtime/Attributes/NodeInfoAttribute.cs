using System;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        private string _nodeTitle;
        private string _menuItem;
        private bool _deletable;
        
        public string Title => _nodeTitle;
        public string MenuItem => _menuItem;
        public bool Deletable => _deletable;

        public NodeInfoAttribute(string nodeTitle, string menuItem = "", bool deletable = true)
        {
            _nodeTitle = nodeTitle;
            _menuItem = menuItem;
            _deletable = deletable; 
        }
    }
}