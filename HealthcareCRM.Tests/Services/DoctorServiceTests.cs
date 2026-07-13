using HealthcareCRM.Models;
using HealthcareCRM.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HealthcareCRM.Tests.Services
{
    public class DoctorServiceTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static Doctor MakeDoctor(string name, string specialization, string phone, bool isActive = true)
        {
            return new Doctor
            {
                Name = name,
                Specialization = specialization,
                Phone = phone,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetAllDoctorsIncludingInactive_ReturnsBothActiveAndInactive_OrderedByName()
        {
            using var context = CreateContext();
            context.Doctors.AddRange(
                MakeDoctor("Zed Smith", "Cardiology", "111", isActive: true),
                MakeDoctor("Amy Adams", "Dermatology", "222", isActive: false)
            );
            await context.SaveChangesAsync();

            var service = new DoctorService(context);
            var result = await service.GetAllDoctorsIncludingInactive();

            Assert.Equal(2, result.Count);
            Assert.Equal("Amy Adams", result[0].Name);
            Assert.Equal("Zed Smith", result[1].Name);
            Assert.Contains(result, d => !d.IsActive);
        }

        [Fact]
        public async Task GetAllDoctorsIncludingInactive_ReturnsEmptyList_WhenNoDoctorsExist()
        {
            using var context = CreateContext();
            var service = new DoctorService(context);

            var result = await service.GetAllDoctorsIncludingInactive();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDoctorById_ReturnsDoctor_WhenExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("John Doe", "Neurology", "333");
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context);
            var result = await service.GetDoctorById(doctor.Id);

            Assert.NotNull(result);
            Assert.Equal("John Doe", result!.Name);
        }

        [Fact]
        public async Task GetDoctorById_ReturnsNull_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var service = new DoctorService(context);

            var result = await service.GetDoctorById(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddDoctor_PersistsDoctor_AndSetsIsActiveTrueAndCreatedAt()
        {
            using var context = CreateContext();
            var service = new DoctorService(context);
            var newDoctor = new Doctor
            {
                Name = "Jane Roe",
                Specialization = "Pediatrics",
                Phone = "444",
                IsActive = false, // should be overridden by AddDoctor
                CreatedAt = DateTime.UtcNow.AddYears(-1) // should be overridden
            };

            var before = DateTime.UtcNow;
            var created = await service.AddDoctor(newDoctor);
            var after = DateTime.UtcNow;

            Assert.True(created.Id > 0);
            Assert.True(created.IsActive);
            Assert.InRange(created.CreatedAt, before.AddSeconds(-1), after.AddSeconds(1));

            var persisted = await context.Doctors.FindAsync(created.Id);
            Assert.NotNull(persisted);
            Assert.Equal("Jane Roe", persisted!.Name);
            Assert.True(persisted.IsActive);
        }

        [Fact]
        public async Task UpdateDoctor_UpdatesNameSpecializationAndPhone_WhenDoctorExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("Old Name", "Old Spec", "000");
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context);
            var updated = new Doctor
            {
                Name = "New Name",
                Specialization = "New Spec",
                Phone = "999"
            };

            var success = await service.UpdateDoctor(doctor.Id, updated);

            Assert.True(success);
            var persisted = await context.Doctors.FindAsync(doctor.Id);
            Assert.Equal("New Name", persisted!.Name);
            Assert.Equal("New Spec", persisted.Specialization);
            Assert.Equal("999", persisted.Phone);
        }

        [Fact]
        public async Task UpdateDoctor_DoesNotModifyIsActiveOrCreatedAt()
        {
            using var context = CreateContext();
            var originalCreatedAt = DateTime.UtcNow.AddDays(-5);
            var doctor = new Doctor
            {
                Name = "Old Name",
                Specialization = "Old Spec",
                Phone = "000",
                IsActive = false,
                CreatedAt = originalCreatedAt
            };
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context);
            var updated = new Doctor
            {
                Name = "New Name",
                Specialization = "New Spec",
                Phone = "999",
                IsActive = true // should have no effect via UpdateDoctor
            };

            await service.UpdateDoctor(doctor.Id, updated);

            var persisted = await context.Doctors.FindAsync(doctor.Id);
            Assert.False(persisted!.IsActive);
            Assert.Equal(originalCreatedAt, persisted.CreatedAt);
        }

        [Fact]
        public async Task UpdateDoctor_ReturnsFalse_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var service = new DoctorService(context);

            var success = await service.UpdateDoctor(999, new Doctor
            {
                Name = "Name",
                Specialization = "Spec",
                Phone = "123"
            });

            Assert.False(success);
        }

        [Fact]
        public async Task DeactivateDoctor_SetsIsActiveFalse_AndReturnsTrue_WhenDoctorExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("Active Doc", "Spec", "123", isActive: true);
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context);
            var success = await service.DeactivateDoctor(doctor.Id);

            Assert.True(success);
            var persisted = await context.Doctors.FindAsync(doctor.Id);
            Assert.False(persisted!.IsActive);
        }

        [Fact]
        public async Task DeactivateDoctor_ReturnsFalse_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var service = new DoctorService(context);

            var success = await service.DeactivateDoctor(999);

            Assert.False(success);
        }

        [Fact]
        public async Task ReactivateDoctor_SetsIsActiveTrue_AndReturnsTrue_WhenDoctorExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("Inactive Doc", "Spec", "123", isActive: false);
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context);
            var success = await service.ReactivateDoctor(doctor.Id);

            Assert.True(success);
            var persisted = await context.Doctors.FindAsync(doctor.Id);
            Assert.True(persisted!.IsActive);
        }

        [Fact]
        public async Task ReactivateDoctor_ReturnsFalse_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var service = new DoctorService(context);

            var success = await service.ReactivateDoctor(999);

            Assert.False(success);
        }
    }
}