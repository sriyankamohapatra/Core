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

        $.validator.addMethod(
            "date",
            function (value, element) {
                var dateParts = value.split("-");
                var date = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
                return this.optional(element) || (date.getFullYear() == dateParts[0] && date.getMonth() + 1 == dateParts[1] && date.getDate() == dateParts[2]);
            },
            "Please enter a valid date."
        );
    }
});