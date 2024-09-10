


// these are set up from the div id's to manipulate the username as the user types.
var screen_name_element = document.getElementById("input_screen_name");
var first_name_element = document.getElementById("input_first_name");
var last_name_element = document.getElementById("input_last_name");
/*screen_name_element.value = RSteele0924;*/
var lastname = "";
var firstInitial = "";
var lastNameInitial = "";
var date = new Date();
var currentYear = date.getFullYear();
var currentMonth = date.getMonth();
var username = "";

var currentMonthTwoDigits = currentMonth < 10 ? '0' + currentMonth : currentMonth.toString();
console.log(currentMonthTwoDigits);

//function that combines all inputs from user to create username following first intial + last name + month + last 2 digits of year
function usernameCreator() {
	screen_name_element.value = firstInitial + lastname + currentMonthTwoDigits + (currentYear%100);
}
//assigns to lastname variable capital for username creation and capitalizes first initial 
last_name_element.addEventListener("focusout", function () {
	lastname = last_name_element.value;
	lastNameInitial = lastname[0].toUpperCase();
	lastname = lastNameInitial + lastname.substring(1, lastname.length);
	console.log(lastname);
	usernameCreator();
});

//assigns to firstInitial a captal letter for use for username creation
first_name_element.addEventListener("focusout", function () {
	firstInitial = first_name_element.value[0];
	firstInitial=firstInitial.toUpperCase();
	console.log("Form ID:", first_name_element.value);
	console.log(firstInitial);
	usernameCreator();
});



//This solution was provided somewhere I cannot remember online.
$('#Phone')
	.keydown(function (e) {
		var key = e.which || e.charCode || e.keyCode || 0;
		$phone = $(this);

		// Don't let them remove the starting '('
		if ($phone.val().length === 1 && (key === 8 || key === 46)) {
			$phone.val('(');
			return false;
		}
		// Reset if they highlight and type over first char.
		else if ($phone.val().charAt(0) !== '(') {
			$phone.val('(' + String.fromCharCode(e.keyCode) + '');
		}

		// Auto-format- do not expose the mask as the user begins to type
		if (key !== 8 && key !== 9) {
			if ($phone.val().length === 4) {
				$phone.val($phone.val() + ')');
			}
			if ($phone.val().length === 5) {
				$phone.val($phone.val() + ' ');
			}
			if ($phone.val().length === 9) {
				$phone.val($phone.val() + '-');
			}
		}

		// Allow numeric (and tab, backspace, delete) keys only
		return (key == 8 ||
			key == 9 ||
			key == 46 ||
			(key >= 48 && key <= 57) ||
			(key >= 96 && key <= 105));
	})

	.bind('focus click', function () {
		$phone = $(this);

		if ($phone.val().length === 0) {
			$phone.val('(');
		}
		else {
			var val = $phone.val();
			$phone.val('').val(val); // Ensure cursor remains at the end
		}
	})

	.blur(function () {
		$phone = $(this);

		if ($phone.val() === '(') {
			$phone.val('');
		}
	});
