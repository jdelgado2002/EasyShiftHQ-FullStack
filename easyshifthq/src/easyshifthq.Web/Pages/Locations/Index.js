$(function () {
    var l = abp.localization.getResource('easyshifthq');
    var createModal = new abp.ModalManager(abp.appPath + 'Locations/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Locations/EditModal');

    var dataTable = $('#LocationsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[0, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(easyshifthq.locations.location.getList),
            columnDefs: [
                {
                    title: l('LocationName'),
                    data: "name"
                },
                {
                    title: l('LocationAddress'),
                    data: "address"
                },
                {
                    title: l('LocationTimeZone'),
                    data: "timeZone"
                },
                {
                    title: l('LocationJurisdictionCode'),
                    data: "jurisdictionCode"
                },
                {
                    title: l('LocationIsActive'),
                    data: "isActive",
                    render: function (data) {
                        return `<span class="badge badge-${data ? 'success' : 'danger'}">${data ? 'Active' : 'Inactive'}</span>`;
                    }
                },
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('Location.Edit'),
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('Location.Delete'),
                                    confirmMessage: function (data) {
                                        return l('LocationDeletionConfirmationMessage', data.record.name);
                                    },
                                    action: function (data) {
                                        easyshifthq.locations.location
                                            .delete(data.record.id)
                                            .then(function () {
                                                abp.notify.info(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            });
                                    }
                                },
                                {
                                    text: function(data) {
                                        return data.record.isActive ? l('Deactivate') : l('Activate');
                                    },
                                    visible: abp.auth.isGranted('Location.ManageActivity'),
                                    action: function (data) {
                                        easyshifthq.locations.location
                                            .setActive(data.record.id, !data.record.isActive)
                                            .then(function () {
                                                abp.notify.info(l('SuccessfullyUpdated'));
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

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewLocationButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});