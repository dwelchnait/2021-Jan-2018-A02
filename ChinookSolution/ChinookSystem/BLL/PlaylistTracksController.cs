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
                if(brokenRules.Count() == 0)
                {
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
                        context.Playlists.Add(playlistExist);
                        tracknumber = 1;
                    }
                    else
                    {
                        //existing playlist
                        //tasks:
                        //  does the track already exist on the playlist? if so, error
                        //  if not, find the highest current tracknumber, increment by 1 (rule)

                    }

                }
                else
                {

                }
            }
        }//eom
        public void MoveTrack(string username, string playlistname, int trackid, int tracknumber, string direction)
        {
            using (var context = new ChinookSystemContext())
            {
                //code to go here 

            }
        }//eom


        public void DeleteTracks(string username, string playlistname, List<int> trackstodelete)
        {
            using (var context = new ChinookSystemContext())
            {
               //code to go here


            }
        }//eom
    }
}
