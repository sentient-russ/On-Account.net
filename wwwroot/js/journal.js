let transaction_array = [];
let journal_array = [];
var journal_id = document.getElementById('journal-id');
var created_by = document.getElementById('created-by');
var journal_date = document.getElementById('journal-date');
var journal_total = document.getElementById('journal-total-amount');
var journal_description = document.getElementById('journal-description');
var journal_status = document.getElementById('journal-status');
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('save_journal_btn').addEventListener('click', function (event) {
        event.preventDefault();
        var passed_check = false;
        passed_check = runValidation();
        if (passed_check) {
            const journalData = collectJournalData();
            const jsonData = JSON.stringify(journalData);
            const formData = new FormData();
            formData.append('journalData', jsonData); // Use jsonData instead of journalEntry
            const transactionContainers = document.querySelectorAll('.transaction-container');
            transactionContainers.forEach((container, index) => {
                const transactionUploadElement = container.querySelector('#transaction-upload');
                if (transactionUploadElement && transactionUploadElement.files.length > 0) {
                    formData.append(`transactionUpload_${index}`, transactionUploadElement.files[0]);
                }
            });
            fetch('/api/journal', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    console.log('Response:', data);
                    if (data.message === "Journal data received successfully") {
                        window.location.href = '/Accounting/';
                    } else {
                        console.error('Unexpected response:', data);
                    }
                })
                .catch(error => console.error('Error:', error));
        }
    });

    var journal_validation_elem = document.getElementById("journal_validation");

    function runValidation() {
        var validation_str = "";
        const dr_total_element = document.getElementById('dr-total');
        const cr_total_element = document.getElementById('cr-total');
        const transaction_date_element = document.getElementById('transaction-date');
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        var transaction_date = new Date(transaction_date_element.value);
        const firstDrAccount = document.querySelector(`#dr-account[data-transaction="1"][data-line="1"]`);
        const firstCrAccount = document.querySelector(`#cr-account[data-transaction="1"][data-line="2"]`);
        // total debits equals the total credits
        if (dr_total_element.value != cr_total_element.value) {
            validation_str += "The total debits must equal the total credits.\n";
        } else if (dr_total_element.value == "$0.00" || cr_total_element.value == "$0.00") {
            validation_str += "The transaction total must be greater than $0.00.\n";
        } else if (transaction_date > today) {
            validation_str += "The transaction date cannot be in the future.\n";
        } else if (firstDrAccount.value === "unselected") {
            validation_str += "The first journal line must have a debit account selected.\n";
        }
        // duplicate accounts
        const accounts = new Set();
        const accountElements = document.querySelectorAll('select[id^="dr-account"], select[id^="cr-account"]');
        accountElements.forEach(element => {
            const accountValue = element.value;
            if (accountValue !== "unselected") {
                if (accounts.has(accountValue)) {
                    validation_str += `Account ${accountValue} is used more than once.\n`;
                } else {
                    accounts.add(accountValue);
                }
            }
        });
        journal_validation_elem.innerHTML = validation_str;
        if (validation_str == "") {
            return true;
        } else {
            return false;
        }
    }
    function collectJournalData() {
        const journalEntry = {
            journal_id: document.getElementById('journal-id').value,
            user_name: document.getElementById('created-by').value,
            journal_date: document.getElementById('journal-date').value,
            journal_total: parseFloat(document.getElementById('journal-total-amount').value),
            journal_status: document.getElementById('journal-status').value,
            journal_notes: document.getElementById('journal-description').value,
            transactions: []
        };

        const transactionContainers = document.querySelectorAll('.transaction-container');
        transactionContainers.forEach(container => {
            const transactionId = container.getAttribute('data-transaction');
            const transaction = {
                data_transaction: transactionId,
                transaction_description: document.querySelector(`#transaction-description[data-transaction="${transactionId}"]`).value,
                transaction_date: document.querySelector(`#transaction-date[data-transaction="${transactionId}"]`).value,
                transaction_upload: document.querySelector(`#transaction-upload[data-transaction="${transactionId}"]`).value,
                line_items: []
            };

            const lineItems = container.querySelectorAll(`.journal-item-row[data-transaction="${transactionId}"]`);
            lineItems.forEach(lineItem => {
                const lineId = lineItem.getAttribute('data-line');
                if (lineId) {
                    const line = {
                        line: lineId,
                        dr_account: document.querySelector(`#dr-account[data-transaction="${transactionId}"][data-line="${lineId}"]`).value,
                        cr_account: document.querySelector(`#cr-account[data-transaction="${transactionId}"][data-line="${lineId}"]`).value,
                        post_ref: document.querySelector(`#post-ref[data-transaction="${transactionId}"][data-line="${lineId}"]`).value,
                        dr_amount: parseFloat(document.querySelector(`#dr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`).value.replace('$', '').replace(/,/g, '')),
                        cr_amount: parseFloat(document.querySelector(`#cr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`).value.replace('$', '').replace(/,/g, ''))
                    };
                    transaction.line_items.push(line);
                }
            });
            journalEntry.transactions.push(transaction);
        });

        return journalEntry;
    }
});

// Create a new line item
function createNewLineItem(transactionId, lineId) {
    const newLineItem = document.createElement('div');
    newLineItem.className = 'row g-1 journal-item-row';
    newLineItem.setAttribute('data-transaction', transactionId);
    newLineItem.setAttribute('data-line', lineId);
    const existingDrAccount = document.querySelector(`#dr-account[data-transaction="${transactionId}"][data-line="1"]`);
    const existingCrAccount = document.querySelector(`#cr-account[data-transaction="${transactionId}"][data-line="1"]`);
    const drAccountOptions = existingDrAccount ? existingDrAccount.innerHTML : '';
    const crAccountOptions = existingCrAccount ? existingCrAccount.innerHTML : '';
    newLineItem.innerHTML = `
            <div class="col-md-3">
                <select class="form-select form-control-sm" data-transaction="${transactionId}" data-line="${lineId}" id="dr-account">
                    <option value="unselected">Select a debit account</option>
                    ${drAccountOptions}
                </select>
            </div>
            <div class="col-md-3">
                <select class="form-select form-control-sm" data-transaction="${transactionId}" data-line="${lineId}" id="cr-account">
                    <option value="unselected">Select a credit account</option>
                    ${drAccountOptions}
                </select>
            </div>
            <div class="col-md-1">
                <div class="">
                    <input type="number" class="form-control muted" data-transaction="${transactionId}" data-line="${lineId}" id="post-ref" readonly>
                </div>
            </div>
            <div class="col-md-2">
                <div class="text-center">
                    <input class="form-control text-end currencyField" value="$0.00" data-transaction="${transactionId}" data-line="${lineId}" id="dr-amount" />
                </div>
            </div>
            <div class="col-md-2">
                <div class="">
                    <input class="form-control text-end currencyField" value="$0.00" data-transaction="${transactionId}" data-line="${lineId}" id="cr-amount" />
                </div>
            </div>
            <div class="col-00">
                <div class="add-new-line btn btn-outline-primary"  data-transaction="${transactionId}" data-line="${lineId}" id="add-new-line">
                    <span class="btn-symbol">+</span>
                </div>
                <div class="remove-new-line btn btn-outline-danger" data-transaction="${transactionId}" data-line="${lineId}" id="remove-new-line">
                    <span class="btn-symbol">-</span>
                </div>
            </div>
        `;
    return newLineItem;
}
// Add new line item
function addNewLine(transactionId, referenceRow) {
    const lastLineId = referenceRow.getAttribute('data-line');
    const newLineId = parseInt(lastLineId) + 1;
    const newLineItem = createNewLineItem(transactionId, newLineId);
    referenceRow.insertAdjacentElement('afterend', newLineItem);
    addCrTotallisteners();
    addDrTotallisteners();
    accountSelectionListeners();
    toggleButtonStates(transactionId);
}
// toggle add remove
function toggleButtonStates(transactionId) {
    const allRows = document.querySelectorAll(`.journal-item-row[data-transaction="${transactionId}"]`);
    allRows.forEach((row, index) => {
        const addButton = row.querySelector('.add-new-line');
        const removeButton = row.querySelector('.remove-new-line');

        if (addButton) {
            const isPriorRow = index <= allRows.length - 2;
            if (isPriorRow) {
                addButton.classList.add('btn-disabled');
                removeButton.classList.add('btn-disabled');
            } else {
                addButton.classList.remove('btn-disabled');
                removeButton.classList.remove('btn-disabled');
            }
        }
    });
}
// add remove new line listeners
document.addEventListener('click', function (event) {
    if (event.target.closest('.add-new-line')) {
        const referenceRow = event.target.closest('.journal-item-row');
        const transactionId = referenceRow.getAttribute('data-transaction');
        addNewLine(transactionId, referenceRow);
    } else if (event.target.closest('.remove-new-line')) {
        const referenceRow = event.target.closest('.journal-item-row');
        const lineId = referenceRow.getAttribute('data-line');
        // encforce two line min rule
        if (lineId !== '2') {  // Skip removing the first line
            referenceRow.remove();
            addCrTotallisteners();
            addDrTotallisteners();
            accountSelectionListeners();
            const transactionId = referenceRow.getAttribute('data-transaction');
            toggleButtonStates(transactionId);
        }
    }
});
function formatCurrency() {
    document.querySelectorAll('.currencyField').forEach(function (input) {
        const currencyFields = document.querySelectorAll('.currencyField');
        currencyFields.forEach(field => {
            let input = field.target;
            let value = field.value.trim(); // Trim spaces from the input
            value = value.replace(/[^0-9.]/g, '');
            let parts = value.split('.');
            let integerPart = parts[0];
            let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
            // Remove leading zeros and handle empty integerPart
            var flt = parseInt(integerPart, 10);
            if (isNaN(flt)) {
                flt = 0;
            }
            // Back to string
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
            field.value = formattedValue;
        });
    });
}
function formatCurrencyOnLoad() {
    const currencyFields = document.querySelectorAll('.currencyField');
    currencyFields.forEach(field => {
        let input = field.target;
        let value = field.value.trim(); // Trim spaces from the input
        value = value.replace(/[^0-9.]/g, '');
        let parts = value.split('.');
        let integerPart = parts[0];
        let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
        // Remove leading zeros and handle empty integerPart
        var flt = parseInt(integerPart, 10);
        if (isNaN(flt)) {
            flt = 0;
        }
        // Back to string
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
        field.value = formattedValue;
    });
}
function addCurrencyFieldListeners() {
    document.querySelectorAll('.currencyField').forEach(function (input) {
        input.addEventListener('fousout', function (e) {
            let input = e.target;
            let value = input.value.trim(); // Trim spaces from the input
            value = value.replace(/[^0-9.]/g, '');
            let parts = value.split('.');
            let integerPart = parts[0];
            let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
            // Remove leading zeros and handle empty integerPart
            var flt = parseInt(integerPart, 10);
            if (isNaN(flt)) {
                flt = 0;
            }
            // Back to string
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

function updateCrTotals() {
    const crAmountElements = document.querySelectorAll('[id="cr-amount"]');
    const totalsByTransaction = {};
    crAmountElements.forEach(element => {
        const transactionNumber = element.getAttribute('data-transaction');
        const amount = currencyToFloat(element.value);
        if (!totalsByTransaction[transactionNumber]) {
            totalsByTransaction[transactionNumber] = 0;
        }
        totalsByTransaction[transactionNumber] += amount;
    });
    for (const transactionNumber in totalsByTransaction) {
        var total = totalsByTransaction[transactionNumber];
        const crTotalElement = document.querySelector(`[id="cr-total"][data-transaction="${transactionNumber}"]`);
        total = total.toString();
        crTotalElement.value = total;
        formatCurrency();
    }
    formatCurrency();
}
function addCrTotallisteners() {
    document.querySelectorAll('[id="cr-amount"]').forEach(element => {
        element.addEventListener('focusout', updateCrTotals);
    });
}
function updateDrTotals() {
    const drAmountElements = document.querySelectorAll('[id="dr-amount"]');
    const totalsByTransaction = {};
    drAmountElements.forEach(element => {
        const transactionNumber = element.getAttribute('data-transaction');
        const amount = currencyToFloat(element.value);
        if (!totalsByTransaction[transactionNumber]) {
            totalsByTransaction[transactionNumber] = 0;
        }
        totalsByTransaction[transactionNumber] += amount;
    });
    for (const transactionNumber in totalsByTransaction) {
        var total = totalsByTransaction[transactionNumber];
        const drTotalElement = document.querySelector(`[id="dr-total"][data-transaction="${transactionNumber}"]`);
        console.log(total);
        total = total.toString();
        console.log(total);
        drTotalElement.value = total;
        formatCurrency();
    }
    formatCurrency();
}
function addDrTotallisteners() {
    document.querySelectorAll('[id="dr-amount"]').forEach(element => {
        element.addEventListener('focusout', updateDrTotals);
    });
}
function accountSelectionListeners() {
    document.querySelectorAll('select[id^="dr-account"], select[id^="cr-account"]').forEach(function (select) {
        select.addEventListener('change', function (event) {
            const select = event.target;
            const transactionId = select.getAttribute('data-transaction');
            const lineId = select.getAttribute('data-line');
            select.style.backgroundColor = 'white';
            select.style.color = 'black';
            if (select.id.startsWith('dr-account')) {
                const crAccountSelect = document.querySelector(`#cr-account[data-transaction="${transactionId}"][data-line="${lineId}"]`);
                const crAmountSelect = document.querySelector(`#cr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`);
                const drAmountSelect = document.querySelector(`#dr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`);
                if (crAccountSelect) {
                    crAccountSelect.value = 'unselected';
                    crAccountSelect.style.backgroundColor = '#6c757d0b';
                    crAccountSelect.style.color = '#6c757d';

                    if (crAmountSelect) {
                        crAmountSelect.value = "$0.00";
                        crAmountSelect.style.backgroundColor = '#6c757d0b';
                        crAmountSelect.style.color = '#6c757d';
                        crAmountSelect.readOnly = true;
                    }
                }
                if (drAmountSelect) {
                    drAmountSelect.readOnly = false;
                    drAmountSelect.style.backgroundColor = 'white';
                    drAmountSelect.style.color = 'black';
                }
            } else if (select.id.startsWith('cr-account')) {
                const drAccountSelect = document.querySelector(`#dr-account[data-transaction="${transactionId}"][data-line="${lineId}"]`);
                const drAmountSelect = document.querySelector(`#dr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`);
                const crAmountSelect = document.querySelector(`#cr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`);
                if (drAccountSelect) {
                    drAccountSelect.value = 'unselected';
                    drAccountSelect.style.backgroundColor = '#6c757d0b';
                    drAccountSelect.style.color = '#6c757d';
                    if (drAmountSelect) {
                        drAmountSelect.value = "$0.00";
                        drAmountSelect.style.backgroundColor = '#6c757d0b';
                        drAmountSelect.style.color = '#6c757d';
                        drAmountSelect.readOnly = true;
                    }
                }
                if (crAmountSelect) {
                    crAmountSelect.readOnly = false;
                    crAmountSelect.style.backgroundColor = 'white';
                    crAmountSelect.style.color = 'black';
                }
            }
        });
    });
}
document.addEventListener('DOMContentLoaded', function () {
    accountSelectionListeners();
});

addCrTotallisteners();
addDrTotallisteners();
formatCurrencyOnLoad();
updateDrTotals();
updateCrTotals();
accountSelectionListeners();

document.getElementById('transaction-upload').addEventListener('change', function (e) {
    const fileInput = e.target;
    const fileName = fileInput.files[0] ? fileInput.files[0].name : 'Supporting Document';
    const fileLink = document.getElementById('file-link');
    const fileUrl = 'https://on-account.net/uploaded_docs/' + fileName;
    fileLink.innerHTML = `<a href="${fileUrl}" target="_blank">${fileName}</a>`;
});
