using UnitTests.Helper;
using Xunit;

namespace UnitTests.BaseClasses
{
    [CollectionDefinition("DB Test")]
    public class DbTestCollection : ICollectionFixture<RamDrive>
    {
        public const string CollectionName = "DB Test";
    }
}