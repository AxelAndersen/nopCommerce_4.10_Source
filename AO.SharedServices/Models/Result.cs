namespace AO.SharedServices.Models
{
    public class Result : IResult
    {
        public string ErrorMessage { get; set; }
        public string InfoMessage { get; set; }
        public bool Success { get; set; }
        public int EntityCount { get; set; }
    }
}
