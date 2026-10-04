using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Enums;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.MariaDb.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tools.Constants;

namespace Infrastructure.Persistence.MariaDb.Seeders
{
    public class TestDataSeeder(
        WritableDbContext context,
        UserManager<UserDao> userManager,
        IOptions<DataConfiguration> dataConfig,
        TimeProvider timeProvider
    ) : SeederBase(context, userManager)
    {
        public override async Task SeedDataAsync()
        {
            if (!dataConfig.Value.SeedTest)
            {
                return;
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(
                async () =>
                {
                    await SeedUsersAsync();
                    //await SeedPollingStationAsync();
                    //await SeedRegistrationRequestsAsync();
                },
                () => Task.FromResult(true)
            );
        }

        private async Task SeedUsersAsync()
        {
            var users = GetMockUsers();

            foreach (var user in users)
            {
                if (!_context.Users.Any(u => u.UserName == user.Item1.UserName))
                    await SeedUserAsync(user.Item1, "Secret12", user.Item2);
            }

            await _context.SaveChangesAsync();
        }

        // Mock data for users

        public static IEnumerable<Tuple<UserDao, List<string>>> GetMockUsers()
        {
            return
            [
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "admin",
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@pcea.com",
                        PhoneNumber = "01 02 03 04 05",

                        AuthProvider = AuthProvider.Email,
                        //UserDistricts = [new() { DistrictId = 16 }],
                    },
                    new List<string> { AppConstants.SuperAdminRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user1",
                        FirstName = "Sam",
                        LastName = "Gamegie",
                        Email = "sam.gamegie@pcea.com",
                        PhoneNumber = "01 02 03 04 05",
                        //EmployeeNumber = "221157T",
                        AuthProvider = AuthProvider.Email,
                        //UserDistricts = [new() { DistrictId = 16 }],
                    },
                    new List<string> { AppConstants.StudentRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user2",
                        FirstName = "Bilbo",
                        LastName = "Baggins",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "bilbo.baggins@pcea.com",
                        AuthProvider = AuthProvider.Email,
                        //UserDistricts = [new() { DistrictId = 151 }],
                    },
                    new List<string> { AppConstants.StudentRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user3",
                        FirstName = "Éowyn",
                        LastName = "Shieldmaiden",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "eowyn.shieldmaiden@pcea.com",
                        AuthProvider = AuthProvider.Email,
                        //UserDistricts = [new() { DistrictId = 151 }],
                    },
                    new List<string> { AppConstants.StudentRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user4",
                        FirstName = "Éomer",
                        LastName = "RiderOfRohan",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "eomer.riderofrohan@pcea.com",
                        AuthProvider = AuthProvider.Email,
                        //UserDistricts = [new() { DistrictId = 16 }],
                    },
                    new List<string> { AppConstants.ParentRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user5",
                        FirstName = "Harvey",
                        LastName = "Spector",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "harvey.spector@pcea.com",
                        AuthProvider = AuthProvider.Email,
                        //UserDistricts = [new() { DistrictId = 152 }],
                    },
                    new List<string> { AppConstants.ParentRole }
                ),
            ];
        }
    }
}
