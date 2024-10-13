let transaction_array = [];
let journal_array = [];
var journal_id = document.getElementById('journal-id');
var created_by = document.getElementById('created-by');
var journal_date = document.getElementById('journal-date');
var journal_total = document.getElementById('journal-total-amount');
var journal_description = document.getElementById('journal-description');
var journal_status = document.getElementById('journal-status');

document.getElementById('save_journal_btn').addEventListener('click', function (event) {
    event.preventDefault();

    function getInputValue(id) {
        const element = document.getElementById(id);
        return element ? element.value : null;
    }

    function getSelectValue(id) {
        const element = document.getElementById(id);
        return element ? element.options[element.selectedIndex].value : null;
    }

    function parseCurrency(value) {
        return parseFloat(value.replace(/[^0-9.-]+/g, "")) || 0;
    }

    const journalData = {
        journal_id: getInputValue('journal-id'),
        user_name: getInputValue('created-by'),
        journal_date: getInputValue('journal-date'),
        journal_total: parseCurrency(getInputValue('journal-total-amount')),
        journal_status: getInputValue('journal-status'),
        journal_notes: getInputValue('journal-description'),
        transactions: []
    };

    const transactionContainers = document.querySelectorAll('.transaction-container');
    transactionContainers.forEach(container => {
        const transactionDate = getInputValue('transaction-date');

        const transactionDescriptionElement = container.querySelector('#transaction_description');
        const transactionDescription = transactionDescriptionElement ? transactionDescriptionElement.value : null;

        const transactionLines = [];
        const lineRows = container.querySelectorAll('.journal-item-row');
        lineRows.forEach(row => {
            const lineNum = row.getAttribute('data-line-num');
            const transactionNum = row.getAttribute('data-transaction');
            if (lineNum !== null && transactionNum !== null) {
                const drAccountElement = row.querySelector(`#dr-account-${lineNum}`);
                const drAccount = drAccountElement ? getSelectValue(drAccountElement.id) : null;

                const crAccountElement = row.querySelector(`#cr-account-${lineNum}`);
                const crAccount = crAccountElement ? getSelectValue(crAccountElement.id) : null;

                const postRefElement = row.querySelector(`#post-ref-${lineNum}`);
                const postRef = postRefElement ? postRefElement.value : null;

                const drAmountElement = row.querySelector(`#dr-ammount-${lineNum}`);
                const drAmount = drAmountElement ? parseCurrency(drAmountElement.value) : 0;

                const crAmountElement = row.querySelector(`#cr-ammount-${lineNum}`);
                const crAmount = crAmountElement ? parseCurrency(crAmountElement.value) : 0;

                // Log the values for debugging
                console.log(`Line ${lineNum}:`, {
                    dr_account: drAccount,
                    cr_account: crAccount,
                    post_ref: postRef,
                    dr_amount: drAmount,
                    cr_amount: crAmount
                });

                // Only include lines where either the debit or credit account is not null or unselected
                if (drAccount !== null && drAccount !== 'unselected' || crAccount !== null && crAccount !== 'unselected') {
                    const lineData = {
                        line: lineNum,
                        dr_account: drAccount,
                        cr_account: crAccount,
                        post_ref: postRef,
                        dr_amount: drAmount,
                        cr_amount: crAmount
                    };
                    transactionLines.push(lineData);
                }
            }
        });

        journalData.transactions.push({
            transaction_date: transactionDate,
            transaction_description: transactionDescription,
            lines: transactionLines
        });
    });

    // Convert the collected data to JSON
    const jsonData = JSON.stringify(journalData, null, 2);
    console.log('JSON Data:', jsonData); // Log the JSON data to the console

    // Create a FormData object to send both JSON data and files
    const formData = new FormData();
    formData.append('journalData', jsonData);

    transactionContainers.forEach((container, index) => {
        const transactionUploadElement = container.querySelector('#transaction-upload');
        if (transactionUploadElement && transactionUploadElement.files.length > 0) {
            formData.append(`transactionUpload_${index}`, transactionUploadElement.files[0]);
            console.log(`File appended: transactionUpload_${index}`, transactionUploadElement.files[0]); // Log the appended file
        } else {
            console.log(`No file found for container ${index}`); // Log if no file is found
        }
    });

    // Log the FormData entries to the console
    for (let pair of formData.entries()) {
        console.log(pair[0] + ':', pair[1]);
    }

    // Send the FormData to the server using fetch
    fetch('/api/journal', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('Error:', error));
});


document.addEventListener('click', function (event) {
    if (event.target.closest('.add-new-line')) {
        event.preventDefault();
        const currentRow = event.target.closest('.journal-item-row');
        if (!currentRow) return;

        // Clone the current row
        const newRow = currentRow.cloneNode(true);

        // Get the next line number
        const lineNum = parseInt(currentRow.getAttribute('data-line-num'), 10) + 1;
        const transactionNum = currentRow.getAttribute('data-transaction');

        // Update the data-line-num and id attributes for the new row
        newRow.setAttribute('data-line-num', lineNum);
        newRow.setAttribute('id', `line-${lineNum}`);

        // Update the data-line-num and id attributes for all child elements
        newRow.querySelectorAll('[data-line-num]').forEach(element => {
            element.setAttribute('data-line-num', lineNum);
        });
        newRow.querySelectorAll('[id]').forEach(element => {
            const oldId = element.id;
            const newId = oldId.replace(/-\d+$/, '') + '-' + lineNum;
            element.id = newId;
        });

        // Reset the values of the new row
        newRow.querySelectorAll('.currencyField').forEach(element => {
            element.value = '$0.00';
        });

        // Insert the new row after the current row
        currentRow.parentNode.insertBefore(newRow, currentRow.nextSibling);

        // Update line numbers for all rows
        updateLineNumbers();

        // Re-initialize currency formatting
        formatCurrency();
    }

    if (event.target.closest('.remove-new-line')) {
        event.preventDefault();
        const currentRow = event.target.closest('.journal-item-row');
        if (!currentRow) return;
        currentRow.remove();
        updateLineNumbers();
        formatCurrency(); // Re-initialize currency formatting
    }
});

function updateLineNumbers() {
    const rows = document.querySelectorAll('.journal-item-row');
    rows.forEach((row, index) => {
        const lineNum = index + 1;
        row.querySelectorAll('[data-line-num]').forEach(element => {
            element.setAttribute('data-line-num', lineNum);
        });
        row.querySelectorAll('[id]').forEach(element => {
            const oldId = element.id;
            const newId = oldId.replace(/-\d+$/, '') + '-' + lineNum;
            element.id = newId;
        });
    });
}
function formatCurrency() {
    document.querySelectorAll('.currencyField').forEach(function (input) {
        input.addEventListener('blur', function (e) {
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
formatCurrency();