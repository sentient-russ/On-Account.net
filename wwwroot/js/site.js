const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

/*
 * ensure adhearand to form input standards.
 */
// these are set up from the div id's to manipulate the username as the user types.
var screen_name_element = document.getElementById("input_screen_name");
var first_name_element = document.getElementById("input_first_name");
var last_name_element = document.getElementById("input_last_name");
var lastname = "";
var firstname = "";
var firstInitial = "";
var lastNameInitial = "";
var date = new Date();
var currentYear = date.getFullYear();
var currentMonth = date.getMonth();
var username = "";
var currentMonthTwoDigits = currentMonth+1 <10 ? '0' + currentMonth : (currentMonth+1).toString();

//function that combines all inputs from user to create username following first intial + last name + month + last 2 digits of year
function usernameCreator() {
    screen_name_element.value = firstInitial + lastname + currentMonthTwoDigits + (currentYear % 100);
}
//assigns to lastname variable capital for username creation and capitalizes first initial
if (last_name_element != null) {
    last_name_element.addEventListener("input", function () {
        lastname = last_name_element.value;
        // Ensure that only apha characters can be entered into the Last Name field
        if (/[^a-zA-Z]/.test(lastname)) {
            // Remove non-letter characters
            lastname = lastname.replace(/[^a-zA-Z]/g, '');
            last_name_element.value = "";
        }
        lastNameInitial = lastname[0].toUpperCase();
        lastname = lastNameInitial + lastname.substring(1, lastname.length);
        usernameCreator();
    });
}

//assigns to firstInitial a captal letter for use for username creation
var first_name_element = document.getElementById("input_first_name");
if (last_name_element != null) {
    first_name_element.addEventListener("input", function () {
        firstname = first_name_element.value;
        //Ensure only alpha can be entered into the First Name field
        if (/[^a-zA-Z]/.test(firstname)) {
            // Remove non-letter characters
            firstname = firstname.replace(/[^a-zA-Z]/g, '');
            first_name_element.value = "";
        }
        firstInitial = firstname[0].toUpperCase();
        usernameCreator();
    });
}
//format phone numbers
var phoneElement = document.getElementById("Phone");

if (phoneElement != null) {
    var phoneNo = phoneElement.value;
    phoneElement.addEventListener('input', function () {
        var phoneNo = phoneElement.value.replace(/\D/g, '');
        var formatNum = "";
        if (phoneNo.length <= 3) {
            formatNum = phoneNo;
        } else if (phoneNo.length <= 6) {
            formatNum = phoneNo.substr(0, 3) + '-' + phoneNo.substr(3);
        } else if (phoneNo.length <= 10) {
            formatNum = phoneNo.substr(0, 3) + '-' + phoneNo.substr(3, 3) + '-' + phoneNo.substr(6);
        } else {
            formatNum = phoneNo.substr(0, 3) + '-' + phoneNo.substr(3, 3) + '-' + phoneNo.substr(6, 4);
        }
        phoneElement.value = formatNum;
    });
}

/*Begin Munu Buttons*/
document.addEventListener('DOMContentLoaded', function () {
    var accountingBtns = document.querySelectorAll('#accounting-btn, .accounting-btn');
    var accountingMenu = document.getElementById('accounting-menu');

    function toggleMenu(menu) {
        if (menu.style.display === 'none' || menu.style.display === '') {
            menu.style.display = 'block';
        } else {
            menu.style.display = 'none';
        }
    }
    accountingBtns.forEach(function (btn) {
        btn.addEventListener('click', function (event) {
            event.stopPropagation();
            toggleMenu(accountingMenu);
        });
    });
    document.addEventListener('click', function (event) {
        if (!accountingMenu.contains(event.target) && !Array.from(accountingBtns).includes(event.target)) {
            accountingMenu.style.display = 'none';
        }
    });
});

/*End Munu Buttons*/
document.querySelectorAll('.currencyField').forEach(function (input) {
    input.addEventListener('blur', function (e) {
        let input = e.target;
        let value = input.value;
        value = value.replace(/[^0-9.]/g, '');
        let parts = value.split('.');
        let integerPart = parts[0];
        let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';       

        //remove leading zero's
        var flt = parseInt(integerPart);

        //back to str
        integerPart = flt.toString()

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
////////////////////////////////////////////////////////////////////////////////
/*Create Account Form*/
var starting_balance = document.getElementById('starting_balance');
var transaction_1_cr = document.getElementById('transaction_1_cr');
var transaction_2_cr = document.getElementById('transaction_2_cr');
var transaction_1_dr = document.getElementById('transaction_1_dr');
var transaction_2_dr = document.getElementById('transaction_2_dr');
var transaction_dr_total = document.getElementById('transaction_dr_total');
var transaction_cr_total = document.getElementById('transaction_cr_total');
var journal_validatation = document.getElementById('journal_validation');
var save_journal_btn = document.getElementById('save_journal_btn');
var total_adjustment = document.getElementById('total_adjustment');

function updateStartingBalance() {
    var str1 = document.getElementById('transaction_dr_total').value.toString();
    var str2 = document.getElementById('transaction_cr_total').value.toString();

    if (str1.localeCompare(str2) == 0) {
        save_journal_btn.disabled = false;
        if (starting_balance != null) {
            starting_balance.value = transaction_dr_total.value;
        } else if (total_adjustment != null) {
            total_adjustment.value = transaction_dr_total.value;
        }
        
        document.getElementById("journal_validation").innerHTML = "";
    } else {
        document.getElementById("journal_validation").innerHTML = "The transaction is out of balance.";
        save_journal_btn.disabled = 'disabled';
    }
}

if (transaction_1_cr != null) {
    transaction_1_cr.addEventListener('focusout', function (e) {
        //calculate the column total and update the sum
        let input = e.target;
        var dr1 = input.value;
        var dr2 = transaction_2_cr.value;
        dr1 = dr1.replace(/[^0-9.]/g, '');
        dr2 = dr2.replace(/[^0-9.]/g, '');
        var total = parseFloat(dr1) + parseFloat(dr2);
        transaction_cr_total.value = total;
        //format the line total
        let value = transaction_cr_total.value;
        value = value.replace(/[^0-9.]/g, '');
        let parts = value.split('.');
        let integerPart = parts[0];
        let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
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
        transaction_cr_total.value = formattedValue;
        updateStartingBalance();
        //and again for the current cell value
        value = input.value;
        value = value.replace(/[^0-9.]/g, '');
        parts = value.split('.');
        integerPart = parts[0];
        decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
        integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        formattedValue = integerPart;
        if (formattedValue.length >= 1) {
            formattedValue = '$' + formattedValue;
        } else {
            formattedValue = '$0';
        }
        newTail = "";
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
        transaction_1_cr.value = formattedValue;

    });
}

if (transaction_2_cr != null) {
    transaction_2_cr.addEventListener('focusout', function (e) {
        //calculate the column total and update the sum
        let input = e.target;
        var dr1 = input.value;
        var dr2 = transaction_1_cr.value;
        dr1 = dr1.replace(/[^0-9.]/g, '');
        dr2 = dr2.replace(/[^0-9.]/g, '');

        var total = parseFloat(dr1) + parseFloat(dr2);
        transaction_cr_total.value = total;
        //format the line total
        let value = transaction_cr_total.value;

        value = value.replace(/[^0-9.]/g, '');
        let parts = value.split('.');
        let integerPart = parts[0];
        let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
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
        transaction_cr_total.value = formattedValue;
        updateStartingBalance();

        //and again for the current cell value
        value = input.value;
        value = value.replace(/[^0-9.]/g, '');
        parts = value.split('.');
        integerPart = parts[0];
        decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
        integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        formattedValue = integerPart;
        if (formattedValue.length >= 1) {
            formattedValue = '$' + formattedValue;
        } else {
            formattedValue = '$0';
        }
        newTail = "";
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
        transaction_2_cr.value = formattedValue;
    });
}
if (transaction_1_dr != null) {
    transaction_1_dr.addEventListener('focusout', function (e) {
        //calculate the column total and update the sum
        let input = e.target;
        var dr1 = input.value;
        var dr2 = transaction_2_dr.value;
        dr1 = dr1.replace(/[^0-9.]/g, '');
        dr2 = dr2.replace(/[^0-9.]/g, '');
        var total = parseFloat(dr1) + parseFloat(dr2);
        transaction_dr_total.value = total;
        //format the line total
        let value = transaction_dr_total.value;
        value = value.replace(/[^0-9.]/g, '');
        let parts = value.split('.');
        let integerPart = parts[0];
        let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
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
        transaction_dr_total.value = formattedValue;
        updateStartingBalance();
        //and again for the current cell value
        value = input.value;
        value = value.replace(/[^0-9.]/g, '');
        parts = value.split('.');
        integerPart = parts[0];
        decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
        integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        formattedValue = integerPart;
        if (formattedValue.length >= 1) {
            formattedValue = '$' + formattedValue;
        } else {
            formattedValue = '$0';
        }
        newTail = "";
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
        transaction_1_dr.value = formattedValue;  

    });
}
if (transaction_2_dr != null) {
    transaction_2_dr.addEventListener('focusout', function (e) {
        //calculate the column total and update the sum
        let input = e.target;
        var dr1 = input.value;
        var dr2 = transaction_1_dr.value;
        dr1 = dr1.replace(/[^0-9.]/g, '');
        dr2 = dr2.replace(/[^0-9.]/g, '');

        var total = parseFloat(dr1) + parseFloat(dr2);
        transaction_dr_total.value = total;
        //format the line total
        let value = transaction_dr_total.value;

        value = value.replace(/[^0-9.]/g, '');
        let parts = value.split('.');
        let integerPart = parts[0];
        let decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
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
        transaction_dr_total.value = formattedValue;
        updateStartingBalance();

        //and again for the current cell value
        value = input.value;
        value = value.replace(/[^0-9.]/g, '');
        parts = value.split('.');
        integerPart = parts[0];
        decimalPart = parts[1] ? parts[1].substring(0, 2) : '';
        integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        formattedValue = integerPart;
        if (formattedValue.length >= 1) {
            formattedValue = '$' + formattedValue;
        } else {
            formattedValue = '$0';
        }
        newTail = "";
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
        transaction_2_dr.value = formattedValue;        
    });
}

document.addEventListener('input', function (event) {
    const target = event.target;
    if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.tagName === 'SELECT') {
        if (target.value.trim() !== '') {
            target.classList.add('has-text');
        } else {
            target.classList.remove('has-text');
        }
    }
});
document.addEventListener('change', function (event) {
    const target = event.target;
    if (target.tagName === 'SELECT') {
        if (target.value.trim() !== '') {
            target.classList.add('has-text');
        } else {
            target.classList.remove('has-text');
        }
    }
});
