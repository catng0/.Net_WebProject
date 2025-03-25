using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class OrderDetails : System.Web.UI.Page
    {
        string connectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            connectionString = clsDatabase.OpenConnection();
            if (!IsPostBack)
            {
                LoadMenuItems();
                LoadOrderDetails();
                GridView2.Visible = false;
            }
        }

        private void LoadMenuItems()
        {
            string query = "SELECT * FROM MenuItems";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        private void LoadOrderDetails()
        {
            int userID = 1;
            int orderID = GetOrderID(userID);

            if (orderID == -1)
            {
                lblTotalPrice.Text = "0";
                GridView2.DataSource = null;
                GridView2.DataBind();
                return;
            }

            string query = "SELECT mi.Name, od.Quantity, od.Price, od.ItemID FROM OrderDetails od " +
                           "JOIN MenuItems mi ON od.ItemID = mi.ItemID WHERE od.OrderID = @OrderID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataBind();

                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToDecimal(row["Price"]) * Convert.ToInt32(row["Quantity"]);
                }
                lblTotalPrice.Text = total.ToString("C");
            }
        }

        private int GetOrderID(int userID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT OrderID FROM Orders WHERE UserID = @UserID AND Status = 'Pending'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            int userID = 1;
            int orderID = GetOrderID(userID);

            if (orderID == -1)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string createOrderQuery = "INSERT INTO Orders (UserID, Status, TotalPrice) OUTPUT INSERTED.OrderID VALUES (@UserID, 'Pending', 0)";
                    SqlCommand cmd = new SqlCommand(createOrderQuery, conn);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    orderID = (int)cmd.ExecuteScalar();
                }
                LoadOrderDetails();
                Response.Write("<script>alert('Đơn hàng mới đã được tạo!');</script>");
            }
            else
            {
                Response.Write("<script>alert('Bạn đã có đơn hàng chưa hoàn tất!');</script>");
            }
        }

        protected void btnCancelOrder_Click(object sender, EventArgs e)
        {
            int userID = 1;
            int orderID = GetOrderID(userID);

            if (orderID != -1)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string deleteOrderDetailsQuery = "DELETE FROM OrderDetails WHERE OrderID = @OrderID";
                    SqlCommand deleteOrderDetailsCmd = new SqlCommand(deleteOrderDetailsQuery, conn);
                    deleteOrderDetailsCmd.Parameters.AddWithValue("@OrderID", orderID);
                    deleteOrderDetailsCmd.ExecuteNonQuery();

                    string deleteOrderQuery = "DELETE FROM Orders WHERE OrderID = @OrderID AND Status = 'Pending'";
                    SqlCommand deleteOrderCmd = new SqlCommand(deleteOrderQuery, conn);
                    deleteOrderCmd.Parameters.AddWithValue("@OrderID", orderID);
                    deleteOrderCmd.ExecuteNonQuery();
                }
                LoadOrderDetails();
                Response.Write("<script>alert('Đơn hàng đã được hủy!');</script>");
            }
            else
            {
                Response.Write("<script>alert('Không có đơn hàng nào để hủy!');</script>");
            }
        }
    }
}
