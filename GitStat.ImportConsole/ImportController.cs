using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using Utils;

namespace GitStat.ImportConsole
{
    public class ImportController
    {
        const string Filename = "commits.txt";
        static ICollection<string[]> _blocks;
        static ICollection<Developer> _developers; 


        /// <summary>
        /// Reads the txt file and return the commits as an array
        /// </summary>
        public static Commit[] ReadFromCsv()
        {

            string path = MyFile.GetFullNameInApplicationTree(Filename);
            string[] lines = File.ReadAllLines(path, Encoding.Default);

            _developers = new List<Developer>();
            _blocks = new List<string[]>();
            int range = 0;
            while(range < lines.Length)
            {
                var block = lines.Skip(range).TakeWhile(tw => !string.IsNullOrEmpty(tw)).ToArray();
                range += block.Count() + 1;
                _blocks.Add(block);
            }
            List<Commit> commits = new List<Commit>();

            foreach (string[] block in _blocks)
            {
                commits.AddRange(CreateCommitsFromBlock(block));
            }
            return commits.ToArray();
        }
        
        /// <summary>
        /// Split the current text block into a header and a footer string and creates a List<Commit>
        /// </summary>
        /// <param name="block"></param>
        /// <returns>List<Commit></returns>
        private static IEnumerable<Commit> CreateCommitsFromBlock(string[] block)
        {
            List<Commit> commits = new List<Commit>();
            int idx = 0;

            while (!block[idx + 1].StartsWith(' '))
            {
                string[] singleLineCommitHeader = block[idx].Split(',');
               
                Commit singleLineCommit = CreateCommit(singleLineCommitHeader);
                commits.Add(singleLineCommit);
                Developer singleLineDeveloper = GetDeveloper(singleLineCommit.Developer.Name);
                singleLineDeveloper.Commits.Add(singleLineCommit);
                idx++;
            }

            string[] header = block[idx].Split(',');
            string[] footer = block[block.Length - 1].Split(',');
            
            Commit commit = CreateCommit(header, footer);
            commits.Add(commit);
            Developer developer = GetDeveloper(commit.Developer.Name);
            developer.Commits.Add(commit);
            return commits;
        }

        /// <summary>
        /// Create a Commit 
        /// if the footer is null then the insertions, deletions, files changed will be 0
        /// </summary>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        /// <returns>Commit</returns>
        private static Commit CreateCommit(string[] header, string[] footer = null)
        {
            string hashCode = header[0];
            string developerName = header[1];
            DateTime date = DateTime.Parse(header[2]);
            string message = header[3];
            if (header.Length > 4)
            {
                for (int i = 4; i < header.Length; i++)
                {
                    message += ", " + header[i];
                }
            }

            Commit commit = new Commit
            {
                HashCode = hashCode,
                Date = date,
                Developer = GetDeveloper(developerName),
                Message = message,
                FilesChanges = GetFileChanged(footer),
                Insertions = GetInsertion(footer),
                Deletions = GetDeletion(footer)
            };
            return commit;
        }
        /// <summary>
        /// return the developer if exists
        /// create a new developer if it doesn't exist in the developers collection and return them
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Developer</returns>
        private static Developer GetDeveloper(string name)
        {
            if (_developers.Where(s => s.Name == name).FirstOrDefault() == null)
            {
                _developers.Add(new Developer { Name = name });
            }
            return _developers.Where(d => d.Name == name).FirstOrDefault();
        }
        /// <summary>
        /// search for the insertion part of the string[] and return its value
        /// return 0 when the part doesn't exists
        /// </summary>
        /// <param name="text"></param>
        /// <returns>int</returns>
        private static int GetInsertion(string[] text)
        {
            int insertions = 0;
            if (text == null)
                return insertions;
            string insertiosString = text.Where(txt => txt.Contains("insertion")).FirstOrDefault();
            if(!string.IsNullOrEmpty(insertiosString))
            {
                string[] result = insertiosString.Split(' ');
                insertions = int.Parse(result[1]);
            }
            return insertions;           
        }
        /// <summary>
        /// search for the deletion part of the string[] and return its value
        /// return 0 when the part doesn't exists
        /// </summary>
        /// <param name="text"></param>
        /// <returns>int</returns>
        private static int GetDeletion(string[] text)
        {
            int deletions = 0;
            if (text == null)
                return deletions;
            string deletionString = text.Where(txt => txt.Contains("deletion")).FirstOrDefault();
            if (!string.IsNullOrEmpty(deletionString))
            {
                string[] result = deletionString.Split(' ');
                deletions = int.Parse(result[1]);
            }
            return deletions;
        }
        /// <summary>
        /// search for the file changed part of the string[] and return its value
        /// return 0 when the part doesn't exists
        /// </summary>
        /// <param name="text"></param>
        /// <returns>int</returns>
        private static int GetFileChanged(string [] text)
        {
            int filesChanged = 0;
            if (text == null)
                return filesChanged;

            string filesChangedString = text.Where(txt => txt.Contains("changed")).FirstOrDefault();
            if (!string.IsNullOrEmpty(filesChangedString))
            {
                string[] result = filesChangedString.Split(' ');
                filesChanged = int.Parse(result[1]);
            }
            return filesChanged;
        }
    }
}
