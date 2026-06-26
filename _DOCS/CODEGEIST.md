# MVC-TOOL — Codebase Guide for LLMs & Developers

> **Note**: This file is intended for AI coding assistants and developers onboarding to the MVC-TOOL project. It provides a structured overview of the codebase, architecture patterns, and key implementation details.

---

## 1. Project Identity

| Property | Value |
|---|---|
| **Name** | MVC-TOOL |
| **Description** | Code generation workbench for ASP.NET MVC / ASP.NET Core projects |
| **Language** | Visual Basic .NET (.NET Framework 4.8.1) |
| **Project Type** | Windows Forms Application (WinExe) |
| **Root Namespace** | `MVC_TOOL` |
| **Startup Object** | `MVC_TOOL.My.MyApplication` |
| **Main Form** | `frmMain` |
| **Platform Target** | x64 (Debug & Release) |
| **Solution File** | `MVC-TOOL.sln` |
| **Project File** | `MVC-TOOL.vbproj` |
| **Assembly Version** | 1.0.0.0 |
| **Company** | jhapps |
| **Copyright** | © 2024 jhapps |
| **Icon** | `Shlyapnikova-Application-Pencil.ico` |
| **Language Version** | Latest |
| **Total Forms** | 10 |
| **Total Lines (approx.)** | ~13,000+ across all files |

---

## 2. Project Structure

```
MVC-TOOL/
├── MVC-TOOL.sln                 # Solution file
├── MVC-TOOL.vbproj              # Project file (.NET 4.8.1, WinForms, VB.NET)
├── App.config                   # Runtime config (framework version)
├── packages.config              # NuGet package references
├── README.md                    # Brief description (currently minimal)
│
├── frmMain.vb                   # ~10,183 lines — Main application window & ALL code gen logic
├── frmMain.Designer.vb          # Designer-generated UI code
├── frmMain.resx                 # Form resources (icons, strings)
│
├── frmAjaxGenerator.vb          # AJAX call code generator dialog
├── frmCreateApiController.vb    # API Controller generator (basic)
├── frmCreateApiControllerCJ.vb  # API Controller generator (CJ template variant)
├── frmCreateModal.vb            # Simple modal name input dialog
├── frmSQL.vb                    # SQL Connection string builder dialog
├── frmSQLBulkCopy.vb            # Standalone Bulk Copy tool (source↔destination)
├── frmSQLEditor.vb              # Minimal text editor dialog (SQL/connection strings)
├── frmTableName.vb              # Table name input dialog
├── frmTuple.vb                  # Tuple index selector dialog (Item1–Item10)
│
├── Class/
│   └── cSQLServer.vb            # ~930 lines — SQL Server data access wrapper
│
├── My Project/
│   ├── Application.myapp        # App manifest settings
│   ├── AssemblyInfo.vb          # Assembly metadata
│   ├── Resources.resx / .Designer.vb
│   └── Settings.settings / .Designer.vb
│
├── _DOCS/
│   ├── 2025-11-25_13-22.png     # Application screenshot
│   └── CODEGEIST.md             # THIS FILE — Codebase guide for LLMs & developers
│
├── bin/Debug/
│   ├── connhistory              # SQL connection strings (plain text)
│   ├── connhistory2             # Bulk Copy connection history (JSON)
│   ├── last.txt                 # RTF content (saves last txtSource content)
│   ├── output/                  # Sample generated output files
│   │   ├── hdr.js / hdr.json
│   │   ├── dtl.js / dtl.json
│   │   ├── subdtl.js / subdtl.json
│   │   └── batch202511.json
│   └── MVC-TOOL.xml             # XML documentation file
│
├── packages/
│   └── Newtonsoft.Json.13.0.4/  # JSON serialization library
│
├── obj/                         # Build intermediate files
└── Resources/                   # Project resource files
```

---

## 3. Architecture Overview

### 3.1 Design Pattern

The application follows a **Monolithic Code Generation Pipeline** pattern:

```
┌─────────────────────────────────────────────────────────────────┐
│                        frmMain (Orchestrator)                     │
│  ┌──────────────┬─────────────┬──────────────┬────────────────┐   │
│  │  API Gen     │  DA Gen     │  UI Gen      │  SQL Tools     │   │
│  │  (SplitBtn)  │  (DropDown) │  (DropDown)  │  (Toolbar)     │   │
│  └──────┬───────┴──────┬──────┴──────┬───────┴───────┬────────┘   │
│         │              │             │               │            │
│         ▼              ▼             ▼               ▼            │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │  Pipeline:                                                   │  │
│  │  1. INPUT → txtSource (C# model class) or DB table schema    │  │
│  │  2. PARSE → Extract properties, types, annotations           │  │
│  │  3. GENERATE → String templates with .Replace() substitution │  │
│  │  4. OUTPUT → txtDest (code) + txtDest2 (scripts)             │  │
│  └─────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌───────────────────┐ │
│  │txtSource │  │ txtDest  │  │txtDest2  │  │ SQL Toolbar       │ │
│  │(left)    │  │(right,   │  │(right,   │  │ [Conn][Table][Gen]│ │
│  │          │  │ Tab 1)   │  │ Tab 2)   │  │                   │ │
│  └──────────┘  └──────────┘  └──────────┘  └───────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
         │                              │
         ▼                              ▼
┌──────────────────┐          ┌──────────────────────┐
│ Dialog Forms     │          │  cSQLServer.vb        │
│ (sub-forms for   │          │  (Data Access Layer)  │
│  specific tasks) │          │  - CRUD operations    │
└──────────────────┘          │  - Pagination helper  │
                              │  - Schema inspection  │
                              └──────────────────────┘
```

### 3.2 Key Characteristics

| Aspect | Detail |
|---|---|
| **Pattern** | Monolithic — all generation logic lives in `frmMain.vb` (one giant file) |
| **Template Method** | XML CDATA literals (`<![CDATA[...]]>`) with chained `.Replace()` calls |
| **UI Model** | Classic event-driven WinForms (no MVVM, no DI, no data binding) |
| **Input Model** | C# class string → `NewPageData()` parser → `List(Of Object)` dictionary |
| **SQL Access** | OleDb for schema browsing (form-level), SqlClient via `cSQLServer` class |
| **JSON** | Newtonsoft.Json 13.0.4 for serialization (connection history, API stubs) |
| **Threading** | Minimal — one `Async` method for `btnConnect`; rest is synchronous |
| **Testing** | None — no unit or integration tests present |

---

## 4. Form-by-Form Reference

### 4.1 `frmMain.vb` — Main Application Window (~10,183 lines)

**Location**: `d:\_WORK\_PERSONAL\MVC-TOOL\frmMain.vb`

This is the core of the application. It contains:

- **UI Layout**: Horizontal split — `txtSource` (left) for C# model input, `TabControl1` (right) with `txtDest` (main output) and `txtDest2` (scripts-only view) on two tab pages.
- **Two Toolbars**: `ToolStrip1` (code generators) and `ToolStrip2` (SQL tools).
- **~55+ code generation features** organized in dropdown menus.

**Key State Variables** (class-level declarations at top of file):

| Variable | Type | Purpose |
|---|---|---|
| `connHistory` | `HashSet(Of String)` | Cached SQL connection strings loaded from `connhistory` file |
| `connpath` | `String` | File path to connection history file |
| `specialk` | `HashSet(Of String)` | VB.NET reserved keywords (used by `ConvertToPropertyName`) |

**Important: `Option Strict Off`** — the project uses late binding extensively.

#### 4.1.1 Code Generation Pipeline (frmMain)

The standard pipeline used by most generators:

1. **Parse Input**: `NewPageData()` reads `txtSource.Lines`, finds `public class ModelName`, extracts properties and their types, and returns a structured `List`:
   - `ds(0)` = modelName (String)
   - `ds(1)` = props (List(Of String)) — raw property declarations like `"public string Name { get; set; }"`
   - `ds(2)` = dialogId (String) — e.g., `"MeterBrandModal"`
   - `ds(3)` = tableId (String) — e.g., `"MeterBrandTable"`
   - `ds(4)` = formId (String) — e.g., `"MeterBrandForm"`
   - `ds(5)` = propertyAnnotations (Dictionary) — extracted `[Required]`, `[MaxLength(n)]` attributes

2. **Type Mapping** (used inside each handler loop over `props`):
   - `type = "int"` AND `fieldname = "id"` → **hidden input** (`<input type="hidden">`)
   - `type = "string"` → **text input** (with special cases: `email`, `password`)
   - `type = "bool"` OR `int` with prefix `is`/suffix `flag`/`enable`/`disable` → **checkbox**
   - `type` contains `int`/`decimal`/`double`/`list`/`[]` (but not `byte`) AND fieldname ends with `id`/`code`/`type`/`status` → **select2 dropdown**
   - `type` starts with `date` → **datepicker** (with InputMask + datetimepicker)
   - `type` starts with `byte[]` → **file upload**

3. **Template Substitution**: Generated code is assembled from XML `CDATA` literal templates with `.Replace()` for variable slots.

4. **Output**: `txtDest.Text = htmlRes` — complete generated code.

#### 4.1.2 Toolbar Menu Structure

**ToolStrip1** (Top Row — Code Generators):

| # | Control | Type | Category | Description |
|---|---|---|---|---|
| 1 | `ToolStripButton1` | SplitButton | API | API Controller generation (2 variants) |
| 2 | `ToolStripButton5` | DropDownButton | DataAccess | Model/DataAccess/Controller builders (6 items) |
| 3 | `ToolStripButton3` | DropDownButton | UI | Form/View generation (25 items — largest menu) |
| 4 | `ToolStripButton2` | SplitButton | AJAX | jQuery AJAX snippets (9 items) |
| 5 | `ToolStripButton4` | SplitButton | Datatable | Bootstrap 4 Datatable builder |
| 6 | `btnSQLBulkCopy` | Button | SQL | Standalone Bulk Copy tool dialog |

**ToolStrip2** (Bottom Row — SQL Tools):

| # | Control | Purpose |
|---|---|---|
| 1 | `txtSQLConnectionString` | ComboBox — connection string input + saved history |
| 2 | `ToolStripButton6` | Build connection string (opens UDL file dialog) |
| 3 | `ToolStripButton9` | Edit connection string (opens frmSQLEditor) |
| 4 | `btnConnect` | Asynchronously connect & populate table list |
| 5 | `cboTable` | ComboBox — select table/view |
| 6 | `btnEditor` | Edit SQL (opens frmSQLEditor with selected table) |
| 7 | `btnGenerateFromTable` | Generate C# model class + CRUD SQL from schema |
| 8 | `btnGenerateProc` | SplitButton — stored procedure templates (MERGE, CRUD, OPENJSON, JSON Schema) |
| 9 | `btnExportJSON` | Export query results to JSON |
| 10 | `ToolStripButton8` | DropDownButton — Options (Annotations, HTML Helpers, Named params, Timeout, Horizontal form, JSONVALUE) |
| 11 | `ToolStripButton7` | Generate INSERT statement from source lines |

#### 4.1.3 Complete Menu Item Catalog & Event Handlers

<details>
<summary><strong>Form Builder (ToolStripButton3) — 25 items</strong></summary>

| Menu Item | Text (as displayed) | Handler Sub | Line |
|---|---|---|---|
| `DefaultToolStripMenuItem` | "Default" | `DefaultToolStripMenuItem_Click` | 33 |
| `ModalPopupToolStripMenuItem` | "Modal Popup" | `ModalPopupToolStripMenuItem_Click` | 408 |
| `ModalPopup2TupleToolStripMenuItem` | "Modal Popup 2 (Tuple)" | `ModalPopup2TupleToolStripMenuItem_Click` | 1275 |
| *(separator)* | | |
| `BlankModalToolStripMenuItem` | "Blank Modal" | `BlankModalToolStripMenuItem_Click` | 2373 |
| `ToolStripMenuItem3` | **"Blank Modal (BS 4.6.x)"** | `ToolStripMenuItem3_Click` | 8915 |
| `HtmlTagHelpersOnlyToolStripMenuItem` | "@HtmlTag Helpers Only" | `HtmlTagHelpersOnlyToolStripMenuItem_Click` | 2405 |
| `Select2ViewBagToolStripMenuItem` | "Select2 - ViewBag" | `Select2ViewBagToolStripMenuItem_Click` | 2661 |
| *(separator)* | | |
| `UseChoicesJSToolStripMenuItem` | "Use ChoicesJS for dropdown select" | `UseChoicesJSToolStripMenuItem_CheckedChanged` | 8144 |
| `ModalPopupcjTemplateToolStripMenuItem` | "Modal Popup (cj template)" | `ModalPopupcjTemplateToolStripMenuItem_Click` | 2944 |
| `ToolStripMenuItem1` | "Modal Popup (jee template)" | `ToolStripMenuItem1_Click` | 3503 |
| `ToolStripMenuItem2` | "Modal Popup (dropdown select)" | `ToolStripMenuItem2_Click` | 5312 |
| `ModalPopupBS46xToolStripMenuItem` | **"Modal Popup (BS 4.6.x)"** | `ModalPopupBS46xToolStripMenuItem_Click` | 7216 |
| *(separator)* | | |
| `InFormDynamicTableToolStripMenuItem` | "In Form (Dynamic Table)" | `InFormDynamicTableToolStripMenuItem_Click` | 4798 |
| `TabsGeneratorToolStripMenuItem` | "Tabs Generator" | `TabsGeneratorToolStripMenuItem_Click` | 4181 |
| *(separator)* | | |
| `DynamicMultiInputToolStripMenuItem` | "Dynamic Multi Input (Table)" | `DynamicMultiInputToolStripMenuItem_Click` | 4490 |
| `LoremImpsumToolStripMenuItem` | "Lorem ipsum" | `LoremImpsumToolStripMenuItem_Click` | 6371 |
| *(separator)* | | |
| `DatasetDummyToolStripMenuItem` | "Dataset dummy" | `DatasetDummyToolStripMenuItem_Click` | 6381 |
| `AccordionToolStripMenuItem` | "Accordion" | `AccordionToolStripMenuItem_Click` | 6481 |
| *(separator)* | | |
| `JSControllerObjectLiteralToolStripMenuItem` | "JS - Controller (Object Literal)" | `JSControllerObjectLiteralToolStripMenuItem_Click` | 7085 |
| `JSObjectLiteralToolStripMenuItem` | "JS - Object Literal Pattern" | `JSObjectLiteralToolStripMenuItem_Click` | 8819 |
| `JSPageFunctionsToolStripMenuItem` | **"JS - Page Function Pattern"** | `JSPageFunctionsToolStripMenuItem_Click` | 8492 |

</details>

<details>
<summary><strong>DataAccess Generator (ToolStripButton5) — 6 items</strong></summary>

| Menu Item | Handler Sub | Line |
|---|---|---|
| `ModelBuilderToolStripMenuItem` ("Model Builder") | `ModelBuilderToolStripMenuItem_Click` | 1178 |
| `DataAccessBuilderToolStripMenuItem` ("DataAccess Builder") | `DataAccessBuilderToolStripMenuItem_Click` | 919 |
| `ControllerBuilderToolStripMenuItem` ("API Controller Builder") | `ControllerBuilderToolStripMenuItem_Click` | 4235 |
| `UIControllerBuilderToolStripMenuItem` ("UI Controller Builder") | `UIControllerBuilderToolStripMenuItem_Click` | 4310 |
| `DataAccessOnUIToolStripMenuItem` ("DataAccess on Controller") | `DataAccessOnUIToolStripMenuItem_Click` | 4615 |
| `DataAccessOnControllerAPIToolStripMenuItem` ("DataAccess on Controller (API)") | `DataAccessOnControllerAPIToolStripMenuItem_Click` | 5156 |

</details>

<details>
<summary><strong>jQuery AJAX Generator (ToolStripButton2) — 9 items</strong></summary>

| Menu Item | Handler Sub | Line |
|---|---|---|
| `GETToolStripMenuItem` ("GET") | `GETToolStripMenuItem_Click` | 4775 |
| `FormPOSTToolStripMenuItem` ("Form POST") | `FormPOSTToolStripMenuItem_Click` | 2022 |
| `FormPOSTJSToolStripMenuItem` ("Form POST (JS)") | `FormPOSTJSToolStripMenuItem_Click` | 2689 |
| `DatatableGETToolStripMenuItem` ("Datatable GET") | `DatatableGETToolStripMenuItem_Click` | 2277 |
| `Select2AjaxToolStripMenuItem` ("Select2 Ajax") | `Select2AjaxToolStripMenuItem_Click` | 2537 |
| `BsSuggestToolStripMenuItem` ("bsSuggest") | `BsSuggestToolStripMenuItem_Click` | 4411 |
| `GETBLOBToolStripMenuItem` ("GET (BLOB)") | `GETBLOBToolStripMenuItem_Click` | 9032 |
| `FormPOSTToolStripMenuItem1` ("Form POST") | `FormPOSTToolStripMenuItem1_Click` | 9763 |
| `FormPOSTToolStripMenuItem2` ("Form POST") | `FormPOSTToolStripMenuItem2_Click` | 9804 |

</details>

<details>
<summary><strong>API Generator (ToolStripButton1) — 2 items</strong></summary>

| Menu Item | Handler Sub | Line |
|---|---|---|
| `APIGeneratorcjTemplateToolStripMenuItem` ("API Generator (cj template)") | `APIGeneratorcjTemplateToolStripMenuItem_Click` | 8127 |
| ButtonClick (default action) | `ToolStripButton1_ButtonClick` | 3491 |

</details>

<details>
<summary><strong>Stored Procedure Generator (btnGenerateProc) — 5 items</strong></summary>

| Menu Item | Handler Sub | Line |
|---|---|---|
| `GenerateMERGETemplateToolStripMenuItem` | `GenerateMERGETemplateToolStripMenuItem_Click` | 9367 |
| `GenerateCRUDProcToolStripMenuItem` | `GenerateCRUDProcToolStripMenuItem_Click` | 9877 |
| `GenerateJSONToolStripMenuItem` | `GenerateJSONToolStripMenuItem_Click` | 9621 |
| `GenerateJSONSchemaToolStripMenuItem` | `GenerateJSONSchemaToolStripMenuItem_Click` | 10178 |
| `GenerateJSONToolStripMenuItem1` | `GenerateJSONToolStripMenuItem1_Click` | 9851 |

</details>

### 4.2 Dialog Forms

| Form | File | Purpose | Key Controls |
|---|---|---|---|
| `frmAjaxGenerator` | `frmAjaxGenerator.vb` | AJAX code generator dialog | TabControl (General tab), Form ID textbox, OK/Cancel |
| `frmCreateApiController` | `frmCreateApiController.vb` | Generate basic API Controller C# class | Controller/model name textboxes, route input, CRUD checkboxes |
| `frmCreateApiControllerCJ` | `frmCreateApiControllerCJ.vb` | API Controller with full DataAccess + DI | Same as above but generates more complete code with `IConfiguration`, connection string injection |
| `frmCreateModal` | `frmCreateModal.vb` | Simple modal name picker | ComboBox (default "MyModal"), OK/Cancel |
| `frmSQL` | `frmSQL.vb` | SQL Connection string builder | Server/user/password fields, database dropdown, test & connect buttons |
| `frmSQLBulkCopy` | `frmSQLBulkCopy.vb` | Bulk data transfer tool | Source/dest connection panels, SQL editor, DataGridView, Export CSV, saves connection history |
| `frmSQLEditor` | `frmSQLEditor.vb` | Minimal text editor | RichTextBox (Consolas), closes on Escape |
| `frmTableName` | `frmTableName.vb` | Table name input | TextBox + OK button |
| `frmTuple` | `frmTuple.vb` | Tuple index selector | ComboBox (Item1–Item10), OK/Cancel |

---

## 5. Data Access Layer — `cSQLServer.vb`

**File**: `Class/cSQLServer.vb` (~930 lines)

A comprehensive SQL Server wrapper class implementing `IDisposable`.

### 5.1 Class Hierarchy

```
cSQLServer
├── Implements IDisposable
├── Inner Class: Parameter    (extends List(Of SqlParameter))
│   ├── AddParameter()         — add or replace parameter
│   ├── ToInsertString()       — "[@col1], [@col2], ..."
│   ├── ToUpdateString()       — "[col1]=@col1, [col2]=@col2, ..."
│   └── Implements ICloneable
├── Inner Class: Pager
│   ├── GetNext() / GetPrevious()  — ROW_NUMBER() based pagination
│   ├── TotalRecordCount / TotalMaxPages
│   └── Offset pagination caching
└── Main Members:
    ├── Shared BuildConnectionString(dataSource, ...) → String
    ├── Shared TestConnection(connString) → Boolean
    ├── Open() / Close()
    ├── ExecuteScalar() / ExecuteNonQuery()
    ├── ExecuteToDatatable() / ExecuteToList()
    ├── QuerySingleResult() → DataRow
    ├── InsertParam() / UpdateParam() / DeleteParam()
    ├── InsertOrReplace() — upsert logic
    ├── InsertAndReturnRow() / UpdateAndReturnRow()
    ├── GetTables() / GetTableColumns() / IsTableExisting()
    ├── CreateTable() / CreateTableFrom(Of T)()
    └── (Private) BuildSelect() / BuildInsert() / BuildUpdate()
```

### 5.2 Usage Pattern

```vbnet
Using db = New cSQLServer(connectionString)
    Dim dt = db.ExecuteToDatatable("SELECT * FROM [Table]")
    ' or parameterized:
    Dim row = db.QuerySingleResult("SELECT * FROM [Table] WHERE id = @id",
                                    db.NewParam.AddParameter("@id", 1))
End Using
```

### 5.3 Key Design Decisions

- **Retry Logic**: Scalar queries retry up to 25 times with 5-second delays; other operations retry 5 times.
- **Parameter Management**: The `Parameter` inner class extends `List(Of SqlParameter)` for fluent parameter building.
- **Pagination**: Uses `ROW_NUMBER()` with `OFFSET...FETCH NEXT` pattern. Caches total record count for UI scenarios.
- **Dynamic Table Creation**: `CreateTableFrom(Of T)()` uses reflection on .NET types to generate `CREATE TABLE` SQL.

---

## 6. Code Generation Patterns & Conventions

### 6.1 XML CDATA Template Pattern

This is the most important pattern to understand. Almost all code generation uses XML literals:

```vbnet
Dim template = <![CDATA[
<div class="form-group">
    <label for="txtFIELD" class="form-label">FIELD</label>
    <input type="text" class="form-control" id="txtFIELD" name="FIELD">
</div>
]]>.Value.Replace("FIELD", fieldName)
```

Key points:
- `<![CDATA[...]]>` preserves whitespace and special characters.
- `.Value` converts the XElement to a String.
- `.Replace()` is chained for multiple substitutions.
- Some templates have hundreds of lines of HTML/JS/Razor embedded this way.

### 6.2 NewPageData() — The Central Parser

```vbnet
Function NewPageData() As List(Of Object)
    ' Returns Nothing if no "public class" found in txtSource
    ' Otherwise returns a List with 6 elements:
    '   [0] modelName (String)
    '   [1] props (List(Of String)) — raw property lines
    '   [2] dialogId (String) — modelName + "Modal"
    '   [3] tableId (String) — modelName + "Table"
    '   [4] formId (String) — modelName + "Form"
    '   [5] propertyAnnotations (Dictionary(Of String, List(Of String)))
End Function
```

**Annotation Extraction**: Scans upward from each property line for `[Attribute]` lines, collecting them in order. Looks specifically for `[Required]` and `[MaxLength(n)]` to generate input validation attributes.

### 6.3 Type Mapping Convention

| C# Type | Field Name Pattern | Generated Control |
|---|---|---|
| `int` | `id`, `Id`, `ID` | Hidden input (`value="-1"`) |
| `string` | — | Text input (`.form-control`) |
| `string` | Ends with `@gmail`, etc. | Email input |
| `string` | `password`, `pwd`, etc. | Password input with show/hide toggle |
| `bool`, `int` flag | Starts `is`/`enable`/ends `flag` | Checkbox (`.custom-control-input`) |
| `int`, `decimal`, `double`, `list`, `[]` | Ends `id`/`code`/`type`/`status` | Select2 dropdown (`.select2`) |
| `int`, `decimal`, `double` | (other) | Number input |
| `DateTime` | — | Datepicker with InputMask |
| `byte[]` | — | File upload |

### 6.4 Generated Output Structure

**For a typical model "MeterBrandModel"**, the generated Razor view produces:

```
1. @model directive (Tuple or non-Tuple based on context)
2. Nav tabs (if applicable)
3. Form content (fields, dropdowns, checkboxes, datepickers)
4. DataTable section (table#MeterBrandTable with columns)
5. Modal popup section (@section popups)
6. CSS section (@section css)
7. Scripts section (@section scripts)
   ├── select2 initialization
   ├── datetimepicker initialization
   ├── Datatable configuration (columns, AJAX)
   ├── Form show/edit/save functions
   └── Status level filter integration
```

---

## 7. Key Helper Functions & Utilities

All located inline in `frmMain.vb`:

| Function | Location | Purpose |
|---|---|---|
| `NewPageData()` | ~7155 | Core C# class parser → structured data |
| `ConvertToPropertyName(input)` | ~6933 | Sanitize arbitrary string → valid .NET property name. Replaces `#`→`_No_`, `$`→`_Amt_`, `@`→`_at_`, `.`→`_dot_`. Appends `_usr` for VB keywords. |
| `ClassStringToJson()` | ~10144 | C# class → JS object literal (camelCase, default values) |
| `DbTypeToString(row)` | ~6787 | OleDb DataRow → C# property declaration |
| `DbTypeToStringFromGetSchemaTable(row)` | ~6848 | Schema table DataRow → C# property with possible `[JsonProperty]` |
| `GetCsNetType(dataType)` | ~6834 | OleDb type code → C# type string |
| `GetCsNetTypeFromSchema(dataType)` | ~6871 | .NET Type → C# type string |
| `GetAnnotations(isNullable, maxLength)` | ~6910 | Returns `[Required]` + `[MaxLength(n)]` |
| `CreateTableSchema(dt, tblName)` | ~6985 | DataTable schema → `CREATE TABLE` SQL with audit columns |
| `generateInsertAndReturn()` | ~6698 | Column dictionary → INSERT SQL with OUTPUT |
| `generateUpdateAndReturn()` | ~6724 | Column dictionary → UPDATE SQL with OUTPUT |
| `enclose_column(name)` | inline | Wraps column name in `[brackets]` |
| **`JSON` inner class** | ~10116 | `Stringify(data)` → lowercase JSON; `Parse(Of T)(json)` ← lowercase JSON |

---

## 8. Configuration & Dependencies

### 8.1 External Dependencies

| Package | Version | Purpose |
|---|---|---|
| `Newtonsoft.Json` | 13.0.4 | JSON serialization (connection history, API responses) |
| `System.Data` | Built-in | ADO.NET (SqlClient, OleDb) |
| `System.Windows.Forms` | Built-in | UI framework |
| `System.Drawing` | Built-in | Graphics & icons |

### 8.2 Runtime Files

| File | Format | Purpose | Managed By |
|---|---|---|---|
| `bin/Debug/connhistory` | Plain text (one connection string per line) | SQL connection string history | `frmMain` — loaded on startup, appended on connect |
| `bin/Debug/connhistory2` | JSON array | Bulk Copy source/destination history | `frmSQLBulkCopy` |
| `bin/Debug/last.txt` | RTF | Last source text content | `frmMain.txtSource_LostFocus` — auto-saves |
| `bin/Debug/output/*.json, *.js` | JSON/JS | Sample generated outputs (batch data) | Manual/from prior runs |

### 8.3 Build Configuration

- **Debug**: x64, Debug symbols, XML documentation file enabled
- **Release**: x64, Optimized, XML documentation file enabled
- **Documentation XML**: Written to `MVC-TOOL.xml` (VB.NET XML doc comments)

---

## 9. Development Guide

### 9.1 How to Build

```
msbuild MVC-TOOL.sln
```
or open in Visual Studio and Build (Ctrl+Shift+B).

**Prerequisites**:
- Visual Studio 2022+ (or MSBuild)
- .NET Framework 4.8.1 SDK / Developer Pack
- NuGet package restore enabled (Newtonsoft.Json 13.0.4)

### 9.2 How to Add a New Feature

1. **Add a new menu item** in the Designer (or create a new `ToolStripMenuItem` in `frmMain.Designer.vb`).
2. **Create the handler** in `frmMain.vb` with the `Handles` clause.
3. **Inside the handler**, call `NewPageData()` to parse the input model from `txtSource`.
4. **Iterate over `props`** and use the **Select Case type** pattern to generate appropriate HTML/JS for each property.
5. **Assemble output** using XML CDATA templates with `.Replace()`.
6. **Set output**: `txtDest.Text = htmlRes`.

### 9.3 Key Conventions to Follow

- **Always use `NewPageData()`** as the entry point for parsing model classes.
- **Use XML literals** (`<![CDATA[...]]>`) for multi-line templates — they preserve formatting and avoid string escaping issues.
- **Use `.Replace()` chaining** for template substitution. Avoid string interpolation for large templates.
- **Structure the type mapping** using `Select Case True` on the `type` variable with `fieldname` pattern matching.
- **Keep `Option Strict Off`** in mind — late binding is used; be careful with type coercion.
- **Don't use `Async`** unless necessary — the codebase is predominantly synchronous.

### 9.4 Code Quality Notes

- The codebase has **no unit tests**.
- `frmMain` is a **monolith** (~10K lines) — consider extracting generation strategies into separate classes.
- **Error handling** is minimal — most handlers lack try/catch, relying on the caller.
- **Stringly-typed**: The `NewPageData()` return type is `List(Of Object)` — no strongly-typed model for parsed data.
- **Duplication**: Several generators have similar logic (e.g., the multiple "Modal Popup" variants) — there is opportunity for refactoring common patterns.

---

## 10. Common Scenarios

### 10.1 Generating Code from a C# Model

1. Paste a C# class into `txtSource` (left panel).
2. Select a generator from the toolbar (e.g., "Modal Popup (BS 4.6.x)" from Form Builder).
3. The generated HTML/Razor/JS appears in `txtDest`.
4. JavaScript-only portions appear in `txtDest2` (Tab 2).

### 10.2 Generating Code from a Database Table

1. Enter SQL Server connection string in the toolbar's connection combo.
2. Click the "Connect" button to populate the table list.
3. Select a table from the dropdown.
4. Click "Generate From Table" — this creates:
   - A C# model class in `txtSource`
   - INSERT/UPDATE SQL with error handling in `txtDest`

### 10.3 Using frmSQLBulkCopy (Standalone)

1. Launch via the SQL Bulk Copy button on the main toolbar.
2. Enter source (left) and destination (right) connection strings.
3. Write SQL queries to extract/transform data.
4. Preview in DataGridView, Execute, or Export to CSV.

---

## 11. Generated Output Samples

Sample output files exist in `bin/Debug/output/`:

| File | Content |
|---|---|
| `hdr.js` / `hdr.json` | Header/billing data arrays (batch data) |
| `dtl.js` / `dtl.json` | Detail line items |
| `subdtl.js` / `subdtl.json` | Sub-detail line items |
| `batch202511.json` | Batch metadata (period: Nov 2025) |

These appear to be actual generated outputs for a billing system with fields like `batchid`, `billperiod`, `routeid`, `unitid`, `datedownload`, `dateupload`, etc.

---

## 12. Appendix: Key File Line References

| Symbol | Line | File |
|---|---|---|
| `NewPageData()` | ~7155 | `frmMain.vb` |
| `ConvertToPropertyName()` | ~6933 | `frmMain.vb` |
| `ClassStringToJson()` | ~10144 | `frmMain.vb` |
| `CreateTableSchema()` | ~6985 | `frmMain.vb` |
| `DbTypeToString()` | ~6787 | `frmMain.vb` |
| `JSON` class | ~10116 | `frmMain.vb` |
| `cSQLServer` class | ~1 | `Class/cSQLServer.vb` |
| `cSQLServer.Parameter` | ~42 | `Class/cSQLServer.vb` |
| `cSQLServer.Pager` | ~680 | `Class/cSQLServer.vb` |
| `cSQLServer.BuildConnectionString()` | ~14 | `Class/cSQLServer.vb` |
| `ModalPopupBS46xToolStripMenuItem_Click` | ~7216 | `frmMain.vb` |

---

*Generated on: 2026-06-26 — This document should be updated as the codebase evolves.*
