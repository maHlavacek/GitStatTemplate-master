using System;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Contracts;
using GitStat.Persistence;

namespace GitStat.ImportConsole
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Import der Commits in die Datenbank");
            using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWorkImport.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWorkImport.MigrateDatabase();
                Console.WriteLine("Commits werden von commits.txt eingelesen");
                var commits = ImportController.ReadFromCsv();
                if (commits.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Commits eingelesen");
                    return;
                }
                Console.WriteLine(
                    $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
                unitOfWorkImport.CommitRepository.AddRange(commits);
                int countDevelopers = commits.GroupBy(c => c.Developer).Count();
                int savedRows = unitOfWorkImport.SaveChanges();
                Console.WriteLine(
                    $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
                Console.WriteLine();
                var csvCommits = commits.Select(c =>
                    $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
                File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);
            }
            Console.WriteLine("Datenbankabfragen");
            Console.WriteLine("=================");
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                Console.WriteLine("Commits der letzten 4 Wochen");
                Console.WriteLine("----------------------------");
                Console.WriteLine($"{"Developer",-20} {"Date",-10} {"FilesChanged",-12} {"Insertions",-10} {"Deletions",-9}");

                var commits = unitOfWork.CommitRepository.GetAllCommitsFromLastWeeks(4);
                foreach (var item in commits)
                {
                    Console.WriteLine($"{item.Developer.Name,-20} {item.Date.ToShortDateString(),10} {item.FilesChanges,12} {item.Insertions,10} {item.Deletions,9}");
                }
                Console.WriteLine();
                Console.WriteLine("Commit mit ID 4");
                Console.WriteLine("---------------");
                Console.WriteLine($"{"Developer",-20} {"Date",-10} {"FilesChanged",-12} {"Insertions",-10} {"Deletions",-9}");

                var commitWithID = unitOfWork.CommitRepository.GetCommitWithId(4);
                Console.WriteLine($"{commitWithID.Developer.Name,-20} {commitWithID.Date.ToShortDateString(),10} {commitWithID.FilesChanges,12} {commitWithID.Insertions,10} {commitWithID.Deletions,9}");
                Console.WriteLine();
                Console.WriteLine("Statistic der Commits der Developer");
                Console.WriteLine("-----------------------------------");
                var statistic = unitOfWork.DeveloperRepository.GetStatisticFromDevelopers();
                Console.WriteLine($"{"Developer",-20} {"Commits",-7} {"FilesChanged",-12} {"Insertions",-10} {"Deletions",-9}");
                foreach (var item in statistic)
                {
                    Console.WriteLine($"{item.DeveloperName,-20} {item.Commits,7} {item.FilesChanged,12} {item.Insertions,10} {item.Deletions,9}");
                }
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

    }
}
