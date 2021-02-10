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

//Grouping

//a) by a column			groupname.Key
//b) by multiple columns	groupname.Key.attribute
//c) by an entity			groupname.Key.attribute

//groups have 2 components
//a) key component (group by);reference this component groupname.Key[.attribute]
//b) data (instances in the group)

//process
//start with a "pile" of data
//specify the grouping attribute(s)
//result is smaller "piles" of data determined by the attributes which can be
//      "reported" upon

//display albums by ReleaseYear
//order by
var resultsorderby = from x in Albums
					  orderby x.ReleaseYear
					  select x;
//resultsorderby.Dump();

//group by ReleaseYear
var resultsgroupby = from x in Albums
					 group x by x.ReleaseYear;
//resultsgroupby.Dump();

//group by Artist name and album ReleaseYear
var resultsgroupbycolumns = from x in Albums
							group x by new {x.Artist.Name, x.ReleaseYear};
//resultsgroupbycolumns.Dump();

//group tracks by their album
var resultsgroupbyentity = from x in Tracks
							group x by x.Album;
//resultsgroupbyentity.Dump();							

//IMPORTANT!!!!!!!!!!!!!!!!!!!!!!!!!
//if you wish to "report" on groups (AFTER the group by)
//    you MUST save the grouping in a temporary dataset
//	  then you MUST use the temmporary dataset to report from

//for query syntax
//your temporary dataset name is created by using ->   into gName

//for method syntax
//your temporary dataset name is the placeholder of your Select -> .Select(gName => ....

//the temporary datasets are created in memory and once the
//	query is completed, the temporary datasets no longer exists.

//group by ReleaseYear
var resultsgroupbyReport = from x in Albums
							group x by x.ReleaseYear into gAlbumYear
							select new
							{
								KeyValue = gAlbumYear.Key,
								numberofAlbums = gAlbumYear.Count(),
								albumandartist = from y in gAlbumYear
												 select new 
												 {
												 	Title = y.Title,
													Year = y.ReleaseYear,
													Artist = y.Artist.Name
												 }
							};
//resultsgroupbyReport.Dump();
							
//group by an entity
var groupAlbumsbyArtist = from x in Albums
							//orderby x.Artist.Name
							where x.ReleaseYear > 1969 && x.ReleaseYear < 1980
							group x by x.Artist into gArtistAlbums
							orderby gArtistAlbums.Key.Name
							where gArtistAlbums.Count() > 1
							select new
							{
								KeyValue = gArtistAlbums.Key.Name,
								numberofAlbums = gArtistAlbums.Count(),
								albumandartist = from y in gArtistAlbums
												 orderby y.ReleaseYear
												 select new 
												 {
												 	Title = y.Title,
													Year = y.ReleaseYear
												 }
							};
groupAlbumsbyArtist.Dump();

//Create a query which will report the employee and their customer base.
//List the employee fullname (phone), number of customer in their base.
//List the fullname, city and state for the customer base.

//how to attack this question
//tips:
//What is the detail of the query? What is reported on most?
//       Cutomers base (big pile of data)
//Is the report one commplete report or is it in smaller components?
//      order by vs group by?
//Can I subdivide (group) my details into specific piles? If so, on what?
//      Employee (smaller piles of data on xxxxxx)
//Is ther an association between Customers and Employees?
//      nav property SupportRep

var groupCustomersOfEmployees = from x in Customers
								group x by x.SupportRep into gTemp
								select new
								{
									Employee = gTemp.Key.LastName + ", " +
												gTemp.Key.FirstName + "(" +
												gTemp.Key.Phone + ")",
									BaseCount = gTemp.Count(),
									CustomerList = from y in gTemp
													select new
													{
														CustName = y.LastName +
														  ", " + y.FirstName,
														City = y.City,
														State = y.State
													}
								};
groupCustomersOfEmployees.Dump();								









