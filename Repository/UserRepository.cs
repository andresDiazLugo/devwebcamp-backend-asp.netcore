using System.ComponentModel.DataAnnotations;
using dev_backend_net.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;


namespace dev_backend_net.Repository
{
    public class UserRepository : DbContext
    {
        public UserRepository(DbContextOptions<UserRepository> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasIndex(u => u.email)
            .IsUnique();
        }

        public async Task registerUser(User user)
        {
            try
            {
                var newUser = new User()
                {
                    name = user.name,
                    email = user.email,
                    phone = user.phone,
                    password = user.password
                };

                await users.AddAsync(newUser);
                await this.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                HandleRegisterUserException(ex);
                throw;
            }
        }

        private void HandleRegisterUserException(Exception ex)
        {
            if (ex.InnerException is MySqlException mySqlException && mySqlException.Number == 1062)
            {
                throw new ApplicationException("Este email ya existe, intente iniciar sesión.");
            }
            // Puedes agregar más lógica de manejo de errores aquí si es necesario
            throw new ApplicationException("Ocurrió un error durante el registro del usuario.");
        }
        public async Task<User> getUserByEmailAndPassword(string email, string password)
        {
            Console.WriteLine(email);
            try
            {
                return await users.FirstOrDefaultAsync(u => u.email == email && u.password == password);
            }
            catch (Exception ex)
            {
                // Manejar excepciones si es necesario
                throw new ApplicationException("Ocurrió un error al intentar obtener el usuario por email.", ex);
            }


        }
    }

}