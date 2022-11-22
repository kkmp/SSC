using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSC.Data;
using SSC.Data.UnitOfWork;
using SSC.DTO.User;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class BasicTests
     : IClassFixture<BaseApplicationFactory>
    {
        private readonly BaseApplicationFactory _factory;

        public BasicTests(BaseApplicationFactory factory)
        {
            _factory = factory;
            CreateDB();
        }

        private void CreateDB()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<DataContext>();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
        }

        [Theory]
        [InlineData("test@zzzdfg.zz", "tester", "547854125", "Administrator", "testowy")]
        [InlineData("test2@zzzdfg.zz", "tester2", "547854122", "Laborant", "testowy2")]
        [InlineData("test3@zzzdfg.zz", "tester3", "547854123", "Lekarz", "testowy3")]
        public async Task AddUserTestCorrect(string email, string name, string phoneNumber, string roleName, string surname)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserCreateDTO dto = new UserCreateDTO
                {
                    Email = email,
                    Name = name,
                    PhoneNumber = phoneNumber,
                    RoleName = roleName,
                    Surname = surname
                };
                var result = await unityOfWork.UserRepository.AddUser(dto);
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(dto.Email, result.Data.Email);
                Assert.Equal(dto.Name, result.Data.Name);
                Assert.Equal(dto.Surname, result.Data.Surname);
                Assert.Equal(dto.PhoneNumber, result.Data.PhoneNumber);
            }
        }

        [Fact]
        public async Task AddUserWithTheSameEmail()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserCreateDTO dto = new UserCreateDTO
                {
                    Email = "test@zzzdfg.zz",
                    Name = "tester",
                    PhoneNumber = "547854125",
                    RoleName = Roles.Admin,
                    Surname = "testowy"
                };

                await unityOfWork.UserRepository.AddUser(dto);
                var result = await unityOfWork.UserRepository.AddUser(dto);
                Assert.False(result.Success);
                Assert.Null(result.Data);
            }
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ChangeActivityTestCorrect(bool activation)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserCreateDTO dto = new UserCreateDTO
                {
                    Email = "test@zzzdfg.zz",
                    Name = "Jan",
                    PhoneNumber = "123456789",
                    RoleName = "Laborant",
                    Surname = "Kowalski"
                };
                var user = await unityOfWork.UserRepository.AddUser(dto);
                var result = await unityOfWork.UserRepository.ChangeActivity(user.Data.Id, new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"), activation);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(activation, result.Data.IsActive);
            }
        }

        [Theory]
        [InlineData("3e7e4cc3-d3e1-0000-9647-42422b4c429a")]
        [InlineData("6fc75b09-f3a5-0000-894a-318e02e1e2e1")]
        [InlineData("9c489ff7-2100-0000-bbf6-e2737ce6ee83")]
        public async Task ChangeActivityUserDoesNotExist(string id)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.ChangeActivity(new Guid("3e7e4cc3-d3e1-0000-9647-42422b4c429a"), new Guid(id), true);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Użytkownik nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("3e7e4cc3-d3e1-4917-9647-42422b4c429a")]
        [InlineData("6fc75b09-f3a5-41eb-894a-318e02e1e2e1")]
        [InlineData("9c489ff7-2100-443c-bbf6-e2737ce6ee83")]
        public async Task ChangeActivitySameUser(string id)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.ChangeActivity(new Guid(id), new Guid(id), true);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Nie można dezaktywować własnego konta", result.Message);
            }
        }

        [Theory]
        [InlineData("3e7e4cc3-d3e1-4917-9647-42422b4c429a")]
        [InlineData("6fc75b09-f3a5-41eb-894a-318e02e1e2e1")]
        [InlineData("9c489ff7-2100-443c-bbf6-e2737ce6ee83")]
        public async Task UserDetailsTestCorrect(string id)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.UserDetails(new Guid(id));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(new Guid(id), result.Data.Id);
                Assert.Equal("Powodzenie", result.Message);
            }
        }

        [Theory]
        [InlineData("3e7e4cc3-d3e1-0000-9647-42422b4c429a")]
        [InlineData("6fc75b09-f3a5-0000-894a-318e02e1e2e1")]
        [InlineData("9c489ff7-2100-0000-bbf6-e2737ce6ee83")]
        public async Task UserDetailsUserDoesNotExist(string id)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.UserDetails(new Guid(id));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Użytkownik nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("test@zzzdfg.zz", "tester", "547854125", "Administrator", "testowy", false)]
        [InlineData("test2@zzzdfg.zz", "tester2", "547854122", "Laborant", "testowy2", false)]
        [InlineData("test3@zzzdfg.zz", "tester3", "547854123", "Lekarz", "testowy3", true)]
        public async Task EditUserTestCorrect(string email, string name, string phoneNumber, string roleName, string surname, bool isActive)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserCreateDTO dto = new UserCreateDTO
                {
                    Email = "test@zzzdfg.zz",
                    Name = "Jan",
                    PhoneNumber = "123456789",
                    RoleName = "Laborant",
                    Surname = "Kowalski"
                };

                var user = await unityOfWork.UserRepository.AddUser(dto);

                UserUpdateDTO updateDto = new UserUpdateDTO
                {
                    Id = user.Data.Id,
                    Name = name,
                    Surname = surname,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    IsActive = isActive,
                    RoleName = roleName
                };

                var result = await unityOfWork.UserRepository.EditUser(updateDto, new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Użytkownik został zedytowany", result.Message);
                Assert.Equal(updateDto.Email, result.Data.Email);
                Assert.Equal(updateDto.Name, result.Data.Name);
                Assert.Equal(updateDto.Surname, result.Data.Surname);
                Assert.Equal(updateDto.PhoneNumber, result.Data.PhoneNumber);
                Assert.Equal(updateDto.IsActive, result.Data.IsActive);
                Assert.Equal(updateDto.RoleName, result.Data.Role.Name);
            }
        }

        [Theory]
        [InlineData("3e7e4cc3-d3e1-0000-9647-42422b4c429a")]
        [InlineData("6fc75b09-f3a5-0000-894a-318e02e1e2e1")]
        [InlineData("9c489ff7-2100-0000-bbf6-e2737ce6ee83")]
        public async Task EditUserUserDoesNotExist(string id)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserUpdateDTO updateDto = new UserUpdateDTO
                {
                    Id = new Guid(id),
                    Email = "test@zzzdfg.zz",
                    Name = "Jan",
                    PhoneNumber = "123456789",
                    RoleName = "Laborant",
                    Surname = "Kowalski",
                    IsActive = true
                };

                var result = await unityOfWork.UserRepository.EditUser(updateDto, new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Użytkownik nie istnieje", result.Message);
            }
        }

        [Fact]
        public async Task EditUserWithExistingEmail()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserCreateDTO dto = new UserCreateDTO
                {
                    Email = "test@zzzdfg.zz",
                    Name = "Jan",
                    PhoneNumber = "123456789",
                    RoleName = "Laborant",
                    Surname = "Kowalski"
                };

                var user = await unityOfWork.UserRepository.AddUser(dto);

                UserUpdateDTO updateDto = new UserUpdateDTO
                {
                    Id = user.Data.Id,
                    Email = "j.kowalski@gmail.com",
                    Name = "Jan",
                    PhoneNumber = "123456789",
                    RoleName = "Laborant",
                    Surname = "Kowalski",
                    IsActive = false
                };

                var result = await unityOfWork.UserRepository.EditUser(updateDto, new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Adres email został już wykorzystany", result.Message);
            }
        }

        [Theory]
        [InlineData("j.kowalski@gmail.com", "password123")]
        [InlineData("s.kowalczyk@gmail.com", "password123")]
        [InlineData("a.nowak@gmail.com", "password123")]
        public async Task AuthenticateUserTestCorrect(string email, string password)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.AuthenticateUser(email, password);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie uwierzytelniania", result.Message);
            }
        }

        [Theory]
        [InlineData("j.kowalski@gmail.com", "password123123")]
        [InlineData("s.kowalczyk@gmail.com", "password123123")]
        [InlineData("a.nowak@gmail.com", "password123123")]
        public async Task AuthenticateUserBadPassword(string email, string password)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.AuthenticateUser(email, password);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Hasło nie jest poprawne", result.Message);
            }
        }

        [Theory]
        [InlineData("jjjjj.kowalski@gmail.com", "password123")]
        [InlineData("sssss.kowalczyk@gmail.com", "password123")]
        [InlineData("aaaaa.nowak@gmail.com", "password123")]
        public async Task AuthenticateUserBadEmail(string email, string password)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.UserRepository.AuthenticateUser(email, password);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Użytkownik nie istnieje", result.Message);
            }
        }

        [Fact]
        public async Task AuthenticateUserAccountNotActive()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                UserCreateDTO dto = new UserCreateDTO
                {
                    Email = "test@zzzdfg.zz",
                    Name = "Jan",
                    PhoneNumber = "123456789",
                    RoleName = "Laborant",
                    Surname = "Kowalski"
                };

                var user = await unityOfWork.UserRepository.AddUser(dto);
                await unityOfWork.UserRepository.ChangeActivity(user.Data.Id, new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"), false);

                var result = await unityOfWork.UserRepository.AuthenticateUser(dto.Email, user.Message);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Konto użytkownika nie jest aktywne", result.Message);
            }
        }
    }
}
