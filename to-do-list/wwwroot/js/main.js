$(document).ready(function () {
    $("#projectHeader").click(function () {
        if ($("#projectHeader .dropdown-list").css("display") == "block") {
            $("#projectHeader .dropdown-list").css("display", "none");
        } else {
            $("#projectHeader .dropdown-list").css("display", "block");
        }
    });

    if ($(window).innerWidth() < 768) {
        $("#bodyUnderBreadcrumb").removeClass("open-menu");
        $("#bodyUnderBreadcrumb").addClass("closed-menu");
    }

    $(window).resize(function () {
        if (!$("#closeNavButton").hasClass("clicked")) {
            if ($(window).innerWidth() < 768) {
                closeNav();
            } else if ($(window).innerWidth() > 768) {
                openNav();
            }
        }
    });

    $("#closeNavButton").click(function () {
        $(this).removeClass("clicked");
        $(this).addClass("clicked");

        $($(".content")[0]).show();
        closeNav();
    });

    $("#openNavButton").click(function () {
        if ($("#closeNavButton").hasClass("clicked")) {
            $("#closeNavButton").removeClass("clicked");
        }
        if ($(window).innerWidth() < 500) {
            $($(".content")[0]).hide();
        }
        openNav();
    });
});

function closeNav() {
    $(".menu-main").css("display", "none");

    $(".bodyUnderBreadcrumb").removeClass("open-menu");
    $(".bodyUnderBreadcrumb").addClass("closed-menu");
}

function openNav() {
    $(".bodyUnderBreadcrumb").removeClass("closed-menu");
    $(".bodyUnderBreadcrumb").addClass("open-menu");
}

function confirmation() {
    return confirm("Are you sure you want to complete this task?");
}

function formatDateddMMyyyy(date) {
    date = new Date(date);
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year;
}

function saveChanges(formId) {
    toastr.options.positionClass = 'toast-bottom-right'
    form = document.getElementById(formId);
    toastr.remove();
    toastr.info("Saving...");

    return $.ajax({
        url: $(form).attr("action"),
        type: $(form).attr("method"),
        data: $(form).serialize(),
        success: (result) => {
            if (result.success == true) {
                form.classList.remove("dirty");
                toastr.remove();
                toastr.success('Saved Successfully');
                return result.data;
            } else {
                toastr.remove();
                toastr.error(result.errors);
            }
        },
        error: (result) => {
            toastr.error(result.responseText);
        }
    });
}

function filter(input, itemName) {
    var filter = input.value.toUpperCase();
    var items = document.getElementsByName(itemName)

    for (var i = 0; i < items.length; i++) {
        txtValue = items[i].textContent || items[i].innerText;

        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            items[i].parentNode.style.display = "";
        } else {
            items[i].parentNode.style.display = "none";
        }
    }
}