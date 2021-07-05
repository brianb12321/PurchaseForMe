//Will require implicit any.
import DarkTheme from '@blockly/theme-dark';
import * as signalR from "@microsoft/signalr";
import * as Blockly from "blockly";
import { registerErrorHandlingBlocks } from './customBlocks/errorHandlingBlocks';
import { registerPipelineBlocks } from './customBlocks/pipelineBlocks';
import { registerWebBlocks } from './customBlocks/webBlocks';
import { AceEditor } from './AceEditor';
import { WebConsole } from './WebConsole';
import { registerCreateObjectBlock } from './customBlocks/createObjectBlock';
import { registerTimerBlocks } from './customBlocks/timerBlocks';
import { registerObjectBlocks } from './customBlocks/objectBlocks';
import { SignalRRunner } from './signalR/SignalRRunner';
import { SignalRPipelineRunner } from './signalR/SignalRPipelineRunner';
import { SignalRTaskRunner } from './signalR/SignalRTaskRunner';

let xmlEditor: AceEditor;
let webConsole: WebConsole;
let workspace: Blockly.WorkspaceSvg;

export function AddAntiForgeryToken(data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};
export function addBlocks() {
    registerWebBlocks();
    registerErrorHandlingBlocks();
    registerPipelineBlocks();
    registerObjectBlocks();
    registerCreateObjectBlock();
    registerTimerBlocks();
}

addBlocks();

export function initWorkspace() {
    //Remove margin at bottom of nav.
    document.querySelector(".navbar").classList.remove("mb-3");
    //https://developers.google.com/blockly/guides/configure/web/resizable

    const workspaceXml = $("#WorkspaceXml").val() as string;
    const projectGuid = $("#ProjectGuid").val();
    const nodeGuid = $("#NodeGuid").val();
    const userId = $("#UserId").val();
    const runnerToConnect = $("#RunnerToConnect").val();

    var blocklyDiv = document.getElementById('blockly-editor');
    workspace = Blockly.inject(blocklyDiv,
        {
            toolbox: document.getElementById("toolbox"),
            grid: {
                spacing: 20,
                length: 3,
                colour: '#ccc',
                snap: true
            },
            theme: DarkTheme
        });
    //Add the pipeline block.
    Blockly.Xml.domToWorkspace(Blockly.Xml.textToDom(workspaceXml), workspace);

    //Configure xml generation and setup editor.
    xmlEditor = new AceEditor();
    xmlEditor.editorElement = document.querySelector("#xmlEditor");
    xmlEditor.initAceEditor();
    workspace.addChangeListener((event) => {
        if (event.type === Blockly.Events.BLOCK_CHANGE ||
            Blockly.Events.BLOCK_CREATE ||
            Blockly.Events.BLOCK_DELETE ||
            Blockly.Events.BLOCK_DRAG ||
            Blockly.Events.BLOCK_MOVE) {
            let generatedXml = Blockly.Xml.domToPrettyText(Blockly.Xml.workspaceToDom(workspace));
            xmlEditor.setValue(generatedXml);
        }
    });

    $("#saveButton").click(evt => {
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var form = $('#__AjaxAntiForgeryForm');
        evt.preventDefault();
        var xml = Blockly.Xml.domToText(Blockly.Xml.workspaceToDom(workspace));
        var body: any = {
            WorkspaceXml: xml,
            ProjectGuid: projectGuid,
            NodeName: $("#NodeName").val(),
            NodeGuid: nodeGuid,
        }
        $.ajax({
            type: "POST",
            url: "BlockEditor",
            headers: {
                "RequestVerificationToken": token.toString()
            },
            data: JSON.stringify(body),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (data, textStatus, response: any) => {
                var result = JSON.parse(response.responseText);
                if (response.status === 200) {
                    webConsole.writeLine(result.Message);
                    $("#NodeGuid").val(result.NodeGuid);
                    $('input[name="CreateNew"]').val("False");
                }
            },
            error: function (jqXHR: any, textStatus, errorThrown) {
                webConsole.writeLine(jqXHR.responseText);
            }
        });
    });

    //Setup run button
    if (runnerToConnect === "TaskRunner") {
        $("#runButton").html("Run task immediately");
    }
    $("#runButton").click((evt) => {
        evt.preventDefault();
        var xml = Blockly.Xml.domToText(Blockly.Xml.workspaceToDom(workspace));
        var runPipelineRequest = {
            WorkspaceXML: xml,
            ReturnCode: true
        };
        let connection: SignalRRunner;
        if (runnerToConnect === "PipelineRunner") {
            connection = new SignalRPipelineRunner(webConsole);
            connection.url = "/pipelineRunner";
        }
        else if (runnerToConnect === "TaskRunner") {
            connection = new SignalRTaskRunner(webConsole);
            connection.url = "/taskRunner";
        }
        connection.setup();
        connection.onConnectedEvent = () => {
            if (runnerToConnect === "PipelineRunner") {
                if (connection.connection.state === signalR.HubConnectionState.Connected) {
                    connection.connection.send("RunPipelineBlockly", JSON.stringify(runPipelineRequest))
                        .then(() => {
                            webConsole.writeLine("Message sent.");
                            $("#runButton").prop("disabled", true);
                        })
                        .catch((error) => {
                            webConsole.writeLine(`An error occurred while sending a message: ${error}`);
                        });
                } else {
                    webConsole.writeLine("Cannot run pipeline; a connection to a pipeline runner has not been established.");
                }
            }
            else if (runnerToConnect === "TaskRunner") {
                var taskRequest = {
                    ProjectGuid: projectGuid,
                    NodeGuid: nodeGuid,
                    UserId: userId
                };
                (connection as SignalRTaskRunner).runTask(taskRequest);
            }
        }
        connection.connect();
    });
    //Setup toggle XML button
    $("#toggleXmlButton").text("Show XML");
    $("#toggleXmlButton").click(evt => {
        evt.preventDefault();
        let xmlEditor = $("#xmlEditor");
        if (xmlEditor.css("display") === "none") {
            xmlEditor.css("display", "block");
            $("#toggleXmlButton").text("Hide XML");
        } else {
            xmlEditor.css("display", "none");
            $("#toggleXmlButton").text("Show XML");
        }
        workspace.resizeContents();
        workspace.resize();
    });
    $("#clearConsoleButton").click(evt => {
        evt.preventDefault();
        webConsole.clear();
    });
}

export function initConsole() {
    webConsole = new WebConsole();
    webConsole.initialize();
}

initConsole();
initWorkspace();