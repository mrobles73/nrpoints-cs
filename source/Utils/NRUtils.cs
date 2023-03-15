using HtmlAgilityPack;
using nrpoints.source.Models;

namespace nrpoints.source.Utils {
    public class NRUtils {

        private static readonly int WINNER_0406 = 180;
        private static readonly int WINNER_0710 = 185;
        private static readonly int[] TOP_10_RESET_0406 = new int[]{5050, 5045, 5040, 5035, 5030, 5025, 5020, 5015, 5010, 5005};
        private static readonly int[] TOP_10_POINTS_17 = new int[]{0, 15, 10, 9, 8, 7, 6, 5, 4, 3, 2};
        private static readonly int[] POINTS_75_10 = new int[]{0, 175, 170, 165, 160, 155, 150, 146, 142, 138, 134, 130, 127, 124, 121, 118, 115, 112, 109, 106, 103, 100, 97, 94, 91, 88, 85, 82, 79, 76, 73, 70, 67, 64, 61, 58, 55, 52, 49, 46, 43, 40, 37, 34};
        public static readonly int[] POINTS_11_15 = new int[]{0, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1};
        public static readonly int[] POINTS_16 = new int[]{0, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0}; //maybe change extended to 1 instead of 0
        public static readonly int[] POINTS_16_TRUCKS = new int[]{0, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static readonly int[] POINTS_17 = new int[]{0, 40, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 0, 0, 0};
        public static readonly int[] POINTS_17_TRUCKS = new int[]{0, 40, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0};
        
        private static readonly Dictionary<Series, Int16> BASE_RACE_COUNT = new Dictionary<Series, Int16>();
        private static readonly Dictionary<Series, Int16> PLAYOFF_CUTOFF = new Dictionary<Series, Int16>();

        public static readonly string DecimalFormat = "{0:0.00}";

        static NRUtils() {
            BASE_RACE_COUNT.Add(Series.CUP, 36);
            BASE_RACE_COUNT.Add(Series.GNS, 33);
            BASE_RACE_COUNT.Add(Series.TRUCKS, 23);
            PLAYOFF_CUTOFF.Add(Series.CUP, 17);
            PLAYOFF_CUTOFF.Add(Series.GNS, 13);
            PLAYOFF_CUTOFF.Add(Series.TRUCKS, 11);
        }


        public static void setResultsPoints(RaceResults raceResults) {
            int year = raceResults.Year;
            raceResults.Races.ForEach(delegate(Race race) {
                race.Results.ForEach(delegate(SingleRaceDriver srDriver) {
                    srDriver.Points = calculateDriverPoints(year, srDriver.Finish, srDriver.LapsLed, srDriver.LapsLedLeader, raceResults.Series);
                });
            });
        }

        public static int calculateDriverPoints(int year, int finish, int led, bool lapsLedLeader, Series series) {
            int points = 0;
            if(year > 1974 && year < 2011) {
                points = POINTS_75_10[finish];
                if(finish == 1) {
                    if((series is Series.GNS && year < 1998) || (series is Series.TRUCKS && year < 1999)) {
                        return 180;
                    }
                    if(year > 2003 && year < 2007) {
                        points = WINNER_0406;
                    } else if(year > 2006) {
                        points = WINNER_0710;
                    }
                }
                if(led > 0) {
                    points += 5;
                }
                if(lapsLedLeader) {
                    points += 5;
                }
            } else if(year > 2010 && year < 2017) {
                int win = 3;
                int lapLed = 1;
                int mLapsLed = 1;
                if(year < 2016) {
                    points = POINTS_11_15[finish];
                } else if(series is Series.TRUCKS){
                    points = POINTS_16_TRUCKS[finish];
                } else {
                    points = POINTS_16[finish];
                }
                if(finish == 1) points += win;
                if(led > 0) points += lapLed;
                if(lapsLedLeader) points += mLapsLed;
            } else {
                if(series is Series.TRUCKS) {
                    points = POINTS_17_TRUCKS[finish];
                } else {
                    points = POINTS_17[finish];
                }
                if(finish == 1) {
                    points += 5;
                }
            }
            return points;
        }

        public static int getSeriesRaceCount(Series series, int year) {
            if(series is Series.TRUCKS && year == 2021)
                return 22;
            
            return BASE_RACE_COUNT[series];
        }

        internal static void handlePlayoffsFinalFour(SingleRaceDriver srDriver, int year, Series series) {
            if(year < 2016) {
                srDriver.Points = POINTS_11_15[srDriver.Finish];
            } else if(year == 2016) {
                if(series is Series.TRUCKS) {
                    srDriver.Points = POINTS_16_TRUCKS[srDriver.Finish];
                } else {
                    srDriver.Points = POINTS_16[srDriver.Finish];
                }
            } else {
                if(series is Series.TRUCKS) {
                    srDriver.Points = POINTS_17_TRUCKS[srDriver.Finish];
                } else {
                    srDriver.Points = POINTS_17[srDriver.Finish];
                }
            }
        }

        internal static void cupPostSeason(SeasonDriver fsDriver, List<SeasonDriver> wildCardPlayoffsDrivers, int year, int racesRun) {
            if(year > 2003 && racesRun == 26) {
                if(year < 2007 && fsDriver.PointsPosition < 11) {
                    fsDriver.InPostSeason = true;
                    fsDriver.Points = TOP_10_RESET_0406[fsDriver.PointsPosition-1];
                } else if(year > 2006 && year < 2011 && fsDriver.PointsPosition < 13) {
                    fsDriver.InPostSeason = true;
                    fsDriver.Points = (fsDriver.Wins * 5) + 5000;
                } else if(year > 2010 && year < 2014) {
                    if(fsDriver.PointsPosition < 11) {
                        fsDriver.InPostSeason = true;
                        fsDriver.Points = (fsDriver.Wins * 3) + 2000;
                    } else if(fsDriver.PointsPosition < 21) {
                        wildCardPlayoffsDrivers.Add(fsDriver);
                    }
                } else if(year > 2013) {
                    if(fsDriver.PointsPosition < 17) {
                        wildCardPlayoffsDrivers.Add(fsDriver);
                    } else if(fsDriver.PointsPosition < 31 && fsDriver.RacesRun == 26 && fsDriver.Wins > 0) {
                        wildCardPlayoffsDrivers.Add(fsDriver);
                    }
                }
            } else if(year > 2013 && (racesRun == 29 || racesRun == 32 || racesRun == 35)) {
                int cutoff = 13;
                if(racesRun == 32) {
                    cutoff = 9;
                }
                if(racesRun == 35) {
                    cutoff = 5;
                }
                if(fsDriver.PointsPosition < cutoff || (fsDriver.InPostSeason && fsDriver.PlayoffWins > 0)) {
                    wildCardPlayoffsDrivers.Add(fsDriver);
                } else if(fsDriver.InPostSeason) {
                    fsDriver.InPostSeason = false;
                    fsDriver.Points = fsDriver.SeasonPlayoffPoints;
                }

            }
        }

        internal static void gnsTrucksPlayoffs(SeasonDriver fsDriver, List<SeasonDriver> wildCardPlayoffsDrivers, Series series, int year, int racesRun, int postSeasonRaceCutoff) {
            bool condition = series is Series.TRUCKS && year < 2020;
            if(year > 2015 && racesRun == postSeasonRaceCutoff) {
                int pStartCutoff = condition ? 9 : PLAYOFF_CUTOFF[series];
                if((fsDriver.PointsPosition < pStartCutoff) || (fsDriver.PointsPosition < 21 && fsDriver.RacesRun == postSeasonRaceCutoff && fsDriver.Wins > 0)) {
                    wildCardPlayoffsDrivers.Add(fsDriver);                    
                }
            } else if(year > 2015 && (racesRun == (postSeasonRaceCutoff+3) || racesRun == (postSeasonRaceCutoff+6))) {
                int cutoff = condition ? 7 : 9;
                if(racesRun == (postSeasonRaceCutoff+6)) {
                    cutoff = 5;
                }
                if(fsDriver.PointsPosition < cutoff || (fsDriver.InPostSeason && fsDriver.PlayoffWins > 0)) {
                    wildCardPlayoffsDrivers.Add(fsDriver);
                } else if(fsDriver.InPostSeason) {
                    fsDriver.InPostSeason = false;
                    fsDriver.Points = fsDriver.SeasonPlayoffPoints;
                }
            }
        }

        internal static void setCupWildCardAndPlayoffs(List<SeasonDriver> wildCardPlayoffsDrivers, int racesRun, int year) {
            int resetPoints = 3000;
            int cutoff = 12;
            if(racesRun == 26) {
                wildCardPlayoffsDrivers.Sort(CompareDriversByWinsThenPoints);
            } else if(racesRun == 29 || racesRun == 32 || racesRun == 35) {
                wildCardPlayoffsDrivers.Sort(CompareDriversByPlayoffWinsThenPoints);
                if(racesRun == 32) {
                    resetPoints = 4000;
                    cutoff = 8;
                } else if(racesRun == 35) {
                    resetPoints = 5000;
                    cutoff = 4;
                }
            }
            if(year < 2014) {
                wildCardPlayoffsDrivers[0].InPostSeason = true;
                wildCardPlayoffsDrivers[0].Points = 2000;
                wildCardPlayoffsDrivers[1].InPostSeason = true;
                wildCardPlayoffsDrivers[1].Points = 2000;
            } else if(racesRun == 26) {
                InitialCutoffLoop(16, year, wildCardPlayoffsDrivers, true);
            } else if(racesRun != 36) {
                SubsequentCutoffLoops(cutoff, year, resetPoints, wildCardPlayoffsDrivers);
            }
        }

        internal static void setGnsTrucksPlayoffs(List<SeasonDriver> wildCardPlayoffsDrivers, Series series, int year, int racesRun, int postSeasonRaceCutoff) {
            if(racesRun == postSeasonRaceCutoff) {
                wildCardPlayoffsDrivers.Sort(CompareDriversByWinsThenPoints);
                int pStartCutoff = series is Series.TRUCKS && year < 2020 ? 9 : PLAYOFF_CUTOFF[series];
                InitialCutoffLoop(pStartCutoff-1, year, wildCardPlayoffsDrivers, false);
                wildCardPlayoffsDrivers.ForEach(driver => { //??
                    driver.InPostSeason = true;
                    if(year < 2017) {
                        driver.Points = (driver.Wins*3) + 2000;
                    }
                });
            } else if(racesRun == (postSeasonRaceCutoff+3) || racesRun == (postSeasonRaceCutoff+6)) {
                wildCardPlayoffsDrivers.Sort(CompareDriversByPlayoffWinsThenPoints);

                int resetPoints = 3000;
                int cutoff = series is Series.TRUCKS && year < 2020 ? 6 : 8;
                if(racesRun == (postSeasonRaceCutoff+6)) {
                    resetPoints = 4000;
                    cutoff = 4;
                }
                SubsequentCutoffLoops(cutoff, year, resetPoints, wildCardPlayoffsDrivers);
            }
        }

        private static void InitialCutoffLoop(int cutoff, int year, List<SeasonDriver> wildCardPlayoffsDrivers, bool cup) {
            for(int i=0; i<cutoff; i++) {
                SeasonDriver driver = wildCardPlayoffsDrivers[i];
                driver.InPostSeason = true;
                if(year < 2017) {
                    driver.Points = (driver.Wins*3) + 2000;
                } else {
                    int pointsFinish = driver.PointsPosition < 11 ? driver.PointsPosition : 0;
                    driver.AddPlayoffPoints(TOP_10_POINTS_17[pointsFinish]);
                    driver.Points = driver.PlayoffPoints + 2000;
                }
                if(cup) {
                    driver.SeasonPlayoffPoints = driver.Points;
                }
            }
        }

        private static void SubsequentCutoffLoops(int cutoff, int year, int resetPoints, List<SeasonDriver> wildCardPlayoffsDrivers) {
            for(int i=0; i<cutoff; i++) {
                SeasonDriver driver = wildCardPlayoffsDrivers[i];
                driver.PlayoffWins = 0;
                if(year < 2017) {
                    driver.Points = resetPoints;
                } else {
                    driver.Points = resetPoints + driver.PlayoffPoints;
                }
            }
            for(int i=cutoff; i<wildCardPlayoffsDrivers.Count; i++) {
                SeasonDriver driver = wildCardPlayoffsDrivers[i];
                driver.InPostSeason = false;
                driver.Points = driver.SeasonPlayoffPoints;
            }
        }

        internal static List<SeasonDriver> MakeDeepCopyOfStandings(List<SeasonDriver> SeasonDriverList) {
            List<SeasonDriver> newList = new List<SeasonDriver>();
            SeasonDriverList.ForEach(driver => newList.Add(SeasonDriver.From(driver)));
            return newList;
        }


        //Comparison
        public static int CompareDriversByPoints(SeasonDriver driverOne, SeasonDriver driverTwo) {
            int ret = driverTwo.Points.CompareTo(driverOne.Points);
            if(ret == 0)
                ret = driverTwo.Wins.CompareTo(driverOne.Wins);
            if(ret == 0)
                ret = CompareDriversByFinish(driverOne, driverTwo);

            return ret;
        }

        private static int CompareDriversByWinsThenPoints(SeasonDriver driverOne, SeasonDriver driverTwo) {
            int ret = driverTwo.Wins.CompareTo(driverOne.Wins);
            if(ret == 0)
                ret = driverTwo.Points.CompareTo(driverOne.Points);
            if(ret == 0)
                ret = CompareDriversByFinish(driverOne, driverTwo);

            return ret;
        }

        private static int CompareDriversByPlayoffWinsThenPoints(SeasonDriver driverOne, SeasonDriver driverTwo) {
            int ret = driverTwo.PlayoffWins.CompareTo(driverOne.PlayoffWins);
            if(ret == 0)
                ret = driverTwo.Points.CompareTo(driverOne.Points);
            if(ret == 0)
                ret = CompareDriversByFinish(driverOne, driverTwo);

            return ret;
        }

        private static int CompareDriversByFinish(SeasonDriver driverOne, SeasonDriver driverTwo) {
            int ret = driverOne.AvgFinish.CompareTo(driverTwo.AvgFinish);
            if(ret == 0)
                ret = (driverTwo.T5s != driverOne.T5s) ? driverTwo.T5s.CompareTo(driverOne.T5s) : driverTwo.T10s.CompareTo(driverOne.T10s);

            return ret;
        }

        
    }
}