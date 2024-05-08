using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using Pathway.Core.Entity;

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
                    configuration.Configure();
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
                    configuration.Configure();
                    configuration
                        .AddAssembly(typeof(PvCollectEntity).Assembly)
                        .AddAssembly(typeof(PvCPUBusyEntity).Assembly)
                        .AddAssembly(typeof(PvCPUManyEntity).Assembly)
                        .AddAssembly(typeof(PvErrorEntity).Assembly)
                        .AddAssembly(typeof(PvPwyListEntity).Assembly)
                        .AddAssembly(typeof(PvPwyManyEntity).Assembly)
                        .AddAssembly(typeof(PvScInfoEntity).Assembly);

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
