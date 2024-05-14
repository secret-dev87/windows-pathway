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

namespace Pathway.Core.Helper
{
    public class NHibernateHelper
    {
        private static ISessionFactory _mainSessionFactory;
        private static ISessionFactory _systemSessionFactory;

        private static ISessionFactory MainSessionFactory
        {
            get
            {
                if (_mainSessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure("main.hibernate.cfg.xml");

                    Fluently.Configure(configuration)
                        .Mappings(m => {
                            m.FluentMappings.AddFromAssemblyOf<PathwayAlertsEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PathwayAllAlertsEntity>();
                        })
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false));

                    _mainSessionFactory = configuration.BuildSessionFactory();
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
                        .Mappings(m =>
                        {
                            m.FluentMappings.AddFromAssemblyOf<PvAlertEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCollectEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCPUBusyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvCPUManyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvErrorEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvPwyListEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvPwyManyEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScInfoEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScLStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScPrStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvScTStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTcpInfoEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTcpStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTcpStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTermStatEntity>();
                            m.FluentMappings.AddFromAssemblyOf<PvTermStusEntity>();
                            m.FluentMappings.AddFromAssemblyOf<TrendPathwayHourlyEntity>();
                        })
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false))
                        .BuildSessionFactory();
                }
                return _systemSessionFactory;
            }
        }

        public static ISession OpenMainSession()
        {
            return MainSessionFactory.OpenSession();
        }

        public static ISession OpenSystemSession()
        {
            return SystemSessionFactory.OpenSession();
        }
    }
}
