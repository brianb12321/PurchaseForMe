import * as Blockly from 'blockly';

export function registerWebBlocks() {
    Blockly.Blocks["web_openDriver"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Opens a Selenium driver instance.");
            block.setColour(100);
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.appendDummyInput()
                 .appendField("Open web driver")
                 .appendField(new Blockly.FieldDropdown([
                         ["Chrome", "Chrome"]
                     ]), "driverType");
            block.appendValueInput("url")
                 .appendField("URL")
                 .setCheck("String")
                . setAlign(Blockly.ALIGN_RIGHT);
            block.appendStatementInput("body")
                 .appendField("body");
        }
    }

    Blockly.Blocks["web_getElement"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Gets an element from the DOM based on a specific criteria.");
            block.setColour(136);
            block.setInputsInline(true);
            block.setOutput(true, "IWebElement");
            block.appendDummyInput()
                 .appendField("DOM element")
                 .appendField(new Blockly.FieldDropdown([
                    ["Id", "Id"],
                    ["Class", "Class"],
                     ["Name", "Name"],
                     ["Tag Name", "TagName"]
                 ]), "elementType");
            block.appendValueInput("elementName")
                 .appendField("of")
                 .setCheck("String")
                 .setAlign(Blockly.ALIGN_RIGHT);
        }
    }
    Blockly.Blocks["web_getSubElements"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Gets all children elements from the DOM based on a specific criteria.");
            block.setColour(136);
            block.setInputsInline(true);
            block.setOutput(true, "Array");
            block.appendDummyInput()
                .appendField("All DOM elements")
                .appendField(new Blockly.FieldDropdown([
                    ["Id", "Id"],
                    ["Class", "Class"],
                    ["Name", "Name"],
                    ["Tag Name", "TagName"]
                ]), "elementType");
            block.appendValueInput("elementName")
                .appendField("of")
                .setCheck("String")
                .setAlign(Blockly.ALIGN_RIGHT);
        }
    }

    Blockly.Blocks["web_getElementDetail"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Extracts specific information from a DOM element. A Selenium driver must be opened");
            block.setColour(160);
            block.setOutput(true, "String");
            block.setInputsInline(true);
            block.appendDummyInput()
                .appendField("Get element")
                .appendField(new Blockly.FieldDropdown([
                    ["Inner HTML", "InnerHtml"]
                ]), "informationType");

            block.appendValueInput("element")
                .appendField("from")
                .setCheck("IWebElement");
        }
    }
}