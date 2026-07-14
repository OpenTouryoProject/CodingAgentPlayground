using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NorthwindWinForms
{
    /// <summary>
    /// データベース接続に関する共通処理を提供するヘルパークラス。
    /// App.config の connectionStrings セクションから接続文字列を取得し、
    /// SqlConnection の生成や疎通テストを行う。
    /// </summary>
    internal static class DbHelper
    {
        /// <summary>App.config に定義した接続文字列の名前。</summary>
        private const string ConnectionStringName = "NorthwindConnection";

        /// <summary>
        /// App.config から NorthWind 接続文字列を取得する。
        /// </summary>
        /// <returns>接続文字列</returns>
        /// <exception cref="ConfigurationErrorsException">接続文字列が未設定の場合。</exception>
        public static string GetConnectionString()
        {
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings[ConnectionStringName];

            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "App.config の connectionStrings に '" + ConnectionStringName +
                    "' が設定されていません。");
            }

            return settings.ConnectionString;
        }

        /// <summary>
        /// 接続文字列から SqlConnection を生成するファクトリ・メソッド。
        /// 接続は開かない（呼び出し側で Open すること）。
        /// </summary>
        /// <returns>未オープンの SqlConnection</returns>
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// SELECT 疎通テスト。DB へ接続し Customers テーブルの件数を取得する。
        /// </summary>
        /// <returns>Customers テーブルの件数</returns>
        public static int TestConnection()
        {
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Customers", connection))
            {
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// SELECT 文を実行し、結果を DataTable として取得する共通メソッド。
        /// </summary>
        /// <param name="sql">SELECT 文</param>
        /// <param name="parameters">SQL パラメータ（省略可）</param>
        /// <returns>結果を格納した DataTable</returns>
        private static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = CreateConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        // ---- マスタデータ取得（ステップ1-1） ----

        /// <summary>
        /// 顧客一覧 (Customers) を取得する。ドロップダウンおよび関連情報表示に使用。
        /// </summary>
        public static DataTable GetCustomers()
        {
            const string sql =
                "SELECT CustomerID, CompanyName, ContactName, Phone, " +
                "       Address, City, Region, PostalCode, Country " +
                "FROM Customers ORDER BY CompanyName";
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 従業員一覧 (Employees) を取得する。氏名・職位・内線番号を含む。
        /// </summary>
        public static DataTable GetEmployees()
        {
            const string sql =
                "SELECT EmployeeID, (LastName + ' ' + FirstName) AS FullName, " +
                "       Title, Extension " +
                "FROM Employees ORDER BY LastName, FirstName";
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 商品一覧 (Products) を取得する。単価・在庫を含む。
        /// </summary>
        public static DataTable GetProducts()
        {
            const string sql =
                "SELECT ProductID, ProductName, UnitPrice, UnitsInStock, Discontinued " +
                "FROM Products ORDER BY ProductName";
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 配送業者一覧 (Shippers) を取得する。社名・電話番号を含む。
        /// </summary>
        public static DataTable GetShippers()
        {
            const string sql =
                "SELECT ShipperID, CompanyName, Phone " +
                "FROM Shippers ORDER BY CompanyName";
            return ExecuteQuery(sql);
        }
    }
}
