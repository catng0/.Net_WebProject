using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Orders : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["restaurantConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadOrders();
                LoadUsers(ddlNewUser);
            }
        }

        private void LoadOrders(string searchKeyword = "", string filterStatus = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT O.OrderID, U.Username, O.TableID, O.OrderTime, O.TotalPrice, O.Status 
                                 FROM Orders O 
                                 JOIN Users U ON O.UserID = U.UserID
                                 WHERE (@Keyword = '' OR U.Username LIKE '%' + @Keyword + '%')
                                 AND (@Status = '' OR O.Status = @Status)";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@Keyword", searchKeyword);
                da.SelectCommand.Parameters.AddWithValue("@Status", filterStatus);

                DataTable dt = new DataTable();
                da.Fill(dt);

                GridViewOrders.DataSource = dt;
                GridViewOrders.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadOrders(txtSearch.Text.Trim(), ddlFilterStatus.SelectedValue);
        }

        protected void btnAddOrder_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu chưa chọn khách hàng
                if (ddlNewUser.SelectedValue == "0")
                {
                    Response.Write("<script>alert('Vui lòng chọn khách hàng!');</script>");
                    return;
                }

                // Kiểm tra giá trị nhập vào
                int userID = Convert.ToInt32(ddlNewUser.SelectedValue);
                string tableID = txtNewTableID.Text.Trim();
                decimal totalPrice;
                DateTime orderTime;

                if (!decimal.TryParse(txtNewTotalPrice.Text, out totalPrice))
                {
                    Response.Write("<script>alert('Tổng tiền phải là số hợp lệ!');</script>");
                    return;
                }

                if (!DateTime.TryParse(txtNewOrderTime.Text, out orderTime))
                {
                    Response.Write("<script>alert('Thời gian đặt không hợp lệ!');</script>");
                    return;
                }

                string status = txtNewStatus.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Orders (UserID, TableID, OrderTime, TotalPrice, Status) VALUES (@UserID, @TableID, @OrderTime, @TotalPrice, @Status)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.Parameters.AddWithValue("@OrderTime", orderTime);
                    cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    cmd.Parameters.AddWithValue("@Status", status);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                // Làm mới danh sách đơn hàng
                LoadOrders();

                // Xóa nội dung đã nhập
                txtNewTableID.Text = "";
                txtNewOrderTime.Text = "";
                txtNewTotalPrice.Text = "";
                txtNewStatus.Text = "";

                Response.Write("<script>alert('Thêm đơn hàng thành công!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Lỗi: " + ex.Message + "');</script>");
            }
        }

        private void LoadUsers(DropDownList ddl)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT UserID, Username FROM Users", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl.DataSource = dt;
                ddl.DataTextField = "Username";
                ddl.DataValueField = "UserID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Chọn khách hàng --", "0"));
            }
        }

        protected void GridViewOrders_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewOrders.EditIndex = e.NewEditIndex;
            LoadOrders();
        }

        protected void GridViewOrders_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridViewOrders.Rows[e.RowIndex];

            int orderID = Convert.ToInt32(GridViewOrders.DataKeys[e.RowIndex].Values["OrderID"]);
            string tableID = ((TextBox)row.Cells[2].Controls[0]).Text;
            DateTime orderTime = Convert.ToDateTime(((TextBox)row.Cells[3].Controls[0]).Text);
            decimal totalPrice = Convert.ToDecimal(((TextBox)row.Cells[4].Controls[0]).Text);
            string status = ((TextBox)row.Cells[5].Controls[0]).Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Orders SET TableID=@TableID, OrderTime=@OrderTime, TotalPrice=@TotalPrice, Status=@Status WHERE OrderID=@OrderID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Parameters.AddWithValue("@TableID", tableID);
                cmd.Parameters.AddWithValue("@OrderTime", orderTime);
                cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            GridViewOrders.EditIndex = -1;
            LoadOrders();
        }

        protected void GridViewOrders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewOrders.EditIndex = -1;
            LoadOrders();
        }

        protected void GridViewOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int orderID = Convert.ToInt32(GridViewOrders.DataKeys[e.RowIndex].Values["OrderID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Orders WHERE OrderID = @OrderID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrderID", orderID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LoadOrders();
        }

        protected void GridViewOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewOrders.PageIndex = e.NewPageIndex;
            LoadOrders();
        }
    }
}
