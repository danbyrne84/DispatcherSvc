using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using SaleCycle.Svc.Dispatcher.Contract;
using SaleCycle.Svc.Dispatcher.Contract.Extensions;

namespace SaleCycle.Svc.Dispatcher.Factories
{
    public static class SmsDispatcherFactory
    {
        private static List<Type> Dispatchers { get; set; }
        private static readonly ILogger Logger = Program.Container.GetInstance<ILogger>();

        // retrieve registered dispatcher by name
        public static ISmsDispatcher GetDispatcherByName(string dispatcherName)
        {
            ISmsDispatcher instance = null;
            try
            {
                var dispatcherType = Dispatchers.FirstOrDefault(x => x.FullName.ToLowerInvariant().Contains(dispatcherName.ToLowerInvariant()));
                if (dispatcherType == null)
                {
                    Logger.Error("Unable to retrieve dispatcher {0} - no dispatcher with that name has been registered".Fmt(dispatcherName));
                    return null;
                }

                instance = (ISmsDispatcher) Activator.CreateInstance(dispatcherType);
            }
            catch
            {
                Logger.Error("Unable to create an instance of {0}".Fmt(dispatcherName));
            }
            return instance;
        }

        // touch the dispatcher plugins so JIT optimization doesn't miss them
        public static void PreloadDispatchers()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.Info("Loading dispatchers from {0}".Fmt(path));

            if (path != null && Directory.Exists(path))
            {
                foreach (var dll in Directory.GetFiles(path, "SaleCycle.Dispatchers*.dll"))
                {
                    Logger.Info("Loading dispatcher {0}".Fmt(dll));

                    try
                    {
                        Assembly.LoadFile(dll);
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("Unable to load dispatcher {0} - {1}".Fmt(dll, ex));
                    }
                }                
            }
            else
            {
                Logger.Error("Unable to access current working directory {0}".Fmt(path));
            }

            // assign dispatcher types to lookup list
            Dispatchers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IDispatcher<ISmsDispatch>).IsAssignableFrom(p))
                .Where(p => p.IsClass)
                .ToList();
        }
    }
}
