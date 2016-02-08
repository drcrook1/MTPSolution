namespace MTP.Core.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TelemetryDB : DbContext
    {
        public TelemetryDB()
            : base("name=TelemetryDB1")
        {
        }

        public virtual DbSet<LightVal> LightVals { get; set; }
        public virtual DbSet<Temperature> Temperatures { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
