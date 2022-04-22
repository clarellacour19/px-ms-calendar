// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContext.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   DataContext class definition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace PG.ABBs.Calendar.Organizer.Data.Context
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using PG.ABBs.Calendar.Organizer.Data.Models;

	public class DataContext : DbContext
	{
		#region Constructors and Destructors

		public DataContext(DbContextOptions options)
			: base(options)
		{
		}

		#endregion

		#region DbSets

		public DbSet<Events> Events { get; set; }
		public DbSet<Calendar> Calendars { get; set; }
		public DbSet<UserCalendar> UserCalendar { get; set; }
		public DbSet<GetCountInTable> GetCountInTable { get; set; }

		#endregion

		#region Model Configuration

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.UseLazyLoadingProxies();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<HreflangTagBinding>(this.ConfigureHreflangTagBinding);
			modelBuilder.Entity<Events>(this.ConfigureEvents);
			modelBuilder.Entity<Calendar>(this.ConfigureCalendar);
			modelBuilder.Entity<UserCalendar>(this.ConfigureUserCalendar);
			modelBuilder.Entity<GetCountInTable>(this.ConfigureGetCountInTable);
		}

		//private void ConfigureHreflangTagBinding(EntityTypeBuilder<HreflangTagBinding> builder)
		//{
		//    builder.ToTable("HreflangTagBinding");
		//}

		private void ConfigureEvents(EntityTypeBuilder<Events> builder)
		{
			builder.ToTable("Events");
		}

		private void ConfigureCalendar(EntityTypeBuilder<Calendar> builder)
		{
			builder.ToTable("Calendar");
		}

		private void ConfigureUserCalendar(EntityTypeBuilder<UserCalendar> builder)
		{
			builder.ToTable("UserCalendar");
		}

		private void ConfigureGetCountInTable(EntityTypeBuilder<GetCountInTable> builder)
		{
			builder.ToTable("Results");
		}

		#endregion
	}
}