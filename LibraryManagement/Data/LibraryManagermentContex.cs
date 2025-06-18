using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class LibraryManagermentContext : DbContext
    {
        public LibraryManagermentContext(DbContextOptions<LibraryManagermentContext> options) : base(options) { }

        public DbSet<Reader> Readers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookReceipt> BookReceipts { get; set; }
        public DbSet<CategoryReport> CategoryReports { get; set; }
        public DbSet<CategoryReportDetail> CategoryReportDetails { get; set; }
        public DbSet<DetailBookReceipt> DetailBookReceipts { get; set; }
        public DbSet<LoanSlipBook> LoanSlipBooks { get; set; }
        public DbSet<OverdueReport> OverdueReports { get; set; }
        public DbSet<OverdueReportDetail> OverdueReportDetails { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<TypeBook> TypeBooks { get; set; }
        public DbSet<TypeReader> TypeReaders { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<HeaderBook> HeaderBooks { get; set; }
        public DbSet<TheBook> TheBooks { get; set; }
        public DbSet<BookWriting> BookWritings { get; set; }
        public DbSet<PenaltyTicket> PenaltyTickets { get; set; }
        public DbSet<FavoriteBook> FavoriteBooks { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Evaluate> Evaluates { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<FavoriteBook> LikedHeaderBooks { get; set; }
        public DbSet<InvalidateToken> InvalidatedTokens { get; set; }
        public DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table names
            modelBuilder.Entity<FavoriteBook>().ToTable("likedheaderbook");
            modelBuilder.Entity<Reader>().ToTable("reader");
            modelBuilder.Entity<TypeReader>().ToTable("typereader");
            modelBuilder.Entity<TypeBook>().ToTable("typebook");
            modelBuilder.Entity<Author>().ToTable("author");
            modelBuilder.Entity<HeaderBook>().ToTable("headerbook");
            modelBuilder.Entity<Book>().ToTable("book");
            modelBuilder.Entity<TheBook>().ToTable("thebook");
            modelBuilder.Entity<BookWriting>().ToTable("book_writing");
            modelBuilder.Entity<BookReceipt>().ToTable("bookreceipt");
            modelBuilder.Entity<DetailBookReceipt>().ToTable("detail_bookreceipt");
            modelBuilder.Entity<LoanSlipBook>().ToTable("loan_slipbook");
            modelBuilder.Entity<PenaltyTicket>().ToTable("penalty_ticket");
            modelBuilder.Entity<CategoryReport>().ToTable("category_report");
            modelBuilder.Entity<CategoryReportDetail>().ToTable("category_reportdetail");
            modelBuilder.Entity<OverdueReport>().ToTable("overdue_report");
            modelBuilder.Entity<OverdueReportDetail>().ToTable("overdue_reportdetail");
            modelBuilder.Entity<Image>().ToTable("image");
            modelBuilder.Entity<Evaluate>().ToTable("evaluate");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<Permission>().ToTable("permissions");
            modelBuilder.Entity<RolePermission>().ToTable("role_permission");
            modelBuilder.Entity<Parameter>().ToTable("parameters");
            modelBuilder.Entity<FavoriteBook>().ToTable("favoritebook");
            modelBuilder.Entity<InvalidateToken>().ToTable("invalidate_token");
            modelBuilder.Entity<Otp>().ToTable("otp");

            // Composite primary keys
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleName, rp.PermissionName });

            modelBuilder.Entity<CategoryReportDetail>()
                .HasKey(c => new { c.IdCategoryReport, c.IdTypeBook });

            modelBuilder.Entity<DetailBookReceipt>()
                .HasKey(d => new { d.IdBookReceipt, d.IdBook });

            modelBuilder.Entity<BookWriting>()
                .HasKey(cb => new { cb.IdHeaderBook, cb.IdAuthor });

            modelBuilder.Entity<OverdueReportDetail>()
                .HasKey(o => new { o.IdOverdueReport, o.IdTheBook });
        }
    }
}
