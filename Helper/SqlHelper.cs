using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.Json;
using System.Data;

namespace ZKLT25.API.Helper
{
    public class SqlHelper
    {
        /// <summary>
        /// 数据库服务器时间
        /// </summary>
        public DateTime DbTime
        {
            get { return (DateTime)GetScalar("select getdate()"); }
        }

        private static readonly string connString = "server=.;database=eWorkSAP;uid=sa;pwd=sa000";
        //private static readonly string connString = "server=172.30.16.40;Database=eWork;User Id=sa;Password=sa000;";

        /// <summary>
        /// 得到数据库连接对象
        /// </summary>
        /// <returns></returns>
        private static SqlConnection Connection()
        {
            string _path = $"appsettings.json";
            IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
               .Add(new JsonConfigurationSource { Path = _path, Optional = false, ReloadOnChange = true })
               .Build();
            string connString = Configuration.GetSection("ConnectionStrings:Default").Value;
            SqlConnection con = new SqlConnection(connString);
            return con;
        }

        /// <summary>
        /// 只进行读取（使用后需要关闭）
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public static SqlDataReader GetSDR(string strSql)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(strSql, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                return sdr;
            }
        }

        /// <summary>
        /// 只进行读取（使用后需要关闭）
        /// </summary>
        /// <param name="strProc">存储过程名称</param>
        /// <param name="paralist">所需参数集合</param>
        /// <returns></returns>
        public static SqlDataReader GetSDR(string strProc, ref SqlParameter[] paralist)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(strProc, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 9999;
                SqlDataReader sdr = cmd.ExecuteReader();
                return sdr;
            }
        }
        /// <summary>
        /// 接受SQL，返回DataSet
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static DataSet GetDS(string sql)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                sda.SelectCommand.CommandTimeout = 6000;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                sda.Dispose();
                return ds;
            }
        }

        /// <summary>
        /// 接受SQL，返回DataSet
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public async static Task<DataSet> GetDSAsync(string sql)
        {
            var res = await Task.Factory.StartNew(() =>
            {
                try
                {
                    using (SqlConnection con = Connection())
                    {
                        con.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                        sda.SelectCommand.CommandTimeout = 6000;
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        sda.Dispose();
                        return ds;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return res;
        }

        /// <summary>
        /// 指定过程名称，返回DataSet
        /// </summary>
        /// <param name="StoreName">存储过程名称</param>
        /// <param name="paralist">所需参数集合</param>
        /// <returns></returns>
        public static DataSet GetDS(string StoreName, ref SqlParameter[] paralist)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(StoreName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 999;
                foreach (SqlParameter sp in paralist)
                {
                    cmd.Parameters.Add(sp);
                }
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                sda.Dispose();
                cmd.Dispose();
                return ds;
            }
        }

        /// <summary>
        /// 执行SQL,如:insert,update,delete(已包含事务)
        /// </summary>
        /// <param name="sql">SQL语句</param>
        public static int ExecCmd(string sql)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction("ExecTran");
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, con, tran);
                    cmd.CommandTimeout = 999;
                    int result = cmd.ExecuteNonQuery();
                    tran.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async static Task<int> ExecCmdAsync(string sql)
        {
            var res = await Task.Factory.StartNew(() =>
            {
                using (SqlConnection con = Connection())
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction("ExecTran");
                    try
                    {
                        SqlCommand cmd = new SqlCommand(sql, con, tran);
                        cmd.CommandTimeout = 999;
                        int result = cmd.ExecuteNonQuery();
                        tran.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }

            });
            return res;
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storeName">存储过程名称</param>
        /// <param name="paralist">所需参数集合</param>
        public static int ExecCmd(string storeName, ref SqlParameter[] paralist)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(storeName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 9999;
                    foreach (SqlParameter sp in paralist)
                    {
                        cmd.Parameters.Add(sp);
                    }
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行存储过程，重载1：无参数
        /// </summary>
        /// <param name="StoreName">存储过程名称</param>
        public static int ExecStore(string StoreName)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(StoreName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行存储过程，重载2：带参数
        /// </summary>
        /// <param name="StoreName">存储过程名称</param>
        /// <param name="paralist">所需参数集合</param>
        public static int ExecStore(string StoreName, ref SqlParameter[] paralist)
        {
            using (SqlConnection con = Connection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(StoreName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter sp in paralist)
                {
                    cmd.Parameters.Add(sp);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 接受SQL，返回首行首列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static object GetScalar(string sql)
        {
            object Rscalar = new object();
            SqlConnection con = Connection();
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            Rscalar = cmd.ExecuteScalar();
            con.Close();
            return Rscalar;
        }

        /// <summary>
        /// 接受SQL，返回首行首列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static async Task<object> GetScalarAsync(string sql)
        {
            var res = await Task.Factory.StartNew(() =>
            {
                object Rscalar = new object();
                SqlConnection con = Connection();
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                Rscalar = cmd.ExecuteScalar();
                con.Close();
                return Rscalar;
            });
            return res;
        }
        /// <summary>
        /// 执行存储过程，返回DataSet，重载1：无参数
        /// </summary>
        /// <param name="StoreName">存储过程名称</param>
        /// <returns></returns>
        public static DataSet ExecStoreDS(string StoreName)
        {
            SqlConnection con = Connection();
            SqlCommand cmd = new SqlCommand(StoreName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }

        /// <summary>
        /// 执行存储过程，返回DataSet，重载2：带参数
        /// </summary>
        /// <param name="StoreName">存储过程名称</param>
        /// <param name="paralist">所需参数集合</param>
        /// <returns></returns>
        public static DataSet ExecStoreDS(string StoreName, ref SqlParameter[] paralist)
        {
            SqlConnection con = Connection();
            SqlCommand cmd = new SqlCommand(StoreName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 999;

            foreach (SqlParameter sp in paralist)
            {
                cmd.Parameters.Add(sp);
            }
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
    }
}
