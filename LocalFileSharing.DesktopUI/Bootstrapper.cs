using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Caliburn.Micro;

using LocalFileSharing.DesktopUI.ViewModels;

using Unity;

namespace LocalFileSharing.DesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private IUnityContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new UnityContainer();

            _container
                .RegisterType<IWindowManager, WindowManager>(TypeLifetime.Singleton)
                .RegisterType<IEventAggregator, EventAggregator>(TypeLifetime.Singleton)
                .RegisterInstance(_container, InstanceLifetime.Singleton);

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType =>
                {
                    _container.RegisterType(viewModelType, viewModelType.ToString());
                });
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.Resolve(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.ResolveAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
