using Vanguard.Domain.Base;

namespace Vanguard.Domain.Enumerations
{
    public class EnrollmentStatus : Enumeration
    {
        public readonly static EnrollmentStatus Active = new(1, "Active", "Currently taking the course", "green");
        public readonly static EnrollmentStatus Completed = new(2, "Completed", "Successfully finished the course", "blue");
        public readonly static EnrollmentStatus Dropped = new(3, "Dropped", "Withdrawn from the course", "red");
        public readonly static EnrollmentStatus OnHold = new(4, "On Hold", "Temporarily paused", "yellow");
        public readonly static EnrollmentStatus Expired = new(5, "Expired", "Access period has ended", "gray");
        public readonly static EnrollmentStatus Pending = new(6, "Pending", "Enrollment awaiting approval", "orange");

        public string Description { get; private set; }
        public string ColorCode { get; private set; }

        private EnrollmentStatus(int id, string name, string description, string colorCode) : base(id, name)
        {
            Description = description;
            ColorCode = colorCode;
        }

        // Helper methods
        public bool IsActive()
        {
            return this == Active;
        }

        public bool IsCompleted()
        {
            return this == Completed;
        }

        public bool IsTerminated()
        {
            return this == Dropped || this == Expired;
        }

        public bool IsPaused()
        {
            return this == OnHold;
        }

        public bool CanResume()
        {
            return this == OnHold || this == Pending;
        }

        public bool CanComplete()
        {
            return this == Active;
        }

        public bool CanModify()
        {
            return this == Active || this == OnHold || this == Pending;
        }

        public string GetStatusClass()
        {
            return $"status-{Name.ToLower().Replace(" ", "-")}";
        }

        public string GetStatusBadge()
        {
            return $"<span class=\"badge bg-{GetBootstrapColor()}\"><i class=\"fa fa-{GetStatusIcon()}\"></i> {Name}</span>";
        }

        private string GetBootstrapColor()
        {
            return this switch
            {
                var s when s == Active => "success",
                var s when s == Completed => "primary",
                var s when s == Dropped => "danger",
                var s when s == OnHold => "warning",
                var s when s == Expired => "secondary",
                var s when s == Pending => "info",
                _ => "light"
            };
        }

        private string GetStatusIcon()
        {
            return this switch
            {
                var s when s == Active => "play-circle",
                var s when s == Completed => "check-circle",
                var s when s == Dropped => "times-circle",
                var s when s == OnHold => "pause-circle",
                var s when s == Expired => "calendar-times",
                var s when s == Pending => "clock",
                _ => "circle"
            };
        }
    }
}
