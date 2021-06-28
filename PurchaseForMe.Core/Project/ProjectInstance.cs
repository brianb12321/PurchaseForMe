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
    }
}