module Directives {
    // Directive to validate a sting "looks" like an email address
    export class Email {
        constructor() {
            this.require = 'ngModel';
            this.restrict = 'A';
        }


        require: string;
        restrict: string;


        // TODO: I think I can delete this.
        // email validation useing regex, source: The Code Project article "Email Address Validation Using Regular Expression" by Mykola Dobrochynskyy
        private emailIsValid(value: string): boolean {
            return /^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$/.test(value);
        }

        link(scope, elem, attrs, ctrl) {
            // validity function
            var emailIsValid = function (value) {
                // source: The Code Project article "Email Address Validation Using Regular Expression" by Mykola Dobrochynskyy
                return /^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$/.test(value);
            };

            // dom to model validation
            ctrl.$parsers.unshift(function (value) {
                var valid = emailIsValid(value);
                ctrl.$setValidity('email', valid);
                return valid ? value : undefined;
            });

            // model to dom validation
            ctrl.$formatters.unshift(function (value) {
                var valid = emailIsValid(value);
                ctrl.$setValidity('email', valid);
                return value;
            });
        }
    }
}
 