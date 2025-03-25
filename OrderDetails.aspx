<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderDetails.aspx.cs" Inherits="WebApplication1.OrderDetails" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Details</title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    
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
        

         h2 {
        color: #6A1B9A;
        transition: transform 0.3s ease-in-out, color 0.3s ease-in-out;
        }
         h2::before {
            content: "\f2e7"; /* Mã Unicode của biểu tượng đồ ăn (tùy chỉnh theo nhu cầu) */
            font-family: "Font Awesome 5 Free";
            font-weight: 900;
            margin-right: 10px; /* Khoảng cách giữa icon và chữ */
        }
        h2:hover {
            color: #8E44AD; /* Thay đổi màu sắc khi hover */
        }

        h2 { color: #6A1B9A; }
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
        .gridview-style tr:nth-child(odd) td {
            background-color: #F3E5F5;
            color: #6A1B9A;
        }
        .gridview-style tr:nth-child(even) td {
            background-color: #FFFFFF;
            color: #6A1B9A;
        }
        .gridview-style td {
            padding: 10px;
            border-bottom: 1px solid #D8BFD8;
            text-align: center;
        }
        .gridview-style tr:hover td {
            background-color: #E1BEE7;
            color: #4A148C;
        }
        .total-price {
            font-size: 18px;
            font-weight: bold;
            margin-top: 20px;
            color: #6A1B9A;
        }
        .btn-toggle-order {
            background-color: #6A1B9A;
            color: white;
            font-size: 16px;
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin-top: 20px;
        }
        .btn-toggle-order:hover {
            background-color: #8E44AD;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>MENU ITEMS</h2>

        <asp:GridView ID="GridView1" runat="server" CssClass="gridview-style" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="ItemID" HeaderText="ID" />
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="Price" HeaderText="Price" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnAdd" runat="server" Text="+" CommandArgument='<%# Eval("ItemID") %>' OnClick="btnAddItem_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Button ID="btnToggleOrder" runat="server" Text="YOUR ORDER" OnClick="btnToggleOrder_Click" CssClass="btn-toggle-order" />

        <h2>YOUR ORDER HERE</h2>

        <asp:GridView ID="GridView2" runat="server" CssClass="gridview-style" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Item Name" />
                <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                <asp:BoundField DataField="Price" HeaderText="Price" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        
                        <asp:Button ID="btnRemove" runat="server" Text="-" CommandArgument='<%# Eval("ItemID") %>' OnClick="btnRemoveItem_Click" />
                        
                    </ItemTemplate>

                </asp:TemplateField>

            </Columns>
        </asp:GridView>

        <div class="total-price">
            Total Price: <asp:Label ID="lblTotalPrice" runat="server" Text="0" />
        </div>
        
        <asp:Button ID="btnCreateOrder" runat="server" Text="TẠO ĐƠN" OnClick="btnCreateOrder_Click" CssClass="btn-action" />
        <asp:Button ID="btnCancelOrder" runat="server" Text="HỦY ĐƠN" OnClick="btnCancelOrder_Click" CssClass="btn-action" />
        
        <asp:Button ID="btnViewInvoice" runat="server" Text="CHECK INVOICE" OnClick="btnViewInvoice_Click" CssClass="btn-toggle-order" />

        <asp:GridView ID="GridViewInvoice" runat="server" CssClass="gridview-style" Visible="false" AutoGenerateColumns="True">
            <Columns>
                <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
                <asp:BoundField DataField="OrderTime" HeaderText="Order Time" />
                <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                <asp:BoundField DataField="Price" HeaderText="Price" />
                <asp:BoundField DataField="TotalPrice" HeaderText="Total Price" />
            </Columns>
        </asp:GridView>

    </form>
</body>
</html>
