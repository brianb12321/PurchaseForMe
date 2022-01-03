import * as Blockly from 'blockly';
import "blockly/javascript";
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
                     ["Chrome", "Chrome"],
                     ["Remote", "Remote"]
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
    Blockly.Blocks["web_selectDropDown"] = {
        init: function() {
            let block = this as Blockly.Block;
            block.setTooltip("Does stuff.");
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendValueInput("element")
                .setCheck("IWebElement")
                .appendField("Select Dropdown from");
            block.appendValueInput("selectValue")
                .setCheck("String")
                .appendField("with value");
        }
    }
    Blockly.Blocks["web_elementIsEnabled"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Does stuff.");
            block.setInputsInline(true);
            block.setOutput(true, "Boolean");
            block.setColour(360);
            block.appendValueInput("element")
                .setCheck("IWebElement")
                .appendField("Element");
            block.appendDummyInput()
                .appendField("enabled");
        }
    }
    Blockly.Blocks["web_executeJavascript"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Does stuff.");
            block.setInputsInline(true);
            block.setOutput(true);
            block.setColour(360);
            block.appendValueInput("statement")
                .setCheck("String")
                .appendField("execute Javascript");
        }
    }
    Blockly.Blocks["web_refresh"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Does stuff.");
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendDummyInput()
                .appendField("Refresh page");
        }
    }
    Blockly.Blocks["web_waitPage"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Does stuff.");
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setColour(360);
            block.appendDummyInput()
                .appendField("Wait for page load");
        }
    }
    Blockly.Blocks["web_waitForElement"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Does stuff.");
            block.setColour(360);
            block.setInputsInline(true);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.appendDummyInput()
                .appendField("Wait for element")
                .appendField(new Blockly.FieldDropdown([
                    ["Id", "Id"],
                    ["Class", "Class"],
                    ["Name", "Name"],
                    ["Tag Name", "TagName"],
                    ["CSS Selector", "CssSelector"],
                    ["XPath", "XPath"]
                ]), "elementType");

            block.appendValueInput("elementName")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("of")
                .setCheck("String")
                .setAlign(Blockly.ALIGN_RIGHT);
            block.appendDummyInput()
                .appendField("to")
                .appendField(new Blockly.FieldDropdown([
                        ["load", "Load"],
                        ["click", "Click"]
                    ]),
                    "waitType");
        }
    }
}