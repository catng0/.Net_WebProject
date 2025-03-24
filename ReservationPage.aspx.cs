using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class ReservationPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra nếu chưa đăng nhập thì chuyển hướng về trang SignIn
            if (Session["UserID"] == null)
            {
                Response.Redirect("SignIn.aspx");
            }

            if (!IsPostBack)
            {
                lblUsername.Text = "Xin chào, " + Session["Username"].ToString(); // Hiển thị tên user
                LoadTables();
                LoadUserReservations();
            }
        }

        void LoadTables()
        {
            clsDatabase.OpenConnection();
            SqlDataAdapter da = new SqlDataAdapter("SELECT TableID FROM Tables WHERE Status = 'Available'", clsDatabase.con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlTable.DataSource = dt;
            ddlTable.DataTextField = "TableID";
            ddlTable.DataValueField = "TableID";
            ddlTable.DataBind();
        }

        void LoadUserReservations()
        {
            int userID = Convert.ToInt32(Session["UserID"]); // Lấy ID từ session

            clsDatabase.OpenConnection();
            SqlDataAdapter da = new SqlDataAdapter("SELECT ReservationID, TableID, DateTime FROM Reservations WHERE UserID = @UserID", clsDatabase.con);
            da.SelectCommand.Parameters.AddWithValue("@UserID", userID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            GridViewReservations.DataSource = dt;
            GridViewReservations.DataBind();
        }

        protected void btnReserve_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu user chưa đăng nhập thì không cho đặt bàn
            if (Session["UserID"] == null)
            {
                Response.Redirect("SignIn.aspx");
                return;
            }

            int userID = Convert.ToInt32(Session["UserID"]); // Lấy userID từ session
            int tableID = int.Parse(ddlTable.SelectedValue);
            DateTime reservationDate;

            if (!DateTime.TryParse(txtDateTime.Text, out reservationDate))
            {
                Response.Write("<script>alert('Vui lòng nhập đúng định dạng ngày giờ!');</script>");
                return;
            }

            clsDatabase.OpenConnection();

            // Kiểm tra xem bàn còn trống không
            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Tables WHERE TableID = @TableID AND Status = 'Available'", clsDatabase.con);
            checkCmd.Parameters.AddWithValue("@TableID", tableID);
            int count = (int)checkCmd.ExecuteScalar();

            if (count == 0)
            {
                Response.Write("<script>alert('Bàn đã được đặt trước!');</script>");
                return;
            }

            // Thêm vào Reservations
            SqlCommand cmd = new SqlCommand("INSERT INTO Reservations (UserID, TableID, DateTime) VALUES (@UserID, @TableID, @DateTime)", clsDatabase.con);
            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@TableID", tableID);
            cmd.Parameters.AddWithValue("@DateTime", reservationDate);
            cmd.ExecuteNonQuery();

            // Cập nhật trạng thái bàn thành 'Reserved'
            SqlCommand updateCmd = new SqlCommand("UPDATE Tables SET Status = 'Reserved' WHERE TableID = @TableID", clsDatabase.con);
            updateCmd.Parameters.AddWithValue("@TableID", tableID);
            updateCmd.ExecuteNonQuery();

            LoadTables();
            LoadUserReservations();

            Response.Write("<script>alert('Đặt bàn thành công!');</script>");
        }


        protected void GridViewReservations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelReservation")
            {
                int reservationID = Convert.ToInt32(e.CommandArgument);

                clsDatabase.OpenConnection();

                // Lấy TableID từ Reservation để cập nhật trạng thái bàn
                SqlCommand getTableCmd = new SqlCommand("SELECT TableID FROM Reservations WHERE ReservationID = @ReservationID", clsDatabase.con);
                getTableCmd.Parameters.AddWithValue("@ReservationID", reservationID);
                int tableID = (int)getTableCmd.ExecuteScalar();

                // Xóa đặt bàn
                SqlCommand deleteCmd = new SqlCommand("DELETE FROM Reservations WHERE ReservationID = @ReservationID", clsDatabase.con);
                deleteCmd.Parameters.AddWithValue("@ReservationID", reservationID);
                deleteCmd.ExecuteNonQuery();

                // Cập nhật lại trạng thái bàn thành 'Available'
                SqlCommand updateCmd = new SqlCommand("UPDATE Tables SET Status = 'Available' WHERE TableID = @TableID", clsDatabase.con);
                updateCmd.Parameters.AddWithValue("@TableID", tableID);
                updateCmd.ExecuteNonQuery();

                LoadUserReservations();
                LoadTables();
                Response.Write("<script>alert('Hủy đặt bàn thành công!');</script>");
            }
        }

    }
}
