<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="WebApplication1.Orders" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Quản Lý Đơn Hàng</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background: radial-gradient(circle, rgba(255, 182, 193, 0.4) 0%, rgba(255, 255, 255, 1) 70%);
            text-align: center;
            margin: 0;
            padding: 0;
        }
        form {
            background-color: white;
            width: 80%;
            margin: auto;
            padding: 20px;
            border-radius: 15px;
            backdrop-filter: blur(10px);
            box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            border: 1px solid rgba(255, 182, 193, 0.6);
        }
        h2 { color: #6A1B9A; }
        .search-box {
            width: 200px;
            padding: 8px;
            border: 1px solid #8E44AD;
            border-radius: 5px;
            margin-bottom: 10px;
        }
        .btn {
            padding: 8px 15px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }
        .btn-search {
            background-color: #6A1B9A;
            color: white;
        }
        .btn-add {
            background-color: #8E44AD;
            color: white;
            margin-top: 10px;
        }
        .gridview-style {
            width: 100%;
            border-collapse: collapse;
            margin: 20px auto;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
            background: rgba(255, 255, 255, 0.8);
            backdrop-filter: blur(8px);
        }
        .gridview-style th {
            background-color: #8E44AD;
            color: white;
            padding: 12px;
            text-align: center;
        }
        .gridview-style tr:nth-child(odd) td { background-color: #F3E5F5; color: #6A1B9A; }
        .gridview-style tr:nth-child(even) td { background-color: #FFFFFF; color: #6A1B9A; }
        .gridview-style td {
            padding: 10px;
            border-bottom: 1px solid #D8BFD8;
            text-align: center;
        }
        .gridview-style tr:hover td { background-color: #E1BEE7; color: #4A148C; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>QUẢN LÝ ĐƠN HÀNG</h2>

        <%-- Tìm kiếm --%>
        <div>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-box" Placeholder="Nhập tên khách hàng..."></asp:TextBox>
            <asp:DropDownList ID="ddlFilterStatus" runat="server">
                <asp:ListItem Text="Tất cả" Value=""></asp:ListItem>
                <asp:ListItem Text="Chưa thanh toán" Value="Chưa thanh toán"></asp:ListItem>
                <asp:ListItem Text="Đã thanh toán" Value="Đã thanh toán"></asp:ListItem>
                <asp:ListItem Text="Đã hủy" Value="Đã hủy"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" OnClick="btnSearch_Click" CssClass="btn btn-search" />
        </div>

        <%-- Thêm đơn hàng --%>
        <div>
            <h3>Thêm Đơn Hàng</h3>
            <table style="margin: auto;">
                <tr>
                    <td>Khách hàng:</td>
                    <td><asp:DropDownList ID="ddlNewUser" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Bàn số:</td>
                    <td><asp:TextBox ID="txtNewTableID" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Thời gian đặt:</td>
                    <td><asp:TextBox ID="txtNewOrderTime" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Tổng tiền:</td>
                    <td><asp:TextBox ID="txtNewTotalPrice" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Trạng thái:</td>
                    <td><asp:TextBox ID="txtNewStatus" runat="server"></asp:TextBox></td>
                </tr>
            </table>
            <asp:Button ID="btnAddOrder" runat="server" Text="Thêm Đơn Hàng" OnClick="btnAddOrder_Click" CssClass="btn btn-add" />
        </div>

        <%-- Danh sách đơn hàng --%>
        <asp:GridView ID="GridViewOrders" runat="server" CssClass="gridview-style" AutoGenerateColumns="False"
            DataKeyNames="OrderID" AllowPaging="True" PageSize="5"
            OnPageIndexChanging="GridViewOrders_PageIndexChanging"
            OnRowEditing="GridViewOrders_RowEditing"
            OnRowUpdating="GridViewOrders_RowUpdating"
            OnRowCancelingEdit="GridViewOrders_RowCancelingEdit"
            OnRowDeleting="GridViewOrders_RowDeleting">
            <Columns>
                <asp:BoundField DataField="OrderID" HeaderText="Mã ĐH" ReadOnly="True" />
                <asp:BoundField DataField="Username" HeaderText="Khách hàng" />
                <asp:BoundField DataField="TableID" HeaderText="Bàn số" />
                <asp:BoundField DataField="OrderTime" HeaderText="Thời gian" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="TotalPrice" HeaderText="Tổng tiền" />
                <asp:BoundField DataField="Status" HeaderText="Trạng thái" />
                <asp:CommandField ShowEditButton="True" HeaderText="Sửa" />
                <asp:CommandField ShowDeleteButton="True" HeaderText="Xóa" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
