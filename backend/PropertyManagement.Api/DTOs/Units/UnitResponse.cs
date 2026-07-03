namespace PropertyManagement.Api.DTOs.Units
{
    public class UnitResponse
    {
        public long UnitId { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
