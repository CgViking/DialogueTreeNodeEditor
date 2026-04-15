using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.Types;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public struct SearchContextElement
    {
        public Type NodeType { get; private set; }
        public string Title { get; private set; }

        public SearchContextElement(Type nodeType, string title)
        {
            this.NodeType = nodeType;
            this.Title = title;
        }
    }
    public class DialogueTreeGraphWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public DialogueTreeGraphView Graph;
        public VisualElement Target;

        private static List<SearchContextElement> _cachedElements;

        [InitializeOnLoadMethod]
        private static void RegisterReloadHook()
        {
            AssemblyReloadEvents.afterAssemblyReload += () => _cachedElements = null;
        }

        private static List<SearchContextElement> GetElements()
        {
            if (_cachedElements != null) return _cachedElements;

            var elements = new List<SearchContextElement>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }

                foreach (Type type in types)
                {
                    var att = type.GetCustomAttribute<NodeInfoAttribute>();
                    if (att == null) continue;
                    if (string.IsNullOrEmpty(att.MenuItem)) continue;
                    if (type == typeof(StartNode)) continue; // Start node is not user-creatable.

                    elements.Add(new SearchContextElement(type, att.MenuItem));
                }
            }

            elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.Title.Split('/');
                string[] splits2 = entry2.Title.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }

                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;
                        return value;
                    }
                }

                return 0;
            });

            _cachedElements = elements;
            return _cachedElements;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 0));

            List<SearchContextElement> elements = GetElements();
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in elements)
            {
                string[] enteryTitle = element.Title.Split('/');
                string groupName = "";

                for (int i = 0; i < enteryTitle.Length - 1; i++)
                {
                    groupName += enteryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(enteryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(enteryTitle.Last()));
                entry.level = enteryTitle.Length;
                entry.userData = element;
                tree.Add(entry);
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = Graph.ChangeCoordinatesTo(Graph,  context.screenMousePosition - Graph.Window.position.position);
            var graphMousePosition = Graph.contentViewContainer.WorldToLocal(windowMousePosition);

            SearchContextElement element = (SearchContextElement)searchTreeEntry.userData;
            DialogueGraphNode node = (DialogueGraphNode)Activator.CreateInstance(element.NodeType);
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            Graph.Add(node);
            return true;
        }
    }
}