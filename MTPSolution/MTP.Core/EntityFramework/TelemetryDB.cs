//namespace MTP.Core.EntityFramework
//{
//    using System;
//    using System.Data.Entity;
//    using System.ComponentModel.DataAnnotations.Schema;
//    using System.Linq;

//    public partial class TelemetryDB : DbContext
//    {
//        public TelemetryDB()
//            : base("name=TelemetryDB")
//        {
//        }

//        public virtual DbSet<PhotoResistance> PhotoResistances { get; set; }
//        public virtual DbSet<Temperature> Temperatures { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<PhotoResistance>()
//                .Property(e => e.Location)
//                .IsFixedLength();

//            modelBuilder.Entity<PhotoResistance>()
//                .Property(e => e.DeviceId)
//                .IsFixedLength();

//            modelBuilder.Entity<Temperature>()
//                .Property(e => e.Location)
//                .IsFixedLength();

//            modelBuilder.Entity<Temperature>()
//                .Property(e => e.DeviceId)
//                .IsFixedLength();
//        }
//    }
//}
