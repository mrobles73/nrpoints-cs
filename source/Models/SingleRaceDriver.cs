namespace nrpoints.source.Models {

    public class SingleRaceDriver : Driver {
        public string Interval { get; }
        public string Status { get; }
        public int Finish { get; }
        public int Start { get; }
        public bool LapsLedLeader { get; }

        public SingleRaceDriver(string name, string interval, string status, int finish, int start, int number, int lapsRun, int lapsLed, int points, int playoffPoints, bool lapsLedLeader) {
            this.Name = name;
            this.Interval = interval;
            this.Status = status;
            this.Finish = finish;
            this.Start = start;
            this.Number = number;
            this.LapsRun = lapsRun;
            this.LapsLed = lapsLed;
            this.Points = points;
            this.PlayoffPoints = playoffPoints;
            this.LapsLedLeader = lapsLedLeader;
        }

        public override string ToString() {
            return "F: " + Finish + " | S: " + Start + " | #" + Number + " | Name: " + Name + " | Interval: " + Interval + " | Laps Run: " + LapsRun + " | Laps Led: " + LapsLed + " | Points: " + Points + " | Status: " + Status + " | Led Most Laps:" + LapsLedLeader;
        }
        
    }

}