//Index page samna ata ha pora page
$(document).ready(function () {
    $("#EmpDetails").show();
    //#trigers id
    //it hides id cpl


    // $("#CPL").hide();

    //var URL = '/WMS/LeaveSettings/CPLList';
    $('#buttonId').click(function () {

        var companyid = document.getElementById("CompanyIDJobCard").value;
        var empNo = document.getElementById("JobEmpNo").value;
        var x = document.getElementById("JobDateFrom").value;
        var d = new Date(x);
        var Year = d.getFullYear();
        //var URL = '/WMS/EditAttendance/GetEmpInfo';
        var URL = '/EditAttendance/GetEmpInfo';
        $.getJSON(URL + '/' + empNo + "w" + companyid + "w" + Year, function (data) {
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
$(document).ready(function () {
    $('#buttonId2').click(function () {
        $("#EmpDetails").show();
        var companyid = document.getElementById("CompanyIDJobCard").value;
        var empNo = document.getElementById("JobEmpNo").value;
        var x = document.getElementById("JobDateFrom").value;
        var d = new Date(x);
        var Year = d.getFullYear();
        //var URL = '/WMS/EditAttendance/GetEmpInfo';
        var URL = '/EditAttendance/GetEmpInfoAtt';
        $.getJSON(URL + '/' + empNo + "w" + companyid + "w" + Year, function (data) {
            var values = data.split('@');
            if (values[0] != "") {
                document.getElementById("EName").value = values[0];
            }
            if (values[1] != "") {
                document.getElementById("EDesignation").value = values[1];
            }
            if (values[2] != "") {
                document.getElementById("ESection").value = values[2];
            }
            if (values[3] != "") {
                document.getElementById("EDOJ").value = values[3];
            }
            if (values[5] != "") {
                document.getElementById("EMPNO").value = values[5];
            }
            if (values[6] != "") {
                document.getElementById("EMPTYPE").value = values[6];
            }
            if (values[7] != "") {
                document.getElementById("EFName").value = values[7];
            }
            if (values[8] != "") {
                document.getElementById("ECELLNO").value = values[8];
            }
            if (values[9] != "") {
                document.getElementById("EPHNO").value = values[9];
            }
            if (values[10] != "") {
                document.getElementById("EADDR").value = values[10];
            }

            if (values[0] == "") {
                document.getElementById("EName").value = "";
            }
            if (values[1] == "") {
                document.getElementById("EDesignation").value = "";
            }
            if (values[2] == "") {
                document.getElementById("ESection").value = "";
            }
            if (values[3] == "") {
                document.getElementById("EDOJ").value = "";
            }
            if (values[5] == "") {
                document.getElementById("EMPNO").value = "";
            }
            if (values[6] == "") {
                document.getElementById("EMPTYPE").value = "";
            }
            if (values[7] == "") {
                document.getElementById("EFName").value = "";
            }
            if (values[8] == "") {
                document.getElementById("ECELLNO").value = "";
            }
            if (values[9] == "") {
                document.getElementById("EPHNO").value = "";
            }
            if (values[10] == "") {
                document.getElementById("EADDR").value = "";
            }


        });


    });
});