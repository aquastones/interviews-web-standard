using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ProgramDbContext : DbContext
    {
        public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options) { }

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Models.Tag> Tags { get; set; }
        public DbSet<Models.TaskTag> TaskTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------------------------------------
            // Configure the many-to-many relationship
            // between Task and Tag using the join entity TaskTag
            // --------------------------------------------------

            // Set the composite primary key for TaskTag (combination of TaskId and TagId)
            // so that the same task-tag pair cant be added twice
            modelBuilder.Entity<TaskTag>()
                .HasKey(tt => new { tt.TaskId, tt.TagId });

            // Define the relationship: One TaskTag links to one Task
            // and one Task has many TaskTags (one-to-many)
            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.Task)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskId);

            // Define the relationship: One TaskTag links to one Tag
            // and one Tag has many TaskTags (one-to-many)
            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId);
        }
    }
}