using nrpoints.source.Utils;
using nrpoints.source.Services;
using nrpoints.source.Dto;

Console.WriteLine("nrpoints\n");
int YEAR = 2011;
string[] filePaths = new string[36];
for(var i=0; i<36; i++) {
    var num = i+1;
    filePaths[i] = "./html/R" + (i+1) + ".html";
}

SeasonDto season = SeasonManager.CreateSeason(filePaths, YEAR, Series.CUP);
Console.WriteLine(season);

Console.ReadKey();