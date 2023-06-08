using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TestPlatform.Database;
using TestPlatform.Migrations;
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

    #region Get Test Logic

    public async Task<IActionResult> GetTest(int id)
    { 
        var test = await _db.Tests.FindAsync(id);
        test ??= await _db.Tests.FirstOrDefaultAsync();

        return View(test);
    }

    public async Task<IActionResult> StartTest(int id, string personalNumber)
    {
        bool isValid = Regex.IsMatch(personalNumber, @"^\d{11}$");
        var test = await _db.Tests.FindAsync(id);
        if (!isValid)
        {
            test.Error = "არასწორი ფორმატი პირადი ნომრის!";

            return View("GetTest", test);
        }
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

    public static Dictionary<int, int> ConvertStringToDictionary(string input)
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

    #endregion

    #region Create Test

    public IActionResult List()
    {
        var test = _db.Tests;
        return View(test);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Test test)
    {
        _db.Add(test);
        _db.SaveChanges();

        return RedirectToAction("Edit", new { id = test.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var test = await _db.Tests.FindAsync(id);

        return View(test);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Test test)
    {
        var tempTest = await _db.Tests.FindAsync(test.Id);
        tempTest.Title = test.Title;
        _db.SaveChanges();

        return RedirectToAction("Edit", new { id = test.Id });
    }

    [HttpGet]
    public IActionResult AddQuetion(int id)
    {
        return View(id);
    }

    [HttpPost]
    public IActionResult AddQuetion(string question, string answers, int correctAnswer, int testId)
    {
        var answersList = answers.Split(',').ToList() ?? new List<string>();
        var quetion = new Quetion
        {
            Text = question,
            Answers = answersList.Select(o => new Answer
            {
                Text = o
            }).ToList()
        };
        quetion.Answers[correctAnswer - 1].IsCorrect = true;
        var test = _db.Tests.Find(testId);
        test.Quetions.Add(quetion);
        _db.SaveChanges();

        return RedirectToAction("EditQuetion", new { id = quetion.Id });
    }

    [HttpGet]
    public IActionResult EditQuetion(int id)
    {
        var quetion = _db.Quetions.Find(id);

        return View(quetion);
    }

    [HttpPost]
    public IActionResult EditQuetion(string question, string answers, int correctAnswer, int id)
    {
        var answersList = answers.Split(',').ToList() ?? new List<string>();

        var quetion = _db.Quetions.Find(id);
        quetion.Text = question;
        quetion.Answers.Clear();
        quetion.Answers = answersList.Take(4).Select(o => new Answer
        {
            Text = o
        }).ToList();
        quetion.Answers[correctAnswer - 1].IsCorrect = true;
        _db.SaveChanges();

        return View(quetion);
    }

    #endregion
}