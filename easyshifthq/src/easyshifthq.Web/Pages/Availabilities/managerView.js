const easyshifthq = window.easyshifthq || {};
(function ($) {
    easyshifthq.availabilities = easyshifthq.availabilities || {};
    const l = abp.localization.getResource('easyshifthq');

    easyshifthq.availabilities.managerView = {
        init: function () {
            console.log('Initializing manager view...');
            const $container = $('#AvailabilityManagerContainer');
            console.log('Container found:', $container.length > 0);
            
            // Format time display
            this._formatTimeDisplays();
            
            // Initialize button handlers
            this._initializeApproveButtons($container);
            this._initializeDenyButtons($container);
            this._initializeDenialForm($container);
        },

        _formatTimeDisplays: function() {
            $('.time-display').each(function() {
                const timeValue = $(this).data('time');
                if (timeValue) {
                    // Handle HH:mm:ss format from API
                    const match = timeValue.match(/^(\d{2}):(\d{2}):(\d{2})$/);
                    if (match) {
                        const hours = match[1];
                        const minutes = match[2];
                        $(this).text(`${hours}:${minutes}`);
                    } else {
                        console.warn('Invalid time format:', timeValue);
                        $(this).text(timeValue); // Show original value if format is unexpected
                    }
                }
            });
        },

        _initializeApproveButtons: function($container) {
            console.log('Setting up approve button handlers');
            $container.on('click', '.btn-approve', (e) => {
                e.preventDefault();
                console.log('Approve button clicked');
                
                const $button = $(e.currentTarget);
                const availabilityId = $button.data('id');
                const startDate = $button.data('start-date');
                const endDate = $button.data('end-date');
                
                console.log('Approve request data:', { availabilityId, startDate, endDate });
                
                abp.message.confirm(
                    l('TimeOffApprovalConfirmation', startDate, endDate),
                    (result) => {
                        if (result) {
                            console.log('Approval confirmed, calling service...');
                            this._approveTimeOffRequest(availabilityId);
                        }
                    }
                );
            });
        },

        _initializeDenyButtons: function($container) {
            console.log('Setting up deny button handlers');
            $container.on('click', '.btn-deny', (e) => {
                e.preventDefault();
                console.log('Deny button clicked');
                
                const $button = $(e.currentTarget);
                const availabilityId = $button.data('id');
                const startDate = $button.data('start-date');
                const endDate = $button.data('end-date');
                
                console.log('Deny request data:', { availabilityId, startDate, endDate });
                
                $('#denyRequestModal').modal('show');
                $('#denialForm').data('id', availabilityId);
                $('#denialFormTimeSpan').text(`${startDate} to ${endDate}`);
            });
        },

        _initializeDenialForm: function($container) {
            const self = this;
            $('#denialForm').on('submit', function(e) {
                e.preventDefault();
                console.log('Denial form submitted');
                
                const availabilityId = $(this).data('id');
                const reason = $('#denialReason').val();
                
                console.log('Denial form data:', { availabilityId, reason });
                
                if (!reason) {
                    abp.message.error(l('PleaseProvideDenialReason'));
                    return;
                }
                
                console.log('Sending denial request...');
                self._denyTimeOffRequest(availabilityId, reason);
            });
        },

        _approveTimeOffRequest: function(availabilityId) {
            console.log('Calling approveTimeOffRequest with ID:', availabilityId);
            console.log('Availability service exists:', !!easyshifthq.availabilities.availability);
            
            easyshifthq.availabilities.availability
                .approveTimeOffRequest(availabilityId)
                .then(() => {
                    console.log('Approval successful');
                    abp.notify.success(l('TimeOffRequestApproved'));
                    location.reload();
                })
                .catch(error => {
                    console.error('Approval failed:', error);
                    abp.notify.error(l('ErrorOccurred'));
                });
        },

        _denyTimeOffRequest: function(availabilityId, reason) {
            console.log('Calling denyTimeOffRequest with:', { availabilityId, reason });
            console.log('Availability service exists:', !!easyshifthq.availabilities.availability);
            
            easyshifthq.availabilities.availability
                .denyTimeOffRequest(availabilityId, reason)
                .then(() => {
                    console.log('Denial successful');
                    $('#denyRequestModal').modal('hide');
                    abp.notify.success(l('TimeOffRequestDenied'));
                    location.reload();
                })
                .catch(error => {
                    console.error('Denial failed:', error);
                    abp.notify.error(l('ErrorOccurred'));
                });
        }
    };

    $(function() {
        easyshifthq.availabilities.managerView.init();
    });
})(jQuery);
