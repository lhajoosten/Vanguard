using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vanguard.Common.Base;

namespace Vanguard.UnitTests.Common
{
    // A simple value object ID type
    public struct TestId
    {
        public Guid Value { get; }
        public TestId(Guid value) => Value = value;
    }

    // A related value object ID type for foreign key testing
    public struct RelatedId
    {
        public Guid Value { get; }
        public RelatedId(Guid value) => Value = value;
    }

    // Test entity with a value object primary key and a foreign key property.
    public class TestEntity
    {
        public TestId Id { get; set; }
        public RelatedId RelatedId { get; set; }
    }

    // Concrete configuration for TestEntity using our base configuration.
    public class TestEntityConfiguration : EntityBaseConfiguration<TestEntity, TestId>
    {
        // No additional configuration is needed;
        // the base implementation will set up the key and converters.
    }

    // An entity that uses Guid directly for testing direct Guid conversion.
    public class DirectGuidEntity
    {
        public Guid Id { get; set; }
    }

    // Configuration for an entity with Guid as primary key.
    public class DirectGuidConfiguration : EntityBaseConfiguration<DirectGuidEntity, Guid>
    {
    }

    public class EntityBaseConfigurationTests
    {
        [Fact]
        public void Configure_SetsPrimaryKey()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            modelBuilder.ApplyConfiguration(new TestEntityConfiguration());

            // Act
            var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

            // Assert
            Assert.NotNull(entityType);
            var primaryKey = entityType.FindPrimaryKey();
            Assert.NotNull(primaryKey);
            Assert.Contains(primaryKey.Properties, p => p.Name == "Id");
        }

        [Fact]
        public void Configure_SetsPrimaryKeyValueConverter_WhenTIdMatches()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
            var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

            // Act
            var idProperty = entityType.FindProperty("Id");

            // Assert
            Assert.NotNull(idProperty);
            var converter = idProperty.GetValueConverter();
            Assert.NotNull(converter);
            // Verify that the converter is of the expected generic type.
            Assert.IsType<ValueConverter<TestId, Guid>>(converter);
        }

        [Fact]
        public void PublicConvertIdToGuid_ReturnsGuid_ForDirectGuid()
        {
            // Arrange
            var testConfig = new TestableDirectGuidConfiguration();
            var guid = Guid.NewGuid();

            // Act
            var result = testConfig.PublicConvertIdToGuid(guid);

            // Assert
            Assert.Equal(guid, result);
        }

        [Fact]
        public void PublicConvertGuidToId_ReturnsId_ForDirectGuid()
        {
            // Arrange
            var testConfig = new TestableDirectGuidConfiguration();
            var guid = Guid.NewGuid();

            // Act
            var result = testConfig.PublicConvertGuidToId(guid);

            // Assert
            Assert.Equal(guid, result);
        }
    }

    public class TestableDirectGuidConfiguration : DirectGuidConfiguration
    {
        public Guid PublicConvertIdToGuid(Guid id)
        {
            return base.ConvertIdToGuid(id);
        }

        public Guid PublicConvertGuidToId(Guid value)
        {
            return base.ConvertGuidToId(value);
        }
    }
}