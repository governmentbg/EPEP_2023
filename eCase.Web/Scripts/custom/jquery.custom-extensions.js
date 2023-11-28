(function ($) {
    $.formDialog = function (options) {
        var opts = $.extend({}, $.formDialog.settings, options);

        var placeHolder = $('<div/>')[0];

        if (opts.content) {
            $(placeHolder).html(opts.content);
        }

        var createDialogFunction = function () {
            var buttons = {};

            if (!opts.hideOkButton) {
                buttons[opts.okButtonText] = function () {
                    opts.okButtonFunction(placeHolder, opts);
                };
            }
            buttons[opts.cancelButtonText] = function () {
                opts.cancelButtonFunction(placeHolder, opts);
            };

            $(placeHolder).dialog({
                modal: opts.modal,
                width: opts.width,
                title: opts.title,
                close: opts.close,
                buttons: buttons
            });

            // handle ENTER to call OK button or cancel button in case OK button is hidden
            $(placeHolder).keypress(function (e) {
                if (((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) && !$(e.srcElement).is('textarea') && !$(e.target).is('textarea')) {
                    if (!opts.hideOkButton) {
                        opts.okButtonFunction(placeHolder, opts);
                    } else {
                        opts.cancelButtonFunction(placeHolder, opts);
                    }
                    return false;
                } else {
                    return true;
                }
            });
        };

        if (opts.contentUrl) {
            $(placeHolder).load(opts.contentUrl, '', function () {
                opts.dialogCreating(placeHolder);
                createDialogFunction();
                opts.dialogCreated(placeHolder);
            });
        }
        else {
            createDialogFunction();
        }
    };

    $.formDialogSetup = function (settings) {
        $.extend($.formDialog.settings, settings);
    };

    $.formDialog.settings = {
        cancelButtonFunction: cancelButtonFunction,
        cancelButtonText: 'Отказ',
        close: function () {
            $(this).dialog('destroy');
        },
        dialogCreating: function () {
        },
        dialogCreated: function () {
        },
        error: function () {
        },
        hideOkButton: false,
        modal: true,
        okButtonFunction: okButtonFunction,
        serialForm: false,
        succeeding: function () {
        },
        success: function () {
        },
        okButtonText: 'Запис',
        type: 'POST',
        width: 450
    };

    function okButtonFunction(placeHolder, options) {
        var forms = $('form', placeHolder);

        var form;
        if (forms.size() == 1) {
            form = forms[0];
        }
        else if (forms.size() > 1) {
            throw new Error("There must be at most one form in the dialog");
        }

        var ajaxOptions = {
            type: options.type,
            url: options.actionUrl,
            dataType: 'json',
            error: options.error
        };

        if (form) {
            $.extend(ajaxOptions, {
                data: $(form).serializeArray(),
                success: function (formDialogResult) {
                    clearErrorsAndSummary(form);

                    if (formDialogResult.result == 'success') {
                        if (options.succeeding(placeHolder) != false) {
                            options.success();
                            $(placeHolder).dialog('destroy');
                        }
                    }
                    else if (formDialogResult.result == 'error') {
                        showErrors(form, formDialogResult.fieldErrors);
                        showSummary(form, formDialogResult.summaryErrors);
                    }
                    else if (formDialogResult.result == 'redirect') {
                        $(placeHolder).dialog('destroy');
                        $.redirect(formDialogResult.url);
                    }
                    else if (formDialogResult.result == 'loadNew') {
                        var newDialogOptions = $.extend({}, options, formDialogResult.options);

                        if (options.serialForm) {
                            $.extend(newDialogOptions, {
                                succeeding: function (newDialogPlaceHolder) {
                                    $.formDialog($.extend({}, options, {
                                        dialogCreating: function () {
                                            $(newDialogPlaceHolder).dialog('destroy');
                                        }
                                    }));

                                    return false;
                                }
                            });
                        }

                        $.extend(newDialogOptions, {
                            dialogCreating: function () {
                                $(placeHolder).dialog('destroy');
                            }
                        });

                        $.formDialog(newDialogOptions);
                    }
                    else {
                        throw new Error("Unknown form dialog result");
                    }
                }
            });
        }
        else {
            $.extend(ajaxOptions, {
                data: {},
                success: function (result) {
                    options.success();

                    if (result.result == 'redirect') {
                        $.redirect(result.url);
                    }

                    $(placeHolder).dialog('destroy');
                }
            });
        }

        $.ajax(ajaxOptions);
    };

    function cancelButtonFunction(placeHolder, options) {
        $(placeHolder).dialog('close');
    };

    function showErrors(form, errors) {
        var formElements =
            $([])
            .add(form.elements)
            .filter(':input')
            .not(':submit, :reset, :image, [disabled]')
            .each(function () {
                var currentElement = this;

                var error = errors[$(currentElement).attr('name')];

                if (error) {
                    label =
                        $('<label/>')
				        .attr({ 'generatedError': true })
				        .addClass('error')
				        .html(error);

                    label.insertAfter(currentElement);
                }
            });
    };

    function showSummary(form, errors) {
        ul =
            $('<ul/>')
	        .attr({ 'generatedError': true })
	        .addClass('error');

        for (var i = 0; i < errors.length; i++) {
            ul.append('<li>' + errors[i] + '</li>');
        }

        ul.appendTo(form);
    };

    function clearErrorsAndSummary(form) {
        $('[generatedError]', form).replaceWith('');
    };
})(jQuery);

(function ($) {
    $.fn.reload = function (options) {
        return this.each(function () {
            var element = this;

            $.ajax({
                url: $(document).attr('location').href,
                type: "GET",
                dataType: "html",
                complete: function (res, status) {
                    // If successful, inject the HTML into all the matched elements
                    if (status == "success" || status == "notmodified") {
                        var selector = "#" + element.id;

                        var resp = res.responseText; //.replace(/<script(.|\s)*?\/script>/g, "");

                        var bodyOpeningTag = /<body>/i;
                        var bodyClosingTag = /<\/body>/i;
                        var match = bodyOpeningTag.exec(resp);
                        if (match) {
                            resp = resp.substring(match.index + match[0].length);

                            match = bodyClosingTag.exec(resp);
                            resp = resp.substring(0, match.index);
                        }

                        var newContent = $(resp).find(selector)[0].innerHTML;

                        $(element).html(newContent);
                    }
                }
            });
        });
    };
})(jQuery);


(function ($) {

    function disableButton(disableAfterClick, button) {
        if (disableAfterClick) {
            if (!$(button).data("clicked")) {
                $(button).data("clicked", true);

                if ($(button).is("input")) {
                    $(button).attr("value", "Изчакайте...");
                }
                else {
                    $(button).find("img").attr("src", "/Content/images/ajax-loader.gif");
                }

                return true;
            }
            else {
                return false;
            }
        }
        else {
            return true;
        }
    }

    $.submitPage = function (url, method, formId, confirmMessage, confirmTitle, actionIsSendOnly, disableAfterClick, button) {

        if (confirmMessage) {
            if (!confirmTitle) {
                confirmTitle = 'Потвърждение';
            }

            var formDialogOpts = {
                title: confirmTitle,
                content: confirmMessage,
                okButtonText: 'Да',
                cancelButtonText: 'Не'
            };

            var additionalOpts;

            if (actionIsSendOnly) {
                additionalOpts = {
                    okButtonFunction: function (placeHolder) {
                        if (disableButton(disableAfterClick, button)) {
                            $.ajax({
                                type: method,
                                url: url,
                                data: $('#' + formId).serializeArray(),
                                dataType: 'json',
                                success: function (result) {
                                    // TODO must be able to show the errors from FormFialogResult in the confirmation dialog
                                    // Maybe we must refactor the $.formDialog extension to allow this                         
                                    $.reloadPage();
                                    $(placeHolder).dialog('destroy');
                                }
                            });
                        }
                    }
                };
            }
            else {
                additionalOpts = {
                    okButtonFunction: function () {
                        if (disableButton(disableAfterClick, button)) {
                            doSubmit(url, method, formId);
                        }
                    }
                };
            }

            $.formDialog($.extend(formDialogOpts, additionalOpts));
        }
        else {
            if (disableButton(disableAfterClick, button)) {
                doSubmit(url, method, formId);
            }
        }
    };

    function doSubmit(url, method, formId) {
        var form = $("<form style='display:none;' action='" + url + "' method='" + method + "' />");

        //TODO: must copy the events from the original form to support javascript validation
        //form.copyEvents($('#' + formId)); //doesn't work

        var swappedFileInputs = false;

        if (formId) {

            // copy the elements by hand because we must preserve the order
            // but also have to handle some special cases
            $('#' + formId + ' *')
                .filter('input, select, textarea')
                .not('[type=submit], [type=button], [type=image]')
                .each(function () {
                    var newElement = $(this).clone();

                    // clone doesn't copy the selected and checked attributes
                    // of select, input[type=checkbox] and input[type=radio] elements
                    // so we do it by hand
                    if ($(this).is('select') || $(this).is('input[type=text]') || $(this).is('textarea')) {
                        newElement.val($(this).val());
                    } else if ($(this).is('input[type=checkbox], input[type=radio]')) {
                        newElement.attr('checked', $(this).attr('checked'));
                    }

                    newElement.appendTo(form);
                });

            var fileInputs = $('#' + formId + ' input[type=file]');

            if (fileInputs.size() > 0) {
                form.attr('enctype', 'multipart/form-data');                

                // current versions of the browsers work if the code below is always executed!
                //if ($.browser.msie || $.browser.chrome) {
                swappedFileInputs = true;

                //IE requires that the encoding attribute is also set for dynamic forms
                form.attr('encoding', 'multipart/form-data');

                //swap the cloned fileInputs with the original ones since IE doesn't clone the value
                fileInputs.each(function () {
                    var newFile = form.children('[name=' + $(this).attr('name') + ']');

                    $(this).replaceWith(newFile).appendTo(form);
                });
                //}
            }
        }

        if (url.indexOf('?') >= 0) {
            var qs = url.substring(url.indexOf('?') + 1, url.length);

            var queryString = $.parseQueryString(qs);

            for (var qsName in queryString) {
                qsValue = queryString[qsName];

                var input = $("<input type='text' name='" + qsName + "' value='" + qsValue + "' />");

                input.appendTo(form);
            }
        }

        form.appendTo('body').submit();

        // if we swapped the fileInputs swap them back
        if (swappedFileInputs) {
            var fileInputs = $('#' + formId + ' input[type=file]');

            fileInputs.each(function () {
                var newFile = form.children('[name=' + $(this).attr('name') + ']');

                $(this).replaceWith(newFile).appendTo(form);
            });
        }
    }
})(jQuery);

(function ($) {
    $.parseQueryString = function (qs) {
        if (qs == null)
            qs = location.search.substring(1, location.search.length);

        qs = qs.replace(/\+/g, ' ');

        var params = {};
        $.each(qs.split('&'), function (i, p) {
            p = p.split('=');
            p[0] = decodeURIComponent(p[0]);
            p[1] = decodeURIComponent(p[1]);
            params[p[0]] = p[1];
        });

        return params;
    }
})(jQuery);

(function ($) {
    $.reloadPage = function () {
        location.reload();
    };
})(jQuery);

(function ($) {
    $.redirect = function (url) {
        window.location.href = url;
    };
})(jQuery);

(function ($) {
    $.fn.clearInputFields = function (options) {
        return this.each(function () {
            
            function clearInput() {
                switch (this.type) {
                    case 'password':
                    case 'select-multiple':
                    case 'select-one':
                    case 'text':
                    case 'textarea':
                        $(this).val('');
                        break;
                    case 'checkbox':
                    case 'radio':
                        this.checked = false;
                }
            }
            
            $(this).each(clearInput);
            $(this).find(':input').each(clearInput);
        });
    }
})(jQuery);

//(function ($) {
//    $.datepicker.regional['bg'] = {
//        closeText: 'затвори',
//        prevText: '&#x3c;назад',
//        nextText: 'напред&#x3e;',
//        nextBigText: '&#x3e;&#x3e;',
//        currentText: 'днес',
//        monthNames: ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни',
//        'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'],
//        monthNamesShort: ['Яну', 'Фев', 'Мар', 'Апр', 'Май', 'Юни',
//        'Юли', 'Авг', 'Сеп', 'Окт', 'Нов', 'Дек'],
//        dayNames: ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'],
//        dayNamesShort: ['Нед', 'Пон', 'Вто', 'Сря', 'Чет', 'Пет', 'Съб'],
//        dayNamesMin: ['Не', 'По', 'Вт', 'Ср', 'Че', 'Пе', 'Съ'],
//        weekHeader: 'Wk',
//        dateFormat: 'dd.mm.yy',
//        firstDay: 1,
//        isRTL: false,
//        showMonthAfterYear: false,
//        yearSuffix: '',
//        changeMonth: true,
//        changeYear: true,
//        showOtherMonths: true,
//        yearRange: '1900:' + (new Date()).getFullYear()
//    };
//    $.datepicker.regional['en'] = {
//        closeText: 'Done',
//        prevText: 'Prev',
//        nextText: 'Next',
//        currentText: 'Today',
//        monthNames: ['January', 'February', 'March', 'April', 'May', 'June',
//		'July', 'August', 'September', 'October', 'November', 'December'],
//        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
//		'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
//        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
//        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
//        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
//        weekHeader: 'Wk',
//        dateFormat: 'dd.mm.yy',
//        firstDay: 1,
//        isRTL: false,
//        showMonthAfterYear: false,
//        yearSuffix: '',
//        changeMonth: true,
//        changeYear: true,
//        showOtherMonths: true,
//        yearRange: '1900:' + (new Date()).getFullYear()
//    };
    
//    //$.datepicker.setDefaults($.datepicker.regional['en']);
//    //$.datepicker.setDefaults($.datepicker.regional['bg']);
//    $('.ui-datepicker-trigger').attr('align', 'middle');
//})(jQuery);