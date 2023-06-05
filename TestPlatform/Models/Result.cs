namespace TestPlatform.Models;

public class Result
{
    public int Id { get; set; }
    public int? CorrectAnswers { get; set; }
    public string StudetnIdNumber { get; set; }
    public virtual Test Test { get; set; }
}