using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TidbeatTests2._0
{
    public class MockServiceProvider : IServiceProvider
    {
        private readonly Mock<IServiceProvider> _mock;
        public MockServiceProvider()
        {
            _mock = new Mock<IServiceProvider>();
        }

        public object? GetService(Type serviceType)
        {
            var fixture = new ApplicationDbContextFixture();
            return fixture.UserManager;
        }
    }
}
