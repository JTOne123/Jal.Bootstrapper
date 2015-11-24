﻿using Jal.Bootstrapper.Impl;
using Jal.Bootstrapper.Interface;
using Jal.Bootstrapper.Tests.Impl;
using NUnit.Framework;
using Shouldly;

namespace Jal.Bootstrapper.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Configure_WithCompositeBootstrapper_True()
        {
            var bootstrapper = new DoSomethingBootstrapper();

            var composite = new CompositeBootstrapper(new IBootstrapper[] { bootstrapper });

            composite.Configure();

            composite.Result.ShouldBe(true);

            bootstrapper.Result.ShouldBe(true);
        }
    }
}
