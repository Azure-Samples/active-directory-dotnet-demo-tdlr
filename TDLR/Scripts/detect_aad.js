$aadButton = $("#aad-button");
$emailField = $("#email");

$emailField.change(function () {
    if (isEmail($(this).val())) {
        isAadTenant(getDomainFromEmail($(this).val()));
    }
    else {
        hideWorkAccountButton();
    }
});

function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

function getDomainFromEmail(email) {
    return email.replace(/.*@/, "").toLowerCase();
}

function showWorkAccountButton() {
    console.log('Showing AAD work account button...');
    $aadButton.slideDown("slow");
}

function hideWorkAccountButton() {
    console.log('Hiding AAD work account button...');
    $aadButton.slideUp("slow");
}

function isAadTenant(domain) {

    return $.ajax({
        url: '/proxy/discovery?domain=' + domain,
        type: "GET",
        statusCode: {
            200: showWorkAccountButton,
            404: hideWorkAccountButton
        }
    });
}

$aadButton.click(function () {
    window.location.href = "/Account/SignUp/AAD?redirectUri=/home/signup&sign_up_hint=" + $emailField.val();
});