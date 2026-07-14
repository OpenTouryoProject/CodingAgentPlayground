using System;
using System.Configuration;
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
    }
}
