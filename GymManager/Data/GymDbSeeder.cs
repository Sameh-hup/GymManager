using GymManager.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Data;

public static class GymDbSeeder
{
    public static async Task SeedAsync(GymDbContext context)
    {
        // Ensure database is created/migrated
        await context.Database.EnsureCreatedAsync();

        // If data already exists, we skip unless empty
        if (await context.Members.AnyAsync())
        {
            return;
        }

        var today = DateTime.Today;

        // ==========================================
        // 1. Membership Plans
        // ==========================================
        var plan1Month = new MembershipPlan
        {
            Name = "1-Month Standard",
            DurationInDays = 30,
            Price = 50.00m,
            Description = "Full access to gym equipment, locker rooms, and general floor.",
            IsActive = true
        };

        var plan3Months = new MembershipPlan
        {
            Name = "3-Months Pro",
            DurationInDays = 90,
            Price = 135.00m,
            Description = "Gym access + 2 free personal training consultation sessions.",
            IsActive = true
        };

        var plan6Months = new MembershipPlan
        {
            Name = "6-Months Fitness Plus",
            DurationInDays = 180,
            Price = 250.00m,
            Description = "Full gym access, sauna, locker, and 1 guest pass per month.",
            IsActive = true
        };

        var plan1YearVip = new MembershipPlan
        {
            Name = "1-Year VIP All-Access",
            DurationInDays = 365,
            Price = 450.00m,
            Description = "Unlimited 24/7 access, VIP locker, spa, personal training & nutrition plan.",
            IsActive = true
        };

        var planStudent = new MembershipPlan
        {
            Name = "Student Monthly",
            DurationInDays = 30,
            Price = 35.00m,
            Description = "Special discount package with valid student ID.",
            IsActive = true
        };

        var planWeekend = new MembershipPlan
        {
            Name = "Weekend Warrior",
            DurationInDays = 30,
            Price = 25.00m,
            Description = "Friday and Saturday access only.",
            IsActive = false
        };

        await context.MembershipPlans.AddRangeAsync(plan1Month, plan3Months, plan6Months, plan1YearVip, planStudent, planWeekend);
        await context.SaveChangesAsync();

        // ==========================================
        // 2. Members
        // ==========================================
        var m1 = new Member { FullName = "Sameh Mostafa", Phone = "01554813321", RegistrationDate = today.AddDays(-60), Notes = "Goal: Hypertrophy & Strength Training. Locker #12." };
        var m2 = new Member { FullName = "Ahmed Hassan", Phone = "01012345678", RegistrationDate = today.AddDays(-45), Notes = "Cardio focus, weight management." };
        var m3 = new Member { FullName = "Omar Khaled", Phone = "01198765432", RegistrationDate = today.AddDays(-30), Notes = "Competitive powerlifter." };
        var m4 = new Member { FullName = "Mahmoud Adel", Phone = "01234567890", RegistrationDate = today.AddDays(-20), Notes = "Knee surgery history, avoid heavy leg press." };
        var m5 = new Member { FullName = "Said Abdelrahman", Phone = "01112222233", RegistrationDate = today.AddDays(-75), Notes = "Long-time regular member." };
        var m6 = new Member { FullName = "Tarek Ali", Phone = "01099887766", RegistrationDate = today.AddDays(-15), Notes = "Morning workouts before 9 AM." };
        var m7 = new Member { FullName = "Sarah Mansour", Phone = "01211223344", RegistrationDate = today.AddDays(-10), Notes = "Fitness classes and HIIT." };
        var m8 = new Member { FullName = "Mariam Kamal", Phone = "01544332211", RegistrationDate = today.AddDays(-5), Notes = "New signup, needs gym orientation." };
        var m9 = new Member { FullName = "Youssef Ibrahim", Phone = "01066554433", RegistrationDate = today.AddDays(-2), Notes = "Student subscription." };
        var m10 = new Member { FullName = "Karim Farouk", Phone = "01144556677", RegistrationDate = today.AddDays(-80), Notes = "Prefers evening sessions." };

        await context.Members.AddRangeAsync(m1, m2, m3, m4, m5, m6, m7, m8, m9, m10);
        await context.SaveChangesAsync();

        // ==========================================
        // 3. Memberships (Active, Expiring Soon, Expired)
        // ==========================================
        var memberships = new List<Membership>
        {
            // Sameh: Active 1-Year VIP (Fully Paid)
            new Membership
            {
                MemberId = m1.Id,
                MembershipPlanId = plan1YearVip.Id,
                StartDate = today.AddDays(-30),
                EndDate = today.AddDays(335),
                PriceAtPurchase = plan1YearVip.Price
            },

            // Ahmed: Active 3-Months (Partial Paid)
            new Membership
            {
                MemberId = m2.Id,
                MembershipPlanId = plan3Months.Id,
                StartDate = today.AddDays(-20),
                EndDate = today.AddDays(70),
                PriceAtPurchase = plan3Months.Price
            },

            // Omar: Expiring Soon! (Ends in 3 days)
            new Membership
            {
                MemberId = m3.Id,
                MembershipPlanId = plan1Month.Id,
                StartDate = today.AddDays(-27),
                EndDate = today.AddDays(3),
                PriceAtPurchase = plan1Month.Price
            },

            // Mahmoud: Expiring Soon! (Ends in 5 days)
            new Membership
            {
                MemberId = m4.Id,
                MembershipPlanId = plan1Month.Id,
                StartDate = today.AddDays(-25),
                EndDate = today.AddDays(5),
                PriceAtPurchase = plan1Month.Price
            },

            // Said - Old Expired membership from 2 months ago
            new Membership
            {
                MemberId = m5.Id,
                MembershipPlanId = plan1Month.Id,
                StartDate = today.AddDays(-75),
                EndDate = today.AddDays(-45),
                PriceAtPurchase = plan1Month.Price
            },

            // Said - New Active Renewed 6-Months membership (Fully Paid)
            new Membership
            {
                MemberId = m5.Id,
                MembershipPlanId = plan6Months.Id,
                StartDate = today.AddDays(-10),
                EndDate = today.AddDays(170),
                PriceAtPurchase = plan6Months.Price
            },

            // Tarek: Active 1-Month (Unpaid / Balance Due)
            new Membership
            {
                MemberId = m6.Id,
                MembershipPlanId = plan1Month.Id,
                StartDate = today.AddDays(-5),
                EndDate = today.AddDays(25),
                PriceAtPurchase = plan1Month.Price
            },

            // Sarah: Active 3-Months (Fully Paid)
            new Membership
            {
                MemberId = m7.Id,
                MembershipPlanId = plan3Months.Id,
                StartDate = today.AddDays(-8),
                EndDate = today.AddDays(82),
                PriceAtPurchase = plan3Months.Price
            },

            // Mariam: Active 1-Month (Partial Paid)
            new Membership
            {
                MemberId = m8.Id,
                MembershipPlanId = plan1Month.Id,
                StartDate = today.AddDays(-3),
                EndDate = today.AddDays(27),
                PriceAtPurchase = plan1Month.Price
            },

            // Youssef: Active Student Plan (Fully Paid Today)
            new Membership
            {
                MemberId = m9.Id,
                MembershipPlanId = planStudent.Id,
                StartDate = today,
                EndDate = today.AddDays(30),
                PriceAtPurchase = planStudent.Price
            },

            // Karim: Expired (ended 10 days ago)
            new Membership
            {
                MemberId = m10.Id,
                MembershipPlanId = plan1Month.Id,
                StartDate = today.AddDays(-40),
                EndDate = today.AddDays(-10),
                PriceAtPurchase = plan1Month.Price
            }
        };

        await context.Memberships.AddRangeAsync(memberships);
        await context.SaveChangesAsync();

        // ==========================================
        // 4. Payments (Across recent days & today)
        // ==========================================
        var payments = new List<Payment>
        {
            // Sameh: 1-Year VIP ($450) -> Paid in 2 installments
            new Payment { MembershipId = memberships[0].Id, Amount = 250.00m, PaymentDate = today.AddDays(-30).AddHours(11), PaymentMethod = "Bank Transfer", Notes = "First installment via Instapay" },
            new Payment { MembershipId = memberships[0].Id, Amount = 200.00m, PaymentDate = today.AddDays(-15).AddHours(14), PaymentMethod = "Credit Card", Notes = "Final installment" },

            // Ahmed: 3-Months ($135) -> Paid $80, Remaining $55
            new Payment { MembershipId = memberships[1].Id, Amount = 80.00m, PaymentDate = today.AddDays(-20).AddHours(17), PaymentMethod = "Cash", Notes = "Deposit paid, balance due next week" },

            // Omar: 1-Month ($50) -> Fully Paid
            new Payment { MembershipId = memberships[2].Id, Amount = 50.00m, PaymentDate = today.AddDays(-27).AddHours(18), PaymentMethod = "Cash", Notes = "Full monthly fee" },

            // Mahmoud: 1-Month ($50) -> Fully Paid
            new Payment { MembershipId = memberships[3].Id, Amount = 50.00m, PaymentDate = today.AddDays(-25).AddHours(10), PaymentMethod = "Debit Card", Notes = "Paid at POS terminal" },

            // Said: Old expired ($50) -> Fully Paid
            new Payment { MembershipId = memberships[4].Id, Amount = 50.00m, PaymentDate = today.AddDays(-75).AddHours(12), PaymentMethod = "Cash", Notes = "Cash receipt #0012" },

            // Said: New 6-Months ($250) -> Fully Paid ($100 last week + $150 yesterday)
            new Payment { MembershipId = memberships[5].Id, Amount = 100.00m, PaymentDate = today.AddDays(-6).AddHours(16), PaymentMethod = "Cash", Notes = "Renewal advance" },
            new Payment { MembershipId = memberships[5].Id, Amount = 150.00m, PaymentDate = today.AddDays(-1).AddHours(19), PaymentMethod = "Credit Card", Notes = "Renewal completion" },

            // Sarah: 3-Months ($135) -> Fully Paid 4 days ago
            new Payment { MembershipId = memberships[7].Id, Amount = 135.00m, PaymentDate = today.AddDays(-4).AddHours(13), PaymentMethod = "Credit Card", Notes = "Full payment" },

            // Mariam: 1-Month ($50) -> Paid $30, Remaining $20
            new Payment { MembershipId = memberships[8].Id, Amount = 30.00m, PaymentDate = today.AddDays(-2).AddHours(15), PaymentMethod = "Cash", Notes = "Partial payment" },

            // Youssef: Student Plan ($35) -> Paid TODAY!
            new Payment { MembershipId = memberships[9].Id, Amount = 35.00m, PaymentDate = today.AddHours(9).AddMinutes(30), PaymentMethod = "Cash", Notes = "Today morning new student signup" }
        };

        await context.Payments.AddRangeAsync(payments);
        await context.SaveChangesAsync();

        // ==========================================
        // 5. Attendance (Today & 7-Day History)
        // ==========================================
        var attendances = new List<Attendance>
        {
            // Today's Check-ins
            new Attendance { MemberId = m1.Id, CheckInDateTime = today.AddHours(7).AddMinutes(45), Notes = "Morning chest & triceps session" },
            new Attendance { MemberId = m5.Id, CheckInDateTime = today.AddHours(8).AddMinutes(15), Notes = "Cardio & stretching" },
            new Attendance { MemberId = m2.Id, CheckInDateTime = today.AddHours(9).AddMinutes(00), Notes = "Personal training consultation" },
            new Attendance { MemberId = m9.Id, CheckInDateTime = today.AddHours(10).AddMinutes(30), Notes = "First workout after student registration" },
            new Attendance { MemberId = m7.Id, CheckInDateTime = today.AddHours(11).AddMinutes(15), Notes = "HIIT Class" },

            // Yesterday Check-ins
            new Attendance { MemberId = m1.Id, CheckInDateTime = today.AddDays(-1).AddHours(8).AddMinutes(00), Notes = "Back & biceps" },
            new Attendance { MemberId = m3.Id, CheckInDateTime = today.AddDays(-1).AddHours(16).AddMinutes(30), Notes = "Deadlift heavy day" },
            new Attendance { MemberId = m4.Id, CheckInDateTime = today.AddDays(-1).AddHours(18).AddMinutes(10), Notes = "Upper body light" },
            new Attendance { MemberId = m5.Id, CheckInDateTime = today.AddDays(-1).AddHours(19).AddMinutes(45), Notes = "Evening workout" },

            // Past 2 to 6 days Check-ins
            new Attendance { MemberId = m1.Id, CheckInDateTime = today.AddDays(-2).AddHours(8), Notes = "Legs workout" },
            new Attendance { MemberId = m2.Id, CheckInDateTime = today.AddDays(-2).AddHours(17), Notes = "Cardio" },
            new Attendance { MemberId = m6.Id, CheckInDateTime = today.AddDays(-2).AddHours(7), Notes = "Shoulders" },

            new Attendance { MemberId = m3.Id, CheckInDateTime = today.AddDays(-3).AddHours(15), Notes = "Bench press day" },
            new Attendance { MemberId = m7.Id, CheckInDateTime = today.AddDays(-3).AddHours(11), Notes = "Yoga & Pilates" },
            new Attendance { MemberId = m5.Id, CheckInDateTime = today.AddDays(-3).AddHours(18), Notes = "Full body" },

            new Attendance { MemberId = m1.Id, CheckInDateTime = today.AddDays(-4).AddHours(8), Notes = "Arms & Abs" },
            new Attendance { MemberId = m4.Id, CheckInDateTime = today.AddDays(-4).AddHours(17), Notes = "Cardio recovery" },

            new Attendance { MemberId = m2.Id, CheckInDateTime = today.AddDays(-5).AddHours(18), Notes = "Core training" },
            new Attendance { MemberId = m6.Id, CheckInDateTime = today.AddDays(-5).AddHours(7), Notes = "Full body circuit" },

            new Attendance { MemberId = m5.Id, CheckInDateTime = today.AddDays(-6).AddHours(19), Notes = "Weekend session" }
        };

        await context.Attendances.AddRangeAsync(attendances);
        await context.SaveChangesAsync();

        // ==========================================
        // 6. Day Passes (Today & Recent Days)
        // ==========================================
        var dayPasses = new List<DayPass>
        {
            // Today's Day Passes
            new DayPass { VisitorName = "Hossam Nader", VisitorPhone = "01055566677", VisitDateTime = today.AddHours(8).AddMinutes(20), Fee = 15.00m, PaymentMethod = "Cash", Notes = "Guest of Sameh Mostafa" },
            new DayPass { VisitorName = "Amr Diab", VisitorPhone = "01122334455", VisitDateTime = today.AddHours(10).AddMinutes(45), Fee = 20.00m, PaymentMethod = "Credit Card", Notes = "Visiting from Alexandria" },
            new DayPass { VisitorName = "Guest Visitor", VisitorPhone = null, VisitDateTime = today.AddHours(11).AddMinutes(30), Fee = 15.00m, PaymentMethod = "Cash", Notes = "One-time trial session" },

            // Yesterday
            new DayPass { VisitorName = "Ziad El-Sayed", VisitorPhone = "01288776655", VisitDateTime = today.AddDays(-1).AddHours(16).AddMinutes(10), Fee = 20.00m, PaymentMethod = "Credit Card", Notes = "Evening pass" },
            new DayPass { VisitorName = "Mostafa Kamel", VisitorPhone = "01599887744", VisitDateTime = today.AddDays(-1).AddHours(18).AddMinutes(00), Fee = 15.00m, PaymentMethod = "Cash", Notes = "Locker key #25" },

            // Previous days
            new DayPass { VisitorName = "Ali Mansour", VisitorPhone = "01033445566", VisitDateTime = today.AddDays(-2).AddHours(14), Fee = 20.00m, PaymentMethod = "Debit Card", Notes = "Hotel guest" },
            new DayPass { VisitorName = "Kareem Saad", VisitorPhone = "01144332211", VisitDateTime = today.AddDays(-3).AddHours(17), Fee = 15.00m, PaymentMethod = "Cash", Notes = "Trial workout" },
            new DayPass { VisitorName = "Fares Nour", VisitorPhone = "01255667788", VisitDateTime = today.AddDays(-5).AddHours(19), Fee = 20.00m, PaymentMethod = "Cash", Notes = "Weekend visitor" }
        };

        await context.DayPasses.AddRangeAsync(dayPasses);
        await context.SaveChangesAsync();
    }
}
