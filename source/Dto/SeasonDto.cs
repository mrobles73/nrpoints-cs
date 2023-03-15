using nrpoints.source.Models;
using nrpoints.source.Utils;

namespace nrpoints.source.Dto {

    public class SeasonDto {

        public int Year { get; }
        public int RaceNumber { get; }
        public Series Series { get; }
        public List<Race> AllRaceResults { get; }
        public List<SeasonDriver> CurrentStandings { get; }
        public List<List<SeasonDriver>> AllStandings { get; }
        public List<String> RaceTracks { get; }

        private SeasonDto(int year, int raceNumber, Series series, List<Race> allRaceResults, List<SeasonDriver> currentStandings, List<List<SeasonDriver>> allStandings, List<String> raceTracks) {
            this.Year = year;
            this.RaceNumber = raceNumber;
            this.Series = series;
            this.AllRaceResults = allRaceResults;
            this.CurrentStandings = currentStandings;
            this.AllStandings = allStandings;
            this.RaceTracks = raceTracks;
        }

        public static SeasonDto From(Season season) {
            return new SeasonDto(
                season.Year, 
                season.getRacesRun(), 
                season.Series,
                season.getRaceResults().Races,
                season.getSeasonDriverList(),
                season.getPrevStandings(),
                season.getRaceTracks()
            );
        }

        public void PrintAllRaceResults() {
            AllRaceResults.ForEach(race => Console.WriteLine(race + "\n"));
        }

        public void PrintCurrentStandings() {
            CurrentStandings.ForEach(driver => Console.WriteLine(driver));
        }

        public void PrintAllStandings() {
            AllStandings.ForEach(standings => {
                standings.ForEach(driver => Console.WriteLine(driver));
                Console.WriteLine();
            });
        }

        public void PrintRaceTracks() {
            RaceTracks.ForEach(track => Console.WriteLine(track));
        }

        public override string ToString() {
            string ret = "Series: " + Series + "\nYear: " + Year + "\nRaces Run: " + RaceNumber + "\nResults:";
            AllRaceResults.ForEach(race => ret += "\n"+race.ToString()+"\n");
            ret += "All Standings:\n";
            AllStandings.ForEach(standings => {
                standings.ForEach(driver => ret += driver.ToString()+"\n");
                ret += "\n";
            });
            return ret;

        }
        
    }

}