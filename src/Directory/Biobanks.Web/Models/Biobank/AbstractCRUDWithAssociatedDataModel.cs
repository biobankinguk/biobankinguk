﻿using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Web.Models.Biobank
{
    public abstract class AbstractCRUDWithAssociatedDataModel
    {
        public string? Notes { get; set; }
        public List<AssociatedDataGroupModel> Groups { get; set; }
        public bool AssociatedDataModelsValid ()
        {
            bool allValid = false;
            
            foreach (var group in Groups)
            {
                foreach(var type in group.Types)
                {
                    if (!type.IsValid())
                    {
                        return false;
                    }
                    else if (type.IsValid())
                    {
                        allValid = true;
                    }
                    
                }
            } 
            return allValid;
        }

        #region Reflected Properties

        public IEnumerable<AssociatedDataModel> ListAssociatedDataModels()
        {
            var models = new List<AssociatedDataModel>();
            foreach (var group in Groups)
            {
                foreach (var x in group.Types)
                {
                    models.Add(x);
                } 
            }
            return models;
        }



        #endregion
    }
}