<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Debug="true" %>

<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>FastReport.Mono Demo</title>
</head>
<body style="background-image: url('images/background.jpg'); background-repeat: no-repeat">
    <form id="form1" runat="server">
    <div>
        <table style="width: 795px">
            <tr>
                <td style="height: 44px; text-align: center; width: 106px;">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/demo_logo.png" EnableViewState="False" /></td>
                <td style="height: 44px; width: 768px;">
                    <span style="font-size: 16pt; font-family: Tahoma"><strong>FastReport.Mono Demo<br />
                        <asp:Label ID="Version" runat="server" Font-Size="Small" Text="ver. 1.0.0" Width="188px" EnableViewState="False"></asp:Label></strong></span></td>
            </tr>
        </table>        
        <hr />
    </div>
                <table style="width: 941px">
                    <tr>
                        <td style="width: 641px; height: 826px;" valign="top">
                                <span style="font-size: 4pt">&nbsp;&nbsp; </span>
                                <br />
                                <cc1:WebReport ID="WebReport1" runat="server" BackColor="White" Font-Bold="False"
                                    Height="700px"
                                    Width="500px" Zoom="0.65" OnStartReport="WebReport1_StartReport" ToolbarColor="Lavender"
                                    PrintInPdf="True"
                                    PdfEmbeddingFonts="True"
                                    Layers="False"
                                    />                            
                        </td>
                    </tr>
                </table>
                <br />
    </form>
</body>
</html>
