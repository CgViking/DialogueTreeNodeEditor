using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.Types;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    public struct SearchContextElement
    {
        public object Target { get; private set; }
        public string Title { get; private set; }

        public SearchContextElement(object target, string title)
        {
            this.Target = target;
            this.Title = title;
        }
    }
    public class DialogueTreeGraphWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public DialogueTreeGraphView Graph;
        public VisualElement Target;

        public static List<SearchContextElement> Elements;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) //TODO: Hide start node.
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 0));
            
            Elements = new List<SearchContextElement>();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.CustomAttributes.ToList() != null)
                    {
                        var attribute = type.GetCustomAttribute(typeof(NodeInfoAttribute));
                        if (attribute != null)
                        {
                            NodeInfoAttribute att =(NodeInfoAttribute)attribute;
                            var node = Activator.CreateInstance(type);
                            
                            if(string.IsNullOrEmpty(att.MenuItem)) {continue;}
                            if(node is StartNode) { continue; } // Skip Start node, should not be able to add manually.
                            
                            Elements.Add(new SearchContextElement(node, att.MenuItem));
                        }
                    }
                }
            }
            //Sort by name
            Elements.Sort((entry1, entry2) =>
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
            
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in Elements)
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
                entry.userData = new  SearchContextElement(element.Target, element.Title);
                tree.Add(entry);
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = Graph.ChangeCoordinatesTo(Graph,  context.screenMousePosition - Graph.Window.position.position);
            var graphMousePosition = Graph.contentViewContainer.WorldToLocal(windowMousePosition);
             
             SearchContextElement element = (SearchContextElement)searchTreeEntry.userData; 
             DialogueGraphNode node = (DialogueGraphNode)element.Target;
             node.SetPosition(new Rect(graphMousePosition, new Vector2()));
             Graph.Add(node);
             return true;
        }
    }
}