module Directives {
    // Directive to ensure a string quantity value is an integer, decimal or fractional format.
    export class Quantity {
        constructor() {
            this.require = 'ngModel';
            this.restrict = 'A';
        }


        require: string;
        restrict: string;


        link(scope, elem, attrs, ngModel) {
            // validity function
            var quantityIsValid = function (value: string): boolean {
                // return false if we don't have some value
                if (!value || value.length == 0) {
                    return false;
                }

                // use an any type to make use of isNaN
                var testValue: any = value.trim();

                // If the string is a number return true
                if (!isNaN(testValue)) {
                    return true;
                }

                // If the string has more than two space deliminted elements return false
                var parts = testValue.split(' ');
                if (parts.length > 2) {
                    return false;
                }

                // string should be a number or a number plus fractional piece
                var valid = true;
                for (var i = 0; i < parts.length; i++) {
                    var fractionParts = parts[i].split('/');
                    if (fractionParts.length == 1) {
                        if (i > 0) {
                            valid = false;
                            break;
                        }
                        else {
                            valid = valid && !isNaN(fractionParts[0]);
                        }
                    }
                    else if (fractionParts.length == 2) {
                        valid = valid && fractionParts[0].trim().length > 0 && !isNaN(fractionParts[0]) && !isNaN(fractionParts[1]) && fractionParts[1] != 0;
                    }
                    else {
                        valid = false;
                        break;
                    }
                }
                return valid;
            }

            // dom to model validation
            ngModel.$parsers.unshift(function (value: string) {
                var valid = quantityIsValid(value);
                ngModel.$setValidity('quantity', valid);
                return valid ? value : undefined;
            });

            // model to dom validation
            ngModel.$formatters.unshift(function (value) {
                var valid = quantityIsValid(value);
                ngModel.$setValidity('quantity', valid);
                return value;
            });
        }
    }
}
 