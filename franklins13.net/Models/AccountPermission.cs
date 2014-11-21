using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using IdentitySample.Models;
using System.ComponentModel.DataAnnotations;


namespace franklins13.net.Models
{
    public class AccountPermission
    {
        [Key]
        public int Id { get; set; }

        public string Permission { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}