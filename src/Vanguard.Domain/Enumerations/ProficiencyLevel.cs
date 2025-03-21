using Vanguard.Common.Base;

namespace Vanguard.Domain.Enumerations
{
    public class ProficiencyLevel : Enumeration
    {
        public readonly static ProficiencyLevel Beginner = new(1, "Beginner", "Basic understanding, needs guidance");
        public readonly static ProficiencyLevel Intermediate = new(2, "Intermediate", "Working knowledge, can perform independently");
        public readonly static ProficiencyLevel Advanced = new(3, "Advanced", "Deep understanding, can teach others");
        public readonly static ProficiencyLevel Expert = new(4, "Expert", "Mastery, can innovate and contribute to the field");

        public string Description { get; private set; }

        private ProficiencyLevel(int id, string name, string description) : base(id, name)
        {
            Description = description;
        }

        // Add additional methods specific to ProficiencyLevel
        public bool IsHigherThan(ProficiencyLevel other)
        {
            return Id > other.Id;
        }

        public bool IsEqualOrHigherThan(ProficiencyLevel other)
        {
            return Id >= other.Id;
        }

        public bool IsLowerThan(ProficiencyLevel other)
        {
            return Id < other.Id;
        }

        public bool IsEqualOrLowerThan(ProficiencyLevel other)
        {
            return Id <= other.Id;
        }
    }
}
