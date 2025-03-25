using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WebAplication1
{
    public partial class CreateReservation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsernames();
                LoadTableIDs();
            }
        }

        private void LoadUsernames()
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "SELECT UserID, Username FROM Users";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlUsername.DataSource = reader;
                    ddlUsername.DataTextField = "Username";
                    ddlUsername.DataValueField = "UserID";
                    ddlUsername.DataBind();

                    reader.Close();
                    clsDatabase.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi tải danh sách User: " + ex.Message);
            }
        }

        private void LoadTableIDs()
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "SELECT TableID FROM Tables";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlTableID.DataSource = reader;
                    ddlTableID.DataTextField = "TableID";
                    ddlTableID.DataValueField = "TableID";
                    ddlTableID.DataBind();

                    reader.Close();
                    clsDatabase.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi tải danh sách Table: " + ex.Message);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    int userID = Convert.ToInt32(ddlUsername.SelectedValue);
                    int tableID = Convert.ToInt32(ddlTableID.SelectedValue);

                    DateTime dateTime;
                    if (!DateTime.TryParse(txtDateTime.Text, out dateTime))
                    {
                        Response.Write("Lỗi: Định dạng ngày không hợp lệ!");
                        return;
                    }

                    string query = "INSERT INTO Reservation (UserID, TableID, DateTime, Status) VALUES (@UserID, @TableID, @DateTime, 0)";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.Parameters.AddWithValue("@DateTime", dateTime);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();

                    Response.Redirect("Reservation.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi thêm đặt bàn: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Reservation.aspx");
        }
    }
}
