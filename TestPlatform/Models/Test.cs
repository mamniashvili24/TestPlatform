﻿namespace TestPlatform.Models;

public class Test
{
    public int Id { get; set; }

    public string Title { get; set; }

    public virtual List<Quetion> Quetions { get; set; }
}