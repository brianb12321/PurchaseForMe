import * as Blockly from 'blockly';

export function registerTimerBlocks() {
    Blockly.Blocks["timer_loopWait"] = {
        init: function() {
            let block = this as Blockly.Block;
            block.setTooltip("Waits for a specified amount of time before looping.");
            block.setColour(180);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.setInputsInline(true);
            block.appendDummyInput()
                .appendField("Loop wait for");

            block.appendValueInput("waitTime")
                .setCheck("Number");
            block.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([
                        ["Milliseconds", "Milliseconds"],
                        ["Seconds", "Seconds"],
                        ["Minutes", "Minutes"],
                        ["Hours", "Hours"]
                    ]), "waitDurationType");

            block.appendStatementInput("loopBody")
                .appendField("run");
        }
    }
    Blockly.Blocks["timer_loopControl"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Determines flow control of a custom loop.");
            block.setColour(360);
            block.setPreviousStatement(true);
            block.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([
                        ["Break", "Break"]
                    ]),
                    "loopControlAction")
                .appendField("loop");
        }
    }
    Blockly.Blocks["timer_wait"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setTooltip("Stops running code for a specified duration");
            block.setColour(180);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.appendDummyInput()
                .appendField("Wait for");
            block.appendValueInput("waitTime")
                .setCheck("Number");
            block.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([
                    ["Milliseconds", "Milliseconds"],
                    ["Seconds", "Seconds"],
                    ["Minutes", "Minutes"],
                    ["Hours", "Hours"]
                ]), "waitDurationType");
        }
    }
}