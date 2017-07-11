using System;
using System.IO;
using System.Linq;
using System.Text;
using Bellona.Analysis.Clustering;

namespace AnpanatorCluster
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("コマンドライン引数でファイルを指定してね。");
                return;
            }

            var path = args[0];
            if (!File.Exists(path))
            {
                Console.WriteLine("ファイル {0} はないよ。", path);
                return;
            }

            Cluster(path);
        }

        private static void Cluster(string path)
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            var csv = lines.Select(line => line.Split(',')).ToArray();

            var questions = csv[0].Skip(1).ToArray();
            var members = csv
                .Skip(1)
                .Select(row => new Member()
                {
                    Name = row[0],
                    Scores = row.Skip(1).Select(cell => cell.Length > 0 ? double.Parse(cell) : 0.0).ToArray(),
                })
                .ToArray();

            var valuableQuestionIndecies = Enumerable.Range(0, questions.Length)
                .OrderByDescending(i => members.Select(member => member.Scores[i]).Count(score => score != 0.0))
                .Take(5)
                .ToArray();

            var startTime = DateTime.Now;

            var model = ClusteringModel.CreateAuto<Member>(member => valuableQuestionIndecies.Select(i => member.Scores[i]).ToArray()).Train(members);

            var endTime = DateTime.Now;

            foreach (var cluster in model.Clusters)
            {
                Console.WriteLine("## {0} ##", cluster.Id);
                foreach (var record in cluster.Records)
                {
                    Console.WriteLine("- {0}", record.Element.Name);
                }
            }

            Console.WriteLine("経過時間：{0}", endTime - startTime);
        }
    }
}
