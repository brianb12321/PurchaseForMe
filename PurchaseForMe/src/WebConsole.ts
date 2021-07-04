import 'jq-console';

export class WebConsole {
    console: any;
    public initialize() {
        this.console = ($('#console') as any).jqconsole("Console ready\n", ">>>", "", true);
    }
    public write(message: string) {
        this.console.Write(message, "jqconsole-output");
    }
    public writeLine(message: string) {
        this.write(message + "\n");
    }
    public clear() {
        this.console.Clear();
    }
}