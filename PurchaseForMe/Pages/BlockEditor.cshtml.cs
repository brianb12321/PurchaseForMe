using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchaseForMe.Actors.Project;
using PurchaseForMe.Core.Project;
using PurchaseForMe.Core.Project.Nodes;

namespace PurchaseForMe.Pages
{
    public class BlockEditorModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ProjectGuid { get; set; }
        [BindProperty(SupportsGet = true)]
        public string NodeGuid { get; set; }
        [BindProperty(SupportsGet = true)]
        public NodeType NodeType { get; set; }
        public string NodeName { get; set; }
        public string ToolkitXml { get; set; }
        public bool CreateNew { get; set; }
        public string WorkspaceXml { get; set; }

        private readonly IActorRef _projectManager;

        public BlockEditorModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectGuid = Guid.Parse(ProjectGuid);
            ProjectInstance project = (await _projectManager.Ask<GetProjectResponseMessage>(new GetProjectMessage(projectGuid, User.FindFirstValue(ClaimTypes.NameIdentifier)))).Project;
            if (!string.IsNullOrEmpty(NodeGuid))
            {
                ProjectNode node = project[Guid.Parse(NodeGuid)];
                if (node.NodeType == NodeType)
                {
                    var blocklyNode = node as BlocklyPipelineNode;
                    WorkspaceXml = blocklyNode.BlocklyWorkspace.InnerXml;
                    NodeName = blocklyNode.NodeName;
                    ToolkitXml = await System.IO.File.ReadAllTextAsync("blocklyToolkits/pipelineToolkit.xml");
                    return Page();
                }
                else return NotFound();
            }
            ToolkitXml = await System.IO.File.ReadAllTextAsync("blocklyToolkits/pipelineToolkit.xml");
            NodeName = "Test Pipeline";
            CreateNew = true;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            dynamic messageObj = JsonConvert.DeserializeObject<dynamic>(await reader.ReadToEndAsync());
            string projectGuidString = ((JValue) messageObj.ProjectGuid).Value<string>();
            Guid projectGuid = Guid.Parse(projectGuidString);
            string newNodeName = ((JValue) messageObj.NodeName).Value<string>();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ProjectInstance project =
                (await _projectManager.Ask<GetProjectResponseMessage>(new GetProjectMessage(projectGuid, userId))).Project;
            Guid nodeGuid = Guid.Empty;
            BlocklyPipelineNode node = null;
            if (((JValue)messageObj.CreateNew).Value<bool>())
            {
                node = new BlocklyPipelineNode();
                node.NodeName = newNodeName;
                project.ProjectItems.Add(node);
                nodeGuid = node.NodeGuid;
            }
            else
            {
                string nodeGuidString = ((JValue) messageObj.NodeGuid).Value<string>();
                nodeGuid = Guid.Parse(nodeGuidString);
                if (!project.ContainsNodeGuid(nodeGuid))
                    throw new ArgumentException(
                        $"Provided Guid {nodeGuid} does not exist. Please use the CreateNew flag.");

                node = (BlocklyPipelineNode)project[nodeGuid];
            }
            
            node.BlocklyWorkspace.LoadXml(((JValue)messageObj.WorkspaceXml).Value<string>());
            node.NodeName = newNodeName;
            _projectManager.Tell(new SaveProjectMessage(projectGuid, userId));
            dynamic resultObject = new ExpandoObject();
            resultObject.NodeGuid = node.NodeGuid;
            resultObject.Message = $"Node {node.NodeName} successfully saved.";
            return StatusCode(200, JObject.FromObject(resultObject).ToString());
        }
    }
}