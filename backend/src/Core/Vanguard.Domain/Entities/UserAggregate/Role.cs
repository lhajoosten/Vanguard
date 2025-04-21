using Vanguard.Core.Base;
using Vanguard.Domain.Enums;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class Role : Enumeration
    {
        public static readonly Role Admin = new AdminRole();
        public static readonly Role ContentManager = new ContentManagerRole();
        public static readonly Role ContentEditor = new ContentEditorRole();
        public static readonly Role Teacher = new TeacherRole();
        public static readonly Role Student = new StudentRole();
        public static readonly Role Guest = new GuestRole();
        public static readonly Role Viewer = new ViewerRole();
        public static readonly Role Moderator = new ModeratorRole();

        public string Description { get; private set; }

        public Role(int id, string name, string description) : base(id, name)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            if (description.Length > 100)
            {
                throw new ArgumentException("Description cannot exceed 100 characters.", nameof(description));
            }

            Description = description;
        }

        public virtual IEnumerable<Permission> GetDefaultPermissions()
        {
            yield break;
        }

        private class AdminRole : Role
        {
            public AdminRole() : base((int)RoleType.Admin, "Admin", "Full access to platform management, user administration, and analytics")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // User Management
                yield return Permission.UsersView;
                yield return Permission.UsersCreate;
                yield return Permission.UsersUpdate;
                yield return Permission.UsersDelete;

                // Course Management
                yield return Permission.CoursesView;
                yield return Permission.CoursesCreate;
                yield return Permission.CoursesUpdate;
                yield return Permission.CoursesDelete;
                yield return Permission.CoursesEnroll;
                yield return Permission.CoursesMaterial;
                yield return Permission.CoursesProgress;

                // Content Management
                yield return Permission.ContentView;
                yield return Permission.ContentCreate;
                yield return Permission.ContentUpdate;
                yield return Permission.ContentDelete;
                yield return Permission.ContentApprove;
                yield return Permission.ContentComment;

                // Assessment Management
                yield return Permission.AssessmentsView;
                yield return Permission.AssessmentsCreate;
                yield return Permission.AssessmentsUpdate;
                yield return Permission.AssessmentsDelete;
                yield return Permission.AssessmentsGrade;
                yield return Permission.AssessmentsSubmit;
                yield return Permission.AssessmentsFeedback;

                // Community Management
                yield return Permission.CommunityView;
                yield return Permission.CommunityModerate;
                yield return Permission.CommunityPost;
                yield return Permission.CommunityDelete;
                yield return Permission.CommunityFlag;

                // System Administration
                yield return Permission.SystemViewSettings;
                yield return Permission.SystemUpdateSettings;
                yield return Permission.SystemViewAuditLogs;
                yield return Permission.SystemManageRoles;
                yield return Permission.SystemManagePermissions;
                yield return Permission.SystemBackupRestore;
            }
        }

        private class ContentEditorRole : Role
        {
            public ContentEditorRole() : base((int)RoleType.ContentEditor, "Content Editor", "Creates and manages course content, including multimedia assets")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // Content Management
                yield return Permission.ContentView;
                yield return Permission.ContentCreate;
                yield return Permission.ContentUpdate;
                yield return Permission.ContentComment;

                // Course Management
                yield return Permission.CoursesView;
                yield return Permission.CoursesMaterial;

                // Community Management
                yield return Permission.CommunityView;
                yield return Permission.CommunityPost;
            }
        }

        private class ContentManagerRole : Role
        {
            public ContentManagerRole() : base((int)RoleType.ContentManager, "Content Manager", "Oversees all content creation, approval workflows, and quality control")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // Content Management
                yield return Permission.ContentView;
                yield return Permission.ContentCreate;
                yield return Permission.ContentUpdate;
                yield return Permission.ContentDelete;
                yield return Permission.ContentApprove;
                yield return Permission.ContentComment;

                // Course Management
                yield return Permission.CoursesView;
                yield return Permission.CoursesCreate;
                yield return Permission.CoursesUpdate;
                yield return Permission.CoursesMaterial;

                // Assessment Management
                yield return Permission.AssessmentsView;
                yield return Permission.AssessmentsCreate;
                yield return Permission.AssessmentsUpdate;

                // Community Management
                yield return Permission.CommunityView;
                yield return Permission.CommunityPost;
                yield return Permission.CommunityModerate;
            }
        }

        private class TeacherRole : Role
        {
            public TeacherRole() : base((int)RoleType.Teacher, "Teacher", "Creates and delivers courses, provides grading and feedback")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // Course Management
                yield return Permission.CoursesView;
                yield return Permission.CoursesCreate;
                yield return Permission.CoursesUpdate;
                yield return Permission.CoursesMaterial;
                yield return Permission.CoursesProgress;

                // Content Management
                yield return Permission.ContentView;
                yield return Permission.ContentCreate;
                yield return Permission.ContentUpdate;
                yield return Permission.ContentComment;

                // Assessment Management
                yield return Permission.AssessmentsView;
                yield return Permission.AssessmentsCreate;
                yield return Permission.AssessmentsUpdate;
                yield return Permission.AssessmentsGrade;
                yield return Permission.AssessmentsFeedback;

                // Community Management
                yield return Permission.CommunityView;
                yield return Permission.CommunityPost;
                yield return Permission.CommunityModerate;
            }
        }

        private class StudentRole : Role
        {
            public StudentRole() : base((int)RoleType.Student, "Student", "Accesses courses, submits assignments, participates in discussions")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // Course Management
                yield return Permission.CoursesView;
                yield return Permission.CoursesEnroll;
                yield return Permission.CoursesProgress;

                // Content Management
                yield return Permission.ContentView;
                yield return Permission.ContentComment;

                // Assessment Management
                yield return Permission.AssessmentsView;
                yield return Permission.AssessmentsSubmit;
                yield return Permission.AssessmentsFeedback;

                // Community Management
                yield return Permission.CommunityView;
                yield return Permission.CommunityPost;
                yield return Permission.CommunityFlag;
            }
        }

        private class GuestRole : Role
        {
            public GuestRole() : base((int)RoleType.Guest, "Guest", "Limited access to preview content and platform features")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // Limited viewing permissions only
                yield return Permission.CoursesView;
                yield return Permission.ContentView;
                yield return Permission.CommunityView;
            }
        }

        private class ViewerRole : Role
        {
            public ViewerRole() : base((int)RoleType.Viewer, "Viewer", "Read-only access to specific content (often used for auditing purposes)")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // View-only permissions across various categories
                yield return Permission.CoursesView;
                yield return Permission.ContentView;
                yield return Permission.AssessmentsView;
                yield return Permission.CommunityView;
                yield return Permission.CoursesProgress;
            }
        }

        private class ModeratorRole : Role
        {
            public ModeratorRole() : base((int)RoleType.Moderator, "Moderator", "Oversees community interactions, enforces guidelines, and manages user-generated content")
            {
            }

            public override IEnumerable<Permission> GetDefaultPermissions()
            {
                // Community Management
                yield return Permission.CommunityView;
                yield return Permission.CommunityModerate;
                yield return Permission.CommunityPost;
                yield return Permission.CommunityDelete;
                yield return Permission.CommunityFlag;

                // Content Management (limited)
                yield return Permission.ContentView;
                yield return Permission.ContentComment;

                // Course Management (limited)
                yield return Permission.CoursesView;
            }
        }
    }
}
