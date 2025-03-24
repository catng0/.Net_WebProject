using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Nếu cần, có thể gọi hàm lấy dữ liệu ở đây
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = "Customer"; // Mặc định là Customer
            Response.Write("<script>alert('Button clicked!');</script>");
            

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Response.Write("<script>alert('Username or password cannot be empty!');</script>");
                return;
            }
            try
            {
                if (!clsDatabase.OpenConnection())
                {
                    Response.Write("<script>alert('Database connection failed!');</script>");
                    return;
                }

                // Kiểm tra username đã tồn tại chưa
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", clsDatabase.con))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    int userCount = (int)checkCmd.ExecuteScalar();

                    if (userCount > 0)
                    {
                        Response.Write("<script>alert('Username already exists!');</script>");
                        clsDatabase.CloseConnection();
                        return;
                    }
                }

                // Nếu Admin đang đăng nhập, có thể tạo tài khoản Staff
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                {
                    role = "Staff"; // Hoặc có thể cấp quyền khác nếu cần
                }

                // Chèn dữ liệu vào bảng Users
                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)", clsDatabase.con))
                {
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@Password", password); // Nên mã hóa mật khẩu
                    insertCmd.Parameters.AddWithValue("@Role", role);

                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Sign-up successful!');</script>");
                        Response.Redirect("SignIn.aspx");
                    }
                    else
                    {
                        Response.Write("<script>alert('Failed to insert data!');</script>");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Response.Write("<script>alert('SQL Error: " + sqlEx.Message + "');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
            finally
            {
                clsDatabase.CloseConnection();
            }
        }
    }
}
