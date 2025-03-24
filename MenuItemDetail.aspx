<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuItemDetail.aspx.cs" Inherits="WebApplication1.MenuItemDetail" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cập nhật sản phẩm</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h2 class="text-center text-primary">Cập nhật sản phẩm</h2>
            <div class="card p-3">
                <div class="mb-3">
                    <label class="form-label"><strong>Tên sản phẩm:</strong></label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-3">
                    <label class="form-label"><strong>Giá:</strong></label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-3">
                    <label class="form-label"><strong>Mô tả:</strong></label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" />
                </div>
                <div class="mb-3">
                    <label class="form-label"><strong>Danh mục:</strong></label>
                    <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-3">
                    <label class="form-label"><strong>Loại:</strong></label>
                    <asp:TextBox ID="txtType" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-3">
                    <label class="form-label"><strong>Hình ảnh:</strong></label>
                    <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" />
                    <asp:Image ID="imgFood" runat="server" Width="300px" Height="300px" CssClass="img-thumbnail mt-2" />
                </div>
                <asp:Button ID="btnUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
                <a href="MenuItem.aspx" class="btn btn-secondary mt-3">Quay lại</a>
            </div>
        </div>
    </form>
</body>
</html>
