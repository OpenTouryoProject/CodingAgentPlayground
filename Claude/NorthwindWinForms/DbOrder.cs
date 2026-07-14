using System;
using System.Data;
using System.Data.SqlClient;

namespace NorthwindWinForms
{
    /// <summary>
    /// 受注（Orders）・受注明細（Order Details）に関するデータアクセスを提供するクラス。
    /// ADO.NET の DataTable を使用し、更新は単一トランザクションで実行する
    /// （DataAdapter.Update は使用しない）。
    /// </summary>
    internal static class DbOrder
    {
        // ---- 受注データ取得（ステップ1-2） ----

        /// <summary>
        /// 受注ID を指定して Orders テーブルから受注情報（1件）を取得する。
        /// </summary>
        /// <param name="orderId">受注ID</param>
        /// <returns>受注情報を格納した DataTable（該当なしの場合は 0 行）</returns>
        public static DataTable GetOrder(int orderId)
        {
            const string sql =
                "SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, " +
                "       ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, " +
                "       ShipPostalCode, ShipCountry " +
                "FROM Orders WHERE OrderID = @OrderID";

            DataTable table = new DataTable();
            using (SqlConnection connection = DbHelper.CreateConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@OrderID", orderId);
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        /// <summary>
        /// 受注ID を指定して Order Details テーブルから受注明細情報を取得する。
        /// 明細合計金額（(UnitPrice - 割引額) * Quantity）を計算列として付与する。
        /// </summary>
        /// <param name="orderId">受注ID</param>
        /// <returns>受注明細を格納した DataTable</returns>
        public static DataTable GetOrderDetails(int orderId)
        {
            const string sql =
                "SELECT OrderID, ProductID, UnitPrice, Quantity, Discount " +
                "FROM [Order Details] WHERE OrderID = @OrderID";

            DataTable table = new DataTable();
            using (SqlConnection connection = DbHelper.CreateConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@OrderID", orderId);
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    // 主キー情報を取得して DataTable に反映（RowState 判定を安定させる）
                    adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    adapter.Fill(table);
                }
            }
            return table;
        }

        // ---- 受注データ更新（ステップ1-3） ----

        /// <summary>
        /// 受注情報と受注明細を単一トランザクションで更新する。
        /// 受注（Orders）は UPDATE、受注明細（Order Details）は DataTable の RowState に応じて
        /// INSERT / UPDATE / DELETE を実行する。
        /// </summary>
        /// <param name="orderRow">更新対象の受注情報を保持する DataRow（Orders）</param>
        /// <param name="orderDetails">受注明細の DataTable（追加/更新/削除を含む）</param>
        public static void UpdateOrder(DataRow orderRow, DataTable orderDetails)
        {
            if (orderRow == null)
            {
                throw new ArgumentNullException("orderRow");
            }
            if (orderDetails == null)
            {
                throw new ArgumentNullException("orderDetails");
            }

            using (SqlConnection connection = DbHelper.CreateConnection())
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        UpdateOrderHeader(connection, transaction, orderRow);
                        SaveOrderDetails(connection, transaction, orderDetails);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 受注情報（Orders テーブル）を UPDATE する。
        /// </summary>
        private static void UpdateOrderHeader(SqlConnection connection, SqlTransaction transaction, DataRow row)
        {
            const string sql =
                "UPDATE Orders SET " +
                "  CustomerID = @CustomerID, EmployeeID = @EmployeeID, " +
                "  OrderDate = @OrderDate, RequiredDate = @RequiredDate, ShippedDate = @ShippedDate, " +
                "  ShipVia = @ShipVia, Freight = @Freight, " +
                "  ShipName = @ShipName, ShipAddress = @ShipAddress, ShipCity = @ShipCity, " +
                "  ShipRegion = @ShipRegion, ShipPostalCode = @ShipPostalCode, ShipCountry = @ShipCountry " +
                "WHERE OrderID = @OrderID";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
                command.Parameters.AddWithValue("@EmployeeID", row["EmployeeID"]);
                command.Parameters.AddWithValue("@OrderDate", row["OrderDate"]);
                command.Parameters.AddWithValue("@RequiredDate", row["RequiredDate"]);
                command.Parameters.AddWithValue("@ShippedDate", row["ShippedDate"]);
                command.Parameters.AddWithValue("@ShipVia", row["ShipVia"]);
                command.Parameters.AddWithValue("@Freight", row["Freight"]);
                command.Parameters.AddWithValue("@ShipName", row["ShipName"]);
                command.Parameters.AddWithValue("@ShipAddress", row["ShipAddress"]);
                command.Parameters.AddWithValue("@ShipCity", row["ShipCity"]);
                command.Parameters.AddWithValue("@ShipRegion", row["ShipRegion"]);
                command.Parameters.AddWithValue("@ShipPostalCode", row["ShipPostalCode"]);
                command.Parameters.AddWithValue("@ShipCountry", row["ShipCountry"]);
                command.Parameters.AddWithValue("@OrderID", row["OrderID"]);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注明細（Order Details テーブル）を RowState に応じて INSERT / UPDATE / DELETE する。
        /// </summary>
        private static void SaveOrderDetails(SqlConnection connection, SqlTransaction transaction, DataTable details)
        {
            foreach (DataRow row in details.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        InsertOrderDetail(connection, transaction, row);
                        break;
                    case DataRowState.Modified:
                        UpdateOrderDetail(connection, transaction, row);
                        break;
                    case DataRowState.Deleted:
                        DeleteOrderDetail(connection, transaction, row);
                        break;
                    // Unchanged / Detached は処理不要
                }
            }
        }

        /// <summary>受注明細を INSERT する。</summary>
        private static void InsertOrderDetail(SqlConnection connection, SqlTransaction transaction, DataRow row)
        {
            const string sql =
                "INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount) " +
                "VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@OrderID", row["OrderID"]);
                command.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                command.Parameters.AddWithValue("@UnitPrice", row["UnitPrice"]);
                command.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                command.Parameters.AddWithValue("@Discount", row["Discount"]);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注明細を UPDATE する。主キー（OrderID, ProductID）は変更前の値で特定する。
        /// </summary>
        private static void UpdateOrderDetail(SqlConnection connection, SqlTransaction transaction, DataRow row)
        {
            const string sql =
                "UPDATE [Order Details] SET " +
                "  UnitPrice = @UnitPrice, Quantity = @Quantity, Discount = @Discount " +
                "WHERE OrderID = @OrderID AND ProductID = @ProductID";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@UnitPrice", row["UnitPrice"]);
                command.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                command.Parameters.AddWithValue("@Discount", row["Discount"]);
                command.Parameters.AddWithValue("@OrderID", row["OrderID", DataRowVersion.Original]);
                command.Parameters.AddWithValue("@ProductID", row["ProductID", DataRowVersion.Original]);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注明細を DELETE する。削除済み行のため主キーは Original 値を参照する。
        /// </summary>
        private static void DeleteOrderDetail(SqlConnection connection, SqlTransaction transaction, DataRow row)
        {
            const string sql =
                "DELETE FROM [Order Details] " +
                "WHERE OrderID = @OrderID AND ProductID = @ProductID";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@OrderID", row["OrderID", DataRowVersion.Original]);
                command.Parameters.AddWithValue("@ProductID", row["ProductID", DataRowVersion.Original]);
                command.ExecuteNonQuery();
            }
        }
    }
}
