using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using Pathway.Core.Entity;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using Pathway.Core.Mapping;

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
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PathwayAlertsEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PathwayAllAlertsEntity>())
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

                    Fluently.Configure(configuration)
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvAlertEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvCollectEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvCPUBusyEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvCPUManyEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvErrorEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvPwyListEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvPwyManyEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvScInfoEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvScLStatEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvScPrStusEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvScStusEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvScTStatEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvTcpInfoEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvTcpStatEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvTcpStusEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvTermStatEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PvTermStusEntity>())
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<TrendPathwayHourlyEntity>())
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, false));

                    /*var configuration = new Configuration();
                    configuration.Configure("system.hibernate.cfg.xml");
                    configuration
                        .AddAssembly(typeof(PvAlertEntity).Assembly)
                        .AddAssembly(typeof(PvCollectEntity).Assembly)
                        .AddAssembly(typeof(PvCPUBusyEntity).Assembly)
                        .AddAssembly(typeof(PvCPUManyEntity).Assembly)
                        .AddAssembly(typeof(PvErrorEntity).Assembly)
                        .AddAssembly(typeof(PvPwyListEntity).Assembly)
                        .AddAssembly(typeof(PvPwyManyEntity).Assembly)
                        .AddAssembly(typeof(PvScInfoEntity).Assembly)
                        .AddAssembly(typeof(PvScLStatEntity).Assembly)
                        .AddAssembly(typeof(PvScPrStusEntity).Assembly)
                        .AddAssembly(typeof(PvScStusEntity).Assembly)
                        .AddAssembly(typeof(PvScTStatEntity).Assembly)
                        .AddAssembly(typeof(PvTcpInfoEntity).Assembly)
                        .AddAssembly(typeof(PvTcpStatEntity).Assembly)
                        .AddAssembly(typeof(PvTcpStusEntity).Assembly)
                        .AddAssembly(typeof(PvTermStatEntity).Assembly)
                        .AddAssembly(typeof(PvTermStusEntity).Assembly)
                        .AddAssembly(typeof(TrendPathwayHourlyEntity).Assembly);*/

                    _systemSessionFactory = configuration.BuildSessionFactory();
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
