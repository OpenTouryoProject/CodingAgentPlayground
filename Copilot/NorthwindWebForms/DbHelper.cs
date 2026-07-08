using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1
{
    /// <summary>
    /// データベース接続の共通ヘルパークラス
    /// </summary>
    public static class DbHelper
    {
        /// <summary>
        /// Web.config から NorthwindConnection 接続文字列を取得する
        /// </summary>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["NorthwindConnection"].ConnectionString;
        }

        /// <summary>
        /// SqlConnection を生成して返す
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// SELECT 疎通テストを実行し、結果メッセージを返す
        /// </summary>
        public static string TestConnection()
        {
            try
            {
                using (SqlConnection conn = CreateConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers", conn))
                    {
                        int count = (int)cmd.ExecuteScalar();
                        return string.Format("接続成功: Customers テーブルに {0} 件のレコードがあります。", count);
                    }
                }
            }
            catch (Exception ex)
            {
                return "接続失敗: " + ex.Message;
            }
        }

        /// <summary>
        /// 顧客一覧を取得する
        /// </summary>
        public static DataTable GetCustomers()
        {
            using (SqlConnection conn = CreateConnection())
            {
                conn.Open();
                string sql = "SELECT CustomerID, CompanyName, ContactName, Phone, Address, City, Country FROM Customers ORDER BY CompanyName";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 従業員一覧を取得する
        /// </summary>
        public static DataTable GetEmployees()
        {
            using (SqlConnection conn = CreateConnection())
            {
                conn.Open();
                string sql = "SELECT EmployeeID, LastName + ' ' + FirstName AS EmployeeName, Title, Extension FROM Employees ORDER BY LastName, FirstName";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 商品一覧を取得する
        /// </summary>
        public static DataTable GetProducts()
        {
            using (SqlConnection conn = CreateConnection())
            {
                conn.Open();
                string sql = "SELECT ProductID, ProductName, UnitPrice, UnitsInStock FROM Products WHERE Discontinued = 0 ORDER BY ProductName";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 配送業者一覧を取得する
        /// </summary>
        public static DataTable GetShippers()
        {
            using (SqlConnection conn = CreateConnection())
            {
                conn.Open();
                string sql = "SELECT ShipperID, CompanyName, Phone FROM Shippers ORDER BY CompanyName";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
