var cabynet = angular.module("cabynet", []);

function getNgScope() {
    return angular.element(document.getElementById('cabynetAngularApp')).scope();
};

function notyConfirmRedirect(newAction) {
    noty({
        text: 'Do you want to continue?',
        layout: 'topRight',
        buttons: [
            {
                addClass: 'btn btn-success btn-clean',
                text: 'Ok',
                onClick: function ($noty) {
                    $noty.close();
                    window.location = newAction;
                }
            },
            {
                addClass: 'btn btn-danger btn-clean',
                text: 'Cancel',
                onClick: function ($noty) {
                    $noty.close();
                }
            }
        ]
    });
};

function notyConfirmApiCall(newAction, message, callback) {
    noty({
        text: message,
        layout: 'topRight',
        buttons: [
            {
                addClass: 'btn btn-success btn-clean',
                text: 'Ok',
                onClick: function ($noty) {
                    window.notyDisplay('', 'Working on it ...', 'warning');
                    $noty.close();
                    $.ajax({
                        type: "GET",
                        url: newAction,
                        async: false
                    })
                        .success(function (data) {
                            if (data.Status === true) {
                                window.notyDisplay('', 'All Done', 'success');
                                callback();
                            } else {
                                window.notyDisplay('', data.Message, 'warning');
                            }
                        })
                        .error(function (data) {
                            window.notyEx();
                        });
                }
            },
            {
                addClass: 'btn btn-danger btn-clean',
                text: 'Cancel',
                onClick: function ($noty) {
                    $noty.close();
                }
            }
        ]
    });
};

function notyDisplay(title, message, type) {
    noty({ text: message, layout: 'topRight', type: type, timeout: 3500 });
};

function notyEx() {
    noty({ text: 'Oops! Something is not right. Please try again or contact Support.', layout: 'topRight', type: 'error', timeout: 3500 });
};

function formatDate(date) {
    return window.moment(date).format("DD-MM-YYYY");
};