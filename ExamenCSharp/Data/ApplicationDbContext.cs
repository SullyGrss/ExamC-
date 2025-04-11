using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExamenCSharp.Models;
public class ApplicationDbContext : IdentityDbContext<User> {
     public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

     public DbSet<Intervention> Interventions { get; set;}
     public DbSet<Service> Services{ get; set;}

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Intervention>()
            .HasOne(i => i.Client)
            .WithMany(u => u.Interventions) 
            .HasForeignKey(i => i.ClientId) 
            .OnDelete(DeleteBehavior.Restrict); 
    }
}