using System;
using System.Collections.Generic;
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

            // ドロップダウン変更イベント（フェーズ4）
            this.cmbCustomer.SelectedIndexChanged += cmbCustomer_SelectedIndexChanged;
            this.cmbEmployee.SelectedIndexChanged += cmbEmployee_SelectedIndexChanged;
            this.cmbShipper.SelectedIndexChanged += cmbShipper_SelectedIndexChanged;
            this.dgvDetails.CurrentCellDirtyStateChanged += dgvDetails_CurrentCellDirtyStateChanged;
            this.dgvDetails.CellValueChanged += dgvDetails_CellValueChanged;
            this.dgvDetails.DataError += dgvDetails_DataError;

            // 明細操作イベント（フェーズ5）
            this.btnAddDetail.Click += btnAddDetail_Click;
            this.dgvDetails.CellContentClick += dgvDetails_CellContentClick;

            // 保存・キャンセル（フェーズ6）
            this.btnCreate.Click += btnCreate_Click;
            this.btnUpdate.Click += btnUpdate_Click;
            this.btnCancel.Click += btnCancel_Click;
        }

        // 編集モード（true）/ 照会モード（false）
        private bool _editable = true;

        // ---- フォームロード（ステップ3-1, 3-2, 3-3） ----

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // マスタの DataSource 設定でも SelectedIndexChanged が発火するため抑制
                _loading = true;
                try
                {
                    LoadMasters();
                }
                finally
                {
                    _loading = false;
                }

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

                // 選択に対応した関連情報（電話・住所・職位・内線）を表示
                RefreshRelatedInfo();

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

                // 読み込み後は編集モードにする
                SetEditable(true);
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

        // ---- フェーズ4: ドロップダウン変更イベント ----

        /// <summary>
        /// 顧客・従業員・配送業者の選択に対応する関連情報をまとめて更新する。
        /// </summary>
        private void RefreshRelatedInfo()
        {
            UpdateCustomerInfo();
            UpdateEmployeeInfo();
            UpdateShipperInfo();
        }

        // ステップ4-1: 顧客選択変更
        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }
            UpdateCustomerInfo();
        }

        private void UpdateCustomerInfo()
        {
            DataRowView row = cmbCustomer.SelectedItem as DataRowView;
            if (row == null)
            {
                lblCustomerPhoneValue.Text = string.Empty;
                lblCustomerAddressValue.Text = string.Empty;
                return;
            }
            lblCustomerPhoneValue.Text = ToStr(row["Phone"]);
            lblCustomerAddressValue.Text = BuildAddress(row);
        }

        // ステップ4-2: 従業員選択変更
        private void cmbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }
            UpdateEmployeeInfo();
        }

        private void UpdateEmployeeInfo()
        {
            DataRowView row = cmbEmployee.SelectedItem as DataRowView;
            if (row == null)
            {
                lblEmployeeTitleValue.Text = string.Empty;
                lblEmployeeExtensionValue.Text = string.Empty;
                return;
            }
            lblEmployeeTitleValue.Text = ToStr(row["Title"]);
            lblEmployeeExtensionValue.Text = ToStr(row["Extension"]);
        }

        // ステップ4-4: 配送業者選択変更
        private void cmbShipper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }
            UpdateShipperInfo();
        }

        private void UpdateShipperInfo()
        {
            DataRowView row = cmbShipper.SelectedItem as DataRowView;
            if (row == null)
            {
                lblShipperPhoneValue.Text = string.Empty;
                return;
            }
            lblShipperPhoneValue.Text = ToStr(row["Phone"]);
        }

        // ステップ4-3: 商品選択変更（DataGridView内）

        /// <summary>
        /// ComboBox セルの選択を即時コミットし、CellValueChanged を発火させる。
        /// </summary>
        private void dgvDetails_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDetails.IsCurrentCellDirty && dgvDetails.CurrentCell is DataGridViewComboBoxCell)
            {
                dgvDetails.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loading || e.RowIndex < 0)
            {
                return;
            }

            // 商品変更 → 選択された商品の単価・在庫を反映
            if (e.ColumnIndex == colProduct.Index)
            {
                UpdateProductInfo(e.RowIndex);
                RecalculateTotals();
            }
            // ステップ5-3: 数量・割引・単価変更 → 合計再計算
            else if (e.ColumnIndex == colQuantity.Index
                  || e.ColumnIndex == colDiscount.Index
                  || e.ColumnIndex == colUnitPrice.Index)
            {
                RecalculateTotals();
            }
        }

        /// <summary>
        /// 指定行の商品IDに基づき、単価をマスタから反映し、在庫情報をツールチップに表示する。
        /// </summary>
        private void UpdateProductInfo(int rowIndex)
        {
            DataGridViewRow gridRow = dgvDetails.Rows[rowIndex];
            object productIdObj = gridRow.Cells[colProduct.Name].Value;
            if (productIdObj == null || productIdObj == DBNull.Value)
            {
                return;
            }

            DataRow[] found = _products.Select("ProductID = " + Convert.ToInt32(productIdObj));
            if (found.Length == 0)
            {
                return;
            }

            DataRow product = found[0];
            gridRow.Cells[colUnitPrice.Name].Value = product["UnitPrice"];

            // 在庫情報は専用の表示欄がないため商品セルのツールチップに表示
            gridRow.Cells[colProduct.Name].ToolTipText = "在庫: " + ToStr(product["UnitsInStock"]);
        }

        private void dgvDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // ComboBox セルの一時的な値不一致などはユーザーへのダイアログを抑止
            e.ThrowException = false;
        }

        // ---- フェーズ5: 明細操作イベント ----

        // ステップ5-1: 明細追加ボタンクリック
        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (_orderDetails == null)
            {
                MessageBox.Show("先に受注を読み込んでください。", "情報",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 明細は (OrderID, ProductID) が主キーのため、既存明細で未使用の商品を
            // 既定選択にする（先頭固定だと連続追加でキー重複エラーになる）
            DataRow defaultProduct = GetFirstUnusedProduct();
            if (defaultProduct == null)
            {
                MessageBox.Show("追加できる商品がありません（全商品が明細に登録済みです）。", "情報",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataRow newRow = _orderDetails.NewRow();
            newRow["OrderID"] = GetCurrentOrderId();
            newRow["ProductID"] = defaultProduct["ProductID"];
            newRow["UnitPrice"] = defaultProduct["UnitPrice"];
            newRow["Quantity"] = (short)1;
            newRow["Discount"] = 0f;

            _orderDetails.Rows.Add(newRow);

            RecalculateTotals();
        }

        // ステップ5-2: 明細削除ボタンクリック（行内の削除ボタン列）
        private void dgvDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colDelete.Index)
            {
                return;
            }
            if (!_editable)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "この明細行を削除しますか？", "削除確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            // 対象行を削除（既存行は RowState=Deleted となり、更新時に DELETE される）
            DataRowView drv = dgvDetails.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv != null)
            {
                drv.Row.Delete();
            }

            RecalculateTotals();
        }

        /// <summary>
        /// 現在の明細（削除済みを除く）で未使用の商品を、商品マスタの先頭から探して返す。
        /// 全商品が使用済みの場合は null。
        /// </summary>
        private DataRow GetFirstUnusedProduct()
        {
            if (_products == null || _products.Rows.Count == 0)
            {
                return null;
            }

            HashSet<int> usedProductIds = new HashSet<int>();
            if (_orderDetails != null)
            {
                foreach (DataRow row in _orderDetails.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    usedProductIds.Add(Convert.ToInt32(row["ProductID"]));
                }
            }

            foreach (DataRow product in _products.Rows)
            {
                if (!usedProductIds.Contains(Convert.ToInt32(product["ProductID"])))
                {
                    return product;
                }
            }
            return null;
        }

        private int GetCurrentOrderId()
        {
            if (_orderTable != null && _orderTable.Rows.Count > 0)
            {
                return Convert.ToInt32(_orderTable.Rows[0]["OrderID"]);
            }
            int id;
            int.TryParse(txtOrderId.Text.Trim(), out id);
            return id;
        }

        // ---- フェーズ6: 保存・キャンセル処理 ----

        // ステップ6-2: 作成ボタンクリック（新規登録）
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (_orderTable == null || _orderTable.Rows.Count == 0)
            {
                Warn("先に受注を読み込んでください。");
                return;
            }
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                dgvDetails.EndEdit();
                WriteFormToOrderRow(_orderTable.Rows[0]);

                int newOrderId = DbOrder.InsertOrder(_orderTable.Rows[0], _orderDetails);

                MessageBox.Show("受注を作成しました。新しい受注ID: " + newOrderId, "作成完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 作成した受注を読み込んで表示（編集モードのまま）
                txtOrderId.Text = newOrderId.ToString();
                LoadOrder();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ステップ6-3: 更新ボタンクリック
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_orderTable == null || _orderTable.Rows.Count == 0)
            {
                Warn("先に受注を読み込んでください。");
                return;
            }
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                dgvDetails.EndEdit();
                WriteFormToOrderRow(_orderTable.Rows[0]);

                DbOrder.UpdateOrder(_orderTable.Rows[0], _orderDetails);

                MessageBox.Show("受注を更新しました。", "更新完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 保存後の状態を再読込し、照会モード（読み取り専用）に遷移
                LoadOrder();
                SetEditable(false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ステップ6-4: キャンセルボタンクリック
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "変更を破棄して照会モードに戻りますか？", "キャンセル確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // 変更を破棄するため再読込し、照会モード（読み取り専用）に遷移
                LoadOrder();
                SetEditable(false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ステップ6-1: 入力検証
        private bool ValidateInput()
        {
            if (cmbCustomer.SelectedIndex < 0 || cmbCustomer.SelectedValue == null)
            {
                Warn("顧客を選択してください。");
                return false;
            }
            if (cmbEmployee.SelectedIndex < 0 || cmbEmployee.SelectedValue == null)
            {
                Warn("従業員を選択してください。");
                return false;
            }

            // 運送料（入力があれば 0 以上の数値）
            if (!string.IsNullOrWhiteSpace(txtFreight.Text))
            {
                decimal freight;
                if (!decimal.TryParse(txtFreight.Text.Trim(), out freight) || freight < 0)
                {
                    Warn("運送料は 0 以上の数値を入力してください。");
                    return false;
                }
            }

            dgvDetails.EndEdit();

            int detailCount = 0;
            if (_orderDetails != null)
            {
                foreach (DataRow row in _orderDetails.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    detailCount++;

                    if (row["ProductID"] == DBNull.Value)
                    {
                        Warn("商品が未選択の明細があります。");
                        return false;
                    }
                    if (Convert.ToInt32(row["Quantity"]) <= 0)
                    {
                        Warn("数量は 1 以上を入力してください。");
                        return false;
                    }
                    double discount = Convert.ToDouble(row["Discount"]);
                    if (discount < 0 || discount > 1)
                    {
                        Warn("割引は 0 ～ 1 の範囲で入力してください。");
                        return false;
                    }
                    if (Convert.ToDecimal(row["UnitPrice"]) < 0)
                    {
                        Warn("単価は 0 以上を入力してください。");
                        return false;
                    }
                }
            }

            if (detailCount < 1)
            {
                Warn("明細を 1 件以上入力してください。");
                return false;
            }

            return true;
        }

        /// <summary>
        /// フォームの入力値を受注 DataRow に書き戻す。
        /// </summary>
        private void WriteFormToOrderRow(DataRow row)
        {
            row["CustomerID"] = ObjOrNull(cmbCustomer.SelectedValue);
            row["EmployeeID"] = ObjOrNull(cmbEmployee.SelectedValue);
            row["ShipVia"] = ObjOrNull(cmbShipper.SelectedValue);
            row["OrderDate"] = DateOrNull(dtpOrderDate);
            row["RequiredDate"] = DateOrNull(dtpRequiredDate);
            row["ShippedDate"] = DateOrNull(dtpShippedDate);
            row["ShipName"] = StrOrNull(txtShipName.Text);
            row["ShipAddress"] = StrOrNull(txtShipAddress.Text);
            row["ShipCity"] = StrOrNull(txtShipCity.Text);
            row["ShipRegion"] = StrOrNull(txtShipRegion.Text);
            row["ShipPostalCode"] = StrOrNull(txtShipPostalCode.Text);
            row["ShipCountry"] = StrOrNull(txtShipCountry.Text);
            row["Freight"] = string.IsNullOrWhiteSpace(txtFreight.Text)
                ? (object)DBNull.Value
                : Convert.ToDecimal(txtFreight.Text.Trim());
        }

        /// <summary>
        /// 編集モード / 照会モード（読み取り専用）を切り替える。
        /// </summary>
        private void SetEditable(bool editable)
        {
            _editable = editable;

            cmbCustomer.Enabled = editable;
            cmbEmployee.Enabled = editable;
            cmbShipper.Enabled = editable;
            dtpOrderDate.Enabled = editable;
            dtpRequiredDate.Enabled = editable;
            dtpShippedDate.Enabled = editable;

            txtShipName.ReadOnly = !editable;
            txtShipAddress.ReadOnly = !editable;
            txtShipCity.ReadOnly = !editable;
            txtShipRegion.ReadOnly = !editable;
            txtShipPostalCode.ReadOnly = !editable;
            txtShipCountry.ReadOnly = !editable;
            txtFreight.ReadOnly = !editable;

            dgvDetails.ReadOnly = !editable;
            btnAddDetail.Enabled = editable;
            btnCreate.Enabled = editable;
            btnUpdate.Enabled = editable;
            btnCancel.Enabled = editable;
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

        /// <summary>
        /// 顧客行の住所関連列を連結して1つの住所文字列にする。
        /// </summary>
        private static string BuildAddress(DataRowView row)
        {
            string[] parts =
            {
                ToStr(row["Address"]),
                ToStr(row["City"]),
                ToStr(row["Region"]),
                ToStr(row["PostalCode"]),
                ToStr(row["Country"])
            };
            return string.Join(" ", Array.FindAll(parts, p => !string.IsNullOrEmpty(p)));
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

        private void Warn(string message)
        {
            MessageBox.Show(message, "入力エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static object ObjOrNull(object value)
        {
            return value == null ? (object)DBNull.Value : value;
        }

        private static object DateOrNull(DateTimePicker picker)
        {
            return picker.Checked ? (object)picker.Value : DBNull.Value;
        }

        private static object StrOrNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text.Trim();
        }
    }
}
