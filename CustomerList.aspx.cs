using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class CustomerList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                getKhachHang();
            }
        }

        void getKhachHang()
        {
            clsDatabase.OpenConnection();
            SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT UserID, Username, Role FROM Users", clsDatabase.con);
            DataTable dt = new DataTable();
            sqlDa.Fill(dt);

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        // Khi nhấn "Edit"
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            getKhachHang();
        }

        // Khi nhấn "Update"
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int userID = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            string username = ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
            string role = ((TextBox)GridView1.Rows[e.RowIndex].Cells[2].Controls[0]).Text;

            clsDatabase.OpenConnection();
            string query = "UPDATE Users SET Username = @Username, Role = @Role WHERE UserID = @UserID";
            SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Role", role);
            cmd.Parameters.AddWithValue("@UserID", userID);

            cmd.ExecuteNonQuery();

            GridView1.EditIndex = -1;
            getKhachHang();
        }

        // Khi nhấn "Cancel"
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            getKhachHang();
        }

        // Khi nhấn "Delete"
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userID = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            clsDatabase.OpenConnection();
            string query = "DELETE FROM Users WHERE UserID = @UserID";
            SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
            cmd.Parameters.AddWithValue("@UserID", userID);

            cmd.ExecuteNonQuery();

            getKhachHang();
        }
    }
}
