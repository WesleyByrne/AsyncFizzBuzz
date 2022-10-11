using System.Diagnostics;

namespace AsyncFizzBuzz
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //takes the number you want the fizzbuzz prog to go to as an argument
                //throws format exception if you dont pass it a number
                Program prog = new (long.Parse(args[0]));

                //timing the two method implementations for fun and sport
                //spoilers: theres no reason to do this with tasks and async
                var watch = Stopwatch.StartNew();
                await Task.Run(prog.SensibleFizzBuzz);
                watch.Stop();

                var watch2 = Stopwatch.StartNew();
                await Task.Run(prog.AsyncFizzBuzz);
                watch2.Stop();

                //if you want to print fizzbuzz to console
                //foreach (string result in prog.results)
                    //Console.Write("{0},   ",result);

                //if you want to write fizzbuzz results to a file for some reason
                File.WriteAllLines(Directory.GetCurrentDirectory()+"test.txt", prog.results);

                Console.WriteLine("Standard FizzBuzz implementation completed in {0} milliseconds", watch.ElapsedMilliseconds);
                Console.WriteLine("Async FizzBuzz implementation completed in {0} milliseconds", watch2.ElapsedMilliseconds);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{args[0]}'");
            }
        }

        public string[] results;
        public long size;
        Program(long size) //constructor call setting how far you are willing to take this
        {
            this.size = size;
            this.results = new string[size];
        }

        public void SensibleFizzBuzz() //Fizzbuzz if you're not overcomplicating things
        {
            this.results = new string[size]; //reset here for testing purposes 
            Console.WriteLine("Start Standard Fizzbuzz {0}", size);

            for (int i = 1; i <= size; i++)
            {
                if (i % 3 == 0 && i % 5 == 0)
                    results[i - 1] = "FizzBuzz";
                else if (i % 3 == 0)
                    results[i - 1] = "Fizz";
                else if (i % 5 == 0)
                    results[i - 1] = "Buzz";
                else
                    results[i - 1] = i.ToString();
            }
        }

        public async Task AsyncFizzBuzz() //Fizzbuzz if you are toying with async calls and dotnet API
        {
            this.results = new string[size]; //reset here for testing purposes 
            Console.WriteLine("Start Async Fizzbuzz {0}", size);
            
            Task fizz = Task.Run(Fizz);
            Task buzz = Task.Run(Buzz);

            //Method for filling in the numbers as a lambda
            Task nums = Task.Run( () =>
            {
                for (int i = 1; i <= size; i++)
                {
                    if (i % 3 != 0 && i % 5 != 0)
                        results[i - 1] = i.ToString();
                }
            });

            var fizzbuzz = new List<Task> { fizz, buzz, nums };

            while (fizzbuzz.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(fizzbuzz);
                if (finishedTask == fizz)
                {
                    Console.WriteLine("fizz dun");
                }
                else if (finishedTask == buzz)
                {
                    Console.WriteLine("buzz dun");
                }
                else
                    Console.WriteLine("nums dun");
                fizzbuzz.Remove(finishedTask);
            }
        }

        private void Fizz() //Method to call for assign fizzes in async
        {
            for (int i = 1; i <= size; i++)
            {
                if (i % 3 == 0)
                    if (results[i - 1] != null)
                        results[i - 1] = "Fizz" + results[i - 1];
                    else
                        results[i - 1] = "Fizz";
            }
        }

        private void Buzz()  //Method to call for assign buzzes in async
        {
            for (int i = 1; i <= size; i++)
            {
                if (i % 5 == 0)
                    if (results[i - 1] != null)
                        results[i - 1] = results[i - 1] + "Buzz";
                    else
                        results[i - 1] = "Buzz";
            }
        }

    }
}