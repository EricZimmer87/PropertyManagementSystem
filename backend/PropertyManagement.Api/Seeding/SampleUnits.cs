using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Seeding
{
    public class SampleUnits
    {
        public static IEnumerable<Unit> Get() =>
            [
                new() { UnitNumber = "101", UnitType = "Double Queen" },
                new() { UnitNumber = "102", UnitType = "Double Queen" },
                new() { UnitNumber = "103", UnitType = "Double Queen" },
                new() { UnitNumber = "104", UnitType = "Double Queen" },
                new() { UnitNumber = "105", UnitType = "Double Queen" },
                new() { UnitNumber = "106", UnitType = "Double Queen" },
                new() { UnitNumber = "107", UnitType = "Double Queen" },
                new() { UnitNumber = "108", UnitType = "Double Queen" },
                new() { UnitNumber = "109", UnitType = "Double Queen" },
                new() { UnitNumber = "110", UnitType = "Handicap, Single Queen" },
                new() { UnitNumber = "111", UnitType = "Double Queen" },
                new() { UnitNumber = "112", UnitType = "Double Queen" },
                new() { UnitNumber = "113", UnitType = "Double Queen" },
                new() { UnitNumber = "114", UnitType = "Double Queen" },
                new() { UnitNumber = "115", UnitType = "King Suite" },
                new() { UnitNumber = "116", UnitType = "King Suite" },
                new() { UnitNumber = "C1", UnitType = "Camper" },
                new() { UnitNumber = "C2", UnitType = "Camper" },
                new() { UnitNumber = "C3", UnitType = "Camper" },
                new() { UnitNumber = "C4", UnitType = "Camper" },
                new() { UnitNumber = "C5", UnitType = "Camper" },
                new() { UnitNumber = "C6", UnitType = "Camper" },
                new() { UnitNumber = "C7", UnitType = "Camper" }
            ];
    }
}
