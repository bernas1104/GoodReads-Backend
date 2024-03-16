using Ardalis.SmartEnum;

namespace GoodReads.Domain.UserAggregate.Enums
{
    public class ModuleUnexpectedError : SmartEnum<ModuleUnexpectedError>
    {
        public static readonly ModuleUnexpectedError User = new(nameof(User), 0);
        public static readonly ModuleUnexpectedError Book = new(nameof(Book), 1);

        public ModuleUnexpectedError(string name, int value) : base(name, value)
        {
        }
    }
}