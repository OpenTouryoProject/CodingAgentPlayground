<%@ Page Title="受注詳細（編集）" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Form1.aspx.cs" Inherits="WebApplication1.Form1" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <h2>受注詳細画面（編集モード）</h2>

        <%-- ========== 受注情報エリア ========== --%>
        <div class="card mb-3">
            <div class="card-header">受注情報</div>
            <div class="card-body">
                <div class="row mb-2">
                    <div class="col-md-3">
                        <label class="form-label">受注ID</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtOrderID" runat="server" CssClass="form-control" />
                            <asp:Button ID="btnLoad" runat="server" Text="読み込み" CssClass="btn btn-outline-primary" OnClick="btnLoad_Click" />
                        </div>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">受注日</label>
                        <asp:TextBox ID="txtOrderDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">必要日</label>
                        <asp:TextBox ID="txtRequiredDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">出荷日</label>
                        <asp:TextBox ID="txtShippedDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-md-4">
                        <label class="form-label">顧客</label>
                        <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">電話番号</label>
                        <asp:Label ID="lblCustomerPhone" runat="server" CssClass="form-control-plaintext" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">住所</label>
                        <asp:Label ID="lblCustomerAddress" runat="server" CssClass="form-control-plaintext" />
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-md-4">
                        <label class="form-label">担当従業員</label>
                        <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">職位</label>
                        <asp:Label ID="lblEmployeeTitle" runat="server" CssClass="form-control-plaintext" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">内線番号</label>
                        <asp:Label ID="lblEmployeeExtension" runat="server" CssClass="form-control-plaintext" />
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-md-3">
                        <label class="form-label">合計金額</label>
                        <asp:Label ID="lblTotalAmount" runat="server" CssClass="form-control-plaintext fw-bold" />
                    </div>
                </div>
            </div>
        </div>

        <%-- ========== 受注明細エリア ========== --%>
        <div class="card mb-3">
            <div class="card-header">受注明細</div>
            <div class="card-body">
                <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm"
                    OnRowCommand="gvDetails_RowCommand" OnRowDataBound="gvDetails_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="商品">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlProduct" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="単価">
                            <ItemTemplate>
                                <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("UnitPrice", "{0:F2}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="数量">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Quantity") %>' AutoPostBack="true" OnTextChanged="txtQuantity_TextChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="割引">
                            <ItemTemplate>
                                <asp:TextBox ID="txtDiscount" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Discount", "{0:F2}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="合計">
                            <ItemTemplate>
                                <asp:Label ID="lblLineTotal" runat="server" Text='<%# Eval("LineTotal", "{0:F2}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:Button ID="btnDeleteRow" runat="server" Text="削除" CssClass="btn btn-danger btn-sm"
                                    CommandName="DeleteRow" CommandArgument='<%# Container.DataItemIndex %>'
                                    OnClientClick="return confirm('この明細行を削除しますか？');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <p class="text-muted">明細がありません。</p>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:Button ID="btnAddDetail" runat="server" Text="明細追加" CssClass="btn btn-secondary" OnClick="btnAddDetail_Click" />
            </div>
        </div>

        <%-- ========== 配送情報エリア ========== --%>
        <div class="card mb-3">
            <div class="card-header">配送情報</div>
            <div class="card-body">
                <div class="row mb-2">
                    <div class="col-md-4">
                        <label class="form-label">配送業者</label>
                        <asp:DropDownList ID="ddlShipper" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlShipper_SelectedIndexChanged" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">配送業者電話番号</label>
                        <asp:Label ID="lblShipperPhone" runat="server" CssClass="form-control-plaintext" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">運送料</label>
                        <asp:TextBox ID="txtFreight" runat="server" CssClass="form-control" />
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-md-4">
                        <label class="form-label">配送先名</label>
                        <asp:TextBox ID="txtShipName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">配送先住所</label>
                        <asp:TextBox ID="txtShipAddress" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">配送先市</label>
                        <asp:TextBox ID="txtShipCity" runat="server" CssClass="form-control" />
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-md-3">
                        <label class="form-label">配送先地域</label>
                        <asp:TextBox ID="txtShipRegion" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">配送先郵便番号</label>
                        <asp:TextBox ID="txtShipPostalCode" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">配送先国</label>
                        <asp:TextBox ID="txtShipCountry" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>
        </div>

        <%-- ========== アクションボタン ========== --%>
        <div class="mb-3">
            <asp:Button ID="btnCreate" runat="server" Text="作成" CssClass="btn btn-success" OnClick="btnCreate_Click" />
            <asp:Button ID="btnUpdate" runat="server" Text="更新" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="キャンセル" CssClass="btn btn-outline-secondary" OnClick="btnCancel_Click"
                OnClientClick="return confirm('変更内容を破棄しますか？');" CausesValidation="false" />
        </div>

    </main>
</asp:Content>
