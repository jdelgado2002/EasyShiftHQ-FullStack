$(function () {
    var l = abp.localization.getResource('easyshifthq');
    var invitationService = easyshifthq.invitations.invitation;

    var dataTable = $('#InvitationsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, "asc"]],
            searching: false,
            ajax: abp.libs.datatables.createAjax(invitationService.getPending),
            columnDefs: [
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
                    render: function (data) {
                        return l('InvitationStatus.' + data);
                    }
                },
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Resend'),
                                    visible: function (data) {
                                        return data.status === 0; // Pending
                                    },
                                    action: function (data) {
                                        invitationService.resend(data.id)
                                            .then(function () {
                                                abp.notify.info(l('InvitationResent'));
                                                dataTable.ajax.reload();
                                            });
                                    }
                                },
                                {
                                    text: l('Revoke'),
                                    visible: function (data) {
                                        return data.status === 0; // Pending
                                    },
                                    confirmMessage: function (data) {
                                        return l('InvitationRevocationConfirmMessage', data.email);
                                    },
                                    action: function (data) {
                                        invitationService.revoke(data.id)
                                            .then(function () {
                                                abp.notify.info(l('InvitationRevoked'));
                                                dataTable.ajax.reload();
                                            });
                                    }
                                }
                            ]
                    }
                }
            ]
        })
    );

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