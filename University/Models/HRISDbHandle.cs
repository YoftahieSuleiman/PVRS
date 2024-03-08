using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace University.Models
{
    public class HRISDbHandle
    {
        protected SqlConnection SqlConnection;
        #region Open Connection

        public bool Open(string Connection = "WebProjectDB")
        {
            SqlConnection = new SqlConnection(@WebConfigurationManager.ConnectionStrings["HRISConnectionString"].ToString());
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    SqlConnection.Open();
                }
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        #endregion

        #region Close Connection

        public bool Close()
        {
            try
            {
                SqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
        #region get table

        public DataTable GetTable(string sql)
        {
            var query = sql;
            var d = new DataTable();
            try
            {
                if (SqlConnection.State == ConnectionState.Open)
                {
                    var cmd = new SqlCommand(query, SqlConnection);
                    var reader = cmd.ExecuteReader();
                    d.Load(reader);
                    reader.Close();
                    return d;
                }
                return d;
            }
            catch (Exception ex)
            {
                return d;
            }
        }

        #endregion
    }
}