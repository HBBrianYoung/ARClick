<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="arclick.Index" %>

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>A/R Click</title>
    <link href="styles/arclick.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script src="https://cdn.datatables.net/1.10.13/js/jquery.dataTables.min.js"></script>
</head>
<body>

    <asp:Label ID="behindvalues" runat="server"></asp:Label>

    <form id="form1" runat="server">
        <div>
            Enter Client Name or Hit Search to Return All: 
            <asp:TextBox runat="server" ID="participant"></asp:TextBox>
            <button id="get">Get ClientNames</button>
        </div>
        <div id="content">
            <table>
                <tr>
                    <td>
                        <select id="drpSections" runat="server"></select></td>
                    <td>
                        <select id="drpPracticeGroups" runat="server"></select></td>
                    <td>
                        <select id="drpOffices" runat="server"></select></td>
                </tr>
            </table>
            <table id="arclickTable" class="arclick stripe hover row-border">
                <thead>
                    <tr>
                        <th>Client</th>
                        <th>Name</th>
                        <th>Atty</th>
                        <th>Currency</th>
                        <th>AR 0-30</th>
                        <th>AR 31-60</th>
                        <th>AR 61-90</th>
                        <th>AR 91-120</th>
                        <th>AR 121-180</th>
                        <th>AR 180+</th>
                        <th>AR Total</th>
                        <th>Unallocated</th>
                        <th>GBNF</th>
                        <th>Trust</th>
                        <th>AR Note</th>
                        <th>WIP 0-60</th>
                        <th>WIP 60+</th>
                        <th>WIP Total</th>
                        <th>Total ICS</th>
                        <th>Section</th>
                        <th>Atty ID</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
        <input type="hidden" id="MyUserLevel" runat="server" />
        <input type="hidden" id="myAllowedSections" runat="server" />
        <input type="hidden" id="mySections" runat="server" />
        <input type="hidden" id="myUID" runat="server" />
        <input type="hidden" id="CurrentPerson" runat="server" />
        <input type="hidden" id="location" runat="server" />
    </form>
    <div id="EditNote" title="Note Edit" style="width: 600px">
        <form class="form-style-1" style="width: 600px">
            <ul>
                <li>
                    <label for="txtLastUpdated">Last Updated:</label><input type="text" id="txtLastUpdated" /></li>
                <li>
                    <label for="txtUpdatedBy">Updated By:</label><input type="text" id="txtUpdatedBy" readonly="true" /></li>
                <li>
                    <label for="taNote">Notes:</label><textarea id="taNote" rows="10" cols="60"></textarea></li>
                <li>
                    <label for="txtCurrency">Currency:</label><input type="text" id="txtCurrency" /></li>
                <li>
                    <input type="button" id="btnUpdate" class="ui-button ui-corner-all ui-widget" value="Enter" onclick="SaveNote()" /></li>
            </ul>
            <input type="hidden" id="NewNoteUpdater" runat="server" />
            <input type="hidden" id="inHidClientID" runat="server" />
            <input type="hidden" id="inHidSupervisingAttorney" runat="server" />
        </form>
    </div>
</body>
<script>

    function SaveNote() {
        $.ajax({
            type: "POST",
            url: "arclick_service.asmx/SaveDetailNotes",
            data: JSON.stringify({ inClientID: document.getElementById("inHidClientID").value, inSupervisingAttorney: document.getElementById("inHidSupervisingAttorney").value, inCurrency: document.getElementById("txtCurrency").value, inNote: document.getElementById("taNote").value, inUser: document.getElementById("NewNoteUpdater").value }),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                alert("Note Updated");
                $('#EditNote').dialog("close");
            },
            error: function (xhr, status, error) {
                alert("error! " + error)
            }
        });
        $('#EditNote').dialog("close");
    }


    $(document).ready(function () {
        $("#get").click(getAccountingData());
        $('#EditNote').dialog({ width: "600px", autoOpen: false });
    });


    var getAccountingData = function () {
        var table = $("#arclickTable").DataTable({
            "oLanguage": {
                "sZeroRecords": "No records to display",
                "sSearch": "Search on Client Name"
            },
            "aLengthMenu": [[20, 50, 100, 150, 250, 500, -1], [20, 50, 100, 150, 250, 500, "All"]],
            "iDisplayLength": 20,
            "bSortClasses": false,
            "bStateSave": false,
            "bPaginate": true,
            "bAutoWidth": false,
            "bProcessing": true,
            "bServerSide": true,
            "bDestroy": true,
            "sAjaxSource": "arclick_service.asmx/GetItems",
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "bDeferRender": true,
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "myUserLevel", "value": $("#MyUserLevel").val() });
                aoData.push({ "name": "myAllowedSections", "value": $("#myAllowedSections").val() });
                aoData.push({ "name": "mySections", "value": $("#mySections").val() });
                aoData.push({ "name": "UID", "value": $("#myUID").val() });
                aoData.push({ "name": "CurrentPerson", "value": $("#CurrentPerson").val() });
                aoData.push({ "name": "location", "value": $("#location").val() });
            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "contentType": "application/json; charset=utf-8",
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success":
                        function (msg) {
                            // preserve newlines, etc - use valid JSON
                            s = msg.d.replace(/\\n/g, "\\n")
                                            .replace(/\\'/g, "\\'")
                                            .replace(/\\"/g, '\\"')
                                            .replace(/\\&/g, "\\&")
                                            .replace(/\\r/g, "\\r")
                                            .replace(/\\t/g, "\\t")
                                            .replace(/\\b/g, "\\b")
                                            .replace(/\\f/g, "\\f");
                            // remove non-printable and other non-valid JSON chars
                            s = s.replace(/[\u0000-\u0019]+/g, "");

                            var json = jQuery.parseJSON(s);
                            fnCallback(json);
                            //$("#sampleTable").show();
                            //hide the last columns? 
                        }
                });
            }
        });

        //Open the Edit Notes Window
        $('#arclickTable tbody').on('dblclick', 'tr', function () {
            var d = table.row($(this).parents('tr')).data();

            //use ajax to call to get this information from the database
            //            GetNoteDetails

            //Populate the hidden fields with information from the datatable            
            document.getElementById("inHidClientID").value = $(this).find("td").eq(0).html();
            document.getElementById("inHidSupervisingAttorney").value = $(this).find("td").eq(2).html();

            $.ajax({
                type: "GET",
                datatype: "xml",
                url: "arclick_service.asmx/GetNoteDetails?inClientID=" + encodeURI($(this).find("td").eq(0).html()) + "&inSupervisingAttorney=" + encodeURI($(this).find("td").eq(2).html()) + "&inCurrency=" + encodeURI($(this).find("td").eq(3).html()) + "",
                //data: { inClientID: $(this).find("td").eq(0).html(), inSupervisingAttorneyName: $(this).find("td").eq(2).html(), inCurrency: $(this).find("td").eq(3).html() },
                success: function (data) {
                    $(data).find("noterec").each(function () {
                        document.getElementById("txtLastUpdated").value = $(this).find('LastUpdatedDate').text();
                        document.getElementById("taNote").innerHTML = $(this).find('note').text();
                        document.getElementById("txtUpdatedBy").value = $(this).find('lastupdatedby').text();
                        document.getElementById("txtCurrency").value = $(this).find('currency').text();
                    });
                    $('#EditNote').dialog("open");
                },
                error: function (xhr, status, error) {
                    alert("error! " + error)
                }
            });
        });

        var detailRows = [];

        //detail View Row
        $('#arclickTable tbody').on('click', 'tr', function () {

            // query to get the information for the details row  --- SelectARClickDetail
            //will need to call the API and pass the primary key data for the query
            //var d = table.row($(this).parents('tr')).data();

            var tr = $(this).closest('tr');
            var row = table.row(tr);
            var idx = $.inArray(tr.attr('id'), detailRows);
            if (row.child.isShown()) {
                tr.removeClass('details');
                row.child.hide();
                // Remove from the 'open' array
                detailRows.splice(idx, 1);
            }
            else {
                tr.addClass('details');

                var outresult = "";
                outresult += "<table id=\"subcontent\" class=\"stripe\"><tr><td><b>Matter</b></td><td><b>Desc</b></td><td><b>Invoice</b></td><td><b>Currency</b></td>";
                outresult += "<td><b>Amount</b></td><td><b>Date</b></td><td><b># Days</b></td><td><b>VatFlag</b></td></tr>";

                $.ajax({
                    type: "GET",
                    datatype: "json",
                    async: false,
                    url: "arclick_service.asmx/GetDetailRow?inClientID=" + $(this).find("td").eq(0).html() + "&inSupervisingAttorneyID=" + $(this).find("td").eq(20).html() + "&inCurrency=" + $(this).find("td").eq(3).html() + "",
                    success: function (data) {

                        $(data).find("detailrowrec").each(function () {
                            outresult += "<tr><td>" + $(this).find('matter').text() + "</td><td>" + $(this).find('MatterName').text() + "</td><td>" + $(this).find('Invoice').text() + "</td><td>" + $(this).find('Currency').text() + "</td>";
                            outresult += "<td> " + $(this).find('ARAmt').text() + " </td><td> " + $(this).find('InvoiceDate').text() + " </td><td> " + $(this).find('#Days').text() + " </td><td> " + $(this).find('VatFlag').text() + " </td></tr>";
                        });
                    },
                    error: function (xhr, status, error) {
                        outresult = "Error retrieving data: ";
                    }
                });
                outresult += "</table>";
                row.child(outresult).show();

                // Add to the 'open' array
                if (idx === -1) {
                    detailRows.push(tr.attr('id'));
                }
            }
        });
    }

</script>
</html>
