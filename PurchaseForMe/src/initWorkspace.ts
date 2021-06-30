//Will require implicit any.
import DarkTheme from '@blockly/theme-dark';
import * as signalR from "@microsoft/signalr";
import * as Blockly from "blockly";
import { registerErrorHandlingBlocks } from './customBlocks/ErrorHandlingBlocks';
import { registerPipelineBlocks } from './customBlocks/PipelineBlocks';
import { registerWebBlocks } from './customBlocks/WebBlocks';
import { AceEditor } from './AceEditor';
import { WebConsole } from './WebConsole';
import { registerCreateObjectBlock } from './customBlocks/createObjectBlock';

let xmlEditor: AceEditor;
let webConsole: WebConsole;

export function AddAntiForgeryToken(data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};
export function addBlocks() {
    registerWebBlocks();
    registerErrorHandlingBlocks();
    registerPipelineBlocks();
    registerCreateObjectBlock();
}
export function setupConnection(): signalR.HubConnection {
    //Open SignalR connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/pipelineRunner")
        .build();

    connection.on("Result",
        (result) => {
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
    } else {
        workspaceXml = '<xml><block type="pipeline_pipelineBody" deletable="false" movable="false"><value name="webDataModel"><block type="pipeline_createWebModel"></block></value></block></xml>';
    }
    var blocklyDiv = document.getElementById('blockly-editor');
    var workspace = Blockly.inject(blocklyDiv,
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
                .then(() => webConsole.writeLine("Message sent."))
                .catch((error) => {
                    webConsole.writeLine(`An error occurred while sending a message: ${error}`);
                });
        } else {
            webConsole.writeLine("Cannot run pipeline; a connection to a pipeline runner has not been established.");
        }
    });
}

export function initConsole() {
    webConsole = new WebConsole();
    webConsole.initialize();
}

initConsole();
initWorkspace();