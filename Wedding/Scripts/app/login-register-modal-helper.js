$(document).ready(function () {

 
    var login_register_modal = $('#LoginRegisterModal');
    var loginButton = login_register_modal.find('#modal_login');
    var registerButton = login_register_modal.find('#modal_register');

    var originalLoginHref = loginButton.attr('href');
    var originalRegisterHref = registerButton.attr('href');

    amplify.subscribe("displayLoginModalForProfilePage", function (returnUrl) {
        loginButton.attr('href', originalLoginHref + '?' + returnUrl);
        registerButton.attr('href', originalRegisterHref + '?' + returnUrl);

        login_register_modal.modal('show');
    });

    login_register_modal.on('hidden.bs.modal', function () {
        loginButton.attr('href', originalLoginHref);
        registerButton.attr('href', originalRegisterHref);
    });

    amplify.subscribe("displayLoginRegisterModal", function () {
        login_register_modal.modal('show');
    });

});