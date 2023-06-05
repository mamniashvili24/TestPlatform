using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPlatform.Database;
using TestPlatform.Models;
using TestPlatform.Models.ViewModel;

namespace TestPlatform.Controllers;

public class TestController : Controller
{
    private readonly TestPlatformContext _db;

    public TestController(TestPlatformContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> GetTest(int id)
    { 
        var test = await _db.Tests.FindAsync(id);
        return View(test);
    }

    public async Task<IActionResult> StartTest(int id, string personalNumber)
    {
        var test = await _db.Tests.FindAsync(id);
        var result = new Result
        {
            Test = test,
            StudetnIdNumber = personalNumber
        };
        _db.Add(result);
        _db.SaveChanges();

        return View(new StartTestModel
        {
            Test = test,
            PersonalNumber = personalNumber
        });
    }

    public async Task<IActionResult> CountResult(string answersString, int id, string personalNumber)
    {
        var answers = ConvertStringToDictionary(answersString);

        var test = await _db.Tests.FindAsync(id);
        var result = await _db.Results.FirstOrDefaultAsync(o => o.StudetnIdNumber == personalNumber && o.Test.Id == test.Id);

        if (test == null || result == null)
        {
            throw new Exception("დაფიქსირდა ტექნიკური შეცდომა!");
        }

        int? correctAnswers = 0;
        foreach (var answer in answers)
        {
            var quetion = _db.Quetions.FirstOrDefault(o => o.Id == answer.Value);
            if (quetion != null && quetion.Answers.Any(o => o.Id == answer.Key && o.IsCorrect))
            {
                correctAnswers++;
            }
        }

        result.CorrectAnswers = correctAnswers;
        _db.SaveChanges();



        return View(correctAnswers);
    }

    static Dictionary<int, int> ConvertStringToDictionary(string input)
    {
        var dictionary = new Dictionary<int, int>();
        string[] keyValuePairs = input.Split(',');

        foreach (var pair in keyValuePairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2)
            {
                int.TryParse(parts[0].Trim(), out int key);
                int.TryParse(parts[1].Trim(), out int value);
                dictionary[key] = value;
            }
        }

        return dictionary;
    }
}