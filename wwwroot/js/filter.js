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
    let floatString = currencyString.replace(/[^0-9.]/g, '');
    return parseFloat(floatString);
}
//This handles filtering for journal detail pos ref links
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById('searchInput');
    const tableBody = document.getElementById('tableBody');
    const tableRows = document.querySelectorAll('table tbody tr');
    const rows = tableBody.getElementsByTagName('tr');
    const inputText = searchInput.value.trim() + "||";
    function filterRows(searchInput) {
        setTimeout(() => {
            tableRows.forEach(row => {
                const hiddenText = row.querySelector('td.text-center[hidden]').innerText.trim();
                const regex = new RegExp(`\\b${searchInput}\\b`);
                if (regex.test(hiddenText)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        }, 200);
    }
    if (searchInput.value.trim() !== "") {
        filterRows(searchInput.value.trim().toUpperCase());
    }
});

