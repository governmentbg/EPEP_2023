/*
* CascadingDropDown : a jQuery plugin, version: 0.2 (2010-05-21)
* @requires jQuery v1.4 or later
*
* CascadingDropDown is a jQuery plugin that can be attached to a select list to get automatic population. 
* Each time the source element changes value, an ajax request is made to retrieve a list of values for 
* the select list. The respose from the ajax request should be in json with the following format
*
*    [{
*        "Text": "John",
*        "Value": "10326"
*    },
*    {
*        "Text": "Jane",
*        "Value": "10801"
*    }] 
*
*
* Licensed under the MIT:
* http://www.opensource.org/licenses/mit-license.php
*
* Raj Kaimal http://weblogs.asp.net/rajbk/
*
* v0.2 Added support for custom postData using functions
*
*
*/

(function ($) {
    $.fn.CascadingDropDown = function (source, actionPath, settings) {

        if (typeof source === 'undefined') {
            throw "A source element is required";
        }

        if (typeof actionPath == 'undefined') {
            throw "An action path is requried";
        }

        var optionTag = '<option></option>';
        var config = $.extend({}, $.fn.CascadingDropDown.defaults, settings);

        return this.each(function () {
            var $this = $(this);

            (function () {
                var methods = {
                    clearItems: function () {
                        $this.empty();
                        if (!$this.attr("disabled")) {
                            $this.attr("disabled", "disabled");
                            $this.addClass(config.disabledClass);
                        }
                    },
                    reset: function () {
                        methods.clearItems();
                        $this.trigger('change');
                    },
                    initialize: function () {
                        if ($this.children().size() == 0) {
                            if (!$this.attr("disabled")) {
                                $this.attr("disabled", "disabled");
                                $this.addClass(config.disabledClass);
                            }
                        }
                    },
                    showLoading: function () {
                        methods.clearItems();
                        $this.append($(optionTag)
                            .attr("value", "")
                            .text(config.loadingText));
                    },
                    loaded: function () {
                        if ($this.children().size() != 0) {
                            $this.removeAttr("disabled");
                            $this.removeClass(config.disabledClass);
                        }
                        $this.trigger('change');
                    },
                    showError: function () {
                        methods.clearItems();
                        $this.append($(optionTag)
                            .attr("value", "")
                            .text(config.errorText));
                    },
                    post: function () {
                        methods.showLoading();
                        $.isFunction(config.onLoading) && config.onLoading.call($this);
                        $.ajax({
                            url: actionPath,
                            type: 'POST',
                            dataType: 'json',
                            data: ((typeof config.postData == "function") ? config.postData.apply($(source)) : config.postData) || (config.parameterName != null ? config.parameterName + "=" + $(source).val() : $(source).serialize()),
                            success: function (data) {
                                methods.reset();
                                $.each(data, function () {
                                    $this.append($(optionTag)
                                        .attr("value", this.Value)
                                        .text(this.Text));
                                });
                                methods.loaded();
                                $.isFunction(config.onLoaded) && config.onLoaded.call($this);
                            },
                            error: function () {
                                methods.showError();
                            }
                        });
                    }
                };

                $(source).change(function () {
                    var val = $(source).val();
                    if (val != '' && val != undefined) {
                        methods.post();
                    }
                    else {
                        methods.reset();
                    }
                });

                methods.initialize();

            })();
        });
    }

    $.fn.CascadingDropDown.defaults = {
        loadingText: 'Loading ..',
        errorText: 'Error loading data.',
        postData: null,
        onLoading: null,
        onLoaded: null,
        parameterName: 'parentId',
        disabledClass: 'disabled'
    }
})(jQuery);