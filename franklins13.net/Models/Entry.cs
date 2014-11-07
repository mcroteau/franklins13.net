using IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace franklins13.net.Models
{
    public class Entry
    {
        [Key]
        public int Id { get; set; }

        [DefaultValue(0)]
        public int Temperance { get; set; }

        [DefaultValue(0)]
        public int Silence { get; set; }

        [DefaultValue(0)]
        public int Order { get; set; }

        [DefaultValue(0)]
        public int Resolution { get; set; }

        [DefaultValue(0)]
        public int Frugality { get; set; }

        [DefaultValue(0)]
        public int Industry { get; set; }

        [DefaultValue(0)]
        public int Sincerity { get; set; }

        [DefaultValue(0)]
        public int Justice { get; set; }

        [DefaultValue(0)]
        public int Moderation { get; set; }

        [DefaultValue(0)]
        public int Cleanliness { get; set; }

        [DefaultValue(0)]
        public int Tranquility { get; set; }

        [DefaultValue(0)]
        public int Chastity { get; set; }

        [DefaultValue(0)]
        public int Humility { get; set; }


        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}