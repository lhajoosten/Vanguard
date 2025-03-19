namespace Vanguard.Common.Base
{
    public record UserId(Guid Value)
    {
        public static UserId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(UserId id) => id.Value;
    }

    public record RoleId(Guid Value)
    {
        public static RoleId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(RoleId id) => id.Value;
    }

    public record PermissionId(Guid Value)
    {
        public static PermissionId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(PermissionId id) => id.Value;
    }

    public record SkillId(Guid Value)
    {
        public static SkillId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillId id) => id.Value;
    }

    public record SkillCategoryId(Guid Value)
    {
        public static SkillCategoryId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillCategoryId id) => id.Value;
    }

    public record CourseId(Guid Value)
    {
        public static CourseId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(CourseId id) => id.Value;
    }

    public record ModuleId(Guid Value)
    {
        public static ModuleId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(ModuleId id) => id.Value;
    }

    public record LessonId(Guid Value)
    {
        public static LessonId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(LessonId id) => id.Value;
    }

    public record EnrollmentId(Guid Value)
    {
        public static EnrollmentId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(EnrollmentId id) => id.Value;
    }

    public record UserSettingsId(Guid Value)
    {
        public static UserSettingsId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(UserSettingsId id) => id.Value;
    }

    public record UserProfileId(Guid Value)
    {
        public static UserProfileId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(UserProfileId id) => id.Value;
    }

    public record SkillAssessmentId(Guid Value)
    {
        public static SkillAssessmentId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillAssessmentId id) => id.Value;
    }

    public record SkillAssessmentQuestionId(Guid Value)
    {
        public static SkillAssessmentQuestionId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillAssessmentQuestionId id) => id.Value;
    }

    public record SkillAssessmentAnswerId(Guid Value)
    {
        public static SkillAssessmentAnswerId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillAssessmentAnswerId id) => id.Value;
    }

    public record SkillAssessmentResultId(Guid Value)
    {
        public static SkillAssessmentResultId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillAssessmentResultId id) => id.Value;
    }

    public record CompletionRequirementId(Guid Value)
    {
        public static CompletionRequirementId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(CompletionRequirementId id) => id.Value;
    }

    public record EnrollmentCertificateId(Guid Value)
    {
        public static EnrollmentCertificateId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(EnrollmentCertificateId id) => id.Value;
    }

    public record CourseTagId(Guid Value)
    {
        public static CourseTagId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(CourseTagId id) => id.Value;
    }

    public record SkillAssessmentQuestionOptionId(Guid Value)
    {
        public static SkillAssessmentQuestionOptionId CreateUnique() => new(Guid.NewGuid());
        public static implicit operator Guid(SkillAssessmentQuestionOptionId id) => id.Value;
    }
}
