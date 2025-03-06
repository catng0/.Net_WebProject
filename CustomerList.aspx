<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerList.aspx.cs" Inherits="WebApplication1.CustomerList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Customer List</title>
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
            max-width: 800px;
            text-align: center;
        }

        h2 {
            color: #9111A5;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
        }

        .grid-container {
            width: 100%;
            overflow-x: auto;
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

        .aspButton {
            background-color: #9111A5;
            color: white;
            border: none;
            padding: 8px 15px;
            border-radius: 10px;
            cursor: pointer;
            font-size: 14px;
            transition: background-color 0.3s ease;
        }

        .aspButton:hover {
            background-color: #7A0D8C;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Customer List</h2>
        <div class="grid-container">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID" 
                OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" 
                OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="UserID" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Username" HeaderText="Tên Đăng Nhập" />
                    <asp:BoundField DataField="Role" HeaderText="Vai Trò" />
                    <asp:CommandField ShowEditButton="True" ButtonType="Button" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" Text="Xóa" CssClass="aspButton" 
                                CommandName="Delete" OnClientClick="return confirm('Bạn có chắc muốn xóa?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
