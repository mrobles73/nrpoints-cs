using HtmlAgilityPack;
using nrpoints.source.Models;

namespace nrpoints.source.Utils {
    public class FileUtils {

        public static HtmlDocument readInFile(string filePath) {
            var doc = new HtmlDocument();
            doc.Load(filePath);
            return doc;
        }

        public static Race parseHTMLRaceStandings(HtmlDocument document) {
            var h3 = document.DocumentNode.Descendants("h3");
            string track = document.DocumentNode.Descendants("h3").First().InnerText.Trim();

            Race race = new Race(track);

            var tables = document.DocumentNode.SelectNodes("/html/body/table");
            IEnumerable<HtmlNode> tableRows;
            if(tables.Count == 1) {
                tableRows = tables.First().Descendants("tr");
            } else {
                tableRows = tables[1].Descendants("tr");
            }
            foreach (var tableRow in tableRows) {
                HtmlNodeCollection tableData = tableRow.SelectNodes("td");
                if(!tableData[0].InnerText.Trim().Equals("F")) {
                    string name = tableData[3].InnerText.Trim();
                    int finish = Int16.Parse(tableData[0].InnerText);
                    int start = Int16.Parse(tableData[1].InnerText);
                    int number = Int16.Parse(tableData[2].InnerText);
                    int laps = Int16.Parse(tableData[5].InnerText);
                    int led = 0;
                    bool lapsLedLeader = false;
                    if(tableData[6].InnerText.Trim().Contains("*")) {
                        lapsLedLeader = true;
                        led = Int16.Parse(tableData[6].InnerText.Replace("*", ""));
                    } else {
                        led = Int16.Parse(tableData[6].InnerText);
                    }
                    int points = Int16.Parse(tableData[7].InnerText);
                    string status = tableData[8].InnerText.Trim();
                    string interval = tableData[8].InnerText.Trim();
                    race.AddDriver(new SingleRaceDriver(name, interval, status, finish, start, number, laps, led, points, 0, lapsLedLeader));
                }
            }

            return race;

        }
        
    }

}