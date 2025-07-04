﻿Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Linq

Public Class frmMain

    Dim connHistory As New HashSet(Of String)
    Dim connpath As String = ""

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath)
        Me.CenterToScreen()
        If IO.File.Exists(".\last.txt") Then
            Try
                txtSource.LoadFile(".\last.txt", RichTextBoxStreamType.RichText)
            Catch ex As Exception
            End Try
        End If

        connpath = Path.Combine(Application.StartupPath, "connhistory")
        If File.Exists(connpath) Then
            connHistory = New HashSet(Of String)(File.ReadAllLines(connpath))
        End If
        txtSQLConnectionString.Items.AddRange(connHistory.ToArray)

        CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Sub DefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)
        Dim l4 As New List(Of String)

        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        l1.Add(<![CDATA[  
            @using LIBCORE.Models;
            @model MeterBrandModel

            @{
             ViewBag.Title = "MeterBrandModel ENTRIES";
            }
            
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/">Home</a></li>
        <li class="breadcrumb-item active" aria-current="page">MeterBrandModel</li>
    </ol>
</nav>

            <h2>@ViewBag.Title</h2>
            <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas rhoncus. </p>
        ]]>.Value.Replace("MeterBrandModel", modelName))

        l1.Add(<![CDATA[ <div class="card shadow-sm p-5"> ]]>.Value)

        l1.Add(<![CDATA[ <div class="text-center mb-3"> ]]>.Value)
        l1.Add(<![CDATA[  <h1 class="display-6">Products</h1> ]]>.Value.Replace("Products", modelName))
        l1.Add(<![CDATA[ </div> ]]>.Value)

        l1.Add(<![CDATA[ <form id="createForm"> ]]>.Value.Replace("createForm", modelName & "Form"))
        'l1.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower()
            Dim field = ch(2).Trim

            l2.Add(<![CDATA[  ID: $("#ID").val()]]>.Value.Replace("ID", field))

            If field <> "id" AndAlso (field.EndsWith("id") Or field.EndsWith("code")) AndAlso ddt.Contains("int") Then
                dp.Add(<![CDATA[ $('#brand').val(js['brand']).trigger('change'); ]]>.Value.Replace("brand", field))

                sl.Add(<![CDATA[
                var sel2brand = $('#brand').select2({
	                theme: "bootstrap-5",
	                //dropdownParent: $('#mymodal'),
	                placeholder: "Select brand",
	                allowClear: true
                });
                var newOption = new Option("", "", true, true);
                sel2brand.append(newOption).trigger('change');
                sel2brand.on('select2:open', function () {
				    document.querySelector('.select2-search__field').focus();
			    });
                ]]>.Value.Replace("brand", field).Replace("mymodal", $"{modelName}Modal"))

                sl2.Add(<![CDATA[ $('#brand').val(null).trigger('change'); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                dp.Add(<![CDATA[ if(js['brand']==1) { $('#brand').prop('checked','checked'); } ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                dp.Add(<![CDATA[ $('#brandPreview').attr('src', 'data:image/png;base64,' + js['brand']); ]]>.Value.Replace("brand", field))

                sl2.Add(<![CDATA[ $('#brandPreview').attr('src', 'https://place-hold.it/200x200?text=YOUR PHOTO'); ]]>.Value.Replace("brand", field))

            Else
                dp.Add(<![CDATA[ $('#brand').val(js['brand']); ]]>.Value.Replace("brand", field))
            End If

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l1.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
                l1.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l1.Add(<![CDATA[ <input type="hidden" id="id" name="id" value="-1"> ]]>.Value)
                Continue For
            End If

            If ddt.Contains("string") Then
                l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password", "pwd", "syspassword"
                        l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select
                l1.Add(<![CDATA[ </div> ]]>.Value)

            ElseIf ddt.Contains("bool") Then
                l1.Add(<![CDATA[ <div class="mb-3 form-check"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.CheckBoxFor(m => m.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-check-label" }) ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[ </div> ]]>.Value)

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.EndsWith("id") Or field.EndsWith("code") Then
                    l1.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l1.Add(<![CDATA[  @Html.DropDownListFor(m => m.brand, new SelectList(ViewBag.Brands, "id", "name"), new { @class = "form-select" }) ]]>.Value.Replace("brand", field))
                    l1.Add(<![CDATA[ </div> ]]>.Value)

                    'l4.Add(<![CDATA[ 
                    '    if ($('#brand').val() == '-1') {
                    '        $('#ajaxResponseError').text('please select a brand').show();
                    '        return;
                    '    }
                    '    ]]>.Value.Replace("brand", field))

                Else
                    l1.Add(<![CDATA[ <div class="mb-3 col-4"> ]]>.Value)
                    l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l1.Add(<![CDATA[ </div> ]]>.Value)

                End If


            ElseIf ddt.StartsWith("date") Then
                l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If
                l1.Add(<![CDATA[ </div> ]]>.Value)

            ElseIf ddt.StartsWith("byte[]") Then
                l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[  <img id="brandPreview" src="https://place-hold.it/500x500?text=" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[ </div> ]]>.Value)

                fl1.Add(<![CDATA[
                    $("#brand").change(function () {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            $('#brandPreview').attr('src', e.target.result);
                        };
                        reader.readAsDataURL(this.files[0]);
                    });]]>.Value.Replace("brand", field))

            End If

            l1.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))


        Next

        l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
        l1.Add(<![CDATA[  <input type="reset" class="btn btn-outline-secondary col-lg-2 col-3" value="Clear" /> ]]>.Value)
        l1.Add(<![CDATA[  <input type="submit" class="btn btn-primary col-lg-2 col-3 m-1" value="Save"/> ]]>.Value)
        l1.Add(<![CDATA[ </div> ]]>.Value)

        l1.Add(<![CDATA[</form> ]]>.Value)
        l1.Add(<![CDATA[</div> ]]>.Value)
        l1.Add("")
        l1.Add("")

        l1.Add(<![CDATA[
@section css {
	<!-- section rendered css -->
	<link href="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.css" rel="stylesheet">
	<!-- select2 -->
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" />
}

@section Scripts{
	<!-- section rendered script -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>
	<script src="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.js"></script>
	<!-- select2 -->
	<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.full.min.js"></script>

    <script>
        function fillProductModelForm(js) {
            <EDIT_VAL>
        }
        function initSelect2ProductModelForm() {
            <SELECT2>
        }
        $(document).ready(function () {
            //fillProductModelForm(js);
            initSelect2ProductModelForm();
            <FILE_PREVIEW>

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission

                if (!$(this).valid()) { return; } // check if current form is valid

                // DROPDOWN_REPLACEMENT

                // look for changes
                var gotchange = false;
                var origdata = $('#ProductModelFormBody').attr('data-js');
                if (origdata) {
                    origdata = JSON.parse($('#ProductModelFormBody').attr('data-js'));
                    for (var key in origdata) {
                        if (origdata.hasOwnProperty(key)) {
                            if ($('#id').val() == "-1") { gotchange = true; break; } // new so no need to check
                            if ($('#' + key).val() == null) { continue; } // skip undefined properties
                            console.log(key + ': ' + $('#' + key).val() + ' = ' + origdata[key]); // TODO:  dont forget to remove
                            if ($('#' + key).val() != origdata[key]) {
                                gotchange = true; break;
                            }
                        }
                    }
                    // dont post if nothing changed
                    if (!gotchange) {
                        $('#clse').click();
                        return; 
                    }
                }

                // clear out any errors first
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();

                // Make the AJAX POST request
                var formData = $(this).serialize(); // Get the form data
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    contentType: "application/x-www-form-urlencoded",
                    data: formData,
                    success: function (response) {
                        console.log("Response:", response['result']);
                        if (response['result'] != "OK") {
                            if (response['result'] == "duplicate") {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> Duplicate records found!').show();
                                return;
                            }
                            if (response['message']) {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> ' + response['message']).show();
                                return;
                            }
                        }
                        // success
                        $(".field-validation-error, .validation-summary-errors > ul").empty();
                        $('#ProductModelForm')[0].reset();
                        $('#clse').click();

                        // reload
                        if (response['result'] != 'no changes') {
                            ShowSwal('Success').then(()=>{
                                location.reload();
                            });
                        }

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
            });

        });
    </script>
}
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1))
        )



        '        l1.Add(<![CDATA[
        '@section Scripts{

        '    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
        '    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>

        '    <script>
        '        $(document).ready(function () {
        '            $("#ProductModelForm").on("submit", function (event) {
        '                event.preventDefault(); // Prevent the default form submission

        '                if (!$(this).valid()) { return; } // check if current form is valid

        '                var formData = {
        '                    Id: $('#Id').val()
        '                };

        '                // Make the AJAX POST request (JSON -> Model Binding)
        '                $.ajax({
        '                    type: "POST",
        '                    url: "../api/products/upsert",
        '                    contentType: "application/json",
        '                    data: JSON.stringify(formData),
        '                    success: function (response) {
        '                        $(".field-validation-error, .validation-summary-errors").empty();
        '                        $('#ProductModelForm')[0].reset();
        '                        console.log("Response:", response['result']);
        '                        alert('ok');
        '                    },
        '                    error: function (xhr, status, error) {
        '                        console.error("Error:", error);
        '                    }
        '                });

        '                // Make the AJAX POST request (application/x-www-form-urlencoded)
        '                $.ajax({
        '                    type: "POST",
        '                    url: "../api/products/upsert2",
        '                    contentType: "application/x-www-form-urlencoded",
        '                    data: formData,
        '                    success: function (response) {
        '                        $(".field-validation-error, .validation-summary-errors").empty();
        '                        $('#ProductModelForm')[0].reset();
        '                        console.log("Response:", response['result']);
        '                        alert('ok');
        '                    },
        '                    error: function (xhr, status, error) {
        '                        console.error("Error:", error);
        '                    }
        '                });

        '            });
        '        });
        '    </script>
        '}
        ']]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("Id: $('#Id').val()", String.Join("," & vbCrLf, l2)))

        l1.Add("")

        'clear validation error and summary
        '$(".field-validation-error, .validation-summary-errors").empty();

        'reset the form
        '$('#ProductModelForm')[0].reset();

        txtDest.Text = String.Join(vbCrLf, l1)

    End Sub

    Private Sub ModalPopupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModalPopupToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)

        l1.Add(<![CDATA[
@using LIBCORE.Models;
@model MeterBrandModel

@{
 ViewBag.Title = "METER BRAND ENTRIES";
}

<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/">Home</a></li>
        <li class="breadcrumb-item active" aria-current="page">METER BRAND</li>
    </ol>
</nav>

<h2>@ViewBag.Title</h2>
<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas rhoncus. </p>
]]>.Value.Replace("MeterBrandModel", modelName).Replace("METER BRAND", modelName))

        l1.Add(<![CDATA[  ]]>.Value)

        l1.Add(<![CDATA[ <button id="btnCreateNew_mymodal" type="button" class="btn btn-primary mt-2 mb-2" data-bs-toggle="modal" data-bs-target="#mymodal"><i class="bi bi-plus-lg"></i> CREATE NEW</button> ]]>.Value.Replace("mymodal", $"{modelName}Modal"))
        l1.Add(<![CDATA[ <a class="btn btn-outline-secondary mb-3"> REFRESH </a> ]]>.Value)
        l1.Add(<![CDATA[ <a class="btn btn-outline-secondary mb-3"> OTHER FUNCTION </a> ]]>.Value)
        l1.Add(<![CDATA[  ]]>.Value)


        'l1.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)

        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim.ToLower

            ' TABLE

            lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))
            'lr.Add(<![CDATA[ <td>@item.brand (<i class="bi bi-pencil-square"></i> edit) </td> ]]>.Value.Replace("brand", field.ToLower))
            lr.Add(<![CDATA[ <td>@item.brand</td> ]]>.Value.Replace("brand", field.ToLower))

            If field <> "id" AndAlso (field.EndsWith("id") Or field.EndsWith("code")) AndAlso ddt.Contains("int") Then
                dp.Add(<![CDATA[ $('#brand').val(js['brand']).trigger('change'); ]]>.Value.Replace("brand", field))

                sl.Add(<![CDATA[
                var sel2brand = $('#brand').select2({
	                theme: "bootstrap-5",
	                dropdownParent: $('#mymodal'),
	                placeholder: "Select brand",
	                allowClear: true
                });
                var newOption = new Option("", "", true, true);
                sel2brand.append(newOption).trigger('change');
                sel2brand.on('select2:open', function () {
				    document.querySelector('.select2-search__field').focus();
			    });
                ]]>.Value.Replace("brand", field).Replace("mymodal", $"{modelName}Modal"))

                sl2.Add(<![CDATA[ $('#brand').val(null).trigger('change'); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                dp.Add(<![CDATA[ if(js['brand']==1) { $('#brand').prop('checked','checked'); } ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                dp.Add(<![CDATA[ $('#brandPreview').attr('src', 'data:image/png;base64,' + js['brand']); ]]>.Value.Replace("brand", field))

                sl2.Add(<![CDATA[ $('#brandPreview').attr('src', 'https://place-hold.it/200x200?text=YOUR PHOTO'); ]]>.Value.Replace("brand", field))

            Else
                dp.Add(<![CDATA[ $('#brand').val(js['brand']); ]]>.Value.Replace("brand", field))
            End If

            ' FORMS

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
                l3.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l3.Add(<![CDATA[ <input type="hidden" id="id" name="id" value="-1"> ]]>.Value)
                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))
                Continue For
            End If

            If ddt.Contains("string") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password", "pwd", "syspassword"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                l3.Add(<![CDATA[ <div class="mb-2 form-check"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.CheckBoxFor(m => m.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-check-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)
                '
                formData.Add(<![CDATA[ formData.append("brand", $('#brand').prop('checked')); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.EndsWith("id") Or field.EndsWith("code") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.brand, new SelectList(ViewBag.Brands, "id", "name"), new { @class = "form-select" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)

                    'l4.Add(<![CDATA[ 
                    '    if ($('#brand').val() == '-1') {
                    '        $('#ajaxResponseError').text('please select a brand').show();
                    '        return;
                    '    }
                    '    ]]>.Value.Replace("brand", field))

                Else
                    l3.Add(<![CDATA[ <div class="mb-2 col-4"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                End If

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("date") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  <img id="brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                fl1.Add(<![CDATA[
                    $("#brand").change(function () {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            $('#brandPreview').attr('src', e.target.result);
                        };
                        reader.readAsDataURL(this.files[0]);
                    });]]>.Value.Replace("brand", field))

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").prop('files')[0]); ]]>.Value.Replace("brand", field))

                gotFile = True

            End If


        Next

        ' TABLE
        l1.Add(<![CDATA[

<table id="mytable" class="table table-striped table-hover table-bordered table-responsive" role="grid" style="width:100%;font-size:12px">
    <thead class="bg-secondary text-white">
        <tr><TH_HEADER></tr>
    </thead>
    <tbody id="mytableRows">
        @if (ViewBag.MeterBrandModel != null)
        {
            @foreach (MeterBrandModel item in ViewBag.MeterBrandModel)
            {
                <tr class="editBtn" data-id="@item.id" data-js="@Json.Serialize(item).ToString()" data-bs-toggle="modal" data-bs-target="#mymodal">
                    <TD_DATA>
                </tr>
            }
        } 
    </tbody>
</table>

]]>.Value.Replace("mytable", $"{modelName}Table").Replace("ViewBag.Brands", $"ViewBag.{modelName}").Replace("MeterBrandModel", modelName).Replace("mymodal", $"{modelName}Modal").
    Replace("<TH_HEADER>", String.Join(vbCrLf, lh)).
    Replace("<TD_DATA>", String.Join(vbCrLf, lr)))

        ' MODAL FORM
        l1.Add(<![CDATA[ 

<div class="modal fade" id="mymodal" data-bs-focus="false" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modaltitle" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="modaltitle"><MODALTITLE></h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <form id="createForm">
                <div class="modal-body ms-2 me-2" data-js="" id="createFormBody">
<FORM CONTENT>
                </div>
                <div class="modal-footer">
                    <input type="reset" class="btn btn-outline-secondary col-lg-3 col-3" value="Clear" />
                    <button id="clse_mymodal" type="button" class="btn btn-secondary col-lg-3 col-3 m-1" data-bs-dismiss="modal">Close</button>
                    <input type="submit" class="btn btn-primary col-lg-4 col-4" value="Save changes"/>
                </div>
            </form>
        </div>
    </div>
</div>

]]>.Value.Replace("<MODALTITLE>", modelName).Replace("createForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM CONTENT>", String.Join(vbCrLf, l3)))

        l1.Add("")
        l1.Add("")

        Dim normalPost = <![CDATA[
                // Make the AJAX POST request
                var formData = $(this).serialize(); // Get the form data
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    contentType: "application/x-www-form-urlencoded",
                    data: formData,
                    success: function (response) {
                        console.log("Response:", response['result']);
                        if (response['result'] != "OK") {
                            if (response['result'] == "duplicate") {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> Duplicate records found!').show();
                                return;
                            }
                            if (response['message']) {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> ' + response['message']).show();
                                return;
                            }
                        }
                        // success
                        $(".field-validation-error, .validation-summary-errors > ul").empty();
                        $('#ProductModelForm')[0].reset();
                        $('#clse_mymodal').click();

                        // reload
                        if (response['result'] != 'no changes') {
                            ShowSwal('Success').then(()=>{
                                location.reload();
                            });
                        }

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")

        If gotFile Then
            normalPost = <![CDATA[
                // Make the AJAX POST request
                var formData = new FormData(); // Get the form data
                <FORM_DATA>
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    success: function (response) {
                        console.log("Response:", response['result']);
                        if (response['result'] != "OK") {
                            if (response['result'] == "duplicate") {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> Duplicate records found!').show();
                                return;
                            }
                            if (response['message']) {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> ' + response['message']).show();
                                return;
                            }
                        }
                        // success
                        $(".field-validation-error, .validation-summary-errors > ul").empty();
                        $('#ProductModelForm')[0].reset();
                        $('#clse_mymodal').click();

                        // reload
                        if (response['result'] != 'no changes') {
                            ShowSwal('Success').then(()=>{
                                location.reload();
                            });
                        }

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")
        End If

        l1.Add(<![CDATA[
@section css {
	<!-- section rendered css -->
	<link href="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.css" rel="stylesheet">
	<!-- select2 -->
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" />
}

@section Scripts{
	<!-- section rendered script -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>
	<script src="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.js"></script>
	<!-- select2 -->
	<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.full.min.js"></script>

    <script>
        function fillProductModelForm(js) {
            <EDIT_VAL>
        }
        function initSelect2ProductModelForm() {
            <SELECT2>
        }
        function initEventsmymodal() {
            // initialize datatable
            $('#mytable').DataTable({
                // ... other DataTable options
                // order: [[1, "desc"], [2, "asc"]], // index based
                stateSave: true,
                scrollX: true,
                // Event handler for row hover
                rowCallback: function (row, data, index) {
                    $(row).hover(function () {
                        $(this).css('cursor', 'pointer');
                    }, function () {
                        $(this).css('cursor', 'default');
                    });
                }
            });

            // on create new clicked
            $('#btnCreateNew_mymodal').on('click', function () {
                // $('#municipalities_id').prop('disabled', 'disabled');
            });

            // on edit clicked
            $('#mytable').on('click', '.editBtn', function () {
                var js = $(this).data('js');
                $('#ProductModelFormBody').attr('data-js', JSON.stringify(js));
                $('#ProductModelForm')[0].reset();
                fillProductModelForm(js);
            });

            // on modal shown 
            $('#mymodal').on('shown.bs.modal', function () {
                // $('#myInput').trigger('focus');
            });

            // on modal closed , clean all need fields
            $('#mymodal').on('hidden.bs.modal', function () {
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();
                $('#ProductModelFormBody').attr('data-js', '');
                $('#ProductModelForm')[0].reset();
                $('#id').val('-1');
                // generated content
                <SELECT2_MODIFIER>
            });
        }

        $(document).ready(function () {
            // check also hidden fields
            $("#ProductModelForm").data("validator").settings.ignore = "";

            initSelect2ProductModelForm();

            <FILE_PREVIEW>

            // ajax code
            // var provinces = [];
            // var municipalities = [];
            // var barangays = [];
            // $.ajax({
            //     url: "../api/locations",
            //     success: function (response) {
            //         if (response['result'] == "OK") {
            //             for (i = 0; i < response['provinces'].length; i++) {
            //                 provinces.push(response["provinces"][i]);
            //             }
            //             for (i = 0; i < response['municipalities'].length; i++) {
            //                 municipalities.push(response["municipalities"][i]);
            //             }
            //             for (i = 0; i < response['barangays'].length; i++) {
            //                 barangays.push(response["barangays"][i]);
            //             }
            //         }
            //     },
            //     error: function (xhr, status, error) {
            //         console.error("Error:", error);
            //     }
            // });

            initEventsmymodal();

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission

                if (!$(this).valid()) { return; } // check if current form is valid

                // DROPDOWN_REPLACEMENT

                // look for changes
                var gotchange = false;
                var origdata = $('#ProductModelFormBody').attr('data-js');
                if (origdata) {
                    origdata = JSON.parse($('#ProductModelFormBody').attr('data-js'));
                    for (var key in origdata) {
                        if (origdata.hasOwnProperty(key)) {
                            if ($('#id').val() == "-1") { gotchange = true; break; }  // new so no need to check
                            if ($('#' + key).val() == null) { continue; } // skip undefined properties
                            console.log(key + ': ' + $('#' + key).val() + ' = ' + origdata[key]); // TODO:  dont forget to remove
                            if ($('#' + key).val() != origdata[key]) {
                                gotchange = true; break;
                            }
                        }
                    }
                    // dont post if nothing changed
                    if (!gotchange) {
                        $('#clse_mymodal').click();
                        return; 
                    }
                }

                // clear out any errors first
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();

                // Make the AJAX POST request
                

            });

        });
    </script>
}
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("mymodal", $"{modelName}Modal").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
        Replace("mytable", $"{modelName}Table").
        Replace("ProductModelFormBody", $"{modelName}FormBody").
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
        Replace("// Make the AJAX POST request", normalPost)
        )

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1).Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2))

    End Sub

    Private Sub DataAccessBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DataAccessBuilderToolStripMenuItem.Click


        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        If frmTableName.ShowDialog <> DialogResult.OK Then
            Return
        End If

        If String.IsNullOrWhiteSpace(frmTableName.txtTableName.Text) Then
            Return
        End If

        Dim tableName = frmTableName.txtTableName.Text.Trim

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)
        Dim l3 As New List(Of String)

        For i = 0 To props.Count - 1

            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower()
            Dim field = ch(2).Trim

            If ddt.Contains("string") Then
                l2.Add(<![CDATA[ name = dr["name"].ToString() ]]>.Value.Replace("name", field))

            ElseIf ddt.Contains("bool") Then
                l2.Add(<![CDATA[ name = bool.Parse(dr["name"].ToString()) ]]>.Value.Replace("name", field))

            ElseIf ddt.Contains("int") Then
                l2.Add(<![CDATA[ name = dr["name"].ParseInt() ]]>.Value.Replace("name", field))

            ElseIf ddt.Contains("decimal") Or ddt.Contains("double") Then
                l2.Add(<![CDATA[ name = dr["name"].ParseDecimal() ]]>.Value.Replace("name", field))

            ElseIf ddt.StartsWith("dateonly") Then
                l2.Add(<![CDATA[ name = dr["name"].ParseDateOnly() ]]>.Value.Replace("name", field))

            ElseIf ddt.StartsWith("date") Then
                l2.Add(<![CDATA[ name = dr["name"].ParseDateOnly() ]]>.Value.Replace("name", field))

            ElseIf ddt.StartsWith("time") Then
                l2.Add(<![CDATA[ name = dr["name"].ParseDateOnly() ]]>.Value.Replace("name", field))

            End If

            If field.ToLower = "id" Then Continue For
            l3.Add(<![CDATA[ param.Add("name", model.name); ]]>.Value.Replace("name", field))

        Next


        l1.Add(<![CDATA[  
using TARONEAPI.JS.Utils;
using System.Data;
using TARONEAPI.Models;

public class TownModelDataAccess
    {
        private OleDB DB;
        public TownModelDataAccess(string conn)
        {
            DB = new OleDB(conn);
        }

		/// <summary>
		/// return list of objects
		/// </summary>
        /// <param name="q"></param>
		/// <returns></returns>
        public List<TownModel> List(string q = "")
        {
            List<TownModel> list = new List<TownModel>();
            DataTable dt = DB.ExecuteToDatatable("SELECT * FROM b_towns order by name asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TownModel model = HelperUtils.BindFrom<TownModel>(dr);
                    list.Add(model);
                }
            }

            return list;
        }

        /// <summary>
        /// returns single element from given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TownModel GetById(int id)
        {
            DataRow dr = DB.QuerySingleResult("SELECT * FROM b_towns WHERE id = " + id, null);
            if (dr != null)
            {
                return HelperUtils.BindFrom<TownModel>(dr);
            }

            return null;
        }

		/// <summary>
		/// perform insert/update
		/// if id value is <= 0 , it will perform insert
		/// if id value is > 0 , it will perform update
		/// </summary>
		/// <param name="model"></param>
		/// <returns>positive number is success, negative means failed</returns>
        public int Upsert(ref TownModel model)
        {
            if (string.IsNullOrWhiteSpace(model.name))
            {
                return -1;
            }
            // for insert
            if (model.id<=0)
            {
                // check for dups
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["code"] = model.code;

                DataRow exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE code=?", param);
                if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                if (exists != null) { return OleDB.DUPLICATE; }

                // prepare 
                param = Utils.HelperUtils.ToDictionary(model);
                param["statuslvl_text"] = model.statuslvl_text;
                param["statuslvl"] = model.statuslvl;
                param["madebyid"] = model.updatedbyid;
                param["madedate"] = model.lastupdated.Value;
                param["updatedbyid"] = model.updatedbyid;
                param["lastupdated"] = model.lastupdated.Value;

                // insert
                var resId = DB.InsertParam("b_towns", param, true);
                if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                if (resId > 0)
                {
                    // return updated model (with id)
                    model = GetById(resId);
                }
                return resId;

            } else
            {
                // for updating

                // verify id
                var orig = GetById(model.id);
                if (orig != null && orig.id > 0)
                {
                    // check if status is allowed for updating 
                    if (orig.statuslvl > HelperUtils.STATUS_LEVEL.LEVEL_1)
                    {
                        return OleDB.NO_CHANGES;
                    }
                    // return if nothing to update
                    if (HelperUtils.IsModelClean(model, orig))
                    {
                        return OleDB.NO_CHANGES;
                    }

                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["code"] = model.code;

                    // check for duplicate
                    DataRow exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE code=? AND id <> {model.id}", param);
                    if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                    if (exists != null) { return OleDB.DUPLICATE; }

                    // prepare 
                    param = Utils.HelperUtils.ToDictionary(model);
                    param.Remove("madedate"); // NOT NEEDED for update
                    param.Remove("madebyid"); // NOT NEEDED for update
                    param.Remove("statuslvl"); // NOT NEEDED for update
                    param.Remove("statuslvl_text"); // NOT NEEDED for update
                    param["updatedbyid"] = model.updatedbyid;
                    param["lastupdated"] = model.lastupdated.Value;

                    // update
                    var resId = DB.UpdateParam("b_towns", $"WHERE id={model.id}", param);
                    if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                    if (resId > 0)
                    {
                        // return updated model (with id)
                        model = GetById(model.id);
                    }
                    return resId;

                }
            }

            return -1;
        }

        /// <summary>
        /// perform status update
        /// if id value is <= 0 , it will perform insert
        /// if id value is > 0 , it will perform update
        /// </summary>
        /// <param name="model"></param>
        /// <returns>positive number is success, negative means failed</returns>
        public int status_update(ref TownModel model)
        {
            if (model.id <= 0)
            {
                return -1;
            }

            var orig = GetById(model.id);
            if (orig != null && orig.id > 0)
            {
                // prepare 
                var param = new Dictionary<string, object>();
                param["statuslvl_text"] = model.statuslvl_text;
                param["statuslvl"] = model.statuslvl;
                param["updatedbyid"] = model.updatedbyid;
                param["lastupdated"] = model.lastupdated.Value;

                // update
                var resId = DB.UpdateParam("b_towns", $"WHERE id={model.id}", param);
                if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                if (resId > 0)
                {
                    // return updated model (with id)
                    model = GetById(model.id);
                }
                return resId;
            }

            return -1;
        }

    }

]]>.Value.Replace("TownModel", modelName).
        Replace("b_towns", tableName).
        Replace("<MODEL_CONTENT>", String.Join("," & vbCrLf, l2)).
        Replace("<MODEL_CONTENT2>", String.Join(vbCrLf, l3))
        )

        txtDest.Text = String.Join(vbCrLf, l1)


    End Sub

    Private Sub ModelBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModelBuilderToolStripMenuItem.Click

        If txtSource.Text.ToLower.Contains("create table") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim tableName = Regex.Match(txtSource.Text, "\[dbo\].\[(.*?)\]", RegexOptions.IgnoreCase).Groups(1).Value
        If String.IsNullOrWhiteSpace(tableName) Then tableName = Regex.Match(txtSource.Text, "CREATE TABLE ""(.*?)"" ", RegexOptions.IgnoreCase).Groups(1).Value
        If String.IsNullOrWhiteSpace(tableName) Then tableName = Regex.Match(txtSource.Text, "CREATE TABLE \[(.*?)\] ", RegexOptions.IgnoreCase).Groups(1).Value

        'Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        'Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        tableName = StrConv(tableName, VbStrConv.ProperCase)

        Dim l1 As New List(Of String)
        l1.Add("public class TownModel {".Replace("Town", tableName))
        l1.Add("")
        For i = 1 To txtSource.Lines.Count - 1

            '[comid] [varchar](10)
            '[birthdate] [date] NULL,
            '[age] [Decimal](18, 2) NULL,

            Dim mm = Regex.Match(txtSource.Lines(i).Trim, "\[(.*?)\] \[(.*?)\](:?\((.*?)\))?")
            If mm.Success = False Then mm = Regex.Match(txtSource.Lines(i).Trim, "\[(.*?)\]   ")
            'If mm.Success = False Then mm = Regex.Match(txtSource.Lines(i).Trim, """(.*?)"" (.*?)(:?\((.*?)\))?")
            If mm.Success = False Then Continue For

            Dim ddFieldOrig = mm.Groups(1).Value.Trim
            Dim ddField = Regex.Replace(mm.Groups(1).Value.Trim, "[^a-z0-9]", "_", RegexOptions.IgnoreCase)
            Dim ddType = mm.Groups(2).Value.Trim.ToLower
            Dim ddRanged = mm.Groups(4).Value.Trim

            Dim l2 As New List(Of String)

            If ddType.Contains("char") Or ddType.Contains("text") Then
                If String.IsNullOrWhiteSpace(ddRanged) = False AndAlso ddRanged.Contains("max") = False Then
                    l2.Add(<![CDATA[ [MaxLength(10)] ]]>.Value.Replace("10", ddRanged))
                End If
                If (ddType.Contains("text")) Then
                    l2.Add(<![CDATA[ // prefer using NVARCHA(MAX) ]]>.Value.Trim)
                End If
                l2.Add(<![CDATA[ public string name { get; set; } ]]>.Value.Replace("name", ddField))

            ElseIf ddType.Contains("int") Then
                If String.IsNullOrWhiteSpace(ddRanged) = False Then
                    'l2.Add(<![CDATA[ [Range(1,0)] ]]>.Value.Replace("10", ddRanged))
                End If
                l2.Add(<![CDATA[ public int name { get; set; } ]]>.Value.Replace("name", ddField))

            ElseIf ddType.Contains("date") Then
                'If ddType.ToLower = "date" Then
                '    l2.Add(<![CDATA[ public DateOnly? name { get; set; } ]]>.Value.Replace("name", ddField))
                'ElseIf ddType.ToLower = "time" Then
                '    l2.Add(<![CDATA[ public TimeOnly? name { get; set; } ]]>.Value.Replace("name", ddField))
                'Else
                '    l2.Add(<![CDATA[ public DateTime? name { get; set; } ]]>.Value.Replace("name", ddField))
                'End If

                l2.Add(<![CDATA[ public DateTime? name { get; set; } ]]>.Value.Replace("name", ddField))

            ElseIf ddType.Contains("numeric") Or ddType.Contains("decimal") Or ddType.Contains("float") Or ddType.Contains("real") Then
                l2.Add(<![CDATA[ public decimal name { get; set; } ]]>.Value.Replace("name", ddField))

            ElseIf ddType.Contains("bit") Then
                l2.Add(<![CDATA[ public bool name { get; set; } ]]>.Value.Replace("name", ddField))

            ElseIf ddType.Contains("binary") Or ddType.Contains("varbinary") Then
                l2.Add(<![CDATA[ public byte[]? name { get; set; } ]]>.Value.Replace("name", ddField))

            End If

            l1.Add(String.Join(vbCrLf, l2))
            l1.Add("")
        Next

        l1.Add("}")

        Dim fullModel = <![CDATA[
public class TownModelFull : TownModel
{
    public string madebydisp { get; set; }
    public string madebyname { get; set; }
    public string updatedbydisp { get; set; }
    public string updatedbyname { get; set; }
}
        ]]>.Value.Replace("Town", tableName)

        txtDest.Text = String.Join(vbCrLf, l1) & vbCrLf & vbCrLf & fullModel




    End Sub

    Private Sub ModalPopup2TupleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModalPopup2TupleToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)

        l1.Add(<![CDATA[
@using LIBCORE.Models;
@model Tuple<MeterBrandModel>

@{
 ViewBag.Title = "METER BRAND ENTRIES";
}

<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/">Home</a></li>
        <li class="breadcrumb-item active" aria-current="page">METER BRAND</li>
    </ol>
</nav>

<h2>@ViewBag.Title</h2>
<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas rhoncus. </p>
]]>.Value.Replace("MeterBrandModel", modelName).Replace("METER BRAND", modelName))

        l1.Add(<![CDATA[  ]]>.Value)

        l1.Add(<![CDATA[ <button id="btnCreateNew_mymodal" type="button" class="btn btn-primary mt-2 mb-2" data-bs-toggle="modal" data-bs-target="#mymodal"><i class="bi bi-plus-lg"></i> CREATE NEW</button> ]]>.Value.Replace("mymodal", $"{modelName}Modal"))
        l1.Add(<![CDATA[ <a class="btn btn-outline-secondary mb-3"> REFRESH </a> ]]>.Value)
        l1.Add(<![CDATA[ <a class="btn btn-outline-secondary mb-3"> OTHER FUNCTION </a> ]]>.Value)
        l1.Add(<![CDATA[  ]]>.Value)


        'l1.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)

        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim.ToLower

            ' TABLE

            lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))
            'lr.Add(<![CDATA[ <td>@item.brand (<i class="bi bi-pencil-square"></i> edit) </td> ]]>.Value.Replace("brand", field.ToLower))
            lr.Add(<![CDATA[ <td>@item.brand</td> ]]>.Value.Replace("brand", field.ToLower))

            If field <> "id" AndAlso (field.EndsWith("id") Or field.EndsWith("code")) AndAlso ddt.Contains("int") Then
                dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']).trigger('change'); ]]>.Value.Replace("brand", field))

                sl.Add(<![CDATA[
                var sel2Item1_brand = $('#Item1_brand').select2({
	                theme: "bootstrap-5",
	                dropdownParent: $('#mymodal'),
	                placeholder: "Select brand",
	                allowClear: true
                });
                var newOption = new Option("", "", true, true);
                sel2Item1_brand.append(newOption).trigger('change');
                sel2Item1_brand.on('select2:open', function () {
				    document.querySelector('.select2-search__field').focus();
			    });
                ]]>.Value.Replace("brand", field).Replace("mymodal", $"{modelName}Modal"))

                sl2.Add(<![CDATA[ $('#Item1_brand').val(null).trigger('change'); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                dp.Add(<![CDATA[ if(js['brand']==1) { $('#Item1_brand').prop('checked','checked'); } ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                dp.Add(<![CDATA[ $('#Item1_brandPreview').attr('src', 'data:image/png;base64,' + js['brand']); ]]>.Value.Replace("brand", field))

                sl2.Add(<![CDATA[ $('#Item1_brandPreview').attr('src', 'https://place-hold.it/200x200?text=YOUR PHOTO'); ]]>.Value.Replace("brand", field))

            Else
                dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']); ]]>.Value.Replace("brand", field))
            End If

            ' FORMS

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
                l3.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l3.Add(<![CDATA[ <input type="hidden" id="Item1_id" name="Item1.id" value="-1"> ]]>.Value)
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                Continue For
            End If

            If ddt.Contains("string") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password", "pwd", "syspassword"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                l3.Add(<![CDATA[ <div class="mb-2 form-check"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.CheckBoxFor(m => m.Item1.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-check-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)
                '
                formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.EndsWith("id") Or field.EndsWith("code") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.Item1.brand, new SelectList(ViewBag.Brands, "id", "name"), new { @class = "form-select" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                    l3.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))

                    'l4.Add(<![CDATA[ 
                    '    if ($('#Item1_brand').val() == '-1') {
                    '        $('#ajaxResponseError').text('please select a brand').show();
                    '        return;
                    '    }
                    '    ]]>.Value.Replace("brand", field))

                Else
                    l3.Add(<![CDATA[ <div class="mb-2 col-4"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                End If

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("date") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  <img id="Item1_brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                fl1.Add(<![CDATA[
                    $("#Item1_brand").change(function () {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            $('#Item1_brandPreview').attr('src', e.target.result);
                        };
                        reader.readAsDataURL(this.files[0]);
                    });]]>.Value.Replace("brand", field))

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").prop('files')[0]); ]]>.Value.Replace("brand", field))

                gotFile = True

            End If

        Next

        gotFile = True ' tuple, so build ajax form

        ' TABLE
        l1.Add(<![CDATA[

<table id="mytable" class="table table-striped table-hover table-bordered table-responsive" role="grid" style="width:100%;font-size:12px">
    <thead class="bg-secondary text-white">
        <tr><TH_HEADER></tr>
    </thead>
    <tbody id="mytableRows" >
        @if (ViewBag.MeterBrandModel != null)
        {
            @foreach (MeterBrandModel item in ViewBag.MeterBrandModel)
            {
                <tr class="editBtn_mytable" data-id="@item.id" data-js="@Json.Serialize(item).ToString()" data-bs-toggle="modal" data-bs-target="#mymodal">
                    <TD_DATA>
                </tr>
            }
        } 
    </tbody>
</table>

]]>.Value.Replace("mytable", $"{modelName}Table").Replace("ViewBag.Brands", $"ViewBag.{modelName}").Replace("MeterBrandModel", modelName).Replace("mymodal", $"{modelName}Modal").
    Replace("<TH_HEADER>", String.Join(vbCrLf, lh)).
    Replace("<TD_DATA>", String.Join(vbCrLf, lr)))

        ' MODAL FORM
        l1.Add(<![CDATA[ 

<div class="modal fade" id="mymodal" data-bs-focus="false" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modaltitle" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="modaltitle"><MODALTITLE></h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <form id="createForm">
                <div class="modal-body ms-2 me-2" data-js="" id="createFormBody">
<FORM CONTENT>
                </div>
                <div class="modal-footer">
                    <input type="reset" class="btn btn-outline-secondary col-lg-3 col-3" value="Clear" />
                    <button id="clse_mymodal" type="button" class="btn btn-secondary col-lg-3 col-3 m-1" data-bs-dismiss="modal">Close</button>
                    <input type="submit" class="btn btn-primary col-lg-4 col-4" value="Save changes"/>
                </div>
            </form>
        </div>
    </div>
</div>

]]>.Value.Replace("<MODALTITLE>", modelName).Replace("createForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM CONTENT>", String.Join(vbCrLf, l3)))

        l1.Add("")
        l1.Add("")

        Dim normalPost = <![CDATA[
                // Make the AJAX POST request
                var formData = $(this).serialize(); // Get the form data
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    contentType: "application/x-www-form-urlencoded",
                    data: formData,
                    success: function (response) {
                        console.log("Response:", response['result']);
                        if (response['result'] != "OK") {
                            if (response['result'] == "duplicate") {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> Duplicate records found!').show();
                                return;
                            }
                            if (response['message']) {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> ' + response['message']).show();
                                return;
                            }
                        }
                        // success
                        $(".field-validation-error, .validation-summary-errors > ul").empty();
                        $('#ProductModelForm')[0].reset();
                        $('#clse_mymodal').click();

                        // reload
                        if (response['result'] != 'no changes') {
                            ShowSwal('Success').then(()=>{
                                location.reload();
                            });
                        }

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")

        If gotFile Then
            normalPost = <![CDATA[
                // Make the AJAX POST request
                var formData = new FormData(); // Get the form data
                <FORM_DATA>
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    success: function (response) {
                        console.log("Response:", response['result']);
                        if (response['result'] != "OK") {
                            if (response['result'] == "duplicate") {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> Duplicate records found!').show();
                                return;
                            }
                            if (response['message']) {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> ' + response['message']).show();
                                return;
                            }
                        }
                        // success
                        $(".field-validation-error, .validation-summary-errors > ul").empty();
                        $('#ProductModelForm')[0].reset();
                        $('#clse_mymodal').click();

                        // reload
                        if (response['result'] != 'no changes') {
                            ShowSwal('Success').then(()=>{
                                location.reload();
                            });
                        }

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")
        End If

        l1.Add(<![CDATA[
@section css {
	<!-- section rendered css -->
	<link href="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.css" rel="stylesheet">
	<!-- select2 -->
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" />
}

@section Scripts{
	<!-- section rendered script -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>
	<script src="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.js"></script>
	<!-- select2 -->
	<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.full.min.js"></script>

    <script>
        function fillProductModelForm(js) {
            <EDIT_VAL>
        }

        function initSelect2ProductModelForm() {
            <SELECT2>
        }

        function initEventsmymodal() {
            // initialize datatable
            $('#mytable').DataTable({
                // ... other DataTable options
                // order: [[1, "desc"], [2, "asc"]], // index based
                stateSave: true,
                scrollX: true,
                // Event handler for row hover
                rowCallback: function (row, data, index) {
                    $(row).hover(function () {
                        $(this).css('cursor', 'pointer');
                    }, function () {
                        $(this).css('cursor', 'default');
                    });
                }

            });

            // on create new clicked
            $('#btnCreateNew_mymodal').on('click', function () {
                // $('#Item1_municipalities_id').prop('disabled', 'disabled');
            });

            // on edit clicked
            $('#mytable').on('click', '.editBtn_mytable', function () {
                var js = $(this).data('js');
                $('#ProductModelFormBody').attr('data-js', JSON.stringify(js));
                $('#ProductModelForm')[0].reset();
                fillProductModelForm(js);                
            });

            // on modal shown 
            $('#mymodal').on('shown.bs.modal', function () {
                // $('#myInput').trigger('focus');
            });

            // on modal closed , clean all need fields
            $('#mymodal').on('hidden.bs.modal', function () {
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();
                $('#ProductModelFormBody').attr('data-js', '');
                $('#ProductModelForm')[0].reset();
                $('#Item1_id').val('-1');
                <SELECT2_MODIFIER>
            });
        }

        $(document).ready(function () {
            // check also hidden fields
            $("#ProductModelForm").data("validator").settings.ignore = "";

            initSelect2ProductModelForm();

            <FILE_PREVIEW>

            // ajax code
            // var provinces = [];
            // var municipalities = [];
            // var barangays = [];
            // $.ajax({
            //     url: "../api/locations",
            //     success: function (response) {
            //         if (response['result'] == "OK") {
            //             for (i = 0; i < response['provinces'].length; i++) {
            //                 provinces.push(response["provinces"][i]);
            //             }
            //             for (i = 0; i < response['municipalities'].length; i++) {
            //                 municipalities.push(response["municipalities"][i]);
            //             }
            //             for (i = 0; i < response['barangays'].length; i++) {
            //                 barangays.push(response["barangays"][i]);
            //             }
            //         }
            //     },
            //     error: function (xhr, status, error) {
            //         console.error("Error:", error);
            //     }
            // });

            initEventsmymodal();

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission

                if (!$(this).valid()) { return; } // check if current form is valid

                // DROPDOWN_REPLACEMENT

                // look for changes
                var gotchange = false;
                var origdata = $('#ProductModelFormBody').attr('data-js');
                if (origdata) {
                    origdata = JSON.parse($('#ProductModelFormBody').attr('data-js'));
                    for (var key in origdata) {
                        if (origdata.hasOwnProperty(key)) {
                            if ($('#Item1_id').val() == "-1") { gotchange = true; break; } // new so no need to check
                            if ($('#Item1_' + key).val() == null) { continue; } // skip undefined properties
                            console.log(key + ': ' + $('#Item1_' + key).val() + ' = ' + origdata[key]); // TODO:  dont forget to remove
                            if ($('#Item1_' + key).val() != origdata[key]) {
                                console.log("gotchange: " + key + " " + origdata[key]);
                                gotchange = true; break;
                            }
                        }
                    }
                    // dont post if nothing changed
                    if (!gotchange) {
                        $('#clse_mymodal').click();
                        return; 
                    }
                }

                // clear out any errors first
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();

                // Make the AJAX POST request
                

            });

        });
    </script>
}
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("mymodal", $"{modelName}Modal").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
        Replace("mytable", $"{modelName}Table").
        Replace("ProductModelFormBody", $"{modelName}FormBody").
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
        Replace("// Make the AJAX POST request", normalPost)
        )

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1).Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2)).Replace("Item1", $"{tupName}")

    End Sub

    Private Sub ToolStripButton2_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripButton2.ButtonClick

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        l1.Add(<![CDATA[ <div class="card shadow-sm p-5"> ]]>.Value)

        l1.Add(<![CDATA[ <div class="text-center mb-3"> ]]>.Value)
        l1.Add(<![CDATA[  <h1 class="display-6">Products</h1> ]]>.Value.Replace("Products", modelName))
        l1.Add(<![CDATA[ </div> ]]>.Value)

        l1.Add(<![CDATA[ <form id="createForm"> ]]>.Value.Replace("createForm", modelName & "Form"))
        l1.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower()
            Dim field = ch(2).Trim

            l2.Add(<![CDATA[  ID: $("#ID").val()]]>.Value.Replace("ID", field))

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l1.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l1.Add(<![CDATA[ <input type="hidden" id="id" name="id" value="-1"> ]]>.Value)
                Continue For
            End If

            If ddt.Contains("string") Then
                l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password"
                        l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select

            ElseIf ddt.Contains("bool") Then
                l1.Add(<![CDATA[ <div class="mb-3 form-check"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.CheckBoxFor(m => m.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[  <label asp-for="brand" class="form-check-label"></label> ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then
                l1.Add(<![CDATA[ <div class="mb-3 col-4"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("date") Then
                l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
                l1.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l1.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If

            End If

            l1.Add(<![CDATA[ </div> ]]>.Value)
            l1.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))


        Next

        l1.Add(<![CDATA[ <div class="mb-3"> ]]>.Value)
        l1.Add(<![CDATA[  <input type="reset" class="btn btn-outline-secondary col-lg-2 col-3" value="Clear" /> ]]>.Value)
        l1.Add(<![CDATA[  <input type="submit" class="btn btn-primary col-lg-2 col-3 m-1" value="Save"/> ]]>.Value)
        l1.Add(<![CDATA[ </div> ]]>.Value)

        l1.Add(<![CDATA[</form> ]]>.Value)
        l1.Add(<![CDATA[</div> ]]>.Value)
        l1.Add("")
        l1.Add("")

        l1.Add(<![CDATA[
@section Scripts{

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>

    <script>
        $(document).ready(function () {

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission

                if (!$(this).valid()) { return; } // check if current form is valid

                // DROPDOWN_REPLACEMENT

                // Make the AJAX POST request
                var formData = $(this).serialize(); // Get the form data
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    contentType: "application/x-www-form-urlencoded",
                    data: formData,
                    success: function (response) {
                        console.log("Response:", response['result']);
                        if (response['result'] != "OK") {
                            if (response['result'] == "duplicate") {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> Duplicate records found!').show();
                                return;
                            }
                            if (response['message']) {
                                $('#ajaxResponseError').text('').append('<i class="bi bi-exclamation-triangle-fill"></i> ' + response['message']).show();
                                return;
                            }
                        }
                        // success
                        $(".field-validation-error, .validation-summary-errors > ul").empty();
                        $('#ProductModelForm')[0].reset();
                        $('#clse').click();

                        // reload
                        if (response['result'] != 'no changes') {
                            ShowSwal('Success').then(()=>{
                                location.reload();
                            });
                        }

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
            });

        });
    </script>
}
]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("Id: $('#Id').val()", String.Join("," & vbCrLf, l2)))



        '        l1.Add(<![CDATA[
        '@section Scripts{

        '    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
        '    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>

        '    <script>
        '        $(document).ready(function () {
        '            $("#ProductModelForm").on("submit", function (event) {
        '                event.preventDefault(); // Prevent the default form submission

        '                if (!$(this).valid()) { return; } // check if current form is valid

        '                var formData = {
        '                    Id: $('#Id').val()
        '                };

        '                // Make the AJAX POST request (JSON -> Model Binding)
        '                $.ajax({
        '                    type: "POST",
        '                    url: "../api/products/upsert",
        '                    contentType: "application/json",
        '                    data: JSON.stringify(formData),
        '                    success: function (response) {
        '                        $(".field-validation-error, .validation-summary-errors").empty();
        '                        $('#ProductModelForm')[0].reset();
        '                        console.log("Response:", response['result']);
        '                        alert('ok');
        '                    },
        '                    error: function (xhr, status, error) {
        '                        console.error("Error:", error);
        '                    }
        '                });

        '                // Make the AJAX POST request (application/x-www-form-urlencoded)
        '                $.ajax({
        '                    type: "POST",
        '                    url: "api/products/upsert2",
        '                    contentType: "application/x-www-form-urlencoded",
        '                    data: formData,
        '                    success: function (response) {
        '                        $(".field-validation-error, .validation-summary-errors").empty();
        '                        $('#ProductModelForm')[0].reset();
        '                        console.log("Response:", response['result']);
        '                        alert('ok');
        '                    },
        '                    error: function (xhr, status, error) {
        '                        console.error("Error:", error);
        '                    }
        '                });

        '            });
        '        });
        '    </script>
        '}
        ']]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("Id: $('#Id').val()", String.Join("," & vbCrLf, l2)))

        l1.Add("")

        'clear validation error and summary
        '$(".field-validation-error, .validation-summary-errors").empty();

        'reset the form
        '$('#ProductModelForm')[0].reset();

        txtDest.Text = String.Join(vbCrLf, l1)

    End Sub

    Private Sub FormPOSTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormPOSTToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text


        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)


        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            If field.ToLower = "id" Then
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                Continue For
            End If

            If ddt.Contains("string") Then
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("date") Then
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").prop('files')[0]); ]]>.Value.Replace("brand", field))
                gotFile = True
            End If
        Next

        gotFile = True ' tuple, so build ajax form

        Dim normalPost = <![CDATA[
                // Make the AJAX POST request
                $(sender).addClass('btn-progress');
                var formData = $(this).serialize(); // Get the form data
                $.ajax({
                    type: "POST",
                    url: "/{Controller}/{Action}",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    complete: function (jqXHR, textStatus) {
                        setTimeout(() => {
                            $(sender).removeClass('btn-progress');
                        }, 300);
                    },
                    success: function (response, textStatus, jqXHR) {
                        var msg;
                        if (response.result == null) {
                            msg = response.toLowerCase();
                        } else {
                            msg = response.result.toLowerCase();
                        }
                        if (msg.includes("success")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                            reloadmytable(); // or dtmytable.ajax.reload(null,false)

                            swal("Saved!", "Record has been saved", "success");

                        } else if (msg.includes("nochange")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                        } else {
                            swal("Error", "An error occured: " + msg + "\n", "warning");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (jqXHR.status == 401) {
                            swal2({
                                title: `Unauthorized`,
                                html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                                icon: 'error',
                                dangerMode: true,
                                showCancelButton: false,
                            }, () => {
                                window.location = "/Authentication/Logout";
                            });
                            return;
                        }
                        swal("Error!", "Oops! something went wrong ... \n", "error");
                    }
                });
]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")

        If gotFile Then
            normalPost = <![CDATA[
                // Make the AJAX POST request
                $(sender).addClass('btn-progress');
                var formData = new FormData(); // Get the form data
                <FORM_DATA>
                $.ajax({
                    type: "POST",
                    url: "/{Controller}/{Action}",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    complete: function (jqXHR, textStatus) {
                        setTimeout(() => {
                            $(sender).removeClass('btn-progress');
                        }, 300);
                    },
                    success: function (response, textStatus, jqXHR) {
                        var msg;
                        if (response.result == null) {
                            msg = response.toLowerCase();
                        } else {
                            msg = response.result.toLowerCase();
                        }
                        if (msg.includes("success")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                            reloadmytable(); // or dtmytable.ajax.reload(null,false)

                            swal("Saved!", "Record has been saved", "success");

                        } else if (msg.includes("nochange")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                        } else {
                            swal("Error", "An error occured: " + msg + "\n", "warning");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (jqXHR.status == 401) {
                            swal2({
                                title: `Unauthorized`,
                                html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                                icon: 'error',
                                dangerMode: true,
                                showCancelButton: false,
                            }, () => {
                                window.location = "/Authentication/Logout";
                            });
                            return;
                        }
                        swal("Error!", "Oops! something went wrong ... \n", "error");
                    }
                });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")
        End If

        l1.Add(<![CDATA[
@section css {
	<!-- section rendered css -->
	<link href="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.css" rel="stylesheet">
	<!-- select2 -->
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" />
}

@section Scripts{
	<!-- section rendered script -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>
	<script src="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.js"></script>
	<!-- select2 -->
	<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.full.min.js"></script>

    <script>
        $(document).ready(function () {
            // check also hidden fields
            $("#ProductModelForm").data("validator").settings.ignore = "";

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission

                if (!$(this).valid()) { return; } // check if current form is valid

                // look for changes
                var gotchange = false;
                var origdata = $('#ProductModelFormBody').attr('data-js');
                if (origdata) {
                    origdata = JSON.parse($('#ProductModelFormBody').attr('data-js'));
                    for (var key in origdata) {
                        if (origdata.hasOwnProperty(key)) {
                            if ($('#Item1_id').val() == "-1") { gotchange = true; break; } // new so no need to check
                            if ($('#Item1_' + key).val() == null) { continue; } // skip undefined properties
                            console.log(key + ': ' + $('#Item1_' + key).val() + ' = ' + origdata[key]); // TODO:  dont forget to remove
                            if ($('#Item1_' + key).val() != origdata[key]) {
                                console.log("gotchange: " + key + " " + origdata[key]);
                                gotchange = true; break;
                            }
                        }
                    }
                    // dont post if nothing changed
                    if (!gotchange) {
                        $('#clse_mymodal').click();
                        return; 
                    }
                }

                // clear out any errors first
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();

                // Make the AJAX POST request

            });

        });
    </script>
}
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("mymodal", $"{modelName}Modal").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
        Replace("mytable", $"{modelName}Table").
        Replace("ProductModelFormBody", $"{modelName}FormBody").
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
        Replace("// Make the AJAX POST request", normalPost)
        )

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1).Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2)).Replace("Item1_", $"{tupName}")

    End Sub

    Private Sub DatatableGETToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatatableGETToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)


        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN

        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            l2.Add(<![CDATA[ "    <td>" + acc[i]['accountno'] + "</td>" + ]]>.Value.Replace("accountno", field))

        Next

        l1.Add(<![CDATA[

        function ReloadMemberAccountModelTable() {
            var id = $('#Item1_id').val();

            $('#MemberAccountModelTable').DataTable().destroy();
            $('#MemberAccountModelTableRows').empty(); // TODO: replace with correct tbody id

            $.ajax({
                url: "../api/cons-maintenance/membersaccount/" + id,
                success: function (response) {
                    if (response['result'] == "OK") {
                        var acc = response['accounts']; // TODO: replace with correct json response

                        for (i = 0; i < acc.length; i++) {
                            var cs = acc[i]['isprimary'] == 1 ? " bg-primary text-white" : '';
                            var markup = "<tr class='editBtn_MemberAccountModelTable" + cs + "' data-id='" + acc[i]['id'] + "' data-js='" + JSON.stringify(acc[i]) + "' data-bs-toggle='modal' data-bs-target='#MemberAccountModelModal'>" +
                                <TDS>
                                "</tr>";

                            $('#MemberAccountModelTableRows').append(markup); // TODO: replace with correct tbody id
                        }

                        // initialize datatable
                        $('#MemberAccountModelTable').DataTable().destroy();
                        $('#MemberAccountModelTable').DataTable({
                            // ... other DataTable options
                            // order: [[1, "desc"], [2, "asc"]], // index based
                            // scrollX: true,
                            stateSave: true,
                            // Event handler for row hover
                            rowCallback: function (row, data, index) {
                                $(row).hover(function () {
                                    $(this).css('cursor', 'pointer');
                                }, function () {
                                    $(this).css('cursor', 'default');
                                });
                            }
                        });

                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error:", error);
                }
            });
        }
]]>.Value.Trim)

        txtDest.Text = String.Join(vbCrLf, l1).Replace("<TDS>", String.Join(vbCrLf, l2)).
            Replace("MemberAccountModel", modelName)

    End Sub

    Private Sub BlankModalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlankModalToolStripMenuItem.Click

        If frmCreateModal.ShowDialog <> DialogResult.OK Then Return

        Dim modalName = frmCreateModal.cboItem.Text

        Dim l1 As New List(Of String)

        l1.Add(<![CDATA[ <button id="btnCreateNew_mymodal" type="button" class="btn btn-primary mt-2 mb-2" data-bs-toggle="modal" data-bs-target="#mymodal"> CREATE NEW </button> ]]>.Value.Trim)
        l1.Add(<![CDATA[
<div class="modal fade" id="mymodal" role="dialog" data-bs-focus="false" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modaltitle" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header bg-dark text-uppercase text-light p-3">
                <h5 class="modal-title"> </h5>
            </div>
            <div class="modal-body">
                
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-success"><span class="far fa-thumbs-up"></span> OK</button>
                <button type="button" class="btn btn-danger" onclick="closeFormIfDirty(this)"><span class="far fa-thumbs-down"></span> CANCEL</button>
            </div>
        </div>
    </div>
</div>
]]>.Value.Trim)

        txtDest.Text = String.Join(vbCrLf, l1).Replace("mymodal", modalName).Trim

    End Sub

    Private Sub HtmlTagHelpersOnlyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HtmlTagHelpersOnlyToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)

        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            If i = 0 Then
                l3.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
            End If

            If field.ToLower = "id" Then
                l3.Add(<![CDATA[ <input type="hidden" id="Item1_id" name="Item1.id" value="-1"> ]]>.Value)
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                Continue For
            End If

            If ddt.Contains("string") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password", "pwd", "syspassword"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select
                l3.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                l3.Add(<![CDATA[ <div class="mb-2 form-check"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.CheckBoxFor(m => m.Item1.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-check-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.EndsWith("id") Or field.EndsWith("code") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.Item1.brand, new SelectList(ViewBag.Brands, "id", "name"), new { @class = "form-select" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                    l3.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[ <div class="mb-2 col-4"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                End If

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("date") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If
                l3.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  <img id="Item1_brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)
                l3.Add(<![CDATA[ @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").prop('files')[0]); ]]>.Value.Replace("brand", field))

            End If
        Next

        Dim s1 = String.Join(vbCrLf, l3)
        If String.IsNullOrWhiteSpace(tupName) Then
            s1 = s1.Replace("m.Item1.", "m.")
        End If

        txtDest.Text = s1.Replace("Item1", $"{tupName}")

    End Sub

    Private Sub Select2AjaxToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Select2AjaxToolStripMenuItem.Click

        Dim ds = NewPageData()
        If IsNothing(ds) Then Return

        Dim modelName = ds(0)
        Dim props As List(Of String) = ds(1)
        Dim dialogId = ds(2)
        Dim tableId = ds(3)
        Dim formId = ds(4)

        Dim cols As New List(Of String)

        For i = 0 To props.Take(2).Count - 1
            Dim line = props(i).Trim
            If String.IsNullOrWhiteSpace(line) Then Continue For
            Dim arr = line.Split(" ")
            Dim type = arr(1).Trim.ToLower
            Dim fieldname = arr(2).Trim

            cols.Add(fieldname)
        Next


        txtDest.Text = <![CDATA[
<div class="form-group">
    <label>consumerid</label>
    <select class="form-control select2" style="width: 100%;" name="consumerid" id="cboconsumerid">
    </select>
</div>
        ]]>.Value.Trim.Replace("consumerid", modelName).Replace("idField", cols(0)).Replace("displayField", cols(1))

        Dim l1 As New List(Of String)
        l1.Add("** you can use this code if you have the createDropdownSelect function")
        l1.Add(<![CDATA[
            // select
            $(document).on('select2:open', `#cboconsumerid`, function (e) {
                document.querySelector('.select2-search__field').focus();
            });

            $(`#cboconsumerid`).select2({
                placeholder: {
                    id: '-1',               // the value of the option
                    text: 'Select option'
                },
                allowClear: true,
                // tags: true,              // for text drop down (non id field)
                // parent: $(`.modal`)      // 
            })

            let cboconsumerid = createDropdownSelect($(`select[name=consumerid]`), "/{controller}/{action}/", "idField", "displayField");

            $.when(
                ShowSwalLoader(),
                cboconsumerid.init()
            ).done(()=>{
                CloseSwalLoader()
            })

        ]]>.Value.Trim.Replace("consumerid", modelName).Replace("idField", cols(0)).Replace("displayField", cols(1)))

        l1.Add("")
        l1.Add("** or this one for more customization")
        l1.Add("")

        l1.Add(<![CDATA[                
function cboBankCodes(ele) {
    var ele = typeof (ele) == 'string' ? $(ele) : ele
    return $.ajax({
        url: "/{controller}/{action}/",
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        complete: function (jqXHR, textStatus) {

        },
        success: function (response, textStatus, jqXHR) {
            ele.empty()
            if (response != null && response.length > 0) {
                response.forEach((e) => {
                    var disp = `${e.name.trim()}}`
                    var opt = new Option(e.code.trim(), e.code.trim());
                    ele.append(opt);
                })
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {

        }
    });
} 
]]>.Value.Trim.Replace("BankCodes", modelName))

        l1.Add("")

        l1.Add(<![CDATA[ 
            function loadAjaxEmployeeCboList() {
                $('#employeeid').empty();
                $.ajax({
                    url: "../api/cons-maintenance/membersaccount/" + id,
                    success: function (response) {
                        if (response['result'] == "OK") {
                            var newOption = new Option("", "", true, true);
                            $('#employeeid').append(newOption).trigger('change');

                            var list = response['accounts']; // TODO: replace with correct json response
                            for (i = 0; i < list.length; i++) {
                                var text = list[i]["lname"] + ' ' + list[i]["fname"] + ' (' + list[i]["comid"] + ')';
                                newOption = new Option(text, list[i]['id'], false, false);
                                $('#employeeid').append(newOption);
                            }
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
            }        
]]>.Value.Trim)

        txtDest2.Text = String.Join(vbCrLf, l1)

    End Sub

    Private Sub Select2ViewBagToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Select2ViewBagToolStripMenuItem.Click

        Dim l1 As New List(Of String)
        l1.Add(<![CDATA[ 
        @{
            IEnumerable<EmployeeModel> employeeList = ViewBag.EmployeesModel;
        }

        function loadEmployeeCboList() {
            var list = @Html.Raw(Json.Serialize(employeeList));
            $('#employeeid').empty();

            var newOption = new Option("", "", true, true);
            $('#employeeid').append(newOption).trigger('change');

            for (i = 0; i < list.length; i++) {
                var text = list[i]["lname"] + ' ' + list[i]["fname"] + ' (' + list[i]["comid"] + ')';
                newOption = new Option(text, list[i]['id'], false, false);
                $('#employeeid').append(newOption);
            }
        }

]]>.Value.Trim)

        txtDest.Text = String.Join(vbCrLf, l1)

    End Sub

    Private Sub FormPOSTJSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FormPOSTJSToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text


        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)


        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            If field.ToLower = "id" Then
                formData.Add(<![CDATA[ brand: $("#Item1_brand").val() ]]>.Value.Replace("brand", field).TrimEnd)
                Continue For
            End If

            If ddt.Contains("string") Or ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then
                formData.Add(<![CDATA[ brand: $("#Item1_brand").val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("bool") Then
                formData.Add(<![CDATA[ brand: $('#Item1_brand').prop('checked') ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("date") Then
                formData.Add(<![CDATA[ brand: $("#Item1_brand").val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("byte[]") Then
                formData.Add(<![CDATA[ brand: $("#Item1_brand").prop('files')[0] ]]>.Value.Replace("brand", field).TrimEnd)
                gotFile = True
            End If

        Next

        gotFile = False ' tuple, so build ajax form

        Dim normalPost = <![CDATA[
                // Make the AJAX POST request
                $(sender).addClass('btn-progress');
                var model = {
                    <FORM_DATA>
                }
                $.ajax({
                    type: "POST",
                    url: "/{Controller}/{Action}",
                    data: JSON.stringify(model),
                    contentType: "application/json;charset=UTF-8",
                    dataType: "json",
                    complete: function (jqXHR, textStatus) {
                        setTimeout(() => {
                            $(sender).removeClass('btn-progress');
                        }, 300);
                    },
                    success: function (response, textStatus, jqXHR) {
                        var msg;
                        if (response.result == null) {
                            msg = response.toLowerCase();
                        } else {
                            msg = response.result.toLowerCase();
                        }
                        if (msg.includes("success")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                            reloadmytable(); // or dtmytable.ajax.reload(null,false)

                            swal("Saved!", "Record has been saved", "success");

                        } else if (msg.includes("nochange")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                        } else {
                            swal("Error", "An error occured: " + msg + "\n", "warning");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (jqXHR.status == 401) {
                            swal2({
                                title: `Unauthorized`,
                                html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                                icon: 'error',
                                dangerMode: true,
                                showCancelButton: false,
                            }, () => {
                                window.location = "/Authentication/Logout";
                            });
                            return;
                        }
                        swal("Error!", "Oops! something went wrong ... \n", "error");
                    }
                });
]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM_DATA>", String.Join("," & vbCrLf, formData))

        If gotFile Then
            normalPost = <![CDATA[
                // Make the AJAX POST request
                $(sender).addClass('btn-progress');
                var formData = new FormData(); // Get the form data
                <FORM_DATA>
                $.ajax({
                    type: "POST",
                    url: "/{Controller}/{Action}",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    complete: function (jqXHR, textStatus) {
                        setTimeout(() => {
                            $(sender).removeClass('btn-progress');
                        }, 300);
                    },
                    success: function (response, textStatus, jqXHR) {
                        var msg;
                        if (response.result == null) {
                            msg = response.toLowerCase();
                        } else {
                            msg = response.result.toLowerCase();
                        }
                        if (msg.includes("success")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                            reloadmytable(); // or dtmytable.ajax.reload(null,false)

                            swal("Saved!", "Record has been saved", "success");

                        } else if (msg.includes("nochange")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                        } else {
                            swal("Error", "An error occured: " + msg + "\n", "warning");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (jqXHR.status == 401) {
                            swal2({
                                title: `Unauthorized`,
                                html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                                icon: 'error',
                                dangerMode: true,
                                showCancelButton: false,
                            }, () => {
                                window.location = "/Authentication/Logout";
                            });
                            return;
                        }
                        swal("Error!", "Oops! something went wrong ... \n", "error");
                    }
                });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal")
        End If

        l1.Add(<![CDATA[
@section css {
	<!-- section rendered css -->
	<link href="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.css" rel="stylesheet">
	<!-- select2 -->
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
	<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" />
}

@section Scripts{
	<!-- section rendered script -->
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>
	<script src="https://cdn.datatables.net/v/bs5/dt-2.0.8/datatables.min.js"></script>
	<!-- select2 -->
	<script src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.full.min.js"></script>

    <script>
        $(document).ready(function () {
            // check also hidden fields
            $("#ProductModelForm").data("validator").settings.ignore = "";

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission

                if (!$(this).valid()) { return; } // check if current form is valid

                // look for changes
                var gotchange = false;
                var origdata = $('#ProductModelFormBody').attr('data-js');
                if (origdata) {
                    origdata = JSON.parse($('#ProductModelFormBody').attr('data-js'));
                    for (var key in origdata) {
                        if (origdata.hasOwnProperty(key)) {
                            if ($('#Item1_id').val() == "-1") { gotchange = true; break; } // new so no need to check
                            if ($('#Item1_' + key).val() == null) { continue; } // skip undefined properties
                            console.log(key + ': ' + $('#Item1_' + key).val() + ' = ' + origdata[key]); // TODO:  dont forget to remove
                            if ($('#Item1_' + key).val() != origdata[key]) {
                                console.log("gotchange: " + key + " " + origdata[key]);
                                gotchange = true; break;
                            }
                        }
                    }
                    // dont post if nothing changed
                    if (!gotchange) {
                        $('#clse_mymodal').click();
                        return; 
                    }
                }

                // clear out any errors first
                $(".field-validation-error, .validation-summary-errors > ul").empty();
                $('#ajaxResponseError').hide();

                // Make the AJAX POST request

            });

        });
    </script>
}
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("mymodal", $"{modelName}Modal").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
        Replace("mytable", $"{modelName}Table").
        Replace("ProductModelFormBody", $"{modelName}FormBody").
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
        Replace("// Make the AJAX POST request", normalPost)
        )

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1).Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2)).Replace("Item1_", $"{tupName}")

    End Sub

    Private Sub ModalPopupcjTemplateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModalPopupcjTemplateToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        modelName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)

        l1.Add(<![CDATA[
@model PABILCOLWHAC.Models.MeterBrandModel

@{
    ViewBag.Title = "MeterBrandModel";
    Layout = "~/Views/Shared/_tarunolayout.cshtml";
}

<script src="~/Content/js/pageJS/meterbrandmodel_v1.0.0.js"></script>

<h5 class="fw-bold text-uppercase">@ViewBag.Title</h5>
<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="@Url.Action("Dashboard", "Home")">Home</a></li>
        <li class="breadcrumb-item active" aria-current="page">@ViewBag.Title</li>
    </ol>
</nav>
<hr />

]]>.Value.Replace("MeterBrandModel", modelName).Replace("METER BRAND", modelName).Replace("meterbrandmodel_v1.0.0.js", $"{modelName.ToLower}_v1.0.0.js"))

        l1.Add(<![CDATA[  ]]>.Value)

        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False
        Dim formdata2 As New List(Of String)
        Dim dtColDef As New List(Of String)
        Dim colCount As Integer = 0

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            dtColDef.Add(<![CDATA[ { "data": "brand", "autoWidth": true } ]]>.Value.Replace("brand", field).TrimEnd)

            ' TABLE

            lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))
            'lr.Add(<![CDATA[ <td>@item.brand (<i class="bi bi-pencil-square"></i> edit) </td> ]]>.Value.Replace("brand", field.ToLower))
            lr.Add(<![CDATA[ <td>@item.brand</td> ]]>.Value.Replace("brand", field.ToLower))

            If field.ToLower <> "id" AndAlso (field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code")) AndAlso ddt.Contains("int") Then
                dp.Add(<![CDATA[ $('#brand').val(js['brand']).trigger('change'); ]]>.Value.Replace("brand", field))

                sl.Add(<![CDATA[
                function populatebrandCbo() {
                    $.ajax({
                        url: "/{Controller}/{Action}/",
                        type: "GET",
                        contentType: "application/json;charset=UTF-8",
                        dataType: "json",
                        success: function (result) {
                            $('#brand').empty();
                            $('#brand').append("<option value></option>");
                            for (var i = 0; i < result.data.length; i++) {
                                var Desc = result.data[i]['brand'];
                                var opt = new Option(Desc, result.data[i]['Id']);
                                $('#brand').append(opt);
                            }
                            var cbo = $('#brand').select2({
                                theme: "bootstrap-5",
                                dropdownParent: $('#mymodal'),
                                placeholder: "Select brand",
                                allowClear: true
                            });
                            cbo.on('select2:open', function () {
                                document.querySelector('.select2-search__field').focus();
                            });
                        },
                        error: function (errormessage) {
                            swal({
                                title: "Error!",
                                text: "Oops! something went wrong ... \n",
                                type: "error",
                                showCancelButton: false,
                                confirmButtonClass: "btn-danger",
                                confirmButtonText: "OK",
                                closeOnConfirm: false
                            });
                        }
                    });
                }
                ]]>.Value.Replace("brand", field).Replace("mymodal", $"{modelName}Modal"))

                sl2.Add(<![CDATA[ $('#brand').val(null).trigger('change'); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                dp.Add(<![CDATA[ if(js['brand']==1) { $('#brand').prop('checked','checked'); } ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                dp.Add(<![CDATA[ $('#brandPreview').attr('src', 'data:image/png;base64,' + js['brand']); ]]>.Value.Replace("brand", field))

                sl2.Add(<![CDATA[ $('#brandPreview').attr('src', 'https://place-hold.it/200x200?text=YOUR PHOTO'); ]]>.Value.Replace("brand", field))

            Else
                dp.Add(<![CDATA[ $('#brand').val(js['brand']); ]]>.Value.Replace("brand", field))
            End If

            ' FORMS

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
                'l3.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l3.Add(<![CDATA[ <input type="hidden" id="id" name="id" value="-1"> ]]>.Value)
                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#brand').val() ]]>.Value.Replace("brand", field).TrimEnd)
                Continue For
            End If

            If ddt.Contains("string") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password", "pwd", "syspassword"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("bool") Then
                l3.Add(<![CDATA[ <div class="mb-2 form-check"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.CheckBoxFor(m => m.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-check-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)
                '
                formData.Add(<![CDATA[ formData.append("brand", $('#brand').prop('checked')); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#brand').prop('checked') ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.brand, new SelectList(new List<string>(), "id", "name"), new { @class = "form-select" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)

                    'l4.Add(<![CDATA[ 
                    '    if ($('#brand').val() == '-1') {
                    '        $('#ajaxResponseError').text('please select a brand').show();
                    '        return;
                    '    }
                    '    ]]>.Value.Replace("brand", field))

                Else
                    l3.Add(<![CDATA[ <div class="mb-2 col-4"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                End If

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("date") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("byte[]") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  <img id="brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                fl1.Add(<![CDATA[
                    $("#brand").change(function () {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            $('#brandPreview').attr('src', e.target.result);
                        };
                        reader.readAsDataURL(this.files[0]);
                    });]]>.Value.Replace("brand", field))

                formData.Add(<![CDATA[ formData.append("brand", $("#brand").prop('files')[0]); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $("#brand").prop('files')[0] ]]>.Value.Replace("brand", field).TrimEnd)

                gotFile = True

            End If


        Next

        ' TABLE
        l1.Add(<![CDATA[
@*---------- Datatable -----------*@
<div class="row justify-content-lg-center" style="overflow-y:scroll;">
    <div class="col-6">
        <button class="btn btn-success" style="margin-bottom:10px;" onclick="showmymodal()"><span class="lni lni-circle-plus"></span> ADD </button>
        <table class="table table-bordered" id="mytable" style="width:100%">
            <thead class="bg-dark text-center text-uppercase text-light">
                <tr class="border-dark">
                    <TH_HEADER>
                    <th></th>
                </tr>
            </thead>
            <tbody></tbody>
        </table>
    </div>
</div>
]]>.Value.Replace("mytable", $"{modelName}Table").Replace("ViewBag.Brands", $"ViewBag.{modelName}").Replace("MeterBrandModel", modelName).Replace("mymodal", $"{modelName}Modal").
    Replace("<TH_HEADER>", String.Join(vbCrLf, lh)).
    Replace("<TD_DATA>", String.Join(vbCrLf, lr)).Trim)

        ' MODAL FORM
        l1.Add(<![CDATA[ 
@*---------- Modal-----------*@
<div class="modal fade" id="mymodal" data-bs-focus="false" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="mymodalTitle" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header bg-dark text-uppercase text-light">
                <h5 class="modal-title">ADD METER TYPE FORM</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <form id="ProductModelForm">
                <div class="modal-body">
                    <FORM CONTENT>
                </div>
                <div class="modal-footer">
                    <button type="submit" class="btn btn-success">SAVE</button>
                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal">CLOSE</button>
                </div>
            </form>
        </div>
    </div>
</div>
]]>.Value.Replace("<FORM CONTENT>", String.Join(vbCrLf, l3)).Replace("ProductModelForm", modelName & "Form"))

        l1.Add("")
        l1.Add("")

        Dim normalPost = <![CDATA[
    var model = {
        <FORM_DATA>
    }

    $.ajax({
        url: "/{Controller}/{Action}",
        data: JSON.stringify(model),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (response) {
            if (response.result === "Success") {
                $('#mymodal').modal('hide');
                loadmytable();
                swal({
                    title: "Saved!",
                    text: "Record has been saved." + "\n",
                    type: "success",
                    showCancelButton: false,
                    confirmButtonClass: "btn-success",
                    confirmButtonText: "OK",
                    closeOnConfirm: false
                },
                    function () {
                        window.location = "../{Controller}/{Action}";
                    }
                );

            } else {
                swal("Error", "An error occured: " + response.result + "\n", "warning");
            }
        },
        error: function (errormessage) {
            swal({
                title: "Error!",
                text: "Oops! something went wrong ... \n",
                type: "error",
                showCancelButton: false,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "OK",
                closeOnConfirm: false
            });
        }
    });

]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM_DATA>", String.Join("," & vbCrLf, formdata2).Trim).Replace("mytable", $"{modelName}Table")

        If gotFile Then
            normalPost = <![CDATA[
                // Make the AJAX POST request
                var formData = new FormData(); // Get the form data
                <FORM_DATA>
                $.ajax({
                    type: "POST",
                    url: "/{Controller}/{Action}",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    success: function (response) {
                        if (response.result === "Success") {
                            $('#mymodal').modal('hide');
                            loadmytable();
                            swal({
                                title: "Saved!",
                                text: "Record has been saved." + "\n",
                                type: "success",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "OK",
                                closeOnConfirm: false
                            },
                                function () {
                                    window.location = "../{Controller}/{Action}";
                                }
                            );

                        } else {
                            swal("Error", "An error occured: " + response.result + "\n", "warning");
                        }
                    },
                    error: function (errormessage) {
                        swal({
                            title: "Error!",
                            text: "Oops! something went wrong ... \n",
                            type: "error",
                            showCancelButton: false,
                            confirmButtonClass: "btn-danger",
                            confirmButtonText: "OK",
                            closeOnConfirm: false
                        });
                    }
                });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("mytable", $"{modelName}Table")
        End If

        l1.Add(<![CDATA[
    @section Scripts{
        <script>
            $(document).ready(initializeData());
        </script>
    } 

    @*---------- todo: save as separate script -----------*@
    <script>
        function fillProductModelForm(js) {
            <EDIT_VAL>
        }

        <SELECT2>

        function loadmytable() {
            $('#mytable').DataTable().destroy();
            $('#mytable').DataTable({
                stateSave: true,
                ajax: {
                    "url": "/{Controller}/{Action}",
                    "type": "GET",
                    datatype: "json"
                },
                pageLength: 10,
                initComplete: function (settings, json) {
                    document.body.style.cursor = 'default';
                },
                columns: [
                    <DT_COL_DEF>
                ],
                aoColumnDefs: [
                    {
                        "width": "100px",
                        "aTargets": [COL_DEF_COUNT],
                        "mData": "Id",
                        "mRender": function (data, type, full) {

                            return '<button class="btn btn-success btn-sm" style="font-size:smaller;" href="#" id="vw_' + data + '" ' +
                                'onclick="showEditModal(\'' + data + '\')">' +
                                '<span class="lni lni-pencil"></span> EDIT</button> ';

                        },
                        "className": "text-center"
                    }
                ]

            });
        }

        function showmymodal() {
            $(".field-validation-error, .validation-summary-errors > ul").empty();
            $('#ProductModelForm')[0].reset();
            $('#id').val('-1');
            <SELECT2_MODIFIER>
            $('.modal-title').text('ADD NEW');
            $('#mymodal').modal('show');
        }

        function showEditModal(id) {
        
            $.ajax({
                url: "/{Controller}/{Action}/?id=" + id,
                type: "GET",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                success: function (result) {
                    if (result.data != null) {
                        fillProductModelForm(result.data);
                        $('.modal-title').text('EDIT ');
                        $('#mymodal').modal('show');

                    } else {
                        swal({
                            title: "Error!",
                            text: "There are no details to display." + "\n",
                            type: "warning",
                            showCancelButton: false,
                            confirmButtonClass: "btn-danger",
                            confirmButtonText: "OK",
                            closeOnConfirm: false
                        });

                    }
                },
                error: function (errormessage) {
                    swal({
                        title: "Error!",
                        text: errormessage.responseText + "\n",
                        type: "error",
                        showCancelButton: false,
                        confirmButtonClass: "btn-danger",
                        confirmButtonText: "OK",
                        closeOnConfirm: false
                    });

                }
            });
        }

        function saveProductModelForm() {
            // Make the AJAX POST request
        }

        function initializeData() {
            // check also hidden fields
            $("#ProductModelForm").data("validator").settings.ignore = "";

            loadmytable();

            // on modal shown 
            $('#mymodal').on('shown.bs.modal', function () {
                $('#myInput').trigger('focus');
            });

            // on modal closed , clean all need fields
            $('#mymodal').on('hidden.bs.modal', function () {
                $(".field-validation-error, .validation-summary-errors > ul").empty(); // clear out any errors first
                $('#ProductModelFormBody').attr('data-js', '');
                $('#ProductModelForm')[0].reset();
                $('#id').val('-1');
                <SELECT2_MODIFIER>
            });

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission
                if (!$(this).valid()) { return; } // check if current form is valid
                $(".field-validation-error, .validation-summary-errors > ul").empty(); // clear out any errors first
                saveProductModelForm();
            });

        }
    </script>
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("mymodal", $"{modelName}Modal").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
        Replace("mytable", $"{modelName}Table").
        Replace("ProductModelFormBody", $"{modelName}FormBody").
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
        Replace("// Make the AJAX POST request", normalPost)
        )

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1).Replace("mymodal", $"{modelName}Modal").
            Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2)).
            Replace("<DT_COL_DEF>", String.Join("," & vbCrLf, dtColDef)).
            Replace("COL_DEF_COUNT", dtColDef.Count)

    End Sub

    Private Sub ToolStripButton1_ButtonClick(sender As Object, e As EventArgs)

    End Sub

    Private Sub ToolStripButton1_ButtonClick_1(sender As Object, e As EventArgs) Handles ToolStripButton1.ButtonClick
        If frmCreateApiController.ShowDialog() = DialogResult.OK Then
            txtDest.Text = frmCreateApiController.GeneratedCode
        End If
    End Sub

    Private Sub APIGeneratorcjTemplateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles APIGeneratorcjTemplateToolStripMenuItem.Click
        If frmCreateApiControllerCJ.ShowDialog() = DialogResult.OK Then
            txtDest.Text = frmCreateApiControllerCJ.GeneratedCode
        End If
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text

        modelName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)

        l1.Add(<![CDATA[
@model Tuple<TARONEAPI.Models.MeterBrandModel>

@{
    ViewBag.Title = "MeterBrandModel";
}
]]>.Value.Replace("MeterBrandModel", modelName).Replace("METER BRAND", modelName).Replace("meterbrandmodel_v1.0.0.js", $"{modelName.ToLower}_v1.0.0.js"))

        l1.Add(<![CDATA[  ]]>.Value)

        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False
        Dim formdata2 As New List(Of String)
        Dim dtColDef As New List(Of String)
        Dim colCount As Integer = 0
        Dim sl3 As New List(Of String)
        Dim l5 As New List(Of String) '
        Dim l6 As New List(Of String)

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            If {"statuslvl", "madebyid", "madedate", "lastupdated", "updatedbyid"}.Contains(field) Then
                If field.ToLower = "lastupdated" Then
                    dtColDef.Add(<![CDATA[ { "data": "brand", "autoWidth": true } ]]>.Value.Replace("brand", field).TrimEnd)
                    lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))
                End If
                Continue For
            End If

            dtColDef.Add(<![CDATA[ { "data": "brand", "autoWidth": true } ]]>.Value.Replace("brand", field).TrimEnd)

            ' TABLE

            lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))
            'lr.Add(<![CDATA[ <td>@item.brand (<i class="bi bi-pencil-square"></i> edit) </td> ]]>.Value.Replace("brand", field.ToLower))
            lr.Add(<![CDATA[ <td>@item.brand</td> ]]>.Value.Replace("brand", field.ToLower))

            If field.ToLower <> "id" AndAlso (field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code")) AndAlso ddt.Contains("int") Then
                dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']).attr('data-id', js['brandid']).trigger('change'); ]]>.Value.Replace("brand", field))

                sl.Add(<![CDATA[
                    var brandsData;
                    function populatebsSuggest_brand() {
                        if (brandsData) {
                            $("#Item1_brand").on('change', function () {
                                if ($(this).val().trim().length == 0) {
                                    $(this).attr('data-id', null);
                                }
                                if ($(this).is(":visible")) {
                                    $(this).closest('form').data('isDirty', true);
                                }
                            });
                        }

                        if (!brandsData) {
                            $.ajax({
                                url: "/{Controller}/{Action}/",
                                type: "GET",
                                contentType: "application/json;charset=UTF-8",
                                dataType: "json",
                                success: function (result) {
                                    $("#Item1_brand").bsSuggest("destroy")
                                    $("#Item1_brand").bsSuggest({
                                        url: null, // URL address for requesting data
                                        jsonp: null, // Set this parameter name to enable the jsonp function, otherwise use the json data structure
                                        data: {
                                            value: result.data
                                        },
                                        // for data-id value
                                        idField: 'id',
                                        // for input value , can also be set manually on onSetSelectValue event
                                        keyField: 'accounttype',
                                        // displayed fields
                                        effectiveFields: ['code', 'accounttype'],
                                        // displayed fields alias (header)
                                        effectiveFieldsAlias: {
                                            code: "CODE",
                                            accounttype: "DESCRIPTION"
                                        },
                                        //showHeader: true,
                                        ignorecase: true,
                                        hideOnSelect: true,
                                        autoSelect: false,
                                        clearable: false,
                                        listStyle: {
                                            "max-height": "300px",
                                            "max-width": "100%x"
                                        }

                                    }).on('onDataRequestSuccess', function (e, result) {
                                        
                                    }).on('onSetSelectValue', function (e, selectedData, selectedRawData) {
                                        $(this).closest('form').data('isDirty', true);

                                    }).on('onUnsetSelectValue', function () {
                                        
                                    }).on('onShowDropdown', function (e, data) {
                                        
                                    }).on('onHideDropdown', function (e, data) {
                                        
                                    });

                                },
                                error: function (errormessage) {
                                    swal("Error", "Oops! something went wrong ... \n", "error");
                                }
                            });
                        }
                    }
                ]]>.Value.Replace("brand", field).Replace("mymodal", $"{modelName}Modal"))

                sl2.Add(<![CDATA[ $('#Item1_brand').val(null).attr('data-id', null).trigger('change'); ]]>.Value.Replace("brand", field))
                l6.Add(<![CDATA[ setTimeout(populatebsSuggest_brand, 1); ]]>.Value.Replace("brand", field))

            ElseIf ddt.Contains("bool") Then
                dp.Add(<![CDATA[ if(js['brand']==1) { $('#Item1_brand').prop('checked','checked'); } ]]>.Value.Replace("brand", field))
                dp.Add(<![CDATA[ $('#Item1_brand').trigger('change'); ]]>.Value.Replace("brand", field))
                l5.Add(<![CDATA[ $('#Item1_brand').on('change', function () {
                            //console.log(Item1_brand.checked);
                        });
                    ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                dp.Add(<![CDATA[ $('#Item1_brandPreview').attr('src', 'data:image/png;base64,' + js['brand']); ]]>.Value.Replace("brand", field))

                sl2.Add(<![CDATA[ $('#Item1_brandPreview').attr('src', 'https://place-hold.it/200x200?text=YOUR PHOTO'); ]]>.Value.Replace("brand", field))

            Else
                dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']); ]]>.Value.Replace("brand", field))

                If i = 0 Then
                    dp.Add(<![CDATA[ $('#Item1_brand').closest('form').data('isDirty', false); ]]>.Value.Replace("brand", field))
                End If

            End If

            ' FORMS

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
                'l3.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l3.Add(<![CDATA[ <input type="hidden" id="Item1_id" name="Item1.id" value="-1"> ]]>.Value.Replace("Item1_id", $"Item1_{field}").Replace("Item1.id", $"Item1.{field}").Replace("Item1.", IIf(tupName.Length >= 0, "", "Item1.")))
                formData.Add(<![CDATA[ formData.append("brand", _.toNumber($("#Item1_brand").val())); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: _.toNumber($('#Item1_brand').val()) ]]>.Value.Replace("brand", field).TrimEnd)
                Continue For
            End If

            If ddt.Contains("string") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                Select Case field.ToLower
                    Case "email"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "email", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case "pass", "password", "pwd", "syspassword"
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "password", @class = "form-control" }) ]]>.Value.Replace("brand", field))

                    Case Else
                        l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @class = "form-control" }) ]]>.Value.Replace("brand", field))

                End Select
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("bool") Then
                l3.Add(<![CDATA[
                <div class="row">
                    <div class="col-lg-12">
                        <div class="pretty p-icon p-curve p-tada">
                            @Html.TextBoxFor(m => m.Item1.brand, new { @type = "checkbox" })
                            <div class="state p-primary-o">
                                <i class="icon material-icons">done</i>
                                @Html.LabelFor(m => m.Item1.brand)
                            </div>
                            @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" })
                        </div>
                    </div>
                </div>
                ]]>.Value.Replace("brand", field))

                '
                formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').prop('checked') ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[
                                @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" })
                                @Html.TextBoxFor(m => m.Item1.brand, new { @class = "form-control bsSuggest", @onfocus = "this.select();", @onmouseup = "return false;" })
                                <ul class="dropdown-menu-c1 dropdown-menu-right" role="menu">
                                </ul>
                                @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" })
                    ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)

                    'l4.Add(<![CDATA[ 
                    '    if ($('#brand').val() == '-1') {
                    '        $('#ajaxResponseError').text('please select a brand').show();
                    '        return;
                    '    }
                    '    ]]>.Value.Replace("brand", field))

                Else
                    l3.Add(<![CDATA[ <div class="mb-2 col-4"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                End If

                formData.Add(<![CDATA[ formData.append("brand", _.toNumber($("#Item1_brand").val())); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: _.toNumber($('#Item1_brand').val()) ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("date") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                End If
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("byte[]") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  <img id="brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                fl1.Add(<![CDATA[
                    $("#Item1_brand").change(function () {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            $('#Item1_brandPreview').attr('src', e.target.result);
                        };
                        reader.readAsDataURL(this.files[0]);
                    });]]>.Value.Replace("brand", field))

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").prop('files')[0]); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $("#Item1_brand").prop('files')[0] ]]>.Value.Replace("brand", field).TrimEnd)

                gotFile = True

            End If


        Next

        ' TABLE
        l1.Add(<![CDATA[
<div class="row">
    <div class="col-12">
        <div class="card card-condense">
            <div class="card-header" style="border-bottom: none;">
                <h4><span class="fas fa-tags fs-6"></span> JOB ORDER COMPLAINT</h4> (view, add, edit)
            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col-12">
        <div class="card card-condense">
            <div class="card-header" style="border-bottom: none;">
                <h4 data-collapse="#mycard-collapse"><span class="fas fa-search fs-6"></span> SEARCH</h4>
                <div class="card-header-action">
                    <a data-collapse="#mycard-collapse" class="btn btn-icon" href="#"><i class="fas fa-plus"></i></a>
                </div>
            </div>
            <div class="collapse" id="mycard-collapse">
                <div class="card-body">
                    
                </div>
                <div class="card-footer">
                    
                </div>
            </div>
        </div>
    </div>
</div>

@*---------- Datatable -----------*@
<div class="row">
    <div class="col-12">
        <div class="card">
            <div class="card-header">
                <h4>@ViewBag.Title</h4>
            </div>
            <div class="card-body">
                <button type="button" class="btn btn-icon icon-left btn-primary text-uppercase mb-3" onclick="showmymodal()"><span class="fas fa-plus-square"></span> Add New </button>
                <table class="table table-hover" id="mytable" style="width:100%">
                    <thead>
                        <tr>
                            <TH_HEADER>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>
</div>
]]>.Value.Replace("mytable", $"{modelName}Table").Replace("ViewBag.Brands", $"ViewBag.{modelName}").Replace("MeterBrandModel", modelName).Replace("mymodal", $"{modelName}Modal").
    Replace("<TH_HEADER>", String.Join(vbCrLf, lh)).
    Replace("<TD_DATA>", String.Join(vbCrLf, lr)).Trim)

        ' MODAL FORM
        l1.Add(<![CDATA[ 
@section popups{
    @* --- modals and popups goes here --- *@
    <div class="modal fade bg-opacity-75 modal2" id="mymodal" role="dialog" data-bs-focus="false" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="mymodalTitle" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-dark text-uppercase text-light p-3">
                    <h5 class="modal-title"> </h5>
                </div>
                <form id="ProductModelForm" autocomplete="off" dirty-checker>
                    <div class="modal-body">
                        <FORM CONTENT>
                    </div>
                    <div class="modal-footer">
                        <button id="btnSave_mymodal" type="button" class="btn btn-success"><span class="far fa-thumbs-up"></span> SAVE</button>
                        <button id="btnClose_mymodal" type="button" class="btn btn-danger" onclick="closeFormIfDirty(this)"><span class="far fa-thumbs-down"></span> CLOSE</button>
                    </div>
                </form>
            </div>
        </div>
    </div>    
}
@section css{
    @* --- stylesheet goes here --- *@
}
]]>.Value.Replace("<FORM CONTENT>", String.Join(vbCrLf, l3)).Replace("ProductModelForm", modelName & "Form"))

        l1.Add("")
        l1.Add("")

        Dim normalPost = <![CDATA[
    var model = {
        <FORM_DATA>
    }

    $.ajax({
        url: "/{Controller}/{Action}",
        data: JSON.stringify(model),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (response, textStatus, jqXHR) {
            var msg;
            if (response.result == null) {
                msg = response.toLowerCase();
            } else {
                msg = response.result.toLowerCase();
            }
            if (msg.includes("success")) {
                $('#mymodal').modal('hide');
                reloadmytable(); // or dtmytable.ajax.reload(null,false)
                swal("Saved!", "Record has been saved", "success");
               
            } else if (msg.includes("nochange")) {
                $('#mymodal').modal('hide');
            } else {
                swal("Error", "An error occured: " + msg + "\n", "warning");
           
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 429) {
                swal("Error!", errorThrown, "error");
                return;
            }
            swal("Error!", "Oops! something went wrong ... \n", "error");
        }
    });

]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM_DATA>", String.Join("," & vbCrLf, formdata2).Trim).Replace("mytable", $"{modelName}Table")

        If gotFile Then
            normalPost = <![CDATA[
                // Make the AJAX POST request
                var formData = new FormData(); // Get the form data
                <FORM_DATA>
                $.ajax({
                    type: "POST",
                    url: "/{Controller}/{Action}",
                    data: formData,
                    contentType: false, // Important for multipart form data
                    processData: false, // Don't process data automatically
                    success: function (response, textStatus, jqXHR) {
                        var msg;
                        if (response.result == null) {
                            msg = response.toLowerCase();
                        } else {
                            msg = response.result.toLowerCase();
                        }
                        if (msg.includes("success")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                            reloadmytable(); // or dtmytable.ajax.reload(null,false)

                            swal("Saved!", "Record has been saved", "success");                        

                        } else if (msg.includes("nochange")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                        } else {
                            swal("Error", "An error occured: " + msg + "\n", "warning");
               
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (jqXHR.status == 429) {
                            swal("Error!", errorThrown, "error");
                            return;
                        }
                        swal("Error!", "Oops! something went wrong ... \n", "error");
                    }
                });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("mytable", $"{modelName}Table")
        End If

        l1.Add(<![CDATA[
    @section scripts{
        @* --- additional scripts goes here --- *@
        <script>
            $(document).ready(function () {
                initializeData();
            });
        </script>
    } 

    @*---------- todo: save as separate script -----------*@
    <script>
        function initializeData() {
            InitValidator();
            initmytable();
            appendRequiredLabel();
            <SELECT_EVENTS>
            <CHECK_EVENTS>
            
            $('#mymodal').on('shown.bs.modal', function () {
                $('#Item1_myInput').trigger('focus');

            }).on('hidden.bs.modal', function () {
                selectedProductModelFormData=null;
                clearFormValidation();
                $('#ProductModelFormBody').attr('data-js', '');
                $('#ProductModelForm')[0].reset();
                $('#Item1_id').val('-1');
                <SELECT2_MODIFIER>
            });

            // on form submit
            $("#ProductModelForm").on("submit", function (event) {
                event.preventDefault(); // Prevent the default form submission
                // check if current form is valid
                if (!$(this).valid()) {
                    swal("Something went wrong!", "Please fill all required field to proceed.", "error");
                    return;
                } 
                clearFormValidation();
                saveProductModelForm();
            });
            
            $('#btnSave_mymodal').on('click', function () {
                $(this).closest('form').submit();
            })

            EnableDisableControls(false);
        }

        var dtmytable;
        var dtmytableData;
        function reloadmytable() {
            dtmytable.ajax.reload(function (json) {
                dtmytableData = json.data
                // add other function to be called after table reloads
            }, false)
        }
        function initmytable() {
            $('#mytable').DataTable().destroy();
            dtmytable = $('#mytable').DataTable({
                dom:
                    "<'row'<'p-2 col-sm-12 col-md-6 col-xl-6'l><'float-right pr-3 pt-3 p-2 col-sm-12 col-md-6 col-xl-6'f>>" +
                    "<'row'<'table-responsive col-sm-12'tr>>" +
                    "<'row'<'pl-2 pt-0 pb-2 col-sm-12 col-md-5'i><'pt-1 pb-1 pr-2 col-sm-12 col-md-7'p>>",
                stateSave: true,
                ajax: {
                    "url": "/{Controller}/{Action}",
                    "type": "GET",
                    datatype: "json",
                    error: function (errormessage) {
                        toastError("Error!", "Failed to load datatable ...")
                    }
                },
                pageLength: 5,
                order: [[1, "asc"]], // index based
                lengthMenu: [
                    [5, 10, 30, 50, -1],
                    [" 5", 10, 30, 50, "All"]
                ],
                autoWidth: true,
                initComplete: function (settings, json) {
                    dtmytableData = json.data;
                    document.body.style.cursor = 'default';
                    setTimeout(EnableDisableControls(true), 1);
                },
                columns: [
                    // data: , name: , orderable: , autoWidth: , width: , className: 'text-center' , "visible":false
                    <DT_COL_DEF>
                ],
                aoColumnDefs: [
                    {
                        "width": "50px",
                        "aTargets": [0], // target column
                        "bSortable":false,
                        "mRender": function (data, type, full, meta) {
                            return `<button class="btn btn-primary btn-sm btnRowEdit" style="font-size:smaller;" id="vw_${full.id}" data-id="${full.id}" onclick="showEditmymodal(${full.id})"> <span class="fas fa-edit"></span> VIEW</button> `;
                        },
                        "className": "text-center text-uppercase"
                    },
                    {
                        "width": "450px",
                        "aTargets": [-1],
                        "mRender": function (data, type, full, meta) {
                            return `
                            <div class="d-flex flex-row">
                                <div class="me-2">
                                    <small>
                                        <strong class="themefont text-uppercase">Created By:</strong><br/> ${full.madebyname} <br/>
                                        ${ToDateTime(full.madedate)}
                                    </small>
                                </div>
                                <div class="me-2">
                                    <small>
                                        <strong class="themefont text-uppercase">Updated By:</strong><br/> ${full.updatedbyname} <br/>
                                        ${ToDateTime(full.lastupdated)}
                                    </small>
                                </div>
                            </div>
                            `

                        },
                        "className": "text-uppercase"
                    }
                ]

            });
        }

        function showmymodal() {
            $(".field-validation-error, .validation-summary-errors > ul").empty();
            $('#ProductModelForm')[0].reset();
            $('#Item1_id').val('-1');
            <SELECT2_MODIFIER>
            $('#mymodal h5').text('ADD NEW DETAILS');
            $('#mymodal').modal('show');
        }

        function showEditmymodal(id) {
        
            $.ajax({
                url: "/{Controller}/{Action}/?id=" + id,
                type: "GET",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                success: function (result, textStatus, jqXHR) {
                    if (result.data != null) {
                        fillProductModelForm(result.data);
                        $('#mymodal h5').text('EDIT DETAILS');
                        $('#mymodal').modal('show');
                        $('#mymodal').find('form').data('isDirty', false);
                    } else {
                        swal("Error!", "There are no details to display.\n", "warning");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (jqXHR.status == 429) {
                        swal("Error!", errorThrown, "error");
                        return;
                    }
                    swal("Error!", "Oops! something went wrong ... \n", "error");
                }
            });
        }
        
        var selectedProductModelFormData;
        function fillProductModelForm(js) {
            selectedProductModelFormData = js;
            <EDIT_VAL>
        }
       
        function saveProductModelForm() {
            // Make the AJAX POST request
        }
        
        <SELECT2>

    </script>
]]>.Value.Replace("ProductModelForm", modelName & "Form").
        Replace("mymodal", $"{modelName}Modal").
        Replace("dtmytable", $"dt{modelName}").
        Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
        Replace("mytable", $"{modelName}Table").
        Replace("ProductModelFormBody", $"{modelName}FormBody").
        Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
        Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
        Replace("// Make the AJAX POST request", normalPost)
        )

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1).Replace("mymodal", $"{modelName}Modal").
            Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2)).
            Replace("<DT_COL_DEF>", String.Join("," & vbCrLf, dtColDef)).
            Replace("<SELECT_EVENTS>", String.Join(vbCrLf, l6).Trim).
            Replace("<CHECK_EVENTS>", String.Join(vbCrLf, l5).Trim).
            Replace("m.Item1.", $"{IIf(String.IsNullOrWhiteSpace(tupName) = False, $"m.{tupName}.", "m.")}").
            Replace("Item1_", $"{IIf(String.IsNullOrWhiteSpace(tupName) = False, $"{tupName}_", "")}")

    End Sub

    Private Sub TabsGeneratorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TabsGeneratorToolStripMenuItem.Click

        Dim no = Val(InputBox("Number of Tabs?", "Generate Tabs", "3"))
        If no <= 0 Then Return

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)
        Dim l3 As New List(Of String)

        For i = 1 To no

            Dim str = <![CDATA[<li class="nav-item">
                                <a class="nav-link active" data-bs-toggle="tab" href="#tabPane1" id="tab1"><span class="fas fa-info-circle"></span> Pane1</a>
                            </li>]]>.Value.Trim.Replace(" active", IIf(i > 1, "", " active")).Replace("Pane1", "Panel" & i).Replace("tab1", "tab" & i)
            l1.Add(str)

            str = <![CDATA[<div class="tab-pane container-fluid active" id="tabPane1">
                                <div class="shadow-sm p-3">
                                    <!-- content goes here -->
                                </div>
                            </div>]]>.Value.Trim.Replace(" active", IIf(i > 1, "", " active")).Replace("Pane1", "Panel " & i)
            l2.Add(str)

            str = <![CDATA[$('#tab1').on('shown.bs.tab', function (e) {
                                    // per tab scripts goes here
                            });]]>.Value.Trim.Replace("tab1", "tab" & i)
            l3.Add(str)

        Next

        l3.Add("
            // dont forget to set ; 
            // $('.nav-tabs a:first').tab('show');
        ")

        txtDest.Text = <![CDATA[<ul class="nav nav-tabs">
                            <!-- NAVS -->
                        </ul>

                        <div class="tab-content">
                            <!-- CONTENTS -->
                        </div>

                        <script>
                            $(document).ready(function(){
                            <!-- JS -->                                                                    
                            });
                        </script>
]]>.Value.Trim.Replace("<!-- NAVS -->", String.Join(vbCrLf, l1)).Trim.
                Replace("<!-- CONTENTS -->", String.Join(vbCrLf, l2)).Trim.
                Replace("<!-- JS -->", String.Join(vbCrLf, l3)).Trim

    End Sub

    Private Sub ControllerBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ControllerBuilderToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault

        Dim ControllerName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim


        Dim template = <![CDATA[

        // top level
        [Route("api")]
        [ApiController]

        // inside class
        string _connectionString;
        public ControllerNameController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("connectionString").ToString();
        }

        // get: api/endpoint/?q=
        [HttpGet("endpoint")]
        public ActionResult list(string q = "")
        {
            List<MemberModel> list = new MemberModelDataAccess(_connectionString).List(q);
            if (list != null && list.Count >= 0) { return Ok(list); }
            return BadRequest();
        }

        // get: api/endpoint/{id}
        [HttpGet("endpoint/{id:int}")]
        public ActionResult getbyid(int id)
        {
            MemberModel model = new MemberModelDataAccess(_connectionString).GetById(id);
            if (model != null) { return Ok(model); }
            return NotFound();
        }

        // post: api/endpoint/upsert
        [HttpPost("endpoint/upsert")]
        public IActionResult upsert(MemberModel data)
        {
            var res = new MemberModelDataAccess(_connectionString).Upsert(ref data);
            if (res > 0) { return Ok(new {result = "success" , data = data}); }
            if (res == OleDB.NO_CHANGES) { return Ok(new { result = "nochange", data = data }); }
            if (res == OleDB.DUPLICATE) { return Conflict(); }
            return BadRequest("failed");
        }

        // post: api/endpoint/status/update
        [HttpPost("endpoint/status/update")]
        public IActionResult status_update(MemberModel data)
        {
            var res = new MemberModelDataAccess(_connectionString).status_update(ref data);
            if (res > 0) { return Ok(new { result = "success", data = data }); }
            if (res == OleDB.NO_CHANGES) { return Ok(new { result = "nochange", data = data }); }
            if (res == OleDB.DUPLICATE) { return Conflict(); }
            return BadRequest("failed");
        }

]]>.Value

        txtDest.Text = template.Replace("MemberModel", modelName).Trim.
            Replace("ControllerName", ControllerName).
            Replace("endpoint", ControllerName.ToLower)



    End Sub

    Private Sub UIControllerBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UIControllerBuilderToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault

        Dim ControllerName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim template = <![CDATA[
        string baseURL = ConfigurationManager.AppSettings.Get("APISERVER");

        #region "MemberModel"
        public async Task<ActionResult> list()
        {
            var param = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request.QueryString["q"]);
            var user = HelperUtils.GetUser(User.Identity.Name.ParseInt());
            //var q = $"WHERE madebyid = {user.id} OR assigned_area_id = {user.empInfo.areaid}";

            List<MemberModelFull> model = new List<MemberModelFull>();
            string results = await HelperUtils.API_GET(baseURL + "api/endpoint/?q=");
            if (!string.IsNullOrWhiteSpace(results))
                model = JsonConvert.DeserializeObject<List<MemberModelFull>>(results);

            
            if (param.ContainsKey("statuslvl"))
            {
                // Group by StatusLvl and get counts
                var transBadge = model
                    .GroupBy(item => item.statuslvl)
                    .Select(group => new
                    {
                        id = group.Key,
                        count = group.Count()
                    }).ToList();

                model = new List<MemberModelFull>(model.Where(x => x.statuslvl == int.Parse(param["statuslvl"].ToString())).ToList());

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model, badgecount = transBadge }), "application/json");
            }

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model }), "application/json");
        }

        public async Task<ActionResult> getbyid(int id)
        {
            MemberModelFull model = new MemberModelFull();
            string results = await HelperUtils.API_GET(baseURL + $"api/endpoint/{id}");
            if (!string.IsNullOrWhiteSpace(results))
                model = JsonConvert.DeserializeObject<MemberModelFull>(results);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model }), "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> upsert(MemberModelFull model)
        {
            model.updatedbyid = User.Identity.Name.ParseInt();
            model.lastupdated = DateTime.Now;

            string msg = string.Empty;
            var myContent = JsonConvert.SerializeObject(model);
            var res = await HelperUtils.API_POST2(baseURL + "api/endpoint/upsert", myContent);
            msg = res.Content.ReadAsStringAsync().Result;
            if (!res.IsSuccessStatusCode)
            {
                msg = res.ReasonPhrase.ToString();
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { result = msg }), "application/json");
            }
            return Content(msg, "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> updatestatus(MemberModelFull model)
        {
            model.updatedbyid = User.Identity.Name.ParseInt();
            model.lastupdated = DateTime.Now;

            string msg = string.Empty;
            var myContent = JsonConvert.SerializeObject(model);
            var res = await HelperUtils.API_POST2(baseURL + "api/endpoint/status/update", myContent);
            msg = res.Content.ReadAsStringAsync().Result;
            if (!res.IsSuccessStatusCode)
            {
                msg = res.ReasonPhrase.ToString();
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { result = msg }), "application/json");
            }
            return Content(msg, "application/json");
        }
        #endregion
]]>.Value

        txtDest.Text = template.Replace("MemberModel", modelName).Trim.
            Replace("members", modelName.ToLower).
            Replace("ControllerName", ControllerName).
            Replace("endpoint", ControllerName.ToLower)

    End Sub

    Private Sub BsSuggestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BsSuggestToolStripMenuItem.Click

        Dim id = InputBox("input id name", "Generate BS Suggess", "_TEST_ID_")

        Dim template = <![CDATA[

            <input id="_TEST_ID_" type="text" class="form-control" />
            <ul class="dropdown-menu-c1 dropdown-menu-right" role="menu">
            </ul>

            $("#_TEST_ID_").on('change', function () {
                if ($(this).val().trim().length == 0) {
                    $(this).attr('data-id', null);
                }
            })

            $.ajax({
                url: "/{controller}/{action}/",
                type: "GET",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                success: function (result) {
                    $("#_TEST_ID_").bsSuggest("destroy")
                    $("#_TEST_ID_").bsSuggest({
                        url: null, // URL address for requesting data
                        jsonp: null, // Set this parameter name to enable the jsonp function, otherwise use the json data structure
                        data: {
                            value: result.data
                        },
                        // for data-id value
                        idField: 'id',
                        // for input value , can also be set manually on onSetSelectValue event
                        keyField: 'id', 
                        // displayed fields
                        effectiveFields: ['id', 'code', 'description'], 
                        // displayed fields alias (header)
                        effectiveFieldsAlias: { 
                            id: "ID",
                            code: "CODE",
                            description: "DESCRIPTION"
                        },
                        //showHeader: true,
                        ignorecase: true,
                        hideOnSelect: true,
                        autoSelect: false,
                        clearable: false,
                        listStyle: {
                            "max-height": "300px",
                            "max-width": "100%x"
                        }

                    }).on('onDataRequestSuccess', function (e, result) {
                       // console.log('onDataRequestSuccess: ', result);

                    }).on('onSetSelectValue', function (e, selectedData, selectedRawData) {
                       //console.log('onSetSelectValue: ', e.target.value, selectedData, selectedRawData);

                    }).on('onUnsetSelectValue', function () {
                       //console.log('onUnsetSelectValue');

                    }).on('onShowDropdown', function (e, data) {
                       //console.log('onShowDropdown', e.target.value, data);

                    }).on('onHideDropdown', function (e, data) {
                       //console.log('onHideDropdown', e.target.value, data);

                    });

                },
                error: function (errormessage) {
                    swal("Error", "Oops! something went wrong ... \n", "error");
                }
            });
]]>.Value

        txtDest.Text = template.Replace("_TEST_ID_", id)

    End Sub

    Private Sub DynamicMultiInputToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DynamicMultiInputToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text

        modelName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim tbl1 = <![CDATA[
<table id="tableRoles" class="table table-sm table-bordered" style="width:100%">
    <thead>
        <tr class="bg-primary-subtle text-uppercase">
            <th style="width:32px;"></th>
            <th class="text-center">role</th>
            <th style="width:48px;" class="text-center">add</th>
            <th style="width:48px;" class="text-center">edit</th>
            <th style="width:48px;" class="text-center">view</th>
            <th style="width:48px;" class="text-center">lvl1</th>
            <th style="width:48px;" class="text-center">lvl2</th>
            <th style="width:48px;" class="text-center">lvl3</th>
            <th style="width:48px;" class="text-center">lvl2return</th>
            <th style="width:48px;" class="text-center">lvl3return</th>
        </tr>
    </thead>
    <tfoot>
        <tr class="">
            <th style="width:32px;"><button type="button" class="btn btn-outline-primary btn-sm" onclick="addRoleRowItem(-1,-1,'');" style="border:none" data-bs-toggle="tooltip" data-placement="top" title="add role"><span class="fas fa-plus-circle"></span></button></th>
            <th class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
            <th style="width:48px;" class="text-center"></th>
        </tr>
    </tfoot>
    <tbody>
    </tbody>
</table>

]]>.Value

        Dim ths As New List(Of String)
        Dim thse As New List(Of String)
        Dim gotFile As Boolean = False

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim

            ths.Add(<![CDATA[<th class="text-center">role</th>]]>.Value.Replace("role", field))
            thse.Add(<![CDATA[<th class="text-center"></th>]]>.Value)

            If field.ToLower <> "id" AndAlso (field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code")) AndAlso ddt.Contains("int") Then

            ElseIf ddt.Contains("bool") Then

            ElseIf ddt.StartsWith("byte[]") Then

            Else

            End If

            ' FORMS
            If i = 0 Then

            End If

            If field.ToLower = "id" Then

                Continue For
            End If

            If ddt.Contains("string") Then

                Select Case field.ToLower
                    Case "email"

                    Case "pass", "password", "pwd", "syspassword"

                    Case Else

                End Select

            ElseIf ddt.Contains("bool") Then

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code") Then

                Else

                End If

            ElseIf ddt.StartsWith("date") Then

                If ddt.Contains("time") Then

                Else

                End If

            ElseIf ddt.StartsWith("byte[]") Then

                gotFile = True
            End If


        Next

    End Sub

    Private Sub DataAccessOnUIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DataAccessOnUIToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault

        If frmTableName.ShowDialog <> DialogResult.OK Then
            Return
        End If

        If String.IsNullOrWhiteSpace(frmTableName.txtTableName.Text) Then
            Return
        End If

        Dim tableName = frmTableName.txtTableName.Text.Trim

        Dim template = <![CDATA[
        private static SqlDb DB;

        // ctor
        DB = new SqlDb(ConfigurationManager.ConnectionStrings["getconnstr"].ToString());

        #region "MemberModel"
        public async Task<string> list()
        {
            Response.ContentType = "application/json";

            List<MemberModel> list = new List<MemberModel>();
            DataTable dt = DB.ExecuteToDatatable("SELECT * FROM b_towns order by name asc");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    MemberModel model = HelperUtils.BindFrom<MemberModel>(dr);
                    list.Add(model);
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new { data = list });
        }

        public async Task<string> getbyid(int id)
        {
            Response.ContentType = "application/json";

            MemberModel model = new MemberModel();
            DataRow dr = DB.QuerySingleResult("SELECT * FROM b_towns WHERE id = " + id, null);
            if (dr != null)
            {
                model = HelperUtils.BindFrom<TownModel>(dr);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model });
        }

        [HttpPost]
        public async Task<string> upsert(MemberModel model)
        {
            Response.ContentType = "application/json";

            Dictionary<string, object> activeuser = HelperUtils.GetActiveUser();
            model.updatedby = activeuser["id"].ToString().ParseInt();
            model.lastupdated = DateTime.Now;

            if (string.IsNullOrWhiteSpace(model.name))
            {
                Response.StatusCode = 400;
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { data = origData, result = "nochange" });
            }

            // for insert
            if (model.id<=0)
            {
                // check for dups
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["code"] = model.code;

                DataRow exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE code=@code", param);
                if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                if (exists != null) { return OleDB.DUPLICATE; }

                // prepare 
                param = Utils.HelperUtils.ToDictionary(model);
                param["madebyid"] = model.updatedbyid;
                param["madedate"] = model.lastupdated.Value;
                param["updatedbyid"] = model.updatedbyid;
                param["lastupdated"] = model.lastupdated.Value;

                // insert
                var resId = DB.InsertParam("b_towns", param, true);
                if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                if (resId > 0)
                {
                    // return updated model (with id)
                    model = GetById(resId);
                }
                return resId;

            } else
            {
                // for updating

                // verify id
                var orig = GetById(model.id);
                if (orig != null && orig.id > 0)
                {
                    // return if nothing to update
                    if (orig == model)
					{
						return OleDB.NO_CHANGES;
					}

                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["code"] = model.code;

                    // check for duplicate
                    DataRow exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE code=@code AND id <> {model.id}", param);
                    if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                    if (exists != null) { return OleDB.DUPLICATE; }

                    // prepare 
                    param = Utils.HelperUtils.ToDictionary(model);
                    param.Remove("madedate"); // NOT NEEDED for update
                    param.Remove("madebyid"); // NOT NEEDED for update
                    param["updatedbyid"] = model.updatedbyid;
                    param["lastupdated"] = model.lastupdated.Value;

                    // update
                    // return DB.UpdateParam("b_towns", $"WHERE id={model.id}", param);
                    var resId = DB.UpdateParam("b_towns", $"WHERE id={model.id}", param);
                    if (DB.LastError != null) { return OleDB.EXEC_ERROR; }
                    if (resId > 0)
                    {
                        // return updated model (with id)
                        model = model = GetById(model.id);
                    }
                    return resId;

                }
            }

            Response.StatusCode = 500;
            Response.StatusDescription = "Error";
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model, result = "error" });

        }
        #endregion
]]>.Value

        txtDest.Text = template.Replace("MemberModel", modelName).Trim.
            Replace("members", modelName.ToLower)

        txtDest.Text = txtDest.Text.Replace("TownModel", modelName).
            Replace("b_towns", tableName)

    End Sub

    Private Sub GETToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GETToolStripMenuItem.Click

        Dim normalPost = <![CDATA[
            return $.ajax({
                    url: "/{controller}/{action}",
                    type: "GET",
                    contentType: "application/json;charset=UTF-8",
                    dataType: "json",
                    success: function (response, textStatus, jqXHR) {
                        console.log(response);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        swal("Error!", errorThrown, "error");
                        return;
                    }
                });
]]>.Value

        txtDest.Text = normalPost

    End Sub

    Private Sub InFormDynamicTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InFormDynamicTableToolStripMenuItem.Click
        Dim des = <![CDATA[
@{
    ViewBag.Title = "COMPLAINT TYPES";
}

<div class="row">
    <div class="col-12">
        <div class="card card-condense">
            <div class="card-header" style="border-bottom: none;">
                <h4 data-collapse="#mycard-collapse"><span class="fas fa-search fs-6"></span> SEARCH</h4>
                <div class="card-header-action">
                    <a data-collapse="#mycard-collapse" class="btn btn-icon" href="#"><i class="fas fa-plus"></i></a>
                </div>
            </div>
            <div class="collapse" id="mycard-collapse">
                <div class="card-body">

                </div>
                <div class="card-footer">

                </div>
            </div>
        </div>
    </div>
</div>


<div class="row">
    <div class="col-12">
        <div class="card">
            <div class="card-header">
                <h4><span class="fas fa-tags fs-6"></span> COMPLAINT TYPES</h4> (view, add, edit)
                <div class="card-header-action">
                    <button type="button" class="btn btn-success btnSave">
                        <span class="far fa-thumbs-up"></span> SAVE CONFIGURATION
                    </button>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-lg-12">
                        <form id="dynform" autocomplete="off" novalidate="novalidate">

                            <table id="multidata" class="table table-sm table-bordered table-responsive" style="width: 100%; font-size: smaller;">
                                <thead>
                                    <tr>
                                        <th style="min-width: 32px;"></th>
                                        <th>ARCHIVED</th>
                                        <th class="col-lg-12">DESCRIPTION</th>
                                        <th style="min-width: 24px;"></th>
                                    </tr>
                                </thead>
                                <tfoot>
                                    <tr class="">
                                        <th colspan="4"><button type="button" class="btn btn-outline-primary btn-sm btnAdd" onclick="addNewTdItem()" style="border:none" data-bs-toggle="tooltip" data-placement="top" title="add"><span class="fas fa-plus-circle"></span></button></th>
                                    </tr>
                                </tfoot>
                                <tbody>

                                    @{
                                        for (int i = 0; i < 15; i++)
                                        {
                                            <tr data-id="">
                                                <td></td>
                                                <td>
                                                    <input name="description" value="" type="text" placeholder="" class="form-control form-control-sm" style="border-style:dashed;width:100%" onfocus="this.select();" onmouseup="return false;" />
                                                    <div class="d-flex flex-row">
                                                        <div class="m-2">
                                                            <small class="">
                                                                <strong class="themefont text-uppercase">Created By:</strong><br>
                                                                <span name="madebyid">SALVADOR, JAIRISH DELA CRUZ</span>
                                                            </small><br>
                                                            <small class="" name="madedate">2024-10-02 09:33 AM</small>
                                                        </div>
                                                        <div class="m-2">
                                                            <small class="">
                                                                <strong class="themefont text-uppercase">Updated By:</strong><br>
                                                                <span name="updatedbyid">SALVADOR, JAIRISH DELA CRUZ</span>
                                                            </small><br>
                                                            <small class="" name="lastupdated">2024-10-02 10:02 AM</small>
                                                        </div>
                                                    </div>
                                                </td>

                                                <td class="text-center">
                                                    <div class="pretty p-icon p-jelly p-round p-bigger">
                                                        <input type="checkbox" name="isarchived" data-id="" checked="checked">
                                                        <div class="state p-danger-o">
                                                            <i class="icon fas fa-times"></i>
                                                            <label></label>
                                                        </div>
                                                    </div>
                                                </td>

                                                <td class="text-center" data-bs-toggle="tooltip" data-placement="top" title="hold and drag to reorder">☰</td>

                                            </tr>
                                        }
                                    }

                                </tbody>
                            </table>

                        </form>

                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section scripts{
    <script>
        $(document).ready(function () {
            setTimeout(LoadList, 1)
            $('.btnSave').on('click', function (e) {
                event.preventDefault(); // Prevent the default form submission
                // check if current form is valid
                if (!$('#dynform').valid()) {
                    swal("Something went wrong!", "Please fill all required field to proceed.", "error");
                    return;
                }

                SaveList();

            });
        });

        function LoadList() {
            return $.ajax({
                url: "/complaintstype/list",
                type: "GET",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                success: function (response, textStatus, jqXHR) {
                    displayTdList(response.data);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    swal("Error!", errorThrown, "error");
                    return;
                }
            });
        }

        function SaveList() {

            var model = {
                id: _.toNumber($('#id').val()),
                description: $('#description').val(),
                isarchived: $('#isarchived').prop('checked'),
                displayindex: _.toNumber($('#displayindex').val())
            }

            // Make the AJAX POST request
            var formData = new FormData(); // Get the form data

            var err = false;
            var l1 = [];
            $('#multidata tr:has(input.description)').each((i, e) => {
                
                var description = $(e).find("input.description").val();
                var isarchived = $(e).find("input[name=isarchived]").prop('checked');

                formData.append(`model[${i}].id`, e.dataset.id);
                formData.append(`model[${i}].description`, description );
                formData.append(`model[${i}].isarchived`, isarchived );
                formData.append(`model[${i}].displayindex`, i);

                if (l1.includes(description)) {
                    err = true;
                    swal("Duplicate Found!", "Please enter unique description only.", "error");
                    return false; // break
                }

                l1.push(description);
            })

            if (err) { return; }

            $.ajax({
                type: "POST",
                url: "/complaintstype/upsert",
                data: formData,
                contentType: false, // Important for multipart form data
                processData: false, // Don't process data automatically
                success: function (response, textStatus, jqXHR) {
                    var msg;
                    if (response.result == null) {
                        msg = response.toLowerCase();
                    } else {
                        msg = response.result.toLowerCase();
                    }
                    if (msg.includes("success")) {
                        displayTdList(response.data);
                        swal("Saved!", "Record has been saved", "success");

                    } else if (msg.includes("nochange")) {
                       
                    } else {
                        swal("Error", "An error occured: " + msg + "\n", "warning");

                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (jqXHR.status == 429) {
                        swal("Error!", errorThrown, "error");
                        return;
                    }
                    swal("Error!", "Oops! something went wrong ... \n", "error");
                }
            });

        }

        var dragableTable;

        function displayTdList(data) {
            $('#multidata>tbody').empty()

            if (!data) {
                return;
            }

            if (data == null || data.length == 0) {
                return
            }

            var lg = ""
            for (var i = 0; i < data.length; i++) {
                var f = data[i];

                addTdItem(f.id);

                $(`tr[data-id=${f.id}] input.description`).val(f.description).trigger('change');
                ($(`tr[data-id=${f.id}] input[name=isarchived]`)[0]).checked = f.isarchived;

                if (f.madebyname.trim().length > 0) {
                    $(`tr[data-id=${f.id}] div[name=made]`).attr('style', null)
                    $(`tr[data-id=${f.id}] span[name=madebyid]`).text(f.madebyname);
                    $(`tr[data-id=${f.id}] small[name=madedate]`).text(ToDateTime(f.madedate));
                }

                if (f.updatedbyname.trim().length > 0) {
                    $(`tr[data-id=${f.id}] div[name=updated]`).attr('style', null)
                    $(`tr[data-id=${f.id}] span[name=updatedbyid]`).text(f.updatedbyname);
                    $(`tr[data-id=${f.id}] small[name=lastupdated]`).text(ToDateTime(f.lastupdated));
                }

            }

            $("#dynform").removeData("validator");
            $("#dynform").removeData("unobtrusiveValidation");
            InitValidator()
            $.validator.unobtrusive.parse("#dynform");
            
        }

        function addNewTdItem() {
            addTdItem(-1);

            $("#dynform").removeData("validator");
            $("#dynform").removeData("unobtrusiveValidation");
            InitValidator()
           $.validator.unobtrusive.parse("#dynform");
            
        }

        var eleId = 0;
        function addTdItem(id) {
            eleId += 1;

            $('#multidata>tbody').append(`
                    <tr data-id="${id}">
                        <td><button type="button" class="btn btn-outline-danger btn-sm" onclick="removeTdItem(this)" style="border:none" data-bs-toggle="tooltip" data-placement="top" title="remove"><span class="fas fa-times-circle"></span></button> </td>

                        <td class="text-center">
                            <div class="pretty p-icon p-jelly p-round p-bigger">
                                <input type="checkbox" name="isarchived" data-id="" class="isarchived">
                                <div class="state p-danger-o">
                                    <i class="icon fas fa-times"></i>
                                    <label></label>
                                </div>
                            </div>
                        </td>

                        <td>
                            <input id="description_${eleId}" name="description_${eleId}" value="" type="text" placeholder="enter complaint description" class="form-control form-control-sm description" 
                            style="border-style:dashed;width:100%;font-weight: 700;text-transform: uppercase;" onfocus="this.select();" onmouseup="return false;" 
                            data-val="true" data-val-required="Description is required." >

                            <span class="field-validation-valid text-danger" data-valmsg-for="description_${eleId}" data-valmsg-replace="true"></span>

                            <div class="d-flex flex-row">
                                <div class="m-2" name="made" style="display: none;">
                                    <small class="">
                                        <strong class="themefont text-uppercase">Created By:</strong><br>
                                        <span name="madebyid"></span>
                                    </small><br>
                                    <small class="" name="madedate"></small>
                                </div>
                                <div class="m-2" name="updated" style="display: none;">
                                    <small class="">
                                        <strong class="themefont text-uppercase">Updated By:</strong><br>
                                        <span name="updatedbyid">NEW</span>
                                    </small><br>
                                    <small class="" name="lastupdated"></small>
                                </div>
                            </div>

                        </td>

                        <td class="text-center" data-bs-toggle="tooltip" data-placement="top" data-bs-original-title="hold and drag to reorder">☰</td>
                    </tr>
                `);

            if (!dragableTable) {
                // Initialize the draggable functionality and get the addNewRow function
                var dr = $('#multidata>tbody>tr:last()>td').length - 1
                dragableTable = makeTableRowsDraggable('multidata', dr);
            } else {
                dragableTable.makeRowDraggable($('#multidata>tbody>tr:last()')[0]);
            }

            // disable remove
            if (id > 0) {
                $('#multidata>tbody>tr:last() button.btn.btn-outline-danger.btn-sm').remove()
            }

            // return last added row
            return $('#multidata>tbody>tr:last()')[0];
        }

        function removeTdItem(s) {
            // if data is already saved, ask user to confirm
            if ($(s).parent().parent().data().id > 0) {
                swal2({
                    title: 'Remove?',
                    text: '<b>Are you sure you want to remove this data?</b><br><br><small>(you will still need to press save to commit changes)<small>',
                    icon: 'warning',
                    dangerMode: true,
                }, () => { $(s).parent().parent().remove(); });

            } else {
                $(s).parent().parent().remove();
            }
        }


    </script>
}
]]>.Value

        txtDest.Text = des

    End Sub

    Private Sub DataAccessOnControllerAPIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DataAccessOnControllerAPIToolStripMenuItem.Click
        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim controllerName = Regex.Replace(modelName, "Model", "", RegexOptions.IgnoreCase).Trim

        If frmTableName.ShowDialog <> DialogResult.OK Then
            Return
        End If

        If String.IsNullOrWhiteSpace(frmTableName.txtTableName.Text) Then
            Return
        End If

        Dim tableName = frmTableName.txtTableName.Text.Trim

        Dim dest = <![CDATA[
        [Route("api")]
        [ApiController]
        public class MemberController : ControllerBase
        {
            private OleDB DB;
            public MemberController(IConfiguration configuration)
            {
                var conn = configuration.GetConnectionString("connectionString").ToString();
                DB = new OleDB(conn);
            }

            // get: api/endpoint
            [HttpGet("endpoint")]
            public ActionResult List()
            {
                List<NewMemberModel> list = new List<NewMemberModel>();
                DataTable dt = DB.ExecuteToDatatable("SELECT * FROM b_newapplication order by name asc");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        NewMemberModel model = HelperUtils.BindFrom<NewMemberModel>(dr);
                        list.Add(model);
                    }
                }
                if (list != null) { return Ok(list); }
                return BadRequest();
            }

            // get: api/endpoint/{id}
            [HttpGet("endpoint/{id:int}")]
            public ActionResult GetById(int id)
            {
                DataRow dr = DB.QuerySingleResult("SELECT * FROM b_newapplication WHERE id = " + id, null);
                NewMemberModel model = null;
                if (dr != null)
                {
                    model = HelperUtils.BindFrom<NewMemberModel>(dr);
                }

                if (model != null) { return Ok(model); }
                return NotFound();
            }

            // post: api/endpoint/upsert
            [HttpPost("endpoint/upsert")]
            public ActionResult Upsert(NewMemberModel model)
            {
                if (string.IsNullOrWhiteSpace(model.name))
                {
                    return BadRequest();
                }

                // for insert
                if (model.id<=0)
                {
                    // check for dups
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["code"] = model.code;

                    DataRow exists = DB.QuerySingleResult($"SELECT * FROM b_newapplication WHERE code=?", param);
                    if (DB.LastError != null) { return BadRequest(); }
                    if (exists != null) { return Conflict(); }

                    // prepare 
                    param = Utils.HelperUtils.ToDictionary(model);
                    param["madebyid"] = model.updatedbyid;
                    param["madedate"] = model.lastupdated.Value;
                    param["updatedbyid"] = model.updatedbyid;
                    param["lastupdated"] = model.lastupdated.Value;

                    // insert
                    var resId = DB.InsertParam("b_newapplication", param, true);
                    if (DB.LastError != null) { return BadRequest(); }
                    if (resId > 0)
                    {
                        model = GetById(resId); // return updated model (with id)
                        return Ok(new {result = "success" , data = model});
                    }
                    return Ok(new { result = "nochange", data = model });

                } else
                {
                    // verify id
                    var orig = GetById(model.id);
                    if (orig != null && orig.id > 0)
                    {
                        // return if nothing to update
                        if (orig == model)
                        {
                            return Ok(new { result = "nochange", data = model });
                        }

                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param["code"] = model.code;

                        // check for duplicate
                        DataRow exists = DB.QuerySingleResult($"SELECT * FROM b_newapplication WHERE code=@code AND id <> {model.id}", param);
                        if (DB.LastError != null) { return BadRequest(); }
                        if (exists != null) { return Conflict(); }

                        // prepare 
                        param = Utils.HelperUtils.ToDictionary(model);
                        param.Remove("madedate"); // NOT NEEDED for update
                        param.Remove("madebyid"); // NOT NEEDED for update
                        param["updatedbyid"] = model.updatedbyid;
                        param["lastupdated"] = model.lastupdated.Value;

                        // update
                        var resId = DB.UpdateParam("b_newapplication", $"WHERE id={model.id}", param);
                        if (DB.LastError != null) { return BadRequest(); }
                        if (resId > 0)
                        {
                            model = GetById(model.id); // return updated model (with id)
                            return Ok(new {result = "success" , data = model});
                        }

                        return Ok(new { result = "nochange", data = orig });

                    }
                }

                return BadRequest();
            }

        }
        ]]>.Value.
        Replace("NewMemberModel", modelName).
        Replace("b_newapplication", tableName).
        Replace("MemberController", controllerName & "Controller").
        Replace("endpoint", controllerName.ToLower)

        txtDest.Text = dest

    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        If frmTuple.ShowDialog <> DialogResult.OK Then Return

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        Dim tupName As String = frmTuple.cboItem.Text

        modelName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        Dim sl As New List(Of String)
        Dim sl2 As New List(Of String)

        Dim fl1 As New List(Of String)

        Dim formData As New List(Of String)

        If String.IsNullOrWhiteSpace(tupName) = False Then
            l1.Add(<![CDATA[
@model Tuple<TARONEAPI.Models.MeterBrandModel>

@{
ViewBag.Title = "MeterBrandModel";
}
]]>.Value.Trim.Replace("MeterBrandModel", modelName).Replace("METER BRAND", modelName).Replace("meterbrandmodel_v1.0.0.js", $"{modelName.ToLower}_v1.0.0.js"))

        Else
            l1.Add(<![CDATA[
@model TARONEAPI.Models.MeterBrandModel

@{
ViewBag.Title = "MeterBrandModel";
}
]]>.Value.Trim.Replace("MeterBrandModel", modelName).Replace("METER BRAND", modelName).Replace("meterbrandmodel_v1.0.0.js", $"{modelName.ToLower}_v1.0.0.js"))
        End If

        l1.Add(<![CDATA[  ]]>.Value)

        Dim l3 As New List(Of String) ' Form Content
        Dim l4 As New List(Of String) ' DROPDOWN
        Dim gotFile As Boolean = False
        Dim formdata2 As New List(Of String)
        Dim dtColDef As New List(Of String)
        Dim colCount As Integer = 0
        Dim sl3 As New List(Of String)
        Dim l5 As New List(Of String) '
        Dim l6 As New List(Of String)
        Dim cboFunc As New List(Of String)

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower
            Dim field = ch(2).Trim
            Dim useChoicesjs As Boolean = False
            Dim useChoicesjsMulti As Boolean = False

            If {"statuslvl_text", "statuslvl", "madebyid", "madedate", "lastupdated", "updatedbyid"}.Contains(field) Then
                If field.ToLower = "madedate" Then
                    dtColDef.Add(<![CDATA[ { "data": "brand", "autoWidth": true, "searchable": false } ]]>.Value.Replace("brand", field).TrimEnd)
                    lh.Add(<![CDATA[ <th class="text-center">CREATED BY</th> ]]>.Value)
                End If
                If field.ToLower = "lastupdated" Then
                    dtColDef.Add(<![CDATA[ { "data": "brand", "autoWidth": true, "searchable": false } ]]>.Value.Replace("brand", field).TrimEnd)
                    lh.Add(<![CDATA[ <th class="text-center">UPDATED BY</th> ]]>.Value)
                End If
                If field.ToLower = "statuslvl_text" Then
                    formData.Add(<![CDATA[ formData.append("statuslvl_text", statuslevelfilter.default_level_text); ]]>.Value.TrimEnd)
                    formdata2.Add(<![CDATA[ statuslvl_text: statuslevelfilter.default_level_text, // set to default  ]]>.Value.TrimEnd)
                End If
                If field.ToLower = "statuslvl" Then
                    formData.Add(<![CDATA[ formData.append("statuslvl", statuslevelfilter.default_level); ]]>.Value.TrimEnd)
                    formdata2.Add(<![CDATA[ statuslvl: statuslevelfilter.default_level, // set to default  ]]>.Value.TrimEnd)
                End If
                Continue For
            End If

            dtColDef.Add(<![CDATA[ { "data": "brand", "autoWidth": true } ]]>.Value.Replace("brand", field).TrimEnd)

            ' TABLE

            lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))
            'lr.Add(<![CDATA[ <td>@item.brand (<i class="bi bi-pencil-square"></i> edit) </td> ]]>.Value.Replace("brand", field.ToLower))
            lr.Add(<![CDATA[ <td>@item.brand</td> ]]>.Value.Replace("brand", field.ToLower))

            If field.ToLower <> "id" AndAlso (field.ToLower.EndsWith("id") Or
                field.ToLower.EndsWith("code") Or field.ToLower.EndsWith("type") Or
                field.ToLower.EndsWith("types") Or field.ToLower.EndsWith("status") Or ddt.Contains("list") Or ddt.Contains("[]")) AndAlso (ddt.Contains("int") Or ddt.Contains("list") Or ddt.Contains("[]")) AndAlso Not ddt.Contains("byte") Then

                If ddt.Contains("[]") Or ddt.Contains("list") Then
                    useChoicesjs = True
                    useChoicesjsMulti = True

                    dp.Add(<![CDATA[ 
                    // select multi items
                    window._cjs_Item1_brand.removeActiveItems();
                    js.Item1_brand.forEach((e)=>{
                        window._cjs_Item1_brand.setChoiceByValue(e.toString())
                    })
                    ]]>.Value.Replace("brand", field))

                Else
                    dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']).trigger('change'); ]]>.Value.Replace("brand", field))

                End If

                Dim add1 As String = ""
                If UseChoicesJSToolStripMenuItem.Checked Or (ddt.Contains("[]") Or ddt.Contains("list")) Then

                    useChoicesjs = True

                    add1 = "
                // init choices js
                window._cjs_Item1_brand = new Choices($('#Item1_brand')[0], {
                    allowHTML: true,
                    closeDropdownOnSelect: true,
                    duplicateItemsAllowed: false,
                    //maxItemCount: 3,
                    shouldSort: false,
                    // user can remove item
                    removeItems: true,
                    removeItemButton: true,
                    // Whether a user can add choices dynamically
                    //addItems: true,
                    //addChoices: true,
                    //editItems: true,
                });" & vbCrLf

                End If

                Dim fn = <![CDATA[
    function populate_brand_cbo() {
        return $.ajax({
            url: "/{Controller}/{Action}/",
            type: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                window.brandsData = result.data;
                setTimeout(function () {
                    $('#Item1_brand').empty();
                    $('#Item1_brand').append("<option value></option>");
                    for (var i = 0; i < brandsData.length; i++) {
                        var Desc = `${brandsData[i]['code']} - ${brandsData[i]['description']}`;
                        var opt = new Option(Desc, brandsData[i]['id']);
                        $('#Item1_brand').append(opt);
                    }
                    <add1>
                },1)
            },
            error: function (errormessage) {
                swal("Error", "Oops! something went wrong ... \n", "error");
            }
        });
    }
    ]]>.Value.Replace("<add1>", add1).Replace("brand", field).Replace("mymodal", $"{modelName}Modal")

                sl.Add(fn)

                If useChoicesjs = False Then
                    sl2.Add(<![CDATA[ $('#Item1_brand').val(null).trigger('change'); ]]>.Value.Replace("brand", field))
                    sl3.Add(<![CDATA[ $('#Item1_brand').on('change', function () {
                    // do what you want ;
                });]]>.Value.Replace("brand", field))
                End If

                cboFunc.Add(Regex.Match(fn, "function (.*?\(\))").Groups(1).Value.Trim)

            ElseIf ddt.Contains("bool") Then
                dp.Add(<![CDATA[ if(js['brand']==1) { $('#Item1_brand').prop('checked','checked'); } ]]>.Value.Replace("brand", field))
                dp.Add(<![CDATA[ $('#Item1_brand').trigger('change'); ]]>.Value.Replace("brand", field))
                l5.Add(<![CDATA[ $('#Item1_brand').on('change', function () {
                //console.log(Item1_brand.checked);
            });
        ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("byte[]") Then
                dp.Add(<![CDATA[ $('#Item1_brandPreview').attr('src', 'data:image/png;base64,' + js['brand']); ]]>.Value.Replace("brand", field))

                sl2.Add(<![CDATA[ $('#Item1_brandPreview').attr('src', 'https://place-hold.it/200x200?text=YOUR PHOTO'); ]]>.Value.Replace("brand", field))

            ElseIf ddt.StartsWith("date") Then
                'ignore

            Else
                dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']); ]]>.Value.Replace("brand", field))

                If i = 0 Then
                    dp.Add(<![CDATA[ $('#Item1_brand').closest('form').data('isDirty', false); ]]>.Value.Replace("brand", field))
                End If

            End If

            ' FORMS

            If i = 0 Then
                'l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[ @Html.ValidationSummary(false, "", new { @class = "text-danger" }) ]]>.Value)
                'l3.Add(<![CDATA[ <div class="alert alert-danger" id="ajaxResponseError" style="display:none;"></div> ]]>.Value)
                'l3.Add(<![CDATA[ </div> ]]>.Value)

            End If

            If field.ToLower = "id" Then
                l3.Add(<![CDATA[ <input type="hidden" id="Item1_id" name="Item1.id" value="-1"> ]]>.Value.Replace("Item1_id", $"Item1_{field}").Replace("Item1.id", $"Item1.{field}").Replace("Item1.", IIf(tupName.Length >= 0, "", "Item1.")))
                formData.Add(<![CDATA[ formData.append("brand", _.toNumber($("#Item1_brand").val())); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: _.toNumber($('#Item1_brand').val()) ]]>.Value.Replace("brand", field).TrimEnd)
                Continue For
            End If

            If ddt.Contains("string") And Not ddt.Contains("[]") Then

                If field.ToLower.EndsWith("filename") Or field.ToLower.Contains("file") Then
                    l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  <img id="brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)

                    fl1.Add(<![CDATA[
                        $("#Item1_brand").change(function () {
                            var reader = new FileReader();
                            reader.onload = function (e) {
                                $('#Item1_brandPreview').attr('src', e.target.result);
                            };
                            reader.readAsDataURL(this.files[0]);
                        });]]>.Value.Replace("brand", field))

                    formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").prop('files')[0]); ]]>.Value.Replace("brand", field))
                    formdata2.Add(<![CDATA[ brand: $("#Item1_brand").prop('files')[0] ]]>.Value.Replace("brand", field).TrimEnd)

                    gotFile = True

                Else

                    l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                    Select Case field.ToLower
                        Case "email"
                            l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "email", @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.Item1.brand) }) ]]>.Value.Replace("brand", field))

                        Case "pass", "password", "pwd", "syspassword"
                            l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "password", @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.Item1.brand) }) ]]>.Value.Replace("brand", field))

                        Case Else
                            l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.Item1.brand) }) ]]>.Value.Replace("brand", field))

                    End Select
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)

                    formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                    formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

                End If

            ElseIf ddt.Contains("bool") Then
                l3.Add(<![CDATA[
<div class="row">
    <div class="col-lg-12">
        <div class="pretty p-icon p-curve p-jelly">
            @Html.TextBoxFor(m => m.Item1.brand, new { @type = "checkbox" })
            <div class="state p-success-o">
                <i class="icon material-icons">done</i>
                @Html.LabelFor(m => m.Item1.brand)
            </div>
            @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" })
        </div>
    </div>
</div>
]]>.Value.Replace("brand", field))

                '
                formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').prop('checked') ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Or ddt.Contains("list") Or ddt.Contains("[]") And Not ddt.Contains("byte") Then

                If field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code") Or
                    field.ToLower.EndsWith("type") Or field.ToLower.EndsWith("types") Or
                    field.ToLower.EndsWith("status") Or ddt.Contains("[]") Or ddt.Contains("list") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                    If useChoicesjsMulti = False Then
                        l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.Item1.brand, new SelectList(new List<string>()), new { @class = "form-control form-select select2 custom-select2", @placeholder = "select option" }) ]]>.Value.Replace("brand", field))

                    Else
                        l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.Item1.brand, new SelectList(new List<string>()), new { @class = "form-control form-select", @placeholder = "select option", @multiple = "multiple" }) ]]>.Value.Replace("brand", field))

                    End If

                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)

                    'l4.Add(<![CDATA[ 
                    '    if ($('#brand').val() == '-1') {
                    '        $('#ajaxResponseError').text('please select a brand').show();
                    '        return;
                    '    }
                    '    ]]>.Value.Replace("brand", field))

                ElseIf field.ToLower.EndsWith("flag") AndAlso ddt.Contains("int") Then
                    l3.Add(<![CDATA[
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="pretty p-icon p-curve p-jelly">
                                    @Html.TextBoxFor(m => m.Item1.brand, new { @type = "checkbox" })
                                    <div class="state p-success-o">
                                        <i class="icon material-icons">done</i>
                                        @Html.LabelFor(m => m.Item1.brand)
                                    </div>
                                    @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" })
                                </div>
                            </div>
                        </div>
                        ]]>.Value.Replace("brand", field))

                    '
                    formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))
                    formdata2.Add(<![CDATA[ brand: $('#Item1_brand').prop('checked') ]]>.Value.Replace("brand", field).TrimEnd)


                Else
                    l3.Add(<![CDATA[ <div class="mb-2 col-4"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "number", @class = "form-control", @Value = "0" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[ </div> ]]>.Value)
                End If

                If useChoicesjsMulti Then
                    formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').val().select(function (x) { return _.toNumber(x) })); ]]>.Value.Replace("brand", field))
                    formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val().select(function (x) { return _.toNumber(x) }) ]]>.Value.Replace("brand", field).TrimEnd)

                Else
                    formData.Add(<![CDATA[ formData.append("brand", _.toNumber($("#Item1_brand").val())); ]]>.Value.Replace("brand", field))
                    formdata2.Add(<![CDATA[ brand: _.toNumber($('#Item1_brand').val()) ]]>.Value.Replace("brand", field).TrimEnd)

                End If

            ElseIf ddt.StartsWith("date") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))

                If ddt.Contains("time") Then
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "datetime-local", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                    dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']); ]]>.Value.Replace("brand", field))
                Else
                    l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "date", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                    dp.Add(<![CDATA[ $('#Item1_brand').val(ToDateTime(js['brand'], "yyyy-MM-DD")); ]]>.Value.Replace("brand", field))
                End If
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.StartsWith("byte[]") Then
                l3.Add(<![CDATA[ <div class="mb-2"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.TextBoxFor(m => m.Item1.brand, new { @type = "file", @accept = "image/*", @class = "form-control" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  <img id="brandPreview" src="https://place-hold.it/200x200?text=YOUR PHOTO" alt="Preview" class="img-thumbnail mt-2" style="width: 200px; height: 200px;" /> ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)

                fl1.Add(<![CDATA[
        $("#Item1_brand").change(function () {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#Item1_brandPreview').attr('src', e.target.result);
            };
            reader.readAsDataURL(this.files[0]);
        });]]>.Value.Replace("brand", field))

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").prop('files')[0]); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $("#Item1_brand").prop('files')[0] ]]>.Value.Replace("brand", field).TrimEnd)

                gotFile = True

            End If


        Next

        ' TABLE
        l1.Add(<![CDATA[
<div class="row">
    <div class="col-12">
        <div class="card card-condense">
            <div class="card-header" style="border-bottom: none;">
                <h4 data-collapse="#mycard-collapse"><span class="fas fa-search fs-6"></span> SEARCH</h4>
                <div class="card-header-action">
                    <a data-collapse="#mycard-collapse" class="btn btn-icon" href="#"><i class="fas fa-plus"></i></a>
                </div>
            </div>
            <div class="collapse" id="mycard-collapse">
                <div class="card-body">
                    
                </div>
                <div class="card-footer">
                    
                </div>
            </div>
        </div>
    </div>
</div>

@*---------- Datatable -----------*@
<div class="row">
<div class="col-12">
<div class="card">
<div class="card-header">
    <h4><span class="fas fa-tags fs-6"></span> @ViewBag.Title</h4> (view, add, edit)
</div>
<div class="card-body">

    <!-- js-level-filter -->
    <div class="row mb-2" id="js-level-filter" style="display:none">
        <div class="col-12">
            <div class="btn-group" role="group" aria-label="status level filter">
                <button type="button" class="btn btn-outline-primary" data-filter="1">Draft <span class="badge bg-secondary">0</span></button>
                <button type="button" class="btn btn-outline-primary" data-filter="2">For Checking <span class="badge bg-warning">0</span></button>
                <button type="button" class="btn btn-outline-primary" data-filter="3">Checked <span class="badge bg-warning">0</span></button>
                <button type="button" class="btn btn-outline-primary" data-filter="9">Approved <span class="badge bg-success">0</span></button>
                <button type="button" class="btn btn-outline-danger" data-filter="5">Cancelled/Deleted <span class="badge bg-danger">0</span></button>
            </div>
        </div>
    </div>

    <div class="btn-group" id="js-crud">
        <button type="button" class="btn btn-icon icon-left btn-primary text-uppercase mb-3" onclick="showmymodal()"><span class="fas fa-plus-square"></span> Add New </button>
        <button type="button" class="btn btn-icon icon-left btn-warning text-uppercase mb-3"><span class="fas fa-print"></span> Reports </button>
    </div>

    <table class="table table-hover table-sm table-responsive" id="mytable" style="width:100%">
        <thead>
            <tr class="bg-primary text-white text-uppercase">
                <TH_HEADER>
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>
</div>
</div>
</div>
]]>.Value.Replace("mytable", $"{modelName}Table").Replace("ViewBag.Brands", $"ViewBag.{modelName}").Replace("MeterBrandModel", modelName).Replace("mymodal", $"{modelName}Modal").
Replace("<TH_HEADER>", String.Join(vbCrLf, lh)).
Replace("<TD_DATA>", String.Join(vbCrLf, lr)).Trim)

        ' MODAL FORM
        l1.Add(<![CDATA[ 
@section popups{
@* --- modals and popups goes here --- *@
<div class="modal fade bg-opacity-75" id="mymodal" role="dialog" data-bs-focus="false" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="mymodalTitle" aria-hidden="true">
<div class="modal-dialog">
<div class="modal-content">
    <div class="modal-header bg-dark text-uppercase text-light p-3">
        <h5 class="modal-title"> </h5>
        
        <!-- js-level-update -->
        <div class="btn-group btn-group-sm js-level-update" id="js-level-update" style="display:none">
            <button id="" type="button" class="btn btn-danger" data-filter="backlevel" data-value="-1" data-bs-toggle="tooltip" data-bs-html="true" data-bs-placement="bottom" title="Move back to previous status/level..."><span class="fas fa-arrow-circle-left"></span> Back to level</button>
            <button id="" type="button" class="btn btn-warning" data-filter="movelevel" data-value="-1" data-bs-toggle="tooltip" data-bs-html="true" data-bs-placement="bottom" title="Move or Submit For..."><span class="fas fa-arrow-circle-right"></span> Submit for checking</button>
        </div>

    </div>
    <form id="ProductModelForm" autocomplete="off" dirty-checker>
        <div class="modal-body">
            <FORM CONTENT>
        </div>
        <div class="modal-footer">
            <button id="btnPrint_mymodal" type="button" class="btn btn-warning"><span class="fas fa-print"></span> PRINT PREVIEW</button>
            <button id="btnSave_mymodal" type="button" class="btn btn-success"><span class="far fa-thumbs-up"></span> SAVE</button>
            <button id="btnClose_mymodal" type="button" class="btn btn-danger" onclick="closeFormIfDirty(this)"><span class="far fa-thumbs-down"></span> CLOSE</button>
        </div>
    </form>
</div>
</div>
</div>    
}
@section css{
@* --- stylesheet goes here --- *@
<style>
    input, textarea, table {
        text-transform: uppercase;
    }
</style>
<link href="~/Content/js/status-level-filter/StatusLevelFilter.css" rel="stylesheet" />
}
]]>.Value.Replace("<FORM CONTENT>", String.Join(vbCrLf, l3)).Replace("ProductModelForm", modelName & "Form"))

        l1.Add("")
        l1.Add("")

        Dim normalPost = <![CDATA[

    $(sender).addClass('btn-progress');

    var model = {
    <FORM_DATA>
    }

    $.ajax({
    url: "/{Controller}/{Action}",
    data: JSON.stringify(model),
    type: "POST",
    contentType: "application/json;charset=UTF-8",
    dataType: "json",
    complete: function (jqXHR, textStatus) {
        setTimeout(() => {
            $(sender).removeClass('btn-progress');
        }, 300);
    },
    success: function (response, textStatus, jqXHR) {
    var msg;
    if (response.result == null) {
        msg = response.toLowerCase();
    } else {
        msg = response.result.toLowerCase();
    }
    if (msg.includes("success")) {
        $('#mymodal').find('form').data('isDirty', false);
        $('#mymodal').modal('hide');
        reloadmytable(); // or dtmytable.ajax.reload(null,false)
        swal("Saved!", "Record has been saved", "success");

    } else if (msg.includes("nochange")) {
        $('#mymodal').find('form').data('isDirty', false);
        $('#mymodal').modal('hide');
    } else {
        swal("Error", "An error occured: " + msg + "\n", "warning");

    }
    },
    error: function (jqXHR, textStatus, errorThrown) {
        if (jqXHR.status == 401) {
            swal2({
                title: `Unauthorized`,
                html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                icon: 'error',
                dangerMode: true,
                showCancelButton: false,
            }, () => {
                window.location = "/Authentication/Logout";
            });
            return;
        }
        swal("Error!", "Oops! something went wrong ... \n", "error");
    }
    });

]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM_DATA>", String.Join("," & vbCrLf, formdata2).Trim).Replace("mytable", $"{modelName}Table")

        If gotFile Then
            normalPost = <![CDATA[

    $(sender).addClass('btn-progress');

    // Make the AJAX POST request
    var formData = new FormData(); // Get the form data
    <FORM_DATA>
    $.ajax({
        type: "POST",
        url: "/{Controller}/{Action}",
        data: formData,
        contentType: false, // Important for multipart form data
        processData: false, // Don't process data automatically
        complete: function (jqXHR, textStatus) {
            setTimeout(() => {
                $(sender).removeClass('btn-progress');
            }, 300);
        },
        success: function (response, textStatus, jqXHR) {
            var msg;
            if (response.result == null) {
                msg = response.toLowerCase();
            } else {
                msg = response.result.toLowerCase();
            }
            if (msg.includes("success")) {
                $('#mymodal').find('form').data('isDirty', false);
                $('#mymodal').modal('hide');
                reloadmytable(); // or dtmytable.ajax.reload(null,false)

                swal("Saved!", "Record has been saved", "success");

            } else if (msg.includes("nochange")) {
                $('#mymodal').find('form').data('isDirty', false);
                $('#mymodal').modal('hide');
            } else {
                swal("Error", "An error occured: " + msg + "\n", "warning");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 401) {
                swal2({
                    title: `Unauthorized`,
                    html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                    icon: 'error',
                    dangerMode: true,
                    showCancelButton: false,
                }, () => {
                    window.location = "/Authentication/Logout";
                });
                return;
            }
            swal("Error!", "Oops! something went wrong ... \n", "error");
        }
    });
]]>.Value.Replace("<FORM_DATA>", String.Join(vbCrLf, formData)).Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("mytable", $"{modelName}Table")
        End If

        l1.Add(<![CDATA[
@section scripts{
@* --- additional scripts goes here --- *@
<script src="~/Content/js/status-level-filter/StatusLevelFilter.js"></script>

<script>
    $(document).ready(function () {
        initializeData();
    });
</script>

@*---------- todo: save as separate script -----------*@
<script>

function initializeData() {
setTimeout(ShowSwalLoader, 1);
InitValidator();
initmytable();
appendRequiredLabel();

<SELECT_FUNCTIONS>
<SELECT_EVENTS>
<CHECK_EVENTS>

$('#mymodal').on('shown.bs.modal', function () {
    $('#Item1_myInput').trigger('focus');

}).on('hidden.bs.modal', function () {
    window._seldata=null;
    clearFormValidation();
    $('#ProductModelFormBody').attr('data-js', '');
    $('#ProductModelForm')[0].reset();
    $('#Item1_id').val('-1');
    <SELECT2_MODIFIER>
});

$('#btnSave_mymodal').on('click', function () {
    if (!$(this).closest('form').valid()) {
        swal("Something went wrong!", "Please fill all required field to proceed.", "error");
        return;
    } 
    clearFormValidation();
    saveProductModelForm(this);
})

// one time run only
var isrun = 0;
$(document).ajaxStop(function (e) {
    if (isrun == 0) {
        setTimeout(CloseSwalLoader, 800);
        isrun = 1;
    }
})
}

        var dtmytable;
        var dtmytableData;
        function reloadmytable() {
            // ---- using normal reload
            dtmytable.ajax.reload(function (json) {
                dtmytableData = json.data
                // add other function to be called after table reloads
                dtmytable.columns.adjust();
            }, false)

            // ---- If using status level filter
            var q = {
                statuslvl: statuslevelfilter.active_filter
            }

            // check if initialized
            if (dtmytable == null) {
                initmytable(JSON.stringify(q))
                return;
            }

            dtmytable.ajax.url("/{Controller}/list?q=" + JSON.stringify(q)).load(function (json) {
                dtmytableData = json.data
                // add other function to be called after table reloads
                dtmytable.columns.adjust();

                setTimeout(statuslevelfilter.updateBadgeCount(json.badgecount), 1); // update badge count
            }, true) // true to reset paging after reload

        }
        function initmytable(q = "") {

            $('#mytable').DataTable().destroy();
            dtmytable = $('#mytable').DataTable({
                dom:
                    "<'row'<'p-2 col-sm-12 col-md-6 col-xl-6'l><'float-right pr-3 pt-3 p-2 col-sm-12 col-md-6 col-xl-6'f>>" +
                    "<'row'<'table-responsive col-sm-12'tr>>" +
                    "<'row'<'pl-2 pt-0 pb-2 col-sm-12 col-md-5'i><'pt-1 pb-1 pr-2 col-sm-12 col-md-7'p>>",
                stateSave: true,
                ajax: {
                    "url": "/{Controller}/list?q=" + q,
                    "type": "GET",
                    datatype: "json",
                    error: function (errormessage) {
                        toastError("Error!", "Failed to load datatable ...")
                    }
                },
                pageLength: 5,
                order: [[1, "asc"]], // index based
                lengthMenu: [
                    [5, 10, 30, 50, -1],
                    [" 5", 10, 30, 50, "All"]
                ],
                autoWidth: true,
                initComplete: function (settings, json) {
                    dtmytableData = json.data;
                    document.body.style.cursor = 'default';

                    // ----
                    if (json.badgecount!=null){
                        setTimeout(statuslevelfilter.updateBadgeCount(json.badgecount), 1); // update badge count
                    }

                },
                drawCallback: function (settings) {
                    // dtanimation2(this)
                },
                columns: [
                    // data: , name: , orderable: , autoWidth: , width: , className: 'text-center' , "visible":false
                    <DT_COL_DEF>
                ],
                aoColumnDefs: [
                    {
                        "width": "90px",
                        "aTargets": [0], // target column
                        "bSortable":false,
                        "mRender": function (data, type, full, meta) {
                            return `<button class="btn btn-primary btn-sm btnRowEdit" style="font-size:smaller;" id="vw_${full.id}" data-id="${full.id}" onclick="showEditmymodal(this, ${full.id})"> <span class="fas fa-edit"></span> VIEW</button> `;
                        },
                        "className": "text-center text-uppercase"
                    },
                    {
                        "aTargets": [-2],
                        "mRender": function (data, type, full, meta) {
                            return `
                            <div class="d-flex flex-row" style="font-size:small;">
                                <div class="me-2">
                                    <small>
                                        <strong class="themefont text-uppercase">${full.madebyname}</strong><br/>
                                        ${ToDateTime(full.madedate)}
                                    </small>
                                </div>
                            </div>
                            `

                        },
                        "className": "text-uppercase"
                    },
                    {
                        "aTargets": [-1],
                        "mRender": function (data, type, full, meta) {
                            return `
                            <div class="d-flex flex-row" style="font-size:small;">
                                <div class="me-2">
                                    <small>
                                        <strong class="themefont text-uppercase">${full.updatedbyname}</strong><br/> 
                                        ${ToDateTime(full.lastupdated)}
                                    </small>
                                </div>
                            </div>
                            `

                        },
                        "className": "text-uppercase"
                    }
                ]

            });
        }

function showmymodal() {
$(".field-validation-error, .validation-summary-errors > ul").empty();
$('#ProductModelForm')[0].reset();
$('#Item1_id').val('-1');
<SELECT2_MODIFIER>
$('#mymodal h5').html(`<span class="fas fa-file fs-6"></span> ADD NEW DETAILS`);
$('#mymodal').modal('show');

btnPrint_mymodal.hidden = true;

// ---- to update next level display
statuslevelfilter.BuildMoveToButtons(0)

}

function showEditmymodal(sender, id) {

$(sender).addClass('btn-progress');

$.ajax({
    url: "/{Controller}/{Action}/?id=" + id,
    type: "GET",
    contentType: "application/json;charset=UTF-8",
    dataType: "json",
    complete: function (jqXHR, textStatus) {
        setTimeout(() => {
            $(sender).removeClass('btn-progress');
        }, 300);
    },
    success: function (result, textStatus, jqXHR) {
        if (result.data != null) {
            fillProductModelForm(result.data);
            $('#mymodal').modal('show');
        } else {
            swal("Error!", "There are no details to display.\n", "warning");
        }
    },
    error: function (jqXHR, textStatus, errorThrown) {
        swal("Error!", "Oops! something went wrong ... \n", "error");

    }
});
}

function fillProductModelForm(js) {
window._seldata = js;
<EDIT_VAL>

btnPrint_mymodal.hidden = false;

$('#mymodal h5').html(`<span class="fas fa-edit fs-6"></span> EDIT DETAILS`);
$('#mymodal').find('form').data('isDirty', false);

// ---- to update next level display
statuslevelfilter.BuildMoveToButtons(js.statuslvl)

}

function saveProductModelForm(sender) {
// Make the AJAX POST request
}

<SELECT2>

</script>

<script>
// *******

function onMoveToLevelButtonClicked(sender) {
    var moveToId = $(sender)[0].dataset.value
    var moveToText = $(sender)[0].dataset.valueText
    var moveToTitle = $(sender)[0].dataset.title

    swal2({
        title: `Update Status?`,
        html: `Are you sure you want to move and update the status to <b>${moveToTitle}</b>`,
        icon: 'warning',
        dangerMode: true,
    }, () => {
        UpdatemymodalStatus(sender, window._seldata.id, moveToId, moveToText)
    });
}

function UpdatemymodalStatus(sender, id, newstatusid, newstatustext) {
    
    $(sender).addClass('btn-progress');

    var model = {
        id: _.toNumber($('#id').val()),
        statuslvl: _.toNumber(newstatusid),
        statuslvl_text: newstatustext,
    }

    $.ajax({
        url: "/{Controller}/updatestatus",
        data: JSON.stringify(model),
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        complete: function (jqXHR, textStatus) {
            setTimeout(() => {
                $(sender).removeClass('btn-progress');
            }, 300);
        },
        success: function (response, textStatus, jqXHR) {
            var msg;
            if (response.result == null) {
                msg = response.toLowerCase();
            } else {
                msg = response.result.toLowerCase();
            }
            if (msg.includes("success")) {
                $('#mymodal').find('form').data('isDirty', false);
                $('#mymodal').modal('hide');
                reloadmytable(); // or dtmytable.ajax.reload(null,false)
                swal("Saved!", "Status has been updated", "success");

            } else if (msg.includes("nochange")) {
                $('#mymodal').find('form').data('isDirty', false);
                $('#mymodal').modal('hide');
            } else {
                swal("Error", "An error occured: " + msg + "\n", "warning");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 401) {
                swal2({
                    title: `Unauthorized`,
                    html: 'It seems you have been logged out.<br><b>Please login and try again.</b>',
                    icon: 'error',
                    dangerMode: true,
                    showCancelButton: false,
                }, () => {
                    window.location = "/Authentication/Logout";
                });
                return;
            }
            swal("Error!", "Oops! something went wrong ... \n", "error");
        }
        
    });

}


var statuslevelfilter = new StatusLevelFilter({
    parent_filter: $('#js-level-filter'),
    parent_statusbutton: $('#js-level-update'),
    // function to call after filter
    onLevelFilterClicked_Handler: reloadmytable,
    // function to click on status update
    onMoveToLevelButtonClicked_Handler: onMoveToLevelButtonClicked,
    // default level id
    default_level: 1,
    // approval level
    approval_levels: [
        {
            id: 1,
            status: "Draft",
            name: "Draft",
            btnclass: `btn btn-outline-primary`,
            badgeclass: `badge bg-primary`,
            backclass: `btn btn-danger`,
            moveclass: `btn btn-success`,
        },
        {
            id: 9,
            status: "Active",
            name: "Active",
            btnclass: `btn btn-outline-primary`,
            badgeclass: `badge bg-primary`,
            backclass: `btn btn-danger`,
            moveclass: `btn btn-success`,
            dontshowcancel: 1, // dont show cancel button on this
        },
        {
            id: 5,
            status: "Inactive",
            name: "Inactive",
            btnclass: `btn btn-outline-danger`,
            badgeclass: `badge bg-danger`,
            backclass: `btn btn-danger`,
            moveclass: `btn btn-warning`,
            iscancelled: true, // to work as 3rd option
        }
    ],
})

// allowed movement
statuslevelfilter.allowed_movement = [
    {
        id: 1,
        backlevel: 1,
        movelevel: 1, 
    },
    {
        id: 2,
        backlevel: 1,
        movelevel: 1, 
    },
    {
        id: 9,
        backlevel: 1,
        movelevel: 1, 
    },
]

// initialize
statuslevelfilter.Initialize()

</script>
} 
]]>.Value.Replace("ProductModelForm", modelName & "Form").
Replace("mymodal", $"{modelName}Modal").
Replace("dtmytable", $"dt{modelName}").
Replace("// DROPDOWN_REPLACEMENT", String.Join(vbCrLf, l4)).
Replace("mytable", $"{modelName}Table").
Replace("ProductModelFormBody", $"{modelName}FormBody").
Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)).Replace("<SELECT2>", String.Join(vbCrLf, sl)).
Replace("<FILE_PREVIEW>", String.Join(vbCrLf, fl1)).
Replace("// Make the AJAX POST request", normalPost)
)

        l1.Add("")

        Dim htmlRes = String.Join(vbCrLf, l1).Replace("mymodal", $"{modelName}Modal").
Replace("<SELECT2_MODIFIER>", String.Join(vbCrLf, sl2)).
Replace("<DT_COL_DEF>", String.Join("," & vbCrLf, dtColDef)).
Replace("<SELECT_EVENTS>", String.Join(vbCrLf, sl3).Trim).
Replace("<CHECK_EVENTS>", String.Join(vbCrLf, l5).Trim).
Replace("m.Item1.", $"{IIf(String.IsNullOrWhiteSpace(tupName) = False, $"m.{tupName}.", "m.")}").
Replace("Item1_", $"{IIf(String.IsNullOrWhiteSpace(tupName) = False, $"{tupName}_", "")}")

        htmlRes = htmlRes.Replace("<SELECT_FUNCTIONS>", String.Join(vbCrLf, cboFunc) & vbCrLf)

        txtDest.Text = htmlRes

        txtDest2.Text = htmlRes.Substring(htmlRes.IndexOf("@section scripts{"))


    End Sub

    Private Sub txtSource_LostFocus(sender As Object, e As EventArgs) Handles txtSource.LostFocus
        Try
            txtSource.SaveFile(".\last.txt", RichTextBoxStreamType.RichText)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub txtSource_TextChanged(sender As Object, e As EventArgs) Handles txtSource.TextChanged

    End Sub

    Private Sub LoremImpsumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoremImpsumToolStripMenuItem.Click
        txtDest.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

- Lorem ipsum dolor sit amet, consectetur adipiscing elit.
- In volutpat quam sit amet eros pretium, sed imperdiet lectus cursus.
- In rhoncus lacus id massa porta, non aliquet nisl aliquam.
- Nullam convallis libero eu nunc gravida ullamcorper eget at libero.
                "
    End Sub

    Private Sub DatasetDummyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatasetDummyToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)

        For i = 0 To props.Count - 1
            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim.ToLower.Replace("?", "")
            Dim field = ch(2).Trim

            Select Case ddt
                Case "int", "float", "decimal", "bool"
                    l1.Add($"0 {field}")

                Case "byte", "byte[]"
                    l1.Add($"null {field}")

                Case "string"
                    l1.Add($"'' {field}")

                Case "date", "datetime", "dateonly", "timeonly"
                    l1.Add($"null {field}")

                Case Else
                    l1.Add($"null {field}")

            End Select

        Next

        txtDest.Text = $"SELECT {String.Join(", " & vbCrLf, l1)}"

    End Sub

    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        ' Create an empty .udl file
        Dim udlFile As String = ".\temp.udl"
        File.WriteAllText(udlFile, "")

        ' Open the .udl file using default system handler (this opens the Data Link UI)
        Dim proc As Process = Process.Start(New ProcessStartInfo(udlFile) With {.WindowStyle = ProcessWindowStyle.Normal})
        proc.WaitForExit()

        'Console.WriteLine("Waiting for user to close the Data Link dialog...")

        '' Poll until the file write time changes (indicating the user saved something)
        'While File.GetLastWriteTime(udlFile) = lastWriteTime
        '    Thread.Sleep(500)
        'End While

        '' Small delay to allow writing to complete
        'Thread.Sleep(500)

        ' Read the connection string (last line in the UDL file)
        Dim cnn = File.ReadAllLines(udlFile).LastOrDefault()

        File.Delete(udlFile)

        If String.IsNullOrWhiteSpace(cnn) = False Then txtSQLConnectionString.Text = cnn

        'Using frmSQL = New frmSQL
        '    If frmSQL.ShowDialog = DialogResult.OK Then
        '        txtSQLConnectionString.Text = frmSQL.connectionString
        '    End If
        'End Using
    End Sub

    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
        Dim l1 = txtSource.Lines.Select(Function(x) $"('{x.Trim}')").ToList

        'Dim tbl = InputBox($"Are you sure you want to INSERT the following values; {vbCrLf}{vbCrLf}{String.Join(vbCrLf, l1.Take(3))}...{vbCrLf}{vbCrLf}from table name:", "Enter Table Name", "")

        'If String.IsNullOrWhiteSpace(tbl) = False Then
        '    Using db = New cSQLServer(txtSQLConnectionString.Text)
        '        Try
        '            db.TransactionBegin()
        '            For i = 0 To l1.Count - 1
        '                db.ExecuteNonQuery("INSERT INTO ")
        '            Next
        '            db.TransactionCommit()
        '        Catch ex As Exception
        '            db.TransactionRollback()
        '        End Try
        '    End Using
        'End If

        txtDest.Text = $"INSERT INTO [TABLE_NAME] (COLUMN_NAME) {vbCrLf} VALUES {vbCrLf} {String.Join($", {vbCrLf} ", l1)}"


    End Sub

    Private Sub AccordionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AccordionToolStripMenuItem.Click

        Dim dest = <![CDATA[
        <div id="accordion_parent_id" style="display:none;">
            <div class="accordion">
                <div class="accordion-header" role="button" data-bs-toggle="collapse" data-bs-target="#panel-body-1" aria-expanded="false">
                    <h4 class="text-uppercase text-center"> <span class="fas fa-flag-checkered"></span> Accomplishment Report</h4>
                </div>

                <!-- panel -->
                <div class="accordion-body collapse" id="panel-body-1" data-parent="#accordion_parent_id" style="">
                    <!-- content -->
                    <SOURCE>
                </div>

            </div>
        </div>
]]>.Value.Replace("<SOURCE>", txtDest.Text)

        txtDest.Text = dest

    End Sub

    Private Async Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        Await Task.Run(Sub()
                           Me.UseWaitCursor = True
                           ToolStrip1.Enabled = False
                           ToolStrip2.Enabled = False
                           Try
                               Using conn = New OleDbConnection(txtSQLConnectionString.Text)
                                   conn.Open()

                                   Dim tbls = conn.GetSchema("Tables")
                                   cboTable.Items.Clear()
                                   cboTable.Items.AddRange(tbls.Rows.OfType(Of DataRow).Where(Function(x) x("TABLE_TYPE").ToString = "TABLE").Select(Function(x) x("TABLE_NAME").ToString).ToArray)
                                   cboTable.Items.Add("--------------------")
                                   cboTable.Items.AddRange(tbls.Rows.OfType(Of DataRow).Where(Function(x) x("TABLE_TYPE").ToString = "VIEW").Select(Function(x) x("TABLE_NAME").ToString).ToArray)

                               End Using

                               If connHistory.Add(txtSQLConnectionString.Text) Then
                                   File.WriteAllLines(connpath, connHistory)
                                   txtSQLConnectionString.Items.Add(txtSQLConnectionString.Text)
                               End If
                           Catch ex As Exception
                               MsgBox(ex.Message, vbExclamation)
                           End Try
                           ToolStrip1.Enabled = True
                           ToolStrip2.Enabled = True
                           Me.UseWaitCursor = False
                       End Sub)
    End Sub

    Private Sub btnGenerateFromTable_Click(sender As Object, e As EventArgs) Handles btnGenerateFromTable.Click

        If String.IsNullOrWhiteSpace(cboTable.Text) Then Return
        If cboTable.Text.StartsWith("----") Then Return

        Dim tblName = cboTable.Text
        Dim l2 As New List(Of String)
        Dim l3 As New List(Of String)
        l3.Add("var data = JRaw.Parse(Request.PostBody());")
        l3.Add("var dic = new Dictionary<string, object>();")

        'Using cn = New OleDbConnection(txtSQLConnectionString.Text)
        '    cn.Open()
        '    Using cmd = cn.CreateCommand()
        '        cmd.CommandText = cboTable.Text
        '        Using reader = cmd.ExecuteReader
        '            Dim dt2 = New DataTable()
        '            dt2.Load(reader)
        '            Debug.Print("")
        '        End Using
        '    End Using
        'End Using

        Using conn = New OleDbConnection(txtSQLConnectionString.Text)
            conn.Open()

            If cboTable.Text.Trim.ToLower.Contains("select ") Or cboTable.Text.Trim.ToLower.StartsWith("exec ") Then
                Using cmd As New OleDbCommand(cboTable.Text, conn)
                    cmd.CommandTimeout = Integer.Parse(optCommandTimeout.Text)

                    Dim dt = cmd.ExecuteReader(CommandBehavior.SchemaOnly).GetSchemaTable
                    For i = 0 To dt.Rows.Count - 1
                        Dim propertyString As String = DbTypeToStringFromGetSchemaTable(dt.Rows(i))
                        l2.Add(propertyString)

                        If propertyString.Contains("[Required]") Then
                            l3.Add(<![CDATA[dic["_"] = "_"; // required ]]>.Value.Replace("_", dt.Rows(i)("ColumnName")))
                        Else
                            l3.Add(<![CDATA[dic["_"] = "_";]]>.Value.Replace("_", dt.Rows(i)("ColumnName")))
                        End If

                    Next

                    conn.Close()
                End Using

                tblName = Regex.Match(cboTable.Text, " from \[(.*?)\]", RegexOptions.IgnoreCase).Groups(1).Value.Trim
                If String.IsNullOrWhiteSpace(tblName) Then tblName = Regex.Match(cboTable.Text, " from (.*?) ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
                If String.IsNullOrWhiteSpace(tblName) Then tblName = Regex.Match(cboTable.Text, " from (.*?)$", RegexOptions.IgnoreCase).Groups(1).Value.Trim
                If String.IsNullOrWhiteSpace(tblName) Then tblName = Regex.Match(cboTable.Text, "exec (.*?) ", RegexOptions.IgnoreCase).Groups(1).Value.Trim
                l3.Add("")
                l3.Add(<![CDATA[var res = DB.InsertParam("_", dic, true);]]>.Value.Replace("_", tblName))

            Else

                Dim dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, {Nothing, Nothing, cboTable.Text, Nothing})
                For i = 0 To dt.Rows.Count - 1
                    Dim propertyString As String = DbTypeToString(dt.Rows(i))
                    l2.Add(propertyString)

                    If propertyString.Contains("[Required]") Then
                        l3.Add(<![CDATA[dic["_"] = "_"; // required]]>.Value.Replace("_", dt.Rows(i)("COLUMN_NAME")))
                    Else
                        l3.Add(<![CDATA[dic["_"] = "_";]]>.Value.Replace("_", dt.Rows(i)("COLUMN_NAME")))
                    End If

                Next
                l3.Add("")
                l3.Add(<![CDATA[var res = DB.InsertParam("_", dic, true);]]>.Value.Replace("_", cboTable.Text))

            End If

        End Using

        l3.Add("if (DB.LastError != null) { throw DB.LastError; }")

        txtSource.Text = <![CDATA[
public class PositionModel
{
// replace
}
]]>.Value.Replace("// replace", String.Join(vbCrLf, l2)).Replace("PositionModel", Regex.Replace(StrConv(tblName, VbStrConv.ProperCase), "[^a-z0-9_]", "", RegexOptions.IgnoreCase))

        txtDest.Text = String.Join(vbCrLf, l3)

    End Sub

    Function DbTypeToString(row As DataRow) As String
        ' Extract column information from the DataRow
        Dim columnName As String = Regex.Replace(row("COLUMN_NAME").ToString().ToLower, "[^a-z0-9_]", "", RegexOptions.IgnoreCase)
        Dim dataType As Integer = Convert.ToInt32(row("DATA_TYPE"))
        Dim isNullable As Boolean = CBool(row("IS_NULLABLE"))
        Dim maxLength As Integer = If(row("CHARACTER_MAXIMUM_LENGTH") Is DBNull.Value, 0, Convert.ToInt32(row("CHARACTER_MAXIMUM_LENGTH")))

        ' Map OleDbType to C# data type
        Dim csType As String = GetCsNetType(dataType)
        Dim annotations As String = ""
        If csType = "string" And optAnnotation.Checked Then
            annotations = GetAnnotations(isNullable, maxLength)
        End If
        ' Build the property string in C# syntax
        Dim propertyString As String = $"{annotations}   public {csType} {columnName} {{ get; set; }}"

        Return propertyString
    End Function

    Function GetCsNetType(dataType As Integer) As String
        ' Map OleDbType to C# data type
        Select Case dataType
            Case CInt(OleDbType.Char), CInt(OleDbType.WChar), CInt(OleDbType.VarWChar), CInt(OleDbType.LongVarWChar)
                Return "string"
            Case CInt(OleDbType.Integer), CInt(OleDbType.UnsignedTinyInt), CInt(OleDbType.UnsignedInt), CInt(OleDbType.SmallInt), CInt(OleDbType.UnsignedSmallInt)
                Return "int"
            Case CInt(OleDbType.BigInt), CInt(OleDbType.UnsignedBigInt)
                Return "long"
            'Case CInt(OleDbType.SmallInt), CInt(OleDbType.UnsignedSmallInt)
            '    Return "short"
            Case CInt(OleDbType.Decimal), CInt(OleDbType.Numeric), CInt(OleDbType.Currency), CInt(OleDbType.Double), CInt(OleDbType.Single)
                Return "decimal"
            'Case CInt(OleDbType.Double)
            '    Return "double"
            'Case CInt(OleDbType.Single)
            '    Return "float"
            Case CInt(OleDbType.Boolean)
                Return "bool"
            Case CInt(OleDbType.Date), CInt(OleDbType.DBDate), CInt(OleDbType.DBTimeStamp)
                Return "DateTime?"
            Case CInt(OleDbType.Binary), CInt(OleDbType.LongVarBinary) ' Handle binary data (e.g., image type)
                Return "byte[]?"
            Case Else
                Return "object?" ' Fallback for unknown types
        End Select
    End Function


    ' dt = cmd.ExecuteReader(CommandBehavior.SchemaOnly).GetSchemaTable
    Function DbTypeToStringFromGetSchemaTable(row As DataRow) As String
        ' Extract column information from the DataRow
        Dim columnName As String = row("ColumnName").ToString()
        Dim dataType As Type = CType(row("DataType"), Type)
        Dim isNullable As Boolean = CBool(row("AllowDBNull"))
        Dim maxLength As Integer = CInt(row("ColumnSize"))

        ' Map OleDbType to C# data type
        Dim csType As String = GetCsNetTypeFromSchema(dataType)
        Dim annotations As New List(Of String)

        Dim propername As String = ConvertToPropertyName(columnName)
        If propername <> columnName Then
            annotations.Add($"   [JsonProperty(""{columnName}"")]" & vbCrLf)
        End If

        If csType = "string" And optAnnotation.Checked Then
            annotations.Add(GetAnnotations(isNullable, maxLength))
        End If

        ' Build the property string in C# syntax
        Dim propertyString As String = $"{String.Join("", annotations)}   public {csType} {propername} {{ get; set; }}"

        Return propertyString
    End Function

    ' Function to map .NET Type to C# data type
    Function GetCsNetTypeFromSchema(dataType As Type) As String
        ' Map .NET Type to C# data type
        Select Case dataType
            Case GetType(String)
                Return "string"
            Case GetType(Integer), GetType(Short)
                Return "int"
            Case GetType(Long)
                Return "long"
            Case GetType(Byte)
                Return "byte"
            Case GetType(Decimal), GetType(Double), GetType(Single)
                Return "decimal"
            Case GetType(Boolean)
                Return "bool"
            Case GetType(DateTime)
                Return "DateTime?"
            Case GetType(Byte())
                Return "byte[]"
            Case Else
                Return "object?"
        End Select
    End Function

    Function GetAnnotations(isNullable As Boolean, maxLength As Integer) As String
        Dim annotations As String = ""

        ' Add [Required] annotation if the column is not nullable
        If Not isNullable Then
            annotations &= "   [Required]" & vbCrLf
        End If

        ' Add [MaxLength] annotation if the column has a maximum length
        If maxLength > 0 Then
            annotations &= $"   [MaxLength({maxLength})]" & vbCrLf
        End If

        Return annotations
    End Function

    Dim specialk = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "class", "public", "private", "function", "property", "end", "if", "then", "else", "module", "return", "string", "integer", "boolean"
    }

    Public Function ConvertToPropertyName(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return "Invalid_usr"

        ' Step 1: Extract leading number or range
        Dim numberPart As String = ""
        Dim numberMatch = Regex.Match(input, "^\s*(\d+(\s*-\s*\d+)?)")
        If numberMatch.Success Then
            numberPart = numberMatch.Value.Trim().Replace(" ", "").Replace("-", "_")
            input = input.Substring(numberMatch.Length).Trim()
        End If

        ' Step 2: Replace special characters contextually
        input = Regex.Replace(input, "(^|\s)#", "${1}No_")
        input = Regex.Replace(input, "#", "_No_")

        input = Regex.Replace(input, "(^|\s)\$", "${1}Amt_")
        input = Regex.Replace(input, "\$", "_Amt_")

        input = Regex.Replace(input, "(^|\s)@", "${1}at_")
        input = Regex.Replace(input, "@", "_at_")

        input = Regex.Replace(input, "(^|\s)\.", "${1}dot_")
        input = Regex.Replace(input, "\.", "_dot_")

        ' Step 3: Normalize whitespace and dashes to underscores
        input = Regex.Replace(input, "[\s\-]+", "_")

        ' Step 4: Remove any leftover non-alphanumeric characters
        input = Regex.Replace(input, "[^\w]", "")

        ' Step 5: Append number part at the end
        Dim result = input
        If Not String.IsNullOrEmpty(numberPart) Then
            result &= "_" & numberPart
        End If

        ' Step 6: Append _usr if result is a VB keyword
        If specialk.Contains(result.ToLower()) Then
            result &= "_usr"
        End If

        ' Step 7: Ensure it doesn't start with a digit
        If Regex.IsMatch(result, "^\d") Then
            result = "_" & result
        End If

        Return result
    End Function


    Function CreateTableSchema(dt As DataTable, tblName As String) As String

        Dim l1 As New List(Of String)
        l1.Add("id".PadRight(20) & "int IDENTITY(1, 1) PRIMARY KEY")

        For i = 0 To dt.Columns.Count - 1
            Dim l2 As New List(Of String)
            Dim index = i
            Dim colName = dt.Columns(i).ColumnName.Trim().ToLower
            Dim colPart = $"[{colName}]".ToLower
            Dim mx = 30

            Dim pad = Math.Max(colPart.Length + 5, 20)

            l2.Add(colPart.PadRight(pad))

            Dim tp = dt.Columns(i).DataType.FullName.ToLower

            Select Case True
                Case tp.Contains("int"), tp.Contains("long")
                    l2.Add("int")

                Case tp.Contains("boolean")
                    l2.Add("bit DEFAULT 0")

                Case tp.Contains("double"), tp.Contains("decimal")
                    l2.Add("numeric(15,2)")

                Case tp.Contains("date"), tp.Contains("datetime")
                    l2.Add("datetime")

                Case tp.Contains("byte")
                    l2.Add("image")

                Case Else

                    If dt.Columns(i).MaxLength > 0 Then
                        If dt.Columns(i).MaxLength > 2000 Then
                            l2.Add($"nvarchar(max)")
                        Else
                            l2.Add($"nvarchar({dt.Columns(i).MaxLength})")
                        End If
                    Else
                        If colName.ToLower.EndsWith("id") OrElse colName.ToLower.EndsWith("number") OrElse colName.ToLower.EndsWith("num") Then
                            If dt.Rows.Count > 0 Then
                                mx = dt.Rows.OfType(Of DataRow).OrderByDescending(Function(x) x.ItemArray(index).ToString.Length).FirstOrDefault.ItemArray(index).ToString.Length
                            End If
                            If mx < 30 Then mx = 30
                            l2.Add($"nvarchar({Math.Min(mx, 30)})")
                        Else
                            l2.Add("nvarchar(255)")

                        End If
                    End If

            End Select

            If dt.Columns(i).AllowDBNull = False Then
                l2.Add(" NOT NULL")
            End If

            If dt.Columns(i).Unique Then
                l2.Add(" UNIQUE")
            End If

            l1.Add(String.Join("", l2))

        Next

        l1.Add("statuslvl_text".PadRight(20) & "nvarchar(50)")
        l1.Add("statuslvl".PadRight(20) & "int")
        l1.Add("madebyid".PadRight(20) & "int")
        l1.Add("madedate".PadRight(20) & "datetime")
        l1.Add("updatedbyid".PadRight(20) & "int")
        l1.Add("lastupdated".PadRight(20) & "datetime")

        Dim l3 As New List(Of String)

        l3.Add($"CREATE TABLE [{tblName}] (")
        l3.Add(String.Join("," & vbCrLf, l1))
        l3.Add($");")

        Return String.Join(vbCrLf, l3)
    End Function

    Private Sub btnEditor_Click(sender As Object, e As EventArgs) Handles btnEditor.Click
        Using frm = New frmSQLEditor
            frm.txtEditor.Text = cboTable.Text
            frm.ShowDialog()
            cboTable.Text = frm.txtEditor.Text
        End Using
    End Sub

    Private Sub ToolStripButton9_Click(sender As Object, e As EventArgs) Handles ToolStripButton9.Click
        Using frm = New frmSQLEditor
            frm.txtEditor.Text = txtSQLConnectionString.Text
            frm.ShowDialog()
            txtSQLConnectionString.Text = frm.txtEditor.Text
        End Using
    End Sub

    Private Sub JSControllerObjectLiteralToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JSControllerObjectLiteralToolStripMenuItem.Click
        Dim js = <![CDATA[
var model = {
    init: function() {
        this.data = null;
        this.initListener();
    },
    initListener: function() {
        // all event listener should be here        
    },
    onAddItem: function(){
        // verify if allowed
        // show dialog
        this.showAddDialog();
    },
    onEditItem: function(id) {
        // verify if allowed
        // get data (ajax)
        // set response to data
        // fill view data
        // show dialog
        this.showEditDialog();
    },
    onDeleteItem: function(id) {
        // verify if allowed
    },
    onSaveItem: function() {
        // verify if allowed
        // check if clean
        // validate
        // save
    },
    showAddDialog: function() {

    },
    showEditDialog: function() {

    },
    closeDialog: function() {
        this.data = null;
    },
    isClean: function(){
        // method to check if form is clean or not
        return true;
    },
    onFillData: function(js) {
        // fill form with data        
    },


}

var table = {

    init: function() {
        // initialize datatable
    },
    loadData: function() {
        // query by ajax
    },
    setData: function(js) {
        // reload datatable
    }

}
        ]]>.Value

        txtDest.Text = js.Trim
    End Sub

    Function NewPageData()
        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return Nothing
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        modelName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim dialogId = $"{StrConv(modelName.ToLower, VbStrConv.ProperCase)}Modal"
        Dim tableId = $"{StrConv(modelName.ToLower, VbStrConv.ProperCase)}Table"
        Dim formId = $"{StrConv(modelName.ToLower, VbStrConv.ProperCase)}Form"

        Return {modelName, props, dialogId, tableId, formId}.ToList
    End Function

    Private Sub ModalPopupBS46xToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModalPopupBS46xToolStripMenuItem.Click

        Dim ds = NewPageData()
        If IsNothing(ds) Then Return

        Dim modelName = ds(0)
        Dim props As List(Of String) = ds(1)
        Dim dialogId = ds(2)
        Dim tableId = ds(3)
        Dim formId = ds(4)

        Dim haveDateMask As Boolean = False
        Dim haveSelect2 As Boolean = False

        Dim formGen As New List(Of String)
        Dim formGen2 As New List(Of String) ' mvc
        Dim scriptGen As New List(Of String)
        Dim scriptFillData As New List(Of String)

        formGen.Add(<![CDATA[]]>.Value)

        formGen2.Add(<![CDATA[ 
            <div class="mb-2">
            @Html.ValidationSummary(false, "", new { @class = "text-danger" }) 
            <div class="alert alert-danger" id="ajaxResponseError" style="display:none;" hidden></div>
            </div> 
        ]]>.Value)

        ' generate table
        For i = 0 To props.Count - 1
            Dim line = props(i).Trim
            If String.IsNullOrWhiteSpace(line) Then Continue For
            Dim arr = line.Split(" ")
            Dim type = arr(1).Trim.ToLower
            Dim fieldname = arr(2).Trim

            Select Case True
                Case type = "string" And Not type.Contains("[]")

                    Dim l0 As New List(Of String)

                    Select Case fieldname.ToLower
                        Case "email"
                            formGen.Add(<![CDATA[
                                <div class="form-group">
                                    <label>consumerid</label>
                                    <input type="email" class="form-control" id="txtconsumerid" name="consumerid" placeholder="consumerid">
                                </div>
                                ]]>.Value.Replace("consumerid", fieldname))

                            l0.Add(<![CDATA[  @Html.TextBoxFor(m => m.consumerid, new { @type = "email", @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.consumerid) }) ]]>.Value)

                        Case "pass", "password", "pwd", "syspassword", "loginpwd"
                            formGen.Add(<![CDATA[
                                <div class="form-group">
                                    <label>consumerid</label>
                                    <div class="input-group">
                                        <input type="password" class="form-control" id="txtconsumerid" name="consumerid" placeholder="consumerid">
                                        <span class="input-group-append">
                                            <button type="button" class="btn btn-sm btn-default btn-flat" data-action="showpassword"> <i class="fas fa-eye"></i> </button>
                                        </span>
                                    </div>
                                </div>
                                ]]>.Value.Replace("consumerid", fieldname))

                            l0.Add(<![CDATA[  @Html.TextBoxFor(m => m.consumerid, new { @type = "password", @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.consumerid) }) ]]>.Value)

                            formGen2.Add(<![CDATA[
                                <div class="form-group">
                                    @Html.LabelFor(m => m.consumerid, new { @class = "form-label" })
                                    <div class="input-group">
                                        <input>
                                        <span class="input-group-append">
                                            <button type="button" class="btn btn-sm btn-default btn-flat" data-action="showpassword"> <i class="fas fa-eye"></i> </button>
                                        </span>
                                    </div>
                                    @Html.ValidationMessageFor(m => m.consumerid, "", new { @class = "text-danger" })
                                </div>
                                ]]>.Value.Replace("<input>", String.Join("", l0)).Replace("consumerid", fieldname))

                            Continue For

                        Case Else
                            formGen.Add(<![CDATA[
                                <div class="form-group">
                                    <label>consumerid</label>
                                    <input type="text" class="form-control" id="txtconsumerid" name="consumerid" placeholder="consumerid">
                                </div>
                                ]]>.Value.Replace("consumerid", fieldname))

                            l0.Add(<![CDATA[  @Html.TextBoxFor(m => m.consumerid, new { @type = "text", @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.consumerid) }) ]]>.Value)

                    End Select

                    formGen2.Add(<![CDATA[
                    <div class="form-group">
                        @Html.LabelFor(m => m.consumerid, new { @class = "form-label" })
                        <input>
                        @Html.ValidationMessageFor(m => m.consumerid, "", new { @class = "text-danger" })
                    </div>
                    ]]>.Value.Replace("<input>", String.Join("", l0)).Replace("consumerid", fieldname))

                Case type = "bool", type = "int" And (fieldname.StartsWith("is") Or fieldname.EndsWith("flag") Or fieldname.Contains("enable") Or fieldname.Contains("disable"))
                    formGen.Add(<![CDATA[
                    <div class="form-group">
                        <label>consumerid</label>
                        <div class="custom-control custom-checkbox">
                            <input class="custom-control-input" type="checkbox" id="chkconsumerid" name="consumerid">
                            <label for="chkconsumerid" class="custom-control-label">consumerid</label>
                        </div>
                    </div>
                    ]]>.Value.Replace("consumerid", fieldname))

                    formGen2.Add(<![CDATA[
                    <div class="form-group">
                        @Html.LabelFor(m => m.consumerid, new { @class = "form-label" })
                        <div class="custom-control custom-checkbox">
                            @Html.TextBoxFor(m => m.consumerid, new { @type="checkbox", @class = "custom-control-input" })                            
                            @Html.LabelFor(m => m.consumerid, new { @class = "custom-control-label" })
                        </div>
                        @Html.ValidationMessageFor(m => m.consumerid, "", new { @class = "text-danger" })
                    </div>
                    ]]>.Value.Replace("consumerid", fieldname))

                Case type.Contains("int"), type.Contains("decimal"), type.Contains("double"), type.Contains("list"),
                     type.Contains("[]") And Not type.Contains("byte")

                    If (fieldname.ToLower.EndsWith("id") And fieldname.ToLower <> "id") Or fieldname.ToLower.EndsWith("code") Or
                        fieldname.ToLower.EndsWith("type") Or fieldname.ToLower.EndsWith("types") Or
                        fieldname.ToLower.EndsWith("status") Or type.Contains("[]") Or type.Contains("list") Then

                        ' dropdown
                        formGen.Add(<![CDATA[
                        <div class="form-group">
                            <label>consumerid</label>
                            <select class="form-control select2" style="width: 100%;" name="consumerid" id="cboconsumerid">
                            </select>
                        </div>
                        ]]>.Value.Replace("consumerid", fieldname))

                        formGen2.Add(<![CDATA[
                        <div class="form-group">
                            @Html.LabelFor(m => m.consumerid, new { @class = "form-label" })
                            @Html.DropDownListFor(m => m.consumerid, new SelectList(new List<string>()), new { @class = "form-control select2", @placeholder = "select option", @style="width: 100%;" })
                            @Html.ValidationMessageFor(m => m.consumerid, "", new { @class = "text-danger" })
                        </div>
                        ]]>.Value.Replace("consumerid", fieldname))

                        'scriptGen.Add(<![CDATA[
                        '$(`#cboconsumerid`).select2({
                        '    placeholder: {
                        '        id: '-1',               // the value of the option
                        '        text: 'Select option'
                        '    },
                        '    allowClear: true,
                        '    // tags: true,              // for text drop down (non id field)
                        '    // parent: $(`.modal`)      // 
                        '})
                        ']]>.Value.Replace("consumerid", fieldname))

                        haveSelect2 = True
                    Else
                        ' normal
                        formGen.Add(<![CDATA[
                        <div class="form-group">
                            <label>consumerid</label>
                            <input type="number" class="form-control" id="txtconsumerid" name="consumerid" placeholder="consumerid">
                        </div>
                        ]]>.Value.Replace("consumerid", fieldname))

                        formGen2.Add(<![CDATA[
                        <div class="form-group">
                            @Html.LabelFor(m => m.consumerid, new { @class = "form-label" })
                            @Html.TextBoxFor(m => m.consumerid, new { @type = "number", @class = "form-control", @placeholder = Html.DisplayNameFor(m => m.consumerid) })
                            @Html.ValidationMessageFor(m => m.consumerid, "", new { @class = "text-danger" })
                        </div>
                        ]]>.Value.Replace("consumerid", fieldname))

                    End If

                Case type.StartsWith("date")

                    formGen.Add(<![CDATA[
                        <div class="form-group">
                            <label>consumerid</label>
                            <div class="input-group date" id="dtconsumerid" data-target-input="nearest">
                                <input type="text" class="form-control datetimepicker-input" data-target="#dtconsumerid" placeholder="MM/DD/YYYY" id="txtconsumerid" name="consumerid" data-inputmask-alias="datetime" data-inputmask-inputformat="mm/dd/yyyy" data-mask />
                                <div class="input-group-append" data-target="#dtconsumerid" data-toggle="datetimepicker">
                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                </div>
                            </div>
                        </div>
                    ]]>.Value.Replace("consumerid", fieldname))

                    formGen2.Add(<![CDATA[
                        <div class="form-group">
                            @Html.LabelFor(m => m.consumerid, new { @class = "form-label" })
                            <div class="input-group date" id="dtconsumerid" data-target-input="nearest">
                                @Html.TextBoxFor(m => m.consumerid, new { @type = "text", @class = "form-control datetimepicker-input", @data_target="#dtconsumerid", @data_inputmask_alias="datetime", @data_inputmask_inputformat="mm/dd/yyyy", @placeholder = "MM/DD/YYYY", @data_mask = "" })
                                <div class="input-group-append" data-target="#dtconsumerid" data-toggle="datetimepicker">
                                    <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                                </div>
                            </div>
                            @Html.ValidationMessageFor(m => m.consumerid, "", new { @class = "text-danger" })
                        </div>
                    ]]>.Value.Replace("consumerid", fieldname))

                    scriptGen.Add(<![CDATA[
                        $('#dtconsumerid').datetimepicker({
                            defaultDate: new Date(),
                            format: 'MM/DD/YYYY',
                        });
                        ]]>.Value.Replace("consumerid", fieldname))

                Case type.StartsWith("byte[]")

            End Select

            Debug.Print("")
        Next

        If haveDateMask Then
            scriptGen.Add(<![CDATA[
                        $('[data-mask]').inputmask()
                        ]]>.Value)
        End If

        If haveSelect2 Then
            scriptGen.Add(<![CDATA[
                        // focus when opened
                        $(document).on('select2:open', function (e) {
                            document.querySelector('.select2-search__field').focus();
                        });
                        // initialize select2
                        $(`select.select2`).select2({
                            placeholder: {
                                id: '-1',
                                text: 'Select option'
                            },
                            allowClear: true,
                            // tags: true,              // for custom input
                            parent: $(`.modal`)
                        })
                        ]]>.Value)
        End If

        Dim source = <![CDATA[

<form autocomplete="off" id="formId">
    <div class="modal fade" id="modalId" role="dialog" data-backdrop="static" data-keyboard="false" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">
                        <i class="fas fa-file-signature"></i> Add or Edit Form
                    </h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">
                    // content
                </div>
                <div class="modal-footer justify-content-between">
                    <button type="button" class="btn btn-default text-primary" data-action="close"> <i class="fas fa-times"></i> Close</button>
                    <button type="button" class="btn btn-primary" data-action="save"> <i class="fas fa-save"></i> Save</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</form>

        ]]>.Value.Replace("formId", formId).Replace("modalId", dialogId)

        If optGenerateByHTMLHelper.Checked Then
            source = source.Replace("// content", String.Join(vbCrLf, formGen2.Select(Function(x) x.Trim))).Trim
        Else
            source = source.Replace("// content", String.Join(vbCrLf, formGen.Select(Function(x) x.Trim))).Trim
        End If

        Dim js = <![CDATA[
             
        ]]>.Value


        txtDest.Text = source 'String.Join(vbCrLf, formGen.Select(Function(x) x.Trim)).Trim
        txtDest2.Text = String.Join(vbCrLf, scriptGen.Select(Function(x) x.Trim)).Trim

    End Sub

    Private Sub UseChoicesJSToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles UseChoicesJSToolStripMenuItem.CheckedChanged
        UseChoicesJSToolStripMenuItem.Image = IIf(UseChoicesJSToolStripMenuItem.Checked, My.Resources.check_box, My.Resources.check_box_uncheck)
    End Sub

    Private Sub optAnnotation_Click(sender As Object, e As EventArgs) Handles optAnnotation.Click
        optAnnotation.Image = IIf(optAnnotation.Checked, My.Resources.check_box, My.Resources.check_box_uncheck)
    End Sub

    Private Sub optGenerateByHTMLHelper_Click(sender As Object, e As EventArgs) Handles optGenerateByHTMLHelper.Click
        optGenerateByHTMLHelper.Image = IIf(optGenerateByHTMLHelper.Checked, My.Resources.check_box, My.Resources.check_box_uncheck)
    End Sub

    Private Sub ToolStripButton4_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripButton4.ButtonClick

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)

        'l1.Add(<![CDATA[ @using LIBCORE.Models; ]]>.Value)
        'l1.Add(<![CDATA[ @model MeterBrandModel ]]>.Value.Replace("MeterBrandModel", modelName))

        l1.Add(<![CDATA[  
@section css {
    <!-- section rendered css -->
    <link rel="stylesheet" href="https://cdn.datatables.net/2.0.8/css/dataTables.dataTables.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/searchbuilder/1.7.1/css/searchBuilder.dataTables.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/3.0.2/css/buttons.dataTables.css" />
    <link rel="stylesheet" href="https://cdn.datatables.net/datetime/1.5.2/css/dataTables.dateTime.min.css" />
}
]]>.Value)

        Dim lh As New List(Of String)
        Dim lr As New List(Of String)
        Dim dp As New List(Of String)

        For i = 0 To props.Count - 1

            Dim v = props(i).Trim
            If String.IsNullOrWhiteSpace(v) Then Continue For
            Dim ch = v.Split(" ")

            Dim ddt = ch(1).Trim
            Dim field = ch(2).Trim

            lh.Add(<![CDATA[ <th>METER BRAND</th> ]]>.Value.Replace("METER BRAND", field.ToUpper))

            'lr.Add(<![CDATA[ <td>@item.brand (<i class="bi bi-pencil-square"></i> edit) </td> ]]>.Value.Replace("brand", field.ToLower))
            lr.Add(<![CDATA[ <td>@item.brand</td> ]]>.Value.Replace("brand", field.ToLower))

            dp.Add(<![CDATA[ $('#brand').val(js['brand']); ]]>.Value.Replace("brand", field))

        Next

        l1.Add(<![CDATA[
<table id="mytable" class="table table-striped table-hover table-bordered">
    <thead class="bg-secondary text-white">
        <tr><TH_HEADER></tr>
    </thead>
    <tbody>
        @if (ViewBag.Brands != null)
        {
            @foreach (MeterBrandModel item in ViewBag.Brands)
            {
                <tr class="editBtn" data-id="@item.id" data-js="@Json.Serialize(item).ToString()" data-bs-toggle="modal" data-bs-target="#mymodal">
                    <TD_DATA>
                </tr>
            }
        }
    </tbody>
</table>
]]>.Value.Replace("mytable", $"{modelName}Table").Replace("ViewBag.Brands", $"ViewBag.{modelName}").Replace("MeterBrandModel", modelName).Replace("mymodal", $"{modelName}Modal").
    Replace("<TH_HEADER>", String.Join(vbCrLf, lh)).
    Replace("<TD_DATA>", String.Join(vbCrLf, lr)))


        l1.Add(<![CDATA[
@section Scripts{
    <!-- section rendered script -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.3/jquery.validate.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.12/jquery.validate.unobtrusive.min.js"></script>

    <script src="https://cdn.datatables.net/2.0.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/searchbuilder/1.7.1/js/dataTables.searchBuilder.js"></script>
    <script src="https://cdn.datatables.net/searchbuilder/1.7.1/js/searchBuilder.dataTables.js"></script>
    <script src="https://cdn.datatables.net/buttons/3.0.2/js/dataTables.buttons.js"></script>
    <script src="https://cdn.datatables.net/buttons/3.0.2/js/buttons.dataTables.js"></script>
    <script src="https://cdn.datatables.net/datetime/1.5.2/js/dataTables.dateTime.min.js"></script>

    <script>
        $(document).ready(function () {
            
            // initialize datatable
            $('#mytable').DataTable({
                // ... other DataTable options
                stateSave: true,
                // Event handler for row hover
                rowCallback: function (row, data, index) {
                    $(row).hover(function () {
                        $(this).css('cursor', 'pointer');
                    }, function () {
                        $(this).css('cursor', 'default');
                    });
                }

            });

            // on edit clicked
            $('#mytable').on('click', '.editBtn', function () {
                var js = $(this).data('js');
                $('#MeterBrandModelFormBody').attr('data-js', JSON.stringify(js));
                $('#MeterBrandModelForm')[0].reset();
                // generated content
                <EDIT_VAL>
            });

        });
    </script>
}

]]>.Value.Replace("mytable", $"{modelName}Table").Replace("MeterBrandModelFormBody", $"{modelName}FormBody").Replace("<EDIT_VAL>", String.Join(vbCrLf, dp)))

        l1.Add("")

        txtDest.Text = String.Join(vbCrLf, l1)

    End Sub

    Private Sub DatatablesBootstrap4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatatablesBootstrap4ToolStripMenuItem.Click


        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList
        modelName = Regex.Replace(modelName, "model", "", RegexOptions.IgnoreCase).Trim

        Dim dialogId = $"{StrConv(modelName.ToLower, VbStrConv.ProperCase)}Modal"
        Dim tableId = $"{StrConv(modelName.ToLower, VbStrConv.ProperCase)}Table"
        Dim formId = $"{StrConv(modelName.ToLower, VbStrConv.ProperCase)}Form"

        Dim css = <![CDATA[
@section css {
    <!-- DataTables -->
    <link rel="stylesheet" href="~/Content/plugins/datatables-bs4/css/dataTables.bootstrap4.min.css">
    <link rel="stylesheet" href="~/Content/plugins/datatables-responsive/css/responsive.bootstrap4.min.css">
    <link rel="stylesheet" href="~/Content/plugins/datatables-buttons/css/buttons.bootstrap4.min.css">
}

]]>.Value.Trim

        Dim js = <![CDATA[
@section scripts {
    <!-- DataTables -->
    <script src="~/Content/plugins/datatables/jquery.dataTables.min.js"></script>
    <script src="~/Content/plugins/datatables-bs4/js/dataTables.bootstrap4.min.js"></script>
    <script src="~/Content/plugins/datatables-responsive/js/dataTables.responsive.min.js"></script>
    <script src="~/Content/plugins/datatables-responsive/js/responsive.bootstrap4.min.js"></script>
    <!-- DataTables Plugins -->
    <script src="~/Content/plugins/datatables-buttons/js/dataTables.buttons.min.js"></script>
    <script src="~/Content/plugins/datatables-buttons/js/buttons.bootstrap4.min.js"></script>
    <script src="~/Content/plugins/jszip/jszip.min.js"></script>
    <script src="~/Content/plugins/pdfmake/pdfmake.min.js"></script>
    <script src="~/Content/plugins/pdfmake/vfs_fonts.js"></script>
    <script src="~/Content/plugins/datatables-buttons/js/buttons.html5.min.js"></script>
    <script src="~/Content/plugins/datatables-buttons/js/buttons.print.min.js"></script>
    <script src="~/Content/plugins/datatables-buttons/js/buttons.colVis.min.js"></script>
}
]]>.Value.Trim

        Dim tableHeaders As New List(Of String)
        Dim datatableColumn As New List(Of String)

        ' generate table
        For i = 0 To props.Count - 1
            Dim line = props(i).Trim
            If String.IsNullOrWhiteSpace(line) Then Continue For
            Dim arr = line.Split(" ")
            Dim type = arr(1).Trim.ToLower
            Dim fieldname = arr(2).Trim

            tableHeaders.Add($"<th>{StrConv(fieldname, VbStrConv.ProperCase)}</th>")

            datatableColumn.Add(<![CDATA[{ data: "column", title: "column", autoWidth: true, searchable: true, orderable: true, },]]>.Value.Replace("column", fieldname))

            Debug.Print("")
        Next

        Dim tableGen As New List(Of String)

        Dim html1 = <![CDATA[
<div class="table-responsive" style="min-height: 65vh;">
    <table class="table table-hover table-sm" id="tableId" style="width:100%">
        <thead>
            <tr class="">
                <th></th>
            </tr>
        </thead>
        <tbody>
        </tbody>
        <tfoot>
            <tr>
                <th></th>
            </tr>
        </tfoot>
    </table>
</div>
]]>.Value.Replace("tableId", tableId).Replace("<th></th>", String.Join(vbCrLf, tableHeaders))

        Dim html3 = <![CDATA[]]>.Value

        Dim html2 = <![CDATA[

function createFunctionName(tableId) {
    let el = typeof (tableId) == 'string' ? $(`#${tableId}`) : tableId
    var table = {
        tableId: tableId,
        init: function() {
            this.data = null;
            this.url = null;
            this.table = el;

            // initialize datatable
            this.table.DataTable().destroy();
            this.datatable = this.table.DataTable({
                initComplete: function (settings, json) {
            
                },
                drawCallback: function (settings) {
            
                },
                rowCallback: function (row, data, index) {
                    $(row).hover(function () {
                        $(this).css('cursor', 'pointer');
                    }, function () {
                        $(this).css('cursor', 'default');
                    });
                },
                stateSave: false,
                responsive: true,
                autoWidth: true,
                ordering: true,
                order: [[1, "desc"]], // index based
                paging: true,
                pageLength: 5,
                lengthChange: true,
                lengthMenu: [
                    [5, 10, 30, 50, -1],
                    [" 5", 10, 30, 50, "All"]
                ],
                // data: , name: , orderable: , autoWidth: , width: , className: 'text-center' , "visible":false
                columns: [
                    // column_list
                ],
                columnDefs: [
             
                ],
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary btn-sm buttons-html5 copy-button', // Custom class for Copy button
                        text: '<i class="fas fa-copy"></i> Copy' // Custom text and icon
                    },
                    {
                        extend: 'csv',
                        className: 'btn btn-secondary btn-sm csv-button', // Custom class for CSV button
                        text: '<i class="fas fa-file-csv"></i> CSV'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary btn-sm excel-button', // Custom class for Excel button
                        text: '<i class="fas fa-file-excel"></i> Excel'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary btn-sm pdf-button', // Custom class for PDF button
                        text: '<i class="fas fa-file-pdf"></i> PDF'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary btn-sm print-button', // Custom class for Print button
                        text: '<i class="fas fa-print"></i> Print'
                    },
                    {
                        extend: 'colvis',
                        className: 'btn btn-secondary btn-sm colvis-button', // Custom class for Column Visibility button
                        text: '<i class="fas fa-eye"></i> Column Visibility'
                    }
                ]
            })
            
            this.datatable.buttons().container().appendTo(`#${tableId}_wrapper .col-md-6:eq(0)`);            
            this.datatable.clear();
            this.datatable.draw();
        },
        loadData: function(url, onInitCallback, onCompleteCallback) {
            this.data = null;
            this.url = url;
            if (typeof (onInitCallback) == 'function') { onInitCallback() }
            return $.ajax({
                url: url,
                type: "GET",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                complete: function (jqXHR, textStatus) {
                    if (typeof (onCompleteCallback) == 'function') { onCompleteCallback(jqXHR, textStatus) }
                },
                success: function (response, textStatus, jqXHR) {
                    if (response != null && response.length > 0) {
                        this.data = response;
                        this.setData(this.data)
                    }
                }.bind(this),
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });

        },
        setData: function(data) {
            this.datatable.clear(); 
            this.datatable.rows.add(data); 
            this.datatable.draw(); 
        },
    }
    return table;
}

]]>.Value.Replace("createFunctionName", $"create{tableId}").Replace("// column_list", String.Join(vbCrLf, datatableColumn)).Trim

        txtDest.Text = html1
        txtDest2.Text = html2

    End Sub

    Private Sub JSPageFunctionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JSPageFunctionsToolStripMenuItem.Click

        Dim ds = NewPageData()
        If IsNothing(ds) Then Return

        Dim modelName = ds(0)
        Dim props = ds(1)
        Dim dialogId = ds(2)
        Dim tableId = ds(3)
        Dim formId = ds(4)


        txtDest.Text = <![CDATA[
function pagescript() {
    /*
    * form functions
    */
    let table = {}
    let seldata = {}

    function init() {
        // listiner , initializer
        // crud
        $(document).on("click", "[data-action=add]", function (e) {
            onAddClick()
        })
        $(document).on("click", "[data-action=edit]", function (e) {
            onEditClick()
        })
        $(document).on("click", "[data-action=delete]", function (e) {
            onDeleteClick()
        })
        // dialog
        $(document).on("click", "[data-action=close]", function (e) {
            hideDialog()
        })
        $(document).on("click", "[data-action=save]", function (e) {
            onSaveClick()
        })

        $(document).on("dblclick", "#trx_table", function (e) {
            
        })
        // modal
        $(document).on('shown.bs.modal', `.modal`, function (e) {
            $(`form:has(.modal) input:visible`)[0].focus()
        });
        $(document).on('hide.bs.modal', `.modal`, function (e) {
            onResetClick()
        });

        // select
        $(document).on('select2:open', function (e) {
            document.querySelector('.select2-search__field').focus();
        });

        $(`select.select2`).select2({
            placeholder: {
                id: '-1',
                text: 'Select option'
            },
            allowClear: true,
            // tags: true,              // for text drop down (non id field)
            parent: $(`.modal`)
        })
    }

    function onItemClick(data) {
        seldata = data[0]
    }

    function onAddClick() {
        onResetClick()
        // button clicked
        // verify if allowed
        // show dialog
        $(`.modal h4`).html(`<i class="fas fa-file-signature"></i> Add New`)
        showDialog()
    }

    function onEditClick(id) {
        onResetClick()
        // button clicked
        // verify if allowed
        // load id
        // show dialog
        $(`.modal h4`).html(`<i class="fas fa-file-signature"></i> Edit Details`)
        fillData(seldata)
        showDialog()
    }

    function onDeleteClick(id) {
        // button clicked
        // verify if allowed
        // verify id
        // confirm dialog
        console.log("delete")
    }

    function onSaveClick() {
        if (!isValid()) { return; }

        var model = {

        }

        // compare data
        var exists = $(`form:has(.modal)`).data('data')
        if (exists != null) {
            if (isShallowEqual(model, exists)) {
                hideDialog()
                return;
            }
        }

        ShowSwalLoader()

        return $.ajax({
            url: exists == null ? `/{controller}/insert` : `/{controller}/{update}`,
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            complete: function (jqXHR, textStatus) {
                if (textStatus != 'error') {
                    setTimeout(() => {
                        CloseSwalLoader();
                    }, 800);
                }
            },
            success: function (response, textStatus, jqXHR) {
                toastr.success('Changes was saved successfuly', null, { timeOut: 1300 })
                getTableData()
                hideDialog()
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal("Error!", "Something went wrong with the request...", "error");
                return;
            }
        });

    }

    function onResetClick() {
        $(`form:has(.modal)`).data('data', null)
        $(`form:has(.modal)`)[0].reset()
        $('form:has(.modal)').validate().resetForm();
        $(`form:has(.modal) select.select2`).each((i, e) => {
            $(e).val(null).trigger('change')
        })
    }

    function showDialog() {
        $(`.modal`).modal('show')
    }

    function hideDialog() {
        $(`.modal`).modal('hide')
    }

    function fillData(data) {
        $(`form:has(.modal)`).data('data', data)
        $(`form:has(.modal) input, form:has(.modal) select`).each((i, e) => {
            const $el = $(e);
            const tag = e.localName;
            const type = (e.type || '').toLowerCase();
            const name = e.name || '';

            var v = getValue(data, e.name)
            if (!v) return; // skip if no value found

            if (tag === 'input') {
                if (type === 'text') {
                    $el.val(v.trim?.() || '');

                } else if (type === 'number') {           
                    $el.val(Number(v));

                } else if (type === 'radio') {
                    $(`input[name="${name}"][value="${v}"]`).prop('checked', true);

                } else if (type === 'checkbox') {
                    $el.prop('checked', !!v);

                } else if (e.type.includes('date')) {
                    $el.val(v.trim?.() || '');
                }
            } else if (tag === 'select') {
                $el.val(v.trim?.() || '-1').trigger('change');

            } else {

            }
        })    
    }

    function isDirty() {
        // compare data to form content
        return false;
    }

    function isValid() {
        return $(`form:has(.modal)`).valid()
    }

    function validator() {
        var form = $(`form:has(.modal)`)
        form.validate({
            ignore: `:hidden, .ignore`,
            errorElement: 'div', // or 'span' or any tag you want
            errorClass: 'invalid-feedback', // Bootstrap-compatible
            highlight: function (element) {
                $(element).addClass('is-invalid');
                if ($(element).hasClass('select2-hidden-accessible')) {
                    $(element).next('.select2').find('.select2-selection').addClass('is-invalid');
                }
            },
            unhighlight: function (element) {
                $(element).removeClass('is-invalid');
                if ($(element).hasClass('select2-hidden-accessible')) {
                    $(element).next('.select2').find('.select2-selection').removeClass('is-invalid');
                }
            },
            errorPlacement: function (error, element) {
                if (element.closest('.input-group').length) {
                    error.insertAfter(element.closest('.input-group')); // for inputs with icons

                } else if (element.is(':checkbox') || element.is(':radio')) {
                    error.appendTo(element.closest('.form-check, .form-group'));

                } else if (element.hasClass('select2-hidden-accessible')) {
                    error.insertAfter(element.next('.select2')); // place after select2 container

                } else {
                    error.insertAfter(element); // default

                }
            }
        });

        // additional rules
        form.find(`input[name=workstationid]`).rules('add', {
            required: true,
            minlength: 3,
            messages: {
                required: "workstationid is required",
                minlength: "Minimum 3 characters required",
            }
        });

        // Remove error state when a selection is made
        form.find(`select.select2`).on('select2:select select2:clear change', function () {
            if ($(this).valid()) {
                $(this).removeClass('is-invalid');
                $(this).next('.select2').find('.select2-selection').removeClass('is-invalid');
            }
        });

    }

    /*
     * Table Functions
     */

    function initTable() {
        $('#trx_table').DataTable().destroy();
        table = $("#trx_table").DataTable({
            // todo: init datatable
        })

        table.buttons().container().appendTo('#trx_button_wrapper');
        table.clear(); // Clear existing data
        table.draw(); // Redraw the table

        initSingleRowSelect(table, (data) => {
            if ($(`[data-action=edit]`).length > 0) $(`[data-action=edit]`)[0].disabled = data.length == 0
            if ($(`[data-action=edit]`).length > 0) $(`[data-action=delete]`)[0].disabled = data.length == 0
            if (data?.length > 0 || 0) { onItemClick(data) }
        })
    }

    function getTableData(q) {
        return $.ajax({
            url: "/{controller}/{action}/",
            type: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            complete: function (jqXHR, textStatus) {

            },
            success: function (response, textStatus, jqXHR) {
                if (response != null && response.length > 0) {
                    setTableData(response)
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                swal("Error!", "Something went wrong with the request...", "error");
                return;
            }
        });
    }

    function setTableData(data) {
        table.clear();
        table.rows.add(data);
        table.draw();
    }

    init()
    initTable()

    // ajax loader
    $.when(
        ShowSwalLoader(),
        // table
        getTableData(),
        // sample dropdown
        // cboRevenueCenters($(`#revcenterid`))
    ).done(() => {
        setTimeout(() => {
            CloseSwalLoader();
        }, 800);
    })

}

        ]]>.Value.Trim
    End Sub

    Private Sub JSObjectLiteralToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JSObjectLiteralToolStripMenuItem.Click
        txtDest.Text = <![CDATA[
function pagescript() {
    let pg = {

        /*
         * form functions
         */

        init: function() {
            // listiner , initializer
            this.initTable()
        },
    
        onAddClick: function() {
            // button clicked
            // verify if allowed
            // show dialog
        },
    
        onEditClick: function(id) {
            // button clicked
            // verify if allowed
            // load id
            // show dialog
        },
    
        onDeleteClick: function(id) {
            // button clicked
            // verify if allowed
            // verify id
            // confirm dialog
        },
    
        onSaveClick: function() {
            // button clicked
            // validate data
            // save data
        },
    
        onResetClick: function() {
            // button clicked
            // reset data
        },
    
        showDialog: function() {
    
        },
    
        hideDialog: function() {
    
        },
    
        fillData: function(data) {
            // fill form data    
        },
    
        isDirty: function() {
            // compare data to form content
            return false;
        },
    
        isValid: function() {
            return false;
        },
    
        /*
         * Table Functions
         */
    
        initTable: function() {
            // datatable initializer
            console.log("initTable")
            this.getTableData()
        },

        getTableData: function(q) {
            // ajax call
            console.log("getTableData")
            this.setTableData()
        },
    
        setTableData: function(data) {
            console.log("setTableData")            
        },
    }
    
    return pg;
}

let app = pagescript()
app.init()

        ]]>.Value.Trim
    End Sub

End Class
