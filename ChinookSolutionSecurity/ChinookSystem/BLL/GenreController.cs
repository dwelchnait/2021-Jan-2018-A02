﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;   
using ChinookSystem.ViewModels; 
using System.ComponentModel;    
#endregion

namespace ChinookSystem.BLL
{
    [DataObject]
    public class GenreController
    {
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SelectionList> List_GenreNames()
        {
            using (var context = new ChinookSystemContext())
            {
                IEnumerable<SelectionList> results = from x in context.Genres
                                                     orderby x.Name
                                                     select new SelectionList
                                                     {
                                                         ValueField = x.GenreId,
                                                         DisplayField = x.Name
                                                     };
                return results.ToList();
            }
        }
    }
}
