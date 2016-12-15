using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSM.Models.Admin
{
    public class BosiStatusCollectionModel : ViewModel
    {
        public List<BosiStatusModel> BosiStatuses { get; set; }

        public BosiStatusCollectionModel()
        {
            BosiStatuses = new List<BosiStatusModel>();
        }

        public BosiStatusCollectionModel(List<BosiStatusModel> statuses)
        {
            if(statuses == null)
                statuses = new List<BosiStatusModel>();

            BosiStatuses = statuses;
        }
    }
}