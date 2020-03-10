//$(document).ready(function () {

//    ////var URL = '/WMS/Emp/SectionList';
//    //var URL = '/Home/TestData';
//    //$.getJSON(URL, function (data) {
//    //    console.log(data);
//    //});


//});
$(document).ready(function () {

    $("#Option1").hide();
    $("#Option2").hide();
    $("#Option3").hide();
    $("#Option4").show();
    $("#doubleDutyDiv").hide();
    $("#BadliDiv").hide();
    $("#EmpDetails").hide();
    $("input[name$='cars']").click(function () {
        var test = $(this).val();
     
        if (test == "shift") {
            $("#EmpDetails").hide();
            $("#Option1").show();
            $("#Option2").hide();
            $("#Option3").hide();
            $("#Option4").hide();
        }
        if (test == "crew") {
            $("#EmpDetails").hide();
            $("#Option1").hide();
            $("#Option2").show();
            $("#Option3").hide();
            $("#Option4").hide();
        }
        if (test == "section") {
            $("#EmpDetails").hide();
            $("#Option1").hide();
            $("#Option2").hide();
            $("#Option3").show();
            $("#Option4").hide();
        }
        if (test == "employee") {
            $("#EmpDetails").show();
            $("#Option1").hide();
            $("#Option2").hide();
            $("#Option3").hide();
            $("#Option4").show();
        }
        var test = $('#JobCardType').val();
        if (test == '8') {
            $("#doubleDutyDiv").show(); $("#EmpDetails").hide();
        }
        if (test == '9') {
            $("#BadliDiv").show(); $("#EmpDetails").hide();
        }
    });
    $('#JobCardType').change(function () {
        var test = $(this).val();
        $("#doubleDutyDiv").hide();
        $("#BadliDiv").hide();
        if (test == '5') {
            $("#TimeIn").show();
        }
        if (test == '8') {
            $("#doubleDutyDiv").show();
        }
        if (test == '9') {
            $("#BadliDiv").show();
        }
    });

    $('#DesignationID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EditAttendance/CompanyIDJobCardList';
    $.getJSON(URL + '/' + $('#CompanyIDJobCard').val(), function (data) {
        var items;
        $.each(data, function (i, state) {
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#DesignationID').html(items);
    });


    $('#CompanyIDJobCard').change(function () {
        $('#DesignationID').empty();
        //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
        var URL = '/EditAttendance/CompanyIDJobCardList';
        $.getJSON(URL + '/' + $('#CompanyIDJobCard').val(), function (data) {
            var items;
            $.each(data, function (i, state) {
                    items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
                // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
            });
            $('#DesignationID').html(items);
        });
    });
});
