$(function () {
    const l = abp.localization.getResource('easyshifthq');
    
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

// Define days array at the top level for both views to use
const DAYS_OF_WEEK = [
    'sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'
];

function initializeWeeklyAvailabilityInterface() {
    // Could add more initialization here if needed
}

// Initialize the time picker fields
function initializeTimeFields() {
    console.log('Initializing time fields...');
    
    // Use setTimeout to ensure DOM is fully ready
    setTimeout(() => {
        $('.time-picker').each((index, element) => {
            console.log('Processing time picker element:', index, element);
            
            const $element = $(element);
            
            // Check if timepicker is already initialized
            if (!$element.hasClass('hasTimepicker') && !$element.data('timepicker-obj')) {
                console.log('Initializing timepicker for element:', index);
                
                // Destroy any existing timepicker first
                try {
                    $element.timepicker('destroy');
                } catch {
                    // Ignore if no timepicker exists
                }
                
                $element.timepicker({
                    timeFormat: 'H:i',  // Correct format for jquery-timepicker
                    interval: 30,
                    minTime: '00:00',
                    maxTime: '23:30',
                    defaultTime: '09:00',
                    dynamic: false,
                    dropdown: true,
                    scrollbar: true,
                    show2400: false,
                    step: 30
                });
            }
        });
    }, 200);
}

// Populate the days of the week rows
function populateDaysOfWeek() {
    const container = $('#weeklyAvailabilityContainer');
    container.empty();
    
    DAYS_OF_WEEK.forEach((day, index) => {
        const dayName = day.charAt(0).toUpperCase() + day.slice(1);
        const rowTemplate = `
            <div class="weekly-availability-row card mb-3" data-day="${index}">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col-md-2">
                            <div class="form-check form-switch">
                                <input class="form-check-input availability-toggle" type="checkbox" id="${day}Toggle">
                                <label class="form-check-label" for="${day}Toggle">${dayName}</label>
                            </div>
                        </div>
                        <div class="col-md-5">
                            <div class="input-group">
                                <span class="input-group-text">Start</span>
                                <input type="text" class="form-control time-picker start-time" id="${day}Start" disabled>
                            </div>
                        </div>
                        <div class="col-md-5">
                            <div class="input-group">
                                <span class="input-group-text">End</span>
                                <input type="text" class="form-control time-picker end-time" id="${day}End" disabled>
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
        const dayElement = $(this).closest('.weekly-availability-row');
        const startTimeField = dayElement.find('.start-time');
        const endTimeField = dayElement.find('.end-time');
        
        startTimeField.prop('disabled', !$(this).is(':checked'));
        endTimeField.prop('disabled', !$(this).is(':checked'));
    });
}

// Load employee's existing weekly availability
async function loadEmployeeWeeklyAvailability() {    
    const l = abp.localization.getResource('easyshifthq');
    
    try {
        // Clear existing state before loading
        $('.weekly-availability-row').removeAttr('data-id');
        $('.availability-toggle').prop('checked', false);
        $('.time-picker').val('').prop('disabled', true);
        
        // Get current user's availability through the secure service endpoint
        const result = await easyshifthq.availabilities.availability
            .getCurrentUserWeeklyAvailability();

        if (result && result.length > 0) {
            result.forEach(function(availability) {
                const dayId = availability.dayOfWeek;
                const rowElement = $(`.weekly-availability-row[data-day="${dayId}"]`);
                
                if (!rowElement.length) {
                    console.warn(`No row element found for day ${dayId}`);
                    return;
                }
                
                try {
                    // Set the ID on the row element
                    rowElement.attr('data-id', availability.id);
                    
                    // Check the availability toggle
                    const toggle = rowElement.find('.availability-toggle');
                    toggle.prop('checked', availability.isAvailable);
                    
                    // Set start and end times
                    const day = DAYS_OF_WEEK[dayId];
                    const startTime = $(`#${day}Start`);
                    const endTime = $(`#${day}End`);
                    
                    if (availability.isAvailable) {
                        startTime.val(formatTime(availability.startTime));
                        endTime.val(formatTime(availability.endTime));
                        startTime.prop('disabled', false);
                        endTime.prop('disabled', false);
                    } else {
                        startTime.val('').prop('disabled', true);
                        endTime.val('').prop('disabled', true);
                    }
                } catch (err) {
                    console.error(`Error updating UI for day ${dayId}:`, err);
                }
            });

            console.log('Availability data loaded and UI updated');
        } else {
            console.log('No availability data found');
        }
    } catch (error) {
        console.error('Error loading availability:', error);
        abp.notify.error(l('ErrorLoadingAvailability'));
        throw error; // Re-throw to handle in calling code if needed
    }
}

// Format time string from backend (HH:mm:ss) to HH:mm
function formatTime(timeString) {
    if (!timeString) return '';
    
    // If in HH:mm:ss format, convert to HH:mm
    if (timeString.match(/^\d{2}:\d{2}:\d{2}$/)) {
        return timeString.substring(0, 5);
    }
    
    // If already in HH:mm format, return as is
    if (timeString.match(/^\d{2}:\d{2}$/)) {
        return timeString;
    }
    
    return '';
}

// Parse time string from HH:mm format to HH:mm:ss for API
function parseTime(timeString) {
    if (!timeString) return null;
    
    // If already in HH:mm:ss format, return as is
    if (timeString.match(/^\d{2}:\d{2}:\d{2}$/)) {
        return timeString;
    }
    
    // If in HH:mm format, add seconds
    if (timeString.match(/^\d{2}:\d{2}$/)) {
        return timeString + ':00';
    }
    
    return null;
}

// Save all weekly availability entries
async function saveAllWeeklyAvailability() {
    const l = abp.localization.getResource('easyshifthq');
    let hasValidationError = false;
    const savePromises = [];
    
    // First do validation pass
    DAYS_OF_WEEK.forEach((day, index) => {
        const isChecked = $(`#${day}Toggle`).is(':checked');
        const startTimeVal = $(`#${day}Start`).val();
        const endTimeVal = $(`#${day}End`).val();
        
        if (isChecked && (!startTimeVal || !endTimeVal)) {
            abp.notify.warn(l('PleaseEnterBothStartAndEndTimes'));
            hasValidationError = true;
            return false;
        }
        
        if (isChecked) {
            const startTime = parseTime(startTimeVal);
            const endTime = parseTime(endTimeVal);
            if (!startTime || !endTime) {
                abp.notify.warn(l('InvalidTimeFormat'));
                hasValidationError = true;
                return false;
            }
        }
    });
    
    if (hasValidationError) {
        return;
    }

    try {
        // Disable save button to prevent double submission
        const saveButton = $('#SaveAllWeeklyAvailability');
        saveButton.prop('disabled', true);
        
        // Process each day
        DAYS_OF_WEEK.forEach((day, index) => {
            const isChecked = $(`#${day}Toggle`).is(':checked');
            const rowElement = $(`.weekly-availability-row[data-day="${index}"]`);
            const existingId = rowElement.attr('data-id');
            
            // Skip if not available and no existing record
            if (!isChecked && !existingId) {
                return;
            }
            
            const startTimeVal = $(`#${day}Start`).val();
            const endTimeVal = $(`#${day}End`).val();
            
            const data = {
                dayOfWeek: index,
                startTime: isChecked ? parseTime(startTimeVal) : null,
                endTime: isChecked ? parseTime(endTimeVal) : null,
                isAvailable: isChecked
            };

            let savePromise;
            if (existingId) {
                savePromise = easyshifthq.availabilities.availability.updateWeeklyAvailability(existingId, data);
            } else if (isChecked) {
                savePromise = easyshifthq.availabilities.availability.submitWeeklyAvailability(data);
            }
            
            if (savePromise) {
                savePromises.push(savePromise);
            }
        });

        // Wait for all saves to complete
        await Promise.all(savePromises);
        
        // Success! Reload the data to get new IDs
        abp.notify.success(l('AvailabilitySavedSuccessfully'));
        await loadEmployeeWeeklyAvailability();
    } catch (error) {
        console.error('Failed to save availability:', error);
        abp.notify.error(l('ErrorSavingAvailability'));
    } finally {
        // Re-enable save button
        $('#SaveAllWeeklyAvailability').prop('disabled', false);
    }
}
