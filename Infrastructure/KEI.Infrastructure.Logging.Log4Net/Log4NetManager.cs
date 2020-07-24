using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net;
using Prism.Ioc;

namespace KEI.Infrastructure
{
    public class Log4NetManager : ILogManager
    {

        public ILogger DefaultLogger { get; private set; }

        public Log4NetManager()
        {
            Configurator.ReadConfiguration();
            log4net.Config.XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(@"Configs\log4net.config"));
            DefaultLogger = GetLogger("System");
        }

        #region ILogManager Members

        public ILogger GetLogger([CallerFilePath] string p_strName = "")
        {
            return new Log4NetLogger(LogManager.GetLogger(Assembly.GetCallingAssembly(), Path.GetFileName(p_strName)));
        }

        public ILogger GetLogger(Type p_Type)
        {
            return new Log4NetLogger(LogManager.GetLogger(p_Type));
        }

        private ILogConfigurator m_Configurator;
        public ILogConfigurator Configurator
        {
            get
            {
                if (m_Configurator == null)
                {
                    m_Configurator = new Log4NetConfigurator();
                }

                return m_Configurator;
            }
        }

        #endregion
    }

    public static class ContainerRegistryExtentions
    {
        public static void RegisterLog4Net(this IContainerRegistry registry)
        {
            registry.RegisterSingleton<ILogManager, Log4NetManager>();
        }
    }
}
