$(function () {
    if ($.validator && $.validator.unobtrusive) {
        $.validator.unobtrusive.adapters.addSingleVal("dateafter", "othername");

        $.validator.addMethod("dateafter", function (value, element, otherName) {
            var otherElement = $('#' + otherName);

            if (otherElement != null && otherElement.length === 1) {
                var otherDate = Date.parse(otherElement.val());
                var myDate = Date.parse(value);

                if (otherDate !== NaN && myDate !== NaN) {
                    if (myDate <= otherDate) {
                        return false;
                    }
                }
            }
            return true;
        });

        $.validator.unobtrusive.adapters.addSingleVal("isvaliddate");

        $.validator.addMethod("isvaliddate", function (value, element, otherName) {

            if(isNaN(Date.parse(value))){
                return false;
            }
            return true;
        });
    }
});