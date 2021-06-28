using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseForMe.Configuration
{
    public class ProjectSettings
    {
        public string ProjectBasePath { get; set; }

        public ProjectSettings()
        {
            ProjectBasePath = "C:/Projects";
        }
    }
}