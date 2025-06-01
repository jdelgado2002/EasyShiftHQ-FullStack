$(function () {
    var l = abp.localization.getResource('easyshifthq');
    var invitationService = easyshifthq.invitations.invitation;

    console.log('Initializing invitations table...');

    var dataTable = $('#InvitationsTable').DataTable({
        processing: true,
        serverSide: false,
        paging: true,
        pageLength: 10,
        order: [[0, "asc"]],
        searching: true,
        ajax: function(data, callback, settings) {
            console.log('Making AJAX request...');
            invitationService.getAll()
                .then(function(result) {
                    console.log('Received data:', result);
                    callback({
                        recordsTotal: result.length,
                        recordsFiltered: result.length,
                        data: result
                    });
                })
                .catch(function(error) {
                    console.error('Error loading invitations:', error);
                    abp.notify.error(l('ErrorLoadingInvitations'));
                });
        },
        columns: [
            {
                title: l('Email'),
                data: "email"
            },
            {
                title: l('FirstName'),
                data: "firstName"
            },
            {
                title: l('LastName'),
                data: "lastName"
            },
            {
                title: l('Role'),
                data: "role"
            },
            {
                title: l('Status'),
                data: "status",
                render: function(data) {
                    var statusMap = {
                        0: 'Pending',
                        1: 'Accepted', 
                        2: 'Revoked',
                        3: 'Expired'
                    };
                    var status = statusMap[data] || 'Unknown';
                    var badgeClass = {
                        'Pending': 'warning',
                        'Accepted': 'success',
                        'Revoked': 'danger',
                        'Expired': 'secondary'
                    }[status];
                    
                    return '<span class="badge bg-' + badgeClass + '">' + 
                           l('InvitationStatus.' + status) + '</span>';
                }
            },
            {
                title: l('Actions'),
                orderable: false,
                render: function(data, type, row) {
                    var actions = '';
                    
                    if (row.status === 0) { // Pending
                        actions += `
                            <button class="btn btn-sm btn-outline-primary mx-1" onclick="resendInvitation('${row.id}')">
                                ${l('Resend')}
                            </button>
                            <button class="btn btn-sm btn-outline-danger mx-1" onclick="revokeInvitation('${row.id}', '${row.email}')">
                                ${l('Revoke')}
                            </button>
                        `;
                    }
                    
                    return actions;
                }
            }
        ]
    });

    // Define action functions in global scope
    window.resendInvitation = function(id) {
        invitationService.resend(id)
            .then(function() {
                abp.notify.success(l('InvitationResent'));
                dataTable.ajax.reload();
            })
            .catch(function(error) {
                console.error('Error resending invitation:', error);
                abp.notify.error(l('ErrorResendingInvitation'));
            });
    };

    window.revokeInvitation = function(id, email) {
        abp.message.confirm(
            l('InvitationRevocationConfirmMessage', email),
            l('AreYouSure'),
            function(isConfirmed) {
                if (isConfirmed) {
                    invitationService.revoke(id)
                        .then(function() {
                            abp.notify.success(l('InvitationRevoked'));
                            dataTable.ajax.reload();
                        })
                        .catch(function(error) {
                            console.error('Error revoking invitation:', error);
                            abp.notify.error(l('ErrorRevokingInvitation'));
                        });
                }
            }
        );
    };

    $('#NewInviteButton').click(function (e) {
        var modal = new abp.ModalManager({
            viewUrl: '/Invitations/CreateModal',
            modalClass: 'CreateInvitation'
        });
        
        modal.open();
        modal.onResult(function () {
            dataTable.ajax.reload();
        });
    });

    $('#BulkInviteButton').click(function (e) {
        var modal = new abp.ModalManager({
            viewUrl: '/Invitations/BulkCreateModal',
            modalClass: 'BulkCreateInvitation'
        });
        
        modal.open();
        modal.onResult(function () {
            dataTable.ajax.reload();
        });
    });
});