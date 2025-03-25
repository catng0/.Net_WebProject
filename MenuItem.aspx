<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuItem.aspx.cs" Inherits="WebApplication1.MenuItem" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Menu Items</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        /* Giữ nguyên các phần khác */
    .GridViewStyle th, .GridViewStyle td {
        text-align: center;
        vertical-align: middle;
    }

    /* Giới hạn chiều rộng của các ô nhập khi Edit */
    .GridViewStyle input[type="text"] {
        width: 100px !important;
        font-size: 14px;
        padding: 4px;
    }

    /* Căn chỉnh lại các nút Edit, Update, Cancel */
    .GridViewStyle .btn {
        padding: 5px 10px;
        font-size: 14px;
    }

        body {
            background-color: #fce4ec;
            font-family: 'Arial', sans-serif;
        }
        .container {
            margin-top: 50px;
        }
        .gridview-container {
            background: white;
            padding: 20px;
            border-radius: 15px;
            box-shadow: 0px 0px 20px rgba(138, 43, 226, 0.3);
        }
        h2 {
            text-align: center;
            color: #8A2BE2;
            margin-bottom: 20px;
            font-weight: bold;
        }
        .add-form {
            display: none;
            background: #fff;
            padding: 15px;
            border-radius: 10px;
            box-shadow: 0px 0px 10px rgba(138, 43, 226, 0.2);
            margin-bottom: 20px;
        }
        .btn-add {
            background-color: #8A2BE2;
            color: white;
            border-radius: 8px;
            padding: 10px;
        }
        .btn-add:hover {
            background-color: #FF69B4;
        }
        .GridViewStyle th {
            background-color: #8A2BE2 !important;
            color: black !important;
            text-align: center;
            padding: 12px;
            font-size: 16px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="gridview-container">
                <h2>Quản lý Menu</h2>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Nhập tên món ăn..."></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    </div>
                </div>


                <div class="text-end mb-3">
                    <asp:Button ID="btnShowAddForm" runat="server" CssClass="btn btn-success" Text="Thêm món" OnClientClick="showAddForm(); return false;" />
                </div>
                <div class="add-form" id="addForm">
                    <h4 class="text-center text-primary">Thêm sản phẩm mới</h4>
                    <div class="row">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Tên sản phẩm"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="Giá"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="Mô tả"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Chọn danh mục" Value="" Selected="True" Disabled="True"></asp:ListItem>
                                <asp:ListItem Text="Khai vị" Value="Appetizer"></asp:ListItem>
                                <asp:ListItem Text="Món chính" Value="Main Course"></asp:ListItem>
                                <asp:ListItem Text="Tráng miệng" Value="Dessert"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4">
                            <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" />
                        </div>

                        <div class="col-md-4">
                            <label class="form-label">Loại:</label>
                            <div>
                                <asp:RadioButton ID="rbFood" runat="server" GroupName="TypeGroup" Text="Food" CssClass="form-check-input" />
                                <label for="rbFood" class="form-check-label"></label>
                            </div>
                            <div>
                                <asp:RadioButton ID="rbDrink" runat="server" GroupName="TypeGroup" Text="Drink" CssClass="form-check-input" />
                                <label for="rbDrink" class="form-check-label"></label>
                            </div>
                        </div>

                    </div>
                    <div class="text-center mt-3">
                        <asp:Button ID="btnAdd" runat="server" Text="Thêm sản phẩm" CssClass="btn btn-add"  />
                    </div>
                </div>
                
               <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered GridViewStyle"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
                    DataKeyNames="ItemID" 
                    PageSize="5"
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    OnRowCommand="GridView1_RowCommand"
                    OnRowEditing="GridView1_RowEditing"
                    OnRowUpdating="GridView1_RowUpdating"
                    OnRowCancelingEdit="GridView1_RowCancelingEdit"
                    OnRowDeleting="GridView1_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="ItemID" HeaderText="ID" ReadOnly="True" SortExpression="ItemID" />
                    <asp:BoundField DataField="Name" HeaderText="Tên" SortExpression="Name" />
                    <asp:BoundField DataField="Price" HeaderText="Giá" SortExpression="Price" 
    DataFormatString="{0:#,0}" HtmlEncode="False" />

                    <asp:BoundField DataField="Description" HeaderText="Mô tả" SortExpression="Description" />
                    <asp:BoundField DataField="Category" HeaderText="Danh mục" SortExpression="Category" />
                    <asp:BoundField DataField="Type" HeaderText="Loại" SortExpression="Type" />
        
        <%-- Nút Xem --%>
        <asp:TemplateField HeaderText="Chi tiết">
            <ItemTemplate>
                <asp:HyperLink ID="lnkView" runat="server" CssClass="btn btn-info" 
                    NavigateUrl='<%# "MenuItemDetail.aspx?ItemID=" + Eval("ItemID") %>' 
                    Text="Xem" />
            </ItemTemplate>
        </asp:TemplateField>


        

        <%-- Cột chức năng Xóa --%>
        <asp:TemplateField HeaderText="Xóa">
            <ItemTemplate>
                <asp:Button ID="btnDelete" runat="server" Text="Xóa" CommandName="Delete" CommandArgument='<%# Eval("ItemID") %>' CssClass="btn btn-danger" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa sản phẩm này?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

                <div id="detailForm" runat="server" ClientIDMode="Static" style="display: none;">
                    <h3>Chi tiết món ăn</h3>
                    <p><strong>Tên món:</strong> <asp:Label ID="lblName" runat="server" /></p>
                    <p><strong>Giá:</strong> <asp:Label ID="lblPrice" runat="server" /></p>
                    <p><strong>Mô tả:</strong> <asp:Label ID="lblDescription" runat="server" /></p>
                    <p><strong>Loại:</strong> <asp:Label ID="lblCategory" runat="server" /></p>
                    <p><strong>Kiểu:</strong> <asp:Label ID="lblType" runat="server" /></p>
                    <asp:Image ID="imgFood" runat="server" Width="200px" Height="200px" />
                    <br />
                    <asp:Button ID="btnClose" runat="server" Text="Đóng" OnClientClick="document.getElementById('detailForm').style.display='none'; return false;" />
                </div>
                
            </div>
        </div>
    </form>

<script>
    function showAddForm() {
        document.getElementById("addForm").style.display = "block";
    }
    function hideDetailForm() {
        document.getElementById("detailForm").style.display = "none";
    }
    function showDetailForm() {
        document.getElementById("detailForm").style.display = "block";
    }
</script>

</body>
</html>
