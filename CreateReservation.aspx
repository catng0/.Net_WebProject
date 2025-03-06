<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateReservation.aspx.cs" Inherits="restaurant.CreateReservation" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tạo đặt bàn mới</title>
    <style>
        body {
            background-color: rgb(249, 198, 207);
            font-family: Arial, sans-serif;
            text-align: center;
        }
        .container {
            width: 50%;
            margin: auto;
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        h2 {
            color: rgb(145, 17, 165);
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
        }
        .form-group input {
            width: 100%;
            padding: 8px;
            border-radius: 5px;
            border: 1px solid #ddd;
        }
        .btn {
            padding: 10px 15px;
            border: none;
            cursor: pointer;
            color: white;
            border-radius: 5px;
            text-decoration: none;
        }
        .btn-submit {
            background-color: green;
        }
        .btn-cancel {
            background-color: red;
        }
    </style>
</head>
<body>
    <div class="container">
        <h2>Tạo đặt bàn mới</h2>
        <form id="form1" runat="server">
            <div class="form-group">
                <label for="txtUserID">User ID</label>
                <asp:TextBox ID="txtUserID" runat="server"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="txtTableID">Table ID</label>
                <asp:TextBox ID="txtTableID" runat="server"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="txtDateTime">Thời gian đến</label>
                <asp:TextBox ID="txtDateTime" runat="server" TextMode="DateTimeLocal"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-submit" Text="Tạo đặt bàn" OnClick="btnSubmit_Click" />
                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-cancel" Text="Hủy" OnClick="btnCancel_Click" />
            </div>
        </form>
    </div>
</body>
</html>