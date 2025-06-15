$(function () {
    // Initialize date picker fields
    initializeDateFields();
    
    // Load existing time off requests
    loadExistingTimeOffRequests();
    
    // Handle form submission
    $('#TimeOffRequestForm').on('submit', function (e) {
        e.preventDefault();
        submitTimeOffRequest();
    });
    
    // Delete time off request
    $(document).on('click', '.delete-time-off', function() {
        const id = $(this).data('id');
        deleteTimeOffRequest(id);
    });
});

// Initialize the date picker fields
function initializeDateFields() {
    $('#startDate, #endDate').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true,
        startDate: new Date()
    });
    
    // Set end date to be after start date
    $('#startDate').on('changeDate', function() {
        const startDate = $(this).datepicker('getDate');
        $('#endDate').datepicker('setStartDate', startDate);
        
        // If end date is before start date, update it
        const endDate = $('#endDate').datepicker('getDate');
        if (endDate < startDate) {
            $('#endDate').datepicker('setDate', startDate);
        }
    });
}

// Load existing time off requests
function loadExistingTimeOffRequests() {
    const employeeId = $('#employeeId').val();
    if (!employeeId) {
        return;
    }
    
    easyshifthq.availabilities.availability
        .getEmployeeTimeOffRequests(employeeId)
        .then(function(result) {
            renderTimeOffRequests(result);
        })
        .catch(function(error) {
            abp.notify.error('Failed to load time off requests');
            console.error(error);
        });
}

// Render time off requests in the table
function renderTimeOffRequests(requests) {
    const container = $('#timeOffRequestsContainer');
    container.empty();
    
    if (!requests || requests.length === 0) {
        container.html('<div class="alert alert-info">No time off requests found.</div>');
        return;
    }
    
    let table = `
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Reason</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
    `;
    
    requests.forEach(function(request) {
        table += `
            <tr>
                <td>${formatDate(request.timeOffStartDate)}</td>
                <td>${formatDate(request.timeOffEndDate)}</td>
                <td>${request.reason || 'Not specified'}</td>
                <td>
                    ${getStatusBadge(request.approvalStatus, request.denialReason)}
                </td>
                <td>
                    ${request.approvalStatus === 'Pending' ?
                        `<button class="btn btn-sm btn-danger delete-time-off" data-id="${request.id}">
                            <i class="fa fa-trash"></i> Cancel
                        </button>` : 
                        ''}
                </td>
            </tr>
        `;
    });
    
    table += `
            </tbody>
        </table>
    `;
    
    container.html(table);
}

// Format date from ISO to user-friendly format
function formatDate(dateString) {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    return date.toLocaleDateString();
}

// Submit new time off request
function submitTimeOffRequest() {
    const startDate = $('#startDate').val();
    const endDate = $('#endDate').val();
    const reason = $('#reason').val();
    
    if (!startDate || !endDate) {
        abp.notify.error('Please select start and end dates');
        return;
    }
    
    const data = {
        startDate: startDate,
        endDate: endDate,
        reason: reason
    };
    
    easyshifthq.availabilities.availability
        .submitTimeOffRequest(data)
        .then(function() {
            abp.notify.success('Time off request submitted successfully');
            $('#TimeOffRequestForm')[0].reset();
            loadExistingTimeOffRequests();
        })
        .catch(function(error) {
            abp.notify.error('Failed to submit time off request');
            console.error(error);
        });
}

// Delete time off request
function deleteTimeOffRequest(id) {
    abp.message.confirm(
        'Are you sure you want to cancel this time off request?',
        'Cancel Time Off Request',
        function(isConfirmed) {
            if (isConfirmed) {
                easyshifthq.availabilities.availability
                    .delete(id)
                    .then(function() {
                        abp.notify.success('Time off request cancelled');
                        loadExistingTimeOffRequests();
                    })
                    .catch(function(error) {
                        abp.notify.error('Failed to cancel time off request');
                        console.error(error);
                    });
            }
        }
    );
}

// Get status badge based on approval status
function getStatusBadge(status, denialReason) {
    switch(status) {
        case 'Pending':
            return `<span class="badge bg-info">Pending</span>`;
        case 'Approved':
            return `<span class="badge bg-success">Approved</span>`;
        case 'Denied':
            return `<div>
                <span class="badge bg-danger">Denied</span>
                ${denialReason ? 
                    `<div class="small text-muted mt-1">Reason: ${denialReason}</div>` : 
                    ''}
            </div>`;
        default:
            return `<span class="badge bg-secondary">Unknown</span>`;
    }
}
