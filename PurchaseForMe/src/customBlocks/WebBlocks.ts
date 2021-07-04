import * as Blockly from 'blockly';
import { registerGetElementBlocks } from './getElementBlocks';

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
                .setAlign(Blockly.ALIGN_RIGHT);
            block.appendStatementInput("body")
                .appendField("body");
            block.appendStatementInput("setupBody")
                .appendField("setup");
        }
    }

    registerGetElementBlocks();

    Blockly.Blocks["web_getElementDetail"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Extracts specific information from a DOM element. NOTE: A Selenium driver must be opened, information will be queried using Javascript");
            block.setColour(160);
            block.setOutput(true, "String");
            block.setInputsInline(true);
            block.appendDummyInput()
                .appendField("Get element")
                .appendField(new Blockly.FieldDropdown([
                    ["Inner HTML", "InnerHtml"],
                    ["Inner Text", "InnerText"]
                ]), "informationType");

            block.appendValueInput("element")
                .appendField("from")
                .setCheck("IWebElement");
        }
    }
    Blockly.Blocks["web_getElementAttribute"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Extracts the specified attribute value from a DOM element. NOTE: A Selenium driver must be opened, information will be queried using Javascript");
            block.setColour(160);
            block.setOutput(true, "String");
            block.appendValueInput("attributeName")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Get element attribute");

            block.appendValueInput("element")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("from")
                .setCheck("IWebElement");
        }
    }
    Blockly.Blocks["web_sendKey"] = {
        init: function() {
            let block = this as Blockly.Block;
            block.setTooltip("Sends the specific string as input to an DOM element.");
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendValueInput("element")
                .appendField("Send Keys to")
                .setCheck("IWebElement");
            block.appendValueInput("keyString")
                .appendField("with")
                .setCheck("String");
            block.appendDummyInput()
                .appendField(new Blockly.FieldCheckbox(), "shouldSendEnterKey")
                .appendField("include ENTER key");
        }
    }
    Blockly.Blocks["web_clickElement"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Clicks the specific element as if a user clicked on an item.");
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendValueInput("element")
                .appendField("Click element")
                .setCheck("IWebElement");
        }
    }
    Blockly.Blocks["web_maximizeWindow"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Maximizes the window instance created by the driver");
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendDummyInput()
                .appendField("Maximize web window");
        }
    }
    Blockly.Blocks["web_navigateUrl"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Instructs the driver to navigate to a specified URL");
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendValueInput("url")
                .setCheck("String")
                .appendField("Navigate to");
        }
    }
}