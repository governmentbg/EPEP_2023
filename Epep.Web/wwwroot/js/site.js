$(function () {
    $(document).on('click', '.form-submit', function () {
        attachAjaxForms(this);
    });
    $(document).on('click', '.gv-loader-tab', function () {
        let tabContent = $(this).data('bs-target');
        let gvContainer = $(tabContent).find('.gridview-container:first');
        if (gvContainer.length > 0) {
            gridViewDifferLoad(`#${gvContainer[0].id}`);
        }
    });
    initControls();
    attachOffcanvasDismis();
});

function attachCertificateCheck(intervalInSeconds) {
    setInterval(CheckCertificate, intervalInSeconds * 1000);
}

function initControls() {
    $('.select2').each(function (i, e) {
        if ($(e).parents('.offcanvas-body').length > 0) {
            let select2parent = $(e).parents('.offcanvas-body')
            $(e).select2({
                dropdownParent: select2parent,
                theme: "bootstrap-5",
                width: '100%'
            });
        } else {
            $(e).select2({
                //dropdownParent: $(e).parent('.offcanvas:first')[0],
                theme: "bootstrap-5",
                width: '100%'
            });
        }
    });


    initDatePicker();
}

function initDatePicker() {
    tempusDominus.extend(tempusDominus.plugins.customDateFormat);
    $(".date-picker").each(function (i, element) {
        if ($(element).data('loaded')) {
            return;
        }
        let selectedValue = undefined;
        if ($(element).data("val").length > 0) {
            selectedValue = $(element).data("val");
        }
        // element.value = selectedValue ? new Date(selectedValue.split(".").reverse().join("-")).toISOString() : "";
        const _picker = new tempusDominus.TempusDominus(element, {
            defaultDate: selectedValue,
            display: {
                viewMode: "calendar",
                components: {
                    decades: false,
                    year: true,
                    month: true,
                    date: true,
                    hours: false,
                    minutes: false,
                    seconds: false
                }
            },
            localization: {
                locale: 'bg',
                today: "Днес",
                clear: "Изчистете",
                close: "Затворете",
                selectMonth: "Изберете месец",
                previousMonth: "Предходен месец",
                nextMonth: "Следващ месец",
                selectYear: "Изберете година",
                previousYear: "Предходна година",
                nextYear: "Следваща година",
                selectDecade: "Изберете десетилетие",
                previousDecade: "Предходно десетилетие",
                nextDecade: "Следващо десетилетие",
                previousCentury: "Предходен век",
                nextCentury: "Следващ век",
                pickHour: "Изберете час",
                incrementHour: "Увеличете времето",
                decrementHour: "Намалете времето",
                pickMinute: "Изберете минута",
                incrementMinute: "Увеличете минута",
                decrementMinute: "Намалете минута",
                pickSecond: "Изберете второ",
                incrementSecond: "Увеличете секунди",
                decrementSecond: "Намалете секунди",
                toggleMeridiem: "Превключете период",
                selectTime: "Изберете време",
                selectDate: "Изберете дата",
                dayViewHeaderFormat: { month: "long", year: "2-digit" },
                format: "dd.MM.yyyy",
                startOfTheWeek: 1
            }
        });

        // _picker.dates.formatInput = function (date) {
        //     if (!date) {
        //         return "";
        //     }
        //     return moment(date).format("DD.MM.YYYY");
        // };

        // //debugger;
        // if (selectedValue) {
        //     const selectedDate = moment(selectedValue, "DD.MM.YYYY");
        //     // const parsedDate = _picker.dates.parseInput(selectedValue);

        //     const parsedDate = _picker.dates.parseInput(selectedValue.split(".").reverse().join("-"));
        //     _picker.dates.setValue(parsedDate, _picker.dates.lastPickedIndex);
        //     console.log(_picker.dates.lastPickedIndex);
        //     // _picker.dates.setValue(parsedDate, _picker.dates.lastPickedIndex);
        //     // _picker.dates.setFromInput(parsedDate, _picker.dates.lastPickedIndex);
        // }
    });
}


function attachOffcanvasDismis() {
    $(document).on('click', '.offcanvas-body button.oc-clear', function () {
        let cvsBody = $(this).parents('.offcanvas-body:first');
        $(cvsBody).find('select').each(function (i, e) {
            let nullVal = $(e).data('nullval');
            if (nullVal) {
                $(e).find('option:selected').removeAttr('selected');
                $(e).find($`option[value="{nullVal}"]`).attr('selected', 'selected');
                $(e).val(nullVal);
            } else {
                $(e).find('option:selected').removeAttr('selected');
                $(e).val('-1');
            }
        });
        $(cvsBody).find('.select2').val('-1').trigger('change');
        $(cvsBody).find('input[type="checkbox"]').each(function (i, e) {
            $(e).prop('checked', false);
        });
        $(cvsBody).find('input[type="text"]').each(function (i, e) {
            $(e).val($(e).data('nullval'));
        });
        $(cvsBody).find('button.oc-submit').trigger('click');
    });
}

function attachAjaxForms(sender) {
    let form = $(sender).parents('.ajax-form:first');
    let postUrl = $(form).data('url');
    let beforeSubmit = $(sender).data('beforesubmit');
    let callback = $(sender).data('callback');
    try {
        if (beforeSubmit) {
            window[beforeSubmit]();
        }
    } catch (e) { }
    let model = $(form).find('.form-content').serializeArray();
    let data = new FormData();
    for (var i = 0; i < model.length; i++) {
        data.append(model[i].name, model[i].value);
    }
    $('span.field-validation-valid,span.field-validation-invalid').text('');
    $('.ajax-form-validation').text('');

    fetch(postUrl,
        {
            method: 'POST',
            body: data
        }
    )
        .then((response) => response.json())
        .then((data) => {
            if (!data.result) {
                if (data.errors && data.errors.length > 0) {
                    for (var i = 0; i < data.errors.length; i++) {
                        let err = data.errors[i];
                        if (err.control && (err.control != null)) {
                            $(`span.field-validation-valid[data-valmsg-for="${err.control}"`).text(err.error);
                        } else {
                            $('.ajax-form-validation').text(err.error);
                        }
                    }
                } else {
                    if (data.message) {
                        //messageHelper.ShowErrorMessage(data.message);
                        $('.ajax-form-validation').text(data.message);
                    }
                }
            } else {
                if (callback) {
                    window[callback](data);
                }
                if (data.message) {
                    messageHelper.ShowSuccessMessage(data.message);
                }
                return false;

            }
        }
        ).catch((error) => {
            console.log(error);
        });
}

var messageHelper = (function () {
    function showMessage(state, message) {
        setTimeout(function () {
            $('#divToast')
                .removeClass('success')
                .removeClass('error')
                .addClass(state)
                .find('.toast-body')
                .text(message);
            $('#divToast').toast("show")
        }, 250);
    }

    function ShowSuccessMessage(message) {
        showMessage('success', message);
    }
    function ShowErrorMessage(message) {
        showMessage('error', message);
    }

    return {
        ShowSuccessMessage,
        ShowErrorMessage
    }
})();


function requestDataForTemplate(container, template, url, data) {
    requestContent(url, data, function (model) {
        templateFromModel(container, template, model);
    });
}

function templateFromModel(container, template, model) {
    let tmpl = $(template).html();
    var hbars = Handlebars.compile(tmpl);
    $(container).html(hbars(model));

}

function fillCombo(items, combo, selected) {
    let tmpl = '{{#each this}}<option value="{{value}}" {{#if selected}}selected="selected"{{/if}}>{{text}}</option>{{/each}}';
    let hbars = Handlebars.compile(tmpl);
    $(combo).html(hbars(setSetSelected(items, selected)));
}
function requestCombo(url, data, combo, selected, callback) {
    requestContent(url, data, function (items) {
        fillCombo(items, combo, selected);
        if (callback) {
            callback(combo);
        }
    });
}
function setSetSelected(items, selected) {
    if (items && (selected !== undefined)) {
        for (var i = 0; i < items.length; i++) {
            if (items[i].value == selected) {
                items[i].selected = true;
            }
        }
    }

    return items;
}

function requestContent(url, data, callback) {
    $.ajax({
        type: 'GET',
        //async: true,
        cache: false,
        url: url,
        data: data,
        success: function (data) {
            callback(data);
        },
        error: function (error) {
            messageHelper.ShowErrorMessage('Проблем при зареждане на съдържание!');
            console.log(error);
        }
    });
}

function postJson(url, data, callback) {
    $.ajax({
        type: 'POST',
        //async: true,
        cache: false,
        content: 'json',
        url: url,
        data: data,
        success: function (data) {
            callback(data);
        },
        error: function (error) {
            messageHelper.ShowErrorMessage('Проблем при подаване на съдържание!');
            console.log(error);
        }
    });
}

function showDetails(type, gid, loadCallback) {
    switch (type) {
        case 2:
            redirect_blank(rootDir + `case/casedetail/${gid}`);
            break;
        default:
            requestOffcanvas(rootDir + 'case/preview', { type: type, gid: gid }, null,false, loadCallback);
            break;
    }


}

function requestOffcanvasSmall(url, data, loadCallback) {
    requestOffcanvas(url, data, null, true, loadCallback);
}

function requestOffcanvas(url, data, canvasContainer, small, loadCallback) {
    canvasContainer = canvasContainer ?? "offcanvasMain";
    $.fn.modal.Constructor.prototype.enforceFocus = function () { };
    small = small ?? false;
    $.ajax({
        type: 'GET',
        async: true,
        cache: false,
        url: url,
        data: data,
        success: function (data) {
            if (data === "null") {
                return false;
            }
            let _offset = document.getElementById(canvasContainer);
            if (_offset.classList.contains('off-small')) {
                _offset.classList.remove('off-small');
            }
            if (small) {
                _offset.classList.add('off-small');
            }
            _offset.innerHTML = data;
            var bsOffcanvas = new bootstrap.Offcanvas(_offset);
            bsOffcanvas.show();
            setTimeout(function () {
                initControls(true);
                if (loadCallback) {
                    loadCallback();
                }
            },250);
            $('.modal-backdrop.fade').each(function (i, e) {
                //Премахва допълните modal-backdrop, които се зареждат за всяко отваряне на offcanvas-а
                if (i > 0) {
                    $(e).remove();
                }
            });
            setTimeout(function () {
                $('a.preview-pdf:first').trigger('click');
            }, 500);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function hideOffcanvas(canvasContainer) {
    try {
        canvasContainer = canvasContainer ?? "offcanvasMain";
        let _offset = document.getElementById(canvasContainer);
        var bsOffcanvas = bootstrap.Offcanvas.getInstance(_offset);
        _offset.innerHTML = '';
        bsOffcanvas.hide();
        setTimeout(function () {
            if (bsOffcanvas) {
                bsOffcanvas.dispose()
            }
        }, 500);
    } catch (e) {
        console.log(e);
    }
}

function previewPdf(url, containerId, proch) {
    let container = document.getElementById(containerId);
    let _objElement = document.getElementById('objPreviewPdf');
    if (container && _objElement) {
        if (_objElement.data.includes(url)) {
            console.log('same file');
            return false;
        }
        container.removeChild(_objElement);
    }
    let _pdfHeight = `${proch}%`;

    let obj = document.createElement('object');
    obj.id = 'objPreviewPdf';
    obj.setAttribute('type', 'application/pdf');
    obj.setAttribute('width', '100%');
    obj.setAttribute('data', url);
    obj.style.height = _pdfHeight;

    let embed = document.createElement('embed');
    embed.setAttribute('type', 'application/pdf');
    embed.setAttribute('width', '100%');
    embed.setAttribute('src', url);
    embed.style.height = _pdfHeight;

    obj.appendChild(embed);

    //container.innerHTML = '';
    container.appendChild(obj);
}

function redirect_blank(url) {
    var a = document.createElement('a');
    a.target = "_blank";
    a.href = url;
    a.click();
}

function displayXhrSaveResult(xhr, successCallback) {
    let resObj = xhr.responseJSON;
    if (!resObj) {
        resObj = JSON.parse(xhr.responseText);
    }


    if (resObj.result == true) {
        messageHelper.ShowSuccessMessage(resObj.message);
        successCallback();
    } else {
        messageHelper.ShowErrorMessage(resObj.message);
    }
}

function displaySaveResult(result, successCallback) {
    if (result.result == true) {
        messageHelper.ShowSuccessMessage(result.message);
        successCallback();
    } else {
        messageHelper.ShowErrorMessage(result.message);
    }
}

function CheckCertificate() {
    if (certNo && certNo !== "" && certCheckPath !== "") {
        $.ajax({
            url: certCheckPath,
            type: "GET",
            xhrFields: {
                withCredentials: true
            },
            crossDomain: true,
            success: function (data) {
                var currentCertNo = data["certno"];
                if (!currentCertNo || certNo.replace(/^0+/, '').toUpperCase() !== currentCertNo.replace(/^0+/, '').toUpperCase()) {
                    window.location.href = signOutUrl;
                }
            }
        });
    }
}