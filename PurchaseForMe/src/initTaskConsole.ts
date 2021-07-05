import { WebConsole } from "./WebConsole";
import { SignalRTaskRunner } from "./signalR/SignalRTaskRunner";
import { connect } from "tls";

let taskConsole: WebConsole = new WebConsole();

$("#runButton").click(event => {
    event.preventDefault();
    var projectGuid = $("#ProjectGuid").val();
    var nodeGuid = $("#TaskGuid").val();
    var userId = $("#UserId").val();
    var taskRequest = {
        ProjectGuid: projectGuid,
        NodeGuid: nodeGuid,
        UserId: userId
    }
    var connection = new SignalRTaskRunner(taskConsole);
    connection.url = "/taskRunner";
    connection.setup();
    connection.connect();
    connection.onConnectedEvent = () => connection.runTask(taskRequest);
});

taskConsole.initialize();