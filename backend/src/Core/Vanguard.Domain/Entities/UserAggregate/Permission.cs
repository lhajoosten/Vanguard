using Vanguard.Core.Base;
using Vanguard.Domain.Enums;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class Permission : Enumeration
    {
        #region User Management Permissions
        public static readonly Permission UsersView = new(1001, "Users.View", PermissionCategoryType.UserManagement);
        public static readonly Permission UsersCreate = new(1002, "Users.Create", PermissionCategoryType.UserManagement);
        public static readonly Permission UsersUpdate = new(1003, "Users.Update", PermissionCategoryType.UserManagement);
        public static readonly Permission UsersDelete = new(1004, "Users.Delete", PermissionCategoryType.UserManagement);
        #endregion

        #region Course Management Permissions
        public static readonly Permission CoursesView = new(2001, "Courses.View", PermissionCategoryType.CourseManagement);
        public static readonly Permission CoursesCreate = new(2002, "Courses.Create", PermissionCategoryType.CourseManagement);
        public static readonly Permission CoursesUpdate = new(2003, "Courses.Update", PermissionCategoryType.CourseManagement);
        public static readonly Permission CoursesDelete = new(2004, "Courses.Delete", PermissionCategoryType.CourseManagement);
        public static readonly Permission CoursesEnroll = new(2005, "Courses.Enroll", PermissionCategoryType.CourseManagement);
        public static readonly Permission CoursesMaterial = new(2006, "Courses.Material", PermissionCategoryType.CourseManagement);
        public static readonly Permission CoursesProgress = new(2007, "Courses.Progress", PermissionCategoryType.CourseManagement);
        #endregion

        #region Content Management Permissions
        public static readonly Permission ContentView = new(3001, "Content.View", PermissionCategoryType.ContentManagement);
        public static readonly Permission ContentCreate = new(3002, "Content.Create", PermissionCategoryType.ContentManagement);
        public static readonly Permission ContentUpdate = new(3003, "Content.Update", PermissionCategoryType.ContentManagement);
        public static readonly Permission ContentDelete = new(3004, "Content.Delete", PermissionCategoryType.ContentManagement);
        public static readonly Permission ContentApprove = new(3005, "Content.Approve", PermissionCategoryType.ContentManagement);
        public static readonly Permission ContentComment = new(3006, "Content.Comment", PermissionCategoryType.ContentManagement);
        #endregion

        #region Assessment Management Permissions
        public static readonly Permission AssessmentsView = new(4001, "Assessments.View", PermissionCategoryType.AssessmentManagement);
        public static readonly Permission AssessmentsCreate = new(4002, "Assessments.Create", PermissionCategoryType.AssessmentManagement);
        public static readonly Permission AssessmentsUpdate = new(4003, "Assessments.Update", PermissionCategoryType.AssessmentManagement);
        public static readonly Permission AssessmentsDelete = new(4004, "Assessments.Delete", PermissionCategoryType.AssessmentManagement);
        public static readonly Permission AssessmentsGrade = new(4005, "Assessments.Grade", PermissionCategoryType.AssessmentManagement);
        public static readonly Permission AssessmentsSubmit = new(4006, "Assessments.Submit", PermissionCategoryType.AssessmentManagement);
        public static readonly Permission AssessmentsFeedback = new(4007, "Assessments.Feedback", PermissionCategoryType.AssessmentManagement);
        #endregion

        #region Community Management Permissions
        public static readonly Permission CommunityView = new(5001, "Community.View", PermissionCategoryType.CommunityManagement);
        public static readonly Permission CommunityModerate = new(5002, "Community.Moderate", PermissionCategoryType.CommunityManagement);
        public static readonly Permission CommunityPost = new(5003, "Community.Post", PermissionCategoryType.CommunityManagement);
        public static readonly Permission CommunityDelete = new(5004, "Community.Delete", PermissionCategoryType.CommunityManagement);
        public static readonly Permission CommunityFlag = new(5005, "Community.Flag", PermissionCategoryType.CommunityManagement);
        #endregion

        #region System Administration Permissions
        public static readonly Permission SystemViewSettings = new(9001, "System.ViewSettings", PermissionCategoryType.SystemAdministration);
        public static readonly Permission SystemUpdateSettings = new(9002, "System.UpdateSettings", PermissionCategoryType.SystemAdministration);
        public static readonly Permission SystemViewAuditLogs = new(9003, "System.ViewAuditLogs", PermissionCategoryType.SystemAdministration);
        public static readonly Permission SystemManageRoles = new(9004, "System.ManageRoles", PermissionCategoryType.SystemAdministration);
        public static readonly Permission SystemManagePermissions = new(9005, "System.ManagePermissions", PermissionCategoryType.SystemAdministration);
        public static readonly Permission SystemBackupRestore = new(9006, "System.BackupRestore", PermissionCategoryType.SystemAdministration);
        #endregion

        public PermissionCategoryType Category { get; private set; }

        private Permission(int id, string name, PermissionCategoryType categoryType) : base(id, name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Category = categoryType;
        }

        /// <summary>
        /// Gets all permissions belonging to a specific category
        /// </summary>
        public static IEnumerable<Permission> GetByCategory(PermissionCategoryType category)
        {
            return GetAll<Permission>().Where(p => p.Category == category);
        }

        /// <summary>
        /// Groups all permissions by their category
        /// </summary>
        public static IEnumerable<IGrouping<PermissionCategoryType, Permission>> GetGroupedByCategory()
        {
            return GetAll<Permission>().GroupBy(p => p.Category);
        }

        /// <summary>
        /// Determines if this permission is related to a specific resource/entity
        /// </summary>
        public bool IsRelatedTo(string resourceName)
        {
            return Name.StartsWith($"{resourceName}.", StringComparison.OrdinalIgnoreCase);
        }
    }
}
