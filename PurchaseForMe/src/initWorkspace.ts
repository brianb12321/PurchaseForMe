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
export function setupConnection(): signalR.HubConnection {
    //Open SignalR connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/pipelineRunner")
        .build();

    connection.on("Result",
        (result) => {
            $("#runButton").prop("disabled", false);
            let resultJson: any = JSON.parse(result);
            if (resultJson.IsSuccessful) {
                webConsole.writeLine("Pipeline ran successfully.");
            } else {
                webConsole.writeLine("Pipeline returned with error.");
            }
            webConsole.writeLine(result);
        });
    connection.on("Console",
        result => {
            webConsole.write(result);
        });
    connection.start()
        .then(() => {
            webConsole.writeLine("Connection established with pipeline runner.");
        })
        .catch(error => webConsole.writeLine(
            `An error occurred while establishing a connection to the pipeline runner: ${error}`));

    return connection;
}

addBlocks();

export function initWorkspace() {
    //Remove margin at bottom of nav.
    document.querySelector(".navbar").classList.remove("mb-3");
    //https://developers.google.com/blockly/guides/configure/web/resizable

    var createNew = $("#CreateNew").val();
    var workspaceXml: string;
    if (createNew === "False") {
        workspaceXml = $("#WorkspaceXml").val() as string;
    }
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

    const connection = setupConnection();

    $("#saveButton").click(evt => {
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        var form = $('#__AjaxAntiForgeryForm');
        evt.preventDefault();
        var xml = Blockly.Xml.domToText(Blockly.Xml.workspaceToDom(workspace));
        var createNew = $("#CreateNew").val();
        var projectGuid = $("#ProjectGuid").val();

        var body: any = {
            WorkspaceXml: xml,
            ProjectGuid: projectGuid,
            CreateNew: createNew,
            NodeName: $("#NodeName").val()
        }
        if (createNew === "False") {
            body.NodeGuid = $("#NodeGuid").val();
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
    $("#runButton").click((evt) => {
        evt.preventDefault();
        var xml = Blockly.Xml.domToText(Blockly.Xml.workspaceToDom(workspace));
        var runPipelineRequest = {
            WorkspaceXML: xml,
            ReturnCode: true
        };

        if (connection.state === signalR.HubConnectionState.Connected) {
            connection.send("RunPipelineBlockly", JSON.stringify(runPipelineRequest))
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