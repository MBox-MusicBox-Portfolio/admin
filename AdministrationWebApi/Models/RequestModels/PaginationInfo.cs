using AdministrationWebApi.Models.Db;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace AdministrationWebApi.Models.RequestModels
{
    public class PaginationInfo
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageIndex must be greater than 0.")]
        public int Page { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PageIndex must be greater than 0.")]
        public int Size { get; set; }
    }
}
