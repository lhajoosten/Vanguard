using Vanguard.Common.Base;

namespace Vanguard.Domain.Enumerations
{
    public class LessonType : Enumeration
    {
        public readonly static LessonType Video = new(1, "Video", "video", "Video lesson with playback controls", "video");
        public readonly static LessonType Text = new(2, "Text", "document", "Text-based lesson with reading material", "article");
        public readonly static LessonType Quiz = new(3, "Quiz", "question-circle", "Interactive quiz to test knowledge", "quiz");
        public readonly static LessonType Assignment = new(4, "Assignment", "tasks", "Practical assignment requiring submission", "assignment");
        public readonly static LessonType Discussion = new(5, "Discussion", "comments", "Group discussion on a topic", "discussion");
        public readonly static LessonType Presentation = new(6, "Presentation", "presentation", "Slide-based presentation", "presentation");
        public readonly static LessonType LiveSession = new(7, "Live Session", "video-camera", "Real-time interactive session", "live-session");
        public readonly static LessonType Simulation = new(8, "Simulation", "vr-cardboard", "Interactive simulation or scenario", "simulation");

        public string IconName { get; private set; }
        public string Description { get; private set; }
        public string UrlSlug { get; private set; }

        private LessonType(int id, string name, string iconName, string description, string urlSlug) : base(id, name)
        {
            IconName = iconName;
            Description = description;
            UrlSlug = urlSlug;
        }

        // Helper methods
        public bool RequiresSubmission()
        {
            return this == Assignment;
        }

        public bool IsInteractive()
        {
            return this == Quiz || this == Assignment || this == Discussion ||
                   this == LiveSession || this == Simulation;
        }

        public bool IsMedia()
        {
            return this == Video || this == Presentation;
        }

        public bool SupportsAttachments()
        {
            return this == Assignment || this == Text || this == Discussion;
        }

        public string GetIconClass()
        {
            return $"fa fa-{IconName}";
        }

        public TimeSpan GetEstimatedDuration(int contentLength)
        {
            // Simple estimation logic based on content length and type
            return this switch
            {
                var t when t == Video => TimeSpan.FromMinutes(contentLength), // Assuming contentLength is in minutes for videos
                var t when t == Text => TimeSpan.FromMinutes(contentLength / 200), // Assuming contentLength is in words, average reading speed
                var t when t == Quiz => TimeSpan.FromMinutes(contentLength * 2), // Assuming contentLength is number of questions
                var t when t == Assignment => TimeSpan.FromHours(contentLength), // Assuming contentLength is estimated hours
                var t when t == Discussion => TimeSpan.FromMinutes(30), // Fixed time for discussions
                var t when t == Presentation => TimeSpan.FromMinutes(contentLength * 2), // Assuming contentLength is number of slides
                var t when t == LiveSession => TimeSpan.FromMinutes(contentLength), // Assuming contentLength is in minutes
                var t when t == Simulation => TimeSpan.FromMinutes(contentLength), // Assuming contentLength is in minutes
                _ => TimeSpan.FromMinutes(30) // Default fallback
            };
        }
    }
}
