namespace MTP.Core.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LightVal
    {
        public int Id { get; set; }

        public double Light { get; set; }

        public DateTime CollectionTime { get; set; }

        [Required]
        [StringLength(50)]
        public string DeviceId { get; set; }

        [Required]
        [StringLength(10)]
        public string SensorId { get; set; }
    }
}
