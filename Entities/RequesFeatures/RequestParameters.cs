using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequesFeatures
{
    public abstract class RequestParameters
    {
        const int maxPageSize = 50;

        //Sadece düm düz get set olduğu için buna
        //Auto-implemented property denir
        public int PageNumber { get; set; }

        //Açıklanarak get set oluşturulmuş
        //Full-property denir
        private int _pageSize;

        public int PageSize 
        { 
            get { return _pageSize; }
            set { _pageSize = value > maxPageSize ? maxPageSize : value; } 
        }

        public string? OrderBy { get; set; }

        public string? Field { get; set; }
    }
}
