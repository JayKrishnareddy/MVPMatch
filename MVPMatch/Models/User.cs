using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVPMatch.Models
{
    public class User
    {
        [Key]
        [JsonIgnore]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public long DespositAmount { get; set; }
        public string Role { get; set; }
    }
}
