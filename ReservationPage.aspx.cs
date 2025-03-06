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
            if (!IsPostBack)
            {
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
            int userID = Convert.ToInt32(Session["UserID"]);
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
            int userID = 2;//Convert.ToInt32(Session["UserID"]);
            int tableID = int.Parse(ddlTable.SelectedValue);
            DateTime reservationDate;

            if (!DateTime.TryParse(txtDateTime.Text, out reservationDate))
            {
                Response.Write("<script>alert('Vui lòng nhập đúng định dạng ngày giờ!');</script>");
                return;
            }

            clsDatabase.OpenConnection();

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
        }
    }
}