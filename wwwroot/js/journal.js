
let transaction_array = [];
let journal_array = [];
var journal_id = document.getElementById('journal-id');
var created_by = document.getElementById('created-by');
var journal_date = document.getElementById('journal-date');
var journal_total = document.getElementById('journal-total-amount');
var journal_description = document.getElementById('journal-description');
var journal_status = document.getElementById('journal-status');

document.addEventListener('DOMContentLoaded', function () {

    // Add event listener to the save_journal_btn
    document.getElementById('save_journal_btn').addEventListener('click', function (event) {
        event.preventDefault();
        var passed_check = false;
        passed_check = runValidation();
        if (passed_check) {
            const journalData = collectJournalData();

            console.log(journalData);
            const jsonData = JSON.stringify(journalData);
            console.log(jsonData);

            // Create a FormData object to send both JSON data and files
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
                .then(data => console.log(data))
                .catch(error => console.error('Error:', error));
        }
    });

    var journal_validation_elem = document.getElementById("journal_validation");
    function runValidation() {
        const passed_check = false;
        var validation_str = "";
        const dr_total_element = document.getElementById('dr-total');
        const cr_total_element = document.getElementById('cr-total');
        const transaction_date_element = document.getElementById('transaction-date');
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        var transaction_date = new Date(transaction_date_element.value);
        const firstDrAccount = document.querySelector(`#dr-account[data-transaction="1"][data-line="1"]`);
        const firstCrAccount = document.querySelector(`#cr-account[data-transaction="1"][data-line="2"]`);

        if (dr_total_element.value != cr_total_element.value) {
            validation_str += "The total debits must equal the total credits.\n";
        } else if (dr_total_element.value == "$0.00" || cr_total_element.value == "$0.00") {
            validation_str += "The transaction total must be greater than $0.00.\n";
        } else if (transaction_date > today) {
            validation_str += "The transaction date cannot be in the future.\n";
        } else if (firstDrAccount.value === "unselected") {
            validation_str += "The first journal line must have a debit account selected.\n";
        } else if (firstCrAccount.value === "unselected") {
            validation_str += "The second journal line must have a credit account selected.\n";
        }

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
                        dr_amount: parseFloat(document.querySelector(`#dr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`).value.replace('$', '')),
                        cr_amount: parseFloat(document.querySelector(`#cr-amount[data-transaction="${transactionId}"][data-line="${lineId}"]`).value.replace('$', ''))
                    };

                    console.log(`Line ${lineId} - dr_account:`, line.dr_account);
                    console.log(`Line ${lineId} - cr_account:`, line.cr_account);

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
                    ${crAccountOptions}
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
                <div class="add-new-line btn btn-outline-primary" id="add-new-line">
                    <span class="btn-symbol">+</span>
                </div>
                <div class="remove-new-line btn btn-outline-danger" id="remove-new-line">
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


}

// Add event listeners to the add line and remove line buttons
document.addEventListener('click', function (event) {
    if (event.target.closest('#add-new-line')) {
        const referenceRow = event.target.closest('.journal-item-row');
        const transactionId = referenceRow.getAttribute('data-transaction');
        addNewLine(transactionId, referenceRow);
    } else if (event.target.closest('#remove-new-line')) {
        const referenceRow = event.target.closest('.journal-item-row');
        referenceRow.remove();
    }
});
function addNewTransaction(event) {
    const currentTransaction = event.target.closest('.transaction-container');
    const lastTransactionId = parseInt(currentTransaction.getAttribute('data-transaction'));
    const newTransactionId = lastTransactionId + 1;

    const newTransaction = document.createElement('div');
    newTransaction.className = 'transaction-container';
    newTransaction.setAttribute('data-transaction', newTransactionId);

    const existingDrAccount = document.querySelector(`#dr-account[data-transaction="1"][data-line="1"]`);
    const existingCrAccount = document.querySelector(`#cr-account[data-transaction="1"][data-line="1"]`);

    const drAccountOptions = existingDrAccount ? existingDrAccount.innerHTML : '';
    const crAccountOptions = existingCrAccount ? existingCrAccount.innerHTML : '';

    newTransaction.innerHTML = `
                <div class="journal-container text-center">
                    <!-- heading row -->
                    <div class="journal-row-heading">
                        <div class="col-md-3 transaction-date-container">
                            <div class="journal-transaction-date">
                                <input class="form-control" type="date" value="@DateTime.Now.ToString("yyyy-MM-dd")" data-transaction="${newTransactionId}" data-line="0" id="transaction-date" />
                            </div>
                        </div>
                        <div class="col-md-6 transaction-upload-container ">
                            <div class="journal-transaction-upload">
                                <input class="form-control" type="file" data-transaction="${newTransactionId}" id="transaction-upload">
                            </div>
                        </div>
                        <div class="col-md-4 add-sub-transactions-container">
                            <div class="col-6">
                                <div class="add-sub-transactions-container">
                                    <span class="add-sub-transactions-label">Add/Subtract Transactions:</span>
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="add-new-transaction btn btn-primary" data-transaction="${newTransactionId}" id="add-transaction">
                                    <span class="btn-symbol">+</span>
                                </div>
                                <div class="remove-new-transaction btn btn-danger" data-transaction="${newTransactionId}" id="remove-transaction">
                                    <span class="btn-symbol">-</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="journal-row">
                        <!-- entry 1.1 row -->
                        <div class="row g-1 journal-item-row" data-transaction="${newTransactionId}" data-line="1">
                            <div class="col-md-3">
                                <span>Debit Account:</span>
                                <select class="form-select form-control-sm" data-transaction="${newTransactionId}" data-line="1" id="dr-account">
                                <option value="unselected">Select a debit account</option>
                                ${drAccountOptions}
                            </select>
                            </div>
                            <div class="col-md-3">
                                <span>Credit Account:</span>
                                <select class="form-select form-control-sm" data-transaction="${newTransactionId}" data-line="1" id="cr-account">
                                <option value="unselected">Select a credit account</option>
                                ${crAccountOptions}
                            </select>
                            </div>
                            <div class="col-md-1">
                                <span>Post Ref:</span>
                                <div class="">
                                    <input type="number" class="form-control muted" data-transaction="${newTransactionId}" data-line="1" id="post-ref" readonly>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <span>Dr.</span>
                                <div class="text-center">
                                    <input class="form-control text-end currencyField" value="$0.00" data-transaction="${newTransactionId}" data-line="1" id="dr-amount" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <span>Cr.</span>
                                <div class="">
                                    <input class="form-control text-end currencyField" value="$0.00" data-transaction="${newTransactionId}" data-line="1" id="cr-amount" />
                                </div>
                            </div>
                        </div>
                        <!-- end entry 1.1 row -->
                        <!-- entry 1.2 row -->
                        <div class="row g-1 journal-item-row" data-transaction="${newTransactionId}" data-line="2">
                            <div class="col-md-3">
                                <select class="form-select form-control-sm" data-transaction="${newTransactionId}" data-line="2" id="dr-account">
                                <option value="unselected">Select a debit account</option>
                                ${drAccountOptions}
                            </select>
                            </div>
                            <div class="col-md-3">
                                <select class="form-select form-control-sm" data-transaction="${newTransactionId}" data-line="2" id="cr-account">
                                <option value="unselected">Select a credit account</option>
                                ${crAccountOptions}
                            </select>
                            </div>
                            <div class="col-md-1">
                                <div class="">
                                    <input type="number" class="form-control muted" data-transaction="${newTransactionId}" data-line="2" id="post-ref" readonly>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="text-center">
                                    <input class="form-control text-end currencyField" value="$0.00" data-transaction="${newTransactionId}" data-line="2" id="dr-amount" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="">
                                    <input class="form-control text-end currencyField" value="$0.00" data-transaction="${newTransactionId}" data-line="2" id="cr-amount" />
                                </div>
                            </div>
                            <div class="col-00">
                                <div class="add-new-line btn btn-outline-primary" id="add-new-line" data-line="2">
                                    <span class="btn-symbol">+</span>
                                </div>
                                <div class="remove-new-line btn btn-outline-danger" >
                                    <span class="btn-symbol">-</span>
                                </div>
                            </div>

                        </div>
                        <!-- end entry 1.2 row -->
                        <div class="row g-1 journal-item-row">
                            <div class="row g-1 journal-item-row">
                                <div class="col-md-6">
                                    <div class="transaction-description">
                                        <input style="text-align:left;" class="form-control transaction-description" placeholder="Transaction Description" data-transaction="${newTransactionId}" id="transaction-description" />
                                    </div>
                                </div>
                                <div class="col-md-1">
                                    <div class="journal-totals">
                                        <span class="totals">Totals</span>
                                    </div>
                                </div>
                                <div class="col-md-2" style="margin-left: 5px;">
                                    <hr class="hr-journal-totals" />
                                    <hr class="hr-journal-totals" />
                                    <div class="">
                                        <input class="form-control text-end currencyField" value="$0.00" data-val="false" data-transaction="${newTransactionId}" id="dr-total" readonly />
                                    </div>
                                </div>
                                <div class="col-md-2" style="margin-left:2.5px">
                                    <hr class="hr-journal-totals" />
                                    <hr class="hr-journal-totals" />
                                    <div class="">
                                        <input class="form-control text-end currencyField" value="$0.00" data-transaction="${newTransactionId}" id="cr-total" readonly />
                                    </div>
                                </div>
                            </div>
                            <div>
                                <div class="warning-row">
                                    <div class="text-danger text-right" id="journal_validation"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;

    // Find the journal-container element
    const journalContainer = currentTransaction.querySelector('.journal-container');

    // Insert the new transaction after the journal-container element
    journalContainer.insertAdjacentElement('afterend', newTransaction);
    addCrTotallisteners();
    addDrTotallisteners();
}

// Remove transaction
function removeTransaction(event) {
    const currentTransaction = event.target.closest('.transaction-container');
    const firstTransaction = document.querySelector('.transaction-container:first-of-type');

    if (currentTransaction !== firstTransaction) {
        currentTransaction.remove();
        addCrTotallisteners();
    }
}

// Add event listeners to the add transaction and remove transaction buttons
document.addEventListener('click', function (event) {
    if (event.target.closest('#add-transaction')) {
        addNewTransaction(event);
        addCrTotallisteners();
    } else if (event.target.closest('#remove-transaction')) {
        removeTransaction(event);
        addCrTotallisteners();
    }
});
// imediate update
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
// Function to convert currency string to float
function currencyToFloat(currencyString) {
    // Remove the dollar sign and commas
    return parseFloat(currencyString.replace(/[$,]/g, ''));
}

// Function to update cr totals
function updateCrTotals() {
    // Select all cr_amount elements
    const crAmountElements = document.querySelectorAll('[id="cr-amount"]');

    // Create an object to store the totals by transaction number
    const totalsByTransaction = {};

    // Loop through each cr_amount element
    crAmountElements.forEach(element => {
        const transactionNumber = element.getAttribute('data-transaction');
        const amount = currencyToFloat(element.value);

        // Initialize the total for this transaction number if not already done
        if (!totalsByTransaction[transactionNumber]) {
            totalsByTransaction[transactionNumber] = 0;
        }

        // Add the amount to the total for this transaction number
        totalsByTransaction[transactionNumber] += amount;
    });

    // Loop through the totalsByTransaction object and update the corresponding cr-total elements
    for (const transactionNumber in totalsByTransaction) {
        var total = totalsByTransaction[transactionNumber];
        const crTotalElement = document.querySelector(`[id="cr-total"][data-transaction="${transactionNumber}"]`);
        console.log(total);
        total = total.toString();
        console.log(total);
        crTotalElement.value = total;
        formatCurrency();

    }
    formatCurrency();
}
function addCrTotallisteners() {
    // Attach the updateTotals function to the focusout event for all cr_amount inputs
    document.querySelectorAll('[id="cr-amount"]').forEach(element => {
        element.addEventListener('focusout', updateCrTotals);
    });
}

// Function to update dr totals
function updateDrTotals() {
    // Select all dr_amount elements
    const drAmountElements = document.querySelectorAll('[id="dr-amount"]');

    // Create an object to store the totals by transaction number
    const totalsByTransaction = {};

    // Loop through each cr_amount element
    drAmountElements.forEach(element => {
        const transactionNumber = element.getAttribute('data-transaction');
        const amount = currencyToFloat(element.value);

        // Initialize the total for this transaction number if not already done
        if (!totalsByTransaction[transactionNumber]) {
            totalsByTransaction[transactionNumber] = 0;
        }

        // Add the amount to the total for this transaction number
        totalsByTransaction[transactionNumber] += amount;
    });

    // Loop through the totalsByTransaction object and update the corresponding cr-total elements
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
    // Attach the updateTotals function to the focusout event for all cr_amount inputs
    document.querySelectorAll('[id="dr-amount"]').forEach(element => {
        element.addEventListener('focusout', updateDrTotals);
    });
}

addCrTotallisteners();
addDrTotallisteners();

