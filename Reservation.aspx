<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reservation.aspx.cs" Inherits="restaurant.Reservation" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reservation Management</title>
    <style>
        body {
            background-color: rgb(249, 198, 207);
            font-family: Arial, sans-serif;
            text-align: center;
        }
        .container {
            width: 90%;
            margin: auto;
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        h2 {
            color: rgb(145, 17, 165);
        }
        .gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        .gridview th {
            background-color: rgb(145, 17, 165);
            color: white;
            padding: 10px;
        }
        .gridview td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }
        .btn {
            padding: 8px 12px;
            border: none;
            cursor: pointer;
            color: white;
            border-radius: 5px;
            text-decoration: none;
        }
        .edit-btn {
            background-color: orange;
        }
        .delete-btn {
            background-color: red;
        }
        .status-btn {
            background-color: green;
        }
        .add-btn {
            background-color: rgb(249, 198, 207);
            padding: 10px 15px;
            color: black;
            font-size: 16px;
            margin-top: 10px;
            border-radius: 5px;
            border: black solid 1px;
            cursor: pointer;
            display: block;
            margin-left: 0px;
        }

        .add-btn:hover{
            background-color: rgb(145, 17, 165);
            color: white;
        }

        .btn-pagination {
            background-color: rgb(249, 198, 207);
            color: black;
            margin-top: 20px;
            cursor: pointer;
            padding: 8px 12px;
            border-radius: 8px;
            border: none;
            width: 80px;
            height: 30px;
        }
        .btn-pagination:disabled {
            background-color: rgb(249, 198, 207);
            color: white;
            cursor: not-allowed;
            opacity: 0.5;
        }
        .pageNumber {
            padding: 0 10px;
        }
        .filter-section {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: center;
            width: 100%;
            display: flex;
            justify-content: center;
        }

        .filter-group {
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        .filter-buttons {
            display:block;
            margin-top: 22px;
        }

        .filter-group label {
            font-weight: bold;
            margin-bottom: 5px;
        }

        .btn-filter, .btn-cancel {
            padding: 8px 15px;
            border: none;
            cursor: pointer;
            border-radius: 5px;
        }
        .btn-filter {
            background-color: pink;
            color: black;
        }
        .btn-cancel {
            background-color: purple;
            color: white;
        }

        .btn-filter:disabled,
        .btn-cancel:disabled {
            background-color: #ddd;
            color: #888;
            cursor: not-allowed;
        }

    </style>
</head>
<body>
    <div class="container">
        <h2>Reservation Management</h2>
        <form id="form1" runat="server">
            <div class="filter-section">
                <div class="filter-group">
                    <label for="txtUsername">Customer Name:</label>
                    <asp:TextBox ID="txtUsername" runat="server" placeholder="Customer Name"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label for="txtDate">Arrive Date:</label>
                    <asp:TextBox ID="txtDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label for="txtTime">Arrive Time:</label>
                    <asp:TextBox ID="txtTime" runat="server" TextMode="Time"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label for="txtTableID">ID Table:</label>
                    <asp:TextBox ID="txtTableID" runat="server" placeholder="ID Table"></asp:TextBox>
                </div>
                <div class="filter-group filter-buttons">
                    <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-filter" Text="Filter" OnClick="btnFilter_Click" />
                    <asp:Button ID="btnCancelFilter" runat="server" CssClass="btn btn-cancel" Text="Cancel" OnClick="btnCancelFilter_Click" />
                </div>
            </div>
            <asp:Button ID="btnNewReservation" runat="server" CssClass="add-btn" Text="New Reservation" OnClick="btnNewReservation_Click" />
            <asp:GridView ID="GridViewReservations" runat="server" CssClass="gridview" AutoGenerateColumns="False" OnRowCommand="GridViewReservations_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ReservationID" HeaderText="ID" />
                    <asp:BoundField DataField="Username" HeaderText="Customer Name" />
                    <asp:BoundField DataField="TableID" HeaderText="ID Table" />
                    <asp:BoundField DataField="DateTime" HeaderText="Arrive Time" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button CommandName="EditReservation" CommandArgument='<%# Eval("ReservationID") %>' CssClass="btn edit-btn" runat="server" Text="Edit" />
                            <asp:Button CommandName="DeleteReservation" CommandArgument='<%# Eval("ReservationID") %>' CssClass="btn delete-btn" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure to delete this reservation?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div class="pagination">
                <asp:Button ID="btnPrevious" runat="server" CssClass="btn-pagination" Text="Previous" OnClick="btnPrevious_Click" />
                <asp:Label ID="lblPageInfo" runat="server" CssClass="pageNumber" Text="1"></asp:Label>
                <asp:Button ID="btnNext" runat="server" CssClass="btn-pagination" Text="Ne" OnClick="btnNext_Click" />
            </div>
        </form>
    </div>
</body>

</html>