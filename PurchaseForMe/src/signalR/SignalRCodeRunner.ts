import { WebConsole } from "../WebConsole";
import { SignalRRunner } from "./SignalRRunner";
import * as signalR from "@microsoft/signalr";
import { ICodeRequest } from "./ICodeRequest";

export class SignalRCodeRunner extends SignalRRunner {
    constructor(private console: WebConsole) {
        super();
    }
    protected onResult(result) {
        $("#runButton").prop("disabled", false);
        let resultJson: any = JSON.parse(result);
        if (resultJson.IsSuccessful) {
            this.console.writeLine("Code ran successfully.");
        } else {
            this.console.writeLine("Code returned with error.");
        }
        this.connection.stop();
        this.console.writeLine(result);
    }
    protected onConsole(message) {
        this.console.write(message);
    }
    protected onConnected() {
        this.console.writeLine("Connection established with code-monitor.");
    }
    public registerToCodeMonitor(codeGuid: string) {
        this.connection.send("RegisterToCodeMonitor", codeGuid);
    }
    public runCode(codeRequest: ICodeRequest) {
        if (this.connection.state === signalR.HubConnectionState.Connected) {
            this.connection.send("RunCodeBlockly", JSON.stringify(codeRequest))
                .then(() => {
                    this.console.writeLine("Message sent.");
                    $("#runButton").prop("disabled", true);
                })
                .catch((error) => {
                    this.console.writeLine(`An error occurred while sending a message: ${error}`);
                });
        }
    }
    protected onConnectionError(error) {
        this.console.writeLine(
            `An error occurred while establishing a connection to the code monitor: ${error}`);
    }
}