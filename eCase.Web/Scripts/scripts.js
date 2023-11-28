document.addEventListener('DOMContentLoaded', function () {


    //GO TO TOP::
    var viewport = $('html, body');
    //animate to top:
    $('.gototop').click(function () {
        viewport.animate({ scrollTop: 0 }, 1400, 'easeOutQuint');
    });
    //stop animation on scroll
    viewport.bind("scroll mousedown DOMMouseScroll mousewheel keyup", function (e) {
        viewport.stop();
    });

    //Main menu
    $("header button.menu").click(function () {
        if ($(".main-menu").hasClass('mobile-open')) $(this).removeClass('close');
        else $(this).addClass('close');
        $(".main-menu").toggleClass('mobile-open');
    });

    //Select2
    if ($(".select2").length > 0)
        $(".select2").select2();

    $(".lawyers-search").select2({
        placeholder: "",
        minimumInputLength: 1,
        quietMillis: 300,
        allowClear: true,
        containerCssClass: "select2-allowclear",
        initSelection: function (element, callback) {
            var id = $(element).val();
            if (id !== "") {
                return $.ajax({
                    url: "/Case/GetLawyer",
                    type: "POST",
                    dataType: "json",
                    data: {
                        id: id
                    }
                }).done(function (data) {
                    var results;
                    results = [];
                    results.push({
                        id: data.id,
                        text: data.text
                    });
                    callback(results[0]);
                });
            }
        },
        ajax: {
            url: "/Case/GetLawyers",
            dataType: 'json',
            type: "POST",
            quietMillis: 250,
            data: function (term) {
                return {
                    term: term
                };
            },
            results: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return {
                            id: item.id,
                            text: item.text
                        }
                    })
                };
            }
        },
    });

    //Checkbox
    $("label.checkbox").click(function () {
        if ($(this).find("input").is(":checked")) $(this).addClass("checked");
        else $(this).removeClass("checked");
    });

    //Tooltip    
    $('[data-toggle="tooltip"]').tooltip({
        html: true
    });

    //Datepicker
    if ($('.datepicker').length > 0) {
        $('input.datepicker').datepicker({
            format: "dd.mm.yyyy",
            language: "bg",
            autoclose: true,
            todayHighlight: true
        });
    }

    //Autogrow
    $('textarea').css('overflow', 'hidden').autogrow();

    //sections slide Up/Down
    $("h3.section-heading, h4.section-heading").click(function () {
        section = $(this).data('section');
        if ($(this).hasClass("opened")) {
            $("section[data-section=" + section + "]").slideUp(300);
        }
        else {
            $("section[data-section=" + section + "]").slideDown(300);
        }
        $(this).toggleClass("opened");
    });

    // Please wait
    $(".wait").click(function (e) {
        if (!$(this).is("a")
            || (e.which != 2 && !(e.shiftKey || e.altKey || e.metaKey || e.ctrlKey))) {
            show_loader();
        }
    });

    function show_loader() {
        $("div.loader").fadeIn(300);
        $("body").css("overflow", "hidden");
    }

    // Summons modal
    if ($('#messageModal').length > 0) {
        $('#messageModal').modal('show');
    }
    
    //init popover on info icons
    if ($('.info-icon').length != 0) {
        $('.info-icon:not([info-icon])')
            .popover({
                container: 'body',
                placement: function (tip, element) {
                    if ($(element).attr('data-placement') !== undefined) {
                        return $(element).attr('data-placement');
                    } else {
                        var offset = $(element).offset();
                        var height = $(document).outerHeight();
                        var width = $(document).outerWidth();
                        var vert = 0.5 * height - offset.top;
                        var vertPlacement = vert > 0 ? 'top' : 'bottom';
                        var horiz = 0.5 * width - offset.left;
                        var horizPlacement = horiz > 0 ? 'right' : 'left';
                        var placement = Math.abs(horiz) > Math.abs(vert) ? horizPlacement : vertPlacement;
                        return placement;
                    }
                }
            });
    }
});