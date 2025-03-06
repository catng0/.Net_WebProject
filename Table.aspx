<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Table.aspx.cs" Inherits="restaurant.Table" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Table Management</title>
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
        .grid-container {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
            padding: 20px;
        }
        .grid-box {
            flex: 1;
            min-width: 45%;
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
            text-align: center;
        }
        .gridview td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
            text-align: center;
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
            margin-left: 20px;
            margin-bottom: -10px;
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
        .add-table-form {
            margin: 15px 0;
            display: flex;
            gap: 10px;
            justify-content: center;
        }
        .add-table-form input {
            padding: 8px;
            border-radius: 5px;
            border: 1px solid #ddd;
        }
        .editSeat-btn {
            background-color: pink;
            padding: 8px 15px;
            color: black;
            border: none;
            cursor: pointer;
            border-radius: 5px;
        }
        .form-control{
            width: 30px;
        }
    </style>
    <script type="text/javascript">
        function toggleAddTableForm() {
            var addTableForm = document.getElementById("addTableForm");
            if (addTableForm.style.display === "none" || addTableForm.style.display === "") {
                addTableForm.style.display = "flex"; // Hiển thị form
            } else {
                addTableForm.style.display = "none"; // Ẩn form
            }
        }
    </script>
</head>
<body>
    <div class="container">
        <h2>Table Management</h2>
        <form id="form1" runat="server">
            <div class="filter-section">
                <div class="filter-group">
                    <label for="txtSeatsFilter">Seats:</label>
                    <asp:TextBox ID="txtSeatsFilter" runat="server" placeholder="Seats"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label for="ddlStatus">Status:</label>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Available" Value="Available"></asp:ListItem>
                        <asp:ListItem Text="Reserved" Value="Reserved"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group filter-buttons">
                    <asp:Button ID="btnFilter" runat="server" CssClass="btn btn-filter" Text="Filter" OnClick="btnFilter_Click" />
                    <asp:Button ID="btnCancelFilter" runat="server" CssClass="btn btn-cancel" Text="Cancel" OnClick="btnCancelFilter_Click" />
                </div>
            </div>
            
            <button class="add-btn" type="button" onclick="toggleAddTableForm()">New Table</button>
            <div id="addTableForm" class="add-table-form" style="display: none;">
                <asp:TextBox ID="txtSeats" runat="server" Placeholder="Enter number of seats"></asp:TextBox>
                <asp:Button ID="btnAddTable" runat="server" CssClass="btn status-btn" Text="Add" OnClick="btnAddTable_Click" />
                <button class="btn delete-btn" type="button" onclick="toggleAddTableForm()">Cancel</button>
            </div>
            
            <div class="grid-container">
                <div class="grid-box">
                    <asp:GridView ID="GridView1" runat="server" CssClass="gridview" AutoGenerateColumns="False"
                        OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit" 
                        OnRowCommand="GridView_RowCommand"
                        DataKeyNames="TableID">
                        <Columns>
            <asp:BoundField DataField="TableID" HeaderText="ID" ReadOnly="True" />
            <asp:TemplateField HeaderText="Seats">
                <ItemTemplate>
                    <%# Eval("Seats") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtSeatsEdit" runat="server" Text='<%# Bind("Seats") %>' CssClass="form-control txtSeatsEdit"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField DataField="Status" HeaderText="Status" ReadOnly="True" />
            
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button CommandName="ChangeStatus" CommandArgument='<%# Eval("TableID") %>' CssClass="btn status-btn" runat="server" Text='<%# Eval("Status") %>' />
                    <asp:Button CommandName="EditTable" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn edit-btn" runat="server" Text="Edit" />
                    <asp:Button CommandName="DeleteTable" CommandArgument='<%# Eval("TableID") %>' CssClass="btn delete-btn" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this table?');" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Button CommandName="UpdateTable" CommandArgument='<%# Eval("TableID") %>' CssClass="btn editSeat-btn" runat="server" Text="Update" />
                    <asp:Button CommandName="CancelEdit" CssClass="btn editSeat-btn" runat="server" Text="Cancel" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
                    </asp:GridView>
                </div>
                <div class="grid-box">
                    <asp:GridView ID="GridView2" runat="server" CssClass="gridview" AutoGenerateColumns="False" 
                        OnRowCommand="GridView_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="TableID" HeaderText="ID" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:BoundField DataField="Seats" HeaderText="Seats" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button CommandName="ChangeStatus" CommandArgument='<%# Eval("TableID") %>' CssClass="btn status-btn" runat="server" Text='<%# Eval("Status") %>' />
                                    <asp:Button CommandName="EditTable" CommandArgument='<%# Eval("TableID") %>' CssClass="btn edit-btn" runat="server" Text="Edit" />
                                    <asp:Button CommandName="DeleteTable" CommandArgument='<%# Eval("TableID") %>' CssClass="btn delete-btn" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this table?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <div class="pagination">
                <asp:Button ID="btnPrevious" runat="server" CssClass="btn-pagination" Text="Previous" OnClick="btnPrevious_Click" />
                <asp:Label ID="lblPageInfo" runat="server" CssClass="pageNumber" Text="1"></asp:Label>
                <asp:Button ID="btnNext" runat="server" CssClass="btn-pagination" Text="Next" OnClick="btnNext_Click" />
            </div>
        </form>
    </div>
</body>
</html>