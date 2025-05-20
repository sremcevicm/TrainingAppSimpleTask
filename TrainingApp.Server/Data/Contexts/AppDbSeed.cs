using TrainingApp.Server.Data.Models;
using TrainingApp.Server.Helpers;

namespace TrainingApp.Server.Data.Contexts
{
    public static class AppDbSeed
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Trainers.Any())
            {
                context.Trainers.AddRange(
                    new Trainer { 
                        Name = "Petar Petrovic", 
                        Email = "petar@example.com", 
                        PhoneNumber = "066112233",
                        AccessCode = SecurityHelper.HashAccessCode("qwer1234"), 
                        CancellationNoticeInHours = 24
                    },
                    new Trainer { 
                        Name = "Iva Ivivc", 
                        Email = "iva@example.com", 
                        PhoneNumber = "066445566", 
                        AccessCode = SecurityHelper.HashAccessCode("tyui5678"),
                        CancellationNoticeInHours = 12
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
