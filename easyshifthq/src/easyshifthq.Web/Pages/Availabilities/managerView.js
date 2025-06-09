const easyshifthq = window.easyshifthq || {};
(function ($) {
    easyshifthq.availabilities = easyshifthq.availabilities || {};
    const l = abp.localization.getResource('easyshifthq');

    easyshifthq.availabilities.managerView = {
        init: function () {
            const $container = $('#AvailabilityManagerContainer');
            
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
                    const hourMatch = timeValue.match(/(\d+)H/);
                    const minuteMatch = timeValue.match(/(\d+)M/);
                    
                    const hours = hourMatch ? parseInt(hourMatch[1]).toString().padStart(2, '0') : '00';
                    const minutes = minuteMatch ? parseInt(minuteMatch[1]).toString().padStart(2, '0') : '00';
                    
                    $(this).text(hours + ':' + minutes);
                }
            });
        },

        _initializeApproveButtons: function($container) {
            $container.on('click', '.btn-approve', (e) => {
                e.preventDefault();
                
                const $button = $(e.currentTarget);
                const availabilityId = $button.data('id');
                const startDate = $button.data('start-date');
                const endDate = $button.data('end-date');
                
                abp.message.confirm(
                    l('TimeOffApprovalConfirmation', startDate, endDate),
                    (result) => {
                        if (result) {
                            this._approveTimeOffRequest(availabilityId);
                        }
                    }
                );
            });
        },

        _initializeDenyButtons: function($container) {
            $container.on('click', '.btn-deny', (e) => {
                e.preventDefault();
                
                const $button = $(e.currentTarget);
                const availabilityId = $button.data('id');
                const startDate = $button.data('start-date');
                const endDate = $button.data('end-date');
                
                $('#denyRequestModal').modal('show');
                $('#denialForm').data('id', availabilityId);
                $('#denialFormTimeSpan').text(`${startDate} to ${endDate}`);
            });
        },

        _initializeDenialForm: function($container) {
            const self = this;
            $('#denialForm').on('submit', function(e) {
                e.preventDefault();
                
                const availabilityId = $(this).data('id');
                const reason = $('#denialReason').val();
                
                if (!reason) {
                    abp.message.error(l('PleaseProvideDenialReason'));
                    return;
                }
                
                self._denyTimeOffRequest(availabilityId, reason);
            });
        },

        _approveTimeOffRequest: function(availabilityId) {
            easyshifthq.availabilities.availability
                .approveTimeOffRequest(availabilityId)
                .then(() => {
                    abp.notify.success(l('TimeOffRequestApproved'));
                    location.reload();
                });
        },

        _denyTimeOffRequest: function(availabilityId, reason) {
            easyshifthq.availabilities.availability
                .denyTimeOffRequest(availabilityId, reason)
                .then(() => {
                    $('#denyRequestModal').modal('hide');
                    abp.notify.success(l('TimeOffRequestDenied'));
                    location.reload();
                });
        }
    };

    $(function() {
        easyshifthq.availabilities.managerView.init();
    });
})(jQuery);
