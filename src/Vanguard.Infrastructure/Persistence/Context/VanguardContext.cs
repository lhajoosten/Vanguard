using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;
using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.EnrollmentAggregate;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Context
{
    public class VanguardContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDomainEventService _domainEventService;

        public VanguardContext(DbContextOptions<VanguardContext> options, ILoggerFactory loggerFactory, IDomainEventService domainEventService)
            : base(options)
        {
            Guard.Against.Null(loggerFactory, nameof(loggerFactory));
            Guard.Against.Null(domainEventService, nameof(domainEventService));

            _loggerFactory = loggerFactory;
            _domainEventService = domainEventService;
        }

        // User aggregate
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();

        // Skill aggregate
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
        public DbSet<SkillAssessment> SkillAssessments => Set<SkillAssessment>();
        public DbSet<SkillAssessmentAnswer> SkillAssessmentAnswers => Set<SkillAssessmentAnswer>();
        public DbSet<SkillAssessmentQuestion> SkillAssessmentQuestions => Set<SkillAssessmentQuestion>();
        public DbSet<SkillAssessmentResult> SkillAssessmentResults => Set<SkillAssessmentResult>();

        // Course aggregate
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseModule> Modules => Set<CourseModule>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<CourseTag> CourseTags => Set<CourseTag>();

        // Enrollment aggregate
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<EnrollmentCertificate> EnrollmentCertificates => Set<EnrollmentCertificate>();
        public DbSet<CompletionRequirement> CompletionRequirements => Set<CompletionRequirement>();

        // Enumeration Types
        public DbSet<ProficiencyLevel> ProficiencyLevels => Set<ProficiencyLevel>();
        public DbSet<DifficultyLevel> DifficultyLevels => Set<DifficultyLevel>();
        public DbSet<Language> Languages => Set<Language>();
        public DbSet<LessonType> LessonTypes => Set<LessonType>();
        public DbSet<UITheme> UIThemes => Set<UITheme>();
        public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();
        public DbSet<CompletionCriteria> CompletionCriterias => Set<CompletionCriteria>();
        public DbSet<ProfileVisibility> ProfileVisibilities => Set<ProfileVisibility>();
        public DbSet<EnrollmentStatus> EnrollmentStatuses => Set<EnrollmentStatus>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: Ignore all ID value objects before applying configurations
            IgnoreTypedIds(modelBuilder);

            base.OnModelCreating(modelBuilder);

            //// Apply entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update creation/modification timestamps
            UpdateTimestamps();

            // Save changes to database
            var result = await base.SaveChangesAsync(cancellationToken);

            // Dispatch domain events after saving changes
            await DispatchEventsAsync(cancellationToken);

            return result;
        }

        private async Task DispatchEventsAsync(CancellationToken cancellationToken)
        {
            // If no event service is available, skip dispatching
            if (_domainEventService == null)
                return;

            // Get all entities that are being tracked and have domain events
            var aggregateRoots = ChangeTracker.Entries<IAggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count != 0)
                .ToList();

            // Dispatch events for each aggregate
            foreach (var aggregateRoot in aggregateRoots)
            {
                await _domainEventService.PublishEventsAsync(aggregateRoot, cancellationToken);
                aggregateRoot.ClearDomainEvents();
            }
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries<IEntity>();
            var now = DateTime.UtcNow;

            foreach (var entry in entities)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(e => e.CreatedAt).CurrentValue = now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(e => e.ModifiedAt).CurrentValue = now;
                }
            }
        }

        private static void IgnoreTypedIds(ModelBuilder modelBuilder)
        {
            // IMPORTANT: Ignore all ID value objects before applying configurations
            modelBuilder.Ignore<CourseId>();
            modelBuilder.Ignore<ModuleId>();
            modelBuilder.Ignore<LessonId>();
            modelBuilder.Ignore<UserId>();
            modelBuilder.Ignore<RoleId>();
            modelBuilder.Ignore<PermissionId>();
            modelBuilder.Ignore<SkillId>();
            modelBuilder.Ignore<SkillCategoryId>();
            modelBuilder.Ignore<SkillAssessmentId>();
            modelBuilder.Ignore<SkillAssessmentQuestionId>();
            modelBuilder.Ignore<SkillAssessmentQuestionOptionId>();
            modelBuilder.Ignore<SkillAssessmentAnswerId>();
            modelBuilder.Ignore<SkillAssessmentResultId>();
            modelBuilder.Ignore<EnrollmentId>();
            modelBuilder.Ignore<CompletionRequirementId>();
            modelBuilder.Ignore<EnrollmentCertificateId>();
            modelBuilder.Ignore<UserProfileId>();
            modelBuilder.Ignore<UserSettingsId>();
            modelBuilder.Ignore<CourseTagId>();
        }
    }
}
