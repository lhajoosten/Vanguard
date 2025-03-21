using Vanguard.Common.Base;

namespace Vanguard.Domain.Enumerations
{
    public class DifficultyLevel : Enumeration
    {
        public readonly static DifficultyLevel Easy = new(1, "Easy", "Basic concepts suitable for beginners");
        public readonly static DifficultyLevel Medium = new(2, "Medium", "Intermediate concepts requiring some background knowledge");
        public readonly static DifficultyLevel Hard = new(3, "Hard", "Advanced concepts requiring strong foundational knowledge");
        public readonly static DifficultyLevel Expert = new(4, "Expert", "Complex concepts requiring specialized knowledge and experience");

        public string Description { get; private set; }
        public int PointMultiplier { get; private set; }

        private DifficultyLevel(int id, string name, string description) : base(id, name)
        {
            Description = description;
            PointMultiplier = id; // Simple multiplier based on difficulty level
        }

        // Methods to compare difficulty levels
        public bool IsHarderThan(DifficultyLevel other)
        {
            return this.Id > other.Id;
        }

        public bool IsEasierThan(DifficultyLevel other)
        {
            return this.Id < other.Id;
        }

        // Method to calculate points based on difficulty
        public int CalculatePoints(int basePoints)
        {
            return basePoints * PointMultiplier;
        }
    }
}
