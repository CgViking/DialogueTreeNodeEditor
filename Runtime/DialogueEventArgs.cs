using System;
using System.Collections.Generic;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    /// <inheritdoc />
    public class DialogueEventArgs : EventArgs
    {
        public string Dialogue { get; }
        public Actor Actor { get; }
        public List<string> Choices { get; }

        public DialogueEventArgs(string dialogue, Actor actor, List<string> choices = null)
        {
            Dialogue = dialogue;
            Actor = actor;
            if (choices != null)
                Choices = choices;
        }
        
    }
}