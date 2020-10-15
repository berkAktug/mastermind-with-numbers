using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace mastermind_with_numbers
{
    class Program
    {
        static List<string> answers;
        static int answerSize;
        static int bulls;
        static int cows;
        static string guess;
        static IEnumerable<string> Permutations(int size, string str)
        {
            if (size > 0)
            {
                foreach (string s in Permutations(size - 1, str))
                    foreach (char n in str)
                        if (!s.Contains(n))
                            yield return s + n;
            }
            else
                yield return "";
        }

        static IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable)
        {
            var r = new Random();
            return enumerable.OrderBy(x => r.Next()).ToList();
        }

        static IEnumerable<T> Shuffle_Parallel<T>(IEnumerable<T> enumerable)
        {
            var r = new Random();
            return enumerable.AsParallel().OrderBy(x => r.Next()).ToList();
        }

        static bool ReadBullsCows(out int bulls, out int cows)
        {
            string[] input = Console.ReadLine().Split(' ').ToArray();
            bulls = cows = 0;
            if (input.Length < 2)
                return false;
            else if (input[0].Length > 1 || input[1].Length > 1)
                return false;
            else
                return int.TryParse(input[0], out bulls)
                    && int.TryParse(input[1], out cows);
        }
        static int Main(string[] args)
        {
            string[] line = Console.ReadLine().Split(' ').ToArray();

            int.TryParse(line[0], out int upperLimit);
            int.TryParse(line[1], out answerSize);
            int.TryParse(line[2], out int maxTurn);
            long.TryParse(line[3], out long maxTime);

            answers = new List<string>();

            string str = "";
            for (int i = 0; i <= upperLimit; i++)
            {
                str += i;
            }

            answers = Permutations(answerSize, str).ToList();

            answers.RemoveAll(x => x.StartsWith("0"));
            int initialCount = answers.Count;
            //if (answerSize >= 8 && upperLimit >= 8)
            //{
            //    answers = Shuffle_Parallel(answers).ToList();
            //}
            //else
            //{
                answers = Shuffle(answers).ToList();
            //}

            Thread.CurrentThread.Name = "Main";
            Stopwatch sw = new Stopwatch();

            for (int turnCount = 0; answers.Count > 1; turnCount++)
            {
                if (turnCount > maxTurn)
                {
                    Console.WriteLine("Turn limit exceeded.");
                    System.Environment.Exit(1);
                }

                guess = answers[0];
                Console.Write(guess + " ");

                if (!ReadBullsCows(out bulls, out cows))
                {
                    Console.WriteLine("Please check your inputs.");
                    turnCount--;
                }
                else
                {
                    sw.Restart();
                    ProcessWithThreadPoolMethod();
                    sw.Stop();
                    if(sw.ElapsedMilliseconds > maxTime)
                    {
                        Console.WriteLine("Time limit exceeded.");
                        System.Environment.Exit(1);
                    }

                    if (bulls == answerSize)
                    {
                        return 0;
                    }
                }
            }
            if (answers.Count == 1)
            {
                Console.WriteLine("Hooray! The answer is {0}!", answers[0]);
                return 0;
            }
            else
                Console.WriteLine("No possible answer fits the scores you gave.");
            return 0;
        }

        static void ProcessWithThreadPoolMethod()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(AlgorithmLoop));
        }

        static void AlgorithmLoop(object callback)
        {
            for (int a = 0; a < answers.Count - 1; a++)
            {
                int tmpBulls = 0, tmpCows = 0;
                for (int ix = 0; ix < answerSize; ix++)
                    if (answers[a][ix] == guess[ix])
                        tmpBulls++;
                    else if (answers[a].Contains(guess[ix]))
                        tmpCows++;
                if ((tmpBulls != bulls) || (tmpCows != cows))
                    answers.RemoveAt(a);
            }
        }

        static void AlgorithmLoop()
        {
            for (int a = 0; a < answers.Count - 1; a++)
            {
                int tmpBulls = 0, tmpCows = 0;
                for (int ix = 0; ix < answerSize; ix++)
                    if (answers[a][ix] == guess[ix])
                        tmpBulls++;
                    else if (answers[a].Contains(guess[ix]))
                        tmpCows++;
                if ((tmpBulls != bulls) || (tmpCows != cows))
                    answers.RemoveAt(a);
            }
        }
    }
}
