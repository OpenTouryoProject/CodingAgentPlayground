namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // 受注情報エリア
            this.grpOrderInfo = new System.Windows.Forms.GroupBox();
            this.lblOrderId = new System.Windows.Forms.Label();
            this.txtOrderId = new System.Windows.Forms.TextBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblCustomerPhone = new System.Windows.Forms.Label();
            this.lblCustomerPhoneValue = new System.Windows.Forms.Label();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.lblCustomerAddressValue = new System.Windows.Forms.Label();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.cmbEmployee = new System.Windows.Forms.ComboBox();
            this.lblEmployeeTitle = new System.Windows.Forms.Label();
            this.lblEmployeeTitleValue = new System.Windows.Forms.Label();
            this.lblEmployeeExtension = new System.Windows.Forms.Label();
            this.lblEmployeeExtensionValue = new System.Windows.Forms.Label();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.dtpOrderDate = new System.Windows.Forms.DateTimePicker();
            this.lblRequiredDate = new System.Windows.Forms.Label();
            this.dtpRequiredDate = new System.Windows.Forms.DateTimePicker();
            this.lblShippedDate = new System.Windows.Forms.Label();
            this.dtpShippedDate = new System.Windows.Forms.DateTimePicker();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblTotalAmountValue = new System.Windows.Forms.Label();

            // 受注明細エリア
            this.grpOrderDetails = new System.Windows.Forms.GroupBox();
            this.dgvOrderDetails = new System.Windows.Forms.DataGridView();
            this.colProduct = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiscount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnAddDetail = new System.Windows.Forms.Button();

            // 配送情報エリア
            this.grpShippingInfo = new System.Windows.Forms.GroupBox();
            this.lblShipper = new System.Windows.Forms.Label();
            this.cmbShipper = new System.Windows.Forms.ComboBox();
            this.lblShipperPhone = new System.Windows.Forms.Label();
            this.lblShipperPhoneValue = new System.Windows.Forms.Label();
            this.lblShipName = new System.Windows.Forms.Label();
            this.txtShipName = new System.Windows.Forms.TextBox();
            this.lblShipAddress = new System.Windows.Forms.Label();
            this.txtShipAddress = new System.Windows.Forms.TextBox();
            this.lblShipCity = new System.Windows.Forms.Label();
            this.txtShipCity = new System.Windows.Forms.TextBox();
            this.lblShipRegion = new System.Windows.Forms.Label();
            this.txtShipRegion = new System.Windows.Forms.TextBox();
            this.lblShipPostalCode = new System.Windows.Forms.Label();
            this.txtShipPostalCode = new System.Windows.Forms.TextBox();
            this.lblShipCountry = new System.Windows.Forms.Label();
            this.txtShipCountry = new System.Windows.Forms.TextBox();
            this.lblFreight = new System.Windows.Forms.Label();
            this.txtFreight = new System.Windows.Forms.TextBox();

            // アクションボタン
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.grpOrderInfo.SuspendLayout();
            this.grpOrderDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetails)).BeginInit();
            this.grpShippingInfo.SuspendLayout();
            this.SuspendLayout();

            // ===== 受注情報エリア =====
            // grpOrderInfo
            this.grpOrderInfo.Location = new System.Drawing.Point(12, 12);
            this.grpOrderInfo.Name = "grpOrderInfo";
            this.grpOrderInfo.Size = new System.Drawing.Size(1010, 170);
            this.grpOrderInfo.TabIndex = 0;
            this.grpOrderInfo.TabStop = false;
            this.grpOrderInfo.Text = "受注情報";

            // 行1: 受注ID、顧客
            this.lblOrderId.AutoSize = true;
            this.lblOrderId.Location = new System.Drawing.Point(15, 25);
            this.lblOrderId.Name = "lblOrderId";
            this.lblOrderId.Size = new System.Drawing.Size(50, 12);
            this.lblOrderId.Text = "受注ID:";

            this.txtOrderId.Location = new System.Drawing.Point(80, 22);
            this.txtOrderId.Name = "txtOrderId";
            this.txtOrderId.Size = new System.Drawing.Size(80, 19);
            this.txtOrderId.ReadOnly = true;

            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(180, 25);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(40, 12);
            this.lblCustomer.Text = "顧客:";

            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.Location = new System.Drawing.Point(230, 22);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(200, 20);
            this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.cmbCustomer_SelectedIndexChanged);

            this.lblCustomerPhone.AutoSize = true;
            this.lblCustomerPhone.Location = new System.Drawing.Point(450, 25);
            this.lblCustomerPhone.Name = "lblCustomerPhone";
            this.lblCustomerPhone.Size = new System.Drawing.Size(50, 12);
            this.lblCustomerPhone.Text = "電話:";

            this.lblCustomerPhoneValue.AutoSize = true;
            this.lblCustomerPhoneValue.Location = new System.Drawing.Point(500, 25);
            this.lblCustomerPhoneValue.Name = "lblCustomerPhoneValue";
            this.lblCustomerPhoneValue.Size = new System.Drawing.Size(11, 12);
            this.lblCustomerPhoneValue.Text = "-";

            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.Location = new System.Drawing.Point(650, 25);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(40, 12);
            this.lblCustomerAddress.Text = "住所:";

            this.lblCustomerAddressValue.Location = new System.Drawing.Point(700, 25);
            this.lblCustomerAddressValue.Name = "lblCustomerAddressValue";
            this.lblCustomerAddressValue.Size = new System.Drawing.Size(290, 12);
            this.lblCustomerAddressValue.Text = "-";

            // 行2: 従業員
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(15, 55);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(50, 12);
            this.lblEmployee.Text = "従業員:";

            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.Location = new System.Drawing.Point(80, 52);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(200, 20);
            this.cmbEmployee.SelectedIndexChanged += new System.EventHandler(this.cmbEmployee_SelectedIndexChanged);

            this.lblEmployeeTitle.AutoSize = true;
            this.lblEmployeeTitle.Location = new System.Drawing.Point(300, 55);
            this.lblEmployeeTitle.Name = "lblEmployeeTitle";
            this.lblEmployeeTitle.Size = new System.Drawing.Size(40, 12);
            this.lblEmployeeTitle.Text = "職位:";

            this.lblEmployeeTitleValue.AutoSize = true;
            this.lblEmployeeTitleValue.Location = new System.Drawing.Point(350, 55);
            this.lblEmployeeTitleValue.Name = "lblEmployeeTitleValue";
            this.lblEmployeeTitleValue.Size = new System.Drawing.Size(11, 12);
            this.lblEmployeeTitleValue.Text = "-";

            this.lblEmployeeExtension.AutoSize = true;
            this.lblEmployeeExtension.Location = new System.Drawing.Point(520, 55);
            this.lblEmployeeExtension.Name = "lblEmployeeExtension";
            this.lblEmployeeExtension.Size = new System.Drawing.Size(50, 12);
            this.lblEmployeeExtension.Text = "内線:";

            this.lblEmployeeExtensionValue.AutoSize = true;
            this.lblEmployeeExtensionValue.Location = new System.Drawing.Point(570, 55);
            this.lblEmployeeExtensionValue.Name = "lblEmployeeExtensionValue";
            this.lblEmployeeExtensionValue.Size = new System.Drawing.Size(11, 12);
            this.lblEmployeeExtensionValue.Text = "-";

            // 行3: 日付
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(15, 85);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(50, 12);
            this.lblOrderDate.Text = "受注日:";

            this.dtpOrderDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrderDate.Location = new System.Drawing.Point(80, 82);
            this.dtpOrderDate.Name = "dtpOrderDate";
            this.dtpOrderDate.Size = new System.Drawing.Size(120, 19);
            this.dtpOrderDate.ShowCheckBox = true;

            this.lblRequiredDate.AutoSize = true;
            this.lblRequiredDate.Location = new System.Drawing.Point(220, 85);
            this.lblRequiredDate.Name = "lblRequiredDate";
            this.lblRequiredDate.Size = new System.Drawing.Size(50, 12);
            this.lblRequiredDate.Text = "必要日:";

            this.dtpRequiredDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRequiredDate.Location = new System.Drawing.Point(280, 82);
            this.dtpRequiredDate.Name = "dtpRequiredDate";
            this.dtpRequiredDate.Size = new System.Drawing.Size(120, 19);
            this.dtpRequiredDate.ShowCheckBox = true;

            this.lblShippedDate.AutoSize = true;
            this.lblShippedDate.Location = new System.Drawing.Point(420, 85);
            this.lblShippedDate.Name = "lblShippedDate";
            this.lblShippedDate.Size = new System.Drawing.Size(50, 12);
            this.lblShippedDate.Text = "出荷日:";

            this.dtpShippedDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpShippedDate.Location = new System.Drawing.Point(480, 82);
            this.dtpShippedDate.Name = "dtpShippedDate";
            this.dtpShippedDate.Size = new System.Drawing.Size(120, 19);
            this.dtpShippedDate.ShowCheckBox = true;

            // 行4: 合計金額
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(15, 115);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(60, 12);
            this.lblTotalAmount.Text = "合計金額:";

            this.lblTotalAmountValue.AutoSize = true;
            this.lblTotalAmountValue.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmountValue.Location = new System.Drawing.Point(80, 112);
            this.lblTotalAmountValue.Name = "lblTotalAmountValue";
            this.lblTotalAmountValue.Size = new System.Drawing.Size(15, 15);
            this.lblTotalAmountValue.Text = "0";

            // grpOrderInfo にコントロール追加
            this.grpOrderInfo.Controls.Add(this.lblOrderId);
            this.grpOrderInfo.Controls.Add(this.txtOrderId);
            this.grpOrderInfo.Controls.Add(this.lblCustomer);
            this.grpOrderInfo.Controls.Add(this.cmbCustomer);
            this.grpOrderInfo.Controls.Add(this.lblCustomerPhone);
            this.grpOrderInfo.Controls.Add(this.lblCustomerPhoneValue);
            this.grpOrderInfo.Controls.Add(this.lblCustomerAddress);
            this.grpOrderInfo.Controls.Add(this.lblCustomerAddressValue);
            this.grpOrderInfo.Controls.Add(this.lblEmployee);
            this.grpOrderInfo.Controls.Add(this.cmbEmployee);
            this.grpOrderInfo.Controls.Add(this.lblEmployeeTitle);
            this.grpOrderInfo.Controls.Add(this.lblEmployeeTitleValue);
            this.grpOrderInfo.Controls.Add(this.lblEmployeeExtension);
            this.grpOrderInfo.Controls.Add(this.lblEmployeeExtensionValue);
            this.grpOrderInfo.Controls.Add(this.lblOrderDate);
            this.grpOrderInfo.Controls.Add(this.dtpOrderDate);
            this.grpOrderInfo.Controls.Add(this.lblRequiredDate);
            this.grpOrderInfo.Controls.Add(this.dtpRequiredDate);
            this.grpOrderInfo.Controls.Add(this.lblShippedDate);
            this.grpOrderInfo.Controls.Add(this.dtpShippedDate);
            this.grpOrderInfo.Controls.Add(this.lblTotalAmount);
            this.grpOrderInfo.Controls.Add(this.lblTotalAmountValue);

            // ===== 受注明細エリア =====
            // grpOrderDetails
            this.grpOrderDetails.Location = new System.Drawing.Point(12, 188);
            this.grpOrderDetails.Name = "grpOrderDetails";
            this.grpOrderDetails.Size = new System.Drawing.Size(1010, 250);
            this.grpOrderDetails.TabIndex = 1;
            this.grpOrderDetails.TabStop = false;
            this.grpOrderDetails.Text = "受注明細";

            // DataGridView列定義
            this.colProduct.HeaderText = "商品";
            this.colProduct.Name = "colProduct";
            this.colProduct.Width = 200;

            this.colQuantity.HeaderText = "数量";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.Width = 80;

            this.colUnitPrice.HeaderText = "単価";
            this.colUnitPrice.Name = "colUnitPrice";
            this.colUnitPrice.Width = 100;

            this.colDiscount.HeaderText = "割引";
            this.colDiscount.Name = "colDiscount";
            this.colDiscount.Width = 80;

            this.colLineTotal.HeaderText = "合計金額";
            this.colLineTotal.Name = "colLineTotal";
            this.colLineTotal.Width = 120;
            this.colLineTotal.ReadOnly = true;

            this.colDelete.HeaderText = "削除";
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "削除";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.Width = 60;

            // dgvOrderDetails
            this.dgvOrderDetails.AllowUserToAddRows = false;
            this.dgvOrderDetails.AllowUserToDeleteRows = false;
            this.dgvOrderDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colProduct,
                this.colQuantity,
                this.colUnitPrice,
                this.colDiscount,
                this.colLineTotal,
                this.colDelete});
            this.dgvOrderDetails.Location = new System.Drawing.Point(15, 20);
            this.dgvOrderDetails.Name = "dgvOrderDetails";
            this.dgvOrderDetails.RowTemplate.Height = 21;
            this.dgvOrderDetails.Size = new System.Drawing.Size(980, 185);
            this.dgvOrderDetails.TabIndex = 0;
            this.dgvOrderDetails.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOrderDetails_CellValueChanged);
            this.dgvOrderDetails.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvOrderDetails_CurrentCellDirtyStateChanged);
            this.dgvOrderDetails.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOrderDetails_CellContentClick);

            // btnAddDetail
            this.btnAddDetail.Location = new System.Drawing.Point(15, 212);
            this.btnAddDetail.Name = "btnAddDetail";
            this.btnAddDetail.Size = new System.Drawing.Size(100, 28);
            this.btnAddDetail.TabIndex = 1;
            this.btnAddDetail.Text = "明細追加";
            this.btnAddDetail.UseVisualStyleBackColor = true;
            this.btnAddDetail.Click += new System.EventHandler(this.btnAddDetail_Click);

            // grpOrderDetails にコントロール追加
            this.grpOrderDetails.Controls.Add(this.dgvOrderDetails);
            this.grpOrderDetails.Controls.Add(this.btnAddDetail);

            // ===== 配送情報エリア =====
            // grpShippingInfo
            this.grpShippingInfo.Location = new System.Drawing.Point(12, 444);
            this.grpShippingInfo.Name = "grpShippingInfo";
            this.grpShippingInfo.Size = new System.Drawing.Size(1010, 200);
            this.grpShippingInfo.TabIndex = 2;
            this.grpShippingInfo.TabStop = false;
            this.grpShippingInfo.Text = "配送情報";

            // 行1: 配送業者
            this.lblShipper.AutoSize = true;
            this.lblShipper.Location = new System.Drawing.Point(15, 25);
            this.lblShipper.Name = "lblShipper";
            this.lblShipper.Size = new System.Drawing.Size(60, 12);
            this.lblShipper.Text = "配送業者:";

            this.cmbShipper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShipper.Location = new System.Drawing.Point(80, 22);
            this.cmbShipper.Name = "cmbShipper";
            this.cmbShipper.Size = new System.Drawing.Size(200, 20);
            this.cmbShipper.SelectedIndexChanged += new System.EventHandler(this.cmbShipper_SelectedIndexChanged);

            this.lblShipperPhone.AutoSize = true;
            this.lblShipperPhone.Location = new System.Drawing.Point(300, 25);
            this.lblShipperPhone.Name = "lblShipperPhone";
            this.lblShipperPhone.Size = new System.Drawing.Size(40, 12);
            this.lblShipperPhone.Text = "電話:";

            this.lblShipperPhoneValue.AutoSize = true;
            this.lblShipperPhoneValue.Location = new System.Drawing.Point(350, 25);
            this.lblShipperPhoneValue.Name = "lblShipperPhoneValue";
            this.lblShipperPhoneValue.Size = new System.Drawing.Size(11, 12);
            this.lblShipperPhoneValue.Text = "-";

            // 行2: 配送先名、配送先住所
            this.lblShipName.AutoSize = true;
            this.lblShipName.Location = new System.Drawing.Point(15, 55);
            this.lblShipName.Name = "lblShipName";
            this.lblShipName.Size = new System.Drawing.Size(60, 12);
            this.lblShipName.Text = "配送先名:";

            this.txtShipName.Location = new System.Drawing.Point(80, 52);
            this.txtShipName.Name = "txtShipName";
            this.txtShipName.Size = new System.Drawing.Size(200, 19);

            this.lblShipAddress.AutoSize = true;
            this.lblShipAddress.Location = new System.Drawing.Point(300, 55);
            this.lblShipAddress.Name = "lblShipAddress";
            this.lblShipAddress.Size = new System.Drawing.Size(70, 12);
            this.lblShipAddress.Text = "配送先住所:";

            this.txtShipAddress.Location = new System.Drawing.Point(380, 52);
            this.txtShipAddress.Name = "txtShipAddress";
            this.txtShipAddress.Size = new System.Drawing.Size(300, 19);

            // 行3: 市、地域、郵便番号、国
            this.lblShipCity.AutoSize = true;
            this.lblShipCity.Location = new System.Drawing.Point(15, 85);
            this.lblShipCity.Name = "lblShipCity";
            this.lblShipCity.Size = new System.Drawing.Size(60, 12);
            this.lblShipCity.Text = "配送先市:";

            this.txtShipCity.Location = new System.Drawing.Point(80, 82);
            this.txtShipCity.Name = "txtShipCity";
            this.txtShipCity.Size = new System.Drawing.Size(120, 19);

            this.lblShipRegion.AutoSize = true;
            this.lblShipRegion.Location = new System.Drawing.Point(220, 85);
            this.lblShipRegion.Name = "lblShipRegion";
            this.lblShipRegion.Size = new System.Drawing.Size(70, 12);
            this.lblShipRegion.Text = "配送先地域:";

            this.txtShipRegion.Location = new System.Drawing.Point(295, 82);
            this.txtShipRegion.Name = "txtShipRegion";
            this.txtShipRegion.Size = new System.Drawing.Size(120, 19);

            this.lblShipPostalCode.AutoSize = true;
            this.lblShipPostalCode.Location = new System.Drawing.Point(435, 85);
            this.lblShipPostalCode.Name = "lblShipPostalCode";
            this.lblShipPostalCode.Size = new System.Drawing.Size(80, 12);
            this.lblShipPostalCode.Text = "配送先〒:";

            this.txtShipPostalCode.Location = new System.Drawing.Point(520, 82);
            this.txtShipPostalCode.Name = "txtShipPostalCode";
            this.txtShipPostalCode.Size = new System.Drawing.Size(100, 19);

            this.lblShipCountry.AutoSize = true;
            this.lblShipCountry.Location = new System.Drawing.Point(640, 85);
            this.lblShipCountry.Name = "lblShipCountry";
            this.lblShipCountry.Size = new System.Drawing.Size(60, 12);
            this.lblShipCountry.Text = "配送先国:";

            this.txtShipCountry.Location = new System.Drawing.Point(710, 82);
            this.txtShipCountry.Name = "txtShipCountry";
            this.txtShipCountry.Size = new System.Drawing.Size(120, 19);

            // 行4: 運送料
            this.lblFreight.AutoSize = true;
            this.lblFreight.Location = new System.Drawing.Point(15, 115);
            this.lblFreight.Name = "lblFreight";
            this.lblFreight.Size = new System.Drawing.Size(50, 12);
            this.lblFreight.Text = "運送料:";

            this.txtFreight.Location = new System.Drawing.Point(80, 112);
            this.txtFreight.Name = "txtFreight";
            this.txtFreight.Size = new System.Drawing.Size(120, 19);

            // grpShippingInfo にコントロール追加
            this.grpShippingInfo.Controls.Add(this.lblShipper);
            this.grpShippingInfo.Controls.Add(this.cmbShipper);
            this.grpShippingInfo.Controls.Add(this.lblShipperPhone);
            this.grpShippingInfo.Controls.Add(this.lblShipperPhoneValue);
            this.grpShippingInfo.Controls.Add(this.lblShipName);
            this.grpShippingInfo.Controls.Add(this.txtShipName);
            this.grpShippingInfo.Controls.Add(this.lblShipAddress);
            this.grpShippingInfo.Controls.Add(this.txtShipAddress);
            this.grpShippingInfo.Controls.Add(this.lblShipCity);
            this.grpShippingInfo.Controls.Add(this.txtShipCity);
            this.grpShippingInfo.Controls.Add(this.lblShipRegion);
            this.grpShippingInfo.Controls.Add(this.txtShipRegion);
            this.grpShippingInfo.Controls.Add(this.lblShipPostalCode);
            this.grpShippingInfo.Controls.Add(this.txtShipPostalCode);
            this.grpShippingInfo.Controls.Add(this.lblShipCountry);
            this.grpShippingInfo.Controls.Add(this.txtShipCountry);
            this.grpShippingInfo.Controls.Add(this.lblFreight);
            this.grpShippingInfo.Controls.Add(this.txtFreight);

            // ===== アクションボタン =====
            this.btnCreate.Location = new System.Drawing.Point(720, 654);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(95, 30);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "作成";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);

            this.btnUpdate.Location = new System.Drawing.Point(821, 654);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(95, 30);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "更新";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);

            this.btnCancel.Location = new System.Drawing.Point(922, 654);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // ===== Form1 =====
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 696);
            this.Controls.Add(this.grpOrderInfo);
            this.Controls.Add(this.grpOrderDetails);
            this.Controls.Add(this.grpShippingInfo);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCancel);
            this.Name = "Form1";
            this.Text = "受注詳細（編集モード）";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grpOrderInfo.ResumeLayout(false);
            this.grpOrderInfo.PerformLayout();
            this.grpOrderDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetails)).EndInit();
            this.grpShippingInfo.ResumeLayout(false);
            this.grpShippingInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        // 受注情報エリア
        private System.Windows.Forms.GroupBox grpOrderInfo;
        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.TextBox txtOrderId;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblCustomerPhone;
        private System.Windows.Forms.Label lblCustomerPhoneValue;
        private System.Windows.Forms.Label lblCustomerAddress;
        private System.Windows.Forms.Label lblCustomerAddressValue;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.ComboBox cmbEmployee;
        private System.Windows.Forms.Label lblEmployeeTitle;
        private System.Windows.Forms.Label lblEmployeeTitleValue;
        private System.Windows.Forms.Label lblEmployeeExtension;
        private System.Windows.Forms.Label lblEmployeeExtensionValue;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.DateTimePicker dtpOrderDate;
        private System.Windows.Forms.Label lblRequiredDate;
        private System.Windows.Forms.DateTimePicker dtpRequiredDate;
        private System.Windows.Forms.Label lblShippedDate;
        private System.Windows.Forms.DateTimePicker dtpShippedDate;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblTotalAmountValue;

        // 受注明細エリア
        private System.Windows.Forms.GroupBox grpOrderDetails;
        private System.Windows.Forms.DataGridView dgvOrderDetails;
        private System.Windows.Forms.DataGridViewComboBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiscount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineTotal;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.Button btnAddDetail;

        // 配送情報エリア
        private System.Windows.Forms.GroupBox grpShippingInfo;
        private System.Windows.Forms.Label lblShipper;
        private System.Windows.Forms.ComboBox cmbShipper;
        private System.Windows.Forms.Label lblShipperPhone;
        private System.Windows.Forms.Label lblShipperPhoneValue;
        private System.Windows.Forms.Label lblShipName;
        private System.Windows.Forms.TextBox txtShipName;
        private System.Windows.Forms.Label lblShipAddress;
        private System.Windows.Forms.TextBox txtShipAddress;
        private System.Windows.Forms.Label lblShipCity;
        private System.Windows.Forms.TextBox txtShipCity;
        private System.Windows.Forms.Label lblShipRegion;
        private System.Windows.Forms.TextBox txtShipRegion;
        private System.Windows.Forms.Label lblShipPostalCode;
        private System.Windows.Forms.TextBox txtShipPostalCode;
        private System.Windows.Forms.Label lblShipCountry;
        private System.Windows.Forms.TextBox txtShipCountry;
        private System.Windows.Forms.Label lblFreight;
        private System.Windows.Forms.TextBox txtFreight;

        // アクションボタン
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
    }
}

