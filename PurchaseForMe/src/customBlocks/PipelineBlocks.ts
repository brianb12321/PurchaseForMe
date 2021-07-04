import * as Blockly from 'blockly';

export function registerPipelineBlocks() {
    Blockly.Blocks["pipeline_pipelineBody"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Represents the body of the web-scraping pipeline. Must return a web data-model.");
            block.setDeletable(false);
            block.setColour(50);
            block.appendStatementInput("body")
                 .appendField("Pipeline body");
            block.appendValueInput("webDataModel")
                 .appendField("Web data model")
                 .setCheck("WebDataModel");
        }
    }
    Blockly.Blocks["pipeline_createWebModel"] = {
        init: function() {
            let block = this as Blockly.Block;
            block.setTooltip("Creates a web model--a collection of scraped data that can be used later in a task.");
            block.setOutput(true, "WebDataModel");
            block.setColour(30);
            block.appendValueInput("object")
                .appendField("New web data model")
                .appendField("object");
        }
    }

    Blockly.Blocks["pipeline_getObjectValue"] = {
        init: function() {
            let block = this as Blockly.Block;
            block.setTooltip("Gets the value from a property attached to an input object. You may use dot notation to traverse the object graph.");
            block.setColour(30);
            block.setInputsInline(true);
            block.setOutput(true);
            block.appendValueInput("propertyName")
                .appendField("Get property")
                .setCheck("String");
            block.appendValueInput("object")
                .appendField("from")
                .setCheck("Object");
        }
    }
}