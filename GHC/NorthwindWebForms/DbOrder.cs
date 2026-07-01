using System;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1
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
            using (SqlConnection conn = DbHelper.CreateConnection())
            {
                conn.Open();
                string sql = @"SELECT o.OrderID, o.CustomerID, o.EmployeeID, o.OrderDate, o.RequiredDate, o.ShippedDate,
                                      o.ShipVia, o.Freight, o.ShipName, o.ShipAddress, o.ShipCity,
                                      o.ShipRegion, o.ShipPostalCode, o.ShipCountry
                               FROM Orders o
                               WHERE o.OrderID = @OrderID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 受注IDを指定して受注明細情報を取得する
        /// </summary>
        public static DataTable GetOrderDetails(int orderId)
        {
            using (SqlConnection conn = DbHelper.CreateConnection())
            {
                conn.Open();
                string sql = @"SELECT od.OrderID, od.ProductID, od.UnitPrice, od.Quantity, od.Discount,
                                      (od.UnitPrice - od.Discount) * od.Quantity AS LineTotal
                               FROM [Order Details] od
                               WHERE od.OrderID = @OrderID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 受注情報を更新する（Orders テーブル UPDATE）
        /// </summary>
        private static void UpdateOrder(SqlConnection conn, SqlTransaction tran,
            int orderId, string customerId, int employeeId,
            DateTime? orderDate, DateTime? requiredDate, DateTime? shippedDate,
            int shipVia, decimal freight, string shipName, string shipAddress,
            string shipCity, string shipRegion, string shipPostalCode, string shipCountry)
        {
            string sql = @"UPDATE Orders SET
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
            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                cmd.Parameters.AddWithValue("@OrderDate", (object)orderDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RequiredDate", (object)requiredDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippedDate", (object)shippedDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipVia", shipVia);
                cmd.Parameters.AddWithValue("@Freight", freight);
                cmd.Parameters.AddWithValue("@ShipName", (object)shipName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipAddress", (object)shipAddress ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipCity", (object)shipCity ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipRegion", (object)shipRegion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipPostalCode", (object)shipPostalCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipCountry", (object)shipCountry ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注情報を新規作成する（Orders テーブル INSERT）
        /// </summary>
        private static int InsertOrder(SqlConnection conn, SqlTransaction tran,
            string customerId, int employeeId,
            DateTime? orderDate, DateTime? requiredDate, DateTime? shippedDate,
            int shipVia, decimal freight, string shipName, string shipAddress,
            string shipCity, string shipRegion, string shipPostalCode, string shipCountry)
        {
            string sql = @"INSERT INTO Orders (CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate,
                            ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)
                           VALUES (@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate,
                            @ShipVia, @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry);
                           SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                cmd.Parameters.AddWithValue("@OrderDate", (object)orderDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RequiredDate", (object)requiredDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippedDate", (object)shippedDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipVia", shipVia);
                cmd.Parameters.AddWithValue("@Freight", freight);
                cmd.Parameters.AddWithValue("@ShipName", (object)shipName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipAddress", (object)shipAddress ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipCity", (object)shipCity ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipRegion", (object)shipRegion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipPostalCode", (object)shipPostalCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipCountry", (object)shipCountry ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// 受注明細を追加する（Order Details テーブル INSERT）
        /// </summary>
        private static void InsertOrderDetail(SqlConnection conn, SqlTransaction tran,
            int orderId, int productId, decimal unitPrice, short quantity, float discount)
        {
            string sql = @"INSERT INTO [Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount)
                           VALUES (@OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";
            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Discount", discount);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注明細を更新する（Order Details テーブル UPDATE）
        /// </summary>
        private static void UpdateOrderDetail(SqlConnection conn, SqlTransaction tran,
            int orderId, int productId, decimal unitPrice, short quantity, float discount)
        {
            string sql = @"UPDATE [Order Details] SET
                            UnitPrice = @UnitPrice,
                            Quantity = @Quantity,
                            Discount = @Discount
                           WHERE OrderID = @OrderID AND ProductID = @ProductID";
            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Discount", discount);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注明細を削除する（Order Details テーブル DELETE）
        /// </summary>
        private static void DeleteOrderDetail(SqlConnection conn, SqlTransaction tran,
            int orderId, int productId)
        {
            string sql = "DELETE FROM [Order Details] WHERE OrderID = @OrderID AND ProductID = @ProductID";
            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 受注全体の既存明細を削除する
        /// </summary>
        private static void DeleteAllOrderDetails(SqlConnection conn, SqlTransaction tran, int orderId)
        {
            string sql = "DELETE FROM [Order Details] WHERE OrderID = @OrderID";
            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 単一トランザクションで受注と受注明細を更新する
        /// dtDetails の各行: ProductID, UnitPrice, Quantity, Discount
        /// </summary>
        public static void SaveOrder(int orderId, string customerId, int employeeId,
            DateTime? orderDate, DateTime? requiredDate, DateTime? shippedDate,
            int shipVia, decimal freight, string shipName, string shipAddress,
            string shipCity, string shipRegion, string shipPostalCode, string shipCountry,
            DataTable dtDetails)
        {
            using (SqlConnection conn = DbHelper.CreateConnection())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 受注ヘッダの更新
                        UpdateOrder(conn, tran, orderId, customerId, employeeId,
                            orderDate, requiredDate, shippedDate,
                            shipVia, freight, shipName, shipAddress,
                            shipCity, shipRegion, shipPostalCode, shipCountry);

                        // 既存明細を全削除してから再挿入する方式
                        DeleteAllOrderDetails(conn, tran, orderId);

                        foreach (DataRow row in dtDetails.Rows)
                        {
                            InsertOrderDetail(conn, tran, orderId,
                                Convert.ToInt32(row["ProductID"]),
                                Convert.ToDecimal(row["UnitPrice"]),
                                Convert.ToInt16(row["Quantity"]),
                                Convert.ToSingle(row["Discount"]));
                        }

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
        /// 単一トランザクションで新規受注と受注明細を作成する
        /// dtDetails の各行: ProductID, UnitPrice, Quantity, Discount
        /// 戻り値: 新規作成された OrderID
        /// </summary>
        public static int CreateOrder(string customerId, int employeeId,
            DateTime? orderDate, DateTime? requiredDate, DateTime? shippedDate,
            int shipVia, decimal freight, string shipName, string shipAddress,
            string shipCity, string shipRegion, string shipPostalCode, string shipCountry,
            DataTable dtDetails)
        {
            using (SqlConnection conn = DbHelper.CreateConnection())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 受注ヘッダの挿入
                        int newOrderId = InsertOrder(conn, tran, customerId, employeeId,
                            orderDate, requiredDate, shippedDate,
                            shipVia, freight, shipName, shipAddress,
                            shipCity, shipRegion, shipPostalCode, shipCountry);

                        // 受注明細の挿入
                        foreach (DataRow row in dtDetails.Rows)
                        {
                            InsertOrderDetail(conn, tran, newOrderId,
                                Convert.ToInt32(row["ProductID"]),
                                Convert.ToDecimal(row["UnitPrice"]),
                                Convert.ToInt16(row["Quantity"]),
                                Convert.ToSingle(row["Discount"]));
                        }

                        tran.Commit();
                        return newOrderId;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
