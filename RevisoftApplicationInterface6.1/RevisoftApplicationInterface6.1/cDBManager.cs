using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace RevisoftApplication
{
   class cDBManager
   {
            
      public static Object EseguiComando(String sqlString)
      {         
         SqlConnection conn = null;
         SqlCommand cmd = null;
			Object ret = null;

			try
         { 
            conn = new SqlConnection(App.connString);
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = sqlString;
            ret = cmd.ExecuteScalar();
            conn.Close();				
         }
         catch (Exception ex)
         {
				if (conn != null && conn.State == ConnectionState.Open)
					conn.Close();
				App.GestioneLog(ex.Message);
			}
			return ret;
		}

    public static SqlConnection GetNewConnection()
    {
      return new SqlConnection(App.connString);
    }

		public static Object EseguiComando(String sqlString, SqlConnection conn)
		{
			var cmd = conn.CreateCommand();
			cmd.CommandText = sqlString;
			return  cmd.ExecuteScalar();
		}

		public static DataSet CaricaDataset(string strCommand)
      {
         DataSet ds = new DataSet();

         SqlConnection conn = null;
         SqlCommand cmd = null;

         try
         {
            conn = new SqlConnection(App.connString);
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = strCommand;
            SqlDataAdapter da = new SqlDataAdapter((SqlCommand)cmd);
            try
            {
               da.Fill(ds);
            }
            catch (Exception ee)
            {
               throw new Exception("(Class RevisoftApplication:CaricaDataset) : " + ee.Message);
            }
            conn.Close();
         }
         catch (Exception ex)
         {
				App.GestioneLog(ex.Message);
			}
         return ds;
      }


      public static DataSet CaricaDatasetDaStoredProcedure(string strCommand, Dictionary<String, String> dic)
      {
         DataSet ds = new DataSet();

         SqlConnection conn = null;
         SqlCommand cmd = null;
         try
         {
            conn = new SqlConnection(App.connString);
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = strCommand;

            foreach (String str in dic.Keys)
            {
               SqlParameter par = new SqlParameter();
               par.ParameterName = "@" + str;
               par.Value = dic[str];
               cmd.Parameters.Add(par);
            }

            SqlDataAdapter da = new SqlDataAdapter((SqlCommand)cmd);
            try
            {
               da.Fill(ds);
            }
            catch (Exception ee)
            {
               throw new Exception("(Class RevisoftApplication:CaricaDatasetDaStoredProcedure) : " + ee.Message);
            }
            conn.Close();
         }
         catch (Exception ex)
         {
					App.GestioneLog(ex.Message);
			}
         return ds;

      }

   }
}