module Directives {
    // Directive to match two inputs, e.g. item and item confirm, source: http://ngmodules.org/modules/angular-input-match
    export class Match {
        constructor() {
            this.require = 'ngModel';
            this.restrict = 'A';
            this.scope = { match: '=' };
        }


        require: string;
        restrict: string;
        scope: any;


        link(scope: any, elem, attrs, ctrl) {
            scope.$watch(function () {
                return (ctrl.$pristine && angular.isUndefined(ctrl.$modelValue)) || scope.match === ctrl.$modelValue;
            }, function (currentValue) {
                    ctrl.$setValidity('match', currentValue);
                });
        }
    }
}
 