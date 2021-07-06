namespace PurchaseForMe.Core.Configuration
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