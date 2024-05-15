using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Helper
{
    internal class DeleteHelper
    {
        public DeleteHelper() { }

        public void DeleteData(string tableName, DateTime fromTimestamp, DateTime toTimestamp)
        {
            using(var session = NHibernateHelper.OpenSystemSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    string hql = "";

                    switch(tableName)
                    {
                        case "PvAlerts":
                            hql = "delete from PvAlertEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvCollects":
                            hql = "delete from PvCollectEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvCPUBusies":
                            hql = "delete from PvCPUBusyEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvCPUMany":
                            hql = "delete from PvCPUManyEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvCPUOnce":
                            hql = "delete from PvCPUOnceEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvErrInfo":
                            hql = "delete from PvErrInfoEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvLmStus":
                            hql = "delete from PvLmStusEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvPwyList":
                            hql = "delete from PvPwyListEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvPwyMany":
                            hql = "delete from PvPwyManyEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvPwyOnce":
                            hql = "delete from PvPwyOnceEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScAssign":
                            hql = "delete from PvScAssignEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScDefine":
                            hql = "delete from PvScDefineEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScInfo":
                            hql = "delete from PvScInfoEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScLStat":
                            hql = "delete from PvScLStatEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScProc":
                            hql = "delete from PvScProcEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScPrStus":
                            hql = "delete from PvScPrStusEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScStus":
                            hql = "delete from PvScStusEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvScTStat":
                            hql = "delete from PvScTStatEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvTcpInfo":
                            hql = "delete from PvTcpInfoEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvTcpStat":
                            hql = "delete from PvTcpStatEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvTcpStus":
                            hql = "delete from PvTcpStusEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvTermInfo":
                            hql = "delete from PvTermInfoEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvTermStat":
                            hql = "delete from PvTermStatEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        case "PvTermStus":
                            hql = "delete from PvTermStusEntity where FromTimestamp >= :fromTimestamp and ToTimestamp <= :toTimestamp";
                            break;
                        default:
                            break;
                    }
                    

                    if (!string.IsNullOrEmpty(hql))
                    {
                        var query = session.CreateQuery(hql);
                        query.SetParameter("fromTimestamp", fromTimestamp);
                        query.SetParameter("toTimestamp", toTimestamp);
                    }

                    transaction.Commit();
                }
            }
            
        }
    }
}
