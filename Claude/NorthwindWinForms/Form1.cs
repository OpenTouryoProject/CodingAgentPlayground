using System;
using System.Data;
using System.Windows.Forms;

namespace NorthwindWinForms
{
    public partial class Form1 : Form
    {
        // マスタデータ（フォームロード時に全件ロード）
        private DataTable _customers;
        private DataTable _employees;
        private DataTable _products;
        private DataTable _shippers;

        // 編集対象の受注・受注明細
        private DataTable _orderTable;
        private DataTable _orderDetails;

        // ロード中のイベント抑制フラグ
        private bool _loading;

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.btnLoad.Click += btnLoad_Click;
        }

        // ---- フォームロード（ステップ3-1, 3-2, 3-3） ----

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadMasters();

                // 受注IDが未指定の場合は動作確認用に既定値を設定
                if (string.IsNullOrWhiteSpace(txtOrderId.Text))
                {
                    txtOrderId.Text = "10248";
                }

                LoadOrder();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LoadOrder();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ---- ステップ3-1: ドロップダウンの初期データロード ----

        private void LoadMasters()
        {
            _customers = DbHelper.GetCustomers();
            _employees = DbHelper.GetEmployees();
            _products = DbHelper.GetProducts();
            _shippers = DbHelper.GetShippers();

            // 各 ComboBox に DataSource / DisplayMember / ValueMember を設定
            cmbCustomer.DisplayMember = "CompanyName";
            cmbCustomer.ValueMember = "CustomerID";
            cmbCustomer.DataSource = _customers;

            cmbEmployee.DisplayMember = "FullName";
            cmbEmployee.ValueMember = "EmployeeID";
            cmbEmployee.DataSource = _employees;

            cmbShipper.DisplayMember = "CompanyName";
            cmbShipper.ValueMember = "ShipperID";
            cmbShipper.DataSource = _shippers;

            // 明細グリッドの商品ドロップダウン列
            colProduct.DisplayMember = "ProductName";
            colProduct.ValueMember = "ProductID";
            colProduct.DataSource = _products;
        }

        // ---- ステップ3-2: 受注情報の読み込みと表示 ----

        private void LoadOrder()
        {
            int orderId;
            if (!int.TryParse(txtOrderId.Text.Trim(), out orderId))
            {
                MessageBox.Show("受注IDは数値で入力してください。", "入力エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _orderTable = DbOrder.GetOrder(orderId);
            if (_orderTable.Rows.Count == 0)
            {
                MessageBox.Show("指定された受注IDは存在しません。", "データエラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow order = _orderTable.Rows[0];

            _loading = true;
            try
            {
                // 顧客・従業員・配送業者
                SetComboValue(cmbCustomer, order["CustomerID"]);
                SetComboValue(cmbEmployee, order["EmployeeID"]);
                SetComboValue(cmbShipper, order["ShipVia"]);

                // 日付（null は DateTimePicker のチェックを外して表現）
                SetDateValue(dtpOrderDate, order["OrderDate"]);
                SetDateValue(dtpRequiredDate, order["RequiredDate"]);
                SetDateValue(dtpShippedDate, order["ShippedDate"]);

                // 配送情報
                txtShipName.Text = ToStr(order["ShipName"]);
                txtShipAddress.Text = ToStr(order["ShipAddress"]);
                txtShipCity.Text = ToStr(order["ShipCity"]);
                txtShipRegion.Text = ToStr(order["ShipRegion"]);
                txtShipPostalCode.Text = ToStr(order["ShipPostalCode"]);
                txtShipCountry.Text = ToStr(order["ShipCountry"]);
                txtFreight.Text = order["Freight"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToDecimal(order["Freight"]).ToString("0.00");

                // 受注明細を DataGridView にバインド
                _orderDetails = DbOrder.GetOrderDetails(orderId);
                dgvDetails.DataSource = _orderDetails;

                // ステップ3-3: 合計金額の計算と表示
                RecalculateTotals();
            }
            finally
            {
                _loading = false;
            }
        }

        // ---- ステップ3-3: 合計金額の計算と表示 ----

        /// <summary>
        /// 各明細行の合計金額（割引適用後）と受注全体の合計金額を計算・表示する。
        /// </summary>
        private void RecalculateTotals()
        {
            decimal grandTotal = 0m;

            foreach (DataGridViewRow row in dgvDetails.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                decimal lineTotal = CalculateLineTotal(row);
                row.Cells[colLineTotal.Name].Value = lineTotal.ToString("0.00");
                grandTotal += lineTotal;
            }

            lblTotalAmountValue.Text = grandTotal.ToString("0.00");
        }

        /// <summary>
        /// 明細1行の合計金額を計算する。
        /// 価格 = 単価 × (1 - 割引率)、合計金額 = 価格 × 数量。
        /// （Northwind の Discount は割引「率」(0～1) のため率として計算する）
        /// </summary>
        private decimal CalculateLineTotal(DataGridViewRow row)
        {
            decimal unitPrice = ToDecimal(row.Cells[colUnitPrice.Name].Value);
            decimal quantity = ToDecimal(row.Cells[colQuantity.Name].Value);
            decimal discount = ToDecimal(row.Cells[colDiscount.Name].Value);

            decimal price = unitPrice * (1m - discount);
            return Math.Round(price * quantity, 2);
        }

        // ---- ヘルパー ----

        private void SetComboValue(ComboBox combo, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                combo.SelectedIndex = -1;
            }
            else
            {
                combo.SelectedValue = value;
            }
        }

        private void SetDateValue(DateTimePicker picker, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                picker.Checked = false;
            }
            else
            {
                picker.Value = Convert.ToDateTime(value);
                picker.Checked = true;
            }
        }

        private static string ToStr(object value)
        {
            return (value == null || value == DBNull.Value) ? string.Empty : value.ToString();
        }

        private static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0m;
            }
            decimal result;
            if (decimal.TryParse(value.ToString(), out result))
            {
                return result;
            }
            return 0m;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
