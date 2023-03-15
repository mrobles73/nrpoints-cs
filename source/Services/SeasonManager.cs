using nrpoints.source.Models;
using nrpoints.source.Utils;
using nrpoints.source.Dto;
using HtmlAgilityPack;

namespace nrpoints.source.Services {

    public class SeasonManager {

        public static SeasonDto CreateSeason(string[] filePaths, int year, Series series) {
            RaceResults raceResults = new RaceResults(year, series);
            foreach (string path in filePaths) {
                HtmlDocument document = FileUtils.readInFile(path);
                Race race = FileUtils.parseHTMLRaceStandings(document);
                raceResults.AddRace(race);
            }
            NRUtils.setResultsPoints(raceResults);
            Season newSeason = new Season(year, series);
            newSeason.addRaces(raceResults);
            SeasonDto seasonResult = SeasonDto.From(newSeason);
            return seasonResult;            
        }
        
    }

}