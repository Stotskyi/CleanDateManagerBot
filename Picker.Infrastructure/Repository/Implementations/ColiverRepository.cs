using System.Text;
using Microsoft.EntityFrameworkCore;
using Picker.Infrastructure.Data;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using SkiaSharp;

namespace Picker.Infrastructure.Repository.Implementations;

public class ColiverRepository(ApplicationContext context) : IColiverRepository
{
    public async Task<string> WriteColiverAsync(string date, string username)
    {
        if (!int.TryParse(date, out int day)) return "Якусь небилицю ти ввів, роззуй очі і введи як мама вчила";
        if (string.IsNullOrWhiteSpace(username)) return "Відкрий нік дурбецало";
        
        var now = DateTime.Now;
        var times = await context.CleaningTimes
            .Where(t => t.Date.Day == day
                        && t.Date.Month == now.Month
                        && t.Date.Year == now.Year)
            .Include(cleaningTime => cleaningTime.Colivers)
            .ToListAsync();

        if (!times.Any()) return "о боже ма читай дату з якого числа по яке можна драяти лох";
        
        foreach (var time in times)
        {
            if (time.Colivers != null)
            {
                int maxUsers = time.Date.DayOfWeek == DayOfWeek.Monday ? 3 : 1;
                if (time.Colivers.Count >= maxUsers)
                    return "зайнято шукай інший день битч";
            }
            else
            {
                time.Colivers = new List<Coliver>();
            }

            time.Colivers.Add(new Coliver() { Username = username });
        }

        context.CleaningTimes.Update(times.FirstOrDefault());
        var result =  await context.SaveChangesAsync();
        return "записан";
    }
    public async Task<string> GetCleanerToday(DateOnly date)
    {
        var cleaningTimes = await context.CleaningTimes
            .Where(ct => ct.Date == date)
            .Include(ct => ct.Colivers)
            .ToListAsync();

        if (!cleaningTimes.Any()) return "Яке курвисько не записалось ?!";

        var usernames = cleaningTimes
            .SelectMany(ct => ct.Colivers)
            .Select(c => c.Username)
            .Distinct();

        var username =  string.Join(Environment.NewLine, usernames);
        return $"Сьогодні драє кухню @{username}";
    }
    public async Task<(DateOnly startDate, DateOnly currentTime)> CreateCycle(byte count)
    {
        var currentTime = DateOnly.FromDateTime(DateTime.Now).AddDays(2);
        List<CleaningTime> cleaningTimesToAdd = new List<CleaningTime>();
       
        while (count > 0)
        {
            count = currentTime.DayOfWeek switch
            {
                DayOfWeek.Monday => count switch
                {
                    2 => (byte)(count - 2),
                    1 => (byte)(count - 1),
                    _ => (byte)(count - 3)
                },
                _ => (byte)(count - 1)
            };
            cleaningTimesToAdd.Add(new CleaningTime() { Date = currentTime });
            
            if(count is not 0) currentTime = currentTime.AddDays(1);
        }

        await context.CleaningTimes.AddRangeAsync(cleaningTimesToAdd);
        await context.SaveChangesAsync();
        
        return (DateOnly.FromDateTime(DateTime.Now), currentTime);
    }
    public async Task<string> GetRangeOfDate()
    {
        DateOnly minDate;
        DateOnly maxDate;
        try
        {
             minDate = await context.CleaningTimes.MinAsync(t => t.Date);
             maxDate = await context.CleaningTimes.MaxAsync(t => t.Date);
        }
        catch (Exception ex)
        {
            return "Питання до програміста, хай щось наклацає";
        }

        
        return $"драяти можна з {minDate} до {maxDate}";
    }

    public async Task<string> RemoveFromTable(string date, string? username)
    {
        if (!int.TryParse(date, out int day)) return "Якусь небилицю ти ввів, роззуй очі і введи як мама вчила";
        if (string.IsNullOrWhiteSpace(username)) return "Відкрий нік дурбецало";
        
        var now = DateTime.Now;
        var times = await context.CleaningTimes
            .Where(t => t.Date.Day == day
                        && t.Date.Month == now.Month
                        && t.Date.Year == now.Year)
            .Include(cleaningTime => cleaningTime.Colivers)
            .ToListAsync();

        if (!times.Any()) return "о боже ма втикай де ти записаний";
        
        foreach (var time in times)
        {
            var coliverToRemove = time.Colivers.FirstOrDefault(x => x.Username == username);
            if (coliverToRemove != null)
            {
                time.Colivers.Remove(coliverToRemove);
            }
        }
        
        var result =  await context.SaveChangesAsync();
        return "делітнув";
    }
    

    public async Task<byte[]> GetCleanersTable()
{
    var cleaningTimes = await context.CleaningTimes
        .Include(ct => ct.Colivers)
        .ToListAsync();

    if (!cleaningTimes.Any()) return null;

    var groupedCleaningTimes = cleaningTimes
        .GroupBy(ct => ct.Date)
        .Select(g => new CleaningGroup
        {
            Date = g.Key,
            Cleaners = g.Key.DayOfWeek == DayOfWeek.Monday
                ? $"cп. {g.Key:yyyy-MM-dd}"
                : $"{g.Key:yyyy-MM-dd}",
            Names = string.Join(", ", g.SelectMany(ct => ct.Colivers.Select(c => c.Username)).Distinct())
        })
        .ToList();

    return await GetPhoto(groupedCleaningTimes);

}

    private async Task<byte[]> GetPhoto(List<CleaningGroup> groupedCleaningTimes)
    {
        
        int rowHeight = 30;
        int columnWidth = 250;
        int padding = 10;
        int startX = padding;
        int startY = padding;

        int imageWidth = 800;
        int imageHeight = (groupedCleaningTimes.Count + 2) * rowHeight + 2 * padding;

        using (var bitmap = new SKBitmap(imageWidth, imageHeight))
        {
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);
            
                using (var paint = new SKPaint())
                {
                    paint.Color = SKColors.Black;
                    paint.TextAlign = SKTextAlign.Center;
                    paint.TextSize = 18.0f;
                    paint.Typeface = SKTypeface.FromFamilyName("Arial");
                
                    canvas.DrawText("Date", startX + columnWidth / 2, startY + rowHeight, paint);
                    canvas.DrawText("Cleaners", startX + columnWidth + columnWidth / 2, startY + rowHeight, paint);
                
                    canvas.DrawLine(startX, startY + rowHeight * 2, startX + imageWidth, startY + rowHeight * 2, paint);
                
                    int currentY = startY + rowHeight * 3;
                    foreach (var item in groupedCleaningTimes)
                    {
                        canvas.DrawText(item.Cleaners, startX + columnWidth / 2, currentY, paint);
                        canvas.DrawText(item.Names, startX + columnWidth + columnWidth / 2, currentY, paint);
                        currentY += rowHeight;
                    }
                }
            }

            // Convert bitmap to PNG format and return as byte array
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = new MemoryStream())
            {
                data.SaveTo(stream);
                return stream.ToArray();
            }
        }
    }
    
    public async Task<string> GetTextTable()
    {
        var cleaningTimes = await context.CleaningTimes
            .Include(ct => ct.Colivers)
            .ToListAsync();

        if (!cleaningTimes.Any()) return "Проєбались записатись";
        
        
        var tableBuilder = new StringBuilder();
        tableBuilder.AppendLine(" Date               |  Cleaners ");
        tableBuilder.AppendLine("-------------|----------------------");

        var groupedCleaningTimes = cleaningTimes
            .GroupBy(ct => ct.Date)
            .Select(g => new
            {
                Date = g.Key,
                Cleaners = string.Join(", ", g.SelectMany(ct => ct.Colivers.Select(c => c.Username)).Distinct())
            });

        foreach (var item in groupedCleaningTimes)
        {
            var dateStr = item.Date.ToString("yyyy-MM-dd");
            tableBuilder.AppendLine($"{dateStr} | {item.Cleaners}");
        }

        return tableBuilder.ToString();
    }
}



public class CleaningGroup
{
    public DateOnly Date { get; set; }
    public string Cleaners { get; set; }
    public string Names { get; set; }
}

