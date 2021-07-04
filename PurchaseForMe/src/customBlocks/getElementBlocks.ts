import * as Blockly from "blockly";

export function registerGetElementBlocks() {
    let mutatorMixim = {
        mutationToDom: function() {
            var block = this as unknown as Blockly.Block;
            var container = document.createElement("mutation");
            var elementInput: any = (block.getFieldValue("from") == "Element");
            container.setAttribute("elementInput", elementInput);
            return container;
        },
        domToMutation(xmlElement) {
            var elementInput = (xmlElement.getAttribute("elementInput") == "true");
        },
        updateShape_(hasElementInput) {
            let block = this as unknown as Blockly.Block;
            let elementInputExist = block.getInput("rootElement");
            if (hasElementInput) {
                if (!elementInputExist) {
                    block.appendValueInput("rootElement")
                        .setCheck("IWebElement")
                        .setAlign(Blockly.ALIGN_RIGHT)
                        .appendField("from");
                }
            } else if (elementInputExist) {
                block.removeInput("rootElement");
            }
        }
    }

    Blockly.Extensions.registerMutator("web_getElement_mutator", mutatorMixim);

    Blockly.Blocks["web_getElement"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.jsonInit({ 'mutator': 'web_getElement_mutator' });
            block.setTooltip("Gets an element from the DOM based on a specific criteria.");
            block.setColour(136);
            block.setOutput(true, "IWebElement");
            var fromField = new Blockly.FieldDropdown([
                ["From root", "Root"],
                ["From element", "Element"]
            ]);
            fromField.setValidator(option => {
                var elementInput = (option == "Element");
                (block as any).updateShape_(elementInput);
            });

            block.appendDummyInput()
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField(fromField, "from")
                .appendField("DOM element")
                .appendField(new Blockly.FieldDropdown([
                    ["Id", "Id"],
                    ["Class", "Class"],
                    ["Name", "Name"],
                    ["Tag Name", "TagName"],
                    ["CSS Selector", "CssSelector"]
                ]), "elementType");

            block.appendValueInput("elementName")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("of")
                .setCheck("String")
                .setAlign(Blockly.ALIGN_RIGHT);
        }
    }
    Blockly.Blocks["web_getSubElements"] = {
        init: function () {
            let block = this as Blockly.Block;
            block.jsonInit({ 'mutator': 'web_getElement_mutator' });
            block.setTooltip("Gets all children elements from the DOM based on a specific criteria.");
            block.setColour(136);
            block.setOutput(true, "Array");
            var fromField = new Blockly.FieldDropdown([
                ["From root", "Root"],
                ["From element", "Element"]
            ]);
            fromField.setValidator(option => {
                var elementInput = (option == "Element");
                (block as any).updateShape_(elementInput);
            });

            block.appendDummyInput()
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField(fromField, "from")
                .appendField("All DOM elements")
                .appendField(new Blockly.FieldDropdown([
                    ["Id", "Id"],
                    ["Class", "Class"],
                    ["Name", "Name"],
                    ["Tag Name", "TagName"],
                    ["CSS Selector", "CssSelector"]
                ]), "elementType");
            block.appendValueInput("elementName")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("of")
                .setCheck("String")
                .setAlign(Blockly.ALIGN_RIGHT);
        }
    }
}