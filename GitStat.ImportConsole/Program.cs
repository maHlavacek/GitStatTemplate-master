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
                var commits = unitOfWork.CommitRepository.GetAllCommitsFromLastWeeks(4);
                foreach (var item in commits)
                {
                    Console.WriteLine($"{item.Developer.Name} {item.Date} {item.FilesChanges} {item.Insertions} {item.Deletions}");
                }
                Console.WriteLine();
                Console.WriteLine("Commit mit ID 4");
                Console.WriteLine("---------------");

                var commitWithID = unitOfWork.CommitRepository.GetCommitWithId(4);
                Console.WriteLine($"{commitWithID.Developer.Name} {commitWithID.Date} {commitWithID.FilesChanges} {commitWithID.Insertions} {commitWithID.Deletions}");

                Console.WriteLine("Statistic der Commits der Developer");
                Console.WriteLine("-----------------------------------");
                var statistic = unitOfWork.DeveloperRepository.GetStatisticFromDevelopers();
                foreach (var item in statistic)
                {
                    Console.WriteLine($"{item.DeveloperName} {item.Commits} {item.FilesChanged} {item.Insertions} {item.Deletions}");
                }
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

    }
}
