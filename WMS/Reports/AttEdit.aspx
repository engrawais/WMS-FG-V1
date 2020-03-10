<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AttEdit.aspx.cs" Inherits="WMS.Reports.AttEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script src="../Scripts/modernizr-2.6.2.js"></script>
    <script src="../Scripts/jquery-2.1.1.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="col-md-5">
        <div class="col-md-6">
            <asp:Label ID="Label1" runat="server" Text="P.No"></asp:Label>
        </div>
        <div class="col-md-6">
            <asp:TextBox ID="tbEmpNo" runat="server"></asp:TextBox>
        </div>
        <div class="col-md-6">
            <asp:Label ID="Label4" runat="server" Text="Date"></asp:Label>
        </div>
        <div class="col-md-6">
            <asp:TextBox ID="tbDate" runat="server"></asp:TextBox>
        </div>
        <div class="col-md-6">
            <asp:Label ID="Label2" runat="server" Text="Time In"></asp:Label>
        </div>
        <div class="col-md-6">
             <asp:TextBox ID="tbTimeIn" runat="server"></asp:TextBox>
        </div>
        <div class="col-md-6">
            <asp:Label ID="Label5" runat="server" Text="Time Out"></asp:Label>
        </div>
        <div class="col-md-6">
             <asp:TextBox ID="tbTimeOut" runat="server"></asp:TextBox>
        </div>
    </section>
    <section class="col-md-7">

    </section>
   
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click1" Text="Button" />
&nbsp;

</asp:Content>
