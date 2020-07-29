using System;
using System.Linq;

namespace FeatureTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int total = 0;
            int failed = 0;
            foreach (var testMethod in typeof(Program).Assembly
                .GetTypes().SelectMany(t => 
                    t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                        .Where(m => m.IsDefined(typeof (FactAttribute), false))))
            {
                total++;
                Console.WriteLine($"{testMethod.DeclaringType.Name}.{testMethod.Name}");
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
