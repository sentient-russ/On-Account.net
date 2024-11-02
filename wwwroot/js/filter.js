document.addEventListener('DOMContentLoaded', function () {
    const dropdownItems = document.querySelectorAll('.dropdown-menu .dropdown-item');
    const tableRows = document.querySelectorAll('table tbody tr');
    const startDateInput = document.getElementById('startDateDropdownForm');
    const endDateInput = document.getElementById('endDateDropdownForm');
    dropdownItems.forEach(item => {
        item.addEventListener('click', function (event) {
            event.preventDefault();
            const selectedStatus = this.innerText.trim();
            filterTableRowsByStatus(selectedStatus);
        });
    });
    function filterTableRowsByStatus(status) {
        tableRows.forEach(row => {
            const hiddenText = row.querySelector('td.text-center[hidden]').innerText.trim();
            if (status === 'Show All') {
                row.style.display = '';
            } else if (hiddenText.includes(status)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }
});
document.addEventListener('DOMContentLoaded', function () {
    const getDateRangeButton = document.getElementById('getDateRangeButton');
    const startDateInput = document.getElementById('startDateDropdownForm');
    const endDateInput = document.getElementById('endDateDropdownForm');
    var rows = document.querySelectorAll('#tableBody tr');
    getDateRangeButton.addEventListener('click', function (event) {

        event.preventDefault();
        const startDate = new Date(startDateInput.value);
        const endDate = new Date(endDateInput.value);
        const journalIds = [];
        rows.forEach((row) => {
            row.style.display = '';
            const transactionDateCell = row.querySelector('td#transaction-date');
            let transactionDateStr = transactionDateCell.textContent.trim();
            const [month, day, year] = transactionDateStr.split('/').map(Number);
            const transactionDate = new Date(year, month - 1, day); 
            if (transactionDate >= startDate && transactionDate <= endDate) {
                const journalId = row.getAttribute('data-journal-id');
                if (journalId) {

                    journalIds.push(journalId);
                    row.style.display = '';
                }
            } else {
                const journalId = row.getAttribute('data-journal-id');
                if (journalIds.includes(journalId)) {

                } else {
                    row.style.display = 'none'; // Show row
                }
            }
        });
    });

});

document.addEventListener('DOMContentLoaded', function () {
    const getAmountRangeButton = document.getElementById('getAmountRangeButton');
    const lowerAmountInput = document.getElementById('startAmountDropdownForm');
    const higherAmountInput = document.getElementById('endAmountDropdownForm');
    var rows = document.querySelectorAll('#tableBody tr');
    getAmountRangeButton.addEventListener('click', function (event) {
        event.preventDefault();
        const lowerAmount = currencyToFloat(lowerAmountInput.value);
        const higherAmount = currencyToFloat(higherAmountInput.value);
        var journalIds = [];
        var rowsArray = Array.from(rows).reverse();
        rowsArray.forEach((row) => {
            row.style.display = 'none';
            const journalId = row.getAttribute('data-journal-id');
            if (journalIds.includes(journalId)) {
                row.style.display = ''; // Show row
            } else {
                const transactionAmountCell = row.querySelector('td#transaction-amount');
                if (transactionAmountCell) {
                    let transactionAmountStr = transactionAmountCell.textContent.trim();
                    let valueStr = transactionAmountStr.replace(/[^0-9.]/g, '');
                    const valueFloat = parseFloat(valueStr);

                    if (valueFloat >= lowerAmount && valueFloat <= higherAmount) {
                        console.log(valueFloat);
                        const journalId = row.getAttribute('data-journal-id');
                        console.log(journalId);
                        if (journalId) {
                            journalIds.push(journalId);
                            row.style.display = ''; // Show row
                        }
                    } else {
                        row.style.display = 'none';
                    }
                }
            }
        });
        
    });
});

function currencyToFloat(currencyString) {
    // Remove any non-numeric characters except for the decimal point
    let floatString = currencyString.replace(/[^0-9.]/g, '');
    return parseFloat(floatString);
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('resetTableButton').addEventListener('click', function () {
        const rows = document.querySelectorAll('#tableBody tr');
        rows.forEach(row => {
            row.style.display = '';
        });
    });
});

function addCurrencyFieldListeners() {
    document.querySelectorAll('.currencyField').forEach(function (input) {
        input.addEventListener('fousout', function (e) {
            let input = e.target;
            let value = input.value.trim();
            value = value.replace(/[^0-9.]/g, '');
            let parts = value.split('.');
            let integerPart = parts[0];
            let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
            var flt = parseInt(integerPart, 10);
            if (isNaN(flt)) {
                flt = 0;
            }
            integerPart = flt.toString();
            integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            let formattedValue = integerPart;
            if (formattedValue.length >= 1) {
                formattedValue = '$' + formattedValue;
            } else {
                formattedValue = '$0';
            }
            let newTail = "";
            if (decimalPart === "") {
                newTail = ".00";
            } else if (decimalPart.length == 1) {
                newTail = "." + decimalPart + "0";
            } else if (decimalPart.length == 2) {
                newTail = "." + decimalPart;
            } else if (decimalPart.length > 2) {
                newTail = "." + decimalPart.substring(0, 2);
            }
            formattedValue = formattedValue + newTail;
            input.value = formattedValue;
        });
    });
}
function currencyToFloat(currencyString) {
    return parseFloat(currencyString.replace(/[$,]/g, ''));
}