
/*
 * ensure adhearand to form input standards.
 */
// these are set up from the div id's to manipulate the username as the user types.
var screen_name_element = document.getElementById("input_screen_name");
var first_name_element = document.getElementById("input_first_name");
var last_name_element = document.getElementById("input_last_name");
/*screen_name_element.value = RSteele0924;*/
var lastname = "";
var firstname = "";
var firstInitial = "";
var lastNameInitial = "";
var date = new Date();
var currentYear = date.getFullYear();
var currentMonth = date.getMonth();
var username = "";
var currentMonthTwoDigits = currentMonth < 10 ? '0' + currentMonth : currentMonth.toString();

//function that combines all inputs from user to create username following first intial + last name + month + last 2 digits of year
function usernameCreator() {
	screen_name_element.value = firstInitial + lastname + currentMonthTwoDigits + (currentYear%100);
}
//assigns to lastname variable capital for username creation and capitalizes first initial
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
//assigns to firstInitial a captal letter for use for username creation
var first_name_element = document.getElementById("input_first_name");
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

//format phone numbers
var phoneElement = document.getElementById("Phone");
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

