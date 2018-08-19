###### Order.cs

```
 public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal Amount { get; set; }
    }
```

###### User.cs

```
 public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }

        [NotMapped]
        public string Orders { get; set; }
    }
```


###### MVCParentChildFormDataContext.cs

```
 public class MVCParentChildFormDataContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order>  Orders { get; set; }
    }
```

###### UserController.cs

```
    public class UserController : Controller
    {
        private MVCParentChildFormDataContext db = new MVCParentChildFormDataContext();

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.OrderList = db.Orders.Where(o => o.UserId == id);
            }
            return View(user);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,EmailId,MobileNo,Orders")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreationDate = DateTime.Now;
                user.UpdateDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(user.Orders))
                {
                    int getUserId = user.Id;
                    string[] strOrders = user.Orders.Split('#');

                    foreach (var strOrder in strOrders)
                    {
                        if (!string.IsNullOrEmpty(strOrder))
                        {
                            string[] orderitem = strOrder.Split('~');

                            Order order = new Order();
                            order.UserId = getUserId;
                            order.ProductName = orderitem[0];
                            order.ProductType = orderitem[1];
                            order.Amount = Convert.ToDecimal(orderitem[2]);
                            db.Orders.Add(order);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.OrderList = db.Orders.Where(o => o.UserId == id);
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,EmailId,MobileNo,Orders")] User user)
        {
            if (ModelState.IsValid)
            {
                
                user.UpdateDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.Entry(user).Property(u => u.CreationDate).IsModified = false;
                db.SaveChanges();

                //Clear related orders
                var removeOrders= db.Orders.Where(o => o.UserId == user.Id);
                db.Orders.RemoveRange(removeOrders);
                db.SaveChanges();
                //add related order
                if (!string.IsNullOrEmpty(user.Orders))
                {
                    int getUserId = user.Id;

                    string[] strOrders = user.Orders.Split('#');

                    foreach (var strOrder in strOrders)
                    {
                        if (!string.IsNullOrEmpty(strOrder))
                        {
                            string[] orderitem = strOrder.Split('~');

                            Order order = new Order();
                            order.UserId = getUserId;
                            order.ProductName = orderitem[0];
                            order.ProductType = orderitem[1];
                            order.Amount = Convert.ToDecimal(orderitem[2]);
                            db.Orders.Add(order);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
```

###### Create.cshtml

```
@model MVCParentChildForm.Models.User

@{
    ViewBag.Title = "Create";
}

<h2>Create</h2>


@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()

    <div class="form-horizontal">
        <h4>User</h4>
        <hr />
        @Html.ValidationSummary(true, "", new { @class = "text-danger" })
        <div class="form-group">
            @Html.LabelFor(model => model.Name, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.Name, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.Name, "", new { @class = "text-danger" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(model => model.EmailId, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.EmailId, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.EmailId, "", new { @class = "text-danger" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(model => model.MobileNo, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.MobileNo, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.MobileNo, "", new { @class = "text-danger" })
            </div>
        </div>


        <h4>Order</h4>
        <hr />
        @Html.HiddenFor(model => model.Orders)
        <div class="form-group">
            <table id="dtTable" class="table table-bordered">
                <tbody>
                    <tr>
                        <th>
                            @Html.TextBox("txtProductName", null, new { @class = "form-control", @placeholder = "Product Name" })
                        </th>
                        <th>
                            @Html.DropDownList("ddlProductType", new List<SelectListItem>{
                                    new SelectListItem{ Text="Select Any Product Type", Value = "" },
                                    new SelectListItem{ Text="Software", Value = "Software" },
                                    new SelectListItem{ Text="Website", Value = "Website" },
                                    new SelectListItem{ Text="Services", Value = "Services" },
                                },
                                new { @class = "form-control", @placeholder = "Designation" }
                               )
                        </th>
                        <th>
                            @Html.TextBox("txtAmount", null, new { @class = "form-control", @placeholder = "Amount", @type = "number" })
                        </th>
                        <th>
                            <input type="button" id="addRow" value="Add New Row" class="btn btn-info" />
                        </th>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <input type="submit" value="Create" class="btn btn-default" />
            </div>
        </div>
    </div>

}

<div>
    @Html.ActionLink("Back to List", "Index")
</div>

@section Scripts {
    @Scripts.Render("~/bundles/jqueryval")

    <script type="text/javascript">

        var dtTableHeader = $("#dtTable tr th");

        $("#addRow").on("click", function () {
            var errorMsg = validate();
            if (errorMsg == "") {

                var dtTbl = $('#dtTable')[0];
                var rowCnt = dtTbl.rows.length; // Get table row count.
                var tr = dtTbl.insertRow(rowCnt); // Table row.
                tr = dtTbl.insertRow(rowCnt);

                for (var c = 0; c < dtTableHeader.length; c++) {
                    var value = "";
                    if (c == 0) {
                        value = $("#txtProductName").val();
                    }
                    else if (c == 1) {
                        value = $("#ddlProductType").val();
                    }
                    else if (c == 2) {
                        value = $("#txtAmount").val();
                    }

                    var td = $("<td>")[0]; // create td element
                    td = tr.insertCell(c);

                    if (c == dtTableHeader.length - 1) { // First column.
                        // Add a button.
                        var button = $('<input>')[0]; //create element and select 0 index
                        button.setAttribute('type', 'button');
                        button.setAttribute('value', 'Remove');
                        button.setAttribute('onclick', 'removeRow(this)');
                        button.setAttribute('class', 'btn btn-danger');
                        td.appendChild(button);
                    }
                    else {
                        // Create and add textbox in each cell.
                        var element = $('<input>')[0]; //create element and select 0 index
                        element.setAttribute('type', 'text');
                        element.setAttribute('value', value);
                        element.setAttribute('class', 'form-control');
                        element.setAttribute('disabled', 'disabled');
                        td.appendChild(element);
                    }
                }
                submitData();
                clearElementValue();
                $("#txtProductName").focus();
            }
            else {
                alert(errorMsg);
            }
        })

        function removeRow(currElement) {
            var dtTbl = $('#dtTable')[0];
            var currRow = currElement.parentElement.parentElement.rowIndex;
            dtTbl.deleteRow(currRow);
            submitData();
        }

        function submitData() {
            var dtTbl = $('#dtTable')[0];
            var values = "";
            for (row = 1; row < dtTbl.rows.length - 1; row++) {
                for (c = 0; c < dtTbl.rows[row].cells.length; c++) {   // EACH CELL IN A ROW.
                    var element = dtTbl.rows.item(row).cells[c].children[0];//find current element
                    if (element.getAttribute('type') == 'text') {
                        values = values + element.value + "~";
                    }
                }
                values = values.substring(0, values.length - 1);//remove last ~
                values = values + "#";
            }
            values = values.substring(0, values.length - 1);//remove last #
            $("#Orders").val(values);
        }

        function clearElementValue() {
            $("#txtProductName").val('');
            $("#ddlProductType").val('');
            $("#txtAmount").val('');
        }

        function validate() {
            var errorMsg = "";
            var txtProductName = $("#txtProductName").val();
            if (txtProductName == "") {
                errorMsg = "ProductName is required.\n";
            }
            var ddlProductType = $("#ddlProductType").val();
            if (ddlProductType == "") {
                errorMsg = errorMsg + "Product Type is required.\n";
            }
            var txtAmount = $("#txtAmount").val();
            if (txtAmount == "") {
                errorMsg = errorMsg + "Amount is required.";
            }
            return errorMsg;
        }

    </script>




}

```

###### Edit.cshtml

```
@model MVCParentChildForm.Models.User

@{
    ViewBag.Title = "Edit";
}

<h2>Edit</h2>


@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()

    <div class="form-horizontal">
        <h4>User</h4>
        <hr />
        @Html.ValidationSummary(true, "", new { @class = "text-danger" })
        @Html.HiddenFor(model => model.Id)

        <div class="form-group">
            @Html.LabelFor(model => model.Name, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.Name, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.Name, "", new { @class = "text-danger" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(model => model.EmailId, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.EmailId, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.EmailId, "", new { @class = "text-danger" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(model => model.MobileNo, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.MobileNo, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.MobileNo, "", new { @class = "text-danger" })
            </div>
        </div>

        <h4>Order</h4>
        <hr />
        @Html.TextBoxFor(model => model.Orders)
        <div class="form-group">
            <table id="dtTable" class="table table-bordered">
                <tbody>
                    <tr>
                        <th>
                            @Html.TextBox("txtProductName", null, new { @class = "form-control", @placeholder = "Product Name" })
                        </th>
                        <th>
                            @Html.DropDownList("ddlProductType", new List<SelectListItem>{
                                    new SelectListItem{ Text="Select Any Product Type", Value = "" },
                                    new SelectListItem{ Text="Software", Value = "Software" },
                                    new SelectListItem{ Text="Website", Value = "Website" },
                                    new SelectListItem{ Text="Services", Value = "Services" },
                                },
                                new { @class = "form-control", @placeholder = "Designation" }
                               )
                        </th>
                        <th>
                            @Html.TextBox("txtAmount", null, new { @class = "form-control", @placeholder = "Amount", @type = "number" })
                        </th>
                        <th>
                            <input type="button" id="addRow" value="Add New Row" class="btn btn-info" />
                        </th>
                    </tr>
                    @if (ViewBag.OrderList != null)
                    {
                        foreach (var order in ViewBag.OrderList)
                        {
                            <tr>
                                <td>
                                    <input type="text" value="@order.ProductName" class="form-control" disabled="disabled">
                                </td>
                                <td>
                                    <input type="text" value="@order.ProductType" class="form-control" disabled="disabled">
                                </td>
                                <td>
                                    <input type="text" value="@order.Amount" class="form-control" disabled="disabled">
                                </td>
                                <td>
                                    <input type="button" value="Remove" onclick="removeRow(this)" class="btn btn-danger">
                                </td>
                            </tr>
                        }
                        <tr></tr>
                    }
                </tbody>
            </table>
        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <input type="submit" value="Save" class="btn btn-default" />
            </div>
        </div>
    </div>
}

<div>
    @Html.ActionLink("Back to List", "Index")
</div>

@section Scripts {
    @Scripts.Render("~/bundles/jqueryval")

    <script type="text/javascript">

        var dtTableHeader = $("#dtTable tr th");
        $(function () {
            submitData();
            submitData();
        });
        $("#addRow").on("click", function () {
            var errorMsg = validate();
            if (errorMsg == "") {

                var dtTbl = $('#dtTable')[0];
                var rowCnt = dtTbl.rows.length; // Get table row count.
                var tr = dtTbl.insertRow(rowCnt); // Table row.
                tr = dtTbl.insertRow(rowCnt);

                for (var c = 0; c < dtTableHeader.length; c++) {
                    var value = "";
                    if (c == 0) {
                        value = $("#txtProductName").val();
                    }
                    else if (c == 1) {
                        value = $("#ddlProductType").val();
                    }
                    else if (c == 2) {
                        value = $("#txtAmount").val();
                    }

                    var td = $("<td>")[0]; // create td element
                    td = tr.insertCell(c);

                    if (c == dtTableHeader.length - 1) { // First column.
                        // Add a button.
                        var button = $('<input>')[0]; //create element and select 0 index
                        button.setAttribute('type', 'button');
                        button.setAttribute('value', 'Remove');
                        button.setAttribute('onclick', 'removeRow(this)');
                        button.setAttribute('class', 'btn btn-danger');
                        td.appendChild(button);
                    }
                    else {
                        // Create and add textbox in each cell.
                        var element = $('<input>')[0]; //create element and select 0 index
                        element.setAttribute('type', 'text');
                        element.setAttribute('value', value);
                        element.setAttribute('class', 'form-control');
                        element.setAttribute('disabled', 'disabled');
                        td.appendChild(element);
                    }
                }
                submitData();
                clearElementValue();
                $("#txtProductName").focus();
            }
            else {
                alert(errorMsg);
            }
        })

        function removeRow(currElement) {
            var dtTbl = $('#dtTable')[0];
            var currRow = currElement.parentElement.parentElement.rowIndex;
            dtTbl.deleteRow(currRow);
            submitData();
        }

        function submitData() {
            var dtTbl = $('#dtTable')[0];
            var values = "";
            for (row = 1; row < dtTbl.rows.length - 1; row++) {
                for (c = 0; c < dtTbl.rows[row].cells.length; c++) {   // EACH CELL IN A ROW.
                    var element = dtTbl.rows.item(row).cells[c].children[0];//find current element
                    if (element.getAttribute('type') == 'text') {
                        values = values + element.value + "~";
                    }
                }
                values = values.substring(0, values.length - 1);//remove last ~
                values = values + "#";
            }
            values = values.substring(0, values.length - 1);//remove last #
            $("#Orders").val(values);
        }

        function clearElementValue() {
            $("#txtProductName").val('');
            $("#ddlProductType").val('');
            $("#txtAmount").val('');
        }

        function validate() {
            var errorMsg = "";
            var txtProductName = $("#txtProductName").val();
            if (txtProductName == "") {
                errorMsg = "ProductName is required.\n";
            }
            var ddlProductType = $("#ddlProductType").val();
            if (ddlProductType == "") {
                errorMsg = errorMsg + "Product Type is required.\n";
            }
            var txtAmount = $("#txtAmount").val();
            if (txtAmount == "") {
                errorMsg = errorMsg + "Amount is required.";
            }
            return errorMsg;
        }

    </script>
}
```

###### Delete.cshtml

```
@model MVCParentChildForm.Models.User

@{
    ViewBag.Title = "Delete";
}

<h2>Delete</h2>

<h3>Are you sure you want to delete this?</h3>
<div>
    <h4>User</h4>
    <hr />
    <dl class="dl-horizontal">
        <dt>
            @Html.DisplayNameFor(model => model.Name)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.Name)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.EmailId)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.EmailId)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.MobileNo)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.MobileNo)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.CreationDate)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.CreationDate)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.UpdateDate)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.UpdateDate)
        </dd>

    </dl>

    @using (Html.BeginForm()) {
        @Html.AntiForgeryToken()

        <div class="form-actions no-color">
            <input type="submit" value="Delete" class="btn btn-default" /> |
            @Html.ActionLink("Back to List", "Index")
        </div>
    }
</div>

```

###### Details.cshtml

```
@model MVCParentChildForm.Models.User

@{
    ViewBag.Title = "Details";
}

<h2>Details</h2>

<div>
    <h4>User</h4>
    <hr />
    <dl class="dl-horizontal">
        <dt>
            @Html.DisplayNameFor(model => model.Name)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.Name)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.EmailId)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.EmailId)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.MobileNo)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.MobileNo)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.CreationDate)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.CreationDate)
        </dd>

        <dt>
            @Html.DisplayNameFor(model => model.UpdateDate)
        </dt>

        <dd>
            @Html.DisplayFor(model => model.UpdateDate)
        </dd>
        <dd>
            <div class="form-group">
                @if (ViewBag.OrderList!=null && Enumerable.Count(ViewBag.OrderList)> 0)
                {
                    <table id="dtTable" class="table table-bordered">
                        <tbody>
                            <tr>
                                <th>
                                    Product Name
                                </th>
                                <th>
                                    Product Type
                                </th>
                                <th>
                                    Amount
                                </th>
                            </tr>
                            @foreach (var order in ViewBag.OrderList)
                            {
                                <tr>
                                    <td>
                                        <input type="text" value="@order.ProductName" class="form-control" disabled="disabled">
                                    </td>
                                    <td>
                                        <input type="text" value="@order.ProductType" class="form-control" disabled="disabled">
                                    </td>
                                    <td>
                                        <input type="text" value="@order.Amount" class="form-control" disabled="disabled">
                                    </td>
                                </tr>
                            }
                            <tr></tr>

                        </tbody>
                    </table>
                }
            </div>
        </dd>
    </dl>
</div>
<p>
    @Html.ActionLink("Edit", "Edit", new { id = Model.Id }) |
    @Html.ActionLink("Back to List", "Index")
</p>

```
###### Index.cshtml

```
@model IEnumerable<MVCParentChildForm.Models.User>

@{
    ViewBag.Title = "Index";
}

<h2>Index</h2>

<p>
    @Html.ActionLink("Create New", "Create")
</p>
<table class="table">
    <tr>
        <th>
            @Html.DisplayNameFor(model => model.Name)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.EmailId)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.MobileNo)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.CreationDate)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.UpdateDate)
        </th>
        <th></th>
    </tr>

@foreach (var item in Model) {
    <tr>
        <td>
            @Html.DisplayFor(modelItem => item.Name)
        </td>
        <td>
            @Html.DisplayFor(modelItem => item.EmailId)
        </td>
        <td>
            @Html.DisplayFor(modelItem => item.MobileNo)
        </td>
        <td>
            @Html.DisplayFor(modelItem => item.CreationDate)
        </td>
        <td>
            @Html.DisplayFor(modelItem => item.UpdateDate)
        </td>
        <td>
            @Html.ActionLink("Edit", "Edit", new { id=item.Id }) |
            @Html.ActionLink("Details", "Details", new { id=item.Id }) |
            @Html.ActionLink("Delete", "Delete", new { id=item.Id })
        </td>
    </tr>
}

</table>

```

###### Web.config

```
 </configSections>
  <connectionStrings>
    <add name="MVCParentChildFormDataContext" connectionString="Data Source=DESKTOP-VDUBU4D;Initial Catalog=MVCParentChildFormDataContext;User ID=sa;Password=asd" providerName="System.Data.SqlClient"  />
  </connectionStrings>
```
