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
                var datetimeParts = value.split(" ");
                var dateParts = datetimeParts[0].split("-");
                var date = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
                return this.optional(element) || (date.getFullYear() == dateParts[0] && date.getMonth() + 1 == dateParts[1] && date.getDate() == dateParts[2]);
            },
            "Invalid date."
        );

        $.validator.addMethod(
            "datetime",
            function (value, element) {
                if (value !== "") {
                    var datetimeParts = value.split(" ");
                    var dateParts = datetimeParts[0].split("-");
                    var timeParts = datetimeParts[1].split(":");
                    var date = new Date(dateParts[0],
                        dateParts[1] - 1,
                        dateParts[2],
                        timeParts[0],
                        timeParts[1],
                        timeParts[2],
                        0);
                    return this.optional(element) ||
                    (date.getFullYear() == dateParts[0] &&
                        date.getMonth() + 1 == dateParts[1] &&
                        date.getDate() == dateParts[2] &&
                        date.getHours() == timeParts[0] &&
                        date.getMinutes() == timeParts[1] &&
                        date.getSeconds() == timeParts[2]);
                } else {
                    return this.optional(element);
                }
            },
            "Invalid date and time."
        );
    }
});