import * as signalR from "@microsoft/signalr";

export class SignalRRunner {
    public url: string;
    public connection: signalR.HubConnection;
    public onConnectedEvent: () => void;
    protected onResult(result: any) {

    }
    protected onConsole(message) {

    }
    protected onConnectionError(error: any) {

    }
    protected onConnected() {

    }

    public setup() {
        //Open SignalR connection
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.url)
            .build();

        this.connection.on("Result",
            (result) => {
                this.onResult(result);
            });
        this.connection.on("Console",
            result => {
                this.onConsole(result);
            });
    }
    public connect() {
        this.connection.start()
            .then(() => {
                this.onConnected();
                this?.onConnectedEvent();
            })
            .catch(error => this.onConnectionError(error));
    }
}