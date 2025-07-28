using DigitalDetox.Data;
using DigitalDetox.Models;
using DigitalDetox.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalDetox.Controllers;

public class SurveyController(ApplicationDbContext context) : Controller
{
    // GET: /Survey/Submit
    public async Task<IActionResult> Submit()
    {
        var questions = await context.Questions.ToListAsync();
        ViewBag.Questions = questions;
        return View();
    }

    // POST: /Survey/Submit
    [HttpPost]
    public async Task<IActionResult> Submit(Student student, Dictionary<int, bool> answers)
    {
        var weekStart = GetCurrentWeekStart();

        var existingStudent = await context.Students
            .FirstOrDefaultAsync(s => s.Class == student.Class && s.Roll == student.Roll);

        if (existingStudent == null)
        {
            existingStudent = student;
            context.Students.Add(existingStudent);
            await context.SaveChangesAsync();
        }

        var alreadySubmitted = await context.StudentResponses
            .AnyAsync(r => r.StudentId == existingStudent.StudentId && r.WeekStartDate == weekStart);

        if (alreadySubmitted)
        {
            TempData["Message"] = "You have already submitted for this week.";
            return RedirectToAction("Submit");
        }

        foreach (var entry in answers)
        {
            context.StudentResponses.Add(new StudentResponse
            {
                StudentId = existingStudent.StudentId,
                QuestionId = entry.Key,
                Answer = entry.Value,
                WeekStartDate = weekStart
            });
        }

        await context.SaveChangesAsync();
        TempData["Message"] = "Submission successful!";
        return RedirectToAction("Submit");
    }

    public async Task<IActionResult> Leaderboard(DateTime? fromDate, DateTime? toDate)
    {
        var responses = context.StudentResponses
            .Include(r => r.Student)
            .Where(r =>
                (!fromDate.HasValue || r.CreatedAt.Date >= fromDate.Value.Date) &&
                (!toDate.HasValue || r.CreatedAt.Date <= toDate.Value.Date)
            );

        var studentsInRange = await responses
            .Select(r => r.Student)
            .Distinct()
            .ToListAsync();

        var yesCounts = await responses
            .Where(r => r.Answer == true)
            .GroupBy(r => r.StudentId)
            .Select(g => new { StudentId = g.Key, YesCount = g.Count() })
            .ToListAsync();

        var leaderboard = studentsInRange
            .Select(student =>
            {
                var yesEntry = yesCounts.FirstOrDefault(y => y.StudentId == student.StudentId);
                return new LeaderboardEntryViewModel
                {
                    Student = student,
                    YesCount = yesEntry?.YesCount ?? 0
                };
            })
            .OrderByDescending(x => x.YesCount)
            .ToList();

        ViewBag.FromDate = fromDate?.ToString("dd-MMMM-yyyy");
        ViewBag.ToDate = toDate?.ToString("dd-MMMM-yyyy");

        return View(leaderboard);
    }





    public async Task<IActionResult> AllSubmissions(DateTime? fromDate, DateTime? toDate)
    {
        var query = context.StudentResponses
      .Include(r => r.Student)
      .Include(r => r.Question)
      .AsQueryable(); 

        if (fromDate.HasValue)
            query = query.Where(r => r.WeekStartDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(r => r.WeekStartDate <= toDate.Value.Date);

        var results = await query
            .OrderBy(r => r.Student.Class)
            .ThenBy(r => r.Student.Roll)
            .ThenBy(r => r.QuestionId)
            .ToListAsync();


        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

        return View(results);
    }

    private static DateTime GetCurrentWeekStart()
    {
        var today = DateTime.Today;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        return today.AddDays(-diff).Date;
    }
}
