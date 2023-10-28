using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    public class LinkResponse
    {
        public bool HasLink { get; set; }
        public List<Entity> ShapedEntities { get; set; }
        public LinkCollectionWrapper<Entity> LinkedEntitites { get; set; }

        public LinkResponse()
        {
            ShapedEntities = new List<Entity>();
            LinkedEntitites = new LinkCollectionWrapper<Entity>();
        }
    }
}
