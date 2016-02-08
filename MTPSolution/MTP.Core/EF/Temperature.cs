namespace MTP.Core.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Temperature
    {
        public int Id { get; set; }

        [Column("Temperature")]
        public double Temperature1 { get; set; }

        public DateTime CollectionTime { get; set; }

        [Required]
        [StringLength(50)]
        public string DeviceId { get; set; }

        [Required]
        [StringLength(10)]
        public string SensorId { get; set; }
    }
}
