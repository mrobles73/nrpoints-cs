using nrpoints.source.Utils;

namespace nrpoints.source.Models
{

    public class RaceResults {
        public int Year { get; }
        public Series Series { get; }  //might use a string
        public List<Race> Races { get; }

        public RaceResults(int year, Series series) {
            this.Year = year;
            this.Series = series;
            Races = new List<Race>();
        }

        public void AddRace(Race race) {
            _ = race ?? throw new ArgumentNullException(nameof(race));
            Races.Add(race);
        }
        
        
    }

}