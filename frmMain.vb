﻿Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class frmMain
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath)
        Me.CenterToScreen()
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click

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

<div class="modal fade" id="mymodal" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modaltitle" aria-hidden="true">
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

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click

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

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click

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
		/// <returns></returns>
        public List<TownModel> List()
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
                DataRow exists = DB.QuerySingleResult("b_towns", "name", model.name);
                if (exists != null) { return OleDB.DUPLICATE; }

                // prepare 
                Dictionary<string, object> param = Utils.HelperUtils.ToDictionary(model);
                param["madebyid"] = model.updatedbyid;
                param["madedate"] = model.lastupdated.Value;
                param["updatedbyid"] = model.updatedbyid;
                param["lastupdated"] = model.lastupdated.Value;

                // insert
                // return DB.InsertParam("b_towns", param);
                var resId = DB.InsertParam("b_towns", param, true);
                if (resId > 0)
                {
                    // return updated model (with id)
                    var exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE id={resId}", param);
                    if (DB.LastError != null || exists == null) { return OleDB.EXEC_ERROR; }
                    model = HelperUtils.BindFrom<TownModel>(exists);
                }
                return resId;

            } else
            {
                // for updating

                // verify id
                DataRow exists = DB.QuerySingleResult("b_towns", "id", model.id);
                if (exists != null)
                {
                    // return if nothing to update
                    if (exists.IsModelClean(model))
					{
						return OleDB.NO_CHANGES;
					}

                    //if (exists["code"].ToString().ToLowerInvariant() == model.code.ToLowerInvariant())
                    //{
                    //    return OleDB.NO_CHANGES;
                    //}

                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param["code"] = model.code;

                    // check for duplicate
                    exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE code=@code AND id <> {model.id}", param);
                    if (exists != null)
                    {
                        return OleDB.DUPLICATE;
                    }

                    // prepare 
                    param = Utils.HelperUtils.ToDictionary(model);
                    param.Remove("madedate"); // NOT NEEDED for update
                    param.Remove("madebyid"); // NOT NEEDED for update
                    param["updatedbyid"] = model.updatedbyid;
                    param["lastupdated"] = model.lastupdated.Value;

                    // update
                    // return DB.UpdateParam("b_towns", $"WHERE id={model.id}", param);
                    var resId = DB.UpdateParam("b_towns", $"WHERE id={model.id}", param);
                    if (resId > 0)
                    {
                        // return updated model (with id)
                        exists = DB.QuerySingleResult($"SELECT * FROM b_towns WHERE id={model.id}", param);
                        if (DB.LastError != null || exists == null) { return OleDB.EXEC_ERROR; }
                        model = HelperUtils.BindFrom<TownModel>(exists);
                    }
                    return resId;

                }
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

        'Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault
        'Dim props = txtSource.Lines.Where(Function(x) x.Contains("public ") And x.Contains(" class ") = False).ToList

        Dim l1 As New List(Of String)
        l1.Add("public class TownModel {".Replace("Town", tableName))
        l1.Add("")
        For i = 1 To txtSource.Lines.Count - 1

            '[comid] [varchar](10)
            '[birthdate] [date] NULL,
            '[age] [Decimal](18, 2) NULL,

            Dim mm = Regex.Match(txtSource.Lines(i).Trim, "\[(.*?)\] \[(.*?)\](:?\((.*?)\))?")
            'If mm.Success = False Then mm = Regex.Match(txtSource.Lines(i).Trim, """(.*?)"" (.*?)(:?\((.*?)\))?")
            If mm.Success = False Then Continue For

            Dim ddFieldOrig = mm.Groups(1).Value.Trim
            Dim ddField = Regex.Replace(mm.Groups(1).Value.Trim, "[^a-z0-9]", "_", RegexOptions.IgnoreCase)
            Dim ddType = mm.Groups(2).Value.Trim.ToLower
            Dim ddRanged = mm.Groups(4).Value.Trim

            Dim l2 As New List(Of String)

            If ddType.Contains("char") Then
                If String.IsNullOrWhiteSpace(ddRanged) = False Then
                    l2.Add(<![CDATA[ [MaxLength(10)] ]]>.Value.Replace("10", ddRanged))
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

            ElseIf ddType.Contains("numeric") Or ddType.Contains("decimal") Or ddType.Contains("float") Then
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

        txtDest.Text = String.Join(vbCrLf, l1)

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

<div class="modal fade" id="mymodal" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modaltitle" aria-hidden="true">
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
<div class="modal fade" id="mymodal" data-bs-backdrop="static" tabindex="-1" aria-labelledby="modaltitle" aria-hidden="true">
    <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="modaltitle">Popup Dialog</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body ms-2 me-2" data-js="" id="mymodalBody">
                    <!-- Content Goes Here -->
                </div>
                <div class="modal-footer">
                    <input type="reset" class="btn btn-outline-secondary" value="Clear" />
                    <button id="btnClose_mymodal" type="button" class="btn btn-secondary m-1" data-bs-dismiss="modal">Close</button>
                    <input type="submit" class="btn btn-primary" value="Save changes"/>
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

        Dim l1 As New List(Of String)
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

        txtDest.Text = String.Join(vbCrLf, l1)

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
                var model = {
                    <FORM_DATA>
                }
                $.ajax({
                    type: "POST",
                    url: "../api/products/upsert",
                    data: JSON.stringify(model),
                    contentType: "application/json;charset=UTF-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.result === "Success") {
                            $('#mymodal').modal('hide');
                            swal({
                                title: "Saved!",
                                text: "Data has been saved." + "\n",
                                type: "success",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "OK",
                                closeOnConfirm: false
                            },
                                function () {
                                    window.location = "/Meter/MeterTypes";
                                }
                            );

                        } else {
                            swal("Error", "An error occured: " + response.result + "\n", "warning");
                        }                        

                    },
                    error: function (xhr, status, error) {
                        console.error("Error:", error);
                    }
                });
]]>.Value.Replace("ProductModelForm", modelName & "Form").Replace("mymodal", $"{modelName}Modal").Replace("<FORM_DATA>", String.Join("," & vbCrLf, formData))

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
<div class="modal fade" id="mymodal" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="mymodalTitle" aria-hidden="true">
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
                dp.Add(<![CDATA[ $('#Item1_brand').val(js['brand']).trigger('change'); ]]>.Value.Replace("brand", field))

                sl.Add(<![CDATA[
                function populatebrandCbo() {
                    $.ajax({
                        url: "/{Controller}/{Action}/",
                        type: "GET",
                        contentType: "application/json;charset=UTF-8",
                        dataType: "json",
                        success: function (result) {
                            setTimeout(function () {
                                $('#Item1_brand').empty();
                                $('#Item1_brand').append("<option value></option>");
                                for (var i = 0; i < result.data.length; i++) {
                                    var Desc = result.data[i]['name'];
                                    var opt = new Option(Desc, result.data[i]['id']);
                                    $('#Item1_brand').append(opt);
                                }
                            },1)
                        },
                        error: function (errormessage) {
                            swal("Error", "Oops! something went wrong ... \n", "error");
                        }
                    });
                }
                ]]>.Value.Replace("brand", field).Replace("mymodal", $"{modelName}Modal"))

                sl2.Add(<![CDATA[ $('#Item1_brand').val(null).trigger('change'); ]]>.Value.Replace("brand", field))
                sl3.Add(<![CDATA[ $('#Item1_brand').on('change', function () {
                            // do what you want ;
                        }); 
                    ]]>.Value.Replace("brand", field))

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
                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val() ]]>.Value.Replace("brand", field).TrimEnd)
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
                l3.Add(<![CDATA[ <div class="mb-2 form-check"> ]]>.Value)
                l3.Add(<![CDATA[  @Html.CheckBoxFor(m => m.Item1.brand, new { @class = "form-check-input" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-check-label" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
                l3.Add(<![CDATA[ </div> ]]>.Value)
                '
                formData.Add(<![CDATA[ formData.append("brand", $('#Item1_brand').prop('checked')); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').prop('checked') ]]>.Value.Replace("brand", field).TrimEnd)

            ElseIf ddt.Contains("int") Or ddt.Contains("decimal") Or ddt.Contains("double") Then

                If field.ToLower.EndsWith("id") Or field.ToLower.EndsWith("code") Then
                    l3.Add(<![CDATA[ <div class="mb-2 col-6"> ]]>.Value)
                    l3.Add(<![CDATA[  @Html.LabelFor(m => m.Item1.brand, new { @class = "form-label" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.DropDownListFor(m => m.Item1.brand, new SelectList(new List<string>()), new { @class = "form-control form-select select2 custom-select2", @placeholder = "select option" }) ]]>.Value.Replace("brand", field))
                    l3.Add(<![CDATA[  @Html.ValidationMessageFor(m => m.Item1.brand, "", new { @class = "text-danger" }) ]]>.Value.Replace("brand", field))
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

                formData.Add(<![CDATA[ formData.append("brand", $("#Item1_brand").val()); ]]>.Value.Replace("brand", field))
                formdata2.Add(<![CDATA[ brand: $('#Item1_brand').val() ]]>.Value.Replace("brand", field).TrimEnd)

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
@*---------- Datatable -----------*@
<div class="row">
    <div class="col-12">
        <div class="card">
            <div class="card-header">
                <h4>@ViewBag.Title</h4>
            </div>
            <div class="card-body">
                <button type="button" class="btn btn-icon icon-left btn-primary text-uppercase mb-3" onclick="showmymodal()"><span class="fas fa-plus-square"></span> Add New </button>
                <table class="table table-striped" id="mytable" style="width:100%">
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
    <div class="modal fade" id="mymodal" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="mymodalTitle" aria-hidden="true">
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
                        <button type="submit" class="btn btn-success"><span class="far fa-thumbs-up"></span> SAVE</button>
                        <button type="button" class="btn btn-danger" onclick="closeFormIfDirty(this)"><span class="far fa-thumbs-down"></span> CLOSE</button>
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
        success: function (response) {
            var msg;
            if (response.result == null) {
                msg = response.toLowerCase();
            } else {
                msg = response.result.toLowerCase();
            }
            if (msg.includes("success")) {
                $('#mymodal').modal('hide');
                loadmytable();
                swal("Saved!", "Record has been saved", "success");
            } else if (msg.includes("nochange")) {
                $('#mymodal').modal('hide');
            } else {
                swal("Error", "An error occured: " + msg + "\n", "warning");
            }
        },
        error: function (errormessage) {
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
                    success: function (response) {
                        var msg;
                        if (response.result == null) {
                            msg = response.toLowerCase();
                        } else {
                            msg = response.result.toLowerCase();
                        }
                        if (msg.includes("success")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                            loadmytable();
                            swal("Saved!", "Record has been saved", "success");
                        } else if (msg.includes("nochange")) {
                            $('#mymodal').find('form').data('isDirty', false);
                            $('#mymodal').modal('hide');
                        } else {
                            swal("Error", "An error occured: " + msg + "\n", "warning");
                        }
                    },
                    error: function (errormessage) {
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
            loadmytable();
            appendRequiredLabel();
            <SELECT_EVENTS>
            <CHECK_EVENTS>
            
            // on modal shown 
            $('#mymodal').on('shown.bs.modal', function () {
                $('#Item1_myInput').trigger('focus');
            });

            // on modal closed , clean all need fields
            $('#mymodal').on('hidden.bs.modal', function () {
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
                    swal({
                        title: "Something went wrong!",
                        text: "Please fill all required field to proceed.",
                        icon: "error",
                        dangerMode: true,
                    });
                    return;
                } 
                clearFormValidation();
                saveProductModelForm();
            });

        }

        function loadmytable() {
            $('#mytable').DataTable().destroy();
            $('#mytable').DataTable({
                dom:
                    "<'row'<'p-2 col-sm-12 col-md-6 col-xl-6'l><'float-right pr-3 pt-3 p-2 col-sm-12 col-md-6 col-xl-6'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'pl-2 pt-0 pb-2 col-sm-12 col-md-5'i><'pt-1 pb-1 pr-2 col-sm-12 col-md-7'p>>",
                stateSave: true,
                ajax: {
                    "url": "/{Controller}/{Action}",
                    "type": "GET",
                    datatype: "json",
                    error: function (errormessage) {
                        swal("Cant Connect?", "Failed to load datatable ...", "error");
                    }
                },
                pageLength: 10,
                order: [[1, "asc"]], // index based
                lengthMenu: [
                    [5, 10, 30, 50, -1],
                    [" 5", 10, 30, 50, "All"]
                ],
                autoWidth: true,
                initComplete: function (settings, json) {
                    document.body.style.cursor = 'default';
                },
                columns: [
                    // data: , name: , orderable: , autoWidth: , width: , className: 'text-center' , "visible":false
                    <DT_COL_DEF>
                ],
                aoColumnDefs: [
                    {
                        "width": "100px",
                        "aTargets": [0], // target column
                        "mData": "id", // target data
                        "bSortable":false,
                        "mRender": function (data, type, full, meta) {

                            return '<a class="btn btn-primary btn-sm" style="font-size:smaller;" href="#" id="vw_' + data + '" ' +
                                'onclick="showEditmymodal(\'' + data + '\')">' +
                                '<span class="fas fa-edit"></span> EDIT</a> ';

                        },
                        "className": "text-center"
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
                success: function (result) {
                    if (result.data != null) {
                        fillProductModelForm(result.data);
                        $('#mymodal h5').text('EDIT DETAILS');
                        $('#mymodal').modal('show');
                        $('#mymodal').find('form').data('isDirty', false);
                    } else {
                        swal("Error!", "There are no details to display.\n", "warning");
                    }
                },
                error: function (errormessage) {
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
            Replace("<SELECT_EVENTS>", String.Join(vbCrLf, sl3).Trim).
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
                                <a class="nav-link active" data-bs-toggle="tab" href="#tabPane1" id="tab1">Pane1</a>
                            </li>]]>.Value.Trim.Replace(" active", IIf(i > 1, "", " active")).Replace("Pane1", "Pane" & i).Replace("tab1", "tab" & i)
            l1.Add(str)

            str = <![CDATA[<div class="tab-pane container-fluid p-2 active" id="tabPane1">
                                <!-- content goes here -->
                            </div>]]>.Value.Trim.Replace(" active", IIf(i > 1, "", " active")).Replace("Pane1", "Pane" & i)
            l2.Add(str)

            str = <![CDATA[$('#tab1').on('shown.bs.tab', function (e) {
                                    // per tab scripts goes here
                                });]]>.Value.Trim.Replace("tab1", "tab" & i)
            l3.Add(str)

        Next


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



        '        l1.Add(<![CDATA[<ul class="nav nav-tabs">
        '                            <li class="nav-item">
        '                                <a class="nav-link active" data-bs-toggle="tab" href="#tabPane1" id="tab1">General</a>
        '                            </li>
        '                            <li class="nav-item">
        '                                <a class="nav-link" data-bs-toggle="tab" href="#tabPane2" id="tab1">Membership</a>
        '                            </li>
        '                            <li class="nav-item">
        '                                <a class="nav-link" data-bs-toggle="tab" href="#tabPane3" id="tab1">Account</a>
        '                            </li>
        '                        </ul>

        '                        <div class="tab-content">
        '                            <div class="tab-pane container-fluid p-2 active" id="tabPane1">
        '                                <!-- content goes here -->
        '                            </div>

        '                            <div class="tab-pane container-fluid p-2 fade" id="tabPane2">
        '                                <!-- content goes here -->
        '                            </div>

        '                            <div class="tab-pane container-fluid p-2 fade" id="tabPane3">
        '                                <!-- content goes here -->
        '                            </div>
        '                        </div>

        '                        <script>
        '                            $(document).ready(function(){
        '                                $('#tab1').on('shown.bs.tab', function (e) {
        '                                    // per tab scripts goes here
        '                                });     

        '                                $('#tab2').on('shown.bs.tab', function (e) {
        '                                    // per tab scripts goes here
        '                                });          

        '                                $('#tab3').on('shown.bs.tab', function (e) {
        '                                    // per tab scripts goes here
        '                                });                                                    
        '                            });
        '                        </script>
        ']]>.Value)

        '        txtDest.Text = String.Join(vbCrLf, l1).Trim

    End Sub

    Private Sub ControllerBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ControllerBuilderToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault

        Dim template = <![CDATA[
        // get: api/members
        [HttpGet("members")]
        public ActionResult list()
        {
            List<MemberModel> list = new MemberModelDataAccess(_connectionString).List();
            if (list != null && list.Count >= 0) { return Ok(list); }
            return BadRequest();
        }

        // get: api/members/{id}
        [HttpGet("members/{id:int}")]
        public ActionResult getbyid(int id)
        {
            MemberModel model = new MemberModelDataAccess(_connectionString).GetById(id);
            if (model != null) { return Ok(model); }
            return NotFound();
        }

        // post: api/members/upsert
        [HttpPost("members/upsert")]
        public IActionResult upsert(MemberModel data)
        {
            //MemberModel data = HelperUtils.BindFrom<MemberModel>(Request.Form);
            var res = new MemberModelDataAccess(_connectionString).Upsert(ref data);
            if (res > 0) { return Ok(new {result = "success" , data = data}); }
            if (res == OleDB.NO_CHANGES) { return Ok(new { result = "nochange", data = data }); }
            if (res == OleDB.DUPLICATE) { return Conflict(); }
            return BadRequest("failed");
        }
]]>.Value

        txtDest.Text = template.Replace("MemberModel", modelName).Trim.
            Replace("members", modelName.ToLower)



    End Sub

    Private Sub UIControllerBuilderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UIControllerBuilderToolStripMenuItem.Click

        If txtSource.Text.Contains("public class") = False Then
            txtDest.Text = "You forgot your Model ..."
            Return
        End If

        Dim modelName = txtSource.Lines.Where(Function(x) x.Contains("public class ")).FirstOrDefault.Trim.Split(" ").LastOrDefault

        Dim template = <![CDATA[
        string baseURL = ConfigurationManager.AppSettings.Get("APISERVER");

        #region "MemberModel"
        public async Task<string> list()
        {
            Response.ContentType = "application/json";
            List<MemberModel> model = new List<MemberModel>();
            string results = await HelperUtils.API_GET(baseURL + "api/uom");
            if (!string.IsNullOrWhiteSpace(results))
                model = JsonConvert.DeserializeObject<List<MemberModel>>(results);

            return Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model });
        }

        public async Task<string> getbyid(int id)
        {
            Response.ContentType = "application/json";
            MemberModel model = new MemberModel();
            string results = await HelperUtils.API_GET(baseURL + $"api/uom/{id}");
            if (!string.IsNullOrWhiteSpace(results))
                model = JsonConvert.DeserializeObject<MemberModel>(results);

            return Newtonsoft.Json.JsonConvert.SerializeObject(new { data = model });
        }

        [HttpPost]
        public async Task<string> upsert(MemberModel model)
        {
            Dictionary<string, object> activeuser = HelperUtils.GetActiveUser();
            model.updatedby = activeuser["id"].ToString().ParseInt();
            model.lastupdated = DateTime.Now;

            string msg = string.Empty;
            var myContent = JsonConvert.SerializeObject(model);
            var res = await HelperUtils.API_POST2(baseURL + "api/uom/upsert", myContent);
            msg = res.Content.ReadAsStringAsync().Result;
            if (!res.IsSuccessStatusCode)
            {
                msg = res.ReasonPhrase.ToString();
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { result = msg });
            }
            return msg;

        }
        #endregion
]]>.Value

        txtDest.Text = template.Replace("MemberModel", modelName).Trim.
            Replace("members", modelName.ToLower)


    End Sub
End Class
