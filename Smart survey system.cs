using System;
using System.Collections.Generic;

namespace SmartSurveySystem
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }
    

    public class UserProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Location { get; set; }
        public double MonthlyIncome { get; set; }

        public static UserProfile TakeUserInput()
        {
            UserProfile user = new UserProfile();

            Console.Write("Enter Name: ");
            while (true)
            {
                user.Name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(user.Name))
                    break;

                Console.Write("Name cannot be empty. Enter again: ");
            }

            Console.Write("Enter Age: ");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int age) && age > 0 && age < 120)
                {
                    user.Age = age;
                    break;
                }
                Console.Write("Invalid age. Enter again: ");
            }

            Console.Write("Enter Gender (Male/Female/Other): ");
            while (true)
            {
                if (Enum.TryParse(Console.ReadLine(), true, out Gender gender))
                {
                    user.Gender = gender;
                    break;
                }
                Console.Write("Invalid gender. Enter again: ");
            }

            Console.Write("Enter Location (optional): ");
            user.Location = Console.ReadLine();

            Console.Write("Enter Monthly Income: ");
            while (true)
            {
                if (double.TryParse(Console.ReadLine(), out double income) && income >= 0)
                {
                    user.MonthlyIncome = income;
                    break;
                }
                Console.Write("Invalid income. Enter again: ");
            }

            return user;
        }
    }

    public class SegmentEngine
    {
        public string GetAgeSegment(int age)
        {
            if (age < 18)
                return "Student";
            else if (age <= 60)
                return "Professional";
            else
                return "Retired";
        }

        public string GetIncomeSegment(double income)
        {
            if (income < 10000)
                return "Low Income";
            else if (income <= 100000)
                return "Mid Income";
            else
                return "High Income";
        }
    }

    public class SurveyManager
    {
        Dictionary<string, List<string>> questions = new Dictionary<string, List<string>>()
        {
            ["Student"] = new List<string>
            {
                "Do you use online learning platforms?",
                "What subject do you enjoy most?"
            },

            ["Professional"] = new List<string>
            {
                "Are you satisfied with your job?",
                "Do you work remotely?"
            },

            ["Retired"] = new List<string>
            {
                "Do you volunteer regularly?",
                "What hobbies keep you engaged?"
            }
        };

        public Dictionary<string, string> ConductSurvey(string ageSegment)
        {
            Dictionary<string, string> responses = new Dictionary<string, string>();

            Console.WriteLine($"\n--- Survey Questions for {ageSegment} ---");

            foreach (var question in questions[ageSegment])
            {
                Console.Write(question + " ");
                string answer = Console.ReadLine();
                responses[question] = answer;
            }

            return responses;
        }
    }

    public class UserRepository
    {
        List<(UserProfile, string, string, Dictionary<string, string>)> database
            = new List<(UserProfile, string, string, Dictionary<string, string>)>();

        public void SaveUserResponse(
            UserProfile user,
            string ageSegment,
            string incomeSegment,
            Dictionary<string, string> responses)
        {
            database.Add((user, ageSegment, incomeSegment, responses));
        }

        public void ShowSummary()
        {
            Console.WriteLine($"\nTotal Users Surveyed: {database.Count}");

            int students = 0, professionals = 0, retired = 0;

            foreach (var record in database)
            {
                if (record.Item2 == "Student") students++;
                else if (record.Item2 == "Professional") professionals++;
                else if (record.Item2 == "Retired") retired++;
            }

            Console.WriteLine($"Students: {students}");
            Console.WriteLine($"Professionals: {professionals}");
            Console.WriteLine($"Retired: {retired}");

            Console.WriteLine("\nDetailed Responses:\n");

            foreach (var record in database)
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine($"Name: {record.Item1.Name}");
                Console.WriteLine($"Age: {record.Item1.Age}");
                Console.WriteLine($"Gender: {record.Item1.Gender}");
                Console.WriteLine($"Age Group: {record.Item2}");
                Console.WriteLine($"Income Group: {record.Item3}");

                foreach (var qa in record.Item4)
                {
                    Console.WriteLine($"Q: {qa.Key}");
                    Console.WriteLine($"A: {qa.Value}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Welcome to Smart Survey System ===\n");

            UserRepository repository = new UserRepository();
            bool continueSurvey = true;

            while (continueSurvey)
            {
                UserProfile user = UserProfile.TakeUserInput();

                SegmentEngine segmentEngine = new SegmentEngine();
                string ageSegment = segmentEngine.GetAgeSegment(user.Age);
                string incomeSegment = segmentEngine.GetIncomeSegment(user.MonthlyIncome);

                Console.WriteLine($"\nUser Segments:");
                Console.WriteLine($"Age Group: {ageSegment}");
                Console.WriteLine($"Income Group: {incomeSegment}\n");

                SurveyManager survey = new SurveyManager();
                var responses = survey.ConductSurvey(ageSegment);

                repository.SaveUserResponse(user, ageSegment, incomeSegment, responses);

                Console.Write("\nDo you want to survey another user? (yes/no): ");
                string choice = Console.ReadLine().ToLower();

                continueSurvey = (choice == "yes");
            }

            Console.WriteLine("\n=== Survey Summary Report ===");
            repository.ShowSummary();

            Console.WriteLine("\nThank you for using Smart Survey System!");
        }
    }
}