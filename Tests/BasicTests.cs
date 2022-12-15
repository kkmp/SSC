using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSC.Data;
using SSC.Data.UnitOfWork;
using SSC.DTO.MedicalHistory;
using SSC.DTO.Patient;
using SSC.DTO.Place;
using SSC.DTO.Treatment;
using SSC.DTO.TreatmentDiseaseCourse;
using SSC.DTO.User;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class BasicTests : IClassFixture<BaseApplicationFactory>
    {
        private readonly BaseApplicationFactory _factory;

        public BasicTests(BaseApplicationFactory factory)
        {
            _factory = factory;
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<DataContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Theory]
        [InlineData("test@zzzdfg.zz", "tester", "547854125", "Administrator", "testowy")]
        [InlineData("test2@zzzdfg.zz", "tester2", "547854122", "Laborant", "testowy2")]
        [InlineData("test3@zzzdfg.zz", "tester3", "547854123", "Lekarz", "testowy3")]
        public async Task AddUserTestCorrect(
            string email, 
            string name, 
            string phoneNumber, 
            string roleName, 
            string surname
            )
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
        [Repeat(3)]
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
        [Repeat(3)]
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
        public async Task AuthenticateUserIncorrectPassword(string email, string password)
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
        public async Task AuthenticateUserIncorrectEmail(string email, string password)
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
        [Repeat(3)]
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


        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'M', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'M', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021956165", "Joanna", "Kowalska", 'F', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task AddPatientTestCorrect(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(dto.Pesel, result.Data.Pesel);
                Assert.Equal(dto.Name, result.Data.Name);
                Assert.Equal(dto.Surname, result.Data.Surname);
                Assert.Equal(dto.BirthDate, result.Data.BirthDate);
                Assert.Equal(dto.Street, result.Data.Street);
                Assert.Equal(dto.Address, result.Data.Address);
                Assert.Equal(dto.CityId, result.Data.CityId);
                Assert.Equal(dto.CitizenshipId, result.Data.CitizenshipId);
                Assert.Equal("Pacjent został dodany", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'M', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'M', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021998398", "Jan", "Kowalski", 'M', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task AddPatientPatientExists(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));
                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent został już dodany", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984715", "Jan", "Kowalski", 'M', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983735", "Marcel", "Kowal", 'M', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021998395", "Jan", "Kowalski", 'M', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task AddPatientIncorrectPesel(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Niepoprawny numer PESEL", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'M', "16.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'M', "19.08.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021998398", "Jan", "Kowalski", 'M', "19.02.2017", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task AddPatientIncorrectBirthDate(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Data urodzenia nie jest powiązana z numerem PESEL", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'F', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'F', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021956165", "Joanna", "Kowalska", 'M', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task AddPatientIncorrectSex(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Płeć nie jest powiązana z numerem PESEL", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'M', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-0000-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'M', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-0304-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021956165", "Joanna", "Kowalska", 'F', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-0333-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task AddPatientCityDoesNotExist(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Miasto nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'M', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-0000-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'M', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-0000-d2e6323d412e")]
        [InlineData("94021956165", "Joanna", "Kowalska", 'F', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-4565-d2e6323d412e")]
        public async Task AddPatientCitizenshipDoesNotExist(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Obywatelstwo nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("Jan", "Kowalski", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("Marcel", "Kowal", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("Marcin", "Masłowski", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task EditPatientTestCorrect(string name, string surname, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                PatientUpdateDTO updateDto = new PatientUpdateDTO
                {
                    Id = patient.Data.Id,
                    Name = name,
                    Surname = surname,
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.EditPatient(updateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Pacjent został zedytowany", result.Message);
                Assert.Equal(updateDto.Name, result.Data.Name);
                Assert.Equal(updateDto.Surname, result.Data.Surname);
                Assert.Equal(updateDto.Street, result.Data.Street);
                Assert.Equal(updateDto.Address, result.Data.Address);
                Assert.Equal(updateDto.CityId, result.Data.CityId);
                Assert.Equal(updateDto.CitizenshipId, result.Data.CitizenshipId);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task EditPatientPatientDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientUpdateDTO updateDto = new PatientUpdateDTO
                {
                    Id = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    Name = "Jan",
                    Surname = "Kowalski",
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var result = await unityOfWork.PatientRepository.EditPatient(updateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("Jan", "Kowalski", "Chlebowa", "16 c", "01288c5a-0000-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("Marcel", "Kowal", "Dębowa", "16 c", "01288c5a-1a7c-0000-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("Marcin", "Masłowski", "Bogata", "16 c", "01288c5a-1a7c-0000-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task EditPatientCityDoesNotExist(string name, string surname, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                PatientUpdateDTO updateDto = new PatientUpdateDTO
                {
                    Id = patient.Data.Id,
                    Name = name,
                    Surname = surname,
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.EditPatient(updateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Miasto nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("Jan", "Kowalski", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-0000-41a3-a0b9-d2e6323d412e")]
        [InlineData("Marcel", "Kowal", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-0000-a0b9-d2e6323d412e")]
        [InlineData("Marcin", "Masłowski", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-0000-d2e6323d412e")]
        public async Task EditPatientCitizenshipDoesNotExist(string name, string surname, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                PatientUpdateDTO updateDto = new PatientUpdateDTO
                {
                    Id = patient.Data.Id,
                    Name = name,
                    Surname = surname,
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var result = await unityOfWork.PatientRepository.EditPatient(updateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Obywatelstwo nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("94021984711", "Jan", "Kowalski", 'M', "19.02.1994", "Chlebowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021983734", "Marcel", "Kowal", 'M', "19.02.1994", "Dębowa", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        [InlineData("94021998398", "Jan", "Kowalski", 'M', "19.02.1994", "Bogata", "16 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232", "35cb0a90-9589-41a3-a0b9-d2e6323d412e")]
        public async Task PatientDetailsTestCorrect(string pesel, string name, string surname, char sex, string birthdate, string street, string address, string cityId, string citizenshipId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = pesel,
                    Name = name,
                    Surname = surname,
                    Sex = sex,
                    BirthDate = DateTime.Parse(birthdate),
                    Street = street,
                    Address = address,
                    CityId = new Guid(cityId),
                    CitizenshipId = new Guid(citizenshipId)
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                var result = await unityOfWork.PatientRepository.PatientDetails(patient.Data.Id);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);
                Assert.Equal(patient.Data.Id, result.Data.Id);
                Assert.Equal(patient.Data.Pesel, result.Data.Pesel);
                Assert.Equal(patient.Data.Name, result.Data.Name);
                Assert.Equal(patient.Data.Surname, result.Data.Surname);
                Assert.Equal(patient.Data.Sex, result.Data.Sex);
                Assert.Equal(patient.Data.BirthDate, result.Data.BirthDate);
                Assert.Equal(patient.Data.Street, result.Data.Street);
                Assert.Equal(patient.Data.Address, result.Data.Address);
                Assert.Equal(patient.Data.CityId, result.Data.CityId);
                Assert.Equal(patient.Data.CitizenshipId, result.Data.CitizenshipId);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task PatientDetailsPatientDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                var result = await unityOfWork.PatientRepository.PatientDetails(new Guid("00000000-0000-0000-0000-000000000000"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("3e7e4cc3-d3e1-4917-9647-42422b4c429a")]
        [InlineData("6fc75b09-f3a5-41eb-894a-318e02e1e2e1")]
        [InlineData("9c489ff7-2100-443c-bbf6-e2737ce6ee83")]
        public async Task AddCodeTestCorrect(string id)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();
                var context = scopedServices.GetRequiredService<DataContext>();

                var result = await unityOfWork.ChangePasswordRepository.AddCode(new Guid(id));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);

                var test = await context.ChangePasswordCodes.FirstOrDefaultAsync(x => x.UserId == new Guid(id));
                Assert.NotNull(test);
                Assert.Equal(test.Code, result.Data.Code);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ChangeCodeTestCorrect()
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
                var user = await unityOfWork.UserRepository.AddUser(dto);
                var code = await unityOfWork.ChangePasswordRepository.AddCode(user.Data.Id);

                var result = await unityOfWork.ChangePasswordRepository.ChangeCode("P@ssword123", code.Data.Code);

                Assert.True(result.Success);
                Assert.Equal("Hasło użytkownika zostało zmienione", result.Message);
                Assert.True(unityOfWork.UserRepository.AuthenticateUser(dto.Email, "P@ssword123").Result.Success);
            }
        }

        [Theory]
        [InlineData("j.kowalski@gmail.com", "P@ssword123")]
        [InlineData("s.kowalczyk@gmail.com", "P@ssw0rd658")]
        [InlineData("a.nowak@gmail.com", "P@ssWrd123")]
        public async Task ChangeCodeCodeDoesNotExist(string email, string password)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.ChangePasswordRepository.ChangeCode(password, "CTJNQjOY8znMAOqddCRCJt5eHN9kn9UqXauhGOHlGbdS6tjIGoEQ9iljqk4ZrM8YTTlhsyS89IMvNSdZngHMaCvnxUf0fL40QUQR");

                Assert.False(result.Success);
                Assert.Equal("Kod nie istnieje", result.Message);
                Assert.False(unityOfWork.UserRepository.AuthenticateUser(email, password).Result.Success);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ChangeCodeCodeExpired()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();
                var context = scopedServices.GetRequiredService<DataContext>();

                var code = await unityOfWork.ChangePasswordRepository.AddCode(new Guid("3e7e4cc3-d3e1-4917-9647-42422b4c429a"));
                code.Data.ExpiredDate = DateTime.Parse("01.01.1999");
                context.Update(code.Data);
                await context.SaveChangesAsync();

                var result = await unityOfWork.ChangePasswordRepository.ChangeCode("P@ssword123", code.Data.Code);

                Assert.False(result.Success);
                Assert.Equal("Kod stracił ważność", result.Message);
                Assert.False(unityOfWork.UserRepository.AuthenticateUser("j.kowalski@gmail.com", "P@ssword123").Result.Success);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ChangePasswordTestCorrect()
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
                var user = await unityOfWork.UserRepository.AddUser(dto);

                var result = await unityOfWork.ChangePasswordRepository.ChangePassword(user.Message, "P@ssword123", user.Data.Id);

                Assert.True(result.Success);
                Assert.Equal("Hasło użytkownika zostało zmienione", result.Message);

                Assert.True(unityOfWork.UserRepository.AuthenticateUser(dto.Email, "P@ssword123").Result.Success);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ChangePasswordIncorrectOldPassword()
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
                var user = await unityOfWork.UserRepository.AddUser(dto);

                var result = await unityOfWork.ChangePasswordRepository.ChangePassword("Błędne hasło", "P@ssword123", user.Data.Id);

                Assert.False(result.Success);
                Assert.Equal("Hasło niepoprawne", result.Message);

                Assert.False(unityOfWork.UserRepository.AuthenticateUser(dto.Email, "P@ssword123").Result.Success);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ChangePasswordTheSamePasswords()
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
                var user = await unityOfWork.UserRepository.AddUser(dto);

                var result = await unityOfWork.ChangePasswordRepository.ChangePassword(user.Message, user.Message, user.Data.Id);

                Assert.False(result.Success);
                Assert.Equal("Stare hasło nie może być takie samo jak nowe hasło", result.Message);
            }
        }

        [Theory]
        [InlineData("12.06.2022", "Przykładowy opis 1")]
        [InlineData("04.05.2019", "Przykładowy opis 2")]
        [InlineData("31.07.2021", "Przykładowy opis 3")]
        public async Task AddMedicalHistoryTestCorrect(string date, string description)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                MedicalHistoryCreateDTO medicalHistoryDto = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    PatientId = patient.Data.Id
                };

                var result = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Dodano historię choroby", result.Message);
                Assert.Equal(medicalHistoryDto.Date, result.Data.Date);
                Assert.Equal(medicalHistoryDto.Description, result.Data.Description);
                Assert.Equal(medicalHistoryDto.PatientId, result.Data.PatientId);
            }
        }

        [Theory]
        [InlineData("12.06.2022", "Przykładowy opis 1")]
        [InlineData("04.05.2022", "Przykładowy opis 2")]
        [InlineData("31.07.2021", "Przykładowy opis 3")]
        public async Task AddMedicalHistoryIncorrectDate(string date, string description)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                MedicalHistoryCreateDTO medicalHistoryDto1 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("19.12.2022"),
                    Description = "Test",
                    PatientId = patient.Data.Id
                };

                await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto1, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                MedicalHistoryCreateDTO medicalHistoryDto2 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    PatientId = patient.Data.Id
                };

                var result = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto2, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Nie można dodać historii choroby. Istnieje już wpis przed podaną datą", result.Message);
            }
        }

        [Theory]
        [InlineData("12.06.2022", "Przykładowy opis 1")]
        [InlineData("04.05.2022", "Przykładowy opis 2")]
        [InlineData("31.07.2021", "Przykładowy opis 3")]
        public async Task AddMedicalHistoryPatientDoesNotExist(string date, string description)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                MedicalHistoryCreateDTO medicalHistoryDto = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    PatientId = new Guid("01288c5a-0000-4b19-9103-62d5ad35d232")
                };

                var result = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("12.06.2022", "Przykładowy opis 1")]
        [InlineData("04.05.2019", "Przykładowy opis 2")]
        [InlineData("31.07.2021", "Przykładowy opis 3")]
        public async Task EditMedicalHistoryTestCorrect(string date, string description)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                MedicalHistoryCreateDTO medicalHistoryDto = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "test",
                    PatientId = patient.Data.Id
                };

                var medicalHistory = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                MedicalHistoryUpdateDTO updateDto = new MedicalHistoryUpdateDTO
                {
                    Id = medicalHistory.Data.Id,
                    Date = DateTime.Parse(date),
                    Description = description
                };

                var result = await unityOfWork.MedicalHistoryRepository.EditMedicalHistory(updateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Historia choroby została zedytowana", result.Message);
                Assert.Equal(updateDto.Date, result.Data.Date);
                Assert.Equal(updateDto.Description, result.Data.Description);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task EditMedicalHistoryHistoryDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                MedicalHistoryUpdateDTO dto = new MedicalHistoryUpdateDTO
                {
                    Id = new Guid("01288c5a-0000-4b19-9103-62d5ad35d232"),
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "Test"
                };

                var result = await unityOfWork.MedicalHistoryRepository.EditMedicalHistory(dto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Nie ma takiego wpisu w historii choroby", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task EditMedicalHistoryIncorrectUser()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryCreateDTO medicalHistoryDto = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "test",
                    PatientId = patient.Data.Id
                };

                var medicalHistory = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryUpdateDTO updateDto = new MedicalHistoryUpdateDTO
                {
                    Id = medicalHistory.Data.Id,
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "Test"
                };

                var result = await unityOfWork.MedicalHistoryRepository.EditMedicalHistory(updateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Tylko użytkownik, który dodał wpis do historii choroby może go edytować", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task EditMedicalHistoryHistoryCannotBeEditedAnymore()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryCreateDTO medicalHistoryDto1 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "test",
                    PatientId = patient.Data.Id
                };

                MedicalHistoryCreateDTO medicalHistoryDto2 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("02.01.2022"),
                    Description = "test 2",
                    PatientId = patient.Data.Id
                };

                var medicalHistory = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto1, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));
                await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto2, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryUpdateDTO updateDto = new MedicalHistoryUpdateDTO
                {
                    Id = medicalHistory.Data.Id,
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "Test"
                };

                var result = await unityOfWork.MedicalHistoryRepository.EditMedicalHistory(updateDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Tej historii choroby nie można już edytować", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task EditMedicalHistoryIncorrectDate()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryCreateDTO medicalHistoryDto1 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "test",
                    PatientId = patient.Data.Id
                };

                MedicalHistoryCreateDTO medicalHistoryDto2 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("02.01.2022"),
                    Description = "test 2",
                    PatientId = patient.Data.Id
                };

                await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto1, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));
                var medicalHistory = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto2, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryUpdateDTO updateDto = new MedicalHistoryUpdateDTO
                {
                    Id = medicalHistory.Data.Id,
                    Date = DateTime.Parse("31.12.2020"),
                    Description = "Test"
                };

                var result = await unityOfWork.MedicalHistoryRepository.EditMedicalHistory(updateDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Nie można dodać historii choroby. Istnieje już wpis przed podaną datą", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ShowMedicalHistoriesTestCorrect()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryCreateDTO medicalHistoryDto1 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "test",
                    PatientId = patient.Data.Id
                };

                MedicalHistoryCreateDTO medicalHistoryDto2 = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("02.01.2022"),
                    Description = "test 2",
                    PatientId = patient.Data.Id
                };

                await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto1, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));
                await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto2, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                var result = await unityOfWork.MedicalHistoryRepository.ShowMedicalHistories(patient.Data.Id);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ShowMedicalHistoriesPatientDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.MedicalHistoryRepository.ShowMedicalHistories(new Guid("9c489ff7-0000-443c-bbf6-e2737ce6ee83"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent nie istnieje", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ShowMedicalHistoryDetailsTestCorrect()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                MedicalHistoryCreateDTO medicalHistoryDto = new MedicalHistoryCreateDTO
                {
                    Date = DateTime.Parse("01.01.2022"),
                    Description = "test",
                    PatientId = patient.Data.Id
                };

                var medicalHistory = await unityOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistoryDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                var result = await unityOfWork.MedicalHistoryRepository.ShowMedicalHistoryDetails(medicalHistory.Data.Id);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ShowMedicalHistoryDetailsHistoryDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.MedicalHistoryRepository.ShowMedicalHistoryDetails(new Guid("9c489ff7-2100-0000-bbf6-e2737ce6ee83"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Historia choroby nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("Placówka 1", "Słoneczna 14 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232")]
        [InlineData("Placówka 2", "Jesienna 12/4", "01288c5a-1a7c-4b19-9103-62d5ad35d232")]
        [InlineData("Placówka 3", "Wesoła 59", "2b22474e-8adb-4e82-915a-d253f6ac5d78")]
        public async Task AddPlaceTestCorrect(string name, string street, string cityId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PlaceCreateDTO dto = new PlaceCreateDTO
                {
                    Name = name,
                    Street = street,
                    CityId = new Guid(cityId)
                };

                var result = await unityOfWork.PlaceRepository.AddPlace(dto);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Placówka została dodana", result.Message);
                Assert.Equal(dto.Name, result.Data.Name);
                Assert.Equal(dto.Street, result.Data.Street);
                Assert.Equal(dto.CityId, result.Data.CityId);
            }
        }

        [Theory]
        [InlineData("Placówka 1", "Słoneczna 14 c", "01288c5a-1a7c-4b19-9103-62d5ad35d232")]
        [InlineData("Placówka 2", "Jesienna 12/4", "01288c5a-1a7c-4b19-9103-62d5ad35d232")]
        [InlineData("Placówka 3", "Wesoła 59", "2b22474e-8adb-4e82-915a-d253f6ac5d78")]
        public async Task AddPlacePlaceExists(string name, string street, string cityId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PlaceCreateDTO dto = new PlaceCreateDTO
                {
                    Name = name,
                    Street = street,
                    CityId = new Guid(cityId)
                };

                await unityOfWork.PlaceRepository.AddPlace(dto);
                var result = await unityOfWork.PlaceRepository.AddPlace(dto);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Placówka została już dodana", result.Message);
            }
        }

        [Theory]
        [InlineData("Placówka 1", "Słoneczna 14 c", "01288c5a-0000-4b19-9103-62d5ad35d232")]
        [InlineData("Placówka 2", "Jesienna 12/4", "01288c5a-1a7c-0000-9103-62d5ad35d232")]
        [InlineData("Placówka 3", "Wesoła 59", "2b22474e-8adb-4e82-0000-d253f6ac5d78")]
        public async Task AddPlaceCityDoesNotExist(string name, string street, string cityId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PlaceCreateDTO dto = new PlaceCreateDTO
                {
                    Name = name,
                    Street = street,
                    CityId = new Guid(cityId)
                };

                var result = await unityOfWork.PlaceRepository.AddPlace(dto);

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Miejscowość nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("af4fafea-b5fe-4a17-be95-60f35f4cca0a", "Administrator")]
        [InlineData("64afcb40-4e45-4b94-9f31-19b57c20027f", "Lekarz")]
        [InlineData("0c051c70-61a2-44bb-bf9c-691bf30a9c11", "Laborant")]
        public async Task GetRoleTestCorrect(string id, string resultName)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.RoleRepository.GetRole(new Guid(id));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);
                Assert.Equal(resultName, result.Data.Name);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task GetRoleRoleDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.RoleRepository.GetRole(new Guid("5a055498-0000-4d96-a8a1-8ef5b08f3e5a"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Rola nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "455df914-7758-4e95-8e15-a32c8dce71aa")]
        [InlineData("11.03.2020", "Opis 2", "455df914-7758-4e95-8e15-a32c8dce71aa")]
        [InlineData("30.04.2022", "Opis 3", "455df914-7758-4e95-8e15-a32c8dce71aa")]
        public async Task AddTreatmentDiseaseCourseTestCorrect(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = patient.Data.Id
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powikłanie zostało dodane", result.Message);
                Assert.Equal(treatmentDiseaseCourseDto.Date, result.Data.Date);
                Assert.Equal(treatmentDiseaseCourseDto.Description, result.Data.Description);
                Assert.Equal(treatmentDiseaseCourseDto.PatientId, result.Data.Treatment.PatientId);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "455df914-7758-0000-8e15-a32c8dce71aa")]
        [InlineData("11.03.2020", "Opis 2", "455df914-0000-4e95-8e15-a32c8dce71aa")]
        [InlineData("30.04.2022", "Opis 3", "455df914-7758-4e95-0000-a32c8dce71aa")]
        public async Task AddTreatmentDiseaseCourseDiseaseDoesNotExists(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = patient.Data.Id
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Powikłanie nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "455df914-7758-0000-8e15-a32c8dce71aa")]
        [InlineData("11.03.2020", "Opis 2", "455df914-0000-4e95-8e15-a32c8dce71aa")]
        [InlineData("30.04.2022", "Opis 3", "455df914-7758-4e95-0000-a32c8dce71aa")]
        public async Task AddTreatmentDiseaseCoursePatientDoesNotExists(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = new Guid("6eac697b-0000-4d87-bf3c-51ba2308e75f")
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("11.03.2020", "Opis 2", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("30.04.2021", "Opis 3", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        public async Task AddTreatmentDiseaseCourseCannotAddBeforeTreatment(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto1 = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse("31.12.2022"),
                    Description = "Test",
                    DiseaseCourseId = new Guid("87feb19d-84c7-4332-856e-cbeefb08a66d"),
                    PatientId = patient.Data.Id
                };

                await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto1, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto2 = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = patient.Data.Id
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto2, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Nie można dodać wpisu przed datą rozpoczęcia leczenia", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("11.03.2020", "Opis 2", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("30.04.2021", "Opis 3", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        public async Task AddTreatmentDiseaseCourseCannotAddBeforeOtherTreatmentDiseaseCourse(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentCreateDTO treatmentDto = new TreatmentCreateDTO
                {
                    StartDate = DateTime.Parse("01.01.2019"),
                    PatientId = patient.Data.Id,
                    TreatmentStatusId = new Guid("3b55e949-1c36-4182-b0c4-38eaf9e70251")
                };

                await unityOfWork.TreatmentRepository.AddTreatment(treatmentDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto1 = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse("31.12.2022"),
                    Description = "Test",
                    DiseaseCourseId = new Guid("87feb19d-84c7-4332-856e-cbeefb08a66d"),
                    PatientId = patient.Data.Id
                };

                await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto1, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto2 = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = patient.Data.Id
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto2, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Nie można dodać wpisu przed innym powikłaniem", result.Message);
            }
        }


        [Theory]
        [InlineData("19.02.2022", "Opis 1", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("11.03.2020", "Opis 2", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("30.04.2022", "Opis 3", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        public async Task ShowTreatmentDiseaseCoursesTestCorrect(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = patient.Data.Id
                };
                await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));
                
                var result = await unityOfWork.TreatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourses(patient.Data.Id);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("11.03.2020", "Opis 2", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("30.04.2022", "Opis 3", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        public async Task ShowTreatmentDiseaseCoursesPatientDoesNotExist(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourses(new Guid("85ddc1f6-b4d5-0000-9e1f-7d8d1e7e6bc9"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Pacjent nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("11.03.2020", "Opis 2", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        [InlineData("30.04.2022", "Opis 3", "5a055498-aa04-4d96-a8a1-8ef5b08f3e5a")]
        public async Task ShowTreatmentDiseaseCourseDetailsTestCorrect(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                    PatientId = patient.Data.Id
                };
                var treatmentDiseaseCourse = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourseDetails(treatmentDiseaseCourse.Data.Id);

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powodzenie", result.Message);
                Assert.Equal(treatmentDiseaseCourseDto.Date, result.Data.Date);
                Assert.Equal(treatmentDiseaseCourseDto.Description, result.Data.Description);
                Assert.Equal(treatmentDiseaseCourseDto.PatientId, result.Data.Treatment.PatientId);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task ShowTreatmentDiseaseCourseDiseaseCourseDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourseDetails(new Guid("01288c5a-1a7c-0000-9103-62d5ad35d232"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Powikłanie nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("19.02.2022", "Opis 1", "455df914-7758-4e95-8e15-a32c8dce71aa")]
        [InlineData("11.03.2020", "Opis 2", "455df914-7758-4e95-8e15-a32c8dce71aa")]
        [InlineData("30.04.2022", "Opis 3", "455df914-7758-4e95-8e15-a32c8dce71aa")]
        public async Task EditTreatmentDiseaseCourseTestCorrect(string date, string description, string diseaseCourseId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourseDto = new TreatmentDiseaseCourseCreateDTO
                {
                    Date = DateTime.Parse("01.02.2019"),
                    Description = "Test",
                    DiseaseCourseId = new Guid("b942cf77-74c0-47f7-a626-dd8784cbcd3e"),
                    PatientId = patient.Data.Id
                };
                var treatmentDiseaseCourse = await unityOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourseDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourseUpdateDto = new TreatmentDiseaseCourseUpdateDTO
                {
                    Id = treatmentDiseaseCourse.Data.Id,
                    Date = DateTime.Parse(date),
                    Description = description,
                    DiseaseCourseId = new Guid(diseaseCourseId),
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.EditTreatmentDiseaseCourse(treatmentDiseaseCourseUpdateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Powikłanie zostało zedytowane", result.Message);
                Assert.Equal(treatmentDiseaseCourseUpdateDto.Date, result.Data.Date);
                Assert.Equal(treatmentDiseaseCourseUpdateDto.Description, result.Data.Description);
                Assert.Equal(treatmentDiseaseCourseUpdateDto.DiseaseCourseId, result.Data.DiseaseCourseId);
            }
        }

        [Fact]
        [Repeat(3)]
        public async Task EditTreatmentDiseaseCourseDiseaseCourseDoesNotExist()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourseUpdateDto = new TreatmentDiseaseCourseUpdateDTO
                {
                    Id = new Guid("6fc75b09-0000-0000-0000-318e02e1e2e1"),
                    Date = DateTime.Parse("19.12.2022"),
                    Description = "Test",
                    DiseaseCourseId = new Guid("ce4d9efd-4065-4c7d-ad64-20ad0e2b65c1"),
                };

                var result = await unityOfWork.TreatmentDiseaseCourseRepository.EditTreatmentDiseaseCourse(treatmentDiseaseCourseUpdateDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Powikłanie nie istnieje", result.Message);
            }
        }

        [Theory]
        [InlineData("12.01.2021", "01.01.2022", false, "3b55e949-1c36-4182-b0c4-38eaf9e70251")]
        [InlineData("31.01.2021", "09.03.2022", true, "3b55e949-1c36-4182-b0c4-38eaf9e70251")]
        [InlineData("12.01.2021", "12.12.2021", true, "3b55e949-1c36-4182-b0c4-38eaf9e70251")]
        public async Task AddTreatmentTestCorrect(string startDate, string endDate, bool isCovid, string tratmentStatusId)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var unityOfWork = scopedServices.GetRequiredService<IUnitOfWork>();

                PatientCreateDTO dto = new PatientCreateDTO
                {
                    Pesel = "94021984711",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Sex = 'M',
                    BirthDate = DateTime.Parse("19.02.1994"),
                    Street = "Słoneczna",
                    Address = "45 a",
                    CityId = new Guid("01288c5a-1a7c-4b19-9103-62d5ad35d232"),
                    CitizenshipId = new Guid("35cb0a90-9589-41a3-a0b9-d2e6323d412e")
                };

                var patient = await unityOfWork.PatientRepository.AddPatient(dto, new Guid("9c489ff7-2100-443c-bbf6-e2737ce6ee83"));

                TreatmentCreateDTO treatmentDto = new TreatmentCreateDTO
                {
                    StartDate = DateTime.Parse(startDate),
                    EndDate = DateTime.Parse(endDate),
                    IsCovid = isCovid,
                    PatientId = patient.Data.Id,
                    TreatmentStatusId = new Guid(tratmentStatusId)

                };

                var result = await unityOfWork.TreatmentRepository.AddTreatment(treatmentDto, new Guid("6fc75b09-f3a5-41eb-894a-318e02e1e2e1"));

                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal("Leczenie zostało dodane", result.Message);
                Assert.Equal(treatmentDto.StartDate, result.Data.StartDate);
                Assert.Equal(treatmentDto.EndDate, result.Data.EndDate);
                Assert.Equal(treatmentDto.IsCovid, result.Data.IsCovid);
                Assert.Equal(treatmentDto.PatientId, result.Data.PatientId);
                Assert.Equal(treatmentDto.TreatmentStatusId, result.Data.TreatmentStatusId);
            }
        }
    }
}
