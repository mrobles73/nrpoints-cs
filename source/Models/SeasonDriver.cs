namespace nrpoints.source.Models {
    public class SeasonDriver : Driver {

        public int SeasonPlayoffPoints { get; set; }
        public int PointsPosition { get; set; }
        public int RacesRun { get; private set; }
        public int PointsToLeader { get; set; }
        public int PointsToNext { get; set; }
        public int Poles { get; private set; }
        public int Wins { get; private set; }
        public int PlayoffWins { get; set; }
        public int T5s { get; private set; }
        public int T10s { get; private set; }
        public int Dnfs { get; private set; }
        public double AvgStart { get; private set; }
        public double AvgFinish { get; private set; }
        public bool InPostSeason { get; set; }

        private readonly string DecimalFormat = "{0:0.00}";

        private SeasonDriver(string name, int number, int points, int playoffPoints, int seasonPlayoffPoints, int pointsPosition, int racesRun, int lapsRun, int lapsLed, int pointsToLeader, int pointsToNext, int poles, int wins, int playoffWins, int t5s, int t10s, int dnfs, double avgStart, double avgFinish, bool inPostSeason) {
            this.Name = name;
            this.Number = number;
            this.Points = points;
            this.PlayoffPoints = playoffPoints;
            this.SeasonPlayoffPoints = seasonPlayoffPoints;
            this.PointsPosition = pointsPosition;
            this.RacesRun = racesRun;
            this.LapsRun = lapsRun;
            this.LapsLed = lapsLed;
            this.PointsToLeader = pointsToLeader;
            this.PointsToNext = pointsToNext;
            this.Poles = poles;
            this.Wins = wins;
            this.PlayoffWins = playoffWins;
            this.T5s = t5s;
            this.T10s = t10s;
            this.Dnfs = dnfs;
            this.AvgStart = avgStart;
            this.AvgFinish = avgFinish;
            this.InPostSeason = inPostSeason;

        }


        public static SeasonDriver From(SingleRaceDriver driver) {
            int poles = driver.Start == 1 ? 1 : 0;
            int wins = driver.Finish == 1 ? 1 : 0;
            int t5 = driver.Finish < 6 ? 1 : 0;
            int t10 = driver.Finish < 11 ? 1 : 0;
            int dnf = driver.Status.Equals("Running") ? 0:1;
            return new SeasonDriver(
                driver.Name,
                driver.Number,
                driver.Points,
                driver.PlayoffPoints,
                0,
                0,
                1,
                driver.LapsRun,
                driver.LapsLed,
                0,
                0,
                poles,
                wins,
                0,
                t5,
                t10,
                dnf,
                driver.Start,
                driver.Finish,
                false
            );
        }

        public static SeasonDriver From(SeasonDriver driver) {
            return new SeasonDriver(
                driver.Name,
                driver.Number,
                driver.Points,
                driver.PlayoffPoints,
                driver.SeasonPlayoffPoints,
                driver.PointsPosition,
                driver.RacesRun,
                driver.LapsRun,
                driver.LapsLed,
                driver.PointsToLeader,
                driver.PointsToNext,
                driver.Poles,
                driver.Wins,
                driver.PlayoffWins,
                driver.T5s,
                driver.T10s,
                driver.Dnfs,
                driver.AvgStart,
                driver.AvgFinish,
                driver.InPostSeason
            );
        }

        public void IncrementRacesRun() {
            RacesRun++;
        }

        public void IncrementPoints(int points) {
            this.Points += points;
        }

        public void IncrementLapsRun(int lapsRun) {
            this.LapsRun += lapsRun;
        }

        public void IncrementLapsLed(int lapsLed) {
            this.LapsLed += lapsLed;
        }

        public void CalcAvgFinish(int finish) {
            AvgFinish = ((AvgFinish * (RacesRun-1)) + finish) / RacesRun;
        }

        public void CalcAvgStart(int start) {
            AvgStart = ((AvgStart * (RacesRun-1)) + start) / RacesRun;
        }

        public void AddPlayoffPoints(int points) {
            PlayoffPoints += points;
        }

        public void IncrementSeasonPlayoffPoints(int points) {
            SeasonPlayoffPoints += points;
        }

        public void IncrementPoles() {
            Poles++;
        }

        public void IncrementT5s() {
            T5s++;
        }

        public void IncrementT10s() {
            T10s++;
        }

        public void IncrementDnfs() {
            Dnfs++;
        }

        public void IncrementWins() {
            Wins++;
        }

        public void IncrementPlayoffWins() {
            PlayoffWins++;
        }

        public override string ToString() {
            return PointsPosition + " | #" + Number + " | Driver: " + Name + " | Points: " + Points + " | Next: " + PointsToNext + " | Leader: " + PointsToLeader + " | Starts: " + RacesRun + " | Poles: " + Poles + " | Wins: " + Wins + " | T5: " + T5s + " | T10: " + T10s + " | DNF: " + Dnfs + " | Laps Completed: " + LapsRun + " | Laps Led: " + LapsLed + " | Average Start: " + String.Format(DecimalFormat, AvgStart) + " | Average Finish: " + String.Format(DecimalFormat, AvgFinish);
        }

    }
}