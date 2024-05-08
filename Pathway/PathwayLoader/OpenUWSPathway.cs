using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Pathway.Core.Abstract;
using Pathway.Core.Concrete;
using Pathway.Core.Infrastructure;
using Pathway.Core.Services;
using Pathway.Loader.Infrastructure;
using Pathway.Core.RemoteAnalyst.Concrete;
using log4net;

namespace Pathway.Loader {
    public class OpenUWSPathway : Header {
        private readonly string _connectionStringMain = "";
        private readonly string _connectionStringSystem = "";
        private readonly ILog _log;
        private readonly string _uwsUnzipPath = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uwsUnzipPath"> Full path of the unzipped data file. 
        /// For now we do not accept zipped file so uwsPath and uwsUnzipPath would have the same value</param>
        /// <param name="newFileLog"> Stream writer of the log file.</param>
        /// <param name="connectionString"> SQL Server Connection String.</param>
        /// <param name="connectionStringSystem"> SQL Server Connection String for Per System Database.</param>
        public OpenUWSPathway(string uwsUnzipPath, ILog log, string connectionString, string connectionStringSystem
#if DEBUG
            , string systemSerial
#endif
            ) {
            _uwsUnzipPath = uwsUnzipPath;
            _log = log;
            _connectionStringMain = connectionString;
            _connectionStringSystem = connectionStringSystem;
#if DEBUG
            UWSSerialNumber = systemSerial;
#endif
        }

        public void LoadPathwayTrend(long intervalInSec, DateTime fromTimestamp, DateTime toTimestamp)
        {
            #region Pathway Trend
            try
            {
                
                var cpu = new CPU(_connectionStringSystem);
                var cpuTableName = cpu.GetLatestTableName();
                var ipu = cpu.GetIPU(cpuTableName);

                IAllPathwayService service = new AllPathwayService(_connectionStringSystem, intervalInSec);

                _log.InfoFormat("collectionInfo.FromTimestamp: {0} ToTimestamp: {1} ", fromTimestamp, toTimestamp);
                _log.InfoFormat("intervalInSec: {0}, AllowTime: {1} ResponseTime: {2} ", intervalInSec,
                    System.Configuration.ConfigurationManager.AppSettings["AllowTime"],
                    System.Configuration.ConfigurationManager.AppSettings["ResponseTime"]);

                for (var start = fromTimestamp; start < toTimestamp; start = start.AddSeconds(intervalInSec))
                {
                    var cpuDetail = service.GetCPUBusyDetailFor(start, start.AddHours(1), UWSSerialNumber, ipu);

                    var trendDatas = new List<TrendData>();
                    foreach (var tempValues in cpuDetail.Select(x => x.Value).ToList())
                    {
                        foreach (var tempValue in tempValues)
                        {
                            if (trendDatas.Any(x => x.PathwayName.Equals(tempValue.PathwayName) && x.Interval.Equals(start)))
                            {
                                //Add the values.
                                var trendInfo = trendDatas.FirstOrDefault(x => x.PathwayName.Equals(tempValue.PathwayName));
                                trendInfo.CpuBusy += tempValue.Value;
                                if (tempValue.Value > trendInfo.PeakCPUBusy)
                                {
                                    trendInfo.PeakCPUBusy = tempValue.Value;
                                }
                                trendInfo.CpuCounter++;
                            }
                            else
                            {
                                if (!tempValue.PathwayName.Equals("Others"))
                                {
                                    //Create new entry.
                                    var trendData = new TrendData();
                                    trendData.Interval = start;
                                    trendData.PathwayName = tempValue.PathwayName;
                                    trendData.CpuBusy = tempValue.Value;
                                    trendData.CpuCounter = 1;
                                    trendData.PeakCPUBusy = tempValue.Value;
                                    trendDatas.Add(trendData);
                                }
                            }
                        }
                    }

                    foreach (var trendData in trendDatas)
                    {
                        trendData.CpuBusy = trendData.CpuBusy / trendData.CpuCounter;
                    }

                    var serverDetails = service.GetTransactionServer(start.AddSeconds(intervalInSec * (0.10 * -1))
                                                            , start.AddHours(1).AddSeconds(intervalInSec * 0.10));

                    foreach (var serverDetail in serverDetails.LinkmonToServer)
                    {
                        if (trendDatas.Any(x => x.PathwayName.Equals(serverDetail.Key) && x.Interval.Equals(start)))
                        {
                            var trendInfo = trendDatas.FirstOrDefault(x => x.PathwayName.Equals(serverDetail.Key));
                            trendInfo.ServerTransactions += serverDetail.Value.TotalIrReqCnt;
                            trendInfo.PeakLinkmonTransaction = serverDetail.Value.PeakReqCnt;
                            trendInfo.AverageLinkmonTransaction = serverDetail.Value.AverageReqCnt;
                        }
                        else
                        {
                            var trendData = new TrendData();
                            trendData.Interval = start;
                            trendData.PathwayName = serverDetail.Key;
                            trendData.CpuBusy = 0d;
                            trendData.PeakLinkmonTransaction = serverDetail.Value.PeakReqCnt;
                            trendData.AverageLinkmonTransaction = serverDetail.Value.AverageReqCnt;
                            trendDatas.Add(trendData);
                        }
                    }

                    foreach (var serverDetail in serverDetails.TcptoServer)
                    {
                        if (trendDatas.Any(x => x.PathwayName.Equals(serverDetail.Key) && x.Interval.Equals(start)))
                        {
                            var trendInfo = trendDatas.FirstOrDefault(x => x.PathwayName.Equals(serverDetail.Key));
                            trendInfo.ServerTransactions += serverDetail.Value.TotalIsReqCnt;
                            trendInfo.PeakTCPTransaction = serverDetail.Value.PeakReqCnt;
                            trendInfo.AverageTCPTransaction = serverDetail.Value.AverageReqCnt;
                        }
                        else
                        {
                            var trendData = new TrendData();
                            trendData.Interval = start;
                            trendData.PathwayName = serverDetail.Key;
                            trendData.CpuBusy = 0d;
                            trendData.PeakTCPTransaction = serverDetail.Value.PeakReqCnt;
                            trendData.AverageTCPTransaction = serverDetail.Value.AverageReqCnt;
                            trendDatas.Add(trendData);
                        }
                    }

                    _log.InfoFormat("start: {0}, start.AddHours(1): {1}, trendDatas {2}", start, start.AddHours(1), trendDatas.Count);

                    if (trendDatas.Count > 0)
                    {
                        var tableName = "TrendPathwayHourly";
                        var helper = new MySQLHelper(_connectionStringSystem);
                        var databaseName = helper.FindDatabaseName(_connectionStringSystem);
                        var trendCpuIntervalExist = helper.CheckMySqlTable(databaseName, "TrendPathwayHourly");

                        if (!trendCpuIntervalExist)
                        {
                            helper.CreateTrendPathwayHourly();
                        }
                        else
                        {
                            List<NewDataBaseColumn> newColumnList = new List<NewDataBaseColumn>();
                            var newColumn = new NewDataBaseColumn();
                            newColumn.TableName = tableName;
                            newColumn.ColumnName = "PeakCPUBusy";
                            newColumn.Query = "ADD COLUMN `" + newColumn.ColumnName + "` DOUBLE NULL AFTER `PathwayName`;";
                            newColumnList.Add(newColumn);

                            newColumn = new NewDataBaseColumn();
                            newColumn.TableName = tableName;
                            newColumn.ColumnName = "PeakLinkmonTransaction";
                            newColumn.Query = "ADD COLUMN `" + newColumn.ColumnName + "` DOUBLE DEFAULT NULL AFTER `CpuBusy`;";
                            newColumnList.Add(newColumn);

                            newColumn = new NewDataBaseColumn();
                            newColumn.TableName = tableName;
                            newColumn.ColumnName = "AverageLinkmonTransaction";
                            newColumn.Query = "ADD COLUMN `" + newColumn.ColumnName + "` DOUBLE DEFAULT NULL AFTER `PeakLinkmonTransaction`;";
                            newColumnList.Add(newColumn);

                            newColumn = new NewDataBaseColumn();
                            newColumn.TableName = tableName;
                            newColumn.ColumnName = "PeakTCPTransaction";
                            newColumn.Query = "ADD COLUMN `" + newColumn.ColumnName + "` DOUBLE DEFAULT NULL AFTER `AverageLinkmonTransaction`;";
                            newColumnList.Add(newColumn);

                            newColumn = new NewDataBaseColumn();
                            newColumn.TableName = tableName;
                            newColumn.ColumnName = "AverageTCPTransaction";
                            newColumn.Query = "ADD COLUMN `" + newColumn.ColumnName + "` DOUBLE DEFAULT NULL AFTER `PeakTCPTransaction`;";
                            newColumnList.Add(newColumn);

                            foreach (var column in newColumnList)
                            {
                                var columnExist = helper.CheckMySqlColumn(databaseName, column.TableName, column.ColumnName);
                                if (!columnExist) helper.AlterTable(column.TableName, column.Query);
                            }
                            //Alter table
                        }

                        //Insert data.
#if (DEBUG)
                        string path = @"";
#else
                        var fileInfo = new FileInfo(_uwsUnzipPath);
                        var path = fileInfo.Directory.FullName;
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
#endif

                        string pathToCsvSystemInterval = path + @"BulkInsertTrendPathwayHourly_" + DateTime.Now.Ticks + ".csv";
                        var sb = new StringBuilder();
                        foreach (var data in trendDatas)
                        {
                            sb.Append(data.Interval.ToString("yyyy-MM-dd HH:mm:ss") + "|" + data.PathwayName + "|" + data.PeakCPUBusy + "|" + data.CpuBusy + "|" + data.PeakLinkmonTransaction + "|" + data.AverageLinkmonTransaction + "|" + data.PeakTCPTransaction + "|" + data.AverageTCPTransaction + "|" + data.ServerTransactions + Environment.NewLine);
                        }
                        File.AppendAllText(pathToCsvSystemInterval, sb.ToString());

                        try
                        {
                            helper.InsertData(tableName, pathToCsvSystemInterval);
                            if (File.Exists(pathToCsvSystemInterval))
                                File.Delete(pathToCsvSystemInterval);
                        }
                        catch
                        {
                            //Try insert statment.
                            var sbInsert = new StringBuilder();
                            sbInsert.Append("INSERT INTO `trendpathwayhourly` (`Interval`, `PathwayName`, `PeakCPUBusy`, `CpuBusy`, `PeakLinkmonTransaction`, `AverageLinkmonTransaction`, `PeakTCPTransaction`, `AverageTCPTransaction`, `ServerTransaction`) VALUES ");
                            foreach (var data in trendDatas)
                            {
                                sbInsert.Append("('" + data.Interval.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                                "'" + data.PathwayName + "', " +
                                                data.CpuBusy + ", " +
                                                data.ServerTransactions + "),");
                            }

                            //Remove last ,
                            sbInsert = sbInsert.Remove(sbInsert.Length - 1, 1);
                            helper.InsertStatement(sbInsert.ToString());

                            if (File.Exists(pathToCsvSystemInterval))
                                File.Delete(pathToCsvSystemInterval);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Pathway Trend Error: {0}", ex);
            }
#endregion
        }

        /// <summary>
        /// Main function of this class.
        /// Call functions to read data and load data into database.
        /// </summary>
        /// <returns> Return a bool value suggests whether the load is successful or not.</returns>
        public CollectionInfo CreateNewData() {
            var collectionInfo = new CollectionInfo();

            _log.InfoFormat("Open UWS Header at: {0}", DateTime.Now);
            
            bool success = OpenUWSPathwayFile(_uwsUnzipPath);
            
            if (!success)
                return collectionInfo;

            _log.InfoFormat("Load Pathway at: {0}", DateTime.Now);
            
            DateTime beforeTime = DateTime.Now;
            CreatePathwayDataSet(_uwsUnzipPath, collectionInfo);

            _log.InfoFormat("Load Pathway Trend at: {0}", DateTime.Now);
            long intervalInSec;
            if (collectionInfo.IntervalType.Equals("H"))
                intervalInSec = collectionInfo.IntervalNumber * 3600;
            else
                intervalInSec = collectionInfo.IntervalNumber * 60;

            LoadPathwayTrend(intervalInSec, collectionInfo.FromTimestamp, collectionInfo.ToTimestamp);

            DateTime afterTime = DateTime.Now;
            TimeSpan span = afterTime - beforeTime;
            _log.InfoFormat("Total Pathway Load time in minutes: {0}", span.TotalMinutes);
            

            return collectionInfo;
        }

        /// <summary>
        ///  Open the UWS pathway file and load data into the variables that defined in Header.
        /// </summary>
        /// <param name="uwsPath"> Full path of the UWS pathway file.</param>
        /// <returns></returns>
        private bool OpenUWSPathwayFile(string uwsPath) {
            using (var stream = new FileStream(uwsPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                //using (StreamReader Reader = new StreamReader(stream))
                using (Reader = new BinaryReader(stream)) {
                    var myEncoding = new ASCIIEncoding();

#region Basic Header Info

                    //Identifier
                    Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    UwsIdentifierByte = Reader.ReadBytes(UwsIdentifierByte.Length);
                    UwsIdentifier = myEncoding.GetString(UwsIdentifierByte).Trim();

                    //Key.
                    Reader.BaseStream.Seek(10, SeekOrigin.Begin);
                    UwsKeyByte = Reader.ReadBytes(UwsKeyByte.Length);
                    UwsKey = myEncoding.GetString(UwsKeyByte).Trim();

                    //System Serial
                    Reader.BaseStream.Seek(10, SeekOrigin.Begin);
                    SystemSerialByte = Reader.ReadBytes(SystemSerialByte.Length);
                    UWSSerialNumber = myEncoding.GetString(SystemSerialByte).Trim();
                    UWSSerialNumber = UWSSerialNumber.Replace("\0", "");
                    while (UWSSerialNumber.Length < 6) {
                        UWSSerialNumber = "0" + UWSSerialNumber;
                    }
					
					/* Special handling for Ingenico */
					if (UWSSerialNumber == "078998") {
						UWSSerialNumber = "078831";
					}
                    //UwsHLen
                    Reader.BaseStream.Seek(36, SeekOrigin.Begin);
                    UwsHLen = Reader.ReadInt16();
                    UwsHLen = Helper.Reverse(UwsHLen);

                    //UwsHVersion
                    Reader.BaseStream.Seek(38, SeekOrigin.Begin);
                    UwsHVersion = Reader.ReadInt16();
                    UwsHVersion = Helper.Reverse(UwsHVersion);

                    //UwsXLen
                    Reader.BaseStream.Seek(40, SeekOrigin.Begin);
                    UwsXLen = Reader.ReadInt16();
                    UwsXLen = Helper.Reverse(UwsXLen);

                    //UwsXRecords
                    Reader.BaseStream.Seek(42, SeekOrigin.Begin);
                    UwsXRecords = Reader.ReadInt16();
                    UwsXRecords = Helper.Reverse(UwsXRecords);

                    //UwsSignatureTypeByte
                    Reader.BaseStream.Seek(66, SeekOrigin.Begin);
                    UwsSignatureTypeByte = Reader.ReadBytes(UwsSignatureTypeByte.Length);
                    UwsSignatureType = myEncoding.GetString(UwsSignatureTypeByte).Trim();

                    //UwsVersion
                    Reader.BaseStream.Seek(216, SeekOrigin.Begin);
                    UwsVersion = Reader.ReadInt32();
                    UwsVersion = Helper.Reverse(UwsVersion);

                    //UwsVstringByte
                    Reader.BaseStream.Seek(220, SeekOrigin.Begin);
                    UwsVstringByte = Reader.ReadBytes(UwsVstringByte.Length);
                    UwsVstring = myEncoding.GetString(UwsVstringByte).Trim();

                    //UwsSystemNameByte
                    Reader.BaseStream.Seek(284, SeekOrigin.Begin);
                    UwsSystemNameByte = Reader.ReadBytes(UwsSystemNameByte.Length);
                    UwsSystemName = myEncoding.GetString(UwsSystemNameByte).Trim();

                    //UwsGMTStartTimestamp
                    Reader.BaseStream.Seek(712, SeekOrigin.Begin);
                    UwsGmtStartTimestamp = Reader.ReadInt64();
                    UwsGmtStartTimestamp = Helper.Reverse(UwsGmtStartTimestamp);
                    //Need to do / 10000 to get current julian time
                    UwsGmtStartTimestamp /= 10000;

                    //UwsGMTStopTimestamp
                    Reader.BaseStream.Seek(720, SeekOrigin.Begin);
                    UwsGmtStopTimestamp = Reader.ReadInt64();
                    UwsGmtStopTimestamp = Helper.Reverse(UwsGmtStopTimestamp);
                    //Need to do / 10000 to get current julian time
                    UwsGmtStopTimestamp /= 10000;

                    //UwsLCTStartTimestamp
                    Reader.BaseStream.Seek(728, SeekOrigin.Begin);
                    UwsLctStartTimestamp = Reader.ReadInt64();
                    UwsLctStartTimestamp = Helper.Reverse(UwsLctStartTimestamp);
                    //Need to do / 10000 to get current julian time
                    UwsLctStartTimestamp /= 10000;

                    //UwsSampleInterval
                    Reader.BaseStream.Seek(736, SeekOrigin.Begin);
                    UwsSampleInterval = Reader.ReadInt64();
                    UwsSampleInterval = Helper.Reverse(UwsSampleInterval);

                    //UwsCdataClassId
                    Reader.BaseStream.Seek(1488, SeekOrigin.Begin);
                    UwsCdataClassId = Reader.ReadInt32();
                    UwsCdataClassId = Helper.Reverse(UwsCdataClassId);

                    //UwsCollectorVersion
                    Reader.BaseStream.Seek(2092, SeekOrigin.Begin);
                    UwsCollectorVersion = Reader.ReadInt32();
                    UwsCollectorVersion = Helper.Reverse(UwsCollectorVersion);

                    //UwsCollectorVstringByte
                    Reader.BaseStream.Seek(2098, SeekOrigin.Begin);
                    UwsCollectorVstringByte = Reader.ReadBytes(UwsCollectorVstringByte.Length);
                    UwsCollectorVstring = myEncoding.GetString(UwsCollectorVstringByte).Trim();

#endregion

#region Create Index

                    //Create Index.
                    //byte[] indexBytes = new byte[60];
                    var indexBytes = new byte[62];
                    var tempShortBytes = new byte[2];
                    var tempIntBytes = new byte[4];

                    int indexPosition = UwsHLen;
                    long tempLen = (Convert.ToInt64(UwsXRecords) * Convert.ToInt64(UwsXLen));
                    long dataPosition = indexPosition + tempLen;

                    //List<Indices> Index = new List<Indices>();
                    for (int x = 0; x < UwsXRecords; x++) {
                        var indexer = new Indices();

                        Reader.BaseStream.Seek(indexPosition, SeekOrigin.Begin);
                        indexBytes = Reader.ReadBytes(indexBytes.Length);

                        //Get Index Name (first 8 bytes).
                        indexer.FName = myEncoding.GetString(indexBytes, 0, 8);

                        //Index Type.
                        tempShortBytes[0] = indexBytes[9];
                        tempShortBytes[1] = indexBytes[8];
                        indexer.FType = Convert.ToInt16(BitConverter.ToInt16(tempShortBytes, 0));

                        //Index Length.
                        tempShortBytes[0] = indexBytes[11];
                        tempShortBytes[1] = indexBytes[10];
                        indexer.FReclen = Convert.ToInt16(BitConverter.ToInt16(tempShortBytes, 0));

                        //Index Dump Occurs.
                        tempIntBytes[0] = indexBytes[15];
                        tempIntBytes[1] = indexBytes[14];
                        tempIntBytes[2] = indexBytes[13];
                        tempIntBytes[3] = indexBytes[12];
                        indexer.FRecords = Convert.ToInt32(BitConverter.ToInt32(tempIntBytes, 0));

                        //Index File Postion.
                        indexer.FilePosition = dataPosition;

                        //Insert into the List.
                        Index.Add(indexer);

                        indexPosition += UwsXLen;
                        tempLen = (Convert.ToInt64(indexer.FRecords) * Convert.ToInt64(indexer.FReclen));
                        dataPosition = dataPosition + tempLen;
                    }

#endregion
                }
            }

            return true;
        }

        /// <summary>
        /// Load the data into the database.
        /// </summary>
        /// <param name="unzipedFile">Full path of the Pathway file.</param>
        /// <param name="collectionInfo">Collection's basic info.</param>
        /// <returns>Return a bool value suggests whether the load is successful or not.</returns>
        private CollectionInfo CreatePathwayDataSet(string unzipedFile, CollectionInfo collectionInfo) {
            bool success = true;
            var tempShort = new byte[2];
            var tempInt = new byte[4];
            var tempLong = new byte[8];

            //SampleInfo values.
            //string systemName = UwsSystemName;
            //string systemSerial = UWSSerialNumber;

            //var startTimeLCT = new DateTime();
            //var stopTimeLCT = new DateTime();
            long sampleInterval = UwsSampleInterval;
            //string sysContent = "";
            int entityID = 0;

            var onceList = new List<string> { "PVCPUONCE", "PVPWYLIST", "PVPWYONCE", "PVRECINFO", "PVSCASSIGN", "PVSCDEFINE", "PVSCINFO", "PVSCPARAM", "PVSCPROC", "PVTCPINFO", "PVTERMINFO" };
            var manyList = new List<string> { "PVCPUMANY", "PVERRINFO", "PVLMSTUS", "PVPWYMANY", "PVSCLSTAT", "PVSCPRSTUS", "PVSCSTUS", "PVSCTSTAT", "PVTCPSTAT", "PVTCPSTUS", "PVTERMSTAT", "PVTERMSTUS" };

            try {
#region Open File and Load Data.

                _log.Info("    -Opening Pathway UWS to populate table");
                
                using (FileStream stream = new FileStream(unzipedFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    //using (StreamReader Reader = new StreamReader(stream))
                    using (Reader = new BinaryReader(stream)) {
                        //Loop thorugh the entity.
                        foreach (Indices index in Index) {
                            if (index.FName.Length != 0 && index.FRecords > 0) {
                                if (index.FName.Trim().ToUpper().Equals("RECINFO") || index.FName.Trim().ToUpper().Equals("COLLKEY"))
                                    continue;

                                string counterName = index.FName;
                                _log.InfoFormat("    -Counter: {0} Started at {1}", counterName, DateTime.Now);
                                
                                DateTime beforeTime = DateTime.Now;

                                string tableName = "pv" + counterName;
                                tableName = FormatTableName(tableName.Trim());
                                _log.InfoFormat("    -Table Name: {0}", tableName);
                                //Get table name according to data format.
                                //Get Column type into the List.
                                IDataDictionaryService dictionaryService = new DataDictionaryService(_connectionStringMain);
                                IList<ColumnInfoView> columnInfoList = dictionaryService.GetPathwayColumnsFor(tableName);

                                int recordLenth = index.FReclen;
                                long filePosition = index.FilePosition;
                                byte[] indexBytes;


                                string fullPath = stream.Name;
                                var dicInfo = new DirectoryInfo(fullPath);
                                try {
#region Create DataSet and insert data

                                    //Create DataSet with DataTable(s).
                                    DataTable myDataTable = CreateDataTableColumn(columnInfoList);

                                    //Loop through the records.
                                    for (int x = 0; x < index.FRecords; x++) {
#region each record

                                        Reader.BaseStream.Seek(filePosition, SeekOrigin.Begin);
                                        indexBytes = Reader.ReadBytes(recordLenth);
                                        long currentPosition = 0;
                                        bool isEmptyRow = false;

                                        //this will create newline for each loop.
                                        //Create new row.
                                        //string insertdbString = string.Empty;
                                        DataRow myDataRow = myDataTable.NewRow();
                                        var tempFromTimestamp = new DateTime();

                                        foreach (ColumnInfoView column in columnInfoList) {
                                            if (column.ColumnName.Equals("FromTimestamp") || column.ColumnName.Equals("ToTimestamp")) {
                                                //Calcuate the From and To Timestamp.

#region PVCOLLECTS

                                                if (tableName.Trim().ToUpper().Equals("PVCOLLECTS")) {
                                                    if (column.ColumnName.Equals("FromTimestamp")) {
                                                        //Get FromPcYear
                                                        string fromPcYear = "";
                                                        for (int z = 0; z < 4; z++) {
                                                            fromPcYear += Convert.ToChar(indexBytes[(currentPosition + 2) + z]);
                                                        }
                                                        //FromPcMonth
                                                        string fromPcMonth = "";
                                                        for (int z = 0; z < 2; z++) {
                                                            fromPcMonth += Convert.ToChar(indexBytes[(currentPosition + 6) + z]);
                                                        }
                                                        //FromPcDay
                                                        string fromPcDay = "";
                                                        for (int z = 0; z < 2; z++) {
                                                            fromPcDay += Convert.ToChar(indexBytes[(currentPosition + 8) + z]);
                                                        }
                                                        //FromHour
                                                        string fromHour = "";
                                                        for (int z = 0; z < 2; z++) {
                                                            fromHour += Convert.ToChar(indexBytes[(currentPosition + 18) + z]);
                                                        }
                                                        var fromTimestamp = new DateTime(Convert.ToInt32(fromPcYear),
                                                            Convert.ToInt32(fromPcMonth),
                                                            Convert.ToInt32(fromPcDay),
                                                            Convert.ToInt32(fromHour), 0, 0);
                                                        collectionInfo.FromTimestamp = fromTimestamp;
                                                        column.TestValue = fromTimestamp.ToString();
                                                        column.TypeValue = 11;
                                                    }

                                                    else if (column.ColumnName.Equals("ToTimestamp")) {
                                                        //ThruPcYear
                                                        string thruPcYear = "";
                                                        for (int z = 0; z < 4; z++) {
                                                            thruPcYear += Convert.ToChar(indexBytes[(currentPosition - 1) + z]);
                                                        }
                                                        //ThruPcMonth
                                                        string thruPcMonth = "";
                                                        for (int z = 0; z < 2; z++) {
                                                            thruPcMonth += Convert.ToChar(indexBytes[(currentPosition + 3) + z]);
                                                        }
                                                        //ThruPcDay
                                                        string thruPcDay = "";
                                                        for (int z = 0; z < 2; z++) {
                                                            thruPcDay += Convert.ToChar(indexBytes[(currentPosition + 5) + z]);
                                                        }
                                                        //ThruHour
                                                        string thruHour = "";
                                                        for (int z = 0; z < 2; z++) {
                                                            thruHour += Convert.ToChar(indexBytes[(currentPosition + 9) + z]);
                                                        }
                                                        var toTimestamp = new DateTime(Convert.ToInt32(thruPcYear),
                                                            Convert.ToInt32(thruPcMonth),
                                                            Convert.ToInt32(thruPcDay),
                                                            Convert.ToInt32(thruHour), 0, 0);

                                                        if (collectionInfo.FromTimestamp.Equals(toTimestamp))
                                                            collectionInfo.ToTimestamp = toTimestamp.AddHours(24);
                                                        else
                                                            collectionInfo.ToTimestamp = toTimestamp;

                                                        //Round up the seconds.
                                                        TimeSpan span1 = collectionInfo.ToTimestamp - collectionInfo.FromTimestamp;
                                                        double seconds1 = span1.TotalSeconds;
                                                        //Get remained seconds.
                                                        double remainSeconds1 = seconds1 % sampleInterval;

                                                        collectionInfo.ToTimestamp = collectionInfo.ToTimestamp.AddSeconds(-remainSeconds1);

                                                        _log.InfoFormat("FromTimestamp: {0}, ToTimestamp: {1}", 
                                                            collectionInfo.FromTimestamp, collectionInfo.ToTimestamp);
                                                        
                                                        //Check duplicated data.
                                                        IPvCollectService collects = new PvCollectService(_connectionStringSystem);
                                                        bool isDuplicated = collects.IsDuplictedFor(collectionInfo.FromTimestamp, collectionInfo.ToTimestamp);

                                                        if (isDuplicated) {
                                                            _log.Info("Duplicated Data, return true!");
                                                            collectionInfo.IsSuccess = true;
                                                            collectionInfo.IsDuplicate = true;
                                                            return collectionInfo;
                                                        }

                                                        column.TestValue = collectionInfo.ToTimestamp.ToString();
                                                        column.TypeValue = 11;
                                                    }
                                                }
#endregion
#region ONCE

                                                else if (onceList.Contains(tableName.Trim().ToUpper())) {
                                                    if (column.ColumnName.Equals("FromTimestamp")) {
                                                        column.TestValue = collectionInfo.FromTimestamp.ToString();
                                                        column.TypeValue = 5;
                                                    }

                                                    else if (column.ColumnName.Equals("ToTimestamp")) {
                                                        column.TestValue = collectionInfo.ToTimestamp.ToString();
                                                        column.TypeValue = 5;
                                                    }
                                                }
#endregion
#region MANY

                                                else if (manyList.Contains(tableName.Trim().ToUpper())) {
                                                    if (column.ColumnName.Equals("FromTimestamp")) {
                                                        tempLong[0] = indexBytes[(currentPosition + 2) + 7];
                                                        tempLong[1] = indexBytes[(currentPosition + 2) + 6];
                                                        tempLong[2] = indexBytes[(currentPosition + 2) + 5];
                                                        tempLong[3] = indexBytes[(currentPosition + 2) + 4];
                                                        tempLong[4] = indexBytes[(currentPosition + 2) + 3];
                                                        tempLong[5] = indexBytes[(currentPosition + 2) + 2];
                                                        tempLong[6] = indexBytes[(currentPosition + 2) + 1];
                                                        tempLong[7] = indexBytes[(currentPosition + 2) + 0];
                                                        long tempDate = Convert.ToInt64(BitConverter.ToInt64(tempLong, 0));
                                                        //Need to do / 10000 to get current julian time
                                                        tempDate /= 10000;

                                                        if (tempDate == 0) {
                                                            column.TestValue = "";
                                                        }
                                                        else {
                                                            var convert = new ConvertJulianTime();
                                                            int obdTimeStamp = convert.JulianTimeStampToOBDTimeStamp(tempDate);
                                                            DateTime dbDate = convert.OBDTimeStampToDBDate(obdTimeStamp);

                                                            //We are collecting hour 00 data on hour 01. so subtract an interval to make it correct time.
                                                            if (collectionInfo.IntervalType.Equals("H"))
                                                                tempFromTimestamp = dbDate.AddHours(collectionInfo.IntervalNumber * -1);
                                                            else
                                                                tempFromTimestamp = dbDate.AddMinutes(collectionInfo.IntervalNumber * -1);

                                                            //Since the first entry is always empty, don't load it to the database.
                                                            if (tempFromTimestamp < collectionInfo.FromTimestamp)
                                                                isEmptyRow = true;
                                                            column.TestValue = tempFromTimestamp.ToString();
                                                        }

                                                        column.TypeValue = 9;
                                                    }

                                                    else if (column.ColumnName.Equals("ToTimestamp")) {
                                                        DateTime newToTimestamp;
                                                        if (collectionInfo.IntervalType.Equals("H"))
                                                            newToTimestamp = tempFromTimestamp.AddHours(collectionInfo.IntervalNumber);
                                                        else
                                                            newToTimestamp = tempFromTimestamp.AddMinutes(collectionInfo.IntervalNumber);

                                                        column.TestValue = newToTimestamp.ToString();
                                                        column.TypeValue = 9;
                                                    }
                                                }

#endregion

                                                currentPosition += column.TypeValue;
                                                continue;
                                            }

                                            // This condition is to check if the number of fields in the UWS file is less than in TabelTable, we have to stop getting data and feed in just 'Null'
                                            if (currentPosition < index.FReclen) {
#region Switch

                                                switch (column.TypeName.ToUpper().Trim()) {
                                                    case "SHORT":
                                                        tempShort[0] = indexBytes[currentPosition + 1];
                                                        tempShort[1] = indexBytes[currentPosition];
                                                        column.TestValue = BitConverter.ToInt16(tempShort, 0).ToString();
                                                        column.TypeValue = 2;
                                                        break;
                                                    case "LONG":
                                                        tempInt[0] = indexBytes[currentPosition + 3];
                                                        tempInt[1] = indexBytes[currentPosition + 2];
                                                        tempInt[2] = indexBytes[currentPosition + 1];
                                                        tempInt[3] = indexBytes[currentPosition + 0];
                                                        column.TestValue = BitConverter.ToInt32(tempInt, 0).ToString();
                                                        column.TypeValue = 4;
                                                        break;
                                                    case "ULONG":
                                                        tempInt[0] = indexBytes[currentPosition + 3];
                                                        tempInt[1] = indexBytes[currentPosition + 2];
                                                        tempInt[2] = indexBytes[currentPosition + 1];
                                                        tempInt[3] = indexBytes[currentPosition + 0];
                                                        column.TestValue = BitConverter.ToUInt32(tempInt, 0).ToString();
                                                        column.TypeValue = 4;
                                                        break;
                                                    case "DOUBLE":
                                                        tempLong[0] = indexBytes[currentPosition + 7];
                                                        tempLong[1] = indexBytes[currentPosition + 6];
                                                        tempLong[2] = indexBytes[currentPosition + 5];
                                                        tempLong[3] = indexBytes[currentPosition + 4];
                                                        tempLong[4] = indexBytes[currentPosition + 3];
                                                        tempLong[5] = indexBytes[currentPosition + 2];
                                                        tempLong[6] = indexBytes[currentPosition + 1];
                                                        tempLong[7] = indexBytes[currentPosition + 0];
                                                        column.TestValue = BitConverter.ToInt64(tempLong, 0).ToString();
                                                        column.TypeValue = 8;
                                                        break;
                                                    case "TEXT":
                                                        string tempString = "";
                                                        for (int z = 0; z < column.TypeValue; z++) {
                                                            tempString += Convert.ToChar(indexBytes[currentPosition + z]);
                                                        }
                                                        if (tableName.Trim().ToUpper().Equals("PVCOLLECTS")) {
                                                            if (column.ColumnName.Equals("IntervalNn"))
                                                                collectionInfo.IntervalNumber = Convert.ToInt32(tempString.Trim());
                                                            else if (column.ColumnName.Equals("IntervalHOrM"))
                                                                collectionInfo.IntervalType = tempString.Trim();
                                                        }

                                                        column.TestValue = tempString.Trim();
                                                        break;
                                                    case "DATE":
                                                        tempLong[0] = indexBytes[currentPosition + 7];
                                                        tempLong[1] = indexBytes[currentPosition + 6];
                                                        tempLong[2] = indexBytes[currentPosition + 5];
                                                        tempLong[3] = indexBytes[currentPosition + 4];
                                                        tempLong[4] = indexBytes[currentPosition + 3];
                                                        tempLong[5] = indexBytes[currentPosition + 2];
                                                        tempLong[6] = indexBytes[currentPosition + 1];
                                                        tempLong[7] = indexBytes[currentPosition + 0];
                                                        long tempDate = Convert.ToInt64(BitConverter.ToInt64(tempLong, 0));
                                                        //Need to do / 10000 to get current julian time
                                                        tempDate /= 10000;

                                                        if (tempDate == 0) {
                                                            column.TestValue = "";
                                                        }
                                                        else {
                                                            var convert = new ConvertJulianTime();
                                                            int obdTimeStamp = convert.JulianTimeStampToOBDTimeStamp(tempDate);
                                                            DateTime dbDate = convert.OBDTimeStampToDBDate(obdTimeStamp);
                                                            column.TestValue = dbDate.ToString();
                                                        }
                                                        column.TypeValue = 8;
                                                        break;
                                                }

#endregion

                                                currentPosition += column.TypeValue;
                                            }
                                            else {
                                                column.TestValue = "";
                                            }
                                        }

                                        //Populate Datatable.
                                        foreach (ColumnInfoView column in columnInfoList) {
#region Switch

                                            switch (column.TypeName.ToUpper().Trim()) {
                                                case "SHORT":
                                                    myDataRow[column.ColumnName] = Convert.ToInt16(column.TestValue);
                                                    break;
                                                case "LONG":
                                                    myDataRow[column.ColumnName] = Convert.ToInt32(column.TestValue);
                                                    break;
                                                case "ULONG":
                                                    myDataRow[column.ColumnName] = Convert.ToUInt32(column.TestValue);
                                                    break;
                                                case "DOUBLE":
                                                    myDataRow[column.ColumnName] = Convert.ToInt64(column.TestValue);
                                                    break;
                                                case "TINYINT":
                                                    myDataRow[column.ColumnName] = Convert.ToByte(column.TestValue);
                                                    break;
                                                case "TEXT":
                                                    myDataRow[column.ColumnName] = column.TestValue;
                                                    break;
                                                case "DATE":
                                                    myDataRow[column.ColumnName] = Convert.ToDateTime(column.TestValue);
                                                    break;
                                            }

#endregion
                                        }

                                        if (!isEmptyRow) {
                                            //Add new row into the dataSet.
                                            myDataTable.Rows.Add(myDataRow);
                                        }

                                        //check to see if the row has more then 10000 rows.
                                        if (myDataTable.Rows.Count > 10000) {
                                            //Insert into the table.

                                            IDataTableService insertTables = new DataTableService(_connectionStringSystem);
                                            insertTables.InsertEntityDataFor(tableName, myDataTable, dicInfo.Parent.FullName);

                                            //Clear the myDataTable.
                                            myDataTable.Rows.Clear();
                                        }

                                        //Increase the start reading position.
                                        filePosition += recordLenth;

#endregion
                                    } // End For

                                    //Insert into the database.
                                    IDataTableService tables = new DataTableService(_connectionStringSystem);
                                    tables.InsertEntityDataFor(tableName, myDataTable, dicInfo.Parent.FullName);
                                    DateTime afterTime = DateTime.Now;
                                    TimeSpan timeSpan = afterTime - beforeTime;
                                    _log.InfoFormat("    -Counter: {0}, Total Time in Minutes: {1}", counterName, timeSpan.TotalMinutes);
#endregion
                                }
                                catch (Exception ex) {
                                    _log.ErrorFormat("Counter: {0} \nRA Message: {1} \n RA File Position = {2}",
                                                        counterName, ex, filePosition);
                                    collectionInfo.IsSuccess = false;
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                }

#endregion

                _log.InfoFormat("Call Alert at: {0}", DateTime.Now);
                
                InsertAlert(collectionInfo);
                _log.InfoFormat("Finish Alert at: {0}", DateTime.Now);
                

                _log.InfoFormat("Call InsertCPUBusies at: {0}", DateTime.Now);
                
                InsertCPUBusies(collectionInfo);
                _log.InfoFormat("Finish InsertCPUBusies at: {0}", DateTime.Now);
                

                _log.InfoFormat("Update  PvPwymany at: {0}", DateTime.Now);
                
                UpdatePvPwymany(collectionInfo);
                _log.InfoFormat("Finish Updating PvPwymany at: {0}", DateTime.Now);
                
                collectionInfo.IsSuccess = true;
            }
            catch (Exception ex) {
                _log.ErrorFormat("EntityID: {0}. {1}", entityID, ex);
                success = false;
            }
            finally {
                _log.InfoFormat("    -Finished populateing RA Database at {0} with {1}", DateTime.Now, success);
                GC.Collect();
            }
            return collectionInfo;
        }

        /// <summary>
        /// Create the table structure.
        /// </summary>
        /// <param name="columnInfo"> List of ColumnInfoView which contains the column info of the table that going to be created.</param>
        /// <returns> Return a DataTable that contains the table structure.</returns>
        private DataTable CreateDataTableColumn(IEnumerable<ColumnInfoView> columnInfo) {
            //This DataTableName has be to start Date(only date part), because I have to compare with data's FromTimestamp.

            var myDataTable = new DataTable();

            /*if (columnInfo[0].ColumnName != "TSID") {
                // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
                myDataColumn = new DataColumn();
                myDataColumn.DataType = Type.GetType("System.Int16");
                myDataColumn.ColumnName = "TSID";
                // Add the Column to the DataColumnCollection.
                myDataTable.Columns.Add(myDataColumn);
            }

            if (columnInfo[1].ColumnName != "DataClass") {
                // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
                myDataColumn = new DataColumn();
                myDataColumn.DataType = Type.GetType("System.Int16");
                myDataColumn.ColumnName = "DataClass";
                // Add the Column to the DataColumnCollection.
                myDataTable.Columns.Add(myDataColumn);
            }

            if (columnInfo[2].ColumnName != "GID") {
                // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
                myDataColumn = new DataColumn();
                myDataColumn.DataType = Type.GetType("System.Int32");
                myDataColumn.ColumnName = "GID";
                // Add the Column to the DataColumnCollection.
                myDataTable.Columns.Add(myDataColumn);
            }*/
            //Insert FromTimestamp and ToTimestamp.


            foreach (ColumnInfoView column in columnInfo) {
                // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
                var myDataColumn = new DataColumn {
                    DataType = Type.GetType(GetSystemValueType(column.TypeName)),
                    ColumnName = column.ColumnName
                };
                // Add the Column to the DataColumnCollection.
                myDataTable.Columns.Add(myDataColumn);
            }

            return myDataTable;
        }

        /// <summary>
        /// Get the system data type according to the SQL Server data type.
        /// </summary>
        /// <param name="type"> SQL Server data type. </param>
        /// <returns> Return a string value which is system data type that used in creating datatable.</returns>
        private string GetSystemValueType(string type) {
            string returnType;
            switch (type.ToUpper()) {
                case "DATE":
                    returnType = "System.DateTime";
                    break;
                case "DOUBLE":
                    returnType = "System.Double";
                    break;
                case "LONG":
                    returnType = "System.Int32";
                    break;
                case "ULONG":
                    returnType = "System.UInt32";
                    break;
                case "SHORT":
                    returnType = "System.Int16";
                    break;
                case "TEXT":
                    returnType = "System.String";
                    break;
                default:
                    returnType = "System.String";
                    break;
            }
            return returnType;
        }

        private CollectionInfo InsertAlert(CollectionInfo collectionInfo) {
            //Insert Alerts.
            IPvPwyListService pvPwyList = new PvPwyListService(_connectionStringSystem);
            List<string> pathwayList = pvPwyList.GetPathwayNamesFor(collectionInfo.FromTimestamp, collectionInfo.ToTimestamp);

            //For each Pathway Name, Loop throuth the From and To Timestamp using Interval and insert empty data.
            IPvAlertService alert = new PvAlertService(_connectionStringSystem, 0);
            foreach (string pathway in pathwayList) {
                for (DateTime startTime = collectionInfo.FromTimestamp;
                    startTime < collectionInfo.ToTimestamp;
                    startTime = collectionInfo.IntervalType.Equals("H") ? startTime.AddHours(collectionInfo.IntervalNumber) : startTime.AddMinutes(collectionInfo.IntervalNumber)) {
                    alert.InsertEmptyDataFor(pathway, startTime, collectionInfo.IntervalType.Equals("H") ? startTime.AddHours(collectionInfo.IntervalNumber) : startTime.AddMinutes(collectionInfo.IntervalNumber));
                }
            }

            //Insert All the Alerts.

            var alertList = new List<string> {
                "TermHiMaxRT",
                "TermHiAvgRT",
                "TermUnused",
                "TermErrorList",
                "TCPQueuedTransactions",
                "TCPLowTermPool",
                "TCPLowServerPool",
                "TCPUnused",
                "TCPErrorList",
                "ServerHiMaxRT",
                "ServerHiAvgRT",
                "ServerQueueTCP",
                "ServerQueueLinkmon",
                "ServerUnusedClass",
                "ServerUnusedProcess",
                "ServerErrorList"
            };

            foreach (string pvAlert in alertList) {
                _log.InfoFormat("Calling {0}", pvAlert);               

                List<Alert> alertView = alert.GetHourlyAlertFor(pvAlert, collectionInfo.FromTimestamp, collectionInfo.ToTimestamp);

                if (alertView.Count > 0) {
                    collectionInfo.IsAlert = true;
                    long intervalInSec;
                    if (collectionInfo.IntervalType.Equals("H"))
                        intervalInSec = collectionInfo.IntervalNumber * 3600;
                    else
                        intervalInSec = collectionInfo.IntervalNumber * 60;

                    alert.UpdateAlertFor(pvAlert, alertView);
                }
            }

            return collectionInfo;
        }

        private void InsertCPUBusies(CollectionInfo collectionInfo) {
            long intervalInSec;
            if (collectionInfo.IntervalType.Equals("H"))
                intervalInSec = collectionInfo.IntervalNumber * 3600;
            else
                intervalInSec = collectionInfo.IntervalNumber * 60;

            IAllPathwayService service = new AllPathwayService(_connectionStringSystem, intervalInSec);
            var summary = service.GetCPUSummaryFor(collectionInfo.FromTimestamp, collectionInfo.ToTimestamp, UWSSerialNumber);

            ICPUBusyService cpuBusyService = new CPUBusyService(_connectionStringSystem);
            var items = summary.AllPathways.OrderByDescending(x => x.Value).ToList();
            foreach (var view in items) {
                cpuBusyService.InsertCPUBusyFor(view.Key,
                                                collectionInfo.FromTimestamp,
                                                collectionInfo.ToTimestamp,
                                                (view.Value / summary.AllPathways.Values.Sum()) * 100);
            }
        }
        public Dictionary<string, AlertView> GetAlerts(CollectionInfo collectionInfo) {
            //Get All the Alerts.
            var alertList = new List<string> {
                "TermHiMaxRT",
                "TermHiAvgRT",
                "TermUnused",
                "TermErrorList",
                "TCPQueuedTransactions",
                "TCPLowTermPool",
                "TCPLowServerPool",
                "TCPUnused",
                "TCPErrorList",
                "ServerHiMaxRT",
                "ServerHiAvgRT",
                "ServerQueueTCP",
                "ServerQueueLinkmon",
                "ServerUnusedClass",
                "ServerUnusedProcess",
                "ServerErrorList"
            };

            //For each Pathway Name, Loop throuth the From and To Timestamp using Interval and insert empty data.
            IPvAlertService alert = new PvAlertService(_connectionStringSystem, 0);
            var collectionAlerts = alert.GetCollectionAlertFor(alertList, collectionInfo.FromTimestamp, collectionInfo.ToTimestamp);
            return collectionAlerts;
        }

        private void UpdatePvPwymany(CollectionInfo collectionInfo) {
            var pwyManyService = new PvPwyManyService(_connectionStringSystem);
            var pathways = pwyManyService.GetPvPwyMany(collectionInfo.FromTimestamp, collectionInfo.ToTimestamp, UWSSerialNumber);

            foreach (var pvPwyManyView in pathways) {

            }
        }
        public bool DeleteData(DateTime fromTimestamp, DateTime toTimestamp) {
            var delete = true;

            try {
                var clean = new DeletePathwayData(_log, _connectionStringSystem);
                clean.DelteAllPathwayDataFor(fromTimestamp, toTimestamp);
            }
            catch (Exception ex) {
                _log.ErrorFormat("DeleteData Error: {0}, {1}", ex.Message, ex.InnerException);
                delete = false;
            }
            return delete;
        }

        private string FormatTableName(string tableName) {
            var newName = "";
            switch (tableName.ToUpper()) {
                case "PVALERTS":
                    newName = "PvAlerts";
                    break;
                case "PVCOLLECTS": newName = "PvCollects";
                    break;
                case "PVCPUBUSIES": newName = "PvCPUBusies";
                    break;
                case "PVCPUMANY": newName = "PvCpumany";
                    break;
                case "PVCPUONCE": newName = "PvCpuonce";
                    break;
                case "PVERRINFO": newName = "PvErrinfo";
                    break;
                case "PVLMSTUS": newName = "PvLmstus";
                    break;
                case "PVPWYLIST": newName = "PvPwylist";
                    break;
                case "PVPWYMANY": newName = "PvPwymany";
                    break;
                case "PVPWYONCE": newName = "PvPwyonce";
                    break;
                case "PVSCASSIGN": newName = "PvScassign";
                    break;
                case "PVSCDEFINE": newName = "PvScdefine";
                    break;
                case "PVSCINFO": newName = "PvScinfo";
                    break;
                case "PVSCLSTAT": newName = "PvSclstat";
                    break;
                case "PVSCPARAM": newName = "PvScparam";
                    break;
                case "PVSCPROC": newName = "PvScproc";
                    break;
                case "PVSCPRSTUS": newName = "PvScprstus";
                    break;
                case "PVSCSTUS": newName = "PvScstus";
                    break;
                case "PVSCTSTAT": newName = "PvSctstat";
                    break;
                case "PVTCPINFO": newName = "PvTcpinfo";
                    break;
                case "PVTCPSTAT": newName = "PvTcpstat";
                    break;
                case "PVTCPSTUS": newName = "PvTcpstus";
                    break;
                case "PVTERMINFO": newName = "PvTerminfo";
                    break;
                case "PVTERMSTAT": newName = "PvTermstat";
                    break;
                case "PVTERMSTUS": newName = "PvTermstus";
                    break;
            }

            return newName;
        }
    }
}