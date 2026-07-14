namespace NorthwindWinForms
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
            this.grpOrder = new System.Windows.Forms.GroupBox();
            this.lblOrderId = new System.Windows.Forms.Label();
            this.txtOrderId = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblTotalAmountValue = new System.Windows.Forms.Label();
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
            this.grpDetails = new System.Windows.Forms.GroupBox();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.colProduct = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiscount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnAddDetail = new System.Windows.Forms.Button();
            this.grpShipping = new System.Windows.Forms.GroupBox();
            this.lblShipper = new System.Windows.Forms.Label();
            this.cmbShipper = new System.Windows.Forms.ComboBox();
            this.lblShipperPhone = new System.Windows.Forms.Label();
            this.lblShipperPhoneValue = new System.Windows.Forms.Label();
            this.lblShipName = new System.Windows.Forms.Label();
            this.txtShipName = new System.Windows.Forms.TextBox();
            this.lblFreight = new System.Windows.Forms.Label();
            this.txtFreight = new System.Windows.Forms.TextBox();
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
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpOrder.SuspendLayout();
            this.grpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.grpShipping.SuspendLayout();
            this.SuspendLayout();
            //
            // grpOrder
            //
            this.grpOrder.Controls.Add(this.lblOrderId);
            this.grpOrder.Controls.Add(this.txtOrderId);
            this.grpOrder.Controls.Add(this.btnLoad);
            this.grpOrder.Controls.Add(this.lblTotalAmount);
            this.grpOrder.Controls.Add(this.lblTotalAmountValue);
            this.grpOrder.Controls.Add(this.lblCustomer);
            this.grpOrder.Controls.Add(this.cmbCustomer);
            this.grpOrder.Controls.Add(this.lblCustomerPhone);
            this.grpOrder.Controls.Add(this.lblCustomerPhoneValue);
            this.grpOrder.Controls.Add(this.lblCustomerAddress);
            this.grpOrder.Controls.Add(this.lblCustomerAddressValue);
            this.grpOrder.Controls.Add(this.lblEmployee);
            this.grpOrder.Controls.Add(this.cmbEmployee);
            this.grpOrder.Controls.Add(this.lblEmployeeTitle);
            this.grpOrder.Controls.Add(this.lblEmployeeTitleValue);
            this.grpOrder.Controls.Add(this.lblEmployeeExtension);
            this.grpOrder.Controls.Add(this.lblEmployeeExtensionValue);
            this.grpOrder.Controls.Add(this.lblOrderDate);
            this.grpOrder.Controls.Add(this.dtpOrderDate);
            this.grpOrder.Controls.Add(this.lblRequiredDate);
            this.grpOrder.Controls.Add(this.dtpRequiredDate);
            this.grpOrder.Controls.Add(this.lblShippedDate);
            this.grpOrder.Controls.Add(this.dtpShippedDate);
            this.grpOrder.Location = new System.Drawing.Point(12, 12);
            this.grpOrder.Name = "grpOrder";
            this.grpOrder.Size = new System.Drawing.Size(910, 185);
            this.grpOrder.TabIndex = 0;
            this.grpOrder.TabStop = false;
            this.grpOrder.Text = "受注情報";
            //
            // lblOrderId
            //
            this.lblOrderId.AutoSize = true;
            this.lblOrderId.Location = new System.Drawing.Point(15, 31);
            this.lblOrderId.Name = "lblOrderId";
            this.lblOrderId.Size = new System.Drawing.Size(52, 16);
            this.lblOrderId.TabIndex = 0;
            this.lblOrderId.Text = "受注ID";
            //
            // txtOrderId
            //
            this.txtOrderId.Location = new System.Drawing.Point(90, 28);
            this.txtOrderId.Name = "txtOrderId";
            this.txtOrderId.Size = new System.Drawing.Size(100, 23);
            this.txtOrderId.TabIndex = 1;
            //
            // btnLoad
            //
            this.btnLoad.Location = new System.Drawing.Point(200, 27);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(70, 25);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "読込";
            this.btnLoad.UseVisualStyleBackColor = true;
            //
            // lblTotalAmount
            //
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Location = new System.Drawing.Point(620, 31);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(68, 16);
            this.lblTotalAmount.TabIndex = 3;
            this.lblTotalAmount.Text = "合計金額:";
            //
            // lblTotalAmountValue
            //
            this.lblTotalAmountValue.AutoSize = true;
            this.lblTotalAmountValue.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmountValue.Location = new System.Drawing.Point(700, 31);
            this.lblTotalAmountValue.Name = "lblTotalAmountValue";
            this.lblTotalAmountValue.Size = new System.Drawing.Size(30, 16);
            this.lblTotalAmountValue.TabIndex = 4;
            this.lblTotalAmountValue.Text = "0.00";
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(15, 66);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(36, 16);
            this.lblCustomer.TabIndex = 5;
            this.lblCustomer.Text = "顧客";
            //
            // cmbCustomer
            //
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(90, 63);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(250, 24);
            this.cmbCustomer.TabIndex = 6;
            //
            // lblCustomerPhone
            //
            this.lblCustomerPhone.AutoSize = true;
            this.lblCustomerPhone.Location = new System.Drawing.Point(360, 66);
            this.lblCustomerPhone.Name = "lblCustomerPhone";
            this.lblCustomerPhone.Size = new System.Drawing.Size(44, 16);
            this.lblCustomerPhone.TabIndex = 7;
            this.lblCustomerPhone.Text = "電話:";
            //
            // lblCustomerPhoneValue
            //
            this.lblCustomerPhoneValue.AutoSize = true;
            this.lblCustomerPhoneValue.Location = new System.Drawing.Point(410, 66);
            this.lblCustomerPhoneValue.Name = "lblCustomerPhoneValue";
            this.lblCustomerPhoneValue.Size = new System.Drawing.Size(0, 16);
            this.lblCustomerPhoneValue.TabIndex = 8;
            //
            // lblCustomerAddress
            //
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.Location = new System.Drawing.Point(540, 66);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(44, 16);
            this.lblCustomerAddress.TabIndex = 9;
            this.lblCustomerAddress.Text = "住所:";
            //
            // lblCustomerAddressValue
            //
            this.lblCustomerAddressValue.AutoSize = true;
            this.lblCustomerAddressValue.Location = new System.Drawing.Point(590, 66);
            this.lblCustomerAddressValue.Name = "lblCustomerAddressValue";
            this.lblCustomerAddressValue.Size = new System.Drawing.Size(0, 16);
            this.lblCustomerAddressValue.TabIndex = 10;
            //
            // lblEmployee
            //
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(15, 101);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(52, 16);
            this.lblEmployee.TabIndex = 11;
            this.lblEmployee.Text = "従業員";
            //
            // cmbEmployee
            //
            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.FormattingEnabled = true;
            this.cmbEmployee.Location = new System.Drawing.Point(90, 98);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(250, 24);
            this.cmbEmployee.TabIndex = 12;
            //
            // lblEmployeeTitle
            //
            this.lblEmployeeTitle.AutoSize = true;
            this.lblEmployeeTitle.Location = new System.Drawing.Point(360, 101);
            this.lblEmployeeTitle.Name = "lblEmployeeTitle";
            this.lblEmployeeTitle.Size = new System.Drawing.Size(44, 16);
            this.lblEmployeeTitle.TabIndex = 13;
            this.lblEmployeeTitle.Text = "職位:";
            //
            // lblEmployeeTitleValue
            //
            this.lblEmployeeTitleValue.AutoSize = true;
            this.lblEmployeeTitleValue.Location = new System.Drawing.Point(410, 101);
            this.lblEmployeeTitleValue.Name = "lblEmployeeTitleValue";
            this.lblEmployeeTitleValue.Size = new System.Drawing.Size(0, 16);
            this.lblEmployeeTitleValue.TabIndex = 14;
            //
            // lblEmployeeExtension
            //
            this.lblEmployeeExtension.AutoSize = true;
            this.lblEmployeeExtension.Location = new System.Drawing.Point(540, 101);
            this.lblEmployeeExtension.Name = "lblEmployeeExtension";
            this.lblEmployeeExtension.Size = new System.Drawing.Size(44, 16);
            this.lblEmployeeExtension.TabIndex = 15;
            this.lblEmployeeExtension.Text = "内線:";
            //
            // lblEmployeeExtensionValue
            //
            this.lblEmployeeExtensionValue.AutoSize = true;
            this.lblEmployeeExtensionValue.Location = new System.Drawing.Point(590, 101);
            this.lblEmployeeExtensionValue.Name = "lblEmployeeExtensionValue";
            this.lblEmployeeExtensionValue.Size = new System.Drawing.Size(0, 16);
            this.lblEmployeeExtensionValue.TabIndex = 16;
            //
            // lblOrderDate
            //
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(15, 141);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(52, 16);
            this.lblOrderDate.TabIndex = 17;
            this.lblOrderDate.Text = "受注日";
            //
            // dtpOrderDate
            //
            this.dtpOrderDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOrderDate.Location = new System.Drawing.Point(90, 137);
            this.dtpOrderDate.Name = "dtpOrderDate";
            this.dtpOrderDate.ShowCheckBox = true;
            this.dtpOrderDate.Size = new System.Drawing.Size(140, 23);
            this.dtpOrderDate.TabIndex = 18;
            //
            // lblRequiredDate
            //
            this.lblRequiredDate.AutoSize = true;
            this.lblRequiredDate.Location = new System.Drawing.Point(260, 141);
            this.lblRequiredDate.Name = "lblRequiredDate";
            this.lblRequiredDate.Size = new System.Drawing.Size(52, 16);
            this.lblRequiredDate.TabIndex = 19;
            this.lblRequiredDate.Text = "必要日";
            //
            // dtpRequiredDate
            //
            this.dtpRequiredDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRequiredDate.Location = new System.Drawing.Point(335, 137);
            this.dtpRequiredDate.Name = "dtpRequiredDate";
            this.dtpRequiredDate.ShowCheckBox = true;
            this.dtpRequiredDate.Size = new System.Drawing.Size(140, 23);
            this.dtpRequiredDate.TabIndex = 20;
            //
            // lblShippedDate
            //
            this.lblShippedDate.AutoSize = true;
            this.lblShippedDate.Location = new System.Drawing.Point(505, 141);
            this.lblShippedDate.Name = "lblShippedDate";
            this.lblShippedDate.Size = new System.Drawing.Size(52, 16);
            this.lblShippedDate.TabIndex = 21;
            this.lblShippedDate.Text = "出荷日";
            //
            // dtpShippedDate
            //
            this.dtpShippedDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpShippedDate.Location = new System.Drawing.Point(580, 137);
            this.dtpShippedDate.Name = "dtpShippedDate";
            this.dtpShippedDate.ShowCheckBox = true;
            this.dtpShippedDate.Size = new System.Drawing.Size(140, 23);
            this.dtpShippedDate.TabIndex = 22;
            //
            // grpDetails
            //
            this.grpDetails.Controls.Add(this.dgvDetails);
            this.grpDetails.Controls.Add(this.btnAddDetail);
            this.grpDetails.Location = new System.Drawing.Point(12, 203);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(910, 210);
            this.grpDetails.TabIndex = 1;
            this.grpDetails.TabStop = false;
            this.grpDetails.Text = "受注明細";
            //
            // dgvDetails
            //
            this.dgvDetails.AllowUserToAddRows = false;
            this.dgvDetails.AllowUserToDeleteRows = false;
            this.dgvDetails.AutoGenerateColumns = false;
            this.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProduct,
            this.colUnitPrice,
            this.colQuantity,
            this.colDiscount,
            this.colLineTotal,
            this.colDelete});
            this.dgvDetails.Location = new System.Drawing.Point(15, 25);
            this.dgvDetails.Name = "dgvDetails";
            this.dgvDetails.RowHeadersWidth = 25;
            this.dgvDetails.RowTemplate.Height = 21;
            this.dgvDetails.Size = new System.Drawing.Size(880, 140);
            this.dgvDetails.TabIndex = 0;
            //
            // colProduct
            //
            this.colProduct.DataPropertyName = "ProductID";
            this.colProduct.HeaderText = "商品";
            this.colProduct.Name = "colProduct";
            this.colProduct.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colProduct.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colProduct.Width = 250;
            //
            // colUnitPrice
            //
            this.colUnitPrice.DataPropertyName = "UnitPrice";
            this.colUnitPrice.HeaderText = "単価";
            this.colUnitPrice.Name = "colUnitPrice";
            this.colUnitPrice.Width = 90;
            //
            // colQuantity
            //
            this.colQuantity.DataPropertyName = "Quantity";
            this.colQuantity.HeaderText = "数量";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.Width = 70;
            //
            // colDiscount
            //
            this.colDiscount.DataPropertyName = "Discount";
            this.colDiscount.HeaderText = "割引";
            this.colDiscount.Name = "colDiscount";
            this.colDiscount.Width = 70;
            //
            // colLineTotal
            //
            this.colLineTotal.HeaderText = "合計金額";
            this.colLineTotal.Name = "colLineTotal";
            this.colLineTotal.ReadOnly = true;
            this.colLineTotal.Width = 110;
            //
            // colDelete
            //
            this.colDelete.HeaderText = "削除";
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "削除";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.Width = 70;
            //
            // btnAddDetail
            //
            this.btnAddDetail.Location = new System.Drawing.Point(15, 173);
            this.btnAddDetail.Name = "btnAddDetail";
            this.btnAddDetail.Size = new System.Drawing.Size(100, 28);
            this.btnAddDetail.TabIndex = 1;
            this.btnAddDetail.Text = "明細追加";
            this.btnAddDetail.UseVisualStyleBackColor = true;
            //
            // grpShipping
            //
            this.grpShipping.Controls.Add(this.lblShipper);
            this.grpShipping.Controls.Add(this.cmbShipper);
            this.grpShipping.Controls.Add(this.lblShipperPhone);
            this.grpShipping.Controls.Add(this.lblShipperPhoneValue);
            this.grpShipping.Controls.Add(this.lblShipName);
            this.grpShipping.Controls.Add(this.txtShipName);
            this.grpShipping.Controls.Add(this.lblFreight);
            this.grpShipping.Controls.Add(this.txtFreight);
            this.grpShipping.Controls.Add(this.lblShipAddress);
            this.grpShipping.Controls.Add(this.txtShipAddress);
            this.grpShipping.Controls.Add(this.lblShipCity);
            this.grpShipping.Controls.Add(this.txtShipCity);
            this.grpShipping.Controls.Add(this.lblShipRegion);
            this.grpShipping.Controls.Add(this.txtShipRegion);
            this.grpShipping.Controls.Add(this.lblShipPostalCode);
            this.grpShipping.Controls.Add(this.txtShipPostalCode);
            this.grpShipping.Controls.Add(this.lblShipCountry);
            this.grpShipping.Controls.Add(this.txtShipCountry);
            this.grpShipping.Location = new System.Drawing.Point(12, 419);
            this.grpShipping.Name = "grpShipping";
            this.grpShipping.Size = new System.Drawing.Size(910, 175);
            this.grpShipping.TabIndex = 2;
            this.grpShipping.TabStop = false;
            this.grpShipping.Text = "配送情報";
            //
            // lblShipper
            //
            this.lblShipper.AutoSize = true;
            this.lblShipper.Location = new System.Drawing.Point(15, 31);
            this.lblShipper.Name = "lblShipper";
            this.lblShipper.Size = new System.Drawing.Size(68, 16);
            this.lblShipper.TabIndex = 0;
            this.lblShipper.Text = "配送業者";
            //
            // cmbShipper
            //
            this.cmbShipper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShipper.FormattingEnabled = true;
            this.cmbShipper.Location = new System.Drawing.Point(100, 28);
            this.cmbShipper.Name = "cmbShipper";
            this.cmbShipper.Size = new System.Drawing.Size(250, 24);
            this.cmbShipper.TabIndex = 1;
            //
            // lblShipperPhone
            //
            this.lblShipperPhone.AutoSize = true;
            this.lblShipperPhone.Location = new System.Drawing.Point(370, 31);
            this.lblShipperPhone.Name = "lblShipperPhone";
            this.lblShipperPhone.Size = new System.Drawing.Size(44, 16);
            this.lblShipperPhone.TabIndex = 2;
            this.lblShipperPhone.Text = "電話:";
            //
            // lblShipperPhoneValue
            //
            this.lblShipperPhoneValue.AutoSize = true;
            this.lblShipperPhoneValue.Location = new System.Drawing.Point(420, 31);
            this.lblShipperPhoneValue.Name = "lblShipperPhoneValue";
            this.lblShipperPhoneValue.Size = new System.Drawing.Size(0, 16);
            this.lblShipperPhoneValue.TabIndex = 3;
            //
            // lblShipName
            //
            this.lblShipName.AutoSize = true;
            this.lblShipName.Location = new System.Drawing.Point(15, 66);
            this.lblShipName.Name = "lblShipName";
            this.lblShipName.Size = new System.Drawing.Size(68, 16);
            this.lblShipName.TabIndex = 4;
            this.lblShipName.Text = "配送先名";
            //
            // txtShipName
            //
            this.txtShipName.Location = new System.Drawing.Point(100, 63);
            this.txtShipName.Name = "txtShipName";
            this.txtShipName.Size = new System.Drawing.Size(250, 23);
            this.txtShipName.TabIndex = 5;
            //
            // lblFreight
            //
            this.lblFreight.AutoSize = true;
            this.lblFreight.Location = new System.Drawing.Point(600, 66);
            this.lblFreight.Name = "lblFreight";
            this.lblFreight.Size = new System.Drawing.Size(52, 16);
            this.lblFreight.TabIndex = 6;
            this.lblFreight.Text = "運送料";
            //
            // txtFreight
            //
            this.txtFreight.Location = new System.Drawing.Point(680, 63);
            this.txtFreight.Name = "txtFreight";
            this.txtFreight.Size = new System.Drawing.Size(100, 23);
            this.txtFreight.TabIndex = 7;
            //
            // lblShipAddress
            //
            this.lblShipAddress.AutoSize = true;
            this.lblShipAddress.Location = new System.Drawing.Point(15, 101);
            this.lblShipAddress.Name = "lblShipAddress";
            this.lblShipAddress.Size = new System.Drawing.Size(84, 16);
            this.lblShipAddress.TabIndex = 8;
            this.lblShipAddress.Text = "配送先住所";
            //
            // txtShipAddress
            //
            this.txtShipAddress.Location = new System.Drawing.Point(100, 98);
            this.txtShipAddress.Name = "txtShipAddress";
            this.txtShipAddress.Size = new System.Drawing.Size(250, 23);
            this.txtShipAddress.TabIndex = 9;
            //
            // lblShipCity
            //
            this.lblShipCity.AutoSize = true;
            this.lblShipCity.Location = new System.Drawing.Point(370, 101);
            this.lblShipCity.Name = "lblShipCity";
            this.lblShipCity.Size = new System.Drawing.Size(68, 16);
            this.lblShipCity.TabIndex = 10;
            this.lblShipCity.Text = "配送先市";
            //
            // txtShipCity
            //
            this.txtShipCity.Location = new System.Drawing.Point(460, 98);
            this.txtShipCity.Name = "txtShipCity";
            this.txtShipCity.Size = new System.Drawing.Size(150, 23);
            this.txtShipCity.TabIndex = 11;
            //
            // lblShipRegion
            //
            this.lblShipRegion.AutoSize = true;
            this.lblShipRegion.Location = new System.Drawing.Point(15, 136);
            this.lblShipRegion.Name = "lblShipRegion";
            this.lblShipRegion.Size = new System.Drawing.Size(84, 16);
            this.lblShipRegion.TabIndex = 12;
            this.lblShipRegion.Text = "配送先地域";
            //
            // txtShipRegion
            //
            this.txtShipRegion.Location = new System.Drawing.Point(100, 133);
            this.txtShipRegion.Name = "txtShipRegion";
            this.txtShipRegion.Size = new System.Drawing.Size(150, 23);
            this.txtShipRegion.TabIndex = 13;
            //
            // lblShipPostalCode
            //
            this.lblShipPostalCode.AutoSize = true;
            this.lblShipPostalCode.Location = new System.Drawing.Point(270, 136);
            this.lblShipPostalCode.Name = "lblShipPostalCode";
            this.lblShipPostalCode.Size = new System.Drawing.Size(68, 16);
            this.lblShipPostalCode.TabIndex = 14;
            this.lblShipPostalCode.Text = "郵便番号";
            //
            // txtShipPostalCode
            //
            this.txtShipPostalCode.Location = new System.Drawing.Point(360, 133);
            this.txtShipPostalCode.Name = "txtShipPostalCode";
            this.txtShipPostalCode.Size = new System.Drawing.Size(120, 23);
            this.txtShipPostalCode.TabIndex = 15;
            //
            // lblShipCountry
            //
            this.lblShipCountry.AutoSize = true;
            this.lblShipCountry.Location = new System.Drawing.Point(500, 136);
            this.lblShipCountry.Name = "lblShipCountry";
            this.lblShipCountry.Size = new System.Drawing.Size(68, 16);
            this.lblShipCountry.TabIndex = 16;
            this.lblShipCountry.Text = "配送先国";
            //
            // txtShipCountry
            //
            this.txtShipCountry.Location = new System.Drawing.Point(580, 133);
            this.txtShipCountry.Name = "txtShipCountry";
            this.txtShipCountry.Size = new System.Drawing.Size(150, 23);
            this.txtShipCountry.TabIndex = 17;
            //
            // btnCreate
            //
            this.btnCreate.Location = new System.Drawing.Point(600, 610);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(90, 32);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "作成";
            this.btnCreate.UseVisualStyleBackColor = true;
            //
            // btnUpdate
            //
            this.btnUpdate.Location = new System.Drawing.Point(700, 610);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(90, 32);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "更新";
            this.btnUpdate.UseVisualStyleBackColor = true;
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(800, 610);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 660);
            this.Controls.Add(this.grpOrder);
            this.Controls.Add(this.grpDetails);
            this.Controls.Add(this.grpShipping);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCancel);
            this.Name = "Form1";
            this.Text = "受注詳細画面（編集モード）";
            this.grpOrder.ResumeLayout(false);
            this.grpOrder.PerformLayout();
            this.grpDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.grpShipping.ResumeLayout(false);
            this.grpShipping.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpOrder;
        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.TextBox txtOrderId;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblTotalAmountValue;
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
        private System.Windows.Forms.GroupBox grpDetails;
        private System.Windows.Forms.DataGridView dgvDetails;
        private System.Windows.Forms.DataGridViewComboBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiscount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineTotal;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.Button btnAddDetail;
        private System.Windows.Forms.GroupBox grpShipping;
        private System.Windows.Forms.Label lblShipper;
        private System.Windows.Forms.ComboBox cmbShipper;
        private System.Windows.Forms.Label lblShipperPhone;
        private System.Windows.Forms.Label lblShipperPhoneValue;
        private System.Windows.Forms.Label lblShipName;
        private System.Windows.Forms.TextBox txtShipName;
        private System.Windows.Forms.Label lblFreight;
        private System.Windows.Forms.TextBox txtFreight;
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
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
    }
}
