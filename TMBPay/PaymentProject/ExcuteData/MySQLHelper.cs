using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace PaymentProject
{
    public class MySQLHelper
    {
        #region Member
        MySqlConnection connection = new MySqlConnection();
        MySqlTransaction transaction = null;
        int commandTimeOut = 60;
        #endregion

        #region Property
        /// <summary>
        /// SqlConnection
        /// </summary>
        public MySqlConnection SqlConnection
        {
            get { return connection; }
        }

        /// <summary>
        /// SqlTransaction
        /// </summary>
        public MySqlTransaction SqlTransaction
        {
            get
            {
                if (transaction == null || transaction.Connection == null)
                {
                    transaction = connection.BeginTransaction();
                }
                return transaction;
            }
        }

        /// <summary>
        /// Config time for excute command.
        /// </summary>
        public int CommandTimeOut
        {
            get { return commandTimeOut; }
            set { commandTimeOut = value; }
        }
        #endregion

        #region OpenConnection
        /// <summary>
        /// Connect to database.
        /// </summary>
        /// <returns>SqlConnection</returns>
        public MySqlConnection OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }

                /*ตัวจริง*/
                //  connection.ConnectionString = "Data Source = 52.74.136.226;User Id =user_payment; password=#PayMent2018;   Database=tmbpromptpay;CHARSET=utf8;Convert Zero Datetime=True;";

                /*ตัวทดสอบ*/
                connection.ConnectionString = "Data Source =localhost;User Id =user_payment; password=#PayMent2016;   Database=tmbpromptpay;CHARSET=utf8;Convert Zero Datetime=True;";
                connection.Open();
                return connection;
            }
            catch (Exception er)
            {
                throw new Exception("Can't connect to DataBase\r\n" + er.Message);
            }
        }
        #endregion

        #region CloseConnection
        /// <summary>
        /// Close connection.
        /// </summary>
        public void CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
                MySqlConnection.ClearPool(connection);
            }
        }
        #endregion

        #region TransactionBegin
        /// <summary>
        /// Begin transaction.
        /// </summary>
        /// <returns>SqlTransaction</returns>
        public MySqlTransaction BeginTransaction()
        {
            if (connection.State == ConnectionState.Closed)
            {
                OpenConnection();
            }
            transaction = connection.BeginTransaction();
            return transaction;
        }
        #endregion

        #region TransactionCommit
        /// <summary>
        /// Commit transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (transaction != null && transaction.Connection != null)
            {
                transaction.Commit();
            }
        }
        #endregion

        #region TransactionRollback
        /// <summary>
        /// Rollback transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (transaction != null && transaction.Connection != null)
            {
                transaction.Rollback();
            }
        }
        #endregion

        #region Insert-Update-Delete Data
        public int InsertRow(DataRow drRowUpdate, string strSpInsert)
        {
            return UpdateData(drRowUpdate, strSpInsert, "", "");
        }

        public int UpdateRow(DataRow drRowUpdate, string strSpUpdate)
        {
            return UpdateData(drRowUpdate, "", strSpUpdate, "");
        }

        public int DeleteRow(DataRow drRowUpdate, string strSpDelete)
        {
            return UpdateData(drRowUpdate, "", "", strSpDelete);
        }

        public int UpdateData(DataRow drRowUpdate, string strSpInsert, string strSpUpdate, string strSpDelete)
        {
            try
            {
                MySqlCommand sqlCommand = null;
                //Create SqlCommand for Insert
                if (drRowUpdate.RowState == DataRowState.Added && !string.IsNullOrEmpty(strSpInsert))
                {
                    sqlCommand = CreateCommand(strSpInsert);
                    GetParameterBySqlCommand(sqlCommand);
                    foreach (MySqlParameter parameter in sqlCommand.Parameters)
                    {//Match Parameter
                        if (drRowUpdate.Table.Columns.Contains(parameter.ParameterName.Replace("@", "")))
                        {
                            //parameter.SourceColumn = parameter.ParameterName.Replace("@", "");
                            parameter.Value = drRowUpdate[parameter.ParameterName.Replace("@", "")];
                        }
                    }
                }
                //Create SqlCommand for Update
                if (drRowUpdate.RowState == DataRowState.Modified && !string.IsNullOrEmpty(strSpUpdate))
                {
                    sqlCommand = CreateCommand(strSpUpdate);
                    GetParameterBySqlCommand(sqlCommand);
                    foreach (MySqlParameter parameter in sqlCommand.Parameters)
                    {//Match Parameter
                        if (drRowUpdate.Table.Columns.Contains(parameter.ParameterName.Replace("@", "")))
                        {
                            //parameter.SourceColumn = parameter.ParameterName.Replace("@", "");
                            parameter.Value = drRowUpdate[parameter.ParameterName.Replace("@", "")];
                        }
                    }
                }
                //Create SqlCommand for Delete
                if (drRowUpdate.RowState == DataRowState.Deleted && !string.IsNullOrEmpty(strSpDelete))
                {
                    sqlCommand = CreateCommand(strSpDelete);
                    GetParameterBySqlCommand(sqlCommand);
                    foreach (MySqlParameter parameter in sqlCommand.Parameters)
                    {//Match Parameter
                        if (drRowUpdate.Table.Columns.Contains(parameter.ParameterName.Replace("@", "")))
                        {
                            //parameter.SourceColumn = parameter.ParameterName.Replace("@", "");
                            parameter.Value = drRowUpdate[parameter.ParameterName.Replace("@", "")];
                        }
                    }
                }
                //Run update dataset
                if (sqlCommand != null) return sqlCommand.ExecuteNonQuery();
                else return -1;
            }
            catch (Exception er)
            {
                throw er;
            }
        }

        /// <summary>
        /// Update all rows in dataset to database.
        /// </summary>
        /// <param name="dataSource">Dataset to update</param>
        /// <param name="tableName">Datatable name to update</param>
        /// <param name="spInsert">StoredProcedure name for insert</param>
        /// <param name="spUpdate">StoredProcedure name for update</param>
        /// <param name="spDelete">StoredProcedure name for delete</param>
        public int UpdateData(DataSet dataSource, string tableName, string spInsert, string spUpdate, string spDelete)
        {
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                //Create SqlCommand for Insert
                if (!string.IsNullOrEmpty(spInsert))
                {
                    MySqlCommand commandInsert = CreateCommand(spInsert);
                    GetParameterBySqlCommand(commandInsert);
                    foreach (MySqlParameter parameter in commandInsert.Parameters)
                    {//Match Parameter
                        if (dataSource.Tables[tableName].Columns.Contains(parameter.ParameterName.Replace("@", "")))
                        {
                            parameter.SourceColumn = parameter.ParameterName.Replace("@", "");
                        }
                    }
                    adapter.InsertCommand = commandInsert;
                }
                //Create SqlCommand for Update
                if (!string.IsNullOrEmpty(spUpdate))
                {
                    MySqlCommand commandUpdate = CreateCommand(spUpdate);
                    GetParameterBySqlCommand(commandUpdate);
                    foreach (MySqlParameter parameter in commandUpdate.Parameters)
                    {//Match Parameter
                        if (dataSource.Tables[tableName].Columns.Contains(parameter.ParameterName.Replace("@", "")))
                        {
                            parameter.SourceColumn = parameter.ParameterName.Replace("@", "");
                        }
                    }
                    adapter.UpdateCommand = commandUpdate;
                }
                //Create SqlCommand for Delete
                if (!string.IsNullOrEmpty(spDelete))
                {
                    MySqlCommand commandDelete = CreateCommand(spDelete);
                    GetParameterBySqlCommand(commandDelete);
                    foreach (MySqlParameter parameter in commandDelete.Parameters)
                    {//Match Parameter
                        if (dataSource.Tables[tableName].Columns.Contains(parameter.ParameterName.Replace("@", "")))
                        {
                            parameter.SourceColumn = parameter.ParameterName.Replace("@", "");
                        }
                    }
                    adapter.DeleteCommand = commandDelete;
                }
                //Run update dataset
                return adapter.Update(dataSource, tableName);
            }
            catch (Exception er)
            {
                throw er;
            }
        }
        #endregion

        #region FillData
        public void FillData(string spName, System.Data.DataSet dataSource, params string[] tableNames)
        {
            FillData(spName, dataSource, tableNames, null);
        }

        public void FillData(string spName, System.Data.DataSet dataSource, string tableName, params MySqlParameter[] parameters)
        {
            FillData(spName, dataSource, new string[] { tableName }, parameters);
        }

        public void FillData(string spName, System.Data.DataSet dataSource, string[] tableNames, params MySqlParameter[] parameters)
        {
            if (dataSource == null) throw new ArgumentNullException("");
            if (string.IsNullOrEmpty(spName) || tableNames.Length == 0) throw new ArgumentException("");

            try
            {
                bool isOpenConnection = false;
                if (connection == null || connection.State == ConnectionState.Closed)
                {
                    OpenConnection();
                    isOpenConnection = true;
                }
                MySqlCommand command = CreateCommand(spName, CommandType.StoredProcedure);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                string tableName = "Table";
                for (int index = 0; index < tableNames.Length; index++)
                {
                    if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                    dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                    tableName += (index + 1).ToString();
                }
                dataAdapter.Fill(dataSource);
                if (isOpenConnection) CloseConnection();
            }
            catch (Exception er)
            {
                throw er;
            }
        }

        public void FillData(string query, CommandType commandType, System.Data.DataSet dataSource, string tableName)
        {
            try
            {
                MySqlCommand command = CreateCommand(query, CommandType.Text);
                MySqlDataAdapter adap = new MySqlDataAdapter();
                adap.SelectCommand = command;
                adap.Fill(dataSource, tableName);
            }
            catch
            {

            }
        }
        #endregion

        #region ExecuteQeury
        public int ExecuteQeury(string spName, params MySqlParameter[] parameters)
        {
            try
            {
                MySqlCommand command = CreateCommand(spName);
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
            catch (Exception er)
            {
                throw er;
            }
        }
        #endregion

        #region ExecuteQueryString
        /// <summary>
        /// Execute query.
        /// </summary>
        /// <param name="qeury">Query string to execute</param>
        /// <returns>Dataset contrin data</returns>
        public DataSet ExecuteQueryString(string qeury)
        {
            try
            {
                DataSet ds = new DataSet();
                if (!string.IsNullOrEmpty(qeury))
                {
                    bool isOpenConnection = false;
                    if (connection == null || connection.State == ConnectionState.Closed)
                    {
                        OpenConnection();
                        isOpenConnection = true;
                    }
                    MySqlCommand command = CreateCommand(qeury, CommandType.Text);
                    MySqlDataAdapter adap = new MySqlDataAdapter(command);
                    adap.Fill(ds);
                    if (isOpenConnection)
                    {
                        CloseConnection();
                        //  MySqlConnection.ClearAllPools();  
                    }
                }
                return ds;
            }
            catch (Exception er)
            {
                throw er;
            }

        }
        #endregion

        #region GetParameterBySqlCommand
        private void GetParameterBySqlCommand(MySqlCommand command)
        {
            if (command != null && command.Connection != null && command.Connection.State == ConnectionState.Open)
            {
                MySqlCommandBuilder.DeriveParameters(command);
            }
        }
        #endregion

        #region CreateCommand
        private MySqlCommand CreateCommand(string commandText)
        {
            return CreateCommand(commandText, CommandType.StoredProcedure);
        }

        private MySqlCommand CreateCommand(string commandText, CommandType type)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = commandText;
            command.CommandType = type;
            command.CommandTimeout = commandTimeOut;
            command.Connection = connection;
            command.Transaction = transaction;
            return command;
        }
        #endregion

        public String ChangeDateTimetoStringyyyyMMdd(DateTime datechange)
        {
            return datechange.ToString("yyyyMMdd", new System.Globalization.CultureInfo("th-TH"));
        }

        public DateTime DateTimetoThai(DateTime datechange)
        {
            string ls_Date = datechange.ToString("ddMMyyyy", new System.Globalization.CultureInfo("th-TH"));
            Int32 ld_Datetemp = Convert.ToInt32(ls_Date) + 543;
            string lsdate = ld_Datetemp.ToString();
            lsdate = lsdate.Substring(0, 2) + "/" + lsdate.Substring(2, 2) + "/" + lsdate.Substring(4, 4);
            DateTime ld_Date = Convert.ToDateTime(lsdate);
            return ld_Date;
        }


        public DataTable MergeTables(DataTable t1, DataTable t2)
        {
            if (t1 == null || t2 == null) throw new ArgumentNullException("t1 or t2", "Both tables must not be null");

            DataTable t3 = t1.Clone();  // first add columns from table1
            foreach (DataColumn col in t2.Columns)
            {
                string newColumnName = col.ColumnName;
                int colNum = 1;
                while (t3.Columns.Contains(newColumnName))
                {
                    newColumnName = string.Format("{0}_{1}", col.ColumnName, ++colNum);
                }
                t3.Columns.Add(newColumnName, col.DataType);
            }
            var mergedRows = t1.AsEnumerable().Zip(t2.AsEnumerable(),
                (r1, r2) => r1.ItemArray.Concat(r2.ItemArray).ToArray());
            foreach (object[] rowFields in mergedRows)
                t3.Rows.Add(rowFields);

            return t3;
        }

    }
}
