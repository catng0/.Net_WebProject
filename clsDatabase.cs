using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1
{
	public class clsDatabase
	{
        public static SqlConnection con;

        public static bool OpenConnection()
        {
            try
            {
                con = new SqlConnection("Server=LAPTOP-8L7Q69M8\\KIEUHOA; Database=restaurant; Integrated Security=True;");
                con.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool CloseConnection()
        {
            try
            {
                con.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
