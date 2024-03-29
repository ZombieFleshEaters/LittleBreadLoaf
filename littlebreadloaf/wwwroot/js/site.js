﻿
$(document).ready(function () {
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
});

$(document).on('click', '[data-toggle="lightbox"]', function (event) {
    event.preventDefault();
    $(this).ekkoLightbox();
});

$.blockUI.defaults.message = '<h3>Please wait...</h3>';
$(document).ajaxStart($.blockUI)
    .ajaxStop($.unblockUI);

$(document).ready(function () {
    $("#delivery_address").autocomplete(
    {
        search: function (e, ui) {
            $(this).data("ui-autocomplete").menu.bindings = $();
        },
        source: function (request, response) {
            $.ajax(
                {
                    url: "/api/address-auto-fill",
                    dataType: "json",
                    data:
                    {
                        addressFilter: request.term
                    },
                    success: function (data) {
                        if (!data.length) {
                            var result = [
                                {
                                    label: 'No matches found',
                                    value: response.term
                                }
                            ];
                            response(result);
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.full_address,
                                    value: item.address_id
                                };
                            }));
                        }
                    }
                });
        },
        minLength: 4,
        select: function (event, ui) {
            $("#delivery_address").val(ui.item.label);
            $("#address_id").val(ui.item.value);
            return false;
        }
    });
});

$(document).ready(function () {

    if ($('#delivery_date_preorder').length > 0) {
        var dates;
        var source = $("#pre_order_source").val();
        $.ajax({
            type: "GET",
            url: "/api/dates?type=" + source,
            success: function (data) {
                dates = data.filter(element => element.type == source)[0].dates;
            }
        });

        $("#delivery_date_preorder").datepicker({
            minDate: "1d",
            maxDate: "10d",
            dateFormat: "yy-mm-dd",
            beforeShowDay: function (date) {
                if (dates.includes(getDayOfWeek(date))) {
                    return [true, ""];
                } else {
                    return [false, ""];
                }
            }
        });
    }
   
    if ($('#delivery_date').length > 0) {
        var deliveryDates;
        $.ajax({
            type: "GET",
            url: "/api/dates",
            success: function (data) {
                deliveryDates = data.filter(element => element.type == "DELIVERY")[0].dates;
            }
        });

        $("#delivery_date").datepicker({
            minDate: "1d",
            maxDate: "10d",
            dateFormat: "yy-mm-dd",
            beforeShowDay: function (date) {
                if (deliveryDates.includes(getDayOfWeek(date))) {
                    return [true, ""];
                } else {
                    return [false, ""];
                }
            }
        });
    }
    
    if ($('#pickup_date').length > 0) {
        var pickupDates;
        $.ajax({
            type: "GET",
            url: "/api/dates",
            success: function (data) {
                pickupDates = data.filter(element => element.type == "PICKUP")[0].dates;
            }
        });

        $("#pickup_date").datepicker({
            minDate: "1d",
            maxDate: "10d",
            dateFormat: "yy-mm-dd",
            beforeShowDay: function (date) {
                if (pickupDates.includes(getDayOfWeek(date))) {
                    return [true, ""];
                } else {
                    return [false, ""];
                }
            }
        });
    }
});

function getDayOfWeek(date) {
    var dayOfWeek = new Date(date).getDay();
    return isNaN(dayOfWeek) ? null : ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][dayOfWeek];
}

$(document).ready(function () {
    $("#delivery_date_from").datepicker({
        minDate: "-1w",
        dateFormat: "yy-mm-dd"
    });
});

$(document).ready(function () {
    $("#delivery_date_to").datepicker({
        dateFormat: "yy-mm-dd"
    });
});

$(document).ready(function () {
    $("#ProductOrderOutage_Start").datepicker({
        dateFormat: "yy-mm-dd"
    });
});

$(document).ready(function () {
    $("#ProductOrderOutage_Stop").datepicker({
        dateFormat: "yy-mm-dd"
    });
});

$(document).ready(function () {
    $("#ErrorList_From").datepicker({
        dateFormat: "yy-mm-dd"
    });
});

$(document).ready(function () {
    $("#ErrorList_To").datepicker({
        dateFormat: "yy-mm-dd"
    });
});

$(document).ready(function () {

    $('#delivery_time').timepicker({
        'timeFormat': 'H:i a',
        'minTime': '1:00pm',
        'maxTime': '4:00pm'
    });

    $('#pickup_time').timepicker({
        'timeFormat': 'H:i a',
        'minTime': '11:00am',
        'maxTime': '1:30pm'
    });
});

$(document).ready(function () {
    $("form[name='add_cart']").submit(function (e) {

        var form = $(this);
        var url = form.attr('action');

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(), // serializes the form's elements.
            success: function (data) {
                $("#cartItemAdded").html('<i class="fa fa-plus fa-fw"></i>' + data.productName + " has been added to the cart!");
                $("#cartItemAdded").removeClass('d-none');
                $("#cartItemCount").html(data.cartCount);
            },
            error: function (data) {
                $("#cartItemAdded").html('<i class="fa fa-plus fa-fw"></i>' + $("#productName").val() + " failed!");
                $("#cartItemAdded").removeClass('alert alert-success');
                $("#cartItemAdded").removeClass('d-none');
                $("#cartItemAdded").addClass('alert alert-danger');
            }
        });

        e.preventDefault(); // avoid execute of the actual submit of the form.
    });
});

$(document).ready(function () {
    $(".block-form-submit").submit(function (e) {
        $.blockUI();
        //$.blockUI({
        //    message: 'Wait',
        //    css: { width: '4%', border: '0px solid #FFFFFF', cursor: 'wait', backgroundColor: '#FFFFFF' },
        //    overlayCSS: { backgroundColor: '#FFFFFF', opacity: 0.0, cursor: 'wait' }
        //}); 
        $(".block-form-submit-button").prop('disabled', true);
    });
});

$(document).ready(function () {
    var bLazy = new Blazy({
        offset: 10 //Load images at 10 pixels from thew view port
    });
});

var toolbarOptions = [
                ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
                ['blockquote', 'code-block'],

                [{ 'header': 1 }, { 'header': 2 }],               // custom button values
                [{ 'list': 'ordered'}, { 'list': 'bullet' }],
                [{ 'script': 'sub'}, { 'script': 'super' }],      // superscript/subscript
                [{ 'indent': '-1'}, { 'indent': '+1' }],          // outdent/indent
                [{ 'direction': 'rtl' }],                         // text direction

                [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
                [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                [ 'link', 'image', 'video', 'formula' ],          // add's image support
                [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
                [{ 'font': [] }],
                [{ 'align': [] }],

                ['clean']                                         // remove formatting button
];
var quill;

$(document).ready(function () {
    if ($('#quilEditor').length > 0)
    {
        quill = new Quill('#quilEditor', {
            modules: {
                toolbar: toolbarOptions
            },

            theme: 'snow'
        });
    }
});

$(document).ready(function () {
    $("form[name='blog_add']").submit(function (e) {

        var form = $(this);
        var url = form.attr('action');
        var html = quill.root.innerHTML;
        if (html !== "<p><br></p>") //Quil will place a BR in a paragraph element when content is "blank"
        {
            $("#blog_content_hidden").val(html);
        }
       
        //e.submit;
    });
});

$(document).ready(function () {
    if ($('#tag_control').length > 0) {
        $('#tag_control').select2({
            tags: true
        });
    }
});

/*$(document).ready(function () {
    if ($('#cart_checkout').length > 0)
    {
        grecaptcha.ready(function () {
            grecaptcha.execute(googleRecaptchaSiteKey, { action: 'CartCheckout' }).then(function (token) {
                $("#review_order").prop('disabled', false);
                $("#g-recaptcha-response").val(token);
                $("#alertReCaptca").html('<i class="fa fa-check" aria-hidden="true" title="You are human"></i> You are human :)');
                $("#alertReCaptca").switchClass("alert-info", "alert-success", 300, "easeInOutQuad");
            });
        });
    }
});*/

