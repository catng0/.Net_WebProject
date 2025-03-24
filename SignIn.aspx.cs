using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra nếu user đã đăng nhập rồi thì không cần login lại
            if (Session["UserID"] != null)
            {
                Response.Redirect("ReservationPage.aspx");
            }
        }

        protected void ButtonSignIn_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Response.Write("<script>alert('Please enter username and password!');</script>");
                return;
            }

            try
            {
                if (!clsDatabase.OpenConnection())
                {
                    Response.Write("<script>alert('Database connection failed!');</script>");
                    return;
                }

                // Lấy UserID và Role
                using (SqlCommand cmd = new SqlCommand("SELECT UserID, Role FROM Users WHERE Username = @Username AND Password = @Password", clsDatabase.con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // Cần mã hóa password trong thực tế

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userID = Convert.ToInt32(reader["UserID"]);
                            string role = reader["Role"].ToString();

                            // Lưu vào Session
                            Session["UserID"] = userID;
                            Session["Username"] = username;
                            Session["Role"] = role;

                            Response.Redirect("ReservationPage.aspx");
                        }
                        else
                        {
                            Response.Write("<script>alert('Invalid username or password!');</script>");
                        }
                    }
                }
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
