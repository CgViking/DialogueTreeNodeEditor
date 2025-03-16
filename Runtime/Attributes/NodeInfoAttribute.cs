using System;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        private string _nodeTitle;
        private string _menuItem;
        private bool _hasFlowInput;
        private bool _hasFlowOutput;
        private int _outputs;
        
        public string Title => _nodeTitle;
        public string MenuItem => _menuItem;
        public bool HasFlowInput => _hasFlowInput;
        public bool HasFlowOutput => _hasFlowOutput;
        public int Outputs => _outputs;

        public NodeInfoAttribute(string nodeTitle, string menuItem = "", bool hasFlowInput = true,  bool hasFlowOutput = true, int outputs = 1)
        {
            _nodeTitle = nodeTitle;
            _menuItem = menuItem;
            _hasFlowInput = hasFlowInput;
            _hasFlowOutput = hasFlowOutput;
            _outputs = outputs;
        }
    }
}