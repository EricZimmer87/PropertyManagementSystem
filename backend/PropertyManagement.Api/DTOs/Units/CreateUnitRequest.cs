using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.DTOs.Units
{
    public class CreateUnitRequest
    {
        [Required]
        public required string UnitNumber { get; init; }

        [Required]
        public required string UnitType { get; init; }

        public string Notes { get; init; } = string.Empty;
    }
}
