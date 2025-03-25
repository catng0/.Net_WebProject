
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class MenuItem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                BindGridView();
            }
        }

        private void BindCategories()
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("Chọn danh mục", ""));
            ddlCategory.Items.Add(new ListItem("Appetizer", "Appetizer"));
            ddlCategory.Items.Add(new ListItem("Main Course", "Main Course"));
            ddlCategory.Items.Add(new ListItem("Dessert", "Dessert"));
        }

        private void LoadData()
        {
            try
            {
                if (!clsDatabase.OpenConnection())
                {
                    ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                    return;
                }

                string query = "SELECT * FROM MenuItems";
                using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi khi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection();
            }
        }


        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                try
                {
                    int index = Convert.ToInt32(e.CommandArgument);

                    if (GridView1.DataKeys[index] != null) // Kiểm tra DataKeys
                    {
                        int itemId = Convert.ToInt32(GridView1.DataKeys[index].Value);

                        string query = "DELETE FROM MenuItems WHERE ItemID=@ItemID";

                        if (!clsDatabase.OpenConnection())
                        {
                            ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                            return;
                        }

                        using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                        {
                            cmd.Parameters.AddWithValue("@ItemID", itemId);
                            cmd.ExecuteNonQuery();
                        }

                        LoadData();
                        ShowAlert("Xóa món ăn thành công!");
                    }
                    else
                    {
                        ShowAlert("Không thể xác định món ăn cần xóa.");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Lỗi khi xóa món ăn: " + ex.Message);
                }
                finally
                {
                    clsDatabase.CloseConnection();
                }
            }

            if (e.CommandName == "ViewDetails")
            {
                try
                {
                    int index = Convert.ToInt32(e.CommandArgument);

                    if (GridView1.DataKeys[index] != null) // Kiểm tra DataKeys
                    {
                        int itemId = Convert.ToInt32(GridView1.DataKeys[index].Value);
                        string query = "SELECT Name, Price, Description, Category, Type, Image FROM MenuItems WHERE ItemID = @ItemID";

                        if (!clsDatabase.OpenConnection())
                        {
                            ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                            return;
                        }

                        using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                        {
                            cmd.Parameters.Add("@ItemID", SqlDbType.Int).Value = itemId;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read()) // Nếu có dữ liệu
                                {
                                    lblName.Text = reader["Name"].ToString();
                                    lblPrice.Text = reader["Price"].ToString();
                                    lblDescription.Text = reader["Description"].ToString();
                                    lblCategory.Text = reader["Category"].ToString();
                                    lblType.Text = reader["Type"].ToString();
                                    imgFood.ImageUrl = reader["Image"].ToString();

                                    // Gọi JavaScript để hiển thị form chi tiết
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showDetail", "showDetailForm();", true);
                                }
                                else
                                {
                                    ShowAlert("Không tìm thấy thông tin món ăn.");
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowAlert("Không thể xác định món ăn.");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Lỗi khi lấy thông tin món ăn: " + ex.Message);
                }
                finally
                {
                    clsDatabase.CloseConnection();
                }
            }
        }






        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            LoadData();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            LoadData();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                // Lấy ID của dòng cần cập nhật
                int ItemID = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

                // Lấy dữ liệu từ ô nhập
                GridViewRow row = GridView1.Rows[e.RowIndex];
                string name = ((TextBox)row.Cells[1].Controls[0]).Text;
                string price = ((TextBox)row.Cells[2].Controls[0]).Text;
                string description = ((TextBox)row.Cells[3].Controls[0]).Text;
                string category = ((TextBox)row.Cells[4].Controls[0]).Text;
                string type = ((TextBox)row.Cells[5].Controls[0]).Text;

                // Mở kết nối CSDL
                if (!clsDatabase.OpenConnection())
                {
                    ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                    return;
                }

                string query = "UPDATE MenuItems SET Name=@Name, Price=@Price, Description=@Description, Category=@Category, Type=@Type WHERE ItemID=@ID";

                using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                {
                    cmd.Parameters.AddWithValue("@ID", ItemID);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(price));
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@Type", type);

                    cmd.ExecuteNonQuery();
                }

                GridView1.EditIndex = -1; // Thoát chế độ Edit
                LoadData(); // Cập nhật lại dữ liệu
                ShowAlert("Cập nhật món ăn thành công!");
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi khi cập nhật món ăn: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection(); // Đóng kết nối sau khi thực hiện xong
            }
        }



        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int itemId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["ItemID"]);

                // Kiểm tra và mở kết nối
                if (!clsDatabase.OpenConnection())
                {
                    ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                    return;
                }

                string query = "DELETE FROM MenuItems WHERE ItemID=@ItemID";

                using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                    cmd.ExecuteNonQuery();
                }

                LoadData(); // Tải lại dữ liệu sau khi xóa
                ShowAlert("Xóa thành công!");
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi khi xóa: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection(); // Đóng kết nối để tránh rò rỉ tài nguyên
            }
        }


        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + message + "');", true);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ form
                string name = txtName.Text.Trim();
                string priceText = txtPrice.Text.Trim();
                string description = txtDescription.Text.Trim();
                string category = ddlCategory.SelectedValue;
                string type = rbFood.Checked ? "Food" : rbDrink.Checked ? "Drink" : "";

                // Kiểm tra dữ liệu hợp lệ
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(category))
                {
                    ShowAlert("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                if (!decimal.TryParse(priceText, out decimal price))
                {
                    ShowAlert("Giá tiền không hợp lệ!");
                    return;
                }

                if (string.IsNullOrEmpty(type))
                {
                    ShowAlert("Vui lòng chọn loại món ăn!");
                    return;
                }

                byte[] imageData = null;

                // Kiểm tra nếu người dùng đã tải lên ảnh
                if (fileUploadImage.HasFile)
                {
                    using (Stream fs = fileUploadImage.PostedFile.InputStream)
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        imageData = br.ReadBytes((int)fs.Length);
                    }
                }
                else
                {
                    ShowAlert("Vui lòng chọn ảnh!");
                    return;
                }

                // Kiểm tra kết nối
                if (!clsDatabase.OpenConnection())
                {
                    ShowAlert("Không thể kết nối cơ sở dữ liệu!");
                    return;
                }

                // Câu lệnh SQL INSERT
                string query = @"INSERT INTO MenuItems (Name, Price, Description, Category, Image, Type) 
                         VALUES (@Name, @Price, @Description, @Category, @Image, @Type)";

                using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.Add("@Image", SqlDbType.VarBinary, -1).Value = imageData;
                    cmd.Parameters.AddWithValue("@Type", type);

                    cmd.ExecuteNonQuery();
                }

                ShowAlert("Thêm món ăn thành công!");

                // Reset form
                txtName.Text = "";
                txtPrice.Text = "";
                txtDescription.Text = "";
                ddlCategory.SelectedIndex = 0;
                rbFood.Checked = false;
                rbDrink.Checked = false;

                LoadData(); // Cập nhật lại danh sách món ăn
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi khi thêm món ăn: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection(); // Đảm bảo luôn đóng kết nối
            }
        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            LoadMenuItems(keyword);
        }

        private void LoadMenuItems(string keyword)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["restaurantConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open(); // Chỉ mở kết nối khi thực sự cần thiết

                    string query = "SELECT * FROM MenuItems";
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        query += " WHERE Name LIKE @Keyword";
                        parameters.Add(new SqlParameter("@Keyword", $"%{keyword}%"));
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                GridView1.DataSource = dt;
                                GridView1.DataBind();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }


        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridView(); // Gọi lại phương thức để nạp dữ liệu
        }

        private void BindGridView()
        {
            // Giả sử bạn đang lấy dữ liệu từ CSDL, thay thế DataTable bằng cách truy vấn thực tế
            string query = "SELECT * FROM MenuItems"; // Thay bằng truy vấn thực tế
            DataTable dt = GetData(query);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        private DataTable GetData(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                if (!clsDatabase.OpenConnection())
                {
                    throw new Exception("Không thể kết nối cơ sở dữ liệu!");
                }

                using (SqlCommand cmd = new SqlCommand(query, clsDatabase.con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hoặc hiển thị thông báo nếu cần
                throw new Exception("Lỗi truy vấn dữ liệu: " + ex.Message);
            }
            finally
            {
                clsDatabase.CloseConnection();
            }

            return dt;
        }





    }
}
