using System;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Xunit;

namespace UnitTests
{
    public class PermissionConversionTests
    {
        public PermissionConversionTests()
        {
        }

        [Fact]
        public void FlagConversionTest()
        {
            var p = new Permission() {EditArticleSettings = true, CanScan = true};

            var flags = (PermissionFlags) Convert.ToInt32(p.ToBits(), 2);
            Assert.Equal(PermissionFlags.EditArticleSettings | PermissionFlags.CanScan, flags);
        }
    }
}