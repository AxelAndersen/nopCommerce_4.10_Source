namespace AO.SharedServices.Models
{ 
    public interface IResult
    {
        string ErrorMessage { get; set; }
        string InfoMessage { get; set; }
        bool Success { get; set; }
        int EntityCount { get; set; }
    }
}
