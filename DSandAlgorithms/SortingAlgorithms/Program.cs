using SortingAlgorithms.Sorting_Services;

namespace SortingAlgorithms
{
    internal class Program
    {
        static void Main(string[] args)
        {

            #region Selection Sort
            //NUMBERs
            //int[] numbers = { -11, 12, -42, 0, 1, 90, 68, 6, -9 };
            //SelectionSort.Sort(numbers);
            //Console.WriteLine(string.Join(',', numbers));

            //STRINGS
            //string[] strings = { "Badr", "Saeed", "Ali", "Mohammed", "Hassan" };
            //SelectionSort.Sort(strings);
            //Console.WriteLine(string.Join(',', strings)); 

            //Custom Object
            //Employee[] emplyees =
            //{
            //    new Employee(1,"Badr", 15000),
            //    new Employee(2,"Saeed",  1500),
            //    new Employee(3,"Ali", 5000),
            //    new Employee(4,"Mohamed",1000),
            //};

            //SelectionSort.Sort(emplyees);

            //foreach (Employee emp in emplyees)
            //{
            //    Console.WriteLine(emp.ToString());
            //}
            #endregion

            int n = Convert.ToInt32(Console.ReadLine());
            double[] numbers = new double[n];
            string[] numbers2 = Console.ReadLine().Split(" ");

            for (int i = 0; i < n; i++)
            {
                numbers[i] = Convert.ToDouble(numbers2[i]);
            }
            int target = Convert.ToInt32(Console.ReadLine());


            Console.WriteLine(Merge(numbers, target));



        }

        public static int Merge(double[] nums, int target)
        {
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] == target) 
                    return i;
            }
            return -1;
        }
    }
}