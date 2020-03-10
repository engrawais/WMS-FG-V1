<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="WMS.LogIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <!-- Basic Page Needs
  ================================================== -->
    <meta charset="utf-8"/>
    <title>Flat Login</title>
    <meta name="description" content=""/>
    <meta name="author" content=""/>

    <!-- Mobile Specific Metas
  ================================================== -->
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"/>
    <link rel="stylesheet" href="Content/LogIn.css" media="screen" type="text/css" />
    <script src="Scripts/jquery-2.1.1.js"></script>
    <script src="Scripts/LogIn.js"></script>
    <!-- CSS
  ================================================== -->

    <!--[if lt IE 9]>
		<script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
	<![endif]-->
</head>
<body>


    <div class="container">
        <div class="flat-form">
            <ul class="tabs">
                <li style="padding-top:25px;">
                    <span style="font-size:27px; margin-left:20px;">WorkForce Management System</span> 
                </li>
            </ul>
            <div id="login" class="form-action show" style="padding-top:25px">
                <h2>Login</h2>
                <form runat="server">
                            <asp:TextBox ID="tbUserName" runat="server"></asp:TextBox>
&nbsp;<asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
&nbsp;<asp:Button ID="btnlogin" runat="server" Text="Login" OnClick="btnlogin_Click" CssClass="button"/>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
