using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ConsultaVagasPOA
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.WriteLine("How many pages?");
            var numberOfPages = Console.ReadLine();
            Console.WriteLine();


            // validate int
            int pages = 0;
            try
            {
                pages = int.Parse(numberOfPages);
            }
            catch (Exception)
            {
                throw;
            }

            List<ResultPage> allResults = PageQuery.QueryPageRange(pages);

            bool isFiltering = true;
            while (isFiltering)
            {

                Console.Clear();

                Console.WriteLine(allResults.Count.ToString() + " results found!");
                Console.WriteLine();

                Console.WriteLine("Looking for?");
                var textToSearch = Console.ReadLine();
                Console.WriteLine();

                List<ResultPage> filteredResults = new List<ResultPage>();

                for (int i = 0; i < allResults.Count; i++)
                {
                    if (allResults[i].title.ToUpper().Contains(textToSearch.ToUpper()))
                    {
                        filteredResults.Add(allResults[i]);
                    }
                }

                if (filteredResults.Count > 0)
                {
                    for (int i = 0; i < filteredResults.Count; i++)
                    {
                        Console.WriteLine(filteredResults[i].title);
                        Console.WriteLine(filteredResults[i].link);
                        Console.WriteLine();
                    }

                    if (WaitForYOrN("Open Links?"))
                    {
                        OpenLinks(filteredResults.Select(x => x.link).ToArray());
                    }

                    Console.WriteLine();

                    if (!WaitForYOrN("Filter Again?"))
                    {
                        isFiltering = false;
                    }
                }

                if (filteredResults.Count <= 0)
                {
                    Console.WriteLine("No Results Found");

                    if (!WaitForYOrN("Filter Again?"))
                    {
                        isFiltering = false;
                    }

                }
            }
            
        }

        private static void OpenLinks(string[] links)
        {
            for (int i = 0; i < links.Length; i++)
            {
                Process.Start(links[i]);
            }
            
        }


        private static bool WaitForYOrN(string question)
        {
            while (true)
            {
                Console.WriteLine(question + " (Y/N)");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        Console.WriteLine();
                        return true;
                    case ConsoleKey.N:
                        Console.WriteLine();
                        return false;
                    default:
                        Console.WriteLine();
                        break;
                }
            }
        }
    }


    class PageQuery
    {
        static string baseUrl = "https://www.vagaspoa.com.br/page/";
        static string queryClass = "entry-title";

        internal static List<ResultPage> QueryPageRange(int numberOfPages)
        {
            Console.Write("Reading...  ");

            int charactersWritten = 0;

            List<ResultPage> results = new List<ResultPage>();
            for (int i = 0; i < numberOfPages; i++)
            {
                results.AddRange(PageQuery.QueryPage(i + 1));

                // update console
                Console.SetCursorPosition(Console.CursorLeft - charactersWritten, Console.CursorTop);
                string textToWrite = (i+1).ToString() + "/" + numberOfPages.ToString();
                charactersWritten = textToWrite.Length;
                Console.Write(textToWrite);
            }
            return results;
        }

        private static List<ResultPage> QueryPage(int pageNumber)
        {
            var htmlweb = new HtmlWeb();
            var html = htmlweb.Load(baseUrl + pageNumber.ToString());
            var document = html.DocumentNode;

            var queried = document.QuerySelectorAll("." + queryClass);

            List<ResultPage> titles = new List<ResultPage>();
            foreach (var item in queried)
            {

                // query a
                var a = item.QuerySelectorAll("a");
                if (!a.Any())
                {
                    continue;
                }

                HtmlNode node = a.First();
                string title = node.InnerHtml;
                string link = node.Attributes["href"].Value;
                titles.Add(new ResultPage(link,title));
            }

            return titles;
        }
    }

    class ResultPage
    {
        public string link;
        public string title;

        public ResultPage(string link, string title)
        {
            this.link = link;
            this.title = title;
        }
    }

}
