namespace nrpoints.source.Models {

    public class Race {
        public string Track { get; }
        public List<SingleRaceDriver> Results { get; }

        public Race(string track) {
            this.Track = track;
            Results = new List<SingleRaceDriver>();
        }

        public void AddDriver(SingleRaceDriver driver) {
            _ = driver ?? throw new ArgumentNullException(nameof(driver));
            Results.Add(driver);
        }

        public override string ToString() {
            string str = "";
            foreach (SingleRaceDriver driver in Results) {
                str += "\n" + driver.ToString();
            }
            return Track + str;
        }
        
        
    }

}