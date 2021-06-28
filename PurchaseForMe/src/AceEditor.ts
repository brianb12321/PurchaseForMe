import * as ace from 'ace-builds'

export class AceEditor {
    editor: ace.Ace.Editor;
    editorElement: Element;
    public initAceEditor() {
        this.editor = ace.edit(this.editorElement);
        ace.config.set("basePath", "https://pagecdn.io/lib/ace/1.4.12");
        this.editor.setTheme("ace/theme/monokai");
        this.editor.session.setMode("ace/mode/xml");
        this.editor.setReadOnly(true);
    }
    public setLanguage(language: string) {
        this.editor.session.setMode(`ace/mode/${language}`);
    }
    public setValue(value: string) {
        this.editor.session.setValue(value);
        this.editor.clearSelection();
    }
}