$(function () {
    const l = abp.localization.getResource('easyshifthq');
    const $employeeSearch = $('#employeeSearch');
    const $employeeCards = $('#employeeCards');
    const $loadingSpinner = $('#loadingSpinner');
    const $noEmployeesMessage = $('#noEmployeesMessage');
    let employees = [];

    // Load employees on page load
    loadEmployees();

    // Search functionality
    $employeeSearch.on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        filterEmployees(searchTerm);
    });

    function loadEmployees() {
        $loadingSpinner.show();
        $employeeCards.empty();
        $noEmployeesMessage.hide();

        // Call ABP Identity API to get users
        abp.ajax({
            url: abp.appPath + 'api/identity/users',
            type: 'GET',
            data: {
                maxResultCount: 1000
            }
        }).done(function (result) {
            employees = result.items || [];
            displayEmployees(employees);
        }).fail(function (error) {
            abp.notify.error(l('LoadingEmployeesFailed'));
            console.error('Error loading employees:', error);
        }).always(function () {
            $loadingSpinner.hide();
        });
    }

    function displayEmployees(employeeList) {
        $employeeCards.empty();
        
        if (employeeList.length === 0) {
            $noEmployeesMessage.show();
            return;
        }

        $noEmployeesMessage.hide();
        employeeList.forEach(function (employee) {
            const cardHtml = createEmployeeCard(employee);
            $employeeCards.append(cardHtml);
        });
    }

    function createEmployeeCard(employee) {
        const displayName = employee.displayName || employee.userName;
        const subtitle = employee.email || '';
        
        return `
            <div class="col-md-6 col-lg-4 mb-3">
                <div class="card employee-card h-100" data-employee-id="${employee.id}" style="cursor: pointer;">
                    <div class="card-body">
                        <h5 class="card-title">${escapeHtml(displayName)}</h5>
                        <p class="card-text text-muted">${escapeHtml(subtitle)}</p>
                        <small class="text-muted">
                            <i class="fas fa-user me-1"></i>${escapeHtml(employee.userName)}
                        </small>
                    </div>
                </div>
            </div>
        `;
    }

    function filterEmployees(searchTerm) {
        const filteredEmployees = employees.filter(function (employee) {
            const searchableText = [
                employee.displayName,
                employee.userName,
                employee.email
            ].join(' ').toLowerCase();
            
            return searchableText.includes(searchTerm);
        });
        
        displayEmployees(filteredEmployees);
    }

    function escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text ? text.replace(/[&<>"']/g, function(m) { return map[m]; }) : '';
    }

    // Handle employee card clicks
    $(document).on('click', '.employee-card', function () {
        const employeeId = $(this).data('employee-id');
        if (employeeId) {
            window.location.href = `/Availabilities/ManagerView/${employeeId}`;
        }
    });

    // Add hover effects
    $(document).on('mouseenter', '.employee-card', function () {
        $(this).addClass('shadow-sm');
    });

    $(document).on('mouseleave', '.employee-card', function () {
        $(this).removeClass('shadow-sm');
    });
});
