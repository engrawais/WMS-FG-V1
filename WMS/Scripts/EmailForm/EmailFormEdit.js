$(document).ready(function () {

    $("#CDept").hide();
    $("#CSec").hide();
    $("#CCat").hide()
    $("#CLoc").hide();
    $("#CSec").hide();
    $("#CType").hide();
    if ($('#CriteriaComp').val() == "C") {
        $("#CComp").show();
        $("#SecDept").show();
        $("#CLoc").hide();
    }
    else {
        $("#CLoc").show();
        $("#CComp").hide();
        $("#CDept").hide();
        $("#CSec").hide();
        $("#SecDept").hide();
    }
    if ($('#HasTypeOrCat').val() == "C") {
        $("#CCat").show();
        $("#CType").hide();
    }
    else if ($('#HasTypeOrCat').val() == "T") {
        $("#CCat").hide();
        $("#CType").show();
    }
    else {
        $("#CCat").hide();
        $("#CType").hide();
    }
    if ($('#CriteriaDepSec').val() == "S") {
        $("#CSec").show();
        $("#CDept").hide();
    }
    else if ($('#CriteriaDepSec').val() == "D") {
        $("#CSec").hide();
        $("#CDept").show();
    }
    else {
        $("#CSec").hide();
        $("#CDept").hide();
    }
    $('#CriteriaComp').change(function () {
        var test = $(this).val();
        if (test == 'C') {
            $("#CComp").show();
            $("#SecDept").show();
            $("#CLoc").hide();
        }
        if (test == 'L') {
            $("#CLoc").show();
            $("#CComp").hide();
            $("#CDept").hide();
            $("#CSec").hide();
            $("#SecDept").hide();

        }
    });
    $('#CriteriaDepSec').change(function () {
        var test = $(this).val();
        if (test == 'S') {

            $("#CSec").show();
            $("#CDept").hide();
        }
        if (test == 'D') {
            $("#CSec").hide();
            $("#CDept").show();
        }
        if (test == 'A') {
            $("#CSec").hide();
            $("#CDept").hide();
        }

    });

    $('#HasTypeOrCat').change(function () {
        var test = $(this).val();
        if (test == 'T') {
            $("#CCat").hide();
            $("#CType").show();
        }
        else if (test == "C") {
            $("#CCat").show();
            $("#CType").hide();
        }
        else {
            $("#CCat").hide();
            $("#CType").hide();
        }
    });

    $('#DepartmentID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetDepartment';
    $.getJSON(URL + '/1', function (data) {
        var selectedItemID = document.getElementById("selectedDeptIDHidden").value;
        var items;
        $.each(data, function (i, state) {

            if (state.Value == selectedItemID)
                items += "<option selected value='" + state.Value + "'>" + state.Text + "</option>";
            else
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#DepartmentID').html(items);
    });

    $('#TypID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetEmpType';
    $.getJSON(URL + '/1', function (data) {
        var selectedItemID = document.getElementById("selectedTypeIDHidden").value;
        var items;
        $.each(data, function (i, state) {

            if (state.Value == selectedItemID)
                items += "<option selected value='" + state.Value + "'>" + state.Text + "</option>";
            else
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#TypID').html(items);
    });

    $('#CatID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetCategory';
    $.getJSON(URL + '/1', function (data) {
        var selectedItemID = document.getElementById("selectedCatIDHidden").value;
        var items;
        $.each(data, function (i, state) {

            if (state.Value == selectedItemID)
                items += "<option selected value='" + state.Value + "'>" + state.Text + "</option>";
            else
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#CatID').html(items);
    });

    $('#SectionID').empty();
    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    var URL = '/EmailForm/GetSection';
    $.getJSON(URL + '/1', function (data) {
        var selectedItemID = document.getElementById("selectedSecIDHidden").value;
        var items;
        $.each(data, function (i, state) {

            if (state.Value == selectedItemID)
                items += "<option selected value='" + state.Value + "'>" + state.Text + "</option>";
            else
                items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
        });
        $('#SectionID').html(items);
    });


    //$('#CompanyID').change(function () {
    //    $('#DepartmentID').empty();
    //    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    //    var URL = '/EmailForm/GetDepartment';
    //    $.getJSON(URL + '/' + $('#CompanyID').val(), function (data) {
    //        var items;
    //        $.each(data, function (i, state) {
    //            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
    //            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
    //        });
    //        $('#DepartmentID').html(items);
    //    });
    //});
    //$('#CompanyID').change(function () {
    //    $('#TypID').empty();
    //    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    //    var URL = '/EmailForm/GetEmpType';
    //    $.getJSON(URL + '/' + $('#CompanyID').val(), function (data) {
    //        var items;
    //        $.each(data, function (i, state) {
    //            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
    //            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
    //        });
    //        $('#TypID').html(items);
    //    });
    //});

    //$('#CompanyID').change(function () {
    //    $('#SectionID').empty();
    //    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    //    var URL = '/EmailForm/GetSection';
    //    $.getJSON(URL + '/' + $('#CompanyID').val(), function (data) {
    //        var items;
    //        $.each(data, function (i, state) {
    //            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
    //            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
    //        });
    //        $('#SectionID').html(items);
    //    });

    //    // loading section if  select department changed/////////////////////////
    //    ////////////////////////////////////////////////////////////


    //    //$('#DepartmentID').change(function () {
    //    //    $('#SectionID').empty();
    //    //    //var URL = '/WMS/EditAttendance/CompanyIDJobCardList';
    //    //    var URL = '/EmailForm/GetDepartment';
    //    //    $.getJSON(URL + '/' + $('#DpartmentID').val(), function (data) {
    //    //        var items;
    //    //        $.each(data, function (i, state) {
    //    //            items += "<option value='" + state.Value + "'>" + state.Text + "</option>";
    //    //            // state.Value cannot contain ' character. We are OK because state.Value = cnt++;
    //    //        });
    //    //        $('#SectionID').html(items);
    //    //    });

    //});
});
