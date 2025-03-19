using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Infrastructure.Persistence.Context;

namespace Vanguard.Infrastructure.Persistence.Services
{
    public class EnumDataSeeder(ILogger<EnumDataSeeder> logger)
    {
        private readonly ILogger<EnumDataSeeder> _logger = logger;

        public async Task SeedAsync(VanguardContext context)
        {
            _logger.LogInformation("Checking if enumeration data needs to be seeded");

            // Check for each enumeration type and seed if necessary
            if (!await context.ProficiencyLevels.AnyAsync())
            {
                _logger.LogInformation("Seeding ProficiencyLevels");
                await SeedProficiencyLevels(context);
            }

            if (!await context.DifficultyLevels.AnyAsync())
            {
                _logger.LogInformation("Seeding DifficultyLevels");
                await SeedDifficultyLevels(context);
            }

            if (!await context.Languages.AnyAsync())
            {
                _logger.LogInformation("Seeding Languages");
                await SeedLanguages(context);
            }

            if (!await context.LessonTypes.AnyAsync())
            {
                _logger.LogInformation("Seeding LessonTypes");
                await SeedLessonTypes(context);
            }

            if (!await context.UIThemes.AnyAsync())
            {
                _logger.LogInformation("Seeding UIThemes");
                await SeedUIThemes(context);
            }

            if (!await context.QuestionTypes.AnyAsync())
            {
                _logger.LogInformation("Seeding QuestionTypes");
                await SeedQuestionTypes(context);
            }

            if (!await context.CompletionCriterias.AnyAsync())
            {
                _logger.LogInformation("Seeding CompletionCriterias");
                await SeedCompletionCriterias(context);
            }

            if (!await context.ProfileVisibilities.AnyAsync())
            {
                _logger.LogInformation("Seeding ProfileVisibilities");
                await SeedProfileVisibilities(context);
            }

            if (!await context.EnrollmentStatuses.AnyAsync())
            {
                _logger.LogInformation("Seeding EnrollmentStatuses");
                await SeedEnrollmentStatuses(context);
            }

            // Save all changes
            await context.SaveChangesAsync();
            _logger.LogInformation("Enumeration data seeding completed");
        }

        private static async Task SeedProficiencyLevels(VanguardContext context)
        {
            await context.ProficiencyLevels.AddRangeAsync([
                ProficiencyLevel.Beginner,
                ProficiencyLevel.Intermediate,
                ProficiencyLevel.Advanced,
                ProficiencyLevel.Expert
            ]);
        }

        private static async Task SeedDifficultyLevels(VanguardContext context)
        {
            await context.DifficultyLevels.AddRangeAsync([
                DifficultyLevel.Easy,
                DifficultyLevel.Medium,
                DifficultyLevel.Hard,
                DifficultyLevel.Expert
            ]);
        }

        private static async Task SeedLanguages(VanguardContext context)
        {
            await context.Languages.AddRangeAsync([
                Language.English,
                Language.Spanish,
                Language.French,
                Language.German,
                Language.Chinese,
                Language.Japanese,
                Language.Arabic,
                Language.Russian,
                Language.Portuguese,
                Language.Hindi
            ]);
        }

        private static async Task SeedLessonTypes(VanguardContext context)
        {
            await context.LessonTypes.AddRangeAsync([
                LessonType.Video,
                LessonType.Text,
                LessonType.Quiz,
                LessonType.Assignment,
                LessonType.Discussion,
                LessonType.Presentation,
                LessonType.LiveSession,
                LessonType.Simulation
            ]);
        }

        private static async Task SeedUIThemes(VanguardContext context)
        {
            await context.UIThemes.AddRangeAsync([
                UITheme.Light,
                UITheme.Dark,
                UITheme.System,
                UITheme.Sepia,
                UITheme.HighContrast,
                UITheme.BlueLight
            ]);
        }

        private static async Task SeedQuestionTypes(VanguardContext context)
        {
            await context.QuestionTypes.AddRangeAsync([
                QuestionType.SingleChoice,
                QuestionType.MultipleChoice,
                QuestionType.OpenText,
                QuestionType.TrueFalse,
                QuestionType.Matching,
                QuestionType.Ordering,
                QuestionType.FillInBlank,
                QuestionType.Hotspot
            ]);
        }

        private static async Task SeedCompletionCriterias(VanguardContext context)
        {
            await context.CompletionCriterias.AddRangeAsync([
                CompletionCriteria.PercentageComplete,
                CompletionCriteria.LessonsCompleted,
                CompletionCriteria.MinimumGrade,
                CompletionCriteria.MinimumDaysActive,
                CompletionCriteria.AssignmentsCompleted,
                CompletionCriteria.QuizzesCompleted,
                CompletionCriteria.PassFinalExam,
                CompletionCriteria.ParticipateInDiscussions
            ]);
        }

        private static async Task SeedProfileVisibilities(VanguardContext context)
        {
            await context.ProfileVisibilities.AddRangeAsync([
                ProfileVisibility.Public,
                ProfileVisibility.Private,
                ProfileVisibility.FriendsOnly,
                ProfileVisibility.Enrolled,
                ProfileVisibility.Verified
            ]);
        }

        private static async Task SeedEnrollmentStatuses(VanguardContext context)
        {
            await context.EnrollmentStatuses.AddRangeAsync([
                EnrollmentStatus.Active,
                EnrollmentStatus.Completed,
                EnrollmentStatus.Dropped,
                EnrollmentStatus.OnHold,
                EnrollmentStatus.Expired,
                EnrollmentStatus.Pending
            ]);
        }
    }

    public class CustomDataSeeder(ILogger<CustomDataSeeder> logger)
    {
        private readonly ILogger<CustomDataSeeder> _logger = logger;

        public async Task SeedAsync(VanguardContext context)
        {
            // Seed roles and permissions if none exist
            if (!await context.Roles.AnyAsync())
            {
                _logger.LogInformation("Seeding roles and permissions");
                await SeedRolesAndPermissions(context);
            }

            // Seed skill categories if none exist
            if (!await context.SkillCategories.AnyAsync())
            {
                _logger.LogInformation("Seeding skill categories");
                await SeedSkillCategories(context);
            }

            // Seed course tags if none exist
            if (!await context.CourseTags.AnyAsync())
            {
                _logger.LogInformation("Seeding course tags");
                await SeedCourseTags(context);
            }

            // Save all changes
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAndPermissions(VanguardContext context)
        {
            // Create permissions
            var createCoursePermission = Permission.Create("CreateCourse", "Permission to create courses");
            var editCoursePermission = Permission.Create("EditCourse", "Permission to edit courses");
            var publishCoursePermission = Permission.Create("PublishCourse", "Permission to publish courses");
            var enrollPermission = Permission.Create("Enroll", "Permission to enroll in courses");
            var createSkillPermission = Permission.Create("CreateSkill", "Permission to create skills");
            var verifySkillPermission = Permission.Create("VerifySkill", "Permission to verify skill assessments");

            await context.Permissions.AddRangeAsync(
                [
                    createCoursePermission,
                    editCoursePermission,
                    publishCoursePermission,
                    enrollPermission,
                    createSkillPermission,
                    verifySkillPermission
                ]);

            // Create roles
            var adminRole = Role.Create("Administrator");
            adminRole.AddPermission(createCoursePermission);
            adminRole.AddPermission(editCoursePermission);
            adminRole.AddPermission(publishCoursePermission);
            adminRole.AddPermission(enrollPermission);
            adminRole.AddPermission(createSkillPermission);
            adminRole.AddPermission(verifySkillPermission);

            var instructorRole = Role.Create("Instructor");
            instructorRole.AddPermission(createCoursePermission);
            instructorRole.AddPermission(editCoursePermission);
            instructorRole.AddPermission(publishCoursePermission);

            var studentRole = Role.Create("Student");
            studentRole.AddPermission(enrollPermission);

            await context.Roles.AddRangeAsync(
                [adminRole, instructorRole, studentRole]);
        }

        private static async Task SeedSkillCategories(VanguardContext context)
        {
            var categories = new[]
            {
                SkillCategory.Create("Programming", "Programming and software development skills"),
                SkillCategory.Create("Design", "Graphic design, UI/UX, and visual design skills"),
                SkillCategory.Create("Business", "Business, management, and entrepreneurship skills"),
                SkillCategory.Create("Marketing", "Digital marketing, SEO, and social media skills"),
                SkillCategory.Create("Data Science", "Data analysis, machine learning, and statistics skills")
            };

            await context.SkillCategories.AddRangeAsync(categories);
        }

        private static async Task SeedCourseTags(VanguardContext context)
        {
            var tags = new[]
            {
                CourseTag.Create("Beginner", "Courses suitable for beginners"),
                CourseTag.Create("Intermediate", "Courses for those with some experience"),
                CourseTag.Create("Advanced", "Courses for advanced learners"),
                CourseTag.Create("Programming", "Courses related to programming"),
                CourseTag.Create("Design", "Courses related to design"),
                CourseTag.Create("Business", "Courses related to business"),
                CourseTag.Create("Marketing", "Courses related to marketing"),
                CourseTag.Create("Data Science", "Courses related to data science")
            };

            await context.CourseTags.AddRangeAsync(tags);
        }
    }
}
