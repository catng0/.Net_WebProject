using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace restaurant
{
    public partial class Reservation : System.Web.UI.Page
    {
        private int _pageSize = 5; // Số lượng đặt bàn hiển thị trên mỗi trang
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
                    if (!string.IsNullOrEmpty(txtTableID.Text.Trim()))
                    {
                        conditions.Add("r.TableID = @TableID");
                        parameters.Add(new SqlParameter("@TableID", txtTableID.Text.Trim()));
                    }

                    // Truy vấn dữ liệu
                    string query = @"
                SELECT r.ReservationID, u.Username, r.TableID, r.DateTime
                FROM Reservation r
                INNER JOIN Users u ON r.UserID = u.UserID";

                    if (conditions.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }

                    query += " ORDER BY r.DateTime DESC";

                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    clsDatabase.CloseConnection();

                    // Phân trang dữ liệu
                    PagedDataSource pds = new PagedDataSource
                    {
                        DataSource = dt.DefaultView,
                        AllowPaging = true,
                        PageSize = _pageSize,
                        CurrentPageIndex = _currentPage
                    };

                    ViewState["TotalPages"] = pds.PageCount;

                    // Hiển thị lên GridView
                    GridViewReservations.DataSource = pds;
                    GridViewReservations.DataBind();

                    // Cập nhật trạng thái phân trang
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


        // Sự kiện khi nhấn nút "Filter"
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            _currentPage = 0; // Reset về trang đầu tiên khi áp dụng bộ lọc
            BindData();
        }

        // Sự kiện khi nhấn nút "Cancel"
        protected void btnCancelFilter_Click(object sender, EventArgs e)
        {
            // Reset các trường lọc về giá trị mặc định
            txtUsername.Text = "";
            txtDate.Text = "";
            txtTime.Text = "";
            txtTableID.Text = "";

            _currentPage = 0; // Reset về trang đầu tiên
            BindData();
        }

        protected void GridViewReservations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int reservationID = Convert.ToInt32(e.CommandArgument);

            //if (e.CommandName == "ChangeStatus")
            //{
            //    ChangeStatus(reservationID);
            //}
            //else
            if (e.CommandName == "EditReservation")
            {
                Response.Redirect($"EditReservation.aspx?ReservationID={reservationID}");
            }
            else if (e.CommandName == "DeleteReservation")
            {
                DeleteReservation(reservationID);
            }
        }

        protected void btnNewReservation_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateReservation.aspx");
        }

        //private void ChangeStatus(int reservationID)
        //{
        //    try
        //    {
        //        if (clsDatabase.OpenConnection())
        //        {
        //            // Cập nhật trạng thái của đặt bàn
        //            string query = "UPDATE Reservation SET Status = CASE WHEN Status = 'Pending' THEN 'Completed' ELSE 'Pending' END WHERE ReservationID = @ReservationID";
        //            SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
        //            cmd.Parameters.AddWithValue("@ReservationID", reservationID);
        //            cmd.ExecuteNonQuery();
        //            clsDatabase.CloseConnection();
        //            BindData();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write("Lỗi khi cập nhật trạng thái: " + ex.Message);
        //    }
        //}

        private void DeleteReservation(int reservationID)
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    // Xóa đặt bàn
                    string query = "DELETE FROM Reservation WHERE ReservationID = @ReservationID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@ReservationID", reservationID);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();
                    BindData();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi xóa đặt bàn: " + ex.Message);
            }
        }

        // Xử lý chuyển trang
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
    }
}