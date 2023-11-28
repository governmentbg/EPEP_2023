function switchInputs() {
    // arguments shoud be (disableGroup, value, value1, selector1, value2, selector2, ....)

    if (arguments.length < 4 || arguments.length % 2 != 0) {
        throw ("Incorrect usage.");
    }
    var disableGroup = arguments[0];
    var value = arguments[1];
    //added
    var chosenSelector;

    var disableGroupClass = 'disable_group_' + disableGroup;

    for (i = 2; i < arguments.length; i += 2) {
        var currValue = arguments[i];
        var currSelector = arguments[i + 1];
        if (currValue == value) {
            //added
            chosenSelector = currSelector;
            //removed:
            //$(currSelector).show().find(':input.' + disableGroupClass).removeAttr('disabled').removeClass(disableGroupClass);
            //$(currSelector).removeAttr('disabled').removeClass(disableGroupClass);
        }
        else {
            $(currSelector).hide().find(':input[disabled!=true]').attr("disabled", "disabled").addClass(disableGroupClass);
            $(currSelector).attr("disabled", "disabled").addClass(disableGroupClass);
        }
    }
    //added
    $(chosenSelector).show().find(':input.' + disableGroupClass).removeAttr('disabled').removeClass(disableGroupClass);
    $(chosenSelector).removeAttr('disabled').removeClass(disableGroupClass);
}

function randomString() {
    return (((1 + Math.random()) * 0x10000000) | 0).toString(16).substring(1);
}
function newId() {
    return (randomString() + randomString());
}

// options
// {
//     defaultValue: 0                                          // Initial value
//     arguments: [ ['0', '#0']  ],                             // [selected_value, dom_element_selector]
//     selector: '[name=person_type_choice@(editorId)]',        // event (change) selector
//     switchDefault: '@showPhysical'                           // default value for not found case
// }

// NOTE: for checkbox switchDefault must be False !!!!!

function switchBlocks(options) {

    setTimeout(function () {
        var disableGroupClass = 'disable_group_' + newId();

        var fnSB = function (_options) {

            for (i = 0; i < _options.arguments.length; i++) {

                var currValue = _options.arguments[i][0];
                var currSelector = _options.arguments[i][1];

                if (currValue == _options.defaultValue) {
                    $(currSelector).show().find(':input.' + disableGroupClass + ':not(.disabled)').removeAttr('disabled').removeClass(disableGroupClass);
                    $(currSelector).removeAttr('disabled').removeClass(disableGroupClass);

                    //$(currSelector).find(':checkbox, input[type=radio]:checked, select').change();
                }
                else {
                    $(currSelector).hide().find(':input[disabled!=true]').attr("disabled", "disabled").addClass(disableGroupClass);
                    $(currSelector).attr("disabled", "disabled").addClass(disableGroupClass);
                }
            }
        }

        fnSB(options);

        $(options.selector).change(function () {

            var visited = false;
            var _options = options;

            for (var i = 0; i < options.arguments.length; i++) {

                if (this.type === 'checkbox') {
                    if (this.checked && options.arguments[i][0] === 'True') {
                        _options.defaultValue = 'True';
                        visited = true;
                        break;
                    }
                }
                else {

                    if (this.value == options.arguments[i][0]) {
                        _options.defaultValue = options.arguments[i][0];
                        visited = true;
                        break;
                    }
                }
            }

            if (!visited) {
                _options.defaultValue = options.switchDefault;
            }

            fnSB(_options);
        });
    }, 0);
}
