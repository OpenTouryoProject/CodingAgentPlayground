using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // マスタデータ保持用フィールド
        private DataTable dtCustomers;
        private DataTable dtEmployees;
        private DataTable dtProducts;
        private DataTable dtShippers;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロードイベント
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadMasterData();

                // 明細のある受注IDで受注・受注明細をロード
                txtOrderId.Text = "10248";
                LoadOrderData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("データの読み込みに失敗しました。\n" + ex.Message,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// マスタデータをロードし、各ドロップダウンに設定する
        /// </summary>
        private void LoadMasterData()
        {
            // 顧客一覧
            dtCustomers = DbHelper.GetCustomers();
            cmbCustomer.DataSource = dtCustomers;
            cmbCustomer.DisplayMember = "CompanyName";
            cmbCustomer.ValueMember = "CustomerID";
            cmbCustomer.SelectedIndex = -1;

            // 従業員一覧
            dtEmployees = DbHelper.GetEmployees();
            cmbEmployee.DataSource = dtEmployees;
            cmbEmployee.DisplayMember = "EmployeeName";
            cmbEmployee.ValueMember = "EmployeeID";
            cmbEmployee.SelectedIndex = -1;

            // 商品一覧
            dtProducts = DbHelper.GetProducts();
            colProduct.DataSource = dtProducts;
            colProduct.DisplayMember = "ProductName";
            colProduct.ValueMember = "ProductID";

            // 配送業者一覧
            dtShippers = DbHelper.GetShippers();
            cmbShipper.DataSource = dtShippers;
            cmbShipper.DisplayMember = "CompanyName";
            cmbShipper.ValueMember = "ShipperID";
            cmbShipper.SelectedIndex = -1;
        }

        /// <summary>
        /// 受注情報を読み込み、フォームに表示する
        /// </summary>
        private void LoadOrderData()
        {
            if (string.IsNullOrWhiteSpace(txtOrderId.Text))
            {
                MessageBox.Show("受注IDを入力してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int orderId;
            if (!int.TryParse(txtOrderId.Text.Trim(), out orderId))
            {
                MessageBox.Show("受注IDは数値で入力してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 受注ヘッダ取得
                DataTable dtOrder = DbOrder.GetOrder(orderId);
                if (dtOrder.Rows.Count == 0)
                {
                    MessageBox.Show("指定された受注IDのデータが見つかりません。",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataRow orderRow = dtOrder.Rows[0];
                DisplayOrderInfo(orderRow);

                // 受注明細取得
                DataTable dtDetails = DbOrder.GetOrderDetails(orderId);
                DisplayOrderDetails(dtDetails);

                // 合計金額計算
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("受注情報の読み込みに失敗しました。\n" + ex.Message,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 受注ヘッダ情報をフォームに表示する
        /// </summary>
        private void DisplayOrderInfo(DataRow orderRow)
        {
            // 顧客選択
            if (orderRow["CustomerID"] != DBNull.Value)
            {
                cmbCustomer.SelectedValue = orderRow["CustomerID"].ToString();
            }

            // 従業員選択
            if (orderRow["EmployeeID"] != DBNull.Value)
            {
                cmbEmployee.SelectedValue = orderRow["EmployeeID"];
            }

            // 日付
            if (orderRow["OrderDate"] != DBNull.Value)
            {
                dtpOrderDate.Checked = true;
                dtpOrderDate.Value = (DateTime)orderRow["OrderDate"];
            }
            else
            {
                dtpOrderDate.Checked = false;
            }

            if (orderRow["RequiredDate"] != DBNull.Value)
            {
                dtpRequiredDate.Checked = true;
                dtpRequiredDate.Value = (DateTime)orderRow["RequiredDate"];
            }
            else
            {
                dtpRequiredDate.Checked = false;
            }

            if (orderRow["ShippedDate"] != DBNull.Value)
            {
                dtpShippedDate.Checked = true;
                dtpShippedDate.Value = (DateTime)orderRow["ShippedDate"];
            }
            else
            {
                dtpShippedDate.Checked = false;
            }

            // 配送業者選択
            if (orderRow["ShipVia"] != DBNull.Value)
            {
                cmbShipper.SelectedValue = orderRow["ShipVia"];
            }

            // 配送情報
            txtShipName.Text = orderRow["ShipName"] != DBNull.Value ? orderRow["ShipName"].ToString() : "";
            txtShipAddress.Text = orderRow["ShipAddress"] != DBNull.Value ? orderRow["ShipAddress"].ToString() : "";
            txtShipCity.Text = orderRow["ShipCity"] != DBNull.Value ? orderRow["ShipCity"].ToString() : "";
            txtShipRegion.Text = orderRow["ShipRegion"] != DBNull.Value ? orderRow["ShipRegion"].ToString() : "";
            txtShipPostalCode.Text = orderRow["ShipPostalCode"] != DBNull.Value ? orderRow["ShipPostalCode"].ToString() : "";
            txtShipCountry.Text = orderRow["ShipCountry"] != DBNull.Value ? orderRow["ShipCountry"].ToString() : "";
            txtFreight.Text = orderRow["Freight"] != DBNull.Value ? Convert.ToDecimal(orderRow["Freight"]).ToString("F2") : "0.00";
        }

        /// <summary>
        /// 受注明細をDataGridViewに表示する
        /// </summary>
        private void DisplayOrderDetails(DataTable dtDetails)
        {
            dgvOrderDetails.Rows.Clear();

            foreach (DataRow row in dtDetails.Rows)
            {
                int rowIndex = dgvOrderDetails.Rows.Add();
                DataGridViewRow dgvRow = dgvOrderDetails.Rows[rowIndex];

                dgvRow.Cells["colProduct"].Value = row["ProductID"];
                dgvRow.Cells["colQuantity"].Value = Convert.ToInt32(row["Quantity"]);
                dgvRow.Cells["colUnitPrice"].Value = Convert.ToDecimal(row["UnitPrice"]).ToString("F2");
                dgvRow.Cells["colDiscount"].Value = Convert.ToDecimal(row["Discount"]).ToString("F2");

                // 行合計 = (単価 - 割引) × 数量
                decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
                decimal discount = Convert.ToDecimal(row["Discount"]);
                int quantity = Convert.ToInt32(row["Quantity"]);
                decimal lineTotal = (unitPrice - discount) * quantity;
                dgvRow.Cells["colLineTotal"].Value = lineTotal.ToString("F2");
            }
        }

        /// <summary>
        /// 合計金額を計算して表示する
        /// </summary>
        private void CalculateTotals()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                if (row.Cells["colLineTotal"].Value != null)
                {
                    decimal lineTotal;
                    if (decimal.TryParse(row.Cells["colLineTotal"].Value.ToString(), out lineTotal))
                    {
                        total += lineTotal;
                    }
                }
            }

            lblTotalAmountValue.Text = total.ToString("C");
        }

        /// <summary>
        /// 顧客選択変更イベント：電話番号、住所を表示
        /// </summary>
        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedIndex < 0 || dtCustomers == null)
            {
                lblCustomerPhoneValue.Text = "-";
                lblCustomerAddressValue.Text = "-";
                return;
            }

            DataRowView drv = cmbCustomer.SelectedItem as DataRowView;
            if (drv != null)
            {
                lblCustomerPhoneValue.Text = drv["Phone"] != DBNull.Value ? drv["Phone"].ToString() : "-";
                string address = "";
                if (drv["Address"] != DBNull.Value) address += drv["Address"].ToString();
                if (drv["City"] != DBNull.Value) address += ", " + drv["City"].ToString();
                if (drv["Country"] != DBNull.Value) address += ", " + drv["Country"].ToString();
                lblCustomerAddressValue.Text = string.IsNullOrEmpty(address) ? "-" : address;
            }
        }

        /// <summary>
        /// 従業員選択変更イベント：職位、内線番号を表示
        /// </summary>
        private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEmployee.SelectedIndex < 0 || dtEmployees == null)
            {
                lblEmployeeTitleValue.Text = "-";
                lblEmployeeExtensionValue.Text = "-";
                return;
            }

            DataRowView drv = cmbEmployee.SelectedItem as DataRowView;
            if (drv != null)
            {
                lblEmployeeTitleValue.Text = drv["Title"] != DBNull.Value ? drv["Title"].ToString() : "-";
                lblEmployeeExtensionValue.Text = drv["Extension"] != DBNull.Value ? drv["Extension"].ToString() : "-";
            }
        }

        /// <summary>
        /// 配送業者選択変更イベント：電話番号を表示
        /// </summary>
        private void cmbShipper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbShipper.SelectedIndex < 0 || dtShippers == null)
            {
                lblShipperPhoneValue.Text = "-";
                return;
            }

            DataRowView drv = cmbShipper.SelectedItem as DataRowView;
            if (drv != null)
            {
                lblShipperPhoneValue.Text = drv["Phone"] != DBNull.Value ? drv["Phone"].ToString() : "-";
            }
        }

        /// <summary>
        /// DataGridView内のComboBox変更を即座にコミットする
        /// </summary>
        private void dgvOrderDetails_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvOrderDetails.IsCurrentCellDirty)
            {
                dgvOrderDetails.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// DataGridViewのセル値変更イベント：商品変更時に単価を自動設定
        /// </summary>
        private void dgvOrderDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 商品列が変更された場合
            if (e.ColumnIndex == colProduct.Index)
            {
                var productIdValue = dgvOrderDetails.Rows[e.RowIndex].Cells["colProduct"].Value;
                if (productIdValue != null && dtProducts != null)
                {
                    DataRow[] rows = dtProducts.Select("ProductID = " + productIdValue.ToString());
                    if (rows.Length > 0)
                    {
                        decimal unitPrice = Convert.ToDecimal(rows[0]["UnitPrice"]);
                        dgvOrderDetails.Rows[e.RowIndex].Cells["colUnitPrice"].Value = unitPrice.ToString("F2");
                        dgvOrderDetails.Rows[e.RowIndex].Cells["colDiscount"].Value = "0.00";

                        // 数量が入力済みなら行合計を再計算
                        RecalculateLineTotal(e.RowIndex);
                    }
                }
            }
            // 数量・単価・割引列が変更された場合も行合計を再計算
            else if (e.ColumnIndex == colQuantity.Index ||
                     e.ColumnIndex == colUnitPrice.Index ||
                     e.ColumnIndex == colDiscount.Index)
            {
                RecalculateLineTotal(e.RowIndex);
            }
        }

        /// <summary>
        /// 指定行の合計金額を再計算する
        /// </summary>
        private void RecalculateLineTotal(int rowIndex)
        {
            var row = dgvOrderDetails.Rows[rowIndex];

            decimal unitPrice = 0;
            decimal discount = 0;
            int quantity = 0;

            if (row.Cells["colUnitPrice"].Value != null)
                decimal.TryParse(row.Cells["colUnitPrice"].Value.ToString(), out unitPrice);
            if (row.Cells["colDiscount"].Value != null)
                decimal.TryParse(row.Cells["colDiscount"].Value.ToString(), out discount);
            if (row.Cells["colQuantity"].Value != null)
                int.TryParse(row.Cells["colQuantity"].Value.ToString(), out quantity);

            decimal lineTotal = (unitPrice - discount) * quantity;
            row.Cells["colLineTotal"].Value = lineTotal.ToString("F2");

            CalculateTotals();
        }

        /// <summary>
        /// 明細追加ボタンクリック：DataGridViewに新しい空行を追加
        /// </summary>
        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvOrderDetails.Rows.Add();
            dgvOrderDetails.Rows[rowIndex].Cells["colQuantity"].Value = 1;
            dgvOrderDetails.Rows[rowIndex].Cells["colUnitPrice"].Value = "0.00";
            dgvOrderDetails.Rows[rowIndex].Cells["colDiscount"].Value = "0.00";
            dgvOrderDetails.Rows[rowIndex].Cells["colLineTotal"].Value = "0.00";
            dgvOrderDetails.CurrentCell = dgvOrderDetails.Rows[rowIndex].Cells["colProduct"];
        }

        /// <summary>
        /// DataGridViewのセルクリックイベント：削除ボタンクリックを検知
        /// </summary>
        private void dgvOrderDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == colDelete.Index)
            {
                DialogResult result = MessageBox.Show("この明細行を削除しますか？",
                    "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dgvOrderDetails.Rows.RemoveAt(e.RowIndex);
                    CalculateTotals();
                }
            }
        }

        /// <summary>
        /// 入力検証を実行する
        /// </summary>
        private bool ValidateInput()
        {
            // 顧客の選択チェック
            if (cmbCustomer.SelectedIndex < 0)
            {
                MessageBox.Show("顧客を選択してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCustomer.Focus();
                return false;
            }

            // 従業員の選択チェック
            if (cmbEmployee.SelectedIndex < 0)
            {
                MessageBox.Show("従業員を選択してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEmployee.Focus();
                return false;
            }

            // 配送業者の選択チェック
            if (cmbShipper.SelectedIndex < 0)
            {
                MessageBox.Show("配送業者を選択してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbShipper.Focus();
                return false;
            }

            // 運送料の妥当性チェック
            decimal freight;
            if (!decimal.TryParse(txtFreight.Text, out freight) || freight < 0)
            {
                MessageBox.Show("運送料は0以上の数値を入力してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFreight.Focus();
                return false;
            }

            // 明細が1件以上存在するか
            if (dgvOrderDetails.Rows.Count == 0)
            {
                MessageBox.Show("受注明細を1件以上追加してください。",
                    "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 各明細行の検証
            for (int i = 0; i < dgvOrderDetails.Rows.Count; i++)
            {
                var row = dgvOrderDetails.Rows[i];
                int rowNum = i + 1;

                // 商品の選択チェック
                if (row.Cells["colProduct"].Value == null)
                {
                    MessageBox.Show("明細行 " + rowNum + " の商品を選択してください。",
                        "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvOrderDetails.CurrentCell = row.Cells["colProduct"];
                    return false;
                }

                // 数量チェック
                int quantity;
                if (row.Cells["colQuantity"].Value == null ||
                    !int.TryParse(row.Cells["colQuantity"].Value.ToString(), out quantity) ||
                    quantity <= 0)
                {
                    MessageBox.Show("明細行 " + rowNum + " の数量は1以上の整数を入力してください。",
                        "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvOrderDetails.CurrentCell = row.Cells["colQuantity"];
                    return false;
                }

                // 単価チェック
                decimal unitPrice;
                if (row.Cells["colUnitPrice"].Value == null ||
                    !decimal.TryParse(row.Cells["colUnitPrice"].Value.ToString(), out unitPrice) ||
                    unitPrice < 0)
                {
                    MessageBox.Show("明細行 " + rowNum + " の単価は0以上の数値を入力してください。",
                        "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvOrderDetails.CurrentCell = row.Cells["colUnitPrice"];
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// DataGridViewから受注明細DataTableを構築する
        /// </summary>
        private DataTable CollectOrderDetails()
        {
            var dt = new DataTable();
            dt.Columns.Add("ProductID", typeof(int));
            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("Quantity", typeof(short));
            dt.Columns.Add("Discount", typeof(float));

            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["ProductID"] = Convert.ToInt32(row.Cells["colProduct"].Value);
                dr["UnitPrice"] = Convert.ToDecimal(row.Cells["colUnitPrice"].Value.ToString());
                dr["Quantity"] = Convert.ToInt16(row.Cells["colQuantity"].Value.ToString());

                float discount = 0;
                if (row.Cells["colDiscount"].Value != null)
                    float.TryParse(row.Cells["colDiscount"].Value.ToString(), out discount);
                dr["Discount"] = discount;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// 作成ボタンクリック処理
        /// </summary>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                string customerId = cmbCustomer.SelectedValue.ToString();
                int employeeId = Convert.ToInt32(cmbEmployee.SelectedValue);
                DateTime? orderDate = dtpOrderDate.Checked ? dtpOrderDate.Value : (DateTime?)null;
                DateTime? requiredDate = dtpRequiredDate.Checked ? dtpRequiredDate.Value : (DateTime?)null;
                DateTime? shippedDate = dtpShippedDate.Checked ? dtpShippedDate.Value : (DateTime?)null;
                int shipVia = Convert.ToInt32(cmbShipper.SelectedValue);
                decimal freight = Convert.ToDecimal(txtFreight.Text);
                DataTable orderDetails = CollectOrderDetails();

                int newOrderId = DbOrder.CreateOrder(
                    customerId, employeeId,
                    orderDate, requiredDate, shippedDate,
                    shipVia, freight,
                    txtShipName.Text, txtShipAddress.Text, txtShipCity.Text,
                    txtShipRegion.Text, txtShipPostalCode.Text, txtShipCountry.Text,
                    orderDetails);

                MessageBox.Show("受注を作成しました。\n受注ID: " + newOrderId,
                    "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtOrderId.Text = newOrderId.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("受注の作成に失敗しました。\n" + ex.Message,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 更新ボタンクリック処理
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOrderId.Text))
            {
                MessageBox.Show("更新対象の受注IDがありません。",
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput()) return;

            try
            {
                int orderId = Convert.ToInt32(txtOrderId.Text.Trim());
                string customerId = cmbCustomer.SelectedValue.ToString();
                int employeeId = Convert.ToInt32(cmbEmployee.SelectedValue);
                DateTime? orderDate = dtpOrderDate.Checked ? dtpOrderDate.Value : (DateTime?)null;
                DateTime? requiredDate = dtpRequiredDate.Checked ? dtpRequiredDate.Value : (DateTime?)null;
                DateTime? shippedDate = dtpShippedDate.Checked ? dtpShippedDate.Value : (DateTime?)null;
                int shipVia = Convert.ToInt32(cmbShipper.SelectedValue);
                decimal freight = Convert.ToDecimal(txtFreight.Text);
                DataTable orderDetails = CollectOrderDetails();

                DbOrder.UpdateOrder(
                    orderId, customerId, employeeId,
                    orderDate, requiredDate, shippedDate,
                    shipVia, freight,
                    txtShipName.Text, txtShipAddress.Text, txtShipCity.Text,
                    txtShipRegion.Text, txtShipPostalCode.Text, txtShipCountry.Text,
                    orderDetails);

                MessageBox.Show("受注を更新しました。",
                    "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("受注の更新に失敗しました。\n" + ex.Message,
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// キャンセルボタンクリック処理
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("変更を破棄しますか？",
                "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
