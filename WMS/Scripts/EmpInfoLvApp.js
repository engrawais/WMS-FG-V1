﻿//Index page samna ata ha pora page
$(document).ready(function () {
    $("#EmpDetails").show();
    //#trigers id
    //it hides id cpl


    // $("#CPL").hide();

    //var URL = '/WMS/LeaveSettings/CPLList';
    $('#buttonId').click(function () {
     
        var companyid = document.getElementById("CompanyID").value;
        var empNo = document.getElementById("EmpNo").value;
        var x = document.getElementById("FromDate").value;
        var d = new Date(x);
        var Year = d.getFullYear();
        //var URL = '/WMS/LvApp/GetEmpInfo';
        var URL = '/LvApp/GetEmpInfoForLeaves';
        $.getJSON(URL + '/' + empNo + "w" + companyid+"w"+Year, function (data) {
            var values = data.split('@');
            document.getElementById("EName").value = values[0];
            document.getElementById("EDesignation").value = values[1];
            document.getElementById("ESection").value = values[2];
            document.getElementById("ECL").value = values[3];
            document.getElementById("EAL").value = values[4];
            document.getElementById("ESL").value = values[5];
            document.getElementById("ECPL").value = values[6];
            document.getElementById("EFName").value = values[7];
            document.getElementById("EDOB").value = values[8];
        });
    });
    

});