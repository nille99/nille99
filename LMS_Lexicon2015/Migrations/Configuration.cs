namespace LMS_Lexicon2015.Migrations
{
    using LMS_Lexicon2015.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LMS_Lexicon2015.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LMS_Lexicon2015.Models.ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            foreach (string roles in new[] { "Elev", "L�rare" })
            {
                if (!context.Roles.Any(r => r.Name == roles))
                {
                    var role = new IdentityRole { Name = roles };
                    roleManager.Create(role);
                }
            }
            /////----------------------

            //Aktivitets typ // l�gg till activityTypes 
            var activityTypes = new[] {
                new ActivityType { Name = "F�rel�sning" },
                new ActivityType { Name = "E-learning" },
                new ActivityType { Name = "�vningsUppgift" },
                new ActivityType { Name = "Projekt" }
           };

            context.ActivityTypes.AddOrUpdate(at => at.Name, activityTypes);
            context.SaveChanges();
            /////----------------------
            //grupper
            var groups = new[] {
                new Group { Name = ".net Maj 2015", Description = "Text text text text", StartDate = new DateTime(2015,11,20), EndDate = new DateTime(2015,12,24)  },
                new Group { Name = ".net Sep 2015", Description = "Text text text text", StartDate = new DateTime(2015, 12, 20), EndDate = new DateTime(2016, 12, 18) },
                new Group { Name = ".net Feb 2016", Description = "Text text text text", StartDate = new DateTime(2016, 02, 28), EndDate = new DateTime(2016, 06, 16) }
            };

            context.Groups.AddOrUpdate(g => g.Name, groups);
            context.SaveChanges();
            /////----------------------

            //Nytt skapa anv�ndare med hash l�senord om det inte finns  
            var store = new UserStore<ApplicationUser>(context);
            var UserManager = new UserManager<ApplicationUser>(store);


            if (UserManager.FindByEmail("nisaw99@hotmail.com") == null)
            {
                var user = new ApplicationUser { UserName = "nisaw99@hotmail.com", Email = "nisaw99@hotmail.com", FirstName = "Kalle", LastName = "Anka", GroupId = groups[1].Id };
                UserManager.Create(user, "hej999");
                context.SaveChanges();
            }
            context.SaveChanges();

            var roleKeeper = UserManager.FindByEmail("nisaw99@hotmail.com");
            UserManager.AddToRole(roleKeeper.Id, "L�rare");  

            if (UserManager.FindByEmail("chrikro129@gmail.com") == null)
            {
                var user = new ApplicationUser { UserName = "chrikro129@gmail.com", Email = "chrikro129@gmail.com", FirstName = "Christina", LastName = "Kronblad", GroupId = groups[0].Id };
                UserManager.Create(user, "hej999");
            }
            context.SaveChanges();

            roleKeeper = UserManager.FindByEmail("chrikro129@gmail.com");
            UserManager.AddToRole(roleKeeper.Id, "Elev");

            /////----------------------
            //kurser
            var courses = new[] {
                new CourseOccasion { Name = "csharp", Description = "Text text text text", StartDate = new DateTime(2016, 02, 28), EndDate = new DateTime(2016, 06, 16), GroupId = groups[0].Id  },
                new CourseOccasion { Name = "Angular JS", Description = "Text text text text", StartDate = new DateTime(2016, 02, 27), EndDate = new DateTime(2016, 06, 16), GroupId = groups[0].Id  },
                new CourseOccasion { Name = "Test", Description = "Text text text text", StartDate = new DateTime(2016, 02, 26), EndDate = new DateTime(2016, 06, 17), GroupId = groups[1].Id  },
                new CourseOccasion { Name = "SQL", Description = "Text text text text", StartDate = new DateTime(2016, 02, 25), EndDate = new DateTime(2016, 06, 18), GroupId = groups[0].Id  },
                new CourseOccasion { Name = "Nuvarande", Description = "Hej Hopp", StartDate = new DateTime(2015, 12, 03), EndDate = new DateTime(2015, 12, 20), GroupId = groups[0].Id  }
            };

            context.CourseOccasions.AddOrUpdate(co => co.Name, courses);
            context.SaveChanges();

            /////----------------------
            //Aktiviteter 
                //obs end i seeden. Olika End-datum
            var activitys = new[] {
                new Activity{ Name = activityTypes[0].Name, Description = "text text", StartDate = new DateTime(2016,02,28), EndDate = new DateTime(2016,06,16), CourseId = courses[4].Id },
                new Activity{ Name = activityTypes[1].Name, Description = "text text", StartDate = new DateTime(2016,06,18), EndDate = new DateTime(2016,06,20), CourseId = courses[4].Id },
                new Activity{ Name = activityTypes[0].Name, Description = "text text", StartDate = new DateTime(2016,06,17), EndDate = new DateTime(2016,06,22), CourseId = courses[4].Id }
           };

            context.Activitys.AddOrUpdate(at => at.EndDate, activitys);
            context.SaveChanges();
        }
    }
}
