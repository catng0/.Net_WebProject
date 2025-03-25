<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderDetails.aspx.cs" Inherits="WebApplication1.OrderDetails" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Details</title>
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
        .total-price {
            font-size: 18px;
            font-weight: bold;
            margin-top: 20px;
            color: #6A1B9A;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Menu Items</h2>

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

        <h2>Your Order</h2>

        <asp:GridView ID="GridView2" runat="server" CssClass="gridview-style" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Item Name" />
                <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                <asp:BoundField DataField="Price" HeaderText="Price" />
            </Columns>
        </asp:GridView>

        <div class="total-price">
            Total Price: <asp:Label ID="lblTotalPrice" runat="server" Text="0" />
        </div>
    </form>
</body>
</html>