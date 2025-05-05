$(function () {
    var l = abp.localization.getResource('easyshifthq');
    var createModal = new abp.ModalManager(abp.appPath + 'Locations/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Locations/EditModal');

    var dataTable = $('#LocationsTable').DataTable({
        processing: true,
        serverSide: false,
        paging: true,
        pageLength: 10,
        order: [[0, "asc"]],
        searching: true,
        ajax: function(data, callback, settings) {
            console.log('Loading locations...');
            easyshifthq.locations.location.getList()
                .then(function(result) {
                    console.log('Received locations:', result);
                    callback({
                        recordsTotal: result.length,
                        recordsFiltered: result.length,
                        data: result
                    });
                })
                .catch(function(error) {
                    console.error('Error loading locations:', error);
                    abp.notify.error(l('ErrorLoadingLocations'));
                });
        },
        columns: [
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
                orderable: false,
                render: function(data, type, row) {
                    var actions = '';
                    
                    if (abp.auth.isGranted('Location.Edit')) {
                        actions += `
                            <button class="btn btn-sm btn-outline-primary mx-1" onclick="editLocation('${row.id}')">
                                ${l('Edit')}
                            </button>
                        `;
                    }
                    
                    if (abp.auth.isGranted('Location.Delete')) {
                        actions += `
                            <button class="btn btn-sm btn-outline-danger mx-1" onclick="deleteLocation('${row.id}', '${row.name}')">
                                ${l('Delete')}
                            </button>
                        `;
                    }
                    
                    if (abp.auth.isGranted('Location.ManageActivity')) {
                        actions += `
                            <button class="btn btn-sm btn-outline-${row.isActive ? 'warning' : 'success'} mx-1" onclick="toggleLocationActive('${row.id}', ${!row.isActive})">
                                ${row.isActive ? l('Deactivate') : l('Activate')}
                            </button>
                        `;
                    }
                    
                    return actions;
                }
            }
        ]
    });

    // Define action functions in global scope
    window.editLocation = function(id) {
        editModal.open({ id: id });
    };

    window.deleteLocation = function(id, name) {
        abp.message.confirm(
            l('LocationDeletionConfirmationMessage', name),
            l('AreYouSure'),
            function(isConfirmed) {
                if (isConfirmed) {
                    easyshifthq.locations.location.delete(id)
                        .then(function() {
                            abp.notify.success(l('SuccessfullyDeleted'));
                            dataTable.ajax.reload();
                        })
                        .catch(function(error) {
                            console.error('Error deleting location:', error);
                            abp.notify.error(l('ErrorDeletingLocation'));
                        });
                }
            }
        );
    };

    window.toggleLocationActive = function(id, isActive) {
        easyshifthq.locations.location.setActive(id, isActive)
            .then(function() {
                abp.notify.success(l('SuccessfullyUpdated'));
                dataTable.ajax.reload();
            })
            .catch(function(error) {
                console.error('Error updating location status:', error);
                abp.notify.error(l('ErrorUpdatingLocation'));
            });
    };

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