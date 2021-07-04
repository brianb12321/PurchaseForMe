import * as Blockly from "blockly";

export function registerCreateObjectBlock() {
    let mutatorMixim = {
        mutationToDom: function () {
            var block = this as unknown as Blockly.Block;
            var container = document.createElement("mutation");
            container.setAttribute("items", (this as unknown as any).propertyCount.toString());
            return container;
        },
        domToMutation: function (xmlElement: any) {
            this["propertyCount"] = parseInt(xmlElement.getAttribute('items'), 10);
            var block = this as unknown as Blockly.Block;
            (this as unknown as any).updateShape(block);
        },
        decompose(workspace: Blockly.Workspace): Blockly.Block {
            var topBlock = workspace.newBlock("pipeline_createObject_empty");
            (topBlock as any).initSvg();
            var connection = topBlock.getInput("propertyList").connection;
            for (let i = 0; i < this["propertyCount"]; i++) {
                var propertyBlock = workspace.newBlock("pipeline_createObject_property");
                (propertyBlock as any).initSvg();
                connection.connect(propertyBlock.previousConnection);
                connection = propertyBlock.nextConnection;
            }
            return topBlock;
        },
        compose(topBlock: Blockly.Block) {
            let block = this as unknown as Blockly.Block;
            var propertyBlock = topBlock.getInputTargetBlock("propertyList");
            var connections = [];
            while (propertyBlock && !propertyBlock.isInsertionMarker()) {
                connections.push((propertyBlock as any).valueConnection_);
                propertyBlock = propertyBlock.nextConnection && propertyBlock.nextConnection.targetBlock();
            }
            for (var i = 0; i < this["propertyCount"]; i++) {
                var connection = block.getInput('value' + i).connection.targetConnection;
                if (connection && connections.indexOf(connection) === -1) {
                    connection.disconnect();
                }
            }
            this["propertyCount"] = connections.length;
            (this as unknown as any).updateShape();
            // Reconnect any child blocks.
            for (var i = 0; i < this["propertyCount"]; i++) {
                Blockly.Mutator.reconnect(connections[i], block, 'value' + i);
            }
        },
        saveConnections(topBlock: Blockly.Block) {
            let block = this as unknown as Blockly.Block;
            var itemBlock = topBlock.getInputTargetBlock('propertyList');
            var i = 0;
            while (itemBlock) {
                var input = block.getInput('value' + i);
                (itemBlock as any).valueConnection_ = input && input.connection.targetConnection;
                i++;
                itemBlock = itemBlock.nextConnection &&
                    itemBlock.nextConnection.targetBlock();
            }
        },
        updateShape() {
            let block = this as unknown as Blockly.Block;
            if (this["propertyCount"] && block.getInput('EMPTY')) {
                block.removeInput('EMPTY');
            } else if (!this["propertyCount"] && !block.getInput('EMPTY')) {
                block.appendDummyInput('EMPTY')
                    .appendField("create empty object");
            }
            for (var i = 0; i < this["propertyCount"]; i++) {
                if (!block.getInput(`value${i}`)) {
                    let textInput = new Blockly.FieldTextInput();
                    textInput.setSpellcheck(false);
                    block.appendValueInput(`value${i}`)
                        .appendField("Name")
                        .appendField(textInput, `property${i}`)
                        .setAlign(Blockly.ALIGN_RIGHT);
                }
            }
            // Remove deleted inputs.
            while (block.getInput('value' + i)) {
                block.removeInput('value' + i);
                i++;
            }
        }
    }


    Blockly.Blocks["pipeline_createObject_property"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setColour(30);
            block.setPreviousStatement(true);
            block.setNextStatement(true);
            block.appendDummyInput()
                .appendField("property");
            block.contextMenu = false;
        }
    }
    Blockly.Blocks["pipeline_createObject_empty"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.setColour(30);
            block.appendStatementInput("propertyList")
                .appendField("properties");
        }
    }

    Blockly.Extensions.registerMutator("pipeline_createObject_mutator", mutatorMixim, null, ["pipeline_createObject_property"]);

    Blockly.Blocks["pipeline_createObject"] = {
        init: function () {
            this["propertyCount"] = 3;
            let block = this as Blockly.Block;
            block.jsonInit({'mutator': 'pipeline_createObject_mutator'});
            block.setTooltip("Creates a new object");
            block.setOutput(true, "Object");
            block.setColour(30);
            (this as any).updateShape();
        }
    }
}