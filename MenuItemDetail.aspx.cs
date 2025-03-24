using System;
using System.Configuration;
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
            string query = "SELECT * FROM MenuItems WHERE ItemID = @ItemID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["restaurantConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["Name"].ToString();
                            decimal price = Convert.ToDecimal(reader["Price"]);
                            txtPrice.Text = price.ToString("N0"); // Giữ nguyên giá trị, chỉ bỏ ,00
                            txtPrice.Text = ((int)price).ToString("N0");
                            txtDescription.Text = reader["Description"].ToString();
                            txtCategory.Text = reader["Category"].ToString();
                            txtType.Text = reader["Type"].ToString();

                            if (reader["Image"] != DBNull.Value)
                            {
                                byte[] imageData = (byte[])reader["Image"];
                                if (imageData.Length > 0)
                                {
                                    string base64String = Convert.ToBase64String(imageData);
                                    imgFood.ImageUrl = $"data:image/png;base64,{base64String}";
                                }
                                else
                                {
                                    imgFood.ImageUrl = "https://via.placeholder.com/300";
                                }
                            }
                            else
                            {
                                imgFood.ImageUrl = "https://via.placeholder.com/300";
                            }
                        }
                    }
                }
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
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["restaurantConnectionString"].ConnectionString))
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    // Nếu có hình ảnh mới, cập nhật toàn bộ
                    if (fileUploadImage.HasFile)
                    {
                        query = "UPDATE MenuItems SET Name = @Name, Price = @Price, Description = @Description, Category = @Category, Type = @Type, Image = @Image WHERE ItemID = @ItemID";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Image", fileUploadImage.FileBytes);
                    }
                    else // Nếu không có hình ảnh mới, chỉ cập nhật các trường khác
                    {
                        query = "UPDATE MenuItems SET Name = @Name, Price = @Price, Description = @Description, Category = @Category, Type = @Type WHERE ItemID = @ItemID";
                        cmd = new SqlCommand(query, conn);
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
                        Response.Write("<script>alert('Cập nhật sản phẩm thành công!'); window.location='MenuItem.aspx';</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lỗi khi cập nhật sản phẩm!');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Lỗi: " + ex.Message + "');</script>");
            }
        }
    }
}
