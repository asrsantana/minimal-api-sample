using Microsoft.EntityFrameworkCore;

namespace MinimalWebApi.Models
{
    public class TodoDb: DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
