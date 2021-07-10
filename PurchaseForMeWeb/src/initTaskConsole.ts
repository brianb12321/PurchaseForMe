import { WebConsole } from "./WebConsole";
import { SignalRCodeRunner } from "./signalR/SignalRCodeRunner";
import { ICodeRequest } from "./signalR/ICodeRequest";

let taskConsole: WebConsole = new WebConsole();

$("#runButton").click(event => {
    event.preventDefault();
    var projectGuid = $("#ProjectGuid").val();
    var nodeGuid = $("#TaskGuid").val();
    var userId = $("#UserId").val();
    var selectedNodeUrl = $("#nodeUrls").val();
    var taskRequest: ICodeRequest = {
        ProjectGuid: projectGuid.toString(),
        NodeGuid: nodeGuid.toString(),
        UserId: userId.toString(),
        ClusterNodeUrl: selectedNodeUrl.toString()
    }
    var connection = new SignalRCodeRunner(taskConsole);
    connection.url = "/codeMonitoring";
    connection.setup();
    connection.connect();
    connection.onConnectedEvent = () => {
        connection.registerToCodeMonitor(nodeGuid.toString());
        connection.runCode(taskRequest);
    };
});

taskConsole.initialize();