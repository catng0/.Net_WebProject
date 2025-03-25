using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class OrderDetails : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["restaurantConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMenuItems();
                LoadOrderDetails();
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
            int userID = 1;  // Giả định UserID = 1 (có thể lấy từ session)
            string query = "SELECT mi.Name, od.Quantity, mi.Price FROM OrderDetails od " +
                           "JOIN MenuItems mi ON od.ItemID = mi.ItemID WHERE od.UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataBind();

                // Tính tổng tiền
                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToDecimal(row["Price"]) * Convert.ToInt32(row["Quantity"]);
                }
                lblTotalPrice.Text = total.ToString("C");
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int itemID = Convert.ToInt32(btn.CommandArgument);
            int userID = 1;  // Giả định UserID = 1 (có thể lấy từ session)

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra nếu món đã có trong OrderDetails
                string checkQuery = "SELECT COUNT(*) FROM OrderDetails WHERE UserID = @UserID AND ItemID = @ItemID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserID", userID);
                checkCmd.Parameters.AddWithValue("@ItemID", itemID);

                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    // Nếu đã có, cập nhật số lượng
                    string updateQuery = "UPDATE OrderDetails SET Quantity = Quantity + 1 WHERE UserID = @UserID AND ItemID = @ItemID";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@UserID", userID);
                    updateCmd.Parameters.AddWithValue("@ItemID", itemID);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    string insertQuery = "INSERT INTO OrderDetails (UserID, ItemID, Quantity) VALUES (@UserID, @ItemID, 1)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@UserID", userID);
                    insertCmd.Parameters.AddWithValue("@ItemID", itemID);
                    insertCmd.ExecuteNonQuery();
                }
            }

            LoadOrderDetails();
        }
    }
}
