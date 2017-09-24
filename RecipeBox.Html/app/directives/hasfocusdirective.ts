module Directives {
    // directive to control input focus, source: http://www.emberex.com/programmatically-setting-focus-angularjs-way/
    export class HasFocus {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$timeout', '$rootScope'];
        constructor(timeout, rootScope) {
            this.restrict = 'A';
            this.rootScopeService = rootScope;
            this.scope = { focusValue: '=hasFocus' };
            this.timeoutService = timeout;
        }


        require: string;
        restrict: string;
        rootScopeService: ng.IRootScopeService;
        scope: any;
        timeoutService: ng.ITimeoutService;


        link(scope: any, element, attrs, ctrl) {
            scope.$watch("focusValue", function (currentValue, previousValue) {
                if (currentValue === true) {
                    element[0].focus();
                } else if (currentValue === false) {
                    element[0].blur();
                }

                // TODO: figure out how to "trigger" an intial setting to make ccurrent and previousValue differ
                //if (currentValue === true && !previousValue) {
                //    element[0].focus();
                //} else if (currentValue === false && previousValue) {
                //    element[0].blur();
                //}
            })
        }
    }
}
