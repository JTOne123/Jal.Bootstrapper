﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Jal.Bootstrapper.Interface;

namespace Jal.Bootstrapper.CastleWindsor
{
    public class IgnoreWindsorBootstrapper : IBootstrapper<IWindsorContainer>
    {
        private readonly IEnumerable<IWindsorInstaller> _specificInstallers;

        private readonly Action<IWindsorContainer> _setupAction;

        private readonly string[] _ignoredAssemblies;

        public IgnoreWindsorBootstrapper(IEnumerable<IWindsorInstaller> specificInstallers, Action<IWindsorContainer> setupAction, string[] ignoredAssemblies=null)
        {
            _specificInstallers = specificInstallers;
            _setupAction = setupAction;
            _ignoredAssemblies = ignoredAssemblies;
        }

        public void Configure()
        {
            var container = new WindsorContainer();

            var assemblies = AssemblyFinder.Impl.AssemblyFinder.Current.GetAssemblies();

            var filteredAssemblies = new List<Assembly>();

            foreach (var assembly in assemblies)
            {
                if (_ignoredAssemblies == null || !_ignoredAssemblies.Contains(assembly.GetName().Name))
                {
                    filteredAssemblies.Add(assembly);
                }
            }
            var installers = AssemblyFinder.Impl.AssemblyFinder.Current.GetInstancesOf<IWindsorInstaller>(filteredAssemblies.ToArray());
            
            var selectedInstallers = new List<IWindsorInstaller>();

            foreach (var installer in installers)
            {
                var type = installer.GetType().Name;

                if (_specificInstallers.All(x => x.GetType().Name != type))
                {
                    selectedInstallers.Add(installer);
                }
                
            }

            if (_specificInstallers != null)
            {
                selectedInstallers.AddRange(_specificInstallers);
            }
            
            _setupAction(container);
            
            container.Install(selectedInstallers.ToArray());
            
            Result = container;
        }

        public IWindsorContainer Result { get; private set; }
    }
}