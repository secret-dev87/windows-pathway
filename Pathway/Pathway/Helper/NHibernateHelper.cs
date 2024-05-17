using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using Pathway.Core.Entity;
using FluentNHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Pathway.Core.Entity.Main;
using Pathway.Core.Entity.Dynamic;
using NHibernate.Mapping;

namespace Pathway.Core.Helper
{
    public class NHibernateHelper
    {
        private static ISessionFactory _mainSessionFactory;
        private static ISessionFactory _systemSessionFactory;
        private static ISessionFactory _dynamicSessionFactory;

        private static ISessionFactory MainSessionFactory
        {
            get
            {
                if (_mainSessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure("main.hibernate.cfg.xml");

                    _mainSessionFactory = Fluently.Configure(configuration)
                        .Mappings(m => {
                            m.FluentMappings.AddFromAssemblyOf<PathwayAlertsEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PathwayAllAlertsEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTableTableNewEntity>();
                            m.FluentMappings.AddFromAssemblyOf<ReportDownloadLogEntity>();
                            m.FluentMappings.AddFromAssemblyOf<SystemTblEntity>();
                        })
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
                        .BuildSessionFactory();
                }
                return _mainSessionFactory;
            }
        }

        private static ISessionFactory SystemSessionFactory
        {
            get
            {
                if (_systemSessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure("system.hibernate.cfg.xml");

                    _systemSessionFactory = Fluently.Configure(configuration)
                        .Mappings(m => {
                            m.FluentMappings.AddFromAssemblyOf<PvAlertEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCollectEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCPUBusyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCPUManyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCPUOnceEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvErrInfoEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvErrorEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvLmStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvPwyListEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvPwyManyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvPwyOnceEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScAssignEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScDefineEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScInfoEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScLStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScProcEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScPrStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScTStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTcpInfoEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTcpStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTcpStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTermInfoEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTermStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTermStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<TrendPathwayHourlyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<CurrentTableEntity>();
                        })
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
                        .BuildSessionFactory();
                }
                return _systemSessionFactory;
            }
        }

        private static ISessionFactory DynamicSessionFactory(string tableName)
        {
            var configuration = new Configuration();
            configuration.Configure("dynamic.hibernate.cfg.xml");

            var fluentConfiguration = Fluently.Configure(configuration)
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<CpuEntity>();
                    m.FluentMappings.AddFromAssemblyOf<ProcessEntity>();
                })
                .ProxyFactoryFactory("NHibernate.Bytecode.DefaultProxyFactoryFactory, NHibernate");

            var config = fluentConfiguration.BuildConfiguration();

            if (tableName.ToLower().Contains("_cpu_"))
            {
                foreach (PersistentClass persistentClass in config.ClassMappings)
                {
                    if (persistentClass.MappedClass == typeof(CpuEntity))
                    {
                        persistentClass.Table.Name = tableName;
                    }
                }
            }
            
            if (tableName.ToLower().Contains("_process_"))
            {
                foreach (PersistentClass persistentClass in config.ClassMappings)
                {
                    if (persistentClass.MappedClass == typeof(ProcessEntity))
                    {
                        persistentClass.Table.Name = tableName;
                    }
                }
            }

            _dynamicSessionFactory = config.BuildSessionFactory();
                    
            return _dynamicSessionFactory;
        }


        public static ISession OpenMainSession()
        {
            return MainSessionFactory.OpenSession();
        }

        public static ISession OpenSystemSession()
        {
            return SystemSessionFactory.OpenSession();
        }

        public static ISession OpenDynamicSession(string tableName)
        {
            return DynamicSessionFactory(tableName).OpenSession();
        }
    }
}
