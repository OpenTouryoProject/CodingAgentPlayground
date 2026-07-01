using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    /// <summary>
    /// 受注管理のデータアクセスクラス
    /// </summary>
    public static class DbOrder
    {
        /// <summary>
        /// 受注IDを指定して受注情報を取得する
        /// </summary>
        public static DataTable GetOrder(int orderId)
        {
            var dt = new DataTable();
            using (var conn = DbHelper.CreateConnection())
            {
                conn.Open();
                var sql = @"SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate,
                                   ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry
                            FROM Orders
                            WHERE OrderID = @OrderID";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 受注IDを指定して受注明細情報を取得する
        /// </summary>
        public static DataTable GetOrderDetails(int orderId)
        {
            var dt = new DataTable();
            using (var conn = DbHelper.CreateConnection())
            {
                conn.Open();
                var sql = @"SELECT od.OrderID, od.ProductID, p.ProductName, od.UnitPrice, od.Quantity, od.Discount,
                                   (od.UnitPrice - od.Discount) * od.Quantity AS LineTotal,
                                   p.UnitsInStock
                            FROM [Order Details] od
                            INNER JOIN Products p ON od.ProductID = p.ProductID
                            WHERE od.OrderID = @OrderID";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 受注情報を更新する（単一トランザクションで受注と受注明細を処理）
        /// </summary>
        public static void UpdateOrder(int orderId, string customerId, int employeeId,
            System.DateTime? orderDate, System.DateTime? requiredDate, System.DateTime? shippedDate,
            int shipVia, decimal freight, string shipName, string shipAddress,
            string shipCity, string shipRegion, string shipPostalCode, string shipCountry,
            DataTable orderDetails)
        {
            using (var conn = DbHelper.CreateConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 受注ヘッダの更新
                        var sqlOrder = @"UPDATE Orders SET
                                            CustomerID = @CustomerID,
                                            EmployeeID = @EmployeeID,
                                            OrderDate = @OrderDate,
                                            RequiredDate = @RequiredDate,
                                            ShippedDate = @ShippedDate,
                                            ShipVia = @ShipVia,
                                            Freight = @Freight,
                                            ShipName = @ShipName,
                                            ShipAddress = @ShipAddress,
                                            ShipCity = @ShipCity,
                                            ShipRegion = @ShipRegion,
                                            ShipPostalCode = @ShipPostalCode,
                                            ShipCountry = @ShipCountry
                                         WHERE OrderID = @OrderID";
                        using (var cmd = new SqlCommand(sqlOrder, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.Parameters.AddWithValue("@CustomerID", customerId);
                            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                            cmd.Parameters.AddWithValue("@OrderDate", (object)orderDate ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@RequiredDate", (object)requiredDate ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShippedDate", (object)shippedDate ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipVia", shipVia);
                            cmd.Parameters.AddWithValue("@Freight", freight);
                            cmd.Parameters.AddWithValue("@ShipName", (object)shipName ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipAddress", (object)shipAddress ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipCity", (object)shipCity ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipRegion", (object)shipRegion ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipPostalCode", (object)shipPostalCode ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipCountry", (object)shipCountry ?? System.DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }

                        // 既存の受注明細を全削除
                        var sqlDeleteDetails = "DELETE FROM [Order Details] WHERE OrderID = @OrderID";
                        using (var cmd = new SqlCommand(sqlDeleteDetails, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        // 受注明細を再挿入
                        InsertOrderDetails(conn, tran, orderId, orderDetails);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 受注情報を新規作成する（単一トランザクションで受注と受注明細を処理）
        /// </summary>
        public static int CreateOrder(string customerId, int employeeId,
            System.DateTime? orderDate, System.DateTime? requiredDate, System.DateTime? shippedDate,
            int shipVia, decimal freight, string shipName, string shipAddress,
            string shipCity, string shipRegion, string shipPostalCode, string shipCountry,
            DataTable orderDetails)
        {
            int newOrderId;
            using (var conn = DbHelper.CreateConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 受注ヘッダの挿入
                        var sqlOrder = @"INSERT INTO Orders (CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate,
                                            ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)
                                         VALUES (@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate,
                                            @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry);
                                         SELECT CAST(SCOPE_IDENTITY() AS int);";
                        using (var cmd = new SqlCommand(sqlOrder, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", customerId);
                            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                            cmd.Parameters.AddWithValue("@OrderDate", (object)orderDate ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@RequiredDate", (object)requiredDate ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShippedDate", (object)shippedDate ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipVia", shipVia);
                            cmd.Parameters.AddWithValue("@Freight", freight);
                            cmd.Parameters.AddWithValue("@ShipName", (object)shipName ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipAddress", (object)shipAddress ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipCity", (object)shipCity ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipRegion", (object)shipRegion ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipPostalCode", (object)shipPostalCode ?? System.DBNull.Value);
                            cmd.Parameters.AddWithValue("@ShipCountry", (object)shipCountry ?? System.DBNull.Value);
                            newOrderId = (int)cmd.ExecuteScalar();
                        }

                        // 受注明細を挿入
                        InsertOrderDetails(conn, tran, newOrderId, orderDetails);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
            return newOrderId;
        }

        /// <summary>
        /// 受注明細を挿入する（内部メソッド）
        /// </summary>
        private static void InsertOrderDetails(SqlConnection conn, SqlTransaction tran, int orderId, DataTable orderDetails)
        {
            var sqlInsertDetail = @"INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)
                                    VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";

            foreach (DataRow row in orderDetails.Rows)
            {
                using (var cmd = new SqlCommand(sqlInsertDetail, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                    cmd.Parameters.AddWithValue("@UnitPrice", row["UnitPrice"]);
                    cmd.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                    cmd.Parameters.AddWithValue("@Discount", row["Discount"]);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
