
//initialises variables
function CustomValidation(input) {
    this.invalidities = [];
    this.validityChecks = [];

    this.inputNode = input;

    this.registerListener();
}

//defines functions
CustomValidation.prototype = {
    addInvalidity: function (message) {
        this.invalidities.push(message);
    },
    getInvalidities: function () {
        return this.invalidities.join('. \n');
    },
    checkValidity: function (input) {
        for (var i = 0; i < this.validityChecks.length; i++) {

            var isInvalid = this.validityChecks[i].isInvalid(input);
            if (isInvalid) {
                this.addInvalidity(this.validityChecks[i].invalidityMessage);
            }

            var requirementElement = this.validityChecks[i].element;

            if (requirementElement) {
                if (isInvalid) {
                    requirementElement.classList.add('invalid');
                    requirementElement.classList.remove('valid');
                } else {
                    requirementElement.classList.add('valid');
                    requirementElement.classList.remove('invalid');
                }
            }
        }
    },
    checkInput: function () {
        this.inputNode.CustomValidation.invalidities = [];
        this.checkValidity(this.inputNode);

        if (this.inputNode.CustomValidation.invalidities.length == 0 && this.inputNode.value != '') {
            this.inputNode.setCustomValidity('');
        } else {
            var message = this.inputNode.CustomValidation.getInvalidities();
            this.inputNode.setCustomValidity(message);
        }
    },
    registerListener: function () {
        var CustomValidation = this;

        this.inputNode.addEventListener('keyup', function () {
            CustomValidation.checkInput();
        });
    }
};

//defines checks for username
var usernameValidityChecks = [
    {
        isInvalid: function (input) {
            var illegalCharacters = input.value.match(/[^a-zA-Z0-9 ]/g);
            return illegalCharacters ? true : false;
        },
        invalidityMessage: 'This input can only contain letters, numbers and spaces.',
        element: document.querySelector('div[id="username"] .input-requirements li:nth-child(1)')
    }
];

//defines checks for password
var passwordValidityChecks = [
    {
        isInvalid: function (input) {
            return input.value.length < 6;
        },
        invalidityMessage: 'This input needs to be at least 6 characters long',
        element: document.querySelector('div[id="password"] .input-requirements li:nth-child(1)')
    },
    {
        isInvalid: function (input) {
            return !input.value.match(/[0-9]/g);
        },
        invalidityMessage: 'At least 1 number is required',
        element: document.querySelector('div[id="password"] .input-requirements li:nth-child(3)')
    },
    {
        isInvalid: function (input) {
            return !input.value.match(/[\!\@\#\$\%\^\&\*\£]/g);
        },
        invalidityMessage: 'At least one special character is required',
        element: document.querySelector('div[id="password"] .input-requirements li:nth-child(2)')
    },
    {
        isInvalid: function (input) {
            return !input.value.match(/[A-Z]/g);
        },
        invalidityMessage: 'At least 1 uppercase letter is required',
        element: document.querySelector('div[id="password"] .input-requirements li:nth-child(4)')
    },
    {
        isInvalid: function (input) {
            return !input.value.match(/[a-z]/g);
        },
        invalidityMessage: 'At least 1 lowercase letter is required',
        element: document.querySelector('div[id="password"] .input-requirements li:nth-child(5)')
    }
];

//defines checks for email
var emailValidityChecks = [
    {
        isInvalid: function (input) {
            var validFormat = input.value.match(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/);
            return validFormat ? false : true
        },
        invalidityMessage: 'This is not a valid email address',
        element: document.querySelector('div[id="email"] .input-requirements li:nth-child(1)')
    }
];

//defines checks for repeat password
var repeatPasswordChecks = [
    {
        isInvalid: function () {
            return repeatPasswordInput.value != passwordInput.value;
        },
        invalidityMessage: 'The passwords do not match',
        element: document.querySelector('div[id="passwordRepeat"] .input-requirements li:nth-child(1)')
    }
];

//gets inputs
var usernameInput = document.getElementById('nameInput');
var passwordInput = document.getElementById('passwordInput');
var emailInput = document.getElementById('emailInput');
var repeatPasswordInput = document.getElementById('passwordRepeatInput');

//gets type of check for specific inputs
usernameInput.CustomValidation = new CustomValidation(usernameInput);
usernameInput.CustomValidation.validityChecks = usernameValidityChecks;

passwordInput.CustomValidation = new CustomValidation(passwordInput);
passwordInput.CustomValidation.validityChecks = passwordValidityChecks;

emailInput.CustomValidation = new CustomValidation(emailInput);
emailInput.CustomValidation.validityChecks = emailValidityChecks;

repeatPasswordInput.CustomValidation = new CustomValidation(passwordRepeatInput);
repeatPasswordInput.CustomValidation.validityChecks = repeatPasswordChecks;


var inputs = document.querySelectorAll('input');
var submit = document.querySelector('button[type="submit"]');
var form = document.getElementById('registerForm');

//define function that validates inputs
function validate() {
    for (var i = 0; i < inputs.length; i++) {
        inputs[i].CustomValidation.checkInput();
    }
}

//adds event listeners to validate
submit.addEventListener('click', validate);
form.addEventListener('submit', validate);