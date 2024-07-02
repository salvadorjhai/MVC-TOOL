Public Class frmCreateApiController

    Property GeneratedCode As String = ""

    Private Sub frmCreateApiController_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click

        Dim l1 As New List(Of String)
        Dim l2 As New List(Of String)({"/* this api routes and action:", ""})

        'l1.Add(<![CDATA[ using Microsoft.AspNetCore.Mvc; ]]>.Value)
        'l1.Add(<![CDATA[ using System.Diagnostics; ]]>.Value)

        l1.Add(<![CDATA[ [Route("api/[controller]")] ]]>.Value.Replace("api/[controller]", cboRoute.Text))
        l1.Add(<![CDATA[ [ApiController] ]]>.Value)
        l1.Add(<![CDATA[ public class ProductsController : Controller { ]]>.Value.Replace("ProductsController", $"{txtControllerName.Text}Controller"))
        'l1.Add(<![CDATA[ private readonly ILogger<ProductsController> _logger; ]]>.Value.Replace("ProductsController", $"{txtControllerName.Text}Controller"))
        'l1.Add("")
        'l1.Add(<![CDATA[ public ProductsController(ILogger<ProductsController> logger) { _logger = logger; } ]]>.Value.Replace("ProductsController", $"{txtControllerName.Text}Controller"))
        l1.Add("")

        For i = 0 To txtModels.Lines.Count - 1
            Dim v = txtModels.Lines(i).Trim

            Dim r = cboRoute.Text.Replace("[controller]", txtControllerName.Text.ToLower)
            If r.EndsWith("/") Then r.Substring(0, r.Length - 1)

            If String.IsNullOrWhiteSpace(v) Then Continue For

            l1.Add(<![CDATA[ #region "Account Types" ]]>.Value.Replace("Account Types", v))
            l1.Add(<![CDATA[  ]]>.Value)

            If i = 0 Then
                l1.Add(<![CDATA[ // nothing to see on root right? ]]>.Value)
                l1.Add(<![CDATA[ [Route("")] ]]>.Value)
                l1.Add(<![CDATA[ [Route("index")] ]]>.Value)
                l1.Add(<![CDATA[ public string Index() { return "Nothing to see here ... "; } ]]>.Value)
                l1.Add("")
            End If

            If chkList.Checked Then
                l2.Add($" {r}/{v}".ToLower & " - GET method to return a list ")
                l1.Add(<![CDATA[ // GET: api/products/brands ]]>.Value.Replace("api/products/brands", $"{r}/{v}").ToLower)
                l1.Add(<![CDATA[ [Route("brands")] ]]>.Value.Replace("brands", v.ToLower))
                l1.Add(<![CDATA[ public ActionResult GetProducts() { ]]>.Value.Replace("GetProducts", $"Get{v}"))
                l1.Add(<![CDATA[      return Json(new {action= "GetProducts", result="OK", message = ""}); ]]>.Value.Replace("GetProducts", $"Get{v}"))
                l1.Add(<![CDATA[ } ]]>.Value)
            End If

            l1.Add("")

            If chkView.Checked Then
                l2.Add($" {r}/{v}/{{id:int}}".ToLower & " - GET method to return a specific object ")
                l1.Add(<![CDATA[ // GET: api/products/brands/{id:int} ]]>.Value.Replace("api/products/brands", $"{r}/{v}").ToLower)
                l1.Add(<![CDATA[ [Route("brands/{id:int}")] ]]>.Value.Replace("brands", v.ToLower))
                l1.Add(<![CDATA[ public ActionResult GetProductById(int id) { ]]>.Value.Replace("GetProductById", $"Get{v}ById"))
                l1.Add(<![CDATA[      return Json(new {action= $"GetProductById {id}", result="OK", message = ""}); ]]>.Value.Replace("GetProductById", $"Get{v}ById"))
                l1.Add(<![CDATA[ } ]]>.Value)
            End If

            If chkFind.Checked Then
                l2.Add($" {r}/{v}/find/?q=".ToLower & " - GET method to search for specific object ")
                l1.Add(<![CDATA[ // GET: api/products/brands/find ]]>.Value.Replace("api/products/brands", $"{r}/{v}").ToLower)
                l1.Add(<![CDATA[ [Route("brands/find")] ]]>.Value.Replace("brands", v.ToLower))
                l1.Add(<![CDATA[ public ActionResult FindProductBy([FromQuery] string q) { ]]>.Value.Replace("FindProductBy", $"Find{v}By"))
                l1.Add(<![CDATA[      return Json(new {action= $"FindProductBy {q}", result="OK", message = ""}); ]]>.Value.Replace("FindProductBy", $"Find{v}By"))
                l1.Add(<![CDATA[ } ]]>.Value)
            End If

            l1.Add("")

            If chkUpsert.Checked Then

                l2.Add($" {r}/{v}/upsert".ToLower & " - POST method to upsert (update/insert) (via Request.Form) (application/x-www-form-urlencoded)")
                l1.Add(<![CDATA[ // POST: api/products/brands/Upsert ]]>.Value.Replace("api/products/brands", $"{r}/{v}").ToLower)
                l1.Add(<![CDATA[ [Route("brands/upsert")] ]]>.Value.Replace("brands", v.ToLower))
                l1.Add(<![CDATA[ [HttpPost] ]]>.Value.Replace("brands", v.ToLower))
                l1.Add(<![CDATA[ public ActionResult UpsertProduct([FromForm] ExpandoObject? dummyForm) { ]]>.Value.Replace("UpsertProduct", $"Upsert{v}"))
                l1.Add(<![CDATA[      // custom binding from dictionary ]]>.Value)
                l1.Add(<![CDATA[      // var model = HelperUtils.BindFrom<ProductModel>(Request.Form); ]]>.Value)
                l1.Add(<![CDATA[      // dont forget to do some server side validation]]>.Value)
                l1.Add(<![CDATA[      
                    //if (string.IsNullOrWhiteSpace(model.name))
                    //{
                    //    return Json(new { action = $"UpsertProduct", result = "err", message = "Please fill up all required field." });
                    //}
                    //// saving records (also needs validations
                    //int res = TownsDataAccess.Upsert(model);
                    //if (res == DB.DUPLICATE)
                    //{
                    //    return Json(new { result = "duplicate" });
                    //}
                    //else if (res == DB.NO_CHANGES)
                    //{
                    //    return Json(new { result = "no changes" });
                    //}
                            ]]>.Value.Replace("UpsertProduct", $"Upsert{v}"))
                l1.Add(<![CDATA[      return Json(new {action= $"UpsertProduct", result="OK"}); ]]>.Value.Replace("UpsertProduct", $"Upsert{v}"))
                l1.Add(<![CDATA[ } ]]>.Value)

            End If

            l1.Add("")
            l1.Add(<![CDATA[ #endregion ]]>.Value)

            l1.Add("")
            l2.Add("")

        Next

        l2.Add(" [FromForm] int id, [FromForm] string name - should match your form")
        l2.Add(" [FromQuery] string q - using url query ex. customers/find/?q=abc")
        l2.Add(" or change to [FromBody] Model model if you want to use model binding (JSON/XML)")
        l2.Add("")
        l2.Add(" you can also watch for REQUEST to check for more info.")
        l2.Add("")
        l2.Add("*/")
        l2.Add("")

        l1.Add(<![CDATA[ } ]]>.Value)

        l1.InsertRange(0, l2)

        GeneratedCode = String.Join(vbCrLf, l1)

        Me.DialogResult = DialogResult.OK

    End Sub

End Class