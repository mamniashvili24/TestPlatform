using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlatform.Models;

public class Quetion
{
    public int Id { get; set; }

    public string Text { get; set; }

    public virtual List<Answer> Answers { get; set; }
}