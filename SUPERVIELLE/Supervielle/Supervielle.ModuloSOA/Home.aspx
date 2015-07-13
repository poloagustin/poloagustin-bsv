<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="ModuloSoa.Home" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxRoundPanel" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPanel" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="align-content:center">
            <tr style="align-content:center">
                <td><dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Elige la acción a realizar:"></dx:ASPxLabel></td>
                <td style="padding-left:30px"><asp:DropDownList runat="server" AutoPostBack ="true" EnableTheming="True" Height="20px" Width="295px" ID="ddlConsultas" OnSelectedIndexChanged="ddlConsultas_SelectedIndexChanged" OnTextChanged="ddlConsultas_SelectedIndexChanged" >
        </asp:DropDownList></td>
            </tr>
        </table>      
    </div>
    </form>
</body>
</html>
