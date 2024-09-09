using Microsoft.EntityFrameworkCore;
using Picker.Domain.Entities.CleaningTime;
using Picker.Domain.Entities.Users;
using Picker.Persistence.Data;
using SkiaSharp;

namespace Picker.Persistence.Repositories;

public class ColiverRepository(ApplicationContext context) : IColiverRepository
{
    public async Task<string> WriteColiverAsync(string date, string username,string firsname,string lastname)
    {
        if (!int.TryParse(date, out int day)) return "Якусь небилицю ти ввів, роззуй очі і введи як мама вчила";

        var cycle = await context.CleaningTimes
            .MaxAsync(t => t.Cycle);

        var now = DateTime.Now;
        var times = await context.CleaningTimes
            .Where(t => t.Cycle == cycle && t.Date.Day == day)
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

            time.Colivers.Add(new Coliver() { Username = username,FirstName = firsname,LastName = lastname });
        }

        context.CleaningTimes.Update(times.FirstOrDefault());

        await context.SaveChangesAsync();

        return "записан";
    }

    public async Task<string> GetCleanerToday(DateOnly date)
    {
        var cleaningTimes = await context.CleaningTimes
            .Where(ct => ct.Date == date)
            .Include(ct => ct.Colivers)
            .ToListAsync();

        if (!cleaningTimes.Any()) return "Яке курвисько не записалось ?!";

        var coliverDetails = cleaningTimes
            .SelectMany(ct => ct.Colivers)
            .Select(c => new
            {
                c.Username
            })
            .Distinct();

        var formattedNames = coliverDetails
            .Select(c => $"{c.Username}");

        var message = $"Сьогодні драє кухню {string.Join(", ", formattedNames)}";

        return message;
    }

    public async Task<(DateOnly startDate, DateOnly currentTime)> CreateCycle(byte count)
    {
        var currentTime = DateOnly.FromDateTime(DateTime.Now);
        var maxValue = await context.CleaningTimes.MaxAsync(t => t.Cycle) + 1;
        
        List<CleaningTime> cleaningTimesToAdd = new List<CleaningTime>();

        while (count > 0)
        {
            count = currentTime.DayOfWeek switch
            {
                DayOfWeek.Monday => count switch
                {
                    1 => (byte)(count - 1),
                    _ => (byte)(count - 2)
                },
                _ => (byte)(count - 1)
            };
            cleaningTimesToAdd.Add(new CleaningTime() { Date = currentTime, Cycle = maxValue });

            if (count is not 0) currentTime = currentTime.AddDays(1);
        }

        await context.CleaningTimes.AddRangeAsync(cleaningTimesToAdd);
        await context.CleaningTimes.Where(c => c.Cycle == maxValue - 1).ExecuteDeleteAsync();
        await context.SaveChangesAsync();

        return (DateOnly.FromDateTime(DateTime.Now), currentTime);
    }

  

    public async Task<string> RemoveFromTable(string date, string? username)
    {
        if (!int.TryParse(date, out int day))
        {
            return "Якусь небилицю ти ввів, роззуй очі і введи як мама вчила";
        }
      

        var now = DateTime.Now;
        var times = await context.CleaningTimes
            .Where(t => t.Date.Day == day
                        && t.Date.Month == now.Month
                        && t.Date.Year == now.Year)
            .Include(cleaningTime => cleaningTime.Colivers)
            .ToListAsync();

        if (!times.Any())
        {
            return "о боже ма втикай де ти записаний";
        }
        

        foreach (var time in times)
        {
            var coliverToRemove = time.Colivers.FirstOrDefault(x => x.Username == username);
            
            if (coliverToRemove != null)
            {
                time.Colivers.Remove(coliverToRemove);
                context.Colivers.Remove(coliverToRemove);
            }
        }
   
        await context.SaveChangesAsync();
        return "делітнув";
    }


    public async Task<byte[]> GetCleanersTable()
    {
        var cleaningTimes = await context.CleaningTimes
            .Include(ct => ct.Colivers)
            .Where(c => c.Cycle == context.CleaningTimes.Max(ct => ct.Cycle))
            .ToListAsync();

        if (!cleaningTimes.Any())
        {
            return null;
        }

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

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}


