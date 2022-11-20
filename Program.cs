using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using var db = new AppDbContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

var c = new Course
{
	Title = "Test Course",
};
db.Add(c);
db.SaveChanges();

var v1 = new Video
{
	CourseId = c.Id,
	Title = "Test Video",
	Duration = TimeSpan.FromMinutes(32),
};
db.Add(v1);

var v2 = new Video
{
	CourseId = c.Id,
	Title = "Test Video",
	Duration = TimeSpan.FromMinutes(50),
};
db.Add(v2);

db.SaveChanges();

// ----

var course = db.Courses.Select(c => new
{
	// TotalDuration = TimeSpan.FromSeconds(c.Videos.Sum(v => v.Duration.TotalSeconds)),
	TotalDuration = EF.Functions.Sum(c.Videos.Select(v => v.Duration)),
}).FirstOrDefault();

Console.WriteLine($"Course total duration is: {course.TotalDuration}");

public class AppDbContext : DbContext
{
	public DbSet<Course> Courses { get; set; }

	public DbSet<Video> Videos { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder
			.UseNpgsql(@"Host=localhost;Username=postgres;Password=123456;Database=foobarbuzz")
			// .LogTo(Console.WriteLine, LogLevel.Information)
			.EnableSensitiveDataLogging();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	}
}

public class Course
{
	public int Id { get; set; }

	public string Title { get; set; }

	public ICollection<Video> Videos { get; set; }
}

public class Video
{
	public int Id { get; set; }

	public string Title { get; set; }

	public TimeSpan Duration { get; set; }

	public int CourseId { get; set; }

	public Course Course { get; set; }
}
