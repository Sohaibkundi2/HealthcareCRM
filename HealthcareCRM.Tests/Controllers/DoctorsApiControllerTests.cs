using HealthcareCRM.Controllers;
using HealthcareCRM.Models;
using HealthcareCRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HealthcareCRM.Tests.Controllers
{
    public class DoctorsApiControllerTests
    {
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private static DoctorsApiController CreateController(AppDbContext context)
        {
            return new DoctorsApiController(new DoctorService(context));
        }

        private static object? GetProp(object obj, string name)
        {
            return obj.GetType().GetProperty(name)?.GetValue(obj);
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

        // ----- GetAll -----

        [Fact]
        public async Task GetAll_DefaultsToActiveOnly_WhenIncludeInactiveNotSpecified()
        {
            using var context = CreateContext();
            context.Doctors.AddRange(
                MakeDoctor("Active Doc", "Cardiology", "111", isActive: true),
                MakeDoctor("Inactive Doc", "Dermatology", "222", isActive: false)
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = Assert.IsType<OkObjectResult>(await controller.GetAll());

            var data = Assert.IsAssignableFrom<List<Doctor>>(GetProp(result.Value!, "data"));
            Assert.Single(data);
            Assert.Equal("Active Doc", data[0].Name);
        }

        [Fact]
        public async Task GetAll_ReturnsAllDoctors_WhenIncludeInactiveTrue()
        {
            using var context = CreateContext();
            context.Doctors.AddRange(
                MakeDoctor("Active Doc", "Cardiology", "111", isActive: true),
                MakeDoctor("Inactive Doc", "Dermatology", "222", isActive: false)
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = Assert.IsType<OkObjectResult>(await controller.GetAll(includeInactive: true));

            var data = Assert.IsAssignableFrom<List<Doctor>>(GetProp(result.Value!, "data"));
            Assert.Equal(2, data.Count);
        }

        // ----- GetById -----

        [Fact]
        public async Task GetById_ReturnsOkWithDoctor_WhenDoctorExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("John Doe", "Neurology", "333");
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = Assert.IsType<OkObjectResult>(await controller.GetById(doctor.Id));

            var data = Assert.IsType<Doctor>(GetProp(result.Value!, "data"));
            Assert.Equal("John Doe", data.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = Assert.IsType<NotFoundObjectResult>(await controller.GetById(999));

            Assert.Equal("Doctor not found", GetProp(result.Value!, "message"));
        }

        // ----- Create -----

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenNameIsMissing()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "", Specialization = "Cardiology", Phone = "111" };

            var result = Assert.IsType<BadRequestObjectResult>(await controller.Create(doctor));

            Assert.Equal("Name is required", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenSpecializationIsMissing()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "Jane Roe", Specialization = "   ", Phone = "111" };

            var result = Assert.IsType<BadRequestObjectResult>(await controller.Create(doctor));

            Assert.Equal("Specialization is required", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenPhoneIsMissing()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "Jane Roe", Specialization = "Cardiology", Phone = "" };

            var result = Assert.IsType<BadRequestObjectResult>(await controller.Create(doctor));

            Assert.Equal("Phone is required", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_AndPersistsDoctor_WhenValid()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "Jane Roe", Specialization = "Pediatrics", Phone = "444" };

            var result = Assert.IsType<CreatedAtActionResult>(await controller.Create(doctor));

            Assert.Equal(nameof(DoctorsApiController.GetById), result.ActionName);
            var data = Assert.IsType<Doctor>(GetProp(result.Value!, "data"));
            Assert.Equal("Jane Roe", data.Name);
            Assert.True(data.IsActive);
            Assert.True(data.Id > 0);

            var persisted = await context.Doctors.FindAsync(data.Id);
            Assert.NotNull(persisted);
        }

        // ----- Update -----

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenNameIsMissing()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "", Specialization = "Cardiology", Phone = "111" };

            var result = Assert.IsType<BadRequestObjectResult>(await controller.Update(1, doctor));

            Assert.Equal("Name is required", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenSpecializationIsMissing()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "Jane Roe", Specialization = "", Phone = "111" };

            var result = Assert.IsType<BadRequestObjectResult>(await controller.Update(1, doctor));

            Assert.Equal("Specialization is required", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenPhoneIsMissing()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "Jane Roe", Specialization = "Cardiology", Phone = "" };

            var result = Assert.IsType<BadRequestObjectResult>(await controller.Update(1, doctor));

            Assert.Equal("Phone is required", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var controller = CreateController(context);
            var doctor = new Doctor { Name = "Jane Roe", Specialization = "Cardiology", Phone = "111" };

            var result = Assert.IsType<NotFoundObjectResult>(await controller.Update(999, doctor));

            Assert.Equal("Doctor not found", GetProp(result.Value!, "message"));
        }

        [Fact]
        public async Task Update_ReturnsOk_AndPersistsChanges_WhenValid()
        {
            using var context = CreateContext();
            var existing = MakeDoctor("Old Name", "Old Spec", "000");
            context.Doctors.Add(existing);
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var updated = new Doctor { Name = "New Name", Specialization = "New Spec", Phone = "999" };

            var result = Assert.IsType<OkObjectResult>(await controller.Update(existing.Id, updated));

            Assert.Equal("Doctor updated successfully", GetProp(result.Value!, "message"));
            var persisted = await context.Doctors.FindAsync(existing.Id);
            Assert.Equal("New Name", persisted!.Name);
            Assert.Equal("New Spec", persisted.Specialization);
            Assert.Equal("999", persisted.Phone);
        }

        // ----- Deactivate -----

        [Fact]
        public async Task Deactivate_ReturnsOk_AndSetsIsActiveFalse_WhenDoctorExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("Active Doc", "Spec", "123", isActive: true);
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = Assert.IsType<OkObjectResult>(await controller.Deactivate(doctor.Id));

            Assert.Equal("Doctor deactivated successfully", GetProp(result.Value!, "message"));
            var persisted = await context.Doctors.FindAsync(doctor.Id);
            Assert.False(persisted!.IsActive);
        }

        [Fact]
        public async Task Deactivate_ReturnsNotFound_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = Assert.IsType<NotFoundObjectResult>(await controller.Deactivate(999));

            Assert.Equal("Doctor not found", GetProp(result.Value!, "message"));
        }

        // ----- Reactivate -----

        [Fact]
        public async Task Reactivate_ReturnsOk_AndSetsIsActiveTrue_WhenDoctorExists()
        {
            using var context = CreateContext();
            var doctor = MakeDoctor("Inactive Doc", "Spec", "123", isActive: false);
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = Assert.IsType<OkObjectResult>(await controller.Reactivate(doctor.Id));

            Assert.Equal("Doctor reactivated successfully", GetProp(result.Value!, "message"));
            var persisted = await context.Doctors.FindAsync(doctor.Id);
            Assert.True(persisted!.IsActive);
        }

        [Fact]
        public async Task Reactivate_ReturnsNotFound_WhenDoctorDoesNotExist()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var result = Assert.IsType<NotFoundObjectResult>(await controller.Reactivate(999));

            Assert.Equal("Doctor not found", GetProp(result.Value!, "message"));
        }
    }
}