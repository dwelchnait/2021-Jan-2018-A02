using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
using ChinookSystem.DAL;
using System.ComponentModel;
using FreeCode.Exceptions;
#endregion

namespace ChinookSystem.BLL
{
    public class PlaylistTracksController
    {
        //class level variable that will hold mutliple strings, each representing
        //      an error message.
        List<Exception> brokenRules = new List<Exception>();

        public List<UserPlaylistTrack> List_TracksForPlaylist(
            string playlistname, string username)
        {
            using (var context = new ChinookSystemContext())
            {
                var results = from x in context.PlaylistTracks
                              where x.Playlist.Name.Equals(playlistname) &&
                                    x.Playlist.UserName.Equals(username)
                              orderby x.TrackNumber
                              select new UserPlaylistTrack
                              {
                                  TrackID = x.TrackId,
                                  TrackNumber = x.TrackNumber,
                                  TrackName = x.Track.Name,
                                  Milliseconds = x.Track.Milliseconds,
                                  UnitPrice = x.Track.UnitPrice
                              };
                return results.ToList();
            }
        }//eom
        public void Add_TrackToPLaylist(string playlistname, string username, int trackid,
                                        string song)
        {
            Playlist playlistExist = null;
            PlaylistTrack playlisttrackExist = null;
            int tracknumber = 0;
            using (var context = new ChinookSystemContext())
            {
                //this class is in what is called the Business Logic Layer
                //Business Logic is the rules of your business
                //Business Logic ensures that rules and data are what is expected
                //Rules:
                //  rule: a track can only exist once on a playlist
                //  rule: playlist names can only be used once for a user, different users
                //        many have the same playlist name
                //  rule: each track on a playlist is assigned a continious
                //        track number
                //
                //The BLL method should also ensure that data exists for
                //      the processing of the transaction
                if (string.IsNullOrEmpty(playlistname))
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<string>("Playlist name is missing. Unable to add track",
                                "Playlist Name", playlistname));
                }
                if(string.IsNullOrEmpty(username))
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<string>("User name is missing. Unable to add track",
                                "User Name", username));
                }
 
                //does the playlist already exist
                playlistExist = (from x in context.Playlists
                                where x.Name.Equals(playlistname) &&
                                        x.UserName.Equals(username)
                                select x).FirstOrDefault();
                if (playlistExist == null)
                {
                    //new playlist
                    //tasks:
                    //  create a new instance of the playlist class
                    //  load the instance with data
                    //  stage the add of the new instance
                    //  set the tracknumber to 1
                    playlistExist = new Playlist()
                                    {
                                        Name = playlistname,
                                        UserName = username
                                    };
                    context.Playlists.Add(playlistExist); //parent is STAGED, in memory
                    tracknumber = 1;
                }
                else
                {
                    //existing playlist
                    //tasks:
                    //  does the track already exist on the playlist? if so, error
                    //  if not, find the highest current tracknumber, increment by 1 (rule)
                    playlisttrackExist = (from x in context.PlaylistTracks
                                          where x.Playlist.Name.Equals(playlistname) &&
                                                  x.Playlist.UserName.Equals(username) &&
                                                  x.TrackId == trackid
                                          select x).FirstOrDefault();
                    if (playlisttrackExist == null)
                    {

                        //track does not exist on the desired playlist
                        tracknumber = (from x in context.PlaylistTracks
                                       where x.Playlist.Name.Equals(playlistname) &&
                                                   x.Playlist.UserName.Equals(username)
                                       select x.TrackNumber).Count();
                        tracknumber++;
                    }
                    else
                    {
                        //business rule broken. track DOES exist already on the desired
                        //  playlist
                        brokenRules.Add(new BusinessRuleException<string>("Track already on playlist",
                               nameof(song), song));
                    }
                }
                //add the track to the playlist in PlaylistTracks
                //create an instance
                playlisttrackExist = new PlaylistTrack();
                //load the instance
                playlisttrackExist.TrackId = trackid;
                playlisttrackExist.TrackNumber = tracknumber;

                //add the instance
                //?????????????
                //What is the playlist id
                //if the playlist exists then we know the id
                //BUT if the playlist is NEW, we DO NOT know the id

                //in one case the id is known BUT in the second case
                //  where the new record is ONLY STAGED, NO primary key
                //  value has been generated yet. <-- problem
                //if you access the new playlist record the pkey would be 0
                //  (default numeric value)

                //the solution to BOTH of these scenarios is to use
                //      navigational properties during the ACTUAL .Add commong
                //      for the new playlisttrack record
                //the entityframework will, on your behave, ensure that the
                //      adding of records to the database will be done in te
                //      appropriate order AND will add the missing compound pkey
                //      value (PlaylistId) to the new playlisttrack record

                //NOT LIKE THIS !!!!!!!! THIS IS WRONG!!!!!!!
                //context.PlaylistTracks.Add(playlisttrackExist);

                //INSTEAD, do the STAGING using the parent.navproperty.Add(xxxx);
                playlistExist.PlaylistTracks.Add(playlisttrackExist);

                //do the commit
                //check to see if ANY business rule exceptions occured
                //
                if (brokenRules.Count() > 0)
                {
                    //at least one error was recorded during the processing of the
                    //  transaction
                    throw new BusinessRuleCollectionException("Add Playlist Track Concerns:", brokenRules);
                }
                else
                {
                    //COMMIT THE TRANSACTION
                    //ALL the staged records will be sent to sql for processing
                    //the transaction is complete
                    //NOTE: a transaction has ONE AND ONLY ONE .SaveChanges()
                    context.SaveChanges();
                }
            }
        }//eom
        public void MoveTrack(MoveTrackItem movetrack)
        {
            int numberoftracks = 0;
            using (var context = new ChinookSystemContext())
            {
                if (string.IsNullOrEmpty(movetrack.PlaylistName))
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<string>("Playlist name is missing. Unable to remove track(s)",
                                "Playlist Name", movetrack.PlaylistName));
                }
                if (string.IsNullOrEmpty(movetrack.UserName))
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<string>("User name is missing. Unable to remove track(s)",
                                "User Name", movetrack.UserName));
                }
                if (movetrack.TrackID <= 0)
                {
                    brokenRules.Add(new BusinessRuleException<int>("Invalid track identifier. Unable to remove track(s)",
                               "Track Identifier", movetrack.TrackID));
                }
                if (movetrack.TrackNumber <= 0)
                {
                    brokenRules.Add(new BusinessRuleException<int>("Invalid track number. Unable to remove track(s)",
                               "Track Number", movetrack.TrackNumber));
                }
                Playlist exist = (from x in context.Playlists
                                 where x.Name.Equals(movetrack.PlaylistName) &&
                                         x.UserName.Equals(movetrack.UserName)
                                 select x).FirstOrDefault();
                if (exist == null)
                {
                    brokenRules.Add(new BusinessRuleException<string>("Playlist does not exist.",
                               nameof(MoveTrackItem.PlaylistName), movetrack.PlaylistName));
                }
                else
                {
                    //due to the way that LInq executes in your program as a "lazy loader"
                    //we need to query directly the number of tracks in the playlist
                    numberoftracks = (context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(movetrack.PlaylistName) && x.Playlist.UserName.Equals(movetrack.UserName))
                                        .Select(x => x)).Count();



                    //check to see if the desired track exists on the database
                    PlaylistTrack trackexist = (from x in context.PlaylistTracks
                                                where x.Playlist.Name.Equals(movetrack.PlaylistName) &&
                                                        x.Playlist.UserName.Equals(movetrack.UserName) &&
                                                        x.TrackId == movetrack.TrackID
                                                select x).FirstOrDefault();
                    if (trackexist == null)
                    {
                        brokenRules.Add(new BusinessRuleException<string>("Playlist track does not exist.",
                              nameof(MoveTrackItem.PlaylistName), movetrack.PlaylistName));
                    }
                    else
                    {
                        //decide the logic depending on direction
                        if (movetrack.Direction.Equals("up"))
                        {
                            //up
                            //not at top
                            if (trackexist.TrackNumber == 1)
                            {
                                brokenRules.Add(new BusinessRuleException<string>("Playlist track already at the top. Refresh your display.",
                              nameof(Track.Name), trackexist.Track.Name));
                            }
                            else
                            {
                                //do the move
                                //get the adjacent track
                                PlaylistTrack othertrack = (from x in context.PlaylistTracks
                                                            where x.Playlist.Name.Equals(movetrack.PlaylistName)
                                                               && x.Playlist.UserName.Equals(movetrack.UserName)
                                                               && x.TrackNumber == trackexist.TrackNumber - 1
                                                            select x).FirstOrDefault();
                                if(othertrack == null)
                                {
                                    brokenRules.Add(new BusinessRuleException<string>("Playlist track to swap seems to be missing. Refresh your display.",
                              nameof(MoveTrackItem.PlaylistName), movetrack.PlaylistName));
                                }
                                else
                                {
                                    //good to swap
                                    //the swap is a matter of changing the tracknumber values
                                    trackexist.TrackNumber -= 1;
                                    othertrack.TrackNumber += 1;

                                    //staging
                                    context.Entry(trackexist).Property(nameof(PlaylistTrack.TrackNumber)).IsModified = true;
                                    context.Entry(othertrack).Property(nameof(PlaylistTrack.TrackNumber)).IsModified = true;
                                }
                            }
                        }
                        else
                        {
                            //down
                            //not at bottom
                            if (trackexist.TrackNumber == numberoftracks)
                            {
                                brokenRules.Add(new BusinessRuleException<string>("Playlist track already at the bottom. Refresh your display.",
                              nameof(Track.Name), trackexist.Track.Name));
                            }
                            else
                            {
                                //do the move
                                //get the adjacent track
                                PlaylistTrack othertrack = (from x in context.PlaylistTracks
                                                            where x.Playlist.Name.Equals(movetrack.PlaylistName)
                                                               && x.Playlist.UserName.Equals(movetrack.UserName)
                                                               && x.TrackNumber == trackexist.TrackNumber + 1
                                                            select x).FirstOrDefault();
                                if (othertrack == null)
                                {
                                    brokenRules.Add(new BusinessRuleException<string>("Playlist track to swap seems to be missing. Refresh your display.",
                              nameof(MoveTrackItem.PlaylistName), movetrack.PlaylistName));
                                }
                                else
                                {
                                    //good to swap
                                    //the swap is a matter of changing the tracknumber values
                                    trackexist.TrackNumber += 1;
                                    othertrack.TrackNumber -= 1;

                                    //staging
                                    context.Entry(trackexist).Property(nameof(PlaylistTrack.TrackNumber)).IsModified = true;
                                    context.Entry(othertrack).Property(nameof(PlaylistTrack.TrackNumber)).IsModified = true;
                                }
                            }
                        }
                    }
                }

                //commit?
                if (brokenRules.Count > 0)
                {
                    throw new BusinessRuleCollectionException("Track Movement Concerns:", brokenRules);
                }
                else
                {
                    context.SaveChanges();
                }

            }
        }//eom


        public void DeleteTracks(string username, string playlistname, List<int> trackstodelete)
        {
            Playlist playlistExist = null;

            int tracknumber = 0;
            using (var context = new ChinookSystemContext())
            {
                if (string.IsNullOrEmpty(playlistname))
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<string>("Playlist name is missing. Unable to remove track(s)",
                                "Playlist Name", playlistname));
                }
                if (string.IsNullOrEmpty(username))
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<string>("User name is missing. Unable to remove track(s)",
                                "User Name", username));
                }
                if (trackstodelete.Count == 0)
                {
                    //there is a data error
                    //setting up an error message
                    brokenRules.Add(new BusinessRuleException<int>("No tracks were selected. Unable to remove track(s)",
                                "Track list count", 0));
                }
                playlistExist = (from x in context.Playlists
                                 where x.Name.Equals(playlistname) &&
                                         x.UserName.Equals(username)
                                 select x).FirstOrDefault();
                if (playlistExist == null)
                {
                    brokenRules.Add(new BusinessRuleException<string>("Playlist does not exist.",
                               nameof(playlistname), playlistname));
                }
                else
                {
                    //list of all tracks that are to be kept
                    var trackskept = context.PlaylistTracks
                                     .Where(x => x.Playlist.Name.Equals(playlistname) &&
                                         x.Playlist.UserName.Equals(username) &&
                                         !trackstodelete.Any(tod => tod == x.TrackId))
                                     .OrderBy(x => x.TrackNumber)
                                     .Select(x => x);

                    //remove the desired tracks
                    PlaylistTrack item = null;
                    foreach (var deleterecord in trackstodelete) //trackids to delete
                    {
                        //getting a single row
                        item = context.PlaylistTracks
                                     .Where(x => x.Playlist.Name.Equals(playlistname) &&
                                         x.Playlist.UserName.Equals(username) &&
                                         x.TrackId == deleterecord)
                                     .Select(x => x).FirstOrDefault();
                        //delete
                        //stage (parent.navproperty.Remove()
                        if (item != null)
                        {
                            playlistExist.PlaylistTracks.Remove(item);
                        }
                    }

                    //re-sequence the kept tracks
                    //option a) use a list and update the records of thelist
                    //option b) delete all children records and re-add only the
                    //              necessary kept records

                    //within this example, you will see how to update specific
                    //  column(s) of a record (option a)
                    tracknumber = 1;
                    foreach(var track in trackskept)
                    {
                        track.TrackNumber = tracknumber;
                        //Stage the update
                        context.Entry(track).Property(nameof(PlaylistTrack.TrackNumber)).IsModified = true;
                        tracknumber++;
                    }
                }

                //commit?
                if (brokenRules.Count > 0)
                {
                    throw new BusinessRuleCollectionException("Track Removal Concerns:", brokenRules);
                }
                else
                {
                    context.SaveChanges();
                }
            }
        }//eom
    }
}
