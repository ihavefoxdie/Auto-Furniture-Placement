using Execution;
using System.ComponentModel.Design;

TestingClass testingClass = new TestingClass();

Console.Write("Type ONLY either of numbers.\n\n1. Genetic testing;\n2. Annealing testing.\n\nEnter corresponding number: ");
switch (Console.ReadLine())
{
    case "1":
        testingClass.GeneticTesting();
        break;
    case "2":
        testingClass.AnnealingTesting();
        break;
    default:
        Console.WriteLine("No such option.");
        break;
}


