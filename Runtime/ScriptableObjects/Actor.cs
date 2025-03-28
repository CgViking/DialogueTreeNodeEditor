using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Actor", menuName = "Dialogue/Actor", order = 1)]
    public class Actor : ScriptableObject
    {
        public string actorName;
        public Color actorColor;
        public Sprite actorIcon;
    }
}