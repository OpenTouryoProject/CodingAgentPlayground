using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Form1 : Page
    {
        // ViewState でマスタデータと明細を保持
        private DataTable CustomersTable
        {
            get { return (DataTable)ViewState["CustomersTable"]; }
            set { ViewState["CustomersTable"] = value; }
        }

        private DataTable EmployeesTable
        {
            get { return (DataTable)ViewState["EmployeesTable"]; }
            set { ViewState["EmployeesTable"] = value; }
        }

        private DataTable ProductsTable
        {
            get { return (DataTable)ViewState["ProductsTable"]; }
            set { ViewState["ProductsTable"] = value; }
        }

        private DataTable ShippersTable
        {
            get { return (DataTable)ViewState["ShippersTable"]; }
            set { ViewState["ShippersTable"] = value; }
        }

        private DataTable DetailsTable
        {
            get { return (DataTable)ViewState["DetailsTable"]; }
            set { ViewState["DetailsTable"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    LoadDropdowns();
                    InitDetailsTable();

                    // QueryString に OrderID があれば読み込む
                    string orderIdParam = Request.QueryString["OrderID"];
                    if (!string.IsNullOrEmpty(orderIdParam))
                    {
                        int orderId;
                        if (int.TryParse(orderIdParam, out orderId))
                        {
                            txtOrderID.Text = orderId.ToString();
                            LoadOrder(orderId);
                        }
                    }
                    else
                    {
                        // デフォルトで明細のある受注をロード
                        txtOrderID.Text = "10248";
                        LoadOrder(10248);
                    }
                }
                catch (Exception ex)
                {
                    ShowError("ページ読み込みエラー: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 読み込みボタンクリック
        /// </summary>
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId;
                if (!int.TryParse(txtOrderID.Text, out orderId))
                {
                    ShowError("有効な受注IDを入力してください。");
                    return;
                }
                LoadOrder(orderId);
            }
            catch (Exception ex)
            {
                ShowError("受注読み込みエラー: " + ex.Message);
            }
        }

        /// <summary>
        /// ドロップダウンリストの初期データロード
        /// </summary>
        private void LoadDropdowns()
        {
            // 顧客
            DataTable dtCustomers = DbHelper.GetCustomers();
            CustomersTable = dtCustomers;
            ddlCustomer.DataSource = dtCustomers;
            ddlCustomer.DataTextField = "CompanyName";
            ddlCustomer.DataValueField = "CustomerID";
            ddlCustomer.DataBind();
            ddlCustomer.Items.Insert(0, new ListItem("-- 選択 --", ""));

            // 従業員
            DataTable dtEmployees = DbHelper.GetEmployees();
            EmployeesTable = dtEmployees;
            ddlEmployee.DataSource = dtEmployees;
            ddlEmployee.DataTextField = "EmployeeName";
            ddlEmployee.DataValueField = "EmployeeID";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("-- 選択 --", ""));

            // 商品（GridView 内のドロップダウン用）
            DataTable dtProducts = DbHelper.GetProducts();
            ProductsTable = dtProducts;

            // 配送業者
            DataTable dtShippers = DbHelper.GetShippers();
            ShippersTable = dtShippers;
            ddlShipper.DataSource = dtShippers;
            ddlShipper.DataTextField = "CompanyName";
            ddlShipper.DataValueField = "ShipperID";
            ddlShipper.DataBind();
            ddlShipper.Items.Insert(0, new ListItem("-- 選択 --", ""));
        }

        /// <summary>
        /// 空の明細 DataTable を初期化する
        /// </summary>
        private void InitDetailsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductID", typeof(int));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("Quantity", typeof(short));
            dt.Columns.Add("Discount", typeof(float));
            dt.Columns.Add("LineTotal", typeof(decimal));
            DetailsTable = dt;
            BindDetailsGrid();
        }

        /// <summary>
        /// 受注情報を読み込んでフォームに表示する
        /// </summary>
        private void LoadOrder(int orderId)
        {
            // 受注ヘッダ取得
            DataTable dtOrder = DbOrder.GetOrder(orderId);
            if (dtOrder.Rows.Count == 0)
            {
                ShowError("受注ID " + orderId + " は存在しません。");
                return;
            }

            DataRow row = dtOrder.Rows[0];

            // 受注情報をコントロールに反映
            txtOrderID.Text = row["OrderID"].ToString();

            if (row["OrderDate"] != DBNull.Value)
                txtOrderDate.Text = Convert.ToDateTime(row["OrderDate"]).ToString("yyyy-MM-dd");
            if (row["RequiredDate"] != DBNull.Value)
                txtRequiredDate.Text = Convert.ToDateTime(row["RequiredDate"]).ToString("yyyy-MM-dd");
            if (row["ShippedDate"] != DBNull.Value)
                txtShippedDate.Text = Convert.ToDateTime(row["ShippedDate"]).ToString("yyyy-MM-dd");

            // ドロップダウン選択
            if (row["CustomerID"] != DBNull.Value)
            {
                ddlCustomer.SelectedValue = row["CustomerID"].ToString();
                ShowCustomerInfo();
            }
            if (row["EmployeeID"] != DBNull.Value)
            {
                ddlEmployee.SelectedValue = row["EmployeeID"].ToString();
                ShowEmployeeInfo();
            }
            if (row["ShipVia"] != DBNull.Value)
            {
                ddlShipper.SelectedValue = row["ShipVia"].ToString();
                ShowShipperInfo();
            }

            // 配送情報
            txtFreight.Text = row["Freight"] != DBNull.Value ? Convert.ToDecimal(row["Freight"]).ToString("F2") : "";
            txtShipName.Text = row["ShipName"] != DBNull.Value ? row["ShipName"].ToString() : "";
            txtShipAddress.Text = row["ShipAddress"] != DBNull.Value ? row["ShipAddress"].ToString() : "";
            txtShipCity.Text = row["ShipCity"] != DBNull.Value ? row["ShipCity"].ToString() : "";
            txtShipRegion.Text = row["ShipRegion"] != DBNull.Value ? row["ShipRegion"].ToString() : "";
            txtShipPostalCode.Text = row["ShipPostalCode"] != DBNull.Value ? row["ShipPostalCode"].ToString() : "";
            txtShipCountry.Text = row["ShipCountry"] != DBNull.Value ? row["ShipCountry"].ToString() : "";

            // 受注明細取得・バインド
            DataTable dtDetails = DbOrder.GetOrderDetails(orderId);
            DetailsTable = dtDetails;
            BindDetailsGrid();

            // 合計金額計算
            CalculateTotals();
        }

        /// <summary>
        /// 明細 GridView をデータバインドする
        /// </summary>
        private void BindDetailsGrid()
        {
            gvDetails.DataSource = DetailsTable;
            gvDetails.DataBind();
        }

        /// <summary>
        /// 合計金額を計算して表示する
        /// </summary>
        private void CalculateTotals()
        {
            decimal total = 0;
            if (DetailsTable != null)
            {
                foreach (DataRow row in DetailsTable.Rows)
                {
                    if (row["LineTotal"] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(row["LineTotal"]);
                    }
                }
            }
            lblTotalAmount.Text = total.ToString("C");
        }

        /// <summary>
        /// 顧客関連情報を表示する
        /// </summary>
        private void ShowCustomerInfo()
        {
            if (CustomersTable == null || string.IsNullOrEmpty(ddlCustomer.SelectedValue))
            {
                lblCustomerPhone.Text = "";
                lblCustomerAddress.Text = "";
                return;
            }

            DataRow[] rows = CustomersTable.Select("CustomerID = '" + ddlCustomer.SelectedValue.Replace("'", "''") + "'");
            if (rows.Length > 0)
            {
                DataRow row = rows[0];
                lblCustomerPhone.Text = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "";
                string address = "";
                if (row["Address"] != DBNull.Value) address += row["Address"].ToString();
                if (row["City"] != DBNull.Value) address += ", " + row["City"].ToString();
                if (row["Country"] != DBNull.Value) address += ", " + row["Country"].ToString();
                lblCustomerAddress.Text = address;
            }
        }

        /// <summary>
        /// 従業員関連情報を表示する
        /// </summary>
        private void ShowEmployeeInfo()
        {
            if (EmployeesTable == null || string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                lblEmployeeTitle.Text = "";
                lblEmployeeExtension.Text = "";
                return;
            }

            DataRow[] rows = EmployeesTable.Select("EmployeeID = " + ddlEmployee.SelectedValue);
            if (rows.Length > 0)
            {
                DataRow row = rows[0];
                lblEmployeeTitle.Text = row["Title"] != DBNull.Value ? row["Title"].ToString() : "";
                lblEmployeeExtension.Text = row["Extension"] != DBNull.Value ? row["Extension"].ToString() : "";
            }
        }

        /// <summary>
        /// 配送業者関連情報を表示する
        /// </summary>
        private void ShowShipperInfo()
        {
            if (ShippersTable == null || string.IsNullOrEmpty(ddlShipper.SelectedValue))
            {
                lblShipperPhone.Text = "";
                return;
            }

            DataRow[] rows = ShippersTable.Select("ShipperID = " + ddlShipper.SelectedValue);
            if (rows.Length > 0)
            {
                lblShipperPhone.Text = rows[0]["Phone"] != DBNull.Value ? rows[0]["Phone"].ToString() : "";
            }
        }

        /// <summary>
        /// エラーメッセージを表示する
        /// </summary>
        private void ShowError(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "error",
                "alert(" + System.Web.HttpUtility.JavaScriptStringEncode(message, true) + ");", true);
        }

        // ========== イベントハンドラ ==========

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCustomerInfo();
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowEmployeeInfo();
        }

        protected void ddlShipper_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowShipperInfo();
        }

        protected void ddlProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlProduct = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlProduct.NamingContainer;
            int rowIndex = row.RowIndex;

            TextBox txtUnitPrice = (TextBox)row.FindControl("txtUnitPrice");
            Label lblLineTotal = (Label)row.FindControl("lblLineTotal");

            if (string.IsNullOrEmpty(ddlProduct.SelectedValue))
            {
                txtUnitPrice.Text = "0.00";
                lblLineTotal.Text = "0.00";
            }
            else
            {
                // 選択された商品の単価を設定
                DataRow[] productRows = ProductsTable.Select("ProductID = " + ddlProduct.SelectedValue);
                if (productRows.Length > 0)
                {
                    decimal unitPrice = productRows[0]["UnitPrice"] != DBNull.Value
                        ? Convert.ToDecimal(productRows[0]["UnitPrice"]) : 0;
                    txtUnitPrice.Text = unitPrice.ToString("F2");

                    // 行合計を再計算
                    TextBox txtQuantity = (TextBox)row.FindControl("txtQuantity");
                    TextBox txtDiscount = (TextBox)row.FindControl("txtDiscount");
                    short quantity;
                    float discount;
                    short.TryParse(txtQuantity.Text, out quantity);
                    float.TryParse(txtDiscount.Text, out discount);

                    decimal lineTotal = (unitPrice - (decimal)discount) * quantity;
                    lblLineTotal.Text = lineTotal.ToString("F2");
                }
            }

            // ViewState の DetailsTable を更新
            if (DetailsTable != null && rowIndex < DetailsTable.Rows.Count)
            {
                DataRow dr = DetailsTable.Rows[rowIndex];
                int productId;
                if (int.TryParse(ddlProduct.SelectedValue, out productId))
                    dr["ProductID"] = productId;
                else
                    dr["ProductID"] = DBNull.Value;

                decimal price;
                decimal.TryParse(txtUnitPrice.Text, out price);
                dr["UnitPrice"] = price;

                TextBox txtQty = (TextBox)row.FindControl("txtQuantity");
                TextBox txtDisc = (TextBox)row.FindControl("txtDiscount");
                short qty;
                float disc;
                short.TryParse(txtQty.Text, out qty);
                float.TryParse(txtDisc.Text, out disc);
                dr["Quantity"] = qty;
                dr["Discount"] = disc;
                dr["LineTotal"] = (price - (decimal)disc) * qty;

                DetailsTable = DetailsTable;
            }

            CalculateTotals();
        }

        protected void gvDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // 削除前に現在のGridView入力値を同期
                    SyncDetailsFromGrid();

                    DataTable dt = DetailsTable;
                    if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        dt.Rows.RemoveAt(rowIndex);
                        DetailsTable = dt;
                    }

                    BindDetailsGrid();
                    CalculateTotals();
                }
                catch (Exception ex)
                {
                    ShowError("明細削除エラー: " + ex.Message);
                }
            }
        }

        protected void gvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlProduct = (DropDownList)e.Row.FindControl("ddlProduct");
                if (ddlProduct != null && ProductsTable != null)
                {
                    ddlProduct.DataSource = ProductsTable;
                    ddlProduct.DataTextField = "ProductName";
                    ddlProduct.DataValueField = "ProductID";
                    ddlProduct.DataBind();
                    ddlProduct.Items.Insert(0, new ListItem("-- 選択 --", ""));

                    // 現在の行の ProductID で選択状態を設定
                    DataRowView drv = (DataRowView)e.Row.DataItem;
                    if (drv["ProductID"] != DBNull.Value)
                    {
                        string productId = drv["ProductID"].ToString();
                        if (ddlProduct.Items.FindByValue(productId) != null)
                        {
                            ddlProduct.SelectedValue = productId;
                        }
                    }
                }
            }
        }

        protected void btnAddDetail_Click(object sender, EventArgs e)
        {
            try
            {
                // 現在のGridView入力値をViewStateに同期
                SyncDetailsFromGrid();

                // 空行を追加
                DataTable dt = DetailsTable;
                DataRow newRow = dt.NewRow();
                newRow["ProductID"] = DBNull.Value;
                newRow["UnitPrice"] = 0m;
                newRow["Quantity"] = (short)1;
                newRow["Discount"] = 0f;
                newRow["LineTotal"] = 0m;
                dt.Rows.Add(newRow);
                DetailsTable = dt;

                BindDetailsGrid();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ShowError("明細追加エラー: " + ex.Message);
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                SyncDetailsFromGrid();

                string error = ValidateInput();
                if (!string.IsNullOrEmpty(error))
                {
                    ShowError(error);
                    return;
                }

                DateTime? orderDate = ParseDate(txtOrderDate.Text);
                DateTime? requiredDate = ParseDate(txtRequiredDate.Text);
                DateTime? shippedDate = ParseDate(txtShippedDate.Text);
                decimal freight = 0;
                decimal.TryParse(txtFreight.Text, out freight);

                int newOrderId = DbOrder.CreateOrder(
                    ddlCustomer.SelectedValue,
                    Convert.ToInt32(ddlEmployee.SelectedValue),
                    orderDate, requiredDate, shippedDate,
                    Convert.ToInt32(ddlShipper.SelectedValue),
                    freight,
                    txtShipName.Text, txtShipAddress.Text, txtShipCity.Text,
                    txtShipRegion.Text, txtShipPostalCode.Text, txtShipCountry.Text,
                    DetailsTable);

                txtOrderID.Text = newOrderId.ToString();
                ShowInfo("受注を作成しました。受注ID: " + newOrderId);
            }
            catch (Exception ex)
            {
                ShowError("作成エラー: " + ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SyncDetailsFromGrid();

                int orderId;
                if (!int.TryParse(txtOrderID.Text, out orderId) || orderId <= 0)
                {
                    ShowError("更新対象の受注IDが無効です。");
                    return;
                }

                string error = ValidateInput();
                if (!string.IsNullOrEmpty(error))
                {
                    ShowError(error);
                    return;
                }

                DateTime? orderDate = ParseDate(txtOrderDate.Text);
                DateTime? requiredDate = ParseDate(txtRequiredDate.Text);
                DateTime? shippedDate = ParseDate(txtShippedDate.Text);
                decimal freight = 0;
                decimal.TryParse(txtFreight.Text, out freight);

                DbOrder.SaveOrder(
                    orderId,
                    ddlCustomer.SelectedValue,
                    Convert.ToInt32(ddlEmployee.SelectedValue),
                    orderDate, requiredDate, shippedDate,
                    Convert.ToInt32(ddlShipper.SelectedValue),
                    freight,
                    txtShipName.Text, txtShipAddress.Text, txtShipCity.Text,
                    txtShipRegion.Text, txtShipPostalCode.Text, txtShipCountry.Text,
                    DetailsTable);

                ShowInfo("受注ID " + orderId + " を更新しました。");
            }
            catch (Exception ex)
            {
                ShowError("更新エラー: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }

        /// <summary>
        /// 入力検証を実行する。エラーがある場合はメッセージを返す。
        /// </summary>
        private string ValidateInput()
        {
            if (string.IsNullOrEmpty(ddlCustomer.SelectedValue))
                return "顧客を選択してください。";

            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
                return "担当従業員を選択してください。";

            if (string.IsNullOrEmpty(ddlShipper.SelectedValue))
                return "配送業者を選択してください。";

            // 運送料の妥当性
            decimal freight;
            if (!string.IsNullOrEmpty(txtFreight.Text) && decimal.TryParse(txtFreight.Text, out freight))
            {
                if (freight < 0)
                    return "運送料は0以上を入力してください。";
            }

            // 明細が1件以上
            DataTable dt = DetailsTable;
            if (dt == null || dt.Rows.Count == 0)
                return "明細を1件以上追加してください。";

            // 各明細行の検証
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                int lineNum = i + 1;

                if (row["ProductID"] == DBNull.Value || Convert.ToInt32(row["ProductID"]) <= 0)
                    return string.Format("明細 {0} 行目: 商品を選択してください。", lineNum);

                short qty = Convert.ToInt16(row["Quantity"]);
                if (qty <= 0)
                    return string.Format("明細 {0} 行目: 数量は1以上を入力してください。", lineNum);

                decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
                if (unitPrice < 0)
                    return string.Format("明細 {0} 行目: 単価は0以上を入力してください。", lineNum);
            }

            return null;
        }

        /// <summary>
        /// 日付文字列をパースする
        /// </summary>
        private DateTime? ParseDate(string text)
        {
            DateTime dt;
            if (DateTime.TryParse(text, out dt))
                return dt;
            return null;
        }

        /// <summary>
        /// 情報メッセージを表示する
        /// </summary>
        private void ShowInfo(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "info",
                "alert(" + System.Web.HttpUtility.JavaScriptStringEncode(message, true) + ");", true);
        }

        /// <summary>
        /// 数量変更時の合計再計算（ステップ5-3）
        /// </summary>
        protected void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQuantity = (TextBox)sender;
            GridViewRow row = (GridViewRow)txtQuantity.NamingContainer;
            int rowIndex = row.RowIndex;

            TextBox txtUnitPrice = (TextBox)row.FindControl("txtUnitPrice");
            TextBox txtDiscount = (TextBox)row.FindControl("txtDiscount");
            Label lblLineTotal = (Label)row.FindControl("lblLineTotal");

            decimal unitPrice;
            short quantity;
            float discount;
            decimal.TryParse(txtUnitPrice.Text, out unitPrice);
            short.TryParse(txtQuantity.Text, out quantity);
            float.TryParse(txtDiscount.Text, out discount);

            decimal lineTotal = (unitPrice - (decimal)discount) * quantity;
            lblLineTotal.Text = lineTotal.ToString("F2");

            // ViewState の DetailsTable を更新
            if (DetailsTable != null && rowIndex < DetailsTable.Rows.Count)
            {
                DataRow dr = DetailsTable.Rows[rowIndex];
                dr["Quantity"] = quantity;
                dr["UnitPrice"] = unitPrice;
                dr["Discount"] = discount;
                dr["LineTotal"] = lineTotal;
                DetailsTable = DetailsTable;
            }

            CalculateTotals();
        }

        /// <summary>
        /// GridView の現在の入力値を ViewState の DetailsTable に同期する
        /// </summary>
        private void SyncDetailsFromGrid()
        {
            DataTable dt = DetailsTable;
            if (dt == null) return;

            for (int i = 0; i < gvDetails.Rows.Count && i < dt.Rows.Count; i++)
            {
                GridViewRow gvRow = gvDetails.Rows[i];
                DataRow dr = dt.Rows[i];

                DropDownList ddlProd = (DropDownList)gvRow.FindControl("ddlProduct");
                TextBox txtPrice = (TextBox)gvRow.FindControl("txtUnitPrice");
                TextBox txtQty = (TextBox)gvRow.FindControl("txtQuantity");
                TextBox txtDisc = (TextBox)gvRow.FindControl("txtDiscount");

                // ProductID
                int productId;
                if (ddlProd != null && int.TryParse(ddlProd.SelectedValue, out productId))
                    dr["ProductID"] = productId;
                else
                    dr["ProductID"] = DBNull.Value;

                // UnitPrice
                decimal unitPrice;
                if (txtPrice != null && decimal.TryParse(txtPrice.Text, out unitPrice))
                    dr["UnitPrice"] = unitPrice;
                else
                    dr["UnitPrice"] = 0m;

                // Quantity
                short qty;
                if (txtQty != null && short.TryParse(txtQty.Text, out qty))
                    dr["Quantity"] = qty;
                else
                    dr["Quantity"] = (short)0;

                // Discount
                float disc;
                if (txtDisc != null && float.TryParse(txtDisc.Text, out disc))
                    dr["Discount"] = disc;
                else
                    dr["Discount"] = 0f;

                // LineTotal
                decimal price = Convert.ToDecimal(dr["UnitPrice"]);
                short quantity = Convert.ToInt16(dr["Quantity"]);
                float discount = Convert.ToSingle(dr["Discount"]);
                dr["LineTotal"] = (price - (decimal)discount) * quantity;
            }

            DetailsTable = dt;
        }
    }
}
