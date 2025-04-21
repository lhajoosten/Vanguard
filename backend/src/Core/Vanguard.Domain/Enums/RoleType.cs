namespace Vanguard.Domain.Enums
{
    public enum RoleType
    {
        // Administrative roles
        Admin = 1,
        ContentManager = 2,
        ContentEditor = 3,

        // Educational roles
        Teacher = 4,
        Student = 5,

        // Access roles
        Guest = 6,
        Viewer = 7,

        // Community roles
        Moderator = 8,
    }
}
