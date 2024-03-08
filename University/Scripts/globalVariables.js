let currentid = '';
function resetDate(x) {

    $("#" + x.id).datepicker({
        forceParse: false, todayHighlight: true, autoclose: "true", todayBtn: true
    }).keyup(function (e) {
        if (e.keyCode == 8 || e.keyCode == 46) {
            this.value = '';
            resetDate(this);

        }
    });
}