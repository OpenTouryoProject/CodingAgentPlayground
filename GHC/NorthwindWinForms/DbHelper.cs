using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    /// <summary>
    /// データベース接続の共通ヘルパークラス
    /// </summary>
    public static class DbHelper
    {
        private const string ConnectionName = "NorthwindConnection";

        /// <summary>
        /// App.configから接続文字列を取得する
        /// </summary>
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
        }

        /// <summary>
        /// SqlConnectionを生成するファクトリメソッド
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// SELECT疎通テスト。接続してSELECT 1を実行し、成功すればtrueを返す。
        /// </summary>
        public static bool TestConnection(out string message)
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        message = "接続成功: SELECT 1 = " + result.ToString();
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                message = "接続失敗: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 顧客一覧を取得する
        /// </summary>
        public static DataTable GetCustomers()
        {
            var dt = new DataTable();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT CustomerID, CompanyName, Phone, Address, City, Country FROM Customers ORDER BY CompanyName";
                using (var cmd = new SqlCommand(sql, conn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// 従業員一覧を取得する
        /// </summary>
        public static DataTable GetEmployees()
        {
            var dt = new DataTable();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT EmployeeID, LastName + ' ' + FirstName AS EmployeeName, Title, Extension FROM Employees ORDER BY LastName, FirstName";
                using (var cmd = new SqlCommand(sql, conn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// 商品一覧を取得する
        /// </summary>
        public static DataTable GetProducts()
        {
            var dt = new DataTable();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT ProductID, ProductName, UnitPrice, UnitsInStock FROM Products ORDER BY ProductName";
                using (var cmd = new SqlCommand(sql, conn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// 配送業者一覧を取得する
        /// </summary>
        public static DataTable GetShippers()
        {
            var dt = new DataTable();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var sql = "SELECT ShipperID, CompanyName, Phone FROM Shippers ORDER BY CompanyName";
                using (var cmd = new SqlCommand(sql, conn))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}
