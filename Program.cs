using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace efcore_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello ASP.NET Core!");
            int entityBlogId;
            using (var db = new BloggingContext())
            {
                Blog entity = new Blog { Url = "http://blogs.msdn.com/adonet" };
                entity.Add(new Post { Title = "Nice", Content = "Bla bla..." });
                db.Blogs.Add(entity);
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
                entityBlogId = entity.BlogId;
            }


            Console.WriteLine("---- New db context and add one new post to the blog.");
            using (var db = new BloggingContext())
            {
                var blogWithPost = db.Blogs.Include(x => x.Posts)
                                     .First(x => x.BlogId == entityBlogId);
                blogWithPost.Add(new Post { Title = "Expected insert", Content = "I see insert statement..." });
                var impactCount = db.SaveChanges();
                Console.WriteLine("{0} records impacted in database", impactCount);
            }

            Console.ReadKey();
        }
    }


    public class BloggingContext : DbContext
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .UseSqlServer(@"Server=localhost;Database=efcore-test;user=sa;password=P@55w0rd;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                        .ToTable("Blogs", "Blogger")
                        .HasKey(x => x.BlogId);
                
            modelBuilder.Entity<Blog>()
                .Property(x => x.Url)
                .HasMaxLength(255)
                .IsRequired();
            modelBuilder.Entity<Blog>()
                        .HasMany(x => x.Posts);
                

            
            modelBuilder.Entity<Post>()
                .HasOne(x => x.Blog);
        }
    }


    public class Blog
    {
        public int BlogId { get;  set; }
        public string Url { get; set; }

        private ISet<Post> posts;
        public IEnumerable<Post> Posts => posts ?? (posts = new HashSet<Post>());

        public void Add(Post post)
        {
            if (posts == null)
                posts = new HashSet<Post>();
            posts.Add(post);
            post.Connect(this);
        }


        public void ClearPosts()
        {
            posts.Clear();
        }
    }


    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; private set; }
        public Blog Blog { get; private set; }

        public void Connect(Blog blog)
        {
            Blog = blog ?? throw new ArgumentNullException(nameof(blog));
            BlogId = Blog.BlogId;
        }
    }
}
