using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace WebApplication1
{
    public partial class MenuItemDetail : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string itemId = Request.QueryString["ItemID"];
                if (!string.IsNullOrEmpty(itemId))
                {
                    LoadItemDetails(itemId);
                }
                else
                {
                    Response.Redirect("MenuItem.aspx");
                }
            }
        }

        private void LoadItemDetails(string itemId)
        {
            try
            {
                if (!clsDatabase.OpenConnection())
                {
                    ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                    return;
                }

                string query = "SELECT * FROM MenuItems WHERE ItemID = @ItemID";
                using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["Name"].ToString();
                            decimal price = Convert.ToDecimal(reader["Price"]);
                            txtPrice.Text = ((int)price).ToString("N0"); // Giữ nguyên giá trị, chỉ bỏ ,00
                            txtDescription.Text = reader["Description"].ToString();
                            txtCategory.Text = reader["Category"].ToString();
                            txtType.Text = reader["Type"].ToString();

                            if (reader["Image"] != DBNull.Value)
                            {
                                byte[] imageData = (byte[])reader["Image"];
                                imgFood.ImageUrl = imageData.Length > 0
                                    ? $"data:image/png;base64,{Convert.ToBase64String(imageData)}"
                                    : "https://via.placeholder.com/300";
                            }
                            else
                            {
                                imgFood.ImageUrl = "https://via.placeholder.com/300";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi khi tải thông tin món ăn: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string itemId = Request.QueryString["ItemID"];
            if (string.IsNullOrEmpty(itemId))
            {
                Response.Redirect("MenuItem.aspx");
                return;
            }

            try
            {
                if (!clsDatabase.OpenConnection())
                {
                    ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                    return;
                }

                string query;
                SqlCommand cmd;

                // Nếu có hình ảnh mới, cập nhật toàn bộ
                if (fileUploadImage.HasFile)
                {
                    query = "UPDATE MenuItems SET Name = @Name, Price = @Price, Description = @Description, Category = @Category, Type = @Type, Image = @Image WHERE ItemID = @ItemID";
                    cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@Image", fileUploadImage.FileBytes);
                }
                else // Nếu không có hình ảnh mới, chỉ cập nhật các trường khác
                {
                    query = "UPDATE MenuItems SET Name = @Name, Price = @Price, Description = @Description, Category = @Category, Type = @Type WHERE ItemID = @ItemID";
                    cmd = new SqlCommand(query, clsDatabase.con);
                }

                cmd.Parameters.AddWithValue("@ItemID", itemId);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Replace(".", ""));
                cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                cmd.Parameters.AddWithValue("@Category", txtCategory.Text);
                cmd.Parameters.AddWithValue("@Type", txtType.Text);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    ShowAlertAndRedirect("Cập nhật sản phẩm thành công!", "MenuItem.aspx");
                }
                else
                {
                    ShowAlert("Lỗi khi cập nhật sản phẩm!");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection();
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{message}');", true);
        }

        private void ShowAlertAndRedirect(string message, string url)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertRedirect", $"alert('{message}'); window.location='{url}';", true);
        }
    }
}
