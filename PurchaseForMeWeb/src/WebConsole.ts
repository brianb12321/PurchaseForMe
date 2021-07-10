import { Terminal } from "xterm";
import { FitAddon } from "xterm-addon-fit";

export class WebConsole {
    console: Terminal;
    public initialize() {
        this.console = new Terminal();
        let fitAddon = new FitAddon();
        this.console.loadAddon(fitAddon);
        this.console.open(document.querySelector("#console"));
        fitAddon.fit();
        window.onresize = () => fitAddon.fit();
    }
    public write(message: string) {
        this.console.write(message);
    }
    public writeLine(message: string) {
        this.write(message + "\n");
    }
    public clear() {
        this.console.clear();
    }
}