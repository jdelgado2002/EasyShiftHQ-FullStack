$(function () {
    var l = abp.localization.getResource('easyshifthq');
    var dataTable = $('#TeamMembersTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, "asc"]],
            searching: false,
            ajax: abp.libs.datatables.createAjax(easyshifthq.invitations.invitation.getPending),
            columnDefs: [
                {
                    title: l('Name'),
                    data: function (row) {
                        return row.firstName + ' ' + row.lastName;
                    }
                },
                {
                    title: l('Email'),
                    data: "email"
                },
                {
                    title: l('Role'),
                    data: "role"
                },
                {
                    title: l('Location'),
                    data: "locationName"
                },
                {
                    title: l('Status'),
                    data: "status",
                    render: function(data) {
                        return `<span class="badge bg-${data === 'Pending' ? 'warning' : data === 'Accepted' ? 'success' : 'danger'}">${l(data)}</span>`;
                    }
                },
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Resend'),
                                visible: function(data) {
                                    return data.status === 'Pending';
                                },
                                action: function (data) {
                                    easyshifthq.invitations.invitation
                                        .resend(data.id)
                                        .then(function () {
                                            abp.notify.success(l('InvitationResent'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            },
                            {
                                text: l('Revoke'),
                                visible: function(data) {
                                    return data.status === 'Pending';
                                },
                                confirmMessage: function (data) {
                                    return l('InvitationRevocationConfirmMessage', data.email);
                                },
                                action: function (data) {
                                    easyshifthq.invitations.invitation
                                        .revoke(data.id)
                                        .then(function () {
                                            abp.notify.success(l('InvitationRevoked'));
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

    var inviteModal = new abp.ModalManager({
        viewUrl: '/Team/InviteModal',
        modalClass: 'InviteTeamMemberModal'
    });

    var bulkInviteModal = new abp.ModalManager({
        viewUrl: '/Team/BulkInviteModal',
        modalClass: 'BulkInviteModal'
    });

    $('#InviteTeamMemberButton').click(function (e) {
        e.preventDefault();
        inviteModal.open();
    });

    $('#BulkInviteButton').click(function (e) {
        e.preventDefault();
        bulkInviteModal.open();
    });

    inviteModal.onResult(function () {
        dataTable.ajax.reload();
    });

    bulkInviteModal.onResult(function () {
        dataTable.ajax.reload();
    });
});