using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace WebAplication1
{
    public partial class Reservation : System.Web.UI.Page
    {
        private int _pageSize = 5; 
        private int _currentPage
        {
            get { return (int)(ViewState["CurrentPage"] ?? 0); }
            set { ViewState["CurrentPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    List<string> conditions = new List<string>();
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(txtUsername.Text.Trim()))
                    {
                        conditions.Add("u.Username LIKE @Username");
                        parameters.Add(new SqlParameter("@Username", "%" + txtUsername.Text.Trim() + "%"));
                    }
                    if (!string.IsNullOrEmpty(txtDate.Text.Trim()))
                    {
                        conditions.Add("CONVERT(date, r.DateTime) = @Date");
                        parameters.Add(new SqlParameter("@Date", txtDate.Text.Trim()));
                    }
                    if (!string.IsNullOrEmpty(txtTime.Text.Trim()))
                    {
                        conditions.Add("CONVERT(time, r.DateTime) = @Time");
                        parameters.Add(new SqlParameter("@Time", txtTime.Text.Trim()));
                    }
                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        conditions.Add("r.Status = @Status");
                        parameters.Add(new SqlParameter("@Status", ddlStatus.SelectedValue));
                    }
                    if (!string.IsNullOrEmpty(txtTableID.Text.Trim()))
                    {
                        conditions.Add("r.TableID = @TableID");
                        parameters.Add(new SqlParameter("@TableID", txtTableID.Text.Trim()));
                    }

                    
                    string query = @"
                        SELECT r.ReservationID, r.UserID, u.Username, r.TableID, r.Status, r.DateTime
                        FROM Reservation r
                        INNER JOIN Users u ON r.UserID = u.UserID";

                    if (conditions.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }

                    query += " ORDER BY \r\n    CASE WHEN r.Status = 0 THEN 0 ELSE 1 END,  \r\n    r.DateTime ASC  \r\n";

                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    clsDatabase.CloseConnection();

                    
                    PagedDataSource pds = new PagedDataSource
                    {
                        DataSource = dt.DefaultView,
                        AllowPaging = true,
                        PageSize = _pageSize,
                        CurrentPageIndex = _currentPage
                    };

                    ViewState["TotalPages"] = pds.PageCount;

                    
                    GridViewReservations.DataSource = pds;
                    GridViewReservations.DataBind();

                    
                    btnPrevious.Enabled = _currentPage > 0;
                    btnNext.Enabled = _currentPage < (int)ViewState["TotalPages"] - 1;

                    lblPageInfo.Text = $"{_currentPage + 1}";
                }
                else
                {
                    Response.Write("Không thể kết nối đến cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi: " + ex.Message);
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            _currentPage = 0; 
            BindData();
        }

        protected void btnCancelFilter_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtDate.Text = "";
            txtTime.Text = "";
            txtTableID.Text = "";

            _currentPage = 0; 
            BindData();
        }

        protected void GridViewReservations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int reservationID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditReservation")
            {

                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                int rowIndex = row.RowIndex;
                GridViewReservations.EditIndex = rowIndex; // Chuyển dòng sang chế độ chỉnh sửa
                BindData(); // Load dữ liệu vào dropdown
            }
            else if (e.CommandName == "UpdateReservation")
            {
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                int rowIndex = row.RowIndex;

                // Lấy ReservationID từ DataKeys
                int editReservationID = Convert.ToInt32(GridViewReservations.DataKeys[rowIndex].Value);

                // Lấy dữ liệu từ dropdown
                DropDownList ddlUsername = (DropDownList)row.FindControl("ddlUsername");
                DropDownList ddlTableID = (DropDownList)row.FindControl("ddlTableID");
                DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
                TextBox txtDateTime = (TextBox)row.FindControl("txtDateTime");

                if (ddlUsername != null && ddlTableID != null && ddlStatus != null && txtDateTime != null)
                {
                    try
                    {
                        if (clsDatabase.OpenConnection())
                        {
                            // Chuyển đổi DateTime từ chuỗi nhập vào thành kiểu DateTime trong C#
                            DateTime parsedDateTime;
                            bool isValidDateTime = DateTime.TryParse(txtDateTime.Text, out parsedDateTime);

                            if (!isValidDateTime)
                            {
                                Response.Write("Lỗi: Định dạng ngày giờ không hợp lệ.");
                                return;
                            }

                            string query = "UPDATE Reservation SET UserID = @UserID, TableID = @TableID, Status = @Status, DateTime = @DateTime WHERE ReservationID = @ReservationID";
                            SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                            cmd.Parameters.AddWithValue("@UserID", ddlUsername.SelectedValue);
                            cmd.Parameters.AddWithValue("@TableID", ddlTableID.SelectedValue);
                            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);

                            // Định dạng DateTime trước khi truyền vào SQL
                            cmd.Parameters.AddWithValue("@DateTime", parsedDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                            cmd.Parameters.AddWithValue("@ReservationID", editReservationID);

                            cmd.ExecuteNonQuery();
                            clsDatabase.CloseConnection();
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("Lỗi khi cập nhật: " + ex.Message);
                    }
                }

                GridViewReservations.EditIndex = -1; // Thoát chế độ chỉnh sửa
                BindData();
            }
            else if (e.CommandName == "CancelReservation")
            {
                GridViewReservations.EditIndex = -1; 
                BindData();
            }
            else if (e.CommandName == "DeleteReservation")
            {
                try
                {
                    clsDatabase.OpenConnection();
                    string query = "DELETE FROM Reservation WHERE ReservationID = @ReservationID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@ReservationID", reservationID);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();
                    BindData();
                }
                catch (Exception ex)
                {
                    Response.Write("Lỗi khi xóa đặt bàn: " + ex.Message);
                }
            }
            else if (e.CommandName == "ChangeStatus")
            {
                try
                {
                    clsDatabase.OpenConnection();
                    string query = "UPDATE Reservation SET Status = CASE \r\n    WHEN Status = 0 THEN 1 \r\n    ELSE 0 \r\nEND  WHERE ReservationID = @ReservationID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@ReservationID", reservationID);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();
                    BindData();
                }
                catch (Exception ex)
                {
                    Response.Write("Lỗi khi cập nhật trạng thái: " + ex.Message);
                }
            }
        }
        protected void GridViewReservations_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex == GridViewReservations.EditIndex)
                {
                    DropDownList ddlUsername = (DropDownList)e.Row.FindControl("ddlUsername");
                    DropDownList ddlTableID = (DropDownList)e.Row.FindControl("ddlTableID");
                    TextBox txtDateTime = (TextBox)e.Row.FindControl("txtDateTime");

                    DataRowView rowView = (DataRowView)e.Row.DataItem;
                    string currentUserID = rowView["UserID"].ToString(); 
                    string currentTableID = rowView["TableID"].ToString();
                    DateTime currentDateTime = Convert.ToDateTime(rowView["DateTime"]);

                    if (ddlUsername != null)
                    {
                        ddlUsername.DataSource = GetUsers();  
                        ddlUsername.DataTextField = "Username";  
                        ddlUsername.DataValueField = "UserID";  
                        ddlUsername.DataBind();
                        ddlUsername.SelectedValue = currentUserID;
                    }

                    if (ddlTableID != null)
                    {
                        ddlTableID.DataSource = GetTables();
                        ddlTableID.DataTextField = "TableID";
                        ddlTableID.DataValueField = "TableID";
                        ddlTableID.DataBind();
                        ddlTableID.SelectedValue = currentTableID;
                    }

                    if (txtDateTime != null)
                    {
                        txtDateTime.Text = currentDateTime.ToString("yyyy-MM-ddTHH:mm"); 
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi: " + ex.Message);
            }
        }

        protected void btnNewReservation_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateReservation.aspx");
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            _currentPage--;
            BindData();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            _currentPage++;
            BindData();
        }

        private DataTable GetUsers()
        {
            DataTable dt = new DataTable();
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "SELECT UserID, Username FROM Users ORDER BY Username";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    clsDatabase.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi lấy danh sách Users: " + ex.Message);
            }
            return dt;
        }

        private DataTable GetTables()
        {
            DataTable dt = new DataTable();
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "SELECT TableID FROM Tables ORDER BY TableID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    clsDatabase.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi lấy danh sách Tables: " + ex.Message);
            }
            return dt;
        }

    }
}