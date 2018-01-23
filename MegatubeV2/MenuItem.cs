using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MegatubeV2
{
    public class MenuItem
    {
        public string Name { get; protected set; }
        public string Icon { get; protected set; }
        public string ActionName { get; protected set; }
        public string ControllerName { get; protected set; }
        public bool HasChildren { get { return Children.Count > 0; } }
        public List<MenuItem> Children;

        public MenuItem(string name, string action, string controller, string icon="")
        {
            Name = name;
            Icon = icon;
            ActionName = action;
            ControllerName = controller;

            Children = new List<MenuItem>();
        }

        public void SetChildred(List<MenuItem> childrenList)
        {
            Children = new List<MenuItem>(childrenList);
        }

        public void AddChild(MenuItem child)
        {
            Children.Add(child);
        }

        public bool ContainsChild(MenuItem child)
        {
            return Children.Contains(child);
        }

        public bool ChildMatches(string controller, string action, ref string pageName)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].ControllerName == controller)
                {
                    pageName = controller;
                    if (Children[i].ActionName == action)
                        return true;
                }
            }
            return false;
        }

    }
}