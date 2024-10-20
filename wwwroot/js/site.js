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
