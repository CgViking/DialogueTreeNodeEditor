using System;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    [System.Serializable]
    public class DialogueGraphNode 
    {
        [SerializeField]
        private string guid;
        [SerializeField]
        private Rect position;

        public string typeName;
        
        public string id => guid;
        public Rect Position => position;

        public DialogueGraphNode()
        {
            NewGUID();
        }

        private void NewGUID()
        {
            guid = Guid.NewGuid().ToString();
        }

        public void SetPosition(Rect newPosition)
        {
            position = newPosition;
        }
    }
}