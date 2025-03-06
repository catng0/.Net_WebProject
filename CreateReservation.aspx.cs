using System;
using System.Data.SqlClient;

namespace restaurant
{
    public partial class CreateReservation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    // Lấy thông tin từ form
                    int userID = Convert.ToInt32(txtUserID.Text);
                    int tableID = Convert.ToInt32(txtTableID.Text);
                    DateTime dateTime = Convert.ToDateTime(txtDateTime.Text);

                    // Thêm đặt bàn mới vào database
                    string query = "INSERT INTO Reservation (UserID, TableID, DateTime, Status) VALUES (@UserID, @TableID, @DateTime, 'Pending')";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.Parameters.AddWithValue("@DateTime", dateTime);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();

                    // Chuyển hướng về trang quản lý đặt bàn
                    Response.Redirect("Reservation.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Chuyển hướng về trang quản lý đặt bàn
            Response.Redirect("Reservation.aspx");
        }
    }
}