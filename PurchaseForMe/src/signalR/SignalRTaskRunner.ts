import { WebConsole } from "../WebConsole";
import { SignalRRunner } from "./SignalRRunner";
import * as signalR from "@microsoft/signalr";

export class SignalRTaskRunner extends SignalRRunner {
    constructor(private taskConsole: WebConsole) {
        super();
    }
    protected onResult(result) {
        $("#runButton").prop("disabled", false);
        let resultJson: any = JSON.parse(result);
        if (resultJson.IsSuccessful) {
            this.taskConsole.writeLine("Task ran successfully.");
        } else {
            this.taskConsole.writeLine("Task returned with error.");
        }
        this.connection.stop();
        this.taskConsole.writeLine(result);
    }
    protected onConsole(message) {
        this.taskConsole.write(message);
    }
    protected onConnected() {
        this.taskConsole.writeLine("Connection established with task runner.");
    }
    public runTask(taskRequest: any) {
        if (this.connection.state === signalR.HubConnectionState.Connected) {
            this.connection.send("RunTaskBlockly", JSON.stringify(taskRequest))
                .then(() => {
                    this.taskConsole.writeLine("Message sent.");
                    $("#runButton").prop("disabled", true);
                })
                .catch((error) => {
                    this.taskConsole.writeLine(`An error occurred while sending a message: ${error}`);
                });
        }
    }
    protected onConnectionError(error) {
        this.taskConsole.writeLine(
            `An error occurred while establishing a connection to the task runner: ${error}`);
    }
}