<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Create.aspx.cs" Inherits="_Create" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Short URL</title>
    
    <style type="text/css">
        body                    {margin:0; padding:0; background-color:#EBE7E0; font-family: Arial; text-align:center;}
        #container div         {padding:5px; text-align:center; margin-bottom:5px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    
    <div id="container">
    
<br/>    
        <div>
            <asp:TextBox ID="txtRealUrl" runat="server" width="400"/>
            <asp:Button ID="btnSubmit" runat="server" Text="Create Short URL" OnClick="GenerateShortUrl" /><br/>
            Your shortened URL is :
            <asp:HyperLink ID="lnkShortUrl" runat="server" Font-Bold="true" OnClick="RedirectShortUrl"/>
        </div>
    
    </div>
    
    </form>
</body>
</html>
