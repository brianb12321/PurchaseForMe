using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseForMe.Core.Project
{
    [Serializable]
    public class ProjectInstance
    {
        public Guid ProjectGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ProjectNode> ProjectItems { get; set; }

        public ProjectInstance()
        {
            ProjectItems = new List<ProjectNode>();
            ProjectGuid = Guid.NewGuid();
        }

        public ProjectNode this[Guid nodeGuid]
        {
            get
            {
                return ProjectItems.First(n => n.NodeGuid == nodeGuid);
            }
        }

        public bool ContainsNodeGuid(Guid nodeGuid)
        {
            return ProjectItems.Any(n => n.NodeGuid == nodeGuid);
        }
    }
}