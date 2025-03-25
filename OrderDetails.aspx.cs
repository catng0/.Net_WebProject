using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class OrderDetails : System.Web.UI.Page
    {
        // Không cần khai báo connection string nữa vì đã dùng clsDatabase
        // string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["restaurantConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
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
            using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
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
            int userID = 1; // Giả định lấy từ session
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

            using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
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
            using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
            {
                string query = "SELECT OrderID FROM Orders WHERE UserID = @UserID AND Status = 'Pending'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int itemID = Convert.ToInt32(btn.CommandArgument);
            int userID = 1; // Giả định UserID lấy từ session
            int orderID = GetOrderID(userID);

            if (orderID == -1)
            {
                using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
                {
                    string createOrderQuery = "INSERT INTO Orders (UserID, Status) OUTPUT INSERTED.OrderID VALUES (@UserID, 'Pending')";
                    SqlCommand createOrderCmd = new SqlCommand(createOrderQuery, conn);
                    createOrderCmd.Parameters.AddWithValue("@UserID", userID);
                    orderID = (int)createOrderCmd.ExecuteScalar();
                }
            }

            using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
            {
                string checkQuery = "SELECT COUNT(*) FROM OrderDetails WHERE OrderID = @OrderID AND ItemID = @ItemID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@OrderID", orderID);
                checkCmd.Parameters.AddWithValue("@ItemID", itemID);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    string updateQuery = "UPDATE OrderDetails SET Quantity = Quantity + 1 WHERE OrderID = @OrderID AND ItemID = @ItemID";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@OrderID", orderID);
                    updateCmd.Parameters.AddWithValue("@ItemID", itemID);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    string insertQuery = "INSERT INTO OrderDetails (OrderID, ItemID, Quantity, Price) VALUES (@OrderID, @ItemID, 1, (SELECT Price FROM MenuItems WHERE ItemID = @ItemID))";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@OrderID", orderID);
                    insertCmd.Parameters.AddWithValue("@ItemID", itemID);
                    insertCmd.ExecuteNonQuery();
                }
            }
            LoadOrderDetails();
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int itemID = Convert.ToInt32(btn.CommandArgument);
            int userID = 1; // Giả định UserID lấy từ session
            int orderID = GetOrderID(userID);

            if (orderID == -1)
            {
                return;
            }

            using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
            {
                string query = "UPDATE OrderDetails SET Quantity = Quantity - 1 WHERE OrderID = @OrderID AND ItemID = @ItemID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Parameters.AddWithValue("@ItemID", itemID);
                cmd.ExecuteNonQuery();

                string deleteQuery = "DELETE FROM OrderDetails WHERE OrderID = @OrderID AND ItemID = @ItemID AND Quantity <= 0";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@OrderID", orderID);
                deleteCmd.Parameters.AddWithValue("@ItemID", itemID);
                deleteCmd.ExecuteNonQuery();
            }

            LoadOrderDetails();
        }

        protected void btnToggleOrder_Click(object sender, EventArgs e)
        {
            GridView2.Visible = !GridView2.Visible;

            if (GridView2.Visible)
            {
                LoadOrderDetails();
            }
        }

        protected void btnViewInvoice_Click(object sender, EventArgs e)
        {
            int userID = 1; // Giả định UserID lấy từ session
            int orderID = GetOrderID(userID);

            if (orderID != -1)
            {
                // Kiểm tra trạng thái hiển thị của bảng GridViewInvoice
                GridViewInvoice.Visible = !GridViewInvoice.Visible;

                if (GridViewInvoice.Visible)
                {
                    LoadInvoiceDetails(orderID);
                }
            }
        }

        private void LoadInvoiceDetails(int orderID)
        {
            string query = "SELECT o.OrderID, o.OrderTime, mi.Name AS ItemName, od.Quantity, od.Price, (od.Quantity * od.Price) AS TotalPrice " +
                           "FROM Orders o " +
                           "JOIN OrderDetails od ON o.OrderID = od.OrderID " +
                           "JOIN MenuItems mi ON od.ItemID = mi.ItemID " +
                           "WHERE o.OrderID = @OrderID";

            using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridViewInvoice.DataSource = dt;
                GridViewInvoice.DataBind();
            }
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            int userID = 1; // Giả định UserID lấy từ session
            int orderID = GetOrderID(userID);

            if (orderID == -1)
            {
                using (SqlConnection conn = clsDatabase.OpenConnection())  // Sử dụng clsDatabase để mở kết nối
                {
                    string createOrderQuery = "INSERT INTO Orders (UserID, Status, TotalPrice) OUTPUT INSERTED.OrderID VALUES (@UserID, 'Pending', 0)";
                    SqlCommand cmd = new SqlCommand(createOrderQuery, conn);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    orderID = (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
