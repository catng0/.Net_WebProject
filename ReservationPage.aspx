<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReservationPage.aspx.cs" Inherits="WebApplication1.ReservationPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Đặt Bàn</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background-color: #F9C6CF;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        form {
            background-color: #FFFFFF;
            padding: 25px;
            border-radius: 20px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            width: 80%;
            max-width: 600px;
            text-align: center;
        }

        h2, h3 {
            color: #9111A5;
            font-size: 22px;
            font-weight: bold;
        }

        label {
            font-weight: bold;
            color: #9111A5;
            display: block;
            margin-top: 10px;
        }

        select, input[type="datetime-local"] {
            width: 100%;
            padding: 10px;
            border: 2px solid #9111A5;
            border-radius: 10px;
            font-size: 16px;
            outline: none;
        }

        .aspButton {
            background-color: #9111A5;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 10px;
            cursor: pointer;
            font-size: 16px;
            transition: background-color 0.3s ease;
            margin-top: 15px;
        }

        .aspButton:hover {
            background-color: #7A0D8C;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        th, td {
            padding: 10px;
            border: 2px solid #9111A5;
            text-align: center;
        }

        th {
            background-color: #9111A5;
            color: white;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Đặt Bàn</h2>
        <p>Xin chào, <asp:Label ID="lblUsername" runat="server" Text=""></asp:Label></p>

        <label for="ddlTable">Chọn bàn:</label>
        <asp:DropDownList ID="ddlTable" runat="server"></asp:DropDownList>

        <label for="txtDateTime">Chọn ngày giờ:</label>
        <asp:TextBox ID="txtDateTime" runat="server" TextMode="DateTimeLocal"></asp:TextBox>

        <asp:Button ID="btnReserve" runat="server" Text="Đặt Bàn" CssClass="aspButton" OnClick="btnReserve_Click" />

        <h3>Danh sách đặt bàn của bạn</h3>
        <asp:GridView ID="GridViewReservations" runat="server" AutoGenerateColumns="False" OnRowCommand="GridViewReservations_RowCommand">
    <Columns>
        <asp:BoundField DataField="ReservationID" HeaderText="ID" />
        <asp:BoundField DataField="TableID" HeaderText="Bàn" />
        <asp:BoundField DataField="DateTime" HeaderText="Thời gian" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
        <asp:TemplateField HeaderText="Hủy">
            <ItemTemplate>
                <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="aspButton" CommandName="CancelReservation" CommandArgument='<%# Eval("ReservationID") %>' OnClientClick="return confirm('Bạn có chắc muốn hủy đặt bàn này?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

    </form>
</body>
</html>
