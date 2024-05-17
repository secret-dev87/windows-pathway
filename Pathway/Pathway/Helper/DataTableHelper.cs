using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Pathway.Core.Entity;
using Pathway.Core.Mapping;

namespace Pathway.Core.Helper
{
    public class DataTableHelper
    {
        public static void InsertData(string tableName, string fileLocation)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSystemSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        switch (tableName)
                        {
                            case "TrendPathwayHourly":
                                using(var reader = new StreamReader(fileLocation))
                                {
                                    using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                                    {
                                        csv.Context.RegisterClassMap<ClassMap<TrendPathwayHourlyEntity>>();
                                        var records = csv.GetRecords<TrendPathwayHourlyEntity>().ToList();

                                        foreach(TrendPathwayHourlyEntity item in records)
                                        {
                                            session.Save(item);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void InsertEntityData(string tableName, DataTable dsData, string path)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSystemSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var jsondata = JsonConvert.SerializeObject(dsData);

                        switch (tableName)
                        {
                            case "PvCollects":
                                List<PvCollectEntity> collects = JsonConvert.DeserializeObject<List<PvCollectEntity>>(jsondata);
                                foreach (PvCollectEntity item in collects)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvAlerts":
                                List<PvAlertEntity> alerts = JsonConvert.DeserializeObject<List<PvAlertEntity>>(jsondata);
                                foreach (PvAlertEntity item in alerts)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvCPUBusies":
                                List<PvCPUBusyEntity> cpubusies = JsonConvert.DeserializeObject<List<PvCPUBusyEntity>>(jsondata);
                                foreach (PvCPUBusyEntity item in cpubusies)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvCpumany":
                                List<PvCPUManyEntity> cpuManys = JsonConvert.DeserializeObject<List<PvCPUManyEntity>>(jsondata);
                                foreach (PvCPUManyEntity item in cpuManys)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvCpuonce":
                                List<PvCPUOnceEntity> cpuonces = JsonConvert.DeserializeObject<List<PvCPUOnceEntity>>(jsondata);
                                foreach (PvCPUOnceEntity item in cpuonces)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvErrinfo":
                                List<PvErrInfoEntity> errInfos = JsonConvert.DeserializeObject<List<PvErrInfoEntity>>(jsondata);
                                foreach (PvErrInfoEntity item in errInfos)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvLmstus":
                                List<PvLmStusEntity> lmStuses = JsonConvert.DeserializeObject<List<PvLmStusEntity>>(jsondata);
                                foreach (PvLmStusEntity item in lmStuses)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvPwylist":
                                List<PvPwyListEntity> pwyLists = JsonConvert.DeserializeObject<List<PvPwyListEntity>>(jsondata);
                                foreach (PvPwyListEntity item in pwyLists)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvPwymany":
                                List<PvPwyManyEntity> pwyManys = JsonConvert.DeserializeObject<List<PvPwyManyEntity>>(jsondata);
                                foreach (PvPwyManyEntity item in pwyManys)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvPwyonce":
                                List<PvPwyOnceEntity> pwyOnces = JsonConvert.DeserializeObject<List<PvPwyOnceEntity>>(jsondata);
                                foreach (PvPwyOnceEntity item in pwyOnces)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScassign":
                                List<PvScAssignEntity> scAssignes = JsonConvert.DeserializeObject<List<PvScAssignEntity>>(jsondata);
                                foreach (PvScAssignEntity item in scAssignes)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScdefine":
                                List<PvScDefineEntity> scDefines = JsonConvert.DeserializeObject<List<PvScDefineEntity>>(jsondata);
                                foreach (PvScDefineEntity item in scDefines)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScinfo":
                                List<PvScInfoEntity> scInfos = JsonConvert.DeserializeObject<List<PvScInfoEntity>>(jsondata);
                                foreach (PvScInfoEntity item in scInfos)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvSclstat":
                                List<PvScLStatEntity> scLstats = JsonConvert.DeserializeObject<List<PvScLStatEntity>>(jsondata);
                                foreach (PvScLStatEntity item in scLstats)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScparam":
                                List<PvScParamEntity> scParams = JsonConvert.DeserializeObject<List<PvScParamEntity>>(jsondata);
                                foreach (PvScParamEntity item in scParams)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScproc":
                                List<PvScProcEntity> scProcs = JsonConvert.DeserializeObject<List<PvScProcEntity>>(jsondata);
                                foreach (PvScProcEntity item in scProcs)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScprstus":
                                List<PvScPrStusEntity> scprStuses = JsonConvert.DeserializeObject<List<PvScPrStusEntity>>(jsondata);
                                foreach (PvScPrStusEntity item in scprStuses)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvScstus":
                                List<PvScStusEntity> scStuses = JsonConvert.DeserializeObject<List<PvScStusEntity>>(jsondata);
                                foreach (PvScStusEntity item in scStuses)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvSctstat":
                                List<PvScTStatEntity> scTStates = JsonConvert.DeserializeObject<List<PvScTStatEntity>>(jsondata);
                                foreach (PvScTStatEntity item in scTStates)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvTcpinfo":
                                List<PvTcpInfoEntity> tcpInfos = JsonConvert.DeserializeObject<List<PvTcpInfoEntity>>(jsondata);
                                foreach (PvTcpInfoEntity item in tcpInfos)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvTcpstat":
                                List<PvTcpStatEntity> tcpStates = JsonConvert.DeserializeObject<List<PvTcpStatEntity>>(jsondata);
                                foreach (PvTcpStatEntity item in tcpStates)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvTcpstus":
                                List<PvTcpStusEntity> tcpStuses = JsonConvert.DeserializeObject<List<PvTcpStusEntity>>(jsondata);
                                foreach (PvTcpStusEntity item in tcpStuses)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvTerminfo":
                                List<PvTermInfoEntity> termInfos = JsonConvert.DeserializeObject<List<PvTermInfoEntity>>(jsondata);
                                foreach (PvTermInfoEntity item in termInfos)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvTermstat":
                                List<PvTermStatEntity> termStates = JsonConvert.DeserializeObject<List<PvTermStatEntity>>(jsondata);
                                foreach (PvTermStatEntity item in termStates)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            case "PvTermstus":
                                List<PvTermStusEntity> termStuses = JsonConvert.DeserializeObject<List<PvTermStusEntity>>(jsondata);
                                foreach (PvTermStusEntity item in termStuses)
                                {
                                    session.SaveOrUpdate(item);
                                }
                                break;
                            default:
                                break;
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Table " + tableName + " :" + ex.Message);
            }
        }
    }
}
