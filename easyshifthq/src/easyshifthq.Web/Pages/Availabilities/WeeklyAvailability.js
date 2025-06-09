$(function () {
    var l = abp.localization.getResource('easyshifthq');
    
    // Initialize interface
    initializeWeeklyAvailabilityInterface();
    
    // Populate days of the week
    populateDaysOfWeek();
    
    // Initialize time picker fields AFTER DOM elements are created
    initializeTimeFields();
    
    // Load existing availability data
    loadEmployeeWeeklyAvailability();
    
    // Handle save all button
    $('#SaveAllWeeklyAvailability').on('click', function () {
        saveAllWeeklyAvailability();
    });
});

function initializeWeeklyAvailabilityInterface() {
    // Could add more initialization here if needed
}

// Initialize the time picker fields
function initializeTimeFields() {
    console.log('Initializing time fields...');
    
    // Use setTimeout to ensure DOM is fully ready
    setTimeout(function() {
        $('.time-picker').each(function(index, element) {
            console.log('Processing time picker element:', index, element);
            
            var $element = $(element);
            
            // Check if timepicker is already initialized
            if (!$element.hasClass('hasTimepicker') && !$element.data('timepicker-obj')) {
                console.log('Initializing timepicker for element:', index);
                
                // Destroy any existing timepicker first
                try {
                    $element.timepicker('destroy');
                } catch(e) {
                    // Ignore if no timepicker exists
                }
                
                $element.timepicker({
                    timeFormat: 'H:i',  // Correct format for jquery-timepicker
                    interval: 30,
                    minTime: '00:00',
                    maxTime: '23:30',
                    defaultTime: '09:00',
                    startTime: '00:00',
                    dynamic: false,
                    dropdown: true,
                    scrollbar: true,
                    show2400: false,
                    step: 30
                });
            } else {
                console.log('Timepicker already initialized for element:', index);
            }
        });
    }, 200); // Increased timeout
}

// Populate the days of the week rows
function populateDaysOfWeek() {
    var daysOfWeek = [
        { id: 0, name: 'Sunday' },
        { id: 1, name: 'Monday' },
        { id: 2, name: 'Tuesday' },
        { id: 3, name: 'Wednesday' },
        { id: 4, name: 'Thursday' },
        { id: 5, name: 'Friday' },
        { id: 6, name: 'Saturday' }
    ];
    
    var container = $('#weeklyAvailabilityContainer');
    container.empty();
    
    daysOfWeek.forEach(function(day) {
        var rowTemplate = `
            <div class="weekly-availability-row card mb-3" data-day="${day.id}">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-check form-switch">
                                <input class="form-check-input availability-toggle" type="checkbox" id="isAvailable-${day.id}" value="true">
                                <label class="form-check-label" for="isAvailable-${day.id}">${day.name}</label>
                            </div>
                        </div>
                        <div class="col-md-5">
                            <div class="input-group">
                                <span class="input-group-text">Start</span>
                                <input type="text" class="form-control time-picker start-time" id="startTime-${day.id}" disabled>
                            </div>
                        </div>
                        <div class="col-md-5">
                            <div class="input-group">
                                <span class="input-group-text">End</span>
                                <input type="text" class="form-control time-picker end-time" id="endTime-${day.id}" disabled>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        container.append(rowTemplate);
    });
    
    // Add event handlers to availability toggles
    $('.availability-toggle').on('change', function() {
        var dayElement = $(this).closest('.weekly-availability-row');
        var startTimeField = dayElement.find('.start-time');
        var endTimeField = dayElement.find('.end-time');
        
        if ($(this).is(':checked')) {
            startTimeField.prop('disabled', false);
            endTimeField.prop('disabled', false);
        } else {
            startTimeField.prop('disabled', true);
            endTimeField.prop('disabled', true);
        }
    });
}

// Load employee's existing weekly availability
function loadEmployeeWeeklyAvailability() {
    var employeeId = $('#employeeId').val();
    if (!employeeId) {
        return;
    }
    
    easyshifthq.availabilities.availability
        .getEmployeeWeeklyAvailability(employeeId)
        .then(function(result) {
            if (result && result.length > 0) {
                result.forEach(function(availability) {
                    var dayId = availability.dayOfWeek;
                    var rowElement = $(`.weekly-availability-row[data-day="${dayId}"]`);
                    
                    rowElement.attr('data-id', availability.id);
                    
                    // Check the availability toggle
                    var toggle = rowElement.find('.availability-toggle');
                    toggle.prop('checked', availability.isAvailable);
                    
                    // Set start and end times
                    var startTime = rowElement.find('.start-time');
                    var endTime = rowElement.find('.end-time');
                    
                    startTime.val(formatTime(availability.startTime));
                    endTime.val(formatTime(availability.endTime));
                    
                    // Enable/disable time fields based on availability
                    startTime.prop('disabled', !availability.isAvailable);
                    endTime.prop('disabled', !availability.isAvailable);
                });
            }
        })
        .catch(function(error) {
            abp.notify.error('Failed to load availability data');
            console.error(error);
        });
}

// Format time string from backend (e.g. "PT08H30M") to "08:30"
function formatTime(timeString) {
    if (!timeString) return '';
    
    // If already formatted as HH:MM, return as is
    if (timeString.match(/^\d{1,2}:\d{2}$/)) {
        return timeString;
    }
    
    // Extract hours and minutes from ISO 8601 duration format
    const hourMatch = timeString.match(/(\d+)H/);
    const minuteMatch = timeString.match(/(\d+)M/);
    
    const hours = hourMatch ? parseInt(hourMatch[1]).toString().padStart(2, '0') : '00';
    const minutes = minuteMatch ? parseInt(minuteMatch[1]).toString().padStart(2, '0') : '00';
    
    return `${hours}:${minutes}`;
}

// Parse time string from "08:30" format to duration format
function parseTime(timeString) {
    if (!timeString) return null;
    
    var parts = timeString.split(':');
    if (parts.length !== 2) return null;
    
    return `PT${parts[0]}H${parts[1]}M`;
}

// Save all weekly availability entries
function saveAllWeeklyAvailability() {
    var rows = $('.weekly-availability-row');
    var promises = [];
    
    rows.each(function() {
        var row = $(this);
        var isChecked = row.find('.availability-toggle').is(':checked');
        var dayOfWeek = parseInt(row.data('day'));
        var existingId = row.data('id');
        
        // Skip if not available and no existing record
        if (!isChecked && !existingId) {
            return;
        }
        
        var startTimeVal = row.find('.start-time').val();
        var endTimeVal = row.find('.end-time').val();
        
        console.log('Raw time values:', { startTimeVal, endTimeVal });
        console.log('Parsed time values:', { 
            startTime: formatTime(startTimeVal), 
            endTime: formatTime(endTimeVal) 
        });
        
        var data = {
            dayOfWeek: dayOfWeek,
            isAvailable: isChecked,
            startTime: formatTime(startTimeVal),
            endTime: formatTime(endTimeVal)
        };
        
        // If we have an existing record, update it
        if (existingId) {
            promises.push(
                easyshifthq.availabilities.availability
                    .updateWeeklyAvailability(existingId, data)
            );
        }
        // Otherwise create a new record if available
        else if (isChecked) {
            promises.push(
                easyshifthq.availabilities.availability
                    .submitWeeklyAvailability(data)
            );
        }
    });
    
    if (promises.length === 0) {
        abp.notify.info('No changes to save');
        return;
    }
    
    Promise.all(promises)
        .then(function() {
            abp.notify.success('Weekly availability saved successfully');
            loadEmployeeWeeklyAvailability(); // Reload to get IDs for new entries
        })
        .catch(function(error) {
            abp.notify.error('Failed to save availability data');
            console.error(error);
        });
}
