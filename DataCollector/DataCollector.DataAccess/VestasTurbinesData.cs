using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace DAL
{
    public class VestasTurbinesData
    {
        string _connectionStringCE;
        string _connectionString;
        SqlConnection _connection;


        public VestasTurbinesData()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ScadaDBConnectionString"].ConnectionString;

            string connString = "Data Source=" + Environment.CurrentDirectory + "\\DB\\DataCollectorSettingsDB.sdf;Max Database Size=1024;Password=D@taC0llectorDB";
            string connString1 = "Data Source=  " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\DB\DataCollectorSettingsDB.sdf;Max Database Size=1024;Password=D@taC0llectorDB";
            _connectionStringCE = connString1;
        }

        public void OpenConnection()
        {
            _connection = new SqlConnection(_connectionString);

            _connection.Open();
        }

        public void CloseConnection()
        {
            if (_connection != null)
                _connection.Close();
        }

        private IEnumerable<TurbineInfo> GetTurbinesIds()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand { Connection = conn, CommandType = System.Data.CommandType.Text })
                    {
                        command.CommandText = "Select park.ParkUnitSerialNumber, * from TParkUnit as park " +
                                            "Join TUnitType as unitType on park.UnitTypeId = unitType.UnitTypeId " +
                                            "Join TControllerUnit as controller on unitType.ControllerUnitId = controller.ControllerUnitId " +
                                            "Where UnitTypeDevice = 'WTG' and UnitTypeManufacturer = 'Vestas Wind Systems'";
                                          //  "Where controller.ControllerUnitId = 28";

                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        //  var t = (from d in dt.AsEnumerable() select d.Field<int>("id"));
                        List<TurbineInfo> ids = new List<TurbineInfo>();
                        foreach (DataRow item in dt.Rows)
                        {
                            var turbineInfo = new TurbineInfo() { TurbineId = Convert.ToInt32(item["ParkUnitSerialNumber"]) };
                            ids.Add(turbineInfo);
                        }
                        return ids.ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<TurbineInfo>();
                }
            }
        }

        private IEnumerable<IEnumerable<TurbineInfo>> GetTableNamesForTurbines(IEnumerable<TurbineInfo> turbinesIds)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand { Connection = conn, CommandType = System.Data.CommandType.Text })
                    {
                        List<List<TurbineInfo>> globalDT = new List<List<TurbineInfo>>();

                        foreach (TurbineInfo turbineIdRow in turbinesIds)
                        {
                            var partialTurbineTableName = "T_" + turbineIdRow.TurbineId;

                            command.CommandText = "SELECT TABLE_NAME " +
                                           "FROM INFORMATION_SCHEMA.TABLES " +
                                           "WHERE " +
                                           "TABLE_TYPE = 'BASE TABLE' AND " +
                                           "TABLE_CATALOG = 'WindMan' " +
                                           "And TABLE_NAME like '%'+ @turbineTableName +'%'";

                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@turbineTableName", partialTurbineTableName);
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            DataTable dt = new DataTable();
                            da.Fill(dt);


                            List<TurbineInfo> turbineInfos = new List<TurbineInfo>();
                            foreach (DataRow item in dt.Rows)
                            {
                                TurbineInfo info = new TurbineInfo() { TurbineId = turbineIdRow.TurbineId, TableName = item["TABLE_NAME"].ToString() };
                                turbineInfos.Add(info);
                            }


                            globalDT.Add(turbineInfos);
                        }

                        return globalDT;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<List<TurbineInfo>>();
                }
            }
        }

        private IEnumerable<IEnumerable<TurbineInfo>> GetLastDataTimeStamp(IEnumerable<IEnumerable<TurbineInfo>> tableNames)
        {
            List<Dictionary<TurbineInfo, DateTime>> dict = new List<Dictionary<TurbineInfo, DateTime>>();
            foreach (var group in tableNames)
            {
                int r = -2;
                Dictionary<TurbineInfo, DateTime> groupDict = new Dictionary<TurbineInfo, DateTime>();
                foreach (var item in group)
                {
                    if (r < 0)
                        item.TimeStamp = new DateTime(2013, 10, 08, 10, 00, 00);
                    else
                        item.TimeStamp = new DateTime(2013, 10, 08, 10, 30, 00);


                    r = -r;
                    //Get last TimeStamp for this table
                    //groupDict.Add(item, new DateTime(2013, 10, 10, 10, 00, 00));
                }
                //dict.Add(groupDict);
            }

            return tableNames;
        }

        private Dictionary<string, DateTime> GetTimeStampForTable()
        {
            using (SqlCeConnection connCE = new SqlCeConnection(_connectionStringCE))
            {
                connCE.Open();
                using (SqlCeCommand command = new SqlCeCommand() { Connection = connCE })
                {
                    command.CommandText = "SELECT TableName, TimeStamp from TurbineDataTimeStamp";

                    SqlCeDataAdapter da = new SqlCeDataAdapter();
                    da.SelectCommand = command;

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    Dictionary<string, DateTime> dict = new Dictionary<string, DateTime>();
                    //This is quicker than linq
                    //foreach (DataRow item in dt.Rows)
                    //    dict.Add(item["TableName"].ToString(), DateTime.Parse(item["TimeStamp"].ToString()));
                   
                    var t = dt.AsEnumerable().ToDictionary<DataRow, string, DateTime>(row => row.Field<string>(0), row => row.Field<DateTime>(1));
                   
                    return dict;
                }
            }
        }

        public IEnumerable<IEnumerable<TurbineInfo>> GetTurbineList()
        {
            var ids = GetTurbinesIds();
            var tableNames = GetTableNamesForTurbines(ids);
            var stampsFromLastDownladedData = GetTimeStampForTable();
            AssingLastTimeStamp(tableNames, stampsFromLastDownladedData);
            var tableNamesWithTimeStamp = GetLastDataTimeStamp(tableNames);

            return tableNamesWithTimeStamp;
        }

        private void AssingLastTimeStamp(IEnumerable<IEnumerable<TurbineInfo>> tableNames, Dictionary<string, DateTime> lastDownloadedStamps)
        {
            foreach (var turbineTables in tableNames)
            {
                foreach (var table in turbineTables)
                {
                    if (lastDownloadedStamps.ContainsKey(table.TableName))
                    {
                        table.TimeStamp = lastDownloadedStamps[table.TableName];
                    }
                }
            }
        }

        public IEnumerable<DataTable> GetDataFromTurbinesWithOpenedConnection(IEnumerable<TurbineInfo> turbinesNamesWithTimeStamps)
        {
            try
            {
                using (SqlCommand command = new SqlCommand { Connection = _connection, CommandType = CommandType.Text })
                {
                    List<DataTable> globalDT = new List<DataTable>();

                    foreach (TurbineInfo turbineTable in turbinesNamesWithTimeStamps)
                    {
                        command.CommandText = "SELECT '" + turbineTable.TableName + "' as [tableName], * from " + turbineTable.TableName +
                                              " WHERE PCTimeStamp = @lastTimeStamp " +
                                              " ORDER BY PCTimeStamp Desc";

                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@lastTimeStamp", turbineTable.TimeStamp);

                        SqlDataAdapter da = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            globalDT.Add(dt);
                    }

                    return globalDT;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DataTable>();
            }
        }

        public IEnumerable<DataTable> GetDataFromTurbines(IEnumerable<TurbineInfo> turbinesNamesWithTimeStamps)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand { Connection = conn, CommandType = CommandType.Text })
                    {
                        List<DataTable> globalDT = new List<DataTable>();

                        foreach (TurbineInfo turbineTable in turbinesNamesWithTimeStamps)
                        {
                            command.CommandText = "SELECT '" + turbineTable.TableName + "' as [tableName], * from " + turbineTable.TableName +
                                                  " WHERE PCTimeStamp = @lastTimeStamp " +
                                                  " ORDER BY PCTimeStamp Desc";

                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@lastTimeStamp", turbineTable.TimeStamp);

                            SqlDataAdapter da = new SqlDataAdapter(command);
                            DataTable dt = new DataTable();

                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                                globalDT.Add(dt);
                        }

                        return globalDT;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<DataTable>();
                }
            }

        }

        public void SetTimeStampsForTables(IEnumerable<TurbineInfo> tables)
        {
            using (SqlCeConnection connCE = new SqlCeConnection(_connectionStringCE))
            {
                connCE.Open();
                using (SqlCeCommand command = new SqlCeCommand() { Connection = connCE })
                {

                    foreach (TurbineInfo table in tables)
                    {
                        command.CommandText = "UPDATE TurbineDataTimeStamp Set TimeStamp = @timeStamp WHERE TableName = @tableName";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@timeStamp", table.TimeStamp);
                        command.Parameters.AddWithValue("@tableName", table.TableName);

                        var result = command.ExecuteNonQuery();
                        if ((int)result < 1)
                        {
                            command.CommandText = "INSERT INTO TurbineDataTimeStamp(TableName, TimeStamp) VALUES (@tableName, @timeStamp)";
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }

  
}
