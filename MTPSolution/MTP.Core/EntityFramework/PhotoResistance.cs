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

        public double LightRatio { get; set; }

        public DateTime CollectionTime { get; set; }

        [StringLength(25)]
        public string DeviceId { get; set; }
    }
}
