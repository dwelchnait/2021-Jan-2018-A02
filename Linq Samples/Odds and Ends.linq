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
genreTrackAll.Dump();












