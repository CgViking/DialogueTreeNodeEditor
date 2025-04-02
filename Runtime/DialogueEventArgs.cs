using System;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    /// <inheritdoc />
    public class DialogueEventArgs : EventArgs
    {
        public string Dialogue { get; }
        public Actor Actor { get; }

        public DialogueEventArgs(string dialogue, Actor actor)
        {
            Dialogue = dialogue;
            Actor = actor;
        }
        
    }
}