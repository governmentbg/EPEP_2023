function createAutoComplete(url, textboxId, codeFieldId, nameFieldId, parentFieldId, additionalOptions) {
    var $textbox = $('#' + textboxId);
    var $nameField = $('#' + nameFieldId);
    var $codeField = $('#' + codeFieldId);

    var getParentFunction;
    if (parentFieldId) {
        getParentFunction = function () {
            return $('#' + parentFieldId).val();
        }
    }

    var options = {
        dataType: 'json',
        parse: function (data) {
            var rows = new Array();
            for (var i = 0; i < data.length; i++) {
                rows[i] = { data: data[i], value: data[i].Tag, result: data[i].Tag };
            }
            return rows;
        },
        formatItem: function (row, i, max) { return row.Tag; },
        extraParams: getParentFunction ? { code: getParentFunction } : {},
        width: 400,
        highlight: false,
        multiple: false,
        max: 20,
        minChars: 3,
        delay: 1000
    };

    if (additionalOptions) {
        options = $.extend(options, additionalOptions);
    }

    $textbox.autocomplete(url, options).result(function (event, item) {
        $nameField.val(item.Tag);
        $codeField.val(item.Code);
    });

    //    $textbox.blur(function () {
    //        if (!$('div.ac_results').is(':visible')) {
    //            if ($textbox.val() == '') {
    //                $nameField.val('');
    //                $codeField.val('');
    //            } else {
    //                $textbox.val($nameField.val());
    //            }
    //        }
    //    });
}

function bindField(sourceField, targetField) {
    var $source = $('#' + sourceField);
    var $target = $('#' + targetField);

    $source.change(function () {
        var $this = $(this);
        var val = $this.val();
        if (val != '' && val != undefined) {
            $target.val($this.find("option:selected").text());
        }
        else {
            $target.val('');
        }

        if ($this.attr("disabled")) {
            if (!$target.attr("disabled")) {
                $target.attr("disabled", "disabled");
            }
        }
        else {
            $target.removeAttr("disabled");
        }
    });
}

function createCascadingDropDown(codeFieldId, nameFieldId, parentFieldId, url, options) {
    var $select = $('#' + codeFieldId);
    var $nameField = $('#' + nameFieldId);
    var $parent = $('#' + parentFieldId);

    $select.CascadingDropDown($parent, url, options);

    bindField(codeFieldId, nameFieldId);
}

function createDropDownListWithAutocomplete(codeFieldId, nameFieldId, codeFieldValue, nameFieldValue, initValues) {

    $(function () {
        
        var $codeField = $('#' + codeFieldId);
        var $nameField = $('#' + nameFieldId);

        $codeField.select2({

            initSelection: function (element, callback) {
                callback({ id: element.val(), text: nameFieldValue });
            },
            
            data: JSON.parse(initValues),

            createSearchChoice: function (term, data) {
                if ($(data).filter(function () {
                    return this.text.localeCompare(term) === 0;
                }).length === 0) {
                    return { id: term, text: term };
                }
            },
            
            // dropdownAutoWidth: true,
            
            allowClear: true

        }).on("change", function (e) {

            if ($(this).select2('data').id === $(this).select2('data').text) {
                $codeField.val('');
            }

            if ($(this).select2('data').id === "_clear") {
                $nameField.val('');
                $codeField.val('');
                $(this).select2('val', '');
            }
            
            if ($(this).select2('data') != null){
                $nameField.val($(this).select2('data').text);
            }
            
        });
    });
}