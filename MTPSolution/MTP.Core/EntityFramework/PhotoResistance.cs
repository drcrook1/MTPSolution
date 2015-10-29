namespace MTP.Core.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PhotoResistance
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Location { get; set; }

        [Column("PhotoResistance")]
        public int PhotoResistance1 { get; set; }

        public DateTime? CollectionTime { get; set; }
    }
}
