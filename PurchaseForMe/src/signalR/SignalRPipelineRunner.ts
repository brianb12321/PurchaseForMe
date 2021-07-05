import { WebConsole } from "../WebConsole";
import { SignalRRunner } from "./SignalRRunner";

export class SignalRPipelineRunner extends SignalRRunner {

    constructor(private webConsole: WebConsole) {
        super();
    }
    protected onResult(result) {
        $("#runButton").prop("disabled", false);
        let resultJson: any = JSON.parse(result);
        if (resultJson.IsSuccessful) {
            this.webConsole.writeLine("Pipeline ran successfully.");
        } else {
            this.webConsole.writeLine("Pipeline returned with error.");
        }
        this.connection.stop();
        this.webConsole.writeLine(result);
    }
    protected onConsole(message) {
        this.webConsole.write(message);
    }
    protected onConnected() {
        this.webConsole.writeLine("Connection established with pipeline runner.");
    }
    protected onConnectionError(error) {
        this.webConsole.writeLine(
            `An error occurred while establishing a connection to the pipeline runner: ${error}`);
    }
}