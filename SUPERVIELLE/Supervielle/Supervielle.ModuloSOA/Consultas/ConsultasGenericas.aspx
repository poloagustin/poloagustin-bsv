<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsultasGenericas.aspx.cs" Inherits="ModuloSoa.WebForm1" %>

<%@ Register Assembly="DevExpress.Dashboard.v14.1.Web, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPivotGrid.Export" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v14.1, Version=14.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #Button1 {
            width: 91px;
        }

        #btnGenerar {
            width: 111px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table align="center" style="height: auto; margin-top: 15px;">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lab_title_consulta" runat="server" Text="Consulta..." Font-Names="Calibri" Font-Size="X-Large" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
                    
                
                <tr style="align-content: center;">
                    <td style="width: 100%;">
                        <table style="height: auto; margin-top: 15px;">
                            <tr>
                                <td>
                                    <asp:Panel ID="panelGrid" runat="server" Visible="true" Height="100%" Width="1200px">
                                        <dx:ASPxGridView ID="Grid" ClientInstanceName="Grid" runat="server" AutoGenerateColumns="False" Width="100%" EnableTheming="True" Theme="MetropolisBlue">
                                            <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
                                        </dx:ASPxGridView>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <table align="right" style="height: auto; margin-top: 15px;" bgcolor="White">
                                <tr>
                                    <td align="right" style="width: 100%;">
                                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Home.aspx">Regresar a la pantalla principal</asp:HyperLink>
                                    </td>
                                </tr>
                            </table>
                                </tr>
                            </table>
                    </td>
                </tr>
            </table>
        </div>


    </form>
</body>
</html>
