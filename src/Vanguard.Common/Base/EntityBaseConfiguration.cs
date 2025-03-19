using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Vanguard.Common.Base
{
    /// <summary>
    /// Base configuration class for entities with specific ID types
    /// </summary>
    public abstract class EntityBaseConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
        where TEntity : class
        where TId : notnull
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Ensure primary key is configured
            ConfigurePrimaryKey(builder);

            // Configure primary key conversion
            ConfigurePrimaryKeyConversion(builder);

            // Configure foreign key conversions
            ConfigureForeignKeyConversions(builder);
        }

        /// <summary>
        /// Configures the primary key
        /// </summary>
        protected virtual void ConfigurePrimaryKey(EntityTypeBuilder<TEntity> builder)
        {
            // By default, use the Id property as the primary key
            // This is overridden in entity-specific configurations
            builder.HasKey("Id");
        }

        /// <summary>
        /// Configures the primary key conversion
        /// </summary>
        protected virtual void ConfigurePrimaryKeyConversion(EntityTypeBuilder<TEntity> builder)
        {
            // Try to find the Id property by convention
            var idProperty = builder.Metadata.FindProperty("Id");

            if (idProperty != null && idProperty.ClrType == typeof(TId))
            {
                // Set up value converter for the primary key
                idProperty.SetValueConverter(
                    CreateIdValueConverter()
                );
            }
        }

        /// <summary>
        /// Creates a value converter for the ID type
        /// </summary>
        protected virtual ValueConverter<TId, Guid> CreateIdValueConverter()
        {
            return new ValueConverter<TId, Guid>(
                id => ConvertIdToGuid(id),
                value => ConvertGuidToId(value)
            );
        }

        /// <summary>
        /// Configures conversions for foreign key properties
        /// </summary>
        protected virtual void ConfigureForeignKeyConversions(EntityTypeBuilder<TEntity> builder)
        {
            // Find properties ending with "Id" that might be foreign keys
            // Exclude Guid types and generic types
            var foreignKeyProperties = builder.Metadata.GetProperties()
                .Where(p => p.Name.EndsWith("Id") &&
                            p.ClrType != typeof(Guid) &&
                            p.ClrType.IsGenericType == false);

            foreach (var property in foreignKeyProperties)
            {
                // Skip the primary key which is handled separately
                if (property.Name == "Id")
                    continue;

                // Check if this is a value object ID type
                if (!IsValueObjectIdType(property.ClrType))
                    continue;

                // Create a generic converter for this property
                var converterType = typeof(ValueConverter<,>).MakeGenericType(property.ClrType, typeof(Guid));

                // Find the appropriate conversion methods
                var toMethod = GetConversionMethod(property.ClrType, typeof(Guid));
                var fromMethod = GetConversionMethod(typeof(Guid), property.ClrType);

                if (toMethod != null && fromMethod != null)
                {
                    // Create the lambda expressions for conversions
                    var idParam = Expression.Parameter(property.ClrType, "id");
                    var guidParam = Expression.Parameter(typeof(Guid), "guid");

                    var toConversion = Expression.Lambda(
                        Expression.Call(null, toMethod, idParam),
                        idParam);

                    var fromConversion = Expression.Lambda(
                        Expression.Call(null, fromMethod, guidParam),
                        guidParam);

                    // Create the converter instance
                    var converter = Activator.CreateInstance(
                        converterType,
                        toConversion,
                        fromConversion,
                        null);

                    if (converter != null)
                    {
                        property.SetValueConverter((ValueConverter)converter);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a type is a value object ID type
        /// </summary>
        protected bool IsValueObjectIdType(Type type)
        {
            return type.Name.EndsWith("Id") &&
                   type.GetProperty("Value")?.PropertyType == typeof(Guid);
        }

        /// <summary>
        /// Finds a method to convert between types
        /// </summary>
        private MethodInfo GetConversionMethod(Type sourceType, Type targetType)
        {
            // Try to find a specific conversion method in this class
            var methodName = $"Convert{sourceType.Name}To{targetType.Name}";
            var method = GetType().GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (method != null)
                return method;

            // For Guid to ID conversions, try to find a constructor
            if (targetType.Name.EndsWith("Id") && sourceType == typeof(Guid))
            {
                var constructor = targetType.GetConstructor([typeof(Guid)]);
                if (constructor != null)
                {
                    // Create a dynamic method that calls the constructor
                    var dynamicMethod = new DynamicMethod($"Create{targetType.Name}FromGuid",
                        targetType, [typeof(Guid)], true);

                    var il = dynamicMethod.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Newobj, constructor);
                    il.Emit(OpCodes.Ret);

                    return dynamicMethod;
                }
            }

            // For ID to Guid conversions, look for a Value property
            if (sourceType.Name.EndsWith("Id") && targetType == typeof(Guid))
            {
                var valueProperty = sourceType.GetProperty("Value");
                if (valueProperty != null && valueProperty.PropertyType == typeof(Guid))
                {
                    // Create a dynamic method that gets the Value property
                    var dynamicMethod = new DynamicMethod($"Get{sourceType.Name}Value",
                        typeof(Guid), [sourceType], true);

                    var il = dynamicMethod.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, valueProperty.GetMethod!);
                    il.Emit(OpCodes.Ret);

                    return dynamicMethod;
                }
            }

            return null!;
        }

        /// <summary>
        /// Converts the ID to Guid 
        /// </summary>
        protected virtual Guid ConvertIdToGuid(TId id)
        {
            // For direct Guid types
            if (id is Guid guidId)
                return guidId;

            // For value object ID types
            var valueProperty = id.GetType().GetProperty("Value");
            if (valueProperty != null)
            {
                var value = valueProperty.GetValue(id);
                if (value is Guid guidValue)
                    return guidValue;
            }

            throw new InvalidOperationException($"Cannot convert {typeof(TId).Name} to Guid");
        }

        /// <summary>
        /// Converts Guid back to ID
        /// </summary>
        protected virtual TId ConvertGuidToId(Guid value)
        {
            // For direct Guid types
            if (typeof(TId) == typeof(Guid))
                return (TId)(object)value;

            // Try to use a constructor
            var constructor = typeof(TId).GetConstructor([typeof(Guid)]);
            if (constructor != null)
            {
                return (TId)constructor.Invoke([value]);
            }

            // Try a static factory method
            var createMethod = typeof(TId).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            if (createMethod != null && createMethod.GetParameters().Length == 1 &&
                createMethod.GetParameters()[0].ParameterType == typeof(Guid))
            {
                return (TId)createMethod.Invoke(null, [value])!;
            }

            throw new InvalidOperationException($"Cannot convert Guid to {typeof(TId).Name}");
        }

        /// <summary>
        /// Helper method to configure a one-to-many relationship
        /// </summary>
        protected void ConfigureOneToMany<TRelatedEntity>(
            EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> navigationExpression,
            Expression<Func<TRelatedEntity, TEntity>> inverseNavigation,
            DeleteBehavior deleteBehavior = DeleteBehavior.Cascade)
            where TRelatedEntity : class
        {
            builder.HasMany(navigationExpression!)
                .WithOne(inverseNavigation!)
                .OnDelete(deleteBehavior);
        }
    }
}