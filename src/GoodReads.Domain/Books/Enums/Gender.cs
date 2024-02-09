using Ardalis.SmartEnum;

namespace GoodReads.Domain.Books.Enums
{
    public sealed class Gender : SmartEnum<Gender>
    {
        public static readonly Gender Novel = new(nameof(Novel), 0);
        public static readonly Gender Romance = new(nameof(Romance), 1);
        public static readonly Gender Mistery = new(nameof(Mistery), 2);
        public static readonly Gender Fiction = new(nameof(Fiction), 3);
        public static readonly Gender Biography = new(nameof(Biography), 4);
        public static readonly Gender SelfHelp = new(nameof(SelfHelp), 5);

        public Gender(string name, int value) : base(name, value)
        {
            //
        }
    }
}