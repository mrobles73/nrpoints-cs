using nrpoints.source.Utils;

namespace nrpoints.source.Models {

    public class Season {
        public int Year { get; }
        private int _racesRun = 0;
        private bool _ended = false;
        public Series Series { get; }
        private RaceResults? _results;
        private List<String> _raceTracks = new List<String>();
        private List<SeasonDriver> _seasonDriverList = new List<SeasonDriver>();
        private List<List<SeasonDriver>> _prevStandings = new List<List<SeasonDriver>>();

        public Season(int year, Series series) {
            this.Year = year;
            this.Series = series;
        }

        public int getRacesRun() {
            return _racesRun;
        }

        public RaceResults getRaceResults() {
            return _results;
        }

        public List<String> getRaceTracks() {
            return _raceTracks;
        }

        public List<SeasonDriver> getSeasonDriverList() {
            return _seasonDriverList;
        }

        public List<List<SeasonDriver>> getPrevStandings() {
            return _prevStandings;
        }

        public void addRaces(RaceResults results) {
            this._results = results;
            int raceCount = ((Series is Series.CUP && Year > 2003) || Year > 2015) ? NRUtils.getSeriesRaceCount(Series, Year) : 0;
            int postSeasonRaceCutoff = raceCount == 0 ? 0 : Series is Series.CUP ? raceCount-10 : raceCount-7;
            bool hasPostSeason = (Series is Series.CUP && Year > 2003) || Year > 2015;

            results.Races.ForEach(delegate(Race race) {
                if(_ended) return;

                _raceTracks.Add(race.Track);
                _racesRun++;
                if(_racesRun == 1) {
                    race.Results.ForEach(delegate(SingleRaceDriver srDriver) {
                        _seasonDriverList.Add(SeasonDriver.From(srDriver));
                    });
                } else {
                    List<SeasonDriver> wildCardPlayoffsDrivers = new List<SeasonDriver>();
                    race.Results.ForEach(delegate(SingleRaceDriver srDriver) {
                        SeasonDriver? fsDriver = _seasonDriverList.Find(seasonDriver => seasonDriver.Name.Equals(srDriver.Name)); //need to make sure it's returning null when driver isn't there
                        if(fsDriver is null) {
                            _seasonDriverList.Add(SeasonDriver.From(srDriver));
                        } else {
                            if(hasPostSeason && Year > 2013 && _racesRun == raceCount && fsDriver.InPostSeason) {
                                NRUtils.handlePlayoffsFinalFour(srDriver, Year, Series);
                            }

                            fsDriver.Number = srDriver.Number;
                            fsDriver.IncrementRacesRun();
                            fsDriver.IncrementPoints(srDriver.Points);
                            fsDriver.IncrementLapsRun(srDriver.LapsRun);
                            fsDriver.IncrementLapsLed(srDriver.LapsLed);
                            fsDriver.CalcAvgFinish(srDriver.Finish);
                            fsDriver.CalcAvgStart(srDriver.Start);
                            fsDriver.AddPlayoffPoints(srDriver.PlayoffPoints);

                            if(fsDriver.InPostSeason)
                                fsDriver.IncrementSeasonPlayoffPoints(srDriver.Points);
                            if(srDriver.Start == 1)
                                fsDriver.IncrementPoles();
                            if(srDriver.Finish < 6)
                                fsDriver.IncrementT5s();
                            if(srDriver.Finish < 11)
                                fsDriver.IncrementT10s();
                            if(!srDriver.Status.Equals("Running"))
                                fsDriver.IncrementDnfs();
                            if(srDriver.Finish == 1) {
                                fsDriver.IncrementWins();
                                if(hasPostSeason && fsDriver.InPostSeason && _racesRun > postSeasonRaceCutoff) {
                                    fsDriver.IncrementPlayoffWins();
                                }
                            }

                            if(hasPostSeason && _racesRun >= postSeasonRaceCutoff) {
                                if(Series is Series.CUP) {
                                    NRUtils.cupPostSeason(fsDriver, wildCardPlayoffsDrivers, Year, _racesRun);
                                } else {
                                    NRUtils.gnsTrucksPlayoffs(fsDriver, wildCardPlayoffsDrivers, Series, Year, _racesRun, postSeasonRaceCutoff);
                                }            
                            }
                        }
                    });
                    if(Series is Series.CUP && Year > 2016 && _racesRun == postSeasonRaceCutoff) {
                        _seasonDriverList.Sort(NRUtils.CompareDriversByPoints);
                        for(int i=0; i<10; i++) {
                            _seasonDriverList[i].PointsPosition = i+1;
                        }
                    }
                    if(wildCardPlayoffsDrivers.Count > 0) {
                        if(Series is Series.CUP) {
                            NRUtils.setCupWildCardAndPlayoffs(wildCardPlayoffsDrivers, _racesRun, Year);
                        } else {
                            NRUtils.setGnsTrucksPlayoffs(wildCardPlayoffsDrivers, Series, Year, _racesRun, postSeasonRaceCutoff);
                        }
                    }
                }

                _seasonDriverList.Sort(NRUtils.CompareDriversByPoints);
                for(int i=0; i<_seasonDriverList.Count; i++) {
                    SeasonDriver nDriver = _seasonDriverList[i];
                    nDriver.PointsPosition = i+1;
                    nDriver.PointsToLeader = _seasonDriverList[0].Points - nDriver.Points;
                    if(i == 0)
                        nDriver.PointsToNext = 0;
                    else 
                        nDriver.PointsToNext = _seasonDriverList[i-1].Points - nDriver.Points;
                }
                _prevStandings.Add(NRUtils.MakeDeepCopyOfStandings(_seasonDriverList));
                
                if(_racesRun == raceCount) 
                    _ended = true;

            });
        }
        
    }

}