import * as Blockly from 'blockly';

export function registerErrorHandlingBlocks() {
    Blockly.Blocks["error_tryCatch"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("If an error occurs during execution, run specified code.");
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(100);
            block.appendStatementInput("tryStatement")
                 .appendField("try");

            block.appendStatementInput("catchStatement")
                 .appendField("catch");
        }
    }
    Blockly.Blocks["error_throw"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Throw a specified error");
            block.setPreviousStatement(true);
            block.setColour(360);
            block.appendValueInput("errorObject")
                 .appendField("Throw error")
                 .setCheck("Error")
                 .setAlign(Blockly.ALIGN_RIGHT);
        }
    }
    Blockly.Blocks["error_createError"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Creates a new error object");
            block.setColour(360);
            block.setOutput(true, "Error");
            block.appendValueInput("errorMessage")
                 .appendField("Error message")
                 .setCheck("String")
                 .setAlign(Blockly.ALIGN_RIGHT);
        }
    }
}