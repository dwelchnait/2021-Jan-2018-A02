<Query Kind="Statements">
  <Connection>
    <ID>dafb55b9-c1f3-4196-b502-b211697f4c21</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

var distinctmq = Customers
				.Select(x => x.Country)
				.Distinct();
//distinctmq.Dump();

var distinctsq = (from x in Customers
				 select x.Country).Distinct();
//distinctsq.Dump();

//Any() and All()

//number of Genres
var GenreCount = Genres.Count();
//GenreCount.Dump();
//Show Genres that have tracks which are not on any playlist
var genreTrackAny = from g in Genres
					where g.Tracks.Any(tr => tr.PlaylistTracks.Count() == 0)
					select g;
//genreTrackAny.Dump();

//show Genres that have all their tracks appearing at least once on a playlist
//what are the popular genres?
//show the genre name, and list of genre tracks and number of playlists
var genreTrackAll = from g in Genres
				    where g.Tracks.All(tr => tr.PlaylistTracks.Count() > 0)
					select new
					{
						name = g.Name,
						thetracks = from y in g.Tracks
									where y.PlaylistTracks.Count() > 0
									select new
									{
										song = y.Name,
										count = y.PlaylistTracks.Count()
									}
					};
//genreTrackAll.Dump();


//comparing the playlists of Roberto Almeida (AlmeidaR) and Michelle Brooks (BrooksM)
//comparing two lists to each other

//obtain a distinct list of all platlist tracks for Roberto
//the .distinct() can destroy the sort of a query snytax, thus we add
//   sort after the .Distinct()
var almeida = (from x in PlaylistTracks
				where x.Playlist.UserName.Contains("AlmeidaR")
				select new
				{
					genre = x.Track.Genre.Name,
					id = x.TrackId,
					song = x.Track.Name,
					artist = x.Track.Album.Artist.Name
				}).Distinct().OrderBy(y => y.song);
//almeida.Dump(); //110

var brooks = (from x in PlaylistTracks
				where x.Playlist.UserName.Contains("BrooksM")
				select new
				{
					genre = x.Track.Genre.Name,
					id = x.TrackId,
					song = x.Track.Name,
					artist = x.Track.Album.Artist.Name
				}).Distinct().OrderBy(y => y.song);
//brooks.Dump(); //88

//list the tracks that both Roberto and Michelle like
//comparing 2 datasets together
//data in listA that is also in listB

var likes = almeida
			.Where(rob => brooks.Any(mic => mic.id == rob.id))
			.OrderBy(rob => rob.song)
			.Select(rob => rob);
//likes.Dump();  //1

//list the tracks that Roberto likes but Michelle does not listen to

var almeidadiffs = almeida
			.Where(rob => !brooks.Any(mic => mic.id == rob.id))
			.OrderBy(rob => rob.song)
			.Select(rob => rob);
//almeidadiffs.Dump(); //109

//list the tracks that Roberto likes but Michelle does not listen to

var brooksdiffsm = brooks
			.Where(mic => almeida.All(rob => mic.id != rob.id))
			.OrderBy(mic => mic.song)
			.Select(mic => mic);
//brooksdiffsm.Dump(); //87

var brooksdiffsq = from mic in brooks
			where almeida.All(rob => mic.id != rob.id)
			orderby mic.song
			select mic;
//brooksdiffsq.Dump(); //87

//using multiple statements to solve a problem is not unusual
//What is really the problem
//   you hvae to do some type of pre-processing to obtain some data
//   and that data is used in the remaining processing

//produce a report (display) wher the track is flag as shorter than average,
//  longer than average or average in play length (milliseconds)

//first what is the average track play time
//THEN on can compare the average play time to each track

//pre-processing to obtain a value needed for the next query
var resultsavg = Tracks
				.Where(tr => tr.Genre.Name.Contains("Rock"))
				.Average(tr => tr.Milliseconds);
//resultsavg.Dump();  //282541

//use the pre-proceed value in another query
var resultsTrackAvgLength = (from x in Tracks
                             where x.Genre.Name.Contains("Rock")
							 select new
							 {
							 	song = x.Name,
								milliseconds = x.Milliseconds,
								length = x.Milliseconds < Tracks
															.Where(tr => tr.Genre.Name.Contains("Rock"))
															.Average(tr => tr.Milliseconds) ? "Shorter" :
								         x.Milliseconds > resultsavg ? "Longer" :
										 "Average"
							 }).OrderBy(x => x.milliseconds);
//resultsTrackAvgLength.Dump();

//Union
//the joining of multiple results into a single query dataset
//syntax (query).Union.(query).Union(query) ....
//rules same as sql
//  number of columns must be the same
//  datatype of columns must be the same
//  ordering should be done as a method() on the unioned dataset

//List the stats of Albums on Tracks (Count, $cost, average track length)
//Note: for cost and average, one will need an instance (track on album) to
//       actually proces the method
//      if an album contains no tracks then no Sum() or Average() can be physical done

//to do this example you will need an Album with no Tracks on your database

var unionresults = (from x in Albums
					where x.Tracks.Count() > 0
                    select new
					{
					   title = x.Title,
					   totalTracks = x.Tracks.Count(),
					   totalPrice = x.Tracks.Sum(tr => tr.UnitPrice),
					   AverageLength = x.Tracks.Average(tr => tr.Milliseconds) / 1000.0
					}).Union(Albums
							.Where(x => x.Tracks.Count() == 0)
							.Select(x => new
								{
								   title = x.Title,
								   totalTracks = 0,
								   totalPrice = 0.00m,
								   AverageLength = 0.0
								})).OrderBy(u => u.totalTracks);
unionresults.Dump();









