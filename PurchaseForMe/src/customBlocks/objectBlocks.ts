import * as Blockly from "blockly";

export function registerObjectBlocks() {
    Blockly.Blocks["object_jsonToObject"] = {
        init: function() {
            let block = this as Blockly.Block;
            block.setTooltip("Creates a new object from JSON string.");
            block.setColour(300);
            block.setOutput(true, "Object");
            block.appendValueInput("jsonText")
                .appendField("from JSON")
                .setCheck("String");
        }
    }
}