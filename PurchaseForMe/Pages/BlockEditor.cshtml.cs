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
using PurchaseForMe.Core.Project.Nodes.Blockly;

namespace PurchaseForMe.Pages
{
    public class BlockEditorModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string ProjectGuid { get; set; }
        [BindProperty(SupportsGet = true)]
        public string NodeGuid { get; set; }
        public string NodeName { get; set; }
        public string ToolkitXml { get; set; }
        public string WorkspaceXml { get; set; }
        public string RunnerToConnect { get; set; }
        public string UserId { get; set; }

        private readonly IActorRef _projectManager;

        public BlockEditorModel(ProjectManagerFactory projectManager)
        {
            _projectManager = projectManager();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projectGuid = Guid.Parse(ProjectGuid);
            ProjectInstance project = (await _projectManager.Ask<GetProjectResponseMessage>(new GetProjectMessage(projectGuid, User.FindFirstValue(ClaimTypes.NameIdentifier)))).Project;
            ProjectNode node = project[Guid.Parse(NodeGuid)];
            var blocklyNode = node as BlocklyNode;
            if (blocklyNode.BlocklyWorkspace == null)
            {
                blocklyNode.BlocklyWorkspace = new XmlDocument();
                if (blocklyNode.NodeType == NodeType.BlocklyPipeline)
                {
                    blocklyNode.BlocklyWorkspace.LoadXml("<xml><block type=\"pipeline_pipelineBody\" deletable=\"false\" movable=\"false\"><value name=\"webDataModel\"><block type=\"pipeline_createWebModel\"></block></value></block></xml>");
                }
                else
                {
                    blocklyNode.BlocklyWorkspace.LoadXml("<xml></xml>");
                }
            }
            WorkspaceXml = blocklyNode.BlocklyWorkspace.InnerXml;
            NodeName = blocklyNode.NodeName;
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blocklyNode.NodeType == NodeType.BlocklyPipeline)
            {
                RunnerToConnect = "PipelineRunner";
                ToolkitXml = await System.IO.File.ReadAllTextAsync("blocklyToolboxes/pipelineToolbox.xml");
                return Page();
            }
            else if (blocklyNode.NodeType == NodeType.BlocklyTask)
            {
                RunnerToConnect = "TaskRunner";
                ToolkitXml = await System.IO.File.ReadAllTextAsync("blocklyToolboxes/taskToolbox.xml");
                return Page();
            }
            else return NotFound();
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
            BlocklyNode node = null;
            string nodeGuidString = ((JValue)messageObj.NodeGuid).Value<string>();
            nodeGuid = Guid.Parse(nodeGuidString);
            if (!project.ContainsNodeGuid(nodeGuid))
                throw new ArgumentException(
                    $"Provided Guid {nodeGuid} does not exist. Please use the CreateNew flag.");

            node = (BlocklyNode)project[nodeGuid];
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