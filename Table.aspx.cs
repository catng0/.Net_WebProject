using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace restaurant
{
    public partial class Table : System.Web.UI.Page
    {
        private int _pageSize = 10;
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

                    // Xử lý bộ lọc
                    if (!string.IsNullOrEmpty(txtSeatsFilter.Text.Trim()))
                    {
                        conditions.Add("Seats >= @Seats");
                        parameters.Add(new SqlParameter("@Seats", txtSeatsFilter.Text.Trim()));
                    }
                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        conditions.Add("Status = @Status");
                        parameters.Add(new SqlParameter("@Status", ddlStatus.SelectedValue));
                    }

                    string query = "SELECT * FROM Tables";
                    if (conditions.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }
                    query += " ORDER BY CASE WHEN Status = 'Available' THEN 0 ELSE 1 END, TableID";

                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    clsDatabase.CloseConnection();

                    // Phân trang chính xác theo _currentPage
                    PagedDataSource pds = new PagedDataSource
                    {
                        DataSource = dt.DefaultView,
                        AllowPaging = true,
                        PageSize = _pageSize,
                        CurrentPageIndex = _currentPage
                    };

                    ViewState["TotalPages"] = pds.PageCount;

                    // Lấy đúng dữ liệu theo trang hiện tại
                    DataTable dtPaged = dt.Clone();
                    int startIndex = _currentPage * _pageSize;
                    int endIndex = Math.Min(startIndex + _pageSize, dt.Rows.Count);

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        dtPaged.ImportRow(dt.Rows[i]);
                    }

                    // Chia nhỏ cho hai GridView
                    // Chia dữ liệu cho 2 GridView
                    DataTable dt1 = dtPaged.Clone();
                    DataTable dt2 = dtPaged.Clone();

                    int totalRows = dtPaged.Rows.Count;
                    int halfSize = (totalRows + 1) / 2; // GridView1 luôn chứa nhiều hơn nếu lẻ

                    for (int i = 0; i < totalRows; i++)
                    {
                        if (i < halfSize)
                        {
                            dt1.ImportRow(dtPaged.Rows[i]);
                        }
                        else
                        {
                            dt2.ImportRow(dtPaged.Rows[i]);
                        }
                    }

                    GridView1.DataSource = dt1;
                    GridView1.DataBind();
                    GridView2.DataSource = dt2;
                    GridView2.DataBind();


                    // Cập nhật trạng thái nút chuyển trang
                    btnPrevious.Enabled = _currentPage > 0;
                    btnNext.Enabled = _currentPage < (int)ViewState["TotalPages"] - 1;
                    lblPageInfo.Text = $"{_currentPage + 1}";
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi: " + ex.Message);
            }
        }


        protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ChangeStatus")
            {
                int tableID = Convert.ToInt32(e.CommandArgument);
                ChangeStatus(tableID);
            }
            else if (e.CommandName == "EditTable")
            {
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                int rowIndex = row.RowIndex;
                GridView1.EditIndex = rowIndex; // Chuyển dòng sang chế độ chỉnh sửa
                BindData();
            }
            else if (e.CommandName == "UpdateTable")
            {
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                int rowIndex = row.RowIndex;

                // Đảm bảo DataKeys không bị null và rowIndex hợp lệ
                if (GridView1.DataKeys != null && rowIndex >= 0 && rowIndex < GridView1.DataKeys.Count)
                {
                    int tableID = Convert.ToInt32(GridView1.DataKeys[rowIndex].Value);
                    TextBox txtSeats = (TextBox)row.FindControl("txtSeats");

                    if (txtSeats != null)
                    {
                        string newSeats = txtSeats.Text.Trim();
                        if (!string.IsNullOrEmpty(newSeats))
                        {
                            try
                            {
                                if (clsDatabase.OpenConnection())
                                {
                                    string query = "UPDATE Tables SET Seats = @Seats WHERE TableID = @TableID";
                                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                                    cmd.Parameters.AddWithValue("@Seats", newSeats);
                                    cmd.Parameters.AddWithValue("@TableID", tableID);

                                    int rowsAffected = cmd.ExecuteNonQuery();
                                    clsDatabase.CloseConnection();

                                    if (rowsAffected <= 0)
                                    {
                                        Response.Write("Không có dòng nào được cập nhật. Kiểm tra TableID!");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Response.Write("Lỗi khi cập nhật: " + ex.Message);
                            }
                        }
                        else
                        {
                            Response.Write("Vui lòng nhập số ghế hợp lệ.");
                        }
                    }

                }

                GridView1.EditIndex = -1; // Thoát chế độ chỉnh sửa
                BindData();
            }
            else if (e.CommandName == "CancelEdit")
            {
                GridView1.EditIndex = -1; // Hủy chỉnh sửa
                BindData();
            }
            else if (e.CommandName == "DeleteTable")
            {
                int tableID = Convert.ToInt32(e.CommandArgument);
                DeleteTable(tableID);
            }
        }

        private void ChangeStatus(int tableID)
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "UPDATE Tables SET Status = CASE WHEN Status = 'Available' THEN 'Reserved' ELSE 'Available' END WHERE TableID = @TableID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();
                    BindData();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi cập nhật trạng thái: " + ex.Message);
            }
        }

        private void DeleteTable(int tableID)
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "DELETE FROM Tables WHERE TableID = @TableID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();
                    BindData();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi xóa bàn: " + ex.Message);
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
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            _currentPage = 0;
            BindData();
        }

        protected void btnCancelFilter_Click(object sender, EventArgs e)
        {
            txtSeatsFilter.Text = "";
            ddlStatus.SelectedIndex = 0;
            _currentPage = 0;
            BindData();
        }
        protected void btnAddTable_Click(object sender, EventArgs e)
        {
            try
            {
                if (clsDatabase.OpenConnection())
                {
                    int seats;
                    if (int.TryParse(txtSeats.Text, out seats) && seats > 0)
                    {
                        string query = "INSERT INTO Tables (Status, Seats) VALUES ('Available', @Seats)";
                        SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                        cmd.Parameters.AddWithValue("@Seats", seats);
                        cmd.ExecuteNonQuery();
                        clsDatabase.CloseConnection();
                        txtSeats.Text = ""; // Reset ô nhập
                        BindData();
                    }
                    else
                    {
                        Response.Write("Vui lòng nhập số ghế hợp lệ.");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi thêm bàn: " + ex.Message);
            }
        }

        // Khi bấm Edit
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex; // Đặt dòng đang chỉnh sửa
            BindData(); // Cập nhật lại GridView
        }

        // Khi bấm Update
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int tableID = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            TextBox txtSeats = (TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0];

            try
            {
                if (clsDatabase.OpenConnection())
                {
                    string query = "UPDATE Tables SET Seats = @Seats WHERE TableID = @TableID";
                    SqlCommand cmd = new SqlCommand(query, clsDatabase.con);
                    cmd.Parameters.AddWithValue("@Seats", txtSeats.Text.Trim());
                    cmd.Parameters.AddWithValue("@TableID", tableID);
                    cmd.ExecuteNonQuery();
                    clsDatabase.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Lỗi khi cập nhật: " + ex.Message);
            }

            GridView1.EditIndex = -1; // Thoát chế độ chỉnh sửa
            BindData(); // Load lại dữ liệu
        }

        // Khi bấm Cancel
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1; // Hủy chỉnh sửa
            BindData(); // Load lại dữ liệu
        }


    }
}