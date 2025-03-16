using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.EnrollmentAggregate;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class User : AggregateRoot<UserId>
    {
        private readonly List<Role> _roles = [];

        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

        public DateTime? LastLoginAt { get; private set; } = null;
        public bool IsActive { get; private set; } = true;

        // Navigation properties for EF Core
        public virtual UserProfile? Profile { get; private set; }
        public virtual UserSettings? Settings { get; private set; }
        public virtual ICollection<Enrollment> Enrollments { get; private set; } = [];
        public virtual ICollection<Course> CreatedCourses { get; private set; } = [];
        public virtual ICollection<SkillAssessment> SkillAssessments { get; private set; } = [];
        public virtual ICollection<SkillAssessment> VerifiedSkillAssessments { get; private set; } = [];

        private User() { } // For EF Core

        private User(UserId id, string email, string firstName, string lastName) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(email, nameof(email), "Email cannot be empty");
            Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName), "First name cannot be empty");
            Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName), "Last name cannot be empty");

            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public static User Create(string email, string firstName, string lastName)
        {
            var user = new User(UserId.CreateUnique(), email, firstName, lastName);
            user.AddDomainEvent(new UserCreatedEvent(user.Id));
            return user;
        }

        public void AddRole(Role role)
        {
            Guard.Against.Null(role, nameof(role));

            if (!_roles.Contains(role))
            {
                _roles.Add(role);
                AddDomainEvent(new UserRoleAddedEvent(Id, role.Id));
            }
        }

        public void RemoveRole(Role role)
        {
            Guard.Against.Null(role, nameof(role));

            if (!_roles.Contains(role))
            {
                return;
            }
            _roles.Remove(role);
            AddDomainEvent(new UserRoleRemovedEvent(Id, role.Id));
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName), "First name cannot be empty");
            Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName), "Last name cannot be empty");

            FirstName = firstName;
            LastName = lastName;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new UserProfileUpdatedEvent(Id));
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        public UserProfile CreateProfile()
        {
            var profile = UserProfile.Create(Id);
            Profile = profile;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new UserProfileCreatedEvent(Id, profile.Id));
            return profile;
        }

        public UserSettings CreateSettings()
        {
            var settings = UserSettings.Create(Id);
            Settings = settings;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new UserSettingsCreatedEvent(Id, settings.Id));
            return settings;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                ModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new UserActivatedEvent(Id));
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                ModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new UserDeactivatedEvent(Id));
            }
        }

        public SkillAssessment AssessSkill(Skill skill, ProficiencyLevel level, string evidence = "")
        {
            Guard.Against.Null(skill, nameof(skill));

            var assessment = SkillAssessment.Create(Id, skill.Id, level, evidence);
            AddDomainEvent(new UserSkillAssessedEvent(Id, skill.Id, assessment.Id));
            return assessment;
        }

        public void VerifySkillAssessment(SkillAssessment assessment)
        {
            Guard.Against.Null(assessment, nameof(assessment));

            // A user cannot verify their own assessment
            if (assessment.UserId == Id)
            {
                throw new InvalidOperationException("Users cannot verify their own skill assessments");
            }

            assessment.Verify(Id);
            // Note: The actual collection management would happen in the repository
        }
    }
}
