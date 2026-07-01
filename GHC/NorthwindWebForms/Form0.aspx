<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Form0.aspx.cs" Inherits="WebApplication1.Form0" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <h2>ほげ</h2>
        <div class="row">
            <div class="col-md-6">
                <asp:Button ID="btnTest" runat="server" Text="テストボタン" OnClick="btnTest_Click" CssClass="btn btn-primary" />
                <br /><br />
                <asp:Label ID="lblResult" runat="server" Text="" />
            </div>
        </div>
    </main>
</asp:Content>
