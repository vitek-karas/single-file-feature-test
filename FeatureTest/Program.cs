using System;
using System.Linq;

namespace FeatureTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filter = args.Length > 0 ? args[0] : null;

            int total = 0;
            int failed = 0;
            foreach (var testMethod in typeof(Program).Assembly
                .GetTypes()
                    .OrderBy(t => t.Name)
                    .SelectMany(t => 
                        t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            .OrderBy(m => m.Name)
                            .Where(m => m.IsDefined(typeof (FactAttribute), false))))
            {
                string testName = $"{testMethod.DeclaringType.Name}.{testMethod.Name}";
                if (filter != null && !testName.Contains(filter))
                {
                    continue;
                }

                total++;
                Console.WriteLine(testName);
                try
                {
                    object testInstance = Activator.CreateInstance(testMethod.DeclaringType);
                    testMethod.Invoke(testInstance, new object[] { });
                    Console.WriteLine("  PASSED");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  FAILED: " + ex.ToString());
                    failed++;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"{total - failed}/{total} passed, {failed} failed.");
        }
    }
}
